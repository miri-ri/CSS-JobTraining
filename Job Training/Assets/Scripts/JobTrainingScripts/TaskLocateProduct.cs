using System;
using Unity.VisualScripting;
using UnityEngine;

public class TaskLocateProduct : Task
{

    public TaskLocateProduct(){
        if(JobTrainingManager.instance==null){
            throw new Exception("The job training manager isn't instantiated yet");
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

    public override void TaskSetup() // maybe making introduction a proper state for consistency
    {//add tts use
        JobTrainingManager.instance.WriteOnUi("In this task, you will have to show the product to the customer."); // maybe replace with Task Description later
        JobTrainingManager.instance.ChangeFrontWallBackground("PlaceholderMarket");

        
        SetInteractionMachine(new InteractionMachine());
        GetInteractionMachine().ChangeState(new FirstDialog());
    }

    
}

class FirstDialog:InteractionState{
    //play audio from virtual client
    public override void Setup()
    {
        JobTrainingManager.instance.PlayDialog("Ciao! Can you show me the tomatoes?",handleTTS);
        JobTrainingManager.instance.WriteOnUi("Ciao! Can you show me the tomatoes?"); // for dynamic first dialogue input from LLM API
        //JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question="FirstDialogInput";
         
    }
    public void handleTTS(int secondsNeeded){
        //set waiting time before change state TODO
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded,new AwaitUserInput());
    }
    public override void Dismantle()
    {
        // logging first dialog
       JobTrainingManager.instance.RemoveTTShandler(handleTTS);
    }
}

// add eventListener For UserPosition and Audio user response, on rsponse arrival then send response of user to the llm
class AwaitUserInput : InteractionState
{
    public override void Dismantle()
    {
        JobTrainingManager.instance.RemoveSTThandler(HandleUserSpoke);
    }

    public override void Setup()
    {
        Debug.Log("Setting up AwaitUserInputState");
        JobTrainingManager.instance.ToggleSpeakerButton(true);
        JobTrainingManager.instance.GetUserDialog(HandleUserSpoke);
    }

    private void HandleUserMoved(Movement userMovement)
    {
        JobTrainingManager.instance.getCurrentTasksFeedbackData().movement=userMovement;
    }

    private void HandleUserSpoke(Speech spokenResponse){//if domenico can output an object from the tts of the same type as those needed by LLM it would make this simpler -> TODO
        //evaluation data
        
        Debug.Log("Trying to add response");
        var task1 = PerformEvaluation(spokenResponse);
        
        Debug.Log("Getting Feedback Data");
        var task2 = PerformLoggingAsync(spokenResponse);

        System.Threading.Tasks.Task.WaitAll(task1, task2);
        
        bool IsResponsePositive = true; // todo: proper adaptation

        if(IsResponsePositive) {
            
            Debug.Log("Positive response, switching to the next state");
            JobTrainingManager.instance.GetTaskManager().CurrentTask.GetInteractionMachine().ChangeState(new PositiveTurnout());
        } else {
            JobTrainingManager.instance.GetTaskManager().CurrentTask.GetInteractionMachine().ChangeState(new NegativeTurnout());
        }

    }

    private static async System.Threading.Tasks.Task PerformEvaluation(Speech spokenResponse)
    {
        await System.Threading.Tasks.Task.Run(()=>
        {
        JobTrainingManager.instance.PerformanceLog.getCurrentTaskData().addResponse(spokenResponse.semantic.reply, true);
        
        Debug.Log("Response added");
        });

    }

    private static async System.Threading.Tasks.Task PerformLoggingAsync(Speech spokenResponse)
    {
        await System.Threading.Tasks.Task.Run(()=>
        {
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech=spokenResponse ;
        
        Debug.Log("Feedback Data set");
        });

    }  
}


//listener for response from llm (is response acceptable and/or understood) and check if user is in right position
class PositiveTurnout : InteractionState
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
        JobTrainingManager.instance.PlayDialog("Thanks for your help!",handleTTS);
        JobTrainingManager.instance.WriteOnUi("Thanks for your help!"); // for dynamic first dialogue input from LLM API
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question="FirstDialogInput";
        
    }
    public void handleTTS(int secondsNeeded){
        //set waiting time before change state
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded, new FeedbackState());
    }
}


class NegativeTurnout : InteractionState
{
    public override void Setup(){
        JobTrainingManager.instance.GenerateLLMCustomerResponse("last transcript",PlayGeneratedResponse);
    }

    public override void Dismantle(){
        JobTrainingManager.instance.RemoveLLMCustomerResponse(PlayGeneratedResponse);
    }
    public void PlayGeneratedResponse(string reply){
        JobTrainingManager.instance.PlayDialog("I didn't quite understand, can you try again?",handleTTS);
        JobTrainingManager.instance.WriteOnUi("I didn't quite understand, can you try again?"); 
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question="FirstDialogInput";
        
    }
    public void handleTTS(int secondsNeeded){
        //set waiting time before change state
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded, new AwaitUserInput());
    }
}

class FeedbackState : InteractionState
{
    public override void Setup(){
        JobTrainingManager.instance.WriteOnUi("Well done! Task completed.");
        JobTrainingManager.instance.PlayDialog("WellDone", handleTTS);
    }

    public override void Dismantle(){
        JobTrainingManager.instance.RemoveTTShandler(handleTTS);
    }

    public void handleTTS(int secondsNeeded){
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded, null);
    }
}




//additional states: client requests to be directed again because the user gave wrong response 