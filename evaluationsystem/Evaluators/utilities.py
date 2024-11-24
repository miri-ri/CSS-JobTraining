import math

def sigmoid(x): 
    return 1 / (1 + math.exp(-x))

def difference(actual:float, target:float):
    if(actual == 0):
        actual = 0.0001
    if(target == 0):
        target = 0.0001

    return sigmoid(-abs(math.log(actual/target))) * 2 * 10
