

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
public class TTSInterface : MonoBehaviour
{
    public event OnTTSPlaying TTsPlaying;
    public void PlayAudio(string text)
    { //lookup how it works with Secco once ready
        MagicRoomManager.instance.MagicRoomTextToSpeachManager.GenerateAudioFromText(text);
        TTsPlaying?.Invoke(ApproximateTimeToSpeach(text));

    }
    public void PlayAudio(string text, string voice = "Magika")
    {
        //foreach(Voices v in MagicRoomManager.instance.MagicRoomTextToSpeachManager.ListOfVoice)
        //  Debug.Log(v.alias);
        Voices vv = MagicRoomManager.instance.MagicRoomTextToSpeachManager.ListOfVoice.FirstOrDefault(x => x.alias == voice);
        MagicRoomManager.instance.MagicRoomTextToSpeachManager.GenerateAudioFromText(text, vv);
        TTsPlaying?.Invoke(ApproximateTimeToSpeach(text));
    }
    public float ApproximateTimeToSpeach(string text)
    {
        int words = text.Split().Length;
        float secondsPerWord = 0.6f;
        return words * secondsPerWord + 4;
    }
}
public delegate void OnTTSPlaying(float lenghtInSeconds);
