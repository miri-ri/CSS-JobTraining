using UnityEngine;
using UnityEditor;
using System.IO;
using System.DateTime;
public class PerformanceLog{

    public List<TaskData> TasksData{
        get;add;
    }
    private string UserIdentifier;
    private DateTime ActivityStart, ActivityEnd;

    public PerformanceLog(string userName){
        UserIdentifier=userName;
        TasksData= new List<TaskData>;
        ActivityStart=DateTime.Now;
    }
    public void EndLog(){
        ActivityEnd=DateTime.Now;
        LogData();
    } 
    private void LogData(){
        StreamWriter writer= new StreamWriter("Assets/Resources/LogData.txt",true);
        string log="Log: User - "+UserIdentifier+ "\nStart - "+ActivityStart+" \n End - "+ActivityEnd+"\nDuration - "+ActivityStart-ActivityEnd;
        int i=1;
        foreach (TaskData task in TasksData)
        {
            log+=1+":  Task "+task.taskName+"\n";
            log+="   Score - "+task.score+"\n";
            log+="   Transcript :\n";
            foreach (string dial in task.dialogTranscript)
            {
                log+=dial+"\n";
            }
            log+"\nFeedback - "+task.feedbackMessage;
        }
        writer.Write(log);
        writer.Close();
        TextAsset logFile=Resources.Load("LogData");
        Debug.Log(logFile.text);

    }

}
public class TaskData{//to be created in Activity manager on new task

    string taskName;
    List<string> dialogTranscript;
    int score;
    string feedbackMessage;
    DateTime taskStart, taskEnd;

    public TaskData(string taskName){ 
        this.taskName=taskName
        dialogTranscript=new List<string>;
        }

    public void addResponse(string text, bool isUser){
        if(isUser) dialogTranscript.add("user -> "text);
        else dialogTranscript.add("customer -> "text);
    }
    public void addConversationError(string errorMessage){
        dialogTranscript.add("ERROR -> "text);
    }


}