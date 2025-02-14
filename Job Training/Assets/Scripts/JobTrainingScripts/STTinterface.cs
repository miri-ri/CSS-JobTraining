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
        Debug.LogError(JobTrainingManager.jobtrainerServer+"/start-stt");
        StartCoroutine(StartListening(JobTrainingManager.jobtrainerServer+"/start-stt"));
    }
    public void GetUserDialog(){
        StartCoroutine(GetData(JobTrainingManager.jobtrainerServer+"/get-stt"));
       
    }

    IEnumerator StartListening(string url)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
       // www.insecureHttpOption = UnityWebRequest.InsecureHttpOption.AlwaysAllowed;///does tis work????

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
        //www.insecureHttpOption = UnityWebRequest.InsecureHttpOption.AlwaysAllowed;
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
