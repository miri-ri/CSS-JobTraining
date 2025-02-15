import CommonTypes
from utilities import *



class TimingBeforeEvaluator:
    def evaluate_before(behavior: CommonTypes.TimingBehavior, what):
        
        score =  difference(behavior.s_before_action, behavior.s_before_action_target, behavior.s_before_action_target, 10)

        #for the movement the kinect have some calibration error and it detect some movement too early
        #the score must be less important in that case and it is reduced to a maximum of 9 point instead of 10
        if what == "muoverti":
            score = (score / 10) * 9

        description = f"Ottima prontezza nel {what}!"
        if(score < 8):
            description = f"La tua prontezza nel {what} può essere migliore!"
            if(score < 5):
                description = f"Devi migliorare molto la tua prontezza nel {what}!"
                
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

        print("speech timing: ", behavior.semantic.reply.split(), words, words_per_second, score)

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

        #in the i3lab 1 unit of movement~=0.3333m
        #that's because of the kinect calibration
        #so the "targeted" value was wrong
        #since this is a very sensible system, we prefer to leave in the api call the correct
        #targeted value in meters, and multiply them by 3-scale factor for the evaluation
        #the "meter_per_second" variable is actually in "unit_per_second" 
        #it should be in meter, but it depends on the kinect. 
        #in this way the evaluation is adjusted.
        meters_to_i3lab_factor = 3
        score = difference(
            meters_per_second, 
            behavior.timing.s_duration_per_unit_target * meters_to_i3lab_factor, 
            0.6 * meters_to_i3lab_factor, 
            3 * meters_to_i3lab_factor
        )
                
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

