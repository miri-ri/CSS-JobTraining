
using System;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.iOS;


public class ActivityManager:MonoBehaviour{

    private ActivityStateMachine stateMachine;
    [SerializeField] TaskManagerScript TaskManager;
    //this class uses a state machine for the entire activity, and loads tasks

    //the following methods should be implemented inside the various activity states ( for list of activity states relate to "activity structure" in design document)
    public void StartActivity(){
        if(TaskManager == null){
            throw new ArgumentNullException(nameof(TaskManager), "TaskManager not asigned!");
        }

        stateMachine = new ActivityStateMachine();
        stateMachine.OnActivityStateComplete += HandleActivityStateCompleted;

        stateMachine.SetState(new ExplanationOfActivity(stateMachine));
    }

    private void getAvailableTasks(){
        
    }

    //to be chosen between each task or from the start
    private void setChosenTask(){
        TaskManager.StartTask(new TaskLocateProduct());
    }

    private void HandleActivityStateCompleted(ActivityState state){
        switch(state) {
        case ExplanationOfActivity:
            stateMachine.SetState(new TaskState(stateMachine, TaskManager));
            break;
        case TaskState:
            break;
        default:
            throw new System.NotImplementedException();
        }
    }
    
}

//the states in this SA handle the interaction during the activity 
public class ActivityStateMachine{

    private ActivityState currentState;
    public event Action<ActivityState> OnActivityStateComplete;

    public void SetState(ActivityState state){
        currentState?.Dismantle();
        currentState = state;
        currentState?.Setup();
    }

    public void CompleteState(){
        OnActivityStateComplete?.Invoke(currentState);
    }

}
public abstract class ActivityState{
    public abstract void Setup();
    public abstract void Dismantle();

}

//user is informed
class ExplanationOfActivity : ActivityState
{

    private ActivityStateMachine stateMachine;

    public ExplanationOfActivity(ActivityStateMachine machine){
        stateMachine = machine;
    }

    public override void Setup()
    {
        // Todo: Show intro UI
        JobTrainingManager.instance.WriteOnUi("testtesttesttesttest");

        // Todo: Start background audio
        JobTrainingManager.instance.PlaySound();
        JobTrainingManager.instance.ChangeFrontWallBackground("background1");
        // await trainer task selection

        stateMachine.CompleteState();
    }

    public override void Dismantle()
    {
        throw new System.NotImplementedException();
    }
}

class TaskState : ActivityState
{
    private ActivityStateMachine stateMachine;
    private TaskManagerScript taskManager;

    public TaskState(ActivityStateMachine machine, TaskManagerScript TaskManager){
        stateMachine = machine;
        taskManager = TaskManager;
    }

    public override void Setup()
    {
        taskManager.StartTask(new TaskLocateProduct()); // Todo: add task choice input here
        taskManager.onTaskCompleted += HandleTaskComplete;
    }

    private void HandleTaskComplete(){
        taskManager.onTaskCompleted -= HandleTaskComplete;
        stateMachine.CompleteState();
    }

    public override void Dismantle()
    {
        taskManager.onTaskCompleted -= HandleTaskComplete;
    }
}

//other states: Activity Start, Task Start(uses the taskManager), end of task,
// waitingState, advancingTask, assesProblem, repeatActivity(basically just task start w/ same task)