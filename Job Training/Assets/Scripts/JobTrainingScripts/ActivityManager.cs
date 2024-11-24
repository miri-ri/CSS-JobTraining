
using System;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.iOS;


public class ActivityManager:MonoBehaviour{

    private ActivityStateMachine stateMachine;
    [SerializeField] TaskManagerScript TaskManager;
    //this class uses a state machine for the entire activity, and loads tasks

    //the following methods should be implemented inside the various activity states ( for list of activity states relate to "activity structure" in design document)
    public void StartActivity(){
        stateMachine = new ActivityStateMachine();
        stateMachine.SetState(new ExplanationOfActivity());
    }

    private void getAvailableTasks(){

    }

    //to be chosen between each task or from the start
    private void setChosenTask(){
        TaskManager.StartTask(new TaskLocateProduct());
    }
    
}

//the states in this SA handle the interaction during the activity 
public class ActivityStateMachine{

    private ActivityState currentState;
    public event Action<ActivityState> OnStateChanged;

    public void SetState(ActivityState state){
        currentState?.Dismantle();
        currentState = state;
        currentState?.Setup();

        OnStateChanged?.Invoke(currentState);
    }

}
public abstract class ActivityState{
    public abstract void Setup();
    public abstract void Dismantle();

}

//user is informed
class ExplanationOfActivity : ActivityState
{

    public override void Setup()
    {
        //show intro UI, play background audio

        throw new System.NotImplementedException();
    }
    public override void Dismantle()
    {
        throw new System.NotImplementedException();
    }
}

class TaskStartState : ActivityState
{

    public override void Setup()
    {
        throw new System.NotImplementedException();
    }
    public override void Dismantle()
    {
        throw new System.NotImplementedException();
    }
}

//other states: Activity Start, Task Start(uses the taskManager), end of task,
// waitingState, advancingTask, assesProblem, repeatActivity(basically just task start w/ same task)