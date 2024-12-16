using UnityEngine;

public class AreaTriggerScript : MonoBehaviour
{
    public OnUserEnteredArea userIN;
    public string AreaName;
    public Event AreaEnterEvent;
    public Event AreaEnterExit;
    public void OnTriggerEnter(){
        //send event entered area
        Debug.Log("entered area "+AreaName);
        userIN.Invoke();
    }
    public void OnTriggerExit(){
        //send event out of area
        Debug.Log("exited area "+AreaName);
    }

}
public delegate void OnUserEnteredArea();
