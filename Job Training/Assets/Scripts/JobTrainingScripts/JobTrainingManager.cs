using UnityEngine;



public class JobTrainingManager:MonoBehaviour{

    public static JobTrainingManager instance;
    public static Vector3 roomCenter=new (0,7,412);
    [SerializeField] ActivityManager ActivityManager;
    [SerializeField] TaskManagerScript TaskManager;

   
    void Awake(){
        instance=this;
        FeedbackUIRef.HideFeedbackUI(); 
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
   // private GameObject txtCloud;

    //here go all the functions that act on the scene, change background, change audio, etch
    public void ChangeFrontWallBackground(string bkgName){
        Renderer ren= FrontWall.GetComponent<Renderer>();
        Material backG=Resources.Load<Material>("Backgrounds/"+bkgName);
        if(backG==null){
            throw new System.Exception("backg: "+bkgName+" not found");
        }
        ren.material=backG;
    }
    public void PlaySound(){
        //call TTS api, which plays text to audio
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
}


