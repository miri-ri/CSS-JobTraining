
using System.Drawing.Text;
using UnityEngine;


public class ActivityManager:MonoBehaviour{

    [SerializeField] TaskManagerScript TaskManager;
    //this class uses a state machine for the entire activity, and loads tasks

    //the following methods should be implemented inside the various activity states ( for list of activity states relate to "activity structure" in design document)


    private void getAvailableTasks(){

    }
    //to be chosen between each task or from the start
    private void setChosenTask(){
        TaskManager.StartTask(new TaskLocateProduct() );
    }
    
}

//the states in this SA handle the interaction during the activity 
public class ActivityStateMachine{

}
abstract class ActivityState{
    public abstract void Setup();
    public abstract void Dismantle();

}

//user is informed
class ExplanationOfActivity : ActivityState
{
    public override void Dismantle()
    {
        throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        throw new System.NotImplementedException();
    }
}

//other states: Activity Start, Task Start(uses the taskManager), end of task,
// waitingState, advancingTask, assesProblem, repeatActivity(basically just task start w/ same task)