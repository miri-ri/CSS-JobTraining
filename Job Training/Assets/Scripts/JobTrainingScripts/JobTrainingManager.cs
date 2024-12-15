using UnityEngine;



public class JobTrainingManager:MonoBehaviour{

    public static JobTrainingManager instance;
    public static Vector3 roomCenter=new (0,7,412);
    [SerializeField] ActivityManager ActivityManager;
    [SerializeField] TaskManagerScript TaskManager;
    public LLMinterface LLM{get;private set;}
    public STTInterface speechTT{get;private set;}
    public TTSInterface TTS{get; private set;}
   
    void Awake(){
        instance=this;
        FeedbackUIRef.HideFeedbackUI(); 
        speechTT=gameObject.AddComponent<STTInterface>();
        LLM=gameObject.AddComponent<LLMinterface>();
        TTS=gameObject.AddComponent<TTSInterface>();
    }

    public TaskManagerScript GetTaskManager(){
        return TaskManager;
    }
    public ActivityManager GetActivityManager(){
        return ActivityManager;
    }


    [SerializeField] Transform UserPosition;
    [SerializeField] Transform ClientPosition;
    [SerializeField] GameObject FrontWall,Floor;
    //[SerializeField] AudioSource RoomSpeaker;
    [SerializeField] TextCloud TextCloudUI;
    [SerializeField] FeedbackUI FeedbackUIRef;
    [SerializeField] AudioSource RoomSpeakers;

   // private GameObject txtCloud;

    //here go all the functions that act on the scene, change background, change audio, etch
    public PerformanceLog PerformanceLog;
    public void ChangeFrontWallBackground(string bkgName){
        Renderer ren= FrontWall.GetComponent<Renderer>();
        Material backG=Resources.Load<Material>("Backgrounds/"+bkgName);
        if(backG==null){
            throw new System.Exception("backg: "+bkgName+" not found");
        }
        ren.material=backG;
    }
    public void PlaySound(){
        RoomSpeakers.Play();
    }

    public void WriteOnUi(string text){
        TextCloudUI.WriteText(text);
    }
    public TextCloud GetTextCloudUi(){
        return TextCloudUI;
    }
    public FeedbackUI GetFeedbackUI(){
        return FeedbackUIRef;
    }
    
    public void StopJobTraining(){
        // stop audio
    }
      public void PlayDialog(string textToTTS, OnTTSPlaying handler){
        TTS.TTsPlaying+=handler;
        TTS.PlayAudio(textToTTS);
    }
    public void RemoveTTShandler(OnTTSPlaying handler){
        TTS.TTsPlaying-=handler;
       
    }
  


    public void GetUserDialog(OnSTTReady handler){
        speechTT.RequestComplete+=handler;
        speechTT.GetUserDialog();
    }
    public void RemoveSTThandler(OnSTTReady handler){
        speechTT.RequestComplete-=handler;  
    }



    public void GetEvaluation(DataForEvaluation dataForEvaluation,  OnEvaluationReady handler)
    {
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
        LLM.ResponseReady+=handler;
        LLM.PrepareResponseToUser(transcript);
    }
    public void RemoveLLMCustomerResponse(OnLLMresponseToUserReady handler){
        LLM.ResponseReady-=handler;
    }
    
    
}


