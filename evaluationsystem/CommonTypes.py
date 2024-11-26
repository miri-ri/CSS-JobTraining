from enum import Enum
from pydantic import BaseModel
from typing import Union

class Role(str, Enum):
    assistant = "assistant"

class Position(BaseModel): 
    x: float
    y: float

class Size(BaseModel):
    w: float
    h: float


class SemanticBehavior(BaseModel):
    question: str
    reply:str

class TimingBehavior(BaseModel):
    s_before_action: float #ms before doing the action (speaking/moving)
    s_duration: float      #ms for doing the action

    s_before_action_target: float = 0 #target nr of ms before doing the action
    s_duration_per_unit_target: float = 0 #target nr of ms for each unit of the action (e.g. ms per meter in standard unit)

class SpeechTimingBehavior(TimingBehavior):
    s_before_action_target:float = 0.200 #200ms before speaking  https://pmc.ncbi.nlm.nih.gov/articles/PMC10077995/
    s_duration_per_unit_target: float = 4.75 #285 word per minute on avarage https://www.sciencedirect.com/science/article/pii/S0749596X19300786
class MovementTimingBehavior(TimingBehavior):
    s_before_action_target:float = 0 #0 before moving, that's just empirical but can be changed
    s_duration_per_unit_target: float = 0.9625 # m/s avarage walking speed indoor on the field and experiment file:///C:/Users/Domen/Downloads/sustainability-16-04813.pdf


class PositioningBehavior(BaseModel):
    start_pos: Union[Position, None]
    user_pos: Union[Position, None]
    target_pos: Union[Position, None]
    ok_radius: float 
    area: Size

class SpeechBehavior(BaseModel):
    semantic: SemanticBehavior
    timing: Union[SpeechTimingBehavior, None]= None

class MovementBehavior(BaseModel):
    positioning: PositioningBehavior
    timing: Union[MovementTimingBehavior, None] = None

class Behavior(BaseModel):
    speech: SpeechBehavior
    movement: Union[MovementTimingBehavior, None] = None


class Evaluation(BaseModel):
    score: float
    description: Union[str, None] = None

class ComplexEvaluation(BaseModel):
    total: float
    evaluations: dict[str, Evaluation] = {}

class GeneratedText(BaseModel):
    s_before_action: float
    s_duration: float
    text: str