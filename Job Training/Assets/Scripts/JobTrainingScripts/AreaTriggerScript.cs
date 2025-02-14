using System;
using UnityEngine;
public class AreaTriggerScript : MonoBehaviour
{
    public string AreaName;
    public event OnUserEnteredArea UserIn;
    private void OnTriggerEnter(Collider other){
        //send event entered area
        Debug.Log("entered area "+AreaName);
        UserIn?.Invoke(new(other.transform.position.x,other.transform.position.z),new(transform.position.x,transform.position.z), DateTime.Now);
    }
    public void OnTriggerExit(){
        //send event out of area
        Debug.Log("exited area "+AreaName);
    }
    void Start(){
        //JobTrainingManager.instance.TriggerableAreas.Add(this);
    }

}
public delegate void OnUserEnteredArea(Vector2 userPosition, Vector2 targetPosition, DateTime arrival);
