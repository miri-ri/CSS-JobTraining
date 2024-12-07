using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
public class LLMinterface : MonoBehaviour
{
    void Start()
    {
    }
    public string evaluateDialog(string ConvoTranscript){//give transcript of conversation
        StartCoroutine(GetData("http://localhost:9000/Eval?"));//to api
        return "";
    }
    public bool evaluateSystemAnswer(string answer){
        StartCoroutine(GetData("http://localhost:9000/answer?"));//to api

        return false;
    }
    public void PrepareResponseToUser(string lastUserResponse){
        StartCoroutine(GetData("http://localhost:9000/respond?"));//to api
    }
    IEnumerator GetData(string url)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        { Debug.LogError(www.error); }
        else
        { // Show results as text 
            Debug.Log(www.downloadHandler.text); // Or retrieve results as binary
            byte[] results = www.downloadHandler.data;
            //launch event and attach text from api
        }
    }
}
