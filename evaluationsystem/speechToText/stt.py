import sys
import os
from vosk import Model, KaldiRecognizer
import pyaudio
import json
import time
import json

# Load the Vosk model
if not os.path.exists("model"):  
    print("Please download the model from https://alphacephei.com/vosk/models and unpack as 'model' in the current folder.")
    sys.exit(1)

model = Model("model")
recognizer = KaldiRecognizer(model, 16000)



def stt():
    
    # Start audio stream
    mic = pyaudio.PyAudio()
    stream = mic.open(format=pyaudio.paInt16, channels=1, rate=16000, input=True, frames_per_buffer=8000)
    stream.start_stream()

    print("Listening...")

    line = 1
    start_time = -1
    started_once = False
    speech = ""

    recent_time = time.time()
    start_time = time.time()
    start_speaking_time = time.time()
    while True:
        data = stream.read(4000)
        if recognizer.AcceptWaveform(data):
            result = json.loads(recognizer.Result())
            current_time = time.time()
            if result["text"] != "":
                recent_time = current_time
                speech += result["text"] + "\n"
            else:
                print("Silent...")
                if current_time - recent_time > 3:
                    return {
                        "s_before" : start_speaking_time - start_time if started_once else -1,
                        "s_duration" : recent_time - start_speaking_time,
                        "text" : speech
                    }
                else:
                    continue

            line+=1
            print(speech)
        else:
            partial = json.loads(recognizer.PartialResult())
            if not started_once and partial["partial"] != "":
                started_once = True
                start_speaking_time= time.time()


if __name__ == '__main__':
    print(stt())