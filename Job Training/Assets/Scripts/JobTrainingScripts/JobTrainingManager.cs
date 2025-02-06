using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//todo fix audio playing wrong sometimes + display all necessary feedback
public class Targets{//make it task dependent
    public float speechDuration;
    public float speechBeforeStart;
    public Position MovementTarget;
    public float MovementOkRadius;

    public Targets(float sD, float sB, Vector2 mT, float kR)
    {
        speechDuration=sD;
        speechBeforeStart=sB;
        MovementTarget=new(mT);
        MovementOkRadius=kR;
    }
}
public delegate void generalTimer();

public class JobTrainingManager:MonoBehaviour{

    public static JobTrainingManager instance;
    public static bool noKinectDebug=false;
    public static Targets EvaluationTargets=new(5,2,new(1,1), 1);//for only locate task todo
    public static Vector3 roomCenter=new (0,7,0);
    [SerializeField] ActivityManager ActivityManager;
    [SerializeField] TaskManagerScript TaskManager;
    public LLMinterface LLM{get;private set;}
    public STTInterface speechTT{get;private set;}
    public TTSInterface TTS{get; private set;}
    public List<AreaTriggerScript> TriggerableAreas;
    public AreaTriggerScript Triggerable;


    void Awake(){
        instance=this;
        TriggerableAreas=new();
        FeedbackUIRef.ToggleHide();
        TriggerableAreas.Add(Triggerable);
        Debug.Log("esiste??? " + Triggerable.AreaName);
        foreach(var it in TriggerableAreas)
         Debug.Log("areassssssss>>>" + it.AreaName);
        speechTT=gameObject.AddComponent<STTInterface>();
        LLM=gameObject.AddComponent<LLMinterface>();
        TTS=gameObject.AddComponent<TTSInterface>();

        HideMicrophoneFeedback();
        speechTT.ListeningComplete+=HideMicrophoneFeedback;

    }
    
    //used to show on the wall that the system is actively listening to the user speech

    public TaskManagerScript GetTaskManager(){
        return TaskManager;
    }
    public ActivityManager GetActivityManager(){
        return ActivityManager;
    }
    public Vector2 getUserPos(){
        return new(UserPosition.position.x, UserPosition.position.z);
    }


    [SerializeField] Transform UserPosition;
    [SerializeField] TextCloud TextCloudUI;
    [SerializeField] FeedbackUI FeedbackUIRef;
    [SerializeField] AudioSource RoomSpeakers;
    [SerializeField] GameObject SpeakerButton;
    [SerializeField] Image wall;
    public PerformanceLog PerformanceLog;


    //here go all the functions that act on the scene, change background, change audio, etch
    
    public void ChangeFrontWallBackground(string bkgName){
        Sprite bkg= Resources.Load <Sprite>("Backgrounds/sp/"+bkgName);
        if(bkg==null) Debug.LogError("missing backgeound for -> "+bkgName);
        wall.sprite=bkg;
    }
    void ShowMicrophoneFeedback(){
        SpeakerButton.GetComponent<CanvasGroup>().alpha=1;
    }   

    void HideMicrophoneFeedback(){
        SpeakerButton.GetComponent<CanvasGroup>().alpha=0;
    }
    public void PlaySound(string soundName){
        if(RoomSpeakers != null){
            AudioClip cl=Resources.Load<AudioClip>("roomBKGNoise/"+soundName);
            if(cl==null)Debug.LogError("no clip audio -> "+soundName);
            RoomSpeakers.PlayOneShot(cl);
        }
    }

    public void WriteOnUi(string text){
        TextCloudUI.WriteText(text);
    }
    public void ShowFeedbackMessages(string feedbackMessage){//todo graphical display for multple points
        FeedbackUIRef.ToggleHide();
        FeedbackUIRef.setFeedback(feedbackMessage);

    }
    public void SubscribeToAreaTrigger(string areaName, OnUserEnteredArea handler){
        foreach (var item in TriggerableAreas){
            if(item.AreaName==areaName){
                item.UserIn+=handler;
                return;
            }
        }
        Debug.LogError("no area called '"+areaName+"' found");
    }
    public void UnsubscribeToAreaTrigger(string areaName,OnUserEnteredArea handler){
        foreach (var item in TriggerableAreas){
            if(item.AreaName==areaName){
                item.UserIn-=handler;
                return;
            }
        }
    }
    public void StopJobTraining(){
        // stop audio
    }

    public generalTimer timer;

    public void SetTimer(int sec, generalTimer handler){
        timer+=handler;
        StartCoroutine(Timer(sec));
    }
    public void DismantleTimer(generalTimer handler){
        timer-=handler;
    }
    IEnumerator Timer(int sec){
        yield return new WaitForSeconds(sec);
        timer.Invoke();
    }
    
    
    
  
//-------TextToSpeech calls 
    public void PlayDialog(string textToTTS, OnTTSPlaying handler){
        TTS.TTsPlaying+=handler;
        Debug.Log("playing voice "+textToTTS);
        WriteOnUi(textToTTS);
        TTS.PlayAudio(textToTTS);//warning tts non instaziato
    }
    public void RemoveTTShandler(OnTTSPlaying handler){
        TTS.TTsPlaying-=handler;
    }
  

//-------SpeachToText calls
    public void GetUserDialog(OnSTTReady handler){
        Debug.Log("getting text from user voice");
        speechTT.RequestComplete+=handler;
        ShowMicrophoneFeedback();
        speechTT.StartTTSListening();
    }
    public void RemoveSTThandler(OnSTTReady handler){
        speechTT.RequestComplete-=handler;  
    }


//-------LLM calls
    public void GetEvaluation(DataForEvaluation dataForEvaluation,  OnEvaluationReady handler)
    {
        Debug.Log("getting evaluation");
        LLM.EvaluationComplete+=handler;
        LLM.evaluateDialog(dataForEvaluation);
    }  
    public void RemoveEvaluationHandler(OnEvaluationReady handler){
        LLM.EvaluationComplete-=handler;  
    }  

    public DataForEvaluation getCurrentTasksFeedbackData(){
        return TaskManager.CurrentTask.dataForEvaluation;
    }
    public void GenerateLLMCustomerResponse(string transcript, OnLLMresponseToUserReady handler){
                Debug.Log("generating response");

        LLM.ResponseReady+=handler;
        LLM.PrepareResponseToUser(transcript);
    }
    public void RemoveLLMCustomerResponse(OnLLMresponseToUserReady handler){
        LLM.ResponseReady-=handler;
    }
    public void getUserWillingess(OnSystemInteractionReady handler){
        LLM.SystemResponseInterpreted+=handler;
        LLM.evaluateSystemAnswer();
    }
    public void RemoveUserWillingessHandler(OnSystemInteractionReady handler){
        LLM.SystemResponseInterpreted-=handler;

    }
    
    
}


