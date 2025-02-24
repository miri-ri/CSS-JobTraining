using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//todo fix audio playing wrong sometimes + display all necessary feedback

public delegate void generalTimer();
public class StartValues
{
    public static string JTSaddress;
    public static bool realMR = true;
}

public class JobTrainingManager : MonoBehaviour
{

    public static JobTrainingManager instance;

    public static Vector3 roomCenter = new(0, 7, 0);
    [SerializeField] ActivityManager ActivityManager;
    [SerializeField] TaskManagerScript TaskManager;
    public LLMinterface LLM { get; private set; }
    public STTInterface speechTT { get; private set; }
    public TTSInterface TTS { get; private set; }
    public List<AreaTriggerScript> TriggerableAreas;
    public AreaTriggerScript Triggerable;
    public static string jobtrainerServer = "http://10.0.0.13:8000";
    public static bool noKinectDebug = false;

    void Awake()
    {
        instance = this;
        jobtrainerServer = "http://" + StartValues.JTSaddress;
        noKinectDebug = !StartValues.realMR;
        Debug.Log("starting using server: " + jobtrainerServer);
        Debug.Log("you are not in the magic room: " + noKinectDebug);
        TriggerableAreas = new();

        TriggerableAreas.Add(Triggerable);

        speechTT = gameObject.AddComponent<STTInterface>();
        LLM = gameObject.AddComponent<LLMinterface>();
        TTS = gameObject.AddComponent<TTSInterface>();
        HideMicrophoneFeedback();
        LLM.SystemResponseInterpreted += HideMicrophoneFeedback;
        speechTT.ListeningComplete += HideMicrophoneFeedback;
        ChangeFrontWallBackground("start");
        ToggleTextUi(false);
    }


    //used to show on the wall that the system is actively listening to the user speech

    public TaskManagerScript GetTaskManager()
    {
        return TaskManager;
    }
    public ActivityManager GetActivityManager()
    {
        return ActivityManager;
    }
    public Vector2 getUserPos()
    {
        return new(UserPosition.position.x, UserPosition.position.z);
    }


    [SerializeField] Transform UserPosition;
    [SerializeField] TextCloud TextCloudUI;
    [SerializeField] AudioSource RoomSpeakers;
    [SerializeField] GameObject SpeakerButton;
    [SerializeField] Image wall;
    [SerializeField] EvalScript FeedbackPanel;
    public PerformanceLog PerformanceLog;


    //here go all the functions that act on the scene, change background, change audio, etch

    public void ChangeFrontWallBackground(string bkgName)
    {
        Sprite bkg = Resources.Load<Sprite>("Backgrounds/sp/" + bkgName);
        if (bkg == null) Debug.LogError("missing backgeound for -> " + bkgName);
        wall.sprite = bkg;
    }
    void ShowMicrophoneFeedback()
    {
        SpeakerButton.GetComponent<CanvasGroup>().alpha = 1;
    }

    void HideMicrophoneFeedback()
    {
        SpeakerButton.GetComponent<CanvasGroup>().alpha = 0;
    }
    void HideMicrophoneFeedback(bool n)
    {
        HideMicrophoneFeedback();
    }

    public void ModifyTargetArea()
    {//enjeneiringg
        Triggerable.modifyForNextTask();
    }

    public void PlaySound(string soundName)
    {
        if (RoomSpeakers != null)
        {
            AudioClip cl = Resources.Load<AudioClip>("roomBKGNoise/" + soundName);
            if (cl == null) Debug.LogError("no clip audio -> " + soundName);
            RoomSpeakers.PlayOneShot(cl);
        }
    }
    public void StopAudioCurrentClip()
    {
        RoomSpeakers.Stop();
    }
    public void showEvaluation(EvaluationResponse eval)
    {
        FeedbackPanel.FlushPanel();
        ChangeFrontWallBackground("evaluation");
        FeedbackPanel.AddFeedbackIcons(eval);
    }
    public void hideEvaluation()
    {
        FeedbackPanel.FlushPanel();
    }
    public void WriteOnUi(string text)
    {
        TextCloudUI.WriteText(text);
    }
    public void ToggleTextUi(bool visible)
    {
        if (visible)
            TextCloudUI.ShowTextUI();
        else TextCloudUI.HideTextUI();
    }
    public void SubscribeToAreaTrigger(string areaName, OnUserEnteredArea handler)
    {
        foreach (var item in TriggerableAreas)
        {
            if (item.AreaName == areaName)
            {
                item.UserIn += handler;
                return;
            }
        }
        Debug.LogError("no area called '" + areaName + "' found");
    }
    public void UnsubscribeToAreaTrigger(string areaName, OnUserEnteredArea handler)
    {
        foreach (var item in TriggerableAreas)
        {
            if (item.AreaName == areaName)
            {
                item.UserIn -= handler;
                return;
            }
        }
    }
    public void StopJobTraining()
    {
        // todo
        // stop audio
    }

    public event generalTimer timer;

    public void SetTimer(int sec, generalTimer handler)
    {
        timer += handler;
        StartCoroutine(Timer(sec));
    }
    public void DismantleTimer(generalTimer handler)
    {
        timer -= handler;
    }
    IEnumerator Timer(int sec)
    {
        yield return new WaitForSeconds(sec);
        timer?.Invoke();
    }




    //-------TextToSpeech calls 
    public void PlayDialog(string textToTTS, OnTTSPlaying handler)
    {
        TTS.TTsPlaying += handler;
        Debug.Log("playing voice -> " + textToTTS);
        WriteOnUi(textToTTS);
        TTS.PlayAudio(textToTTS);
    }
    public void PlayDialog(string textToTTS, OnTTSPlaying handler, string voice)
    {
        TTS.TTsPlaying += handler;
        Debug.Log("playing voice -> " + textToTTS);
        WriteOnUi(textToTTS);
        TTS.PlayAudio(textToTTS, voice);
    }

       public void PlayDialog(string textToTTS, Action handler)
    {
        Debug.Log("playing voice -> " + textToTTS);
        MagicRoomManager.instance.MagicRoomTextToSpeachManager.EndSpeak+=handler;
        WriteOnUi(textToTTS);
        TTS.PlayAudio(textToTTS);
    }
    public void PlayDialog(string textToTTS, Action handler, string voice)
    {
        Debug.Log("playing voice -> " + textToTTS);
        MagicRoomManager.instance.MagicRoomTextToSpeachManager.EndSpeak+=handler;
        WriteOnUi(textToTTS);
        TTS.PlayAudio(textToTTS, voice);
    }
    public void RemoveTTShandler(OnTTSPlaying handler)
    {
        TTS.TTsPlaying -= handler;
    }


    //-------SpeechToText calls
    public void GetUserDialog(OnSTTReady handler)
    {
        Debug.Log("getting text from user voice");
        speechTT.RequestComplete += handler;
        ShowMicrophoneFeedback();
        speechTT.StartTTSListening();
    }
    public void RemoveSTThandler(OnSTTReady handler)
    {
        speechTT.RequestComplete -= handler;
    }


    //-------LLM calls
    public void GetEvaluation(DataForEvaluation dataForEvaluation, OnEvaluationReady handler)
    {
        Debug.Log("getting evaluation");
        LLM.EvaluationComplete += handler;
        LLM.evaluateDialog(dataForEvaluation);
    }
    public void RemoveEvaluationHandler(OnEvaluationReady handler)
    {
        LLM.EvaluationComplete -= handler;
    }

    public DataForEvaluation getCurrentTasksFeedbackData()
    {
        return TaskManager.CurrentTask.dataForEvaluation;
    }
    public void GenerateLLMCustomerResponse(string transcript, OnLLMresponseToUserReady handler)
    {
        Debug.Log("generating response");

        LLM.ResponseReady += handler;
        LLM.PrepareResponseToUser(transcript);
    }
    public void RemoveLLMCustomerResponse(OnLLMresponseToUserReady handler)
    {
        LLM.ResponseReady -= handler;
    }
    public void getUserWillingess(OnSystemInteractionReady handler)
    {
        LLM.SystemResponseInterpreted += handler;
        Debug.Log("analyzing user willingness");
        ShowMicrophoneFeedback();
        LLM.evaluateSystemAnswer();
    }
    public void RemoveUserWillingessHandler(OnSystemInteractionReady handler)
    {
        LLM.SystemResponseInterpreted -= handler;

    }


}


