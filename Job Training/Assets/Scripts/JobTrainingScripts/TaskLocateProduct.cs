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
        
        
    }
    void ShowFeedback(EvaluationResponse eval){

        JobTrainingManager.instance.WriteOnUi(eval.evaluations[0]);//todo  ->show all info and send to TTS and graphic of evaluation screen

        JobTrainingManager.instance.PerformanceLog.getCurrentTaskData().setFeedback(eval);//logs feedback
        JobTrainingManager.instance.RemoveEvaluationHandler(ShowFeedback);

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
        //JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question="FirstDialogInput";
         
    }
    public void handleTTS(int secondsNeeded){
        //set waiting time before change state TODO
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded,new AwaitUserUserInput());
    }
    public override void Dismantle()
    {
        // logging first dialog
       JobTrainingManager.instance.RemoveTTShandler(handleTTS);
    }
}

// add eventListener For UserPosition and Audio user response, on rsponse arrival then send response of user to the llm
class AwaitUserUserInput : InteractionState
{
    public override void Dismantle()
    {
        JobTrainingManager.instance.RemoveSTThandler(HandleUserSpoke);
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

    private void HandleUserSpoke(Speech spokenResponse){//if domenico can output an object from the tts of the same type as those needed by LLM it would make this simpler -> TODO
        //evaluation data
        JobTrainingManager.instance.PerformanceLog.getCurrentTaskData().addResponse(spokenResponse.semantic.reply,true);

        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech=spokenResponse ;
        

    }   
}
//listener for response from llm (is response acceptable and/or understood) and check if user is in right position
class ClientEndsDialog : InteractionState
{
    public override void Dismantle()
    {
        JobTrainingManager.instance.RemoveLLMCustomerResponse(PLayGeneratedResponse);
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