using System.Collections.Generic;
using UnityEngine;



public class JobTrainingManager:MonoBehaviour{

    public static JobTrainingManager instance;
    public static Vector3 roomCenter=new (0,7,412);
    [SerializeField] ActivityManager ActivityManager;
    [SerializeField] TaskManagerScript TaskManager;
    public LLMinterface LLM{get;private set;}
    public STTInterface speechTT{get;private set;}
    public TTSInterface TTS{get; private set;}
    public List<AreaTriggerScript> TriggerableAreas;

    void Awake(){
        instance=this;
        TriggerableAreas=new();
        FeedbackUIRef.ToggleHide(); 

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
    [SerializeField] Transform ClientPosition;
    [SerializeField] GameObject FrontWall,Floor;
    [SerializeField] TextCloud TextCloudUI;
    [SerializeField] FeedbackUI FeedbackUIRef;
    [SerializeField] AudioSource RoomSpeakers;
    [SerializeField] GameObject SpeakerButton;
    public PerformanceLog PerformanceLog;

    //here go all the functions that act on the scene, change background, change audio, etch
    public void ChangeFrontWallBackground(string bkgName){
        Renderer ren= FrontWall.GetComponent<Renderer>();
        Material backG=Resources.Load<Material>("Backgrounds/"+bkgName);
        if(backG==null){
            throw new System.Exception("backg: "+bkgName+" not found");
        }
        ren.material=backG;
    }
    void ShowMicrophoneFeedback(){
        SpeakerButton.GetComponent<CanvasGroup>().alpha=1;
    }   

    void HideMicrophoneFeedback(){
        SpeakerButton.GetComponent<CanvasGroup>().alpha=0;
    }
    public void PlaySound(){
        if(RoomSpeakers != null){
            RoomSpeakers.PlayOneShot((AudioClip)Resources.Load("supermarket-17823"));;
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
    public void StopJobTraining(){
        // stop audio
    }
    
  
//-------TextToSpeech calls 
    public void PlayDialog(string textToTTS, OnTTSPlaying handler){
        TTS.TTsPlaying+=handler;
        Debug.Log("playing voice "+textToTTS);
        TTS.PlayAudio(textToTTS);
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
    
    
}


