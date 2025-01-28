using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
using System;
using Newtonsoft.Json;

public delegate void OnSTTReady(Speech response);
public delegate void STTstoppedListening();//for the sake of visual feedback to the user

public class STTInterface : MonoBehaviour
{
    public event OnSTTReady RequestComplete;
    public event STTstoppedListening ListeningComplete;

    public void StartTTSListening(){
        StartCoroutine(StartListening("http://localhost:8000/start-stt"));
    }
    public void GetUserDialog(){
        StartCoroutine(GetData("http://localhost:8000/get-stt"));
       
    }

    IEnumerator StartListening(string url)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        { Debug.LogError(www.error); }
        else
        {
            //Debug.Log(www.downloadHandler.text); 
                ListeningComplete.Invoke();
                //the ouput is realtime on my (nicola) computer, not sure on the actual machine
                Debug.Log("stopped listening to user, retrieving transcript");
                GetUserDialog();
            
            
        
        }
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
            
            RequestComplete?.Invoke(JsonConvert.DeserializeObject<Speech>( www.downloadHandler.text));
        
        }
    }
}
