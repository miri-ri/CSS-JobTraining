using Unity.VisualScripting;
using UnityEngine;

public class UserEntersIsle : MonoBehaviour
{

    public GameObject taskManager;
    public ProtoTaskManager RefManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other){
        
        RefManager.WriteOnBubble("User in Isle");
        RefManager.userInRightIsle=true;
    }
    private void OnTriggerExit(Collider other){
        
        RefManager.WriteOnBubble("User out of Isle");
        RefManager.userInRightIsle=false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
