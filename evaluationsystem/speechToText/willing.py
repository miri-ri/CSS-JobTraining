
import CommonTypes
from utilities import *
import math

class Willing:
    def __init__(self, LLM, stt):
        self.LLM = LLM
        self.stt = stt
    
    #Function that return Truth value of a propmpt based on the function declared below in this file
    #The LLM version does not work so much better with low power network, and it is way slower.

    def evaluate(self):
        
        #From the STT it gets the first recognized non-empty text 
        reply = self.stt.listen_first_non_empty().upper()

        #it get the score
        score = check_truth(reply) 
        
        
        ##LLM VERSION (NOT WORKING)

        # prompt = (
        #     "Valuta NUMERICAMENTE da 0 a 10 se una risposta nega (0) o afferma(10) la richiesta nella domanda.\n"
        #     # "Esempi: 'Ti va di continuare?' Risposta: 'S\u00ec' -> 10. 'Ti va di smettere?' Risposta: 'Mi' -> 0.\n"
        #     f"Domanda: {behavior.question}\n"
        #     f"Risposta: {behavior.reply}"
        # ) 
        # print(prompt)

        # answer = "9. Ok!"
        # answer =   self.LLM.create_chat_completion(messages=[ 
        #     {
        #         "role": "user", 
        #         "content": prompt},
        # ])["choices"][0]["message"]["content"]

        # score = first_int_in_string(answer) / 10
        truth = score >= 0.5

        return CommonTypes.Truth(value = truth, score = score, description = reply)
 
#This function estimates positive / negative value based on certain part of phrases combined together
def check_truth(reply):
    reply = reply.upper()
    positive = ["SÌ", "SI", "OK", "OKAY", "OKEY", "VA", "VÀ", "CERT", "SENZA DUBBIO", "ADESSO", "SUBITO"]
    doubtful = ["FORSE", "CRED", "PENS", "SAPREI"]
    negative = ["NO", "IMPOSSIBILE" ]
 


    count_negative = sum(reply.count(word) for word in negative)
    count_positive = sum(reply.count(word) for word in positive)
    count_doubtful = sum(reply.count(word) for word in doubtful)

    total = count_doubtful + count_negative + count_positive 
    if(total == 0):
        return 0.5
 

    balance = (count_positive - count_negative) - count_negative
    if(abs(balance) == 0):
        normalized = 0.5
    else:
        normalized = (balance + abs(balance))/(2*abs(balance))
 
    doubt = (math.atan(count_doubtful) / (math.pi/2)) * 0.5


    if( normalized < 0.5 ):
        doubt *= -1

    normalized = normalized - doubt 
         




    
    return normalized

if __name__ == '__main__':
    print(check_truth("Non mi va"))
    print(check_truth("Non penso di credere di NO"))
    print(check_truth("Penso"))
    print(check_truth("Credo di si"))
    print(check_truth("Si si"))
    print(check_truth("No grazie"))
    print(check_truth("certamente no!"))
    print(check_truth("Dai si"))
    print(check_truth("no no"))
    print(check_truth("no no no si"))