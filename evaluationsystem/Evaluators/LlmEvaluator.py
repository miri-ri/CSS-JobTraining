from pydantic import BaseModel
import CommonTypes
import re
from utilities import *




class LlmEvaluator:
    def answer(LLM, content: str) -> str: 
        return LLM.create_chat_completion(messages=[
            {
                "role": "system",
                "content": "La conversazione avviene in un supermercato. Ogni risposta deve essere semplice, diretta anche per persone autistiche",
            },
            {"role": "user", "content":  content},
        ])["choices"][0]["message"]["content"]
    
    def assistant(LLM, behavior: CommonTypes.SemanticBehavior) -> CommonTypes.Evaluation:
        text = f'Valutazione numerica della risposta del commesso di un supermercato da 1 (pessima) a 10 (ottima).\nCliente: "{behavior.question}".\nRisposta del commesso: "{behavior.reply}".'
        print(text)
        answer = LlmEvaluator.answer(LLM, text)
        # answer = "8. Okay?"

        return CommonTypes.Evaluation(score = first_int_in_string(answer), description = re.sub(r'\d+', '', answer))


if __name__ == "__main__":
    from llama_cpp import Llama
    LLM = Llama(verbose=True, model_path="../model.gguf", chat_format="chatml", n_gpu_layers=-1)

    print("start")
    answer = LlmEvaluator.answer(LLM, "Ciao, come stai? Mi scrivi un pezzo della divina commedia?")
    print(answer)
    print("end")

