import CommonTypes
import Evaluators.LlmEvaluator as llme
import Evaluators.TimingEvaluator as te
import Evaluators.PositioningEvaluator as pe

from llama_cpp import Llama

from contextlib import asynccontextmanager
from fastapi import FastAPI, BackgroundTasks



EFFICIENT_MODE = True
 
if not EFFICIENT_MODE: 
    from speechToText import stt, willing
    app_stt = stt.Stt()
    LLM =  Llama(verbose=False, model_path="model.gguf", chat_format="chatml") 
else: 
    app_stt = None
    LLM = None

 

app = FastAPI()

@app.get("/willing")
async def willing_response() -> CommonTypes.Truth:
    global LLM, app_stt
    
    if EFFICIENT_MODE: 
        return
    willChecker = willing.Willing(LLM, app_stt)
    return willChecker.evaluate()

#NOTA IMP: forse con async il sistema rimane in grado di rispondere AD ATLTRE COSE! PER VEDERE STATO DEL DISCORSO!
@app.post("/evaluate/{role}")
async def evaluate_role( role: CommonTypes.Role, behavior: CommonTypes.Behavior) -> CommonTypes.ComplexEvaluation:
    global LLM

    evaluations = []

    if(role is CommonTypes.Role.assistant):
        
        if not EFFICIENT_MODE: 
            evaluations.append(llme.LlmEvaluator.assistant(LLM, behavior.speech.semantic))

        evaluations.append(te.TimingBeforeEvaluator.evaluate_before(behavior.speech.timing))

        evaluations.append(te.SpeechTimingEvaluator.evaluate(behavior.speech))

        evaluations.append(te.TimingBeforeEvaluator.evaluate_before(behavior.movement.timing))

        evaluations.append(te.MovementTimingEvaluator.evaluate(behavior.movement))

        evaluations.append(pe.PositioningEvaluator.evaluate(behavior.movement.positioning))
    
    scoresum = 0
    for case in evaluations:
        scoresum += case.score
    
    return CommonTypes.ComplexEvaluation(total = scoresum / len(evaluations), evaluations = evaluations)


##THIS function does not work. It should return a the status of stt, but the stt is still blocking.
#That's better because we avoid conflicts, but it can be improved for giving the user more feedback
# @app.get("/get-stt-status")
# async def sttStatus() -> str:
#     global app_stt
#     return app_stt.status

@app.get("/start-stt")
async def startStt(background_tasks: BackgroundTasks) -> str:
    global app_stt
    
    if EFFICIENT_MODE: 
        return

    app_stt.start()

    return "Finished"



@app.get("/get-stt")
async def getStt() ->  CommonTypes.SpeechBehavior:
    global app_stt 

    
    if EFFICIENT_MODE: 
        return

    app_stt.analyze()
 

    return CommonTypes.SpeechBehavior(
        semantic= CommonTypes.SemanticBehavior(
            question="", 
            reply=app_stt.text
        ),
        timing= CommonTypes.SpeechTimingBehavior(
            s_before_action = app_stt.s_before_start,
            s_duration = app_stt.s_duration
        )
    ) 


    