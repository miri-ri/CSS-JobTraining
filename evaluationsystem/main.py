import CommonTypes
# import Evaluators.LlmEvaluator as llme
import Evaluators.TimingEvaluator as te
import Evaluators.PositioningEvaluator as pe
from fastapi import FastAPI, BackgroundTasks

from speechToText import stt

app = FastAPI()




@app.post("/evaluate/{role}")
async def evaluate_role( role: CommonTypes.Role, behavior: CommonTypes.Behavior) -> CommonTypes.ComplexEvaluation:


    evaluations:dict[str, CommonTypes.Evaluation] = {}

    if(role is CommonTypes.Role.assistant):
        # evaluations["speech.semantic"] = llme.LlmEvaluator.assistant(behavior.speech.semantic)

        evaluations["speech.timing_before"] = te.TimingBeforeEvaluator.evaluate_before(behavior.speech.timing)

        evaluations["speech.speed"] = te.SpeechTimingEvaluator.evaluate(behavior.speech)

        evaluations["movement.timing_before"] = te.TimingBeforeEvaluator.evaluate_before(behavior.movement.timing)

        evaluations["movement.speed"] = te.MovementTimingEvaluator.evaluate(behavior.movement)

        evaluations["movement.positioning"] = pe.PositioningEvaluator.evaluate(behavior.movement.positioning)
    
    scoresum = 0
    for case in evaluations:
        scoresum += evaluations[case].score
    
    return CommonTypes.ComplexEvaluation(total = scoresum / len(evaluations), evaluations = evaluations)


generated_text: CommonTypes.GeneratedText = CommonTypes.GeneratedText(s_before_action = -1, s_duration = -1, text="")

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
    global generated_text

    generated_text = CommonTypes.GeneratedText(s_before_action = -1, s_duration = -1, text="")

    background_tasks.add_task(generateText)

    return "Ok"



@app.get("/get-stt")
async def getStt() ->  CommonTypes.GeneratedText:
    global generated_text
    return generated_text


    