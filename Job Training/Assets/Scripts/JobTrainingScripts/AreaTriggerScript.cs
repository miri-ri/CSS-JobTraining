using UnityEngine;

public class AreaTriggerScript : MonoBehaviour
{

    public string AreaName;
    public Event AreaEnterEvent;
    public Event AreaEnterExit;
    public void OnTriggerEnter(){
        //send event entered area
        Debug.Log("entered area "+AreaName);
    }
    public void OnTriggerExit(){
        //send event out of area
        Debug.Log("exited area "+AreaName);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
