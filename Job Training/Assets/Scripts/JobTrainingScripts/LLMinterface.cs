using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.Security.Cryptography;


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

    //!!!!!works with stub!!! however adapt and check with the correct api
    public void evaluateDialog(DataForEvaluation dataTask){//give transcript of conversation
      
        var jsonData=Newtonsoft.Json.Linq.JObject.FromObject(dataTask);
        Debug.Log(jsonData.ToString());
        StartCoroutine(PostData("http://127.0.0.1:8000/evaluate/assistant", jsonData.ToString()));

                
    }
    //need api
    public void evaluateSystemAnswer(string answer){//interaction user - system
        StartCoroutine(GetData("http://localhost:8000/",true));

    }
    //need api
    public void PrepareResponseToUser(string lastUserResponse){
        StartCoroutine(GetData("http://localhost:9000/respond?",false));
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
                SystemResponseInterpreted?.Invoke(www.downloadHandler.text=="accept");

            }else{

                ResponseReady?.Invoke(www.downloadHandler.text);
            }
            Debug.Log(www.downloadHandler.text); // Or retrieve results as binary
            
            //launch event and attach text from api
        }
    }
}
public class EvaluationResponse{
    public string description;
    public int Score;
}

//to json
public class DataForEvaluation
{
    public Speech speech { get; set; }
    public Movement movement { get; set; }
}

public class Speech
{
    public Semantic semantic { get; set; }
    public Timing timing { get; set; }
}

public class Semantic
{
    public string question { get; set; }
    public string reply { get; set; }
}

public class Timing
{
    public float s_before_action { get; set; }
    public float s_duration { get; set; }
    public float s_before_action_target { get; set; }
    public float s_duration_per_unit_target { get; set; }
}

public class Movement
{
    public float s_before_action { get; set; }
    public float s_duration { get; set; }
    public float s_before_action_target { get; set; }
    public float s_duration_per_unit_target { get; set; }
}
