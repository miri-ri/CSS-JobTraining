import CommonTypes
from utilities import *



class TimingBeforeEvaluator:
    def evaluate_before(behavior: CommonTypes.TimingBehavior):
        
        score =  difference(behavior.s_before_action, behavior.s_before_action_target, behavior.s_before_action_target, 10)
        description = "Ottimo tempismo!"
        if(score < 8):
            description = "Il tempismo può essere migliore!"
            if(score < 5):
                description = "Devi migliorare molto il tuo tempismo!"
                
            if(behavior.s_before_action < behavior.s_before_action_target):
                description += " (Serve diminuire)"
            else:
                description += " (Serve aumentare)"

        return CommonTypes.Evaluation(score = score, description = description)


class SpeechTimingEvaluator:
    def evaluate(behavior: CommonTypes.SpeechBehavior):
        words = len(behavior.semantic.reply.split())
 
        if(behavior.timing.s_duration > 0):
            words_per_second = words/behavior.timing.s_duration
        else:
            words_per_second = 10000
        print("words per seconds: ", words_per_second)
        score =  difference(words_per_second, behavior.timing.s_duration_per_unit_target, 4, 9)     

        description = "Ottima velocità nel parlare! "
        if(score < 8):
            description = f"La velocità nel parlare può essere migliore!"
            if(score < 5):
                description = "Devi migliorare di molto la tua velocità nel parlare"
            
            if(words_per_second < behavior.timing.s_duration_per_unit_target):
                description += " (Serve aumentare la velocità)"
            else:
                description += " (Serve diminuire la velocità)"
        
        return CommonTypes.Evaluation(score = score, description = description)




class MovementTimingEvaluator:
    def evaluate(behavior: CommonTypes.MovementBehavior):
        meters = math.sqrt(
            ( behavior.positioning.start_pos.x - behavior.positioning.user_pos.x ) ** 2 +
            ( behavior.positioning.start_pos.y - behavior.positioning.user_pos.y ) ** 2
        )
        
         
        if(behavior.timing.s_duration > 0):
            meters_per_second = meters/behavior.timing.s_duration
        else:
            meters_per_second = 10000

        score = difference(meters_per_second, behavior.timing.s_duration_per_unit_target, 0.6, 3)
                
        description = "Ottima velocità di movimento!"
        if(score < 8):
            description = "La velocità di movimento può essere migliore!"
            if(score < 5):
                description = "Devi migliorare di molto la tua velocità di movimento"
            if(meters_per_second < behavior.timing.s_duration_per_unit_target):
                description += " (Serve aumentare)"
            else:
                description += " (Serve diminuire)"

        
        return CommonTypes.Evaluation(score = score, description = description)

