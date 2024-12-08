using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
public class STTInterface : MonoBehaviour
{
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
        { // Show results as text 
            Debug.Log(www.downloadHandler.text); // Or retrieve results as binary
            byte[] results = www.downloadHandler.data;
            //launch event and attach text from api
        }
    }
}
