using System;
using System.Collections;
using UnityEngine;

public class TaskManagerScript : MonoBehaviour
{
    //[SerializeField] ContextMachine StateMachine;
    
    public Task CurrentTask;
    public event Action onTaskCompleted; // Todo: Implement
    //reference to objects in gameScene

    public void StartTask(Task chosen){
        if(CurrentTask!=null){
            throw new Exception("Another task is already running!");
        }
        CurrentTask=chosen;
        CurrentTask.dataForEvaluation=new();
        CurrentTask.TaskSetup();
        

    }

    public string TaskDescription(TaskList requested){
        switch (requested)
        {
            case  TaskList.LocateProduct:
                return "in this task you will be asked to locate a specific product"; 
            default: return "";
        }
    }

    //onEventUserAcceptsToStartAfterIntroduction(){ CurrentTask.Interaction}

    public void TriggerTaskCompleted()
    {
        CurrentTask=null;
        onTaskCompleted?.Invoke();
    } 
    void handleTTS(int sec){

    }
    public void ChangeStateOnTimer(int sec, InteractionState next){
        StartCoroutine(CompleteStateAfterWait(sec,next));
    }
    IEnumerator CompleteStateAfterWait(int sec, InteractionState next){
        yield return new WaitForSeconds(sec);
        Task.interactionMachine.ChangeState(next);
    }
    
    
}

public class UserInput{
    public static event Action<string> OnUserSpoke;
    public static event Action<Vector3> OnUserMoved; // Movement action probably has more input

    public static void HandleSpeechInput(string spokenText)
    {
        OnUserSpoke?.Invoke(spokenText);
    }

    public static void HandleMovementInput(Vector3 newPosition)
    {
        OnUserMoved?.Invoke(newPosition);
    }

}

