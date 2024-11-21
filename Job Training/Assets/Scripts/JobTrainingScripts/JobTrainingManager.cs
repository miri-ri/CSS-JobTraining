using Unity.VisualScripting;
using UnityEngine;



public class JobTrainingManager:MonoBehaviour{

    public static JobTrainingManager instance;
    private ActivityManager ActivityManager;
    private TaskManagerScript TaskManager;

    void Start(){
        ActivityManager=new ActivityManager();
        TaskManager=new TaskManagerScript();
    }

    GameObject FrontWall,Floor;
    AudioSource RoomSpeaker;
    

    //here go all the functions that act on the scene, change background, change audio, etch
    public void ChangeFrontWallBackground(){
        
    }
//etc
}
