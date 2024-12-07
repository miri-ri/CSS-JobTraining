

using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
public class TTSInterface : MonoBehaviour
{
    void Start()
    {
    }
    public void PlayAudio(string text){
        StartCoroutine(GetData("http://localhost:9000/TTS?"+text));//to api
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
        }
    }
}
