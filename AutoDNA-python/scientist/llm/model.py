import os
from langchain_openai import ChatOpenAI
from langchain_google_genai import ChatGoogleGenerativeAI


model_tag = "gemini-2.5-pro"
model = ChatGoogleGenerativeAI(
    model="gemini-2.5-pro",
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)


react_model = ChatGoogleGenerativeAI(
    model="gemini-2.5-pro",
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)


paper_qa_model = "gemini/gemini-2.5-flash"

paper_qa_key = os.getenv("GEMINI_API_KEY")

coder_model_tag = "gemini-2.5-pro"
code_model = ChatGoogleGenerativeAI(
    model=coder_model_tag,
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)

plan_model_tag = "gemini-2.5-pro"
plan_model = ChatGoogleGenerativeAI(
    model=plan_model_tag,
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)

reasoner_model_tag = "gemini-2.5-pro"
reasoner_model = ChatGoogleGenerativeAI(
    model=reasoner_model_tag,
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)

pharmacy_model_tag = "gemini-2.5-pro"
pharmacy_model = ChatGoogleGenerativeAI(
    model=pharmacy_model_tag,
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)

hypothesis_model_tag = "gemini-2.5-pro"
hypothesis_model = ChatGoogleGenerativeAI(
    model=hypothesis_model_tag,
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)

workflow_model_tag = "gemini-2.5-pro"
workflow_model = ChatGoogleGenerativeAI(
    model=workflow_model_tag,
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)



corrector_model_tag = "gemini-2.5-pro"
corrector_model = ChatGoogleGenerativeAI(
    model=corrector_model_tag,
    api_key=os.getenv("GEMINI_API_KEY"),
    temperature=1.0,
)