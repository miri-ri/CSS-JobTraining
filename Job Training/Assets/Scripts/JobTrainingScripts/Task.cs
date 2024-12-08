


using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Task{
    public string Description;//no need to have this here, better to have a enum
    private static InteractionMachine interactionMachine;

    protected void CompleteTask()
    {
        JobTrainingManager.instance.GetTaskManager()?.TriggerTaskCompleted();
    }

    public abstract void TaskSetup();
    public abstract void Feedback();

    public static void SetInteractionMachine(InteractionMachine InteractionMachine){
        interactionMachine = InteractionMachine;
        return;
    }
    public static InteractionMachine GetInteractionMachine(){
        return interactionMachine;
    }

}
public enum TaskList{
    LocateProduct=1

}

public class InteractionMachine{

    private InteractionState CurrentState;

    public void ChangeState(InteractionState next){
        CurrentState?.Dismantle();
        CurrentState=next;
        CurrentState.Setup();
    }
}

public abstract class InteractionState{

    //logic of the state IE all from making changes to scene to adding the needed EventListeners to handle the user interaction
    public abstract void Setup();
    //this removes eventListeners and logs eventual data for feedback reason collected (like time to respond and response lenght)
    public abstract void Dismantle();


}