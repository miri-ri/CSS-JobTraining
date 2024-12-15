using System;
using Unity.VisualScripting;
using UnityEngine;

public class TaskLocateProduct : Task
{

    public TaskLocateProduct(){
        if(JobTrainingManager.instance==null){
            throw new System.Exception("The job training manager isn't instantiated yet");
        }
    }
    
    public override void Feedback()
    {
        //send data logged during states, or only user responses to feedback api
        JobTrainingManager.instance.GetEvaluation(JobTrainingManager.instance.getCurrentTasksFeedbackData(),ShowFeedback);

        // Evaluation
        // log data
        // send extensive evaluation to trainer
        
        
    }
    void ShowFeedback(EvaluationResponse eval){
        JobTrainingManager.instance.WriteOnUi(eval.description);
        JobTrainingManager.instance.PerformanceLog.TasksData[^1].score=eval.Score;
        JobTrainingManager.instance.PerformanceLog.TasksData[^1].feedbackMessage=eval.description;

        CompleteTask();
    }

    public override void TaskSetup()
    {//add tts use
        JobTrainingManager.instance.WriteOnUi("In this task you will have to show the product to the customer."); // maybe replace with Task Description later
        JobTrainingManager.instance.ChangeFrontWallBackground("PlaceholderMarket");

        
        SetInteractionMachine(new InteractionMachine());
        GetInteractionMachine().ChangeState(new FirstDialog());
    }

    
}

class FirstDialog:InteractionState{
    //play audio from virtual client
    public override void Setup()
    {
        JobTrainingManager.instance.PlayDialog("FirstDialogInput",handleTTS);
        JobTrainingManager.instance.WriteOnUi("FirstDialogInput"); // for dynamic first dialogue input from LLM API
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question="FirstDialogInput";
         
    }
    public void handleTTS(int secondsNeeded){
        //set waiting time before change state
    }
    public override void Dismantle()
    {
        // logging first dialog
        throw new System.NotImplementedException();
    }
}

// add eventListener For UserPosition and Audio user response, on rsponse arrival then send response of user to the llm
class AwaitUserUserInput : InteractionState
{
    public override void Dismantle()
    {
        throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        JobTrainingManager.instance.GetUserDialog(HandleUserSpoke);
        //UserInput.OnUserSpoke += HandleUserSpoke;
        //UserInput.OnUserMoved += HandleUserMoved; 
    }

    private void HandleUserMoved(Movement userMovement)
    {
        JobTrainingManager.instance.getCurrentTasksFeedbackData().movement=userMovement;
    }

    private void HandleUserSpoke(UserResponseDialog spokenText){//if domenico can output an object from the tts of the same type as those needed by LLM it would make this simpler -> TODO
        //evaluation data
        JobTrainingManager.instance.PerformanceLog.TasksData[^1].addResponse(spokenText.transcript,true);
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.reply=spokenText.transcript;
        float timeBeforeResponse= (float)spokenText.startListening.Subtract(spokenText.StartedSpeaking).TotalSeconds;
        float responseTime= (float)spokenText.StartedSpeaking.Subtract(spokenText.EndedSpeaking).TotalSeconds;
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.timing.s_before_action=timeBeforeResponse;
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.timing.s_duration=responseTime;


    }   
}
//listener for response from llm (is response acceptable and/or understood) and check if user is in right position
class ClientEndsDialog : InteractionState
{
    public override void Dismantle()
    {
        throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        JobTrainingManager.instance.GenerateLLMCustomerResponse("last transcript",PLayGeneratedResponse);
    }
    public void PLayGeneratedResponse(string reply){
        JobTrainingManager.instance.PlayDialog("FirstDialogInput",handleTTS);
        JobTrainingManager.instance.WriteOnUi("FirstDialogInput"); // for dynamic first dialogue input from LLM API
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question="FirstDialogInput";
        
    }
    public void handleTTS(int secondsNeeded){
        //set waiting time before change state
    }
}





//additional states: client requests to be directed again because the user gave wrong response 