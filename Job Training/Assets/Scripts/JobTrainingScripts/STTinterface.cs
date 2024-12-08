using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
public class STTInterface : MonoBehaviour
{
    public delegate void OnSTTReady(string response);
    public event OnSTTReady RequestComplete;
    void Start()
    {
    }
    public string GetUserDialog(){
        StartCoroutine(GetData("http://localhost:9000/STT?"));//to api
        return "";
    }
    IEnumerator GetData(string url)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        { Debug.LogError(www.error); }
        else
        {
            Debug.Log(www.downloadHandler.text); 
            RequestComplete?.Invoke(www.downloadHandler.text);
        
        }
    }
}
