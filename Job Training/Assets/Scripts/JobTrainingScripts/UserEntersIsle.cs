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
        Debug.Log("entratooo");
        RefManager.WriteOnBubble("utente in Isle");
        RefManager.userInRightIsle=true;
    }
    private void OnTriggerExit(Collider other){
        
       // RefManager.WriteOnBubble("utente in Isle");
        RefManager.userInRightIsle=false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
