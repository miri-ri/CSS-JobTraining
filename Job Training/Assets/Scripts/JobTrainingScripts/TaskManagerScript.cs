using System;
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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
   //     StateMachine.HandleStateLogic();
    }

    public void TriggerTaskCompleted()
    {
        CurrentTask=null;
        onTaskCompleted?.Invoke();
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

