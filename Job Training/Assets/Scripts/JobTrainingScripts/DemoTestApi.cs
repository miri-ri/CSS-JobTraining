
using System;
using System.IO;
using UnityEngine;
 
public class DemoTestApi :MonoBehaviour{
    

    void Start(){
       
  }
  

    
    void callTTS(){
        TTSInterface ttsReq=gameObject.AddComponent<TTSInterface>();
        ttsReq.PlayAudio("questo è un super test");
        
    }






    void callStt(){
        STTInterface speechTT=gameObject.AddComponent<STTInterface>();
       // speechTT.RequestComplete+=handleSTTresponse;
        speechTT.GetUserDialog();
    }
    public void handleSTTresponse(string yye){
        Debug.Log("resp STT----   "+yye );
    }






    void callLLMDialog(string userResp){
        LLMinterface LLM=gameObject.AddComponent<LLMinterface>();
        LLM.ResponseReady+=handleLLMDialogResp;
        LLM.PrepareResponseToUser(userResp);


   
       
    }


    
     void callLLMevaluate(DataForEvaluation data){
        LLMinterface LLM=gameObject.AddComponent<LLMinterface>();
        LLM.EvaluationComplete+=handleLLMEvaluate;
        LLM.evaluateDialog(data);
    }



    public void handleLLMEvaluate(EvaluationResponse response){
       
        
    }





    public void handleInterpretation(bool response){
        if(response){
            Debug.Log("user agrees");
        }
        else    Debug.Log("user disagrees");

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