using System;

public class TaskShowInfopoint : Task
{
    protected override TaskList GetTaskType() => TaskList.ShowInfopoint;
    protected override string GetInitialDialog() => "Hello! Can you show me the Info Box?";
    public override string GetAreaTrigger() => "locateTask"; 
    public TaskShowInfopoint(){
        if (JobTrainingManager.instance == null){
            throw new Exception("The job training manager isn't instantiated yet");
        }
    }

    public override void Feedback()
    {
    }
}