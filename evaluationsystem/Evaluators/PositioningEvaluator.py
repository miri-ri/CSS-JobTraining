from utilities import *
import CommonTypes
class PositioningEvaluator:

    def evaluate(behavior: CommonTypes.PositioningBehavior):
        meters = math.sqrt(
            ( behavior.user_pos.x - behavior.target_pos.x ) ** 2 +
            ( behavior.user_pos.y - behavior.target_pos.y ) ** 2
        )
        
        distance_from_ok = meters - behavior.ok_radius if meters - behavior.ok_radius > 0 else 0
        relative_to = max(behavior.area.w, behavior.area.h)

        
        print(meters, distance_from_ok, relative_to)

        score = 10 - distance_from_ok/relative_to * 10 

        description = "Ottimo posizonamento!"
        if(score < 7):
            description = "Posizionamento può essere migliore!"
            if(score < 3):
                description = "Devi migliorare di molto la tua posizione"
            if(distance_from_ok/relative_to < behavior.ok_radius/relative_to):
                description += " (Serve diminuire)"
            else:
                description += " (Serve aumentare)"

        
        return CommonTypes.Evaluation(score = score, description = description)
print("posd")
