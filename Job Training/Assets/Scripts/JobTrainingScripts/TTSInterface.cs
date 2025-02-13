

using UnityEngine; 
using UnityEngine.Networking; 
using System.Collections;
using System.Linq;
public class TTSInterface : MonoBehaviour
{
    public event OnTTSPlaying TTsPlaying;
    public void PlayAudio(string text){ //lookup how it works with Secco once ready
        MagicRoomManager.instance.MagicRoomTextToSpeachManager.GenerateAudioFromText(text);
        TTsPlaying.Invoke(ApproximateTimeToSpeach(text));
       
    }
    public float ApproximateTimeToSpeach(string text){
        int words=text.Split().Length;
        float secondsPerWord=0.6f;
        return words*secondsPerWord;
    }
}
    public delegate void OnTTSPlaying(float lenghtInSeconds);
