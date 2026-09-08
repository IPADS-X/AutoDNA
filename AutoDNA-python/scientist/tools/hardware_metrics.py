"""Fill one metric record without submitting or executing experimental code."""

import json

from prompts.agents.Hardware.metric_filling import METRIC_FILLING_PROMPT


def build_metric_filling_prompt(metrics, scheduler_results, experiment_context=""):
    if isinstance(scheduler_results, dict):
        scheduler_results = [scheduler_results]
    observations = [
        item for item in scheduler_results
        if item.get("type") == "workflow_results"
    ]
    payload = {
        "requested_metrics": list(metrics),
        "experiment_context": experiment_context,
        "scheduler_results": observations,
    }
    return METRIC_FILLING_PROMPT + json.dumps(payload, ensure_ascii=False, indent=2)


def fill_metrics(
    metrics,
    *,
    real=False,
    scheduler_results=(),
    experiment_context="",
    model=None,
    input_func=None,
):
    """Use manual input by default, or one react_model call for the whole record."""
    metrics = [metric.strip() for metric in metrics if metric.strip()]
    if not real:
        if input_func is None:
            input_func = lambda metric: input(f"Please enter the {metric} of the experiment: ")
        return {metric: input_func(metric) for metric in metrics}

    if model is None:
        from llm.model import react_model

        model = react_model

    prompt = build_metric_filling_prompt(metrics, scheduler_results, experiment_context)
    response = model.invoke(prompt)
    content = response.content if hasattr(response, "content") else str(response)
    if isinstance(content, list):
        content = "\n".join(
            block if isinstance(block, str) else block.get("text", "")
            for block in content
            if isinstance(block, str)
            or (isinstance(block, dict) and block.get("type", "text") == "text")
        )
    content = content.strip()
    if content.startswith("```") and content.endswith("```"):
        content = "\n".join(content.splitlines()[1:-1]).strip()
    values = json.loads(content)
    if not isinstance(values, dict) or set(values) != set(metrics):
        raise ValueError("Metric response must contain exactly the requested metric names.")
    return {metric: values[metric] for metric in metrics}
