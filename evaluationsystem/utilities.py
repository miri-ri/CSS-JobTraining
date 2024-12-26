import math
import re

def sigmoid(x): 
    return 1 / (1 + math.exp(-x))

def difference(actual:float, target:float):
    if(actual == 0):
        actual = 0.0001
    if(target == 0):
        target = 0.0001

    return sigmoid(-abs(math.log(actual/target))) * 2 * 10
    
def first_int_in_string(s: str) -> int:
    # Use regex to find the first sequence of digits
    match = re.search(r'\d+', s)
    # If a match is found, return it as an integer
    return int(match.group()) if match else -1
