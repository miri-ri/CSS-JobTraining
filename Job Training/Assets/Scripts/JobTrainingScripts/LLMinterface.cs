using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.Security.Cryptography;
using System.Collections.Generic;


public delegate void OnLLMresponseToUserReady(string response);
public delegate void OnEvaluationReady(EvaluationResponse response);
public delegate void OnSystemInteractionReady(bool res);



public class LLMinterface : MonoBehaviour
{

    public event OnLLMresponseToUserReady ResponseReady;


    public event OnEvaluationReady EvaluationComplete;



    public event OnSystemInteractionReady SystemResponseInterpreted;



    IEnumerator PostData(string url, string jsonData){
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else { 
            EvaluationComplete?.Invoke(JsonConvert.DeserializeObject<EvaluationResponse>(  request.downloadHandler.text));
            Debug.Log("Response: " + request.downloadHandler.text); 
        }
    }

    public void evaluateDialog(DataForEvaluation dataTask){//give transcript of conversation
      
        var jsonData=Newtonsoft.Json.Linq.JObject.FromObject(dataTask);
        Debug.Log(jsonData.ToString());
        StartCoroutine(PostData("http://127.0.0.1:8000/evaluate/assistant", jsonData.ToString()));

                
    }
    
    public void evaluateSystemAnswer(){//interaction user - system
        StartCoroutine(GetData("http://localhost:8000/willing",true));

    }
    //need api
    public void PrepareResponseToUser(string lastUserResponse){
        StartCoroutine(GetData("http://localhost:8000/respond?",false));
    }
    IEnumerator GetData(string url, bool systemic)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        { Debug.LogError(www.error); }
        else
        { // Show results as text 

            if(systemic){
                SystemResponseInterpreted?.Invoke(JsonConvert.DeserializeObject<UserWillingness>(www.downloadHandler.text).value);

            }else{

                ResponseReady?.Invoke(www.downloadHandler.text);
            }
            Debug.Log(www.downloadHandler.text); 
            
            
        }
    }
}

public class UserWillingness{
    public bool value;
    public float score;
    public string description;

}


public class Evaluation
{
    public double Score { get; set; }
    public string Description { get; set; }//add a descriptor for the type of eval- (speech duration, speed etccc)
}


public class EvaluationResponse
{
    public double Total { get; set; }
    public Evaluation[] Evaluations { get; set; }}


//to json
public class DataForEvaluation{
    public Speech speech { get; set; }
    public Movement movement { get; set; }
    public DataForEvaluation(){
        speech=new();
        movement=new();
    }

}

public class Speech
{
    public Semantic semantic { get; set; }
    public Timing timing { get; set; }
    public Speech(){
        semantic=new();
        timing=new();
    }
}

public class Semantic
{
    public string question { get; set; }
    public string reply { get; set; }
}

public class Timing
{
    public float s_before_action { get; set; }//how, the user is never really static with the kinect, would need a complex way to check it
    public float s_duration { get; set; }
    public float s_before_action_target { get; set; }
    public float s_duration_per_unit_target { get; set; }
}

public class Movement
{
    public Positioning positioning;
    public Timing timing;
    public Movement(){
        positioning=new();
        timing=new();
    }
}
public class Positioning
{
    public Position start_pos { get; set; }
    public Position user_pos { get; set; }
    public Position target_pos { get; set; }
    public float ok_radius { get; set; }
    public Area area { get; set; }

    public Positioning(){
        area=new();
        ok_radius=1;
    }
}

public class Position
{
    public float x { get; set; }
    public float y { get; set; }
    public Position(Vector2 coord){
        x=coord.x;
        y=coord.y;
    }
    public Position(){
        x=0;
        y=0;
    }
}

public class Area
{
    public float w { get; set; }
    public float h { get; set; }
    public Area(){
        w=1;
        h=1;
    }
}



