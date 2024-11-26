


using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Task:MonoBehaviour{
    public string Description;//no need to have this here, better to have a enum
    public Task(){

    }
    protected void CompleteTask()
    {
        JobTrainingManager.instance.GetTaskManager()?.TriggerTaskCompleted();
    }

    public abstract void Introduction();
    public abstract void Interaction();
    public abstract void Feedback();


}
public enum TaskList{
    LocateProduct=1

}

public class InteractionMachine{

    private InteractionState CurrentState;
    public InteractionMachine(InteractionState first){
        CurrentState=first;
        CurrentState.Setup(JobTrainingManager.instance);
    }

    public void ChangeState(InteractionState next){
        CurrentState.Dismantle();
        CurrentState=next;
        CurrentState.Setup(JobTrainingManager.instance);
    }
}

public abstract class InteractionState{

    //logic of the state IE all from making changes to scene to adding the needed EventListeners to handle the user interaction
    public abstract void Setup(JobTrainingManager JTManager);
    //this removes eventListeners and logs eventual data for feedback reason collected (like time to respond and response lenght)
    public abstract void Dismantle();


}