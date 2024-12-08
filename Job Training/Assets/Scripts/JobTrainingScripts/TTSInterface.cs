

using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
public class TTSInterface : MonoBehaviour
{
    void Start()
    { 
       // PlayAudio("piosposdoas"); works!
    }
    public void PlayAudio(string text){
        StartCoroutine(GetData("https://e7860e6c-7a73-4998-a664-8611b99efe03.mock.pstmn.io/TTS"));//to api
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
        }
    }
}
