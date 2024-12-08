using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
public class LLMinterface : MonoBehaviour
{

    public delegate void OnLLMresponseToUserReady(string response);
    public event OnLLMresponseToUserReady ResponseReady;


    public delegate void OnEvaluationReady(string response);
    public event OnEvaluationReady EvaluationComplete;



     public delegate void OnSystemInteractionReady(bool res);
    public event OnSystemInteractionReady SystemResponseInterpreted;

    void Start(){ 
    }
   
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
            Debug.Log("Response: " + request.downloadHandler.text); 
        }
    }

    public void evaluateDialog(string ConvoTranscript){//give transcript of conversation
        StartCoroutine(PostData("https://api.example.com/data", "{\" key \":\"value\"}"));
    }
    public void evaluateSystemAnswer(string answer){//interaction user - system
        StartCoroutine(GetData("http://localhost:9000/answer?",true));

    }
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
