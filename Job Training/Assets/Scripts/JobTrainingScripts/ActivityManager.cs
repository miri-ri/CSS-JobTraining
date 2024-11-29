
using System;
using System.Threading;
using UnityEngine;

public class ActivityManager:MonoBehaviour{

    private ActivityStateMachine stateMachine;
    private TaskManagerScript TaskManager;

    void Start(){
        TaskManager=JobTrainingManager.instance.GetTaskManager();
        if(TaskManager == null){
            throw new ArgumentNullException(nameof(TaskManager), "TaskManager not asigned!");
        }
        

        stateMachine = new ActivityStateMachine(TaskManager);

        stateMachine.SetState(new ExplanationOfActivity(stateMachine, TaskManager));
    }

    //this class uses a state machine for the entire activity, and loads tasks

    //the following methods should be implemented inside the various activity states ( for list of activity states relate to "activity structure" in design document)

    private void getAvailableTasks(){
        
    }

    //to be chosen between each task or from the start
    private void setChosenTask(){
        //TaskManager.StartTask(new TaskLocateProduct());
    }

    
}

//the states in this SA handle the interaction during the activity 
public class ActivityStateMachine{

    private ActivityState currentState;
    private TaskManagerScript taskManager;

    public ActivityStateMachine(TaskManagerScript TaskManager){
        taskManager = TaskManager;
    }

    public void SetState(ActivityState state){
        currentState?.Dismantle();
        currentState = state;
        currentState?.Setup();
    }
    
    public void CompleteState(ActivityState nextState = null){
        switch(currentState) {
        case ExplanationOfActivity:
            SetState(new TaskState(this, taskManager));
            break;
        case TaskState:
            SetState(new TaskCompleteState(this, taskManager));
            break;
        case TaskCompleteState:
            SetState(nextState);
            break;
        case WaitingState:
            SetState(new TaskCompleteState(this, taskManager));
            break;
        default:
            throw new System.NotImplementedException();
        }
    }
}

public abstract class ActivityState{

    public ActivityStateMachine stateMachine;
    public TaskManagerScript taskManager;

    protected ActivityState(ActivityStateMachine machine, TaskManagerScript TaskManager)
    {
        stateMachine = machine;
        taskManager = TaskManager;
    }

    public abstract void Setup();
    public abstract void Dismantle();

}

//user is informed
class ExplanationOfActivity : ActivityState
{
    public ExplanationOfActivity(ActivityStateMachine machine, TaskManagerScript TaskManager) : base(machine, TaskManager)
    {}

    public override void Setup()
    {
        // Todo: Show intro UI
        JobTrainingManager.instance.WriteOnUi("testtesttesttesttest");

        // Todo: Start background audio
        JobTrainingManager.instance.PlaySound();
        // await trainer task selection

        stateMachine.CompleteState();
    }

    public override void Dismantle()
    {
        //throw new System.NotImplementedException();
    }
}

class TaskState : ActivityState
{
    public TaskState(ActivityStateMachine machine, TaskManagerScript TaskManager) : base(machine, TaskManager)
    {}

    public override void Setup()
    {
        taskManager.StartTask(new TaskLocateProduct()); // Todo: add task choice input here
        taskManager.onTaskCompleted += CompleteTask; // onTaskCompleted only triggered when no problem appeared
    }

    private void CompleteTask(){
        taskManager.onTaskCompleted -= CompleteTask;
        stateMachine.CompleteState();
    }

    public override void Dismantle()
    {
        taskManager.onTaskCompleted -= CompleteTask;
    }
}

class TaskCompleteState : ActivityState
{
    public TaskCompleteState(ActivityStateMachine machine, TaskManagerScript TaskManager) : base(machine, TaskManager)
    {}

    public override void Dismantle()
    {
        throw new NotImplementedException();
    }

    public override void Setup()
    {
        JobTrainingManager.instance.WriteOnUi("Do you want to proceed to the next task, take a break or stop the activity?");
        string userInput = "";// user selection input
        if(userInput=="next"){
            stateMachine.CompleteState(new TaskState(stateMachine, new TaskManagerScript()));
        } else if (userInput == "wait"){
            stateMachine.CompleteState(new WaitingState(stateMachine, taskManager));
        } else if (userInput == "stop"){
            stateMachine.CompleteState(new StopActivity(stateMachine, taskManager));
        } else {
            throw new ArgumentException("TaskCompleteState: invalid user selection");
        }
    }
}

class WaitingState : ActivityState
{
    public WaitingState(ActivityStateMachine machine, TaskManagerScript TaskManager) : base(machine, TaskManager)
    {}

    public override void Dismantle()
    {
        throw new NotImplementedException();
    }

    public override void Setup()
    {
        JobTrainingManager.instance.WriteOnUi("Alright, let's take a short break of 3 minutes!");
        Thread.Sleep(3*1000); // maybe async needed?
        stateMachine.CompleteState();
    }
}

class StopActivity : ActivityState
{
    public StopActivity(ActivityStateMachine machine, TaskManagerScript TaskManager) : base(machine, TaskManager)
    {}

    public override void Dismantle()
    {
        throw new NotImplementedException();
    }

    public override void Setup()
    {
        JobTrainingManager.instance.WriteOnUi("Goodbye!");
        // final logging (wait till finished)
        JobTrainingManager.instance.StopJobTraining();

    }
}