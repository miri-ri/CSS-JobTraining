using System;

public class TaskShowInfopoint : Task
{
    protected override TaskList GetTaskType() => TaskList.ShowInfopoint;
    protected override string GetInitialDialog() => "Scusa, sto cercando un punto informazioni, puoi aiutarmi?";
    public override string GetAreaTrigger() => "locateTask"; 
    public override string GetBackgroundImage() => "market_info"; 
    public TaskShowInfopoint(){
        if (JobTrainingManager.instance == null){
            throw new Exception("The job training manager isn't instantiated yet");
        }
    }

    public override void Feedback()
    {
    }

    protected override string GetIntroduction()=>"Un cliente chiederà di essere indirizzato verso un infopoint, aiutalo rispondendo educatamente e mostrandogli dove si trova";
}