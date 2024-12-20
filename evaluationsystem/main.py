import CommonTypes
import Evaluators.LlmEvaluator as llme
import Evaluators.TimingEvaluator as te
import Evaluators.PositioningEvaluator as pe

from contextlib import asynccontextmanager
from fastapi import FastAPI, BackgroundTasks


from speechToText import stt


app_stt = stt.Stt()

 

app = FastAPI()


@app.post("/evaluate/{role}")
async def evaluate_role( role: CommonTypes.Role, behavior: CommonTypes.Behavior) -> CommonTypes.ComplexEvaluation:


    evaluations:dict[str, CommonTypes.Evaluation] = {}

    if(role is CommonTypes.Role.assistant):
        evaluations["speech.semantic"] = llme.LlmEvaluator.assistant(behavior.speech.semantic)

        evaluations["speech.timing_before"] = te.TimingBeforeEvaluator.evaluate_before(behavior.speech.timing)

        evaluations["speech.speed"] = te.SpeechTimingEvaluator.evaluate(behavior.speech)

        evaluations["movement.timing_before"] = te.TimingBeforeEvaluator.evaluate_before(behavior.movement.timing)

        evaluations["movement.speed"] = te.MovementTimingEvaluator.evaluate(behavior.movement)

        evaluations["movement.positioning"] = pe.PositioningEvaluator.evaluate(behavior.movement.positioning)
    
    scoresum = 0
    for case in evaluations:
        scoresum += evaluations[case].score
    
    return CommonTypes.ComplexEvaluation(total = scoresum / len(evaluations), evaluations = evaluations)



def generateText() -> None:
    global generated_text
    stt_data = stt.stt()


    generated_text =  CommonTypes.GeneratedText(
        s_before_action= stt_data["s_before"],
        s_duration= stt_data["s_duration"],
        text=stt_data["text"]
    )
 

@app.get("/start-stt")
async def startStt(background_tasks: BackgroundTasks) -> str:
    global app_stt

    app_stt.start()

    return "Finished"



@app.get("/get-stt")
async def getStt() ->  CommonTypes.SpeechBehavior:
    global app_stt 

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


    