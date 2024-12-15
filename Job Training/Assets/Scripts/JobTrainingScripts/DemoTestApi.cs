
using System;
using System.IO;
using UnityEngine;
 
public class DemoTestApi :MonoBehaviour{
    

    void Start(){
        DataForEvaluation test=new();
    
       test = new DataForEvaluation
        {
            speech = new Speech
            {
                semantic = new Semantic
                {
                    question = "string",
                    reply = "string"
                },
                timing = new Timing
                {
                    s_before_action = 0,
                    s_duration = 0,
                    s_before_action_target = 0.2f,
                    s_duration_per_unit_target = 4.75f
                }
            },
            movement = new Movement
            {
                positioning = new Positioning
                {
                    start_pos = new Position { x = 0, y = 0 },
                    user_pos = new Position { x = 0, y = 0 },
                    target_pos = new Position { x = 0, y = 0 },
                    ok_radius = 1,
                    area = new Area { w = 2, h = 2 }
                },
                timing = new Timing
                {
                    s_before_action = 0,
                    s_duration = 0,
                    s_before_action_target = 0,
                    s_duration_per_unit_target = 0.9625f
                }
            }
        };
        
        // Serialize the object to JSON
        var jsonString = Newtonsoft.Json.Linq.JObject.FromObject(test);
        Debug.Log(jsonString.ToString());
    callLLMevaluate(test);  
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
        Debug.Log(" SCORE -  "+response.total);
        foreach (var item in response.evaluations)
        {
            Debug.Log(item);
        }
        
    }

    void InterpreteSystemInteraction(string userRes){
        LLMinterface LLM=gameObject.AddComponent<LLMinterface>();
        //=new();
        LLM.SystemResponseInterpreted+=handleInterpretation;
        LLM.evaluateSystemAnswer(userRes);
    
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