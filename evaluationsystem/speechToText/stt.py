from vosk import Model, KaldiRecognizer  
import pyaudio
import numpy as np
import noisereduce as nr
import time
import json
import sys
import os

import wave

class Stt:
    RATE = 16000  # Sample rate (Vosk model requires 16kHz)
    CHANNELS = 1 # Mono input
    FORMAT = pyaudio.paInt16  # 16-bit PCM format
    s_BUFFER_DURATION = 1 # duration of each chunk
    FRAMES_PER_BUFFER = round( s_BUFFER_DURATION * RATE )

    s_RECOGNITION_FRAME_DURATION = 0.125
    RECOGNITION_FRAME = round( s_RECOGNITION_FRAME_DURATION * RATE )

    s_TIMEOUT_BEFORE_SPEAKING = 8
    s_TIMEOUT_INTERRUPTING_SPEAKING = 2

    status = "off"

    def __init__(self):
        
        # Initialize Vosk Model
        
        print("Loading models...")
        self.fast_vosk = Model(os.path.join(os.getcwd(), 'vosk-model-small-it-0.22'))
        self.slow_vosk = Model(os.path.join(os.getcwd(), 'vosk-model-it-0.22'))

        self.fast_recognizer = KaldiRecognizer(self.fast_vosk, self.RATE)
        self.slow_recognizer = KaldiRecognizer(self.slow_vosk, self.RATE)

        
        self.s = 0
        self.s_before_start = -1
        self.s_last_non_empty = -1
        self.s_duration = -1
        self.text = ""
        
        self.recording = b''

        self.status = "ready"
        
        print("Ready!")

    def listen_first_non_empty(self) -> str:
        
        self.status = "starting"
        self.p = pyaudio.PyAudio()
        
        stream = self.p.open(
            format=self.FORMAT, 
            channels=self.CHANNELS, 
            rate=self.RATE, 
            input=True, 
            frames_per_buffer=self.FRAMES_PER_BUFFER
        )

        all_empty = True
        print("Listening...")
        
        self.status = "listening"
        while True:
            chunk = stream.read(self.RECOGNITION_FRAME, exception_on_overflow=False)
             

            # Perform transcription using Vosk
            if self.slow_recognizer.AcceptWaveform(chunk):
                result = self.slow_recognizer.Result()
                value = json.loads(result)["text"]

                print(value) 

                if len(value) > 0:
                    all_empty = False
                
                    break
        
        self.status = "ready"
        return value




    def start(self):
        self.status = "starting"
                
        # PyAudio Setup
        self.p = pyaudio.PyAudio()
        
        stream = self.p.open(
            format=self.FORMAT, 
            channels=self.CHANNELS, 
            rate=self.RATE, 
            input=True, 
            frames_per_buffer=self.FRAMES_PER_BUFFER
        )


        self.s = 0
        self.s_before_start = -1
        self.s_last_non_empty = -1
        self.s_duration = -1

        self.recording = b''

        self.text = ""

        start_time = time.time()

        print("Listening...")
        
        self.status = "listening"
        while True:

            chunk = stream.read(self.RECOGNITION_FRAME, exception_on_overflow=False)
             

            # Perform transcription using Vosk
            if self.fast_recognizer.AcceptWaveform(chunk):
                result = self.fast_recognizer.Result()
                value = json.loads(result)["text"]
                print(round((time.time()-start_time)*1000 )/1000, "s passed - analizing ",self.s, "sec" , end="\r")
            else:
                partial_result = self.fast_recognizer.PartialResult()
                value = json.loads(partial_result)["partial"]
                if( (self.s / self.s_RECOGNITION_FRAME_DURATION)%1 == 0 ):
                    pass
                    # print(round( (time.time()-start_time)*1000 )/1000, "s passed - analizing ",self.s, "sec", end="\r")

            if len(value) > 0:
                self.s_last_non_empty = self.s

            if self.s_before_start < 0 and len(value) > 0:
                self.s_before_start = self.s


            self.s += self.s_RECOGNITION_FRAME_DURATION

            self.recording += chunk
            
            if (self.s - self.s_last_non_empty > self.s_TIMEOUT_INTERRUPTING_SPEAKING and self.s_before_start > 0) \
                or (self.s_before_start < 0 and self.s > self.s_TIMEOUT_BEFORE_SPEAKING):
                self.s_duration = self.s_last_non_empty - self.s_before_start
                break
        
        print("Stop...")
        
        self.status = "stopped"

        
        # wavefile = wave.open("original.wav", "wb")
        # wavefile.setnchannels(self.CHANNELS)
        # wavefile.setsampwidth(self.p.get_sample_size(self.FORMAT))
        # wavefile.setframerate(self.RATE)
        # wavefile.writeframes(self.recording)
        # wavefile.close()

        # print("Created audio file...")



    def analyze(self):
        
        self.status = "elaborating"
        self.text = ""

        print("copying data...")
        data = np.frombuffer(self.recording, dtype=np.int16)
        print("cleaning audio....")
        cleaned_data = nr.reduce_noise(y=data,  sr=self.RATE, prop_decrease=1)
        print("getting audio bytes....")
        cleaned = cleaned_data.tobytes()
        
        # wavefile = wave.open("cleaned.wav", "wb")
        # wavefile.setnchannels(self.CHANNELS)
        # wavefile.setsampwidth(self.p.get_sample_size(self.FORMAT))
        # wavefile.setframerate(self.RATE)
        # wavefile.writeframes(cleaned)
        # wavefile.close()

        print("analyzing")

        start_time = time.time()
            
        frames_nr = round(self.s / self.s_RECOGNITION_FRAME_DURATION)
        read = b''
        print(self.s, self.s_RECOGNITION_FRAME_DURATION, frames_nr, round(frames_nr))

        text_sure = ""
        unsure = ""
 
        for i in range(0, frames_nr * self.RECOGNITION_FRAME, self.RECOGNITION_FRAME):  
                
            chunk = cleaned[i*2:i*2 + self.RECOGNITION_FRAME*2 ]

            read += chunk

            if self.slow_recognizer.AcceptWaveform(chunk):
                result = self.slow_recognizer.Result()
                value = json.loads(result)["text"] 
                print("GREAT!>", round((time.time()-start_time)*1000 )/1000, "s passed - analizing ",(i/self.RECOGNITION_FRAME)*self.s_RECOGNITION_FRAME_DURATION, "sec")
                text_sure += " "+value

            else:
                # print("      > ", round((time.time()-start_time)*1000 )/1000, "s passed - analizing ",(i/self.RECOGNITION_FRAME)*self.s_RECOGNITION_FRAME_DURATION, "sec")
                
                partial_result = self.slow_recognizer.PartialResult()
                value = json.loads(partial_result)["partial"] 
                unsure = value

            # print(text_sure+" "+unsure)
        
        self.text = text_sure+" "+unsure
        
        self.status = "ready"
            
            
        
                


            # wavefile = wave.open("read.wav", "wb")
            # wavefile.setnchannels(self.CHANNELS)
            # wavefile.setsampwidth(self.p.get_sample_size(self.FORMAT))
            # wavefile.setframerate(self.RATE)
            # wavefile.writeframes(read)
            # wavefile.close()
                

            # print("Created audio file...")

if __name__ == '__main__':
    stt = Stt()

    print(stt.listen_first_non_empty())

    # stt.start()

    # stt.analyze()

        


