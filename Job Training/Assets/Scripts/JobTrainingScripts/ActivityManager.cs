
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class ActivityManager:MonoBehaviour{

    private ActivityStateMachine stateMachine;
    private TaskManagerScript TaskManager;

    public void Start(){
        TaskManager=JobTrainingManager.instance.GetTaskManager();
        if(TaskManager == null){
            throw new ArgumentNullException(nameof(TaskManager), "TaskManager not asigned!");
        }

        stateMachine = new ActivityStateMachine();
        stateMachine.SetState(new ExplanationOfActivity());
    }

    //this class uses a state machine for the entire activity, and loads tasks

    //the following methods should be implemented inside the various activity states ( for list of activity states relate to "activity structure" in design document)

    private void getAvailableTasks(){
        
    }

    //to be chosen between each task or from the start
    private void setChosenTask(){
        //TaskManager.StartTask(new TaskLocateProduct());
    }

    public ActivityStateMachine GetActivityStateMachine(){
        return stateMachine;
    }
    
}

//the states in this SA handle the interaction during the activity 
public class ActivityStateMachine {

    private ActivityState currentState;

    public void SetState(ActivityState state){
        currentState?.Dismantle();
        currentState = state;
        currentState?.Setup();
    }

    public void CompleteState(ActivityState nextState = null, int delaySeconds = 0)
    {
        if (delaySeconds > 0)
        {
            JobTrainingManager.instance.GetActivityManager()
                .StartCoroutine(CompleteStateAfterWait(delaySeconds, nextState));
        }
        else
        {
            SetNextState(nextState);
        }
    }
    
    public void SetNextState(ActivityState nextState = null){
        switch(currentState) {
        case ExplanationOfActivity:
            SetState(new TaskState());
            break;
        case TaskState:
            SetState(new TaskCompleteState());
            break;
        case TaskCompleteState:
            if(nextState==null){
                throw new Exception("Change state: invalid next state!");
            }
            SetState(nextState);
            break;
        case WaitingState:
            SetState(new TaskCompleteState());
            break;
        default:
            throw new System.NotImplementedException();
        }
    }

    public IEnumerator CompleteStateAfterWait(int waitingSeconds, ActivityState nextState = null){
        Debug.Log($"Waiting for {waitingSeconds} seconds before changing state.");
        yield return new WaitForSeconds(waitingSeconds);
        if(nextState==null){SetNextState();} else {SetNextState(nextState);}
    }
}

public abstract class ActivityState {

    public ActivityStateMachine stateMachine;
    public TaskManagerScript taskManager;

    protected ActivityState()
    {
        stateMachine = JobTrainingManager.instance.GetActivityManager().GetActivityStateMachine();
        taskManager = JobTrainingManager.instance.GetTaskManager();
    }

    public abstract void Setup();
    public abstract void Dismantle();

}

//user is informed
class ExplanationOfActivity : ActivityState
{
    public override void Setup()
    {
        // Todo: Show intro UI
        JobTrainingManager.instance.WriteOnUi("testtesttesttesttest");

        // Todo: Start background audio
        JobTrainingManager.instance.PlaySound();
        // await trainer task selection
        
        stateMachine.CompleteState(null, 5);
    }


    public override void Dismantle()
    {
        //throw new System.NotImplementedException();
    }
}
class TaskState : ActivityState
{

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

    public override void Dismantle()
    {
        throw new NotImplementedException();
    }

    public override void Setup()
    {
        JobTrainingManager.instance.WriteOnUi("Do you want to proceed to the next task, take a break or stop the activity?");
        string userInput = "";// user selection input
        if(userInput=="next"){
            stateMachine.CompleteState(new TaskState());
        } else if (userInput == "wait"){
            stateMachine.CompleteState(new WaitingState());
        } else if (userInput == "stop"){
            stateMachine.CompleteState(new StopActivity());
        } else {
            throw new ArgumentException("TaskCompleteState: invalid user selection");
        }
    }
}

class WaitingState : ActivityState
{

    public override void Dismantle()
    {
        throw new NotImplementedException();
    }

    public override void Setup()
    {
        JobTrainingManager.instance.WriteOnUi("Alright, let's take a short break of 3 minutes!");
        // continue instead of timer
        //Thread.Sleep(3*1000); // maybe async needed?
        stateMachine.CompleteState(null, 3*60);
    }
}

class StopActivity : ActivityState
{

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