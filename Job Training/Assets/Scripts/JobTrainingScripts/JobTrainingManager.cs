using Unity.VisualScripting;
using UnityEngine;



public class JobTrainingManager:MonoBehaviour{

    public static JobTrainingManager instance;
    public static Vector3 roomCenter=new (0,12,410);
    [SerializeField] ActivityManager ActivityManager;
    [SerializeField] TaskManagerScript TaskManager;

    void Start(){
        instance=this;
        ActivityManager.StartActivity();
    }

    [SerializeField] GameObject FrontWall,Floor;
    [SerializeField] AudioSource RoomSpeaker;
    [SerializeField] GameObject TextCloudPrefab;
   // private GameObject txtCloud;

    //here go all the functions that act on the scene, change background, change audio, etch
    public void ChangeFrontWallBackground(string bkgName){
        Renderer ren= FrontWall.GetComponent<Renderer>();
        ren.material=Resources.Load<Material>("background/background2");
    }
    public void PlaySound(){
        //streaming audio seems a bit sketchy in unity
    }

    public void WriteOnUi(string text){
        TextCloudPrefab.GetComponent<TextCloud>().WriteText(text);
    }

}


