using UnityEngine;

public class TaskManagerScript : MonoBehaviour
{
    //[SerializeField] ContextMachine StateMachine;
    
    public Task CurrentTask;
    //reference to objects in gameScene

    public void StartTask(Task chosen){
        CurrentTask=chosen;
        CurrentTask.Introduction();

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

        
}



