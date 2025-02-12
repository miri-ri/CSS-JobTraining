Download model from
https://huggingface.co/tensorblock/LLaMAntino-3-ANITA-8B-Inst-DPO-ITA-GGUF/blob/main/LLaMAntino-3-ANITA-8B-Inst-DPO-ITA-Q4_K_M.gguf

Save it 
model.gguf
Put it inside projectroot/evaluationsystem/ (here)


Download 
https://alphacephei.com/vosk/models/vosk-model-small-it-0.22.zip
Extrat the only folder inside the zip, called
vosk-model-small-it-0.22
And put this folder 
vosk-model-small-it-0.22
inside projectroot/evaluationsystem/ (here)

Download 
https://alphacephei.com/vosk/models/vosk-model-it-0.22.zip
Extrat the only folder inside the zip, called
vosk-model-it-0.22
And put this folder 

vosk-model-it-0.22
Put it inside projectroot/evaluationsystem/ (here)


USE PYTHON 3.12

create a python virtual environment
python -m venv .venv

open virtual environment
WINDOWS: .\.venv\Scripts\activate

install llama cpp FOR GPU
install cuda 
we used cuda 12.3
https://developer.nvidia.com/cuda-12-3-0-download-archive?target_os=Windows&target_arch=x86_64&target_version=Server2019&target_type=exe_local

download a wheel from the official repository
https://github.com/abetlen/llama-cpp-python
in particular you can dowload the wheel for cuda 12.3 from
https://abetlen.github.io/llama-cpp-python/whl/cu123/llama-cpp-python/
download the file corresponding to last version, your processor and python 3.12
we downloaded
https://github.com/abetlen/llama-cpp-python/releases/download/v0.3.4-cu123/llama_cpp_python-0.3.4-cp312-cp312-win_amd64.whl
move the whl file into the current working directory and install from it python llama cpp
python -m pip install .\llama_cpp_python-0.3.4-cp312-cp312-win_amd64.whl

install llama cpp FOR CPU (llamacpppython needs sdk and c++ tools for the os).
python -m pip install llama-cpp-python

install python packages for python 3.12.4 
python -m pip install fastapi[standard]
python -m pip install pyaudio
python -m pip install noisereducer
python -m pip install vosk

start with
 python -m fastapi dev main.py

api calls
GET /start-stt start to listen, return, after one minute or after 8 second of silence or after 3-4 second of silence if the user stop to speak

GET /get-stt return stt data in a format easy to send to the evaluate system (SpeechBehavior) IMPORTANT: the text is inside "semantic.reply", while semanti.question will always be empty and must be supplied for evaluation


POST /evaluate/assistant
{
  "speech": {
    "semantic": {
      "question": "Ciao, mi puoi indicare dove si trovano le mele?",
      "reply": "Certo, le faccio vedere: le mele si trovano esattamente nell'angolo in fondo a destra"
    },
    "timing": {
      "s_before_action": 0.2,
      "s_duration": 4,
      "s_before_action_target": 0.2,
      "s_duration_per_unit_target": 4.75
    }
  },
  "movement": {
    "positioning": {
      "start_pos": {
        "x": 0,
        "y": 0
      },
      "user_pos": {
        "x": 3,
        "y": 3
      },
      "target_pos": {
        "x": 4,
        "y": 4
      },
      "ok_radius": 1,
      "area": {
        "w": 10,
        "h": 10
      }
    },
    "timing": {
      "s_before_action": 4,
      "s_duration": 3,
      "s_before_action_target": 4,
      "s_duration_per_unit_target": 0.9625
    }
  }
}
