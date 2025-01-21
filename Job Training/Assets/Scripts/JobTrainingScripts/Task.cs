
using System;
using System.Collections;
using UnityEngine;

public abstract class Task{
    public static InteractionMachine interactionMachine;
    public DataForEvaluation dataForEvaluation;

    protected void CompleteTask()
    {
        Debug.Log($"Task completed.");
        JobTrainingManager.instance.GetTaskManager()?.TriggerTaskCompleted();
    }

    public abstract void TaskSetup();

    public abstract void Feedback();

    public static void SetInteractionMachine(InteractionMachine InteractionMachine){
        interactionMachine = InteractionMachine;
    }
    public InteractionMachine GetInteractionMachine(){
        return interactionMachine;
    }

}   
public enum TaskList{
    LocateProduct=1

}

public class InteractionMachine {

    private InteractionState CurrentState;

    public void ChangeState(InteractionState next){
        Debug.Log($"InteractionMachine: Changing from {CurrentState?.GetType().Name} to {next?.GetType().Name}");
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