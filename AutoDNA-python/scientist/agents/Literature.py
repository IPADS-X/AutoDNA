import traceback
import os
import sys

MODIFIED_PAPER_QA_PATH = "../Lib/paper-qa"
sys.path.insert(0, MODIFIED_PAPER_QA_PATH) 

from paperqa import Settings, ask
from paperqa.settings import AgentSettings
from llm.model import paper_qa_model, paper_qa_key
from config import output_path, paper_path, qa_prompt_suffix, qa_prompt_option, input_path, settings
from langchain.tools import tool
from dotenv import load_dotenv
from langchain.prompts import PromptTemplate
from tools.utils import static_init, AnswerCache
from tools.file_manager import file_manager
from loguru import logger
from agents.constants import AgentOutputPrefix, AgentName


cache_name = f"{AgentOutputPrefix.LRA_OUTPUT}_cache.json"


def answer_one_question(question, suffix=None, mock=False, save=False, filter=None):
    if mock is False:
        if suffix is None:
            question += qa_prompt_suffix + qa_prompt_option 
        else:
            question += suffix
        if filter is not None:
            question += filter
    else:
        with open(os.path.join(input_path, "temp.md"), "r", encoding='utf-8') as f:
            question = f.read()

    is_DNA_storage = (settings.write or settings.read)
    
    model = paper_qa_model
    # set environment variable GEMINI_API_KEY
    os.environ["GEMINI_API_KEY"] = paper_qa_key
    paper_settings = Settings(
        llm=model,
        summary_llm=model,
        agent=AgentSettings(
            agent_llm=model,
            index_concurrency=3),
        paper_directory=os.path.join(paper_path, "paper400" if is_DNA_storage else "final"),
        use_absolute_paper_directory=False,
        embedding="gemini/text-embedding-004",  
        embedding_config={"trust_remote_code": True},
        verbosity=3,
    )
    paper_settings.answer.answer_max_sources = 30
    paper_settings.answer.evidence_k = 50
    paper_settings.agent.search_count = 16
    paper_settings.parsing.chunk_size = 8000
    paper_settings.parsing.overlap = 700
    paper_settings.agent.return_paper_metadata = False
    #paper_settings.texts_index_mmr_lambda = 0.95
    #paper_settings.answer.evidence_skip_summary = True
    paper_settings.answer.answer_length = """
    YOUR ANSWER MUST BE STRICTLY IDENTICAL AS WHAT IS IN THE PAPER, WITH ACCURATE DETAILS.
    """
    try:
        answer_response = ask(
            query=question,
            settings=paper_settings,
        )
        if save:
            AnswerCache.push_question_and_answer(question, answer_response.session.answer)
    except Exception as e:
        # print Exception
        logger.error(f"An error occurred: {e}")
        # print complete trace
        traceback.print_exc()
        
    return answer_response.session.answer


@tool
def Literature(questions: list[str], experiment_name: str) -> str:
    """Searches scientific literature and documents to answer specific questions and collects information about an experiment. 

    This agent performs targeted information retrieval from a local database of scientific papers and manuals. 
    Args:
        questions (list[str]): A list of specific questions to ask the literature database. Frame them clearly with questions starting with "What" or "How".
        experiment_name (str): The name or high-level goal of the experiment with related user requirements.
    Returns:
        str: A compiled answer to the questions. """

    # check if cached 
    cache_result = file_manager.load_from_cache_kv(cache_name, experiment_name)
    if cache_result:
        logger.info(f"Cache hit for experiment '{experiment_name}'")
        return cache_result

    filter_template = PromptTemplate.from_template(
        """
        The focus is on {experiment_name}, remove irrelevant information.
        """
    )
    filter = filter_template.format(experiment_name=experiment_name)    
    full_question = ""
    for question in questions:
        full_question += question + " "

    reply = answer_one_question(full_question, save=True, filter=filter)

    # The result saved in the shared_files 
    file_id = file_manager.add_file(
        agent_name="Literature",
        content=reply,
        original_filename="last_answer.md",
        description="Gathered infos from question: " + full_question
    )
    EOF_line = "\n The information has been saved to the file: " + file_id 

    # save to cache
    file_manager.save_to_cache_kv(cache_name, experiment_name, reply + EOF_line)

    return reply + EOF_line    