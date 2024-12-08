
using System.IO;
using UnityEngine;

public class DemoTestApi :MonoBehaviour{
    

    void Start(){
       
testLogg();
    }
    
    
    void callTTS(){
        TTSInterface ttsReq=new();
        ttsReq.PlayAudio("questo è un super test");

    }






    void callStt(){
        STTInterface speechTT=new();
        speechTT.RequestComplete+=handleSTTresponse;
        speechTT.GetUserDialog();
    }
    public void handleSTTresponse(string yye){
        Debug.Log("resp STT----   "+yye );
    }






    void callLLMDialog(string userResp){
        LLMinterface LLM=new();
        LLM.ResponseReady+=handleLLMDialogResp;
        LLM.PrepareResponseToUser(userResp);
    }
     void callLLMevaluate(string userTranscript){
        LLMinterface LLM=new();
        LLM.EvaluationComplete+=handleLLMEvaluate;
        LLM.evaluateDialog(userTranscript);
    }

    void InterpreteSystemInteraction(string userRes){
        LLMinterface LLM=new();
        LLM.SystemResponseInterpreted+=handleInterpretation;
        LLM.evaluateSystemAnswer(userRes);
    
    }



    public void handleInterpretation(bool response){
        if(response){
            Debug.Log("user agrees");
        }
        else    Debug.Log("user disagrees");

    }
    public void handleLLMEvaluate(string txt){
        Debug.Log(" SCORE -  "+txt);

    }
    public void handleLLMDialogResp(string res){
        Debug.Log(" virtual customer -  "+res);

    }



    void testLogg(){
          StreamWriter writer= new StreamWriter("Assets/Resources/TEst.txt",true);
          writer.Write("questo è un test per vedere come funzia la scrittura a file in Resources");
        writer.Close();
        TextAsset logFile= (TextAsset)Resources.Load("TEst");
        Debug.Log(logFile.text);
    }
}