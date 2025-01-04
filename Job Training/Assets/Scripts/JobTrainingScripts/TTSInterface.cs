

using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
using System.Linq;
public class TTSInterface : MonoBehaviour
{
    public event OnTTSPlaying TTsPlaying;
    public void PlayAudio(string text){
        StartCoroutine(GetData("https://e7860e6c-7a73-4998-a664-8611b99efe03.mock.pstmn.io/TTS",text));//to api
    }
    IEnumerator GetData(string url,string ttsText)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url+"?text="+ttsText);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        { Debug.LogError(www.error); }
        else
        { 
            TTsPlaying?.Invoke(ApproximateTimeToSpeach(www.downloadHandler.text));//find if we can get lenght from api, otherwise just count n of words and approximate;
            Debug.Log(www.downloadHandler.text);
        }
    }
    public float ApproximateTimeToSpeach(string text){
        int words=text.Split().Length;
        float secondsPerWord=0.2f;
        return words*secondsPerWord;
    }
}
    public delegate void OnTTSPlaying(float lenghtInSeconds);
