from llama_cpp import Llama
from pydantic import BaseModel
import CommonTypes
import re

def first_int_in_string(s: str) -> int:
    # Use regex to find the first sequence of digits
    match = re.search(r'\d+', s)
    # If a match is found, return it as an integer
    return int(match.group()) if match else -1



LLM =  Llama(verbose=False, model_path="model.gguf", chat_format="chatml") 

class LlmEvaluator:
    def answer(content: str) -> str: 
        return LLM.create_chat_completion(messages=[
            {
                "role": "system",
                "content": "La conversazione avviene in un supermercato. Ogni risposta deve essere semplice, diretta anche per persone autistiche",
            },
            {"role": "user", "content":  content},
        ])["choices"][0]["message"]["content"]
    
    def assistant(behavior: CommonTypes.SemanticBehavior) -> CommonTypes.Evaluation:
        text = f'Valutazione numerica della risposta del commesso di un supermercato da 1 (pessima) a 10 (ottima).\nCliente: "{behavior.question}".\nRisposta del commesso: "{behavior.reply}".'
        print(text)
        answer = LlmEvaluator.answer(text)
        # answer = "8. Okay?"

        return CommonTypes.Evaluation(score = first_int_in_string(answer), description = answer)

        