using System;
using System.Collections;
using UnityEngine;

public class TaskManagerScript : MonoBehaviour
{
    //[SerializeField] ContextMachine StateMachine;
    
    public Task CurrentTask;
    public event Action onTaskCompleted; // Todo: Implement
    //reference to objects in gameScene

    public void StartTask(TaskList chosen){
        if(CurrentTask!=null){
            throw new Exception("Another task is already running!");
        }

        Debug.Log($"Starting Task: {chosen}");

        switch (chosen){
            case TaskList.LocateProduct:
                CurrentTask = new TaskLocateProduct();
                break;
            default:
                throw new Exception("Unknown task type!");
        }
        CurrentTask.dataForEvaluation=new();
        CurrentTask.TaskSetup();
        

    }

    public string TaskDescription(TaskList requested){
        switch (requested)
        {
            case  TaskList.LocateProduct:
                return "In this Task you will be asked to locate a specific product."; 
            default: return "Unknown Task";
        }
    }

    //onEventUserAcceptsToStartAfterIntroduction(){ CurrentTask.Interaction}

    public void TriggerTaskCompleted()
    {
        Debug.Log("Task completed!");
        CurrentTask=null;
        onTaskCompleted?.Invoke();
    } 

    public void ChangeStateOnTimer(float sec, InteractionState next){
        
        Debug.Log($"Waiting {sec} seconds before going to {next}");
        StartCoroutine(CompleteStateAfterWait(sec,next));
    }

    private IEnumerator CompleteStateAfterWait(float sec, InteractionState next){
        yield return new WaitForSeconds(sec);
        Task.interactionMachine.ChangeState(next);
    }
    
    
}

