using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
public class PerformanceLog{

    public List<TaskData> TasksData{
        get;
    }
    private string UserIdentifier;
    private DateTime ActivityStart, ActivityEnd;

    public PerformanceLog(string userName){
        UserIdentifier=userName;
        TasksData= new List<TaskData>();
        ActivityStart=DateTime.Now;
    }
    public void EndLog(){
        ActivityEnd=DateTime.Now;
        LogData();
    } 
    public TaskData getCurrentTaskData(){
        return TasksData[^1];
    }
    public void MovementDataLogger(Movement movement){//for development data
        string positionData="Position Data : \n", timingData="Timing Data : \n";
        Positioning pos=movement.positioning;
        Timing tt=movement.timing;
        
        positionData+="starting position -> "+PositionToCoordinate(pos.start_pos)+"\n";
        positionData+="user final position -> "+PositionToCoordinate(pos.user_pos)+"\n";
        positionData+="target position -> "+PositionToCoordinate(pos.target_pos)+"\n";
        positionData+="acceptability radius -> "+pos.ok_radius+"\n";
        positionData+="area(?) -> ("+pos.area.w+","+pos.area.h+")\n";

        
        timingData+="time before action -> "+tt.s_before_action+"\n";
        timingData+="duration -> "+tt.s_duration+"\n";
        timingData+="T-before target -> "+tt.s_before_action_target+"\n";
        timingData+="duration target -> "+tt.s_duration_per_unit_target+"\n";
        StreamWriter writer= new StreamWriter("Assets/Resources/logData_Movement"+UserIdentifier+".txt",true);

        string wr="";
        if(JobTrainingManager.noKinectDebug)
            wr+="DATA FROM HARDCODED DEBUG -- ";
        wr+="Logged data of activity -> "+DateTime.Now+" \n\n"+ positionData+"______\n"+ timingData+"______\n";
        writer.Write(wr);
        writer.Close();
    }
    
    private string PositionToCoordinate(Position pp){
        return "("+pp.x+","+pp.y+")";
    }
    private void LogData(){
        StreamWriter writer= new StreamWriter("Assets/Resources/logData_"+UserIdentifier+".txt",true);
        string log = "Log: User - " + UserIdentifier + "\nStart - " + ActivityStart + " \n End - " + ActivityEnd + "\nDuration - " + ActivityStart.Subtract( ActivityEnd).TotalMinutes;
        foreach (TaskData task in TasksData)
        {
            log+=1+":  Task "+task.taskName+"\n";
            log+="   Score - "+task.score+"\n";
            log+="   Transcript :\n";
            foreach (string dial in task.dialogTranscript)
            {
                log+=dial+"\n";
            }
            log+="\nFeedback - "+task.feedbackMessage;
        }
        writer.Write(log);
        writer.Close();
       // TextAsset logFile= (TextAsset)Resources.Load("LogData");
       // Debug.Log(logFile.text);

    }

}
public class TaskData{//to be created in Activity manager on new task
//TODO add timings of response time and movement time
    public string taskName;
    public List<string> dialogTranscript;
    public int score;
    public string feedbackMessage;
    public DataForEvaluation dataForEvaluation;
    public DateTime taskStart, taskEnd;

    public void setFeedback(EvaluationResponse eval){
        score= (int)eval.Total;
        feedbackMessage="feedback\n";
        //feedbackMessage=Pono(eval.Evaluations);
        feedbackMessage+="Final Score -> "+eval.Total+"\n";
        feedbackMessage+="____ evaluations ____ \n";
        foreach (var item in eval.Evaluations)
        {
            feedbackMessage+="partial score -> "+item.Score+"\n";
            feedbackMessage+="feedback -> "+item.Description+"\n";
            feedbackMessage+="___\n";
        }


        TaskDebugLog(feedbackMessage);
        
    }

    private void TaskDebugLog(string txt){
        StreamWriter writer= new StreamWriter("Assets/Resources/logData_debugTask.txt",true);
        writer.Write(txt+"\n");
        writer.Close();
    }

    public TaskData(string taskName){ 
        this.taskName=taskName;
        taskStart=DateTime.Now;
        dialogTranscript=new List<string>();
        dataForEvaluation=new();
        
        }
    public void EndTask(){
        taskEnd=DateTime.Now;
    }
    public void addResponse(string text, bool isUser){
        if(isUser){
            dialogTranscript.Add("user -> "+text);
            TaskDebugLog(DateTime.Now+"-- user -> "+text);
        }
        else {
            dialogTranscript.Add("customer -> "+text);
            TaskDebugLog(DateTime.Now+"-- customer -> "+text);
        }
    }
    public void addConversationError(string errorMessage){
        dialogTranscript.Add("ERROR -> "+ errorMessage);
        TaskDebugLog(DateTime.Now+"-- ERROR -> "+ errorMessage);
    }


}