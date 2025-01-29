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

        CurrentTask = CreateTask(chosen);
        CurrentTask.dataForEvaluation=new();
        CurrentTask.TaskSetup();
    }

    public string TaskDescription(TaskList requested){
        switch (requested)
        {
            case  TaskList.LocateProduct:
                return "In this task you will be asked to locate a specific product."; 
            case TaskList.ShowInfopoint:
                return "In this task, a customer approaches you and wants to report an expired product. You will have to guide them to the infopoint.";
            default: return "Unknown Task";
        }
    }

    private Task CreateTask(TaskList taskType){
        return taskType switch {
            TaskList.LocateProduct => new TaskLocateProduct(),
            TaskList.ShowInfopoint => new TaskShowInfopoint(),
            _ => throw new Exception("Unknown task type!")
        };
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

