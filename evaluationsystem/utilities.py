import math
import re
# in quale punto del seguente range mi trovo?
# |scire:0|---deltaInf----|target score:10|------------deltaSup---------|score:0|
def difference(actual:float, target:float, deltaInf, deltaSup, exp:float = 1):
    minimum = target - deltaInf
    maximum = target + deltaSup

    print(actual, target, minimum, maximum)

    if actual < minimum:
        return 0
    if actual > maximum:
        return 0
    
    if actual == target:
        return 10
    
    if actual > target:
        return (1 - (actual - target)**exp / deltaSup) * 10
    
    if actual < target:
        return (1 - (target - actual)**exp / deltaInf) * 10
    

def sigmoid(x): 
    return 1 / (1 + math.exp(-x))

    # if(actual == 0):
    #     actual = 0.0001
    # if(target == 0):
    #     target = 0.0001

    # return sigmoid(-abs(math.log(actual/target))) * 2 * 10
    
def first_int_in_string(s: str) -> int:
    # Use regex to find the first sequence of digits
    match = re.search(r'\d+', s)
    # If a match is found, return it as an integer
    return int(match.group()) if match else -1
