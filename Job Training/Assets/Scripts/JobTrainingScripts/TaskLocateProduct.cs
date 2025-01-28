using System;
using System.Threading;
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
    }

    public override void TaskSetup() // maybe making introduction a proper state for consistency
    {
        JobTrainingManager.instance.GetTaskManager().TaskDescription(TaskList.LocateProduct);
        JobTrainingManager.instance.ChangeFrontWallBackground("PlaceholderMarket");

        SetInteractionMachine(new InteractionMachine());
        GetInteractionMachine().ChangeState(new FirstDialog());
    }

    
}

class FirstDialog:InteractionState{
    //play audio from virtual client
    public override void Setup()
    {
        string txtForTTS="Ciao! Can you show me the tomatoes?";
        JobTrainingManager.instance.PlayDialog(txtForTTS,handleTTS);
        //JobTrainingManager.instance.WriteOnUi("Ciao! Can you show me the tomatoes?"); // for dynamic first dialogue input from LLM API
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question=txtForTTS;// here we have to insert the question not FirstDialogInput
        JobTrainingManager.instance.PerformanceLog.getCurrentTaskData().addResponse(txtForTTS, false);
         
    }
    public void handleTTS(float secondsNeeded){
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
        JobTrainingManager.instance.UnsubscribeToAreaTrigger("locateTask",HandleUserInTarget);
    }
    private Movement actionForFeedBack;
    private DateTime startMovement;
    private bool movementFinished,SpeechFinished;

    private  delegate void onTimeout();
    private event onTimeout TimesUp;
    public override void Setup()
    {
        Debug.Log("Setting up AwaitUserInputState");
        actionForFeedBack=new();
        startMovement=DateTime.Now;
        JobTrainingManager.instance.GetUserDialog(HandleUserSpoke);
        JobTrainingManager.instance.SubscribeToAreaTrigger("locateTask",HandleUserInTarget);
        actionForFeedBack.positioning.start_pos=new(JobTrainingManager.instance.getUserPos());
        if(JobTrainingManager.noKinectDebug)
            movementFinished=true;//this should be false. true to bypass waiting WARNING
        SpeechFinished=false;

        TimesUp+=HandleTimeOut;
        //todo needs a timer for a maximun duration -> calls TimesUp
    }
    public void HandleUserInTarget(Vector2 userPosition, Vector2 targetPosition, DateTime arrival){//todo fix up start pos and area
        actionForFeedBack.positioning.user_pos=new(userPosition);

        actionForFeedBack.positioning.target_pos=new(targetPosition);
        
        actionForFeedBack.timing.s_before_action=0;//unclear
        actionForFeedBack.timing.s_duration=arrival.Subtract(startMovement).Seconds;
        JobTrainingManager.instance.getCurrentTasksFeedbackData().movement=actionForFeedBack;
        JobTrainingManager.instance.PerformanceLog.MovementDataLogger(JobTrainingManager.instance.getCurrentTasksFeedbackData().movement);
        movementFinished=true;


        if(SpeechFinished) ToNextState();
    }

    public void HandleTimeOut(){//todo lab decide timer
        if(!movementFinished){

        }
        if(!SpeechFinished){

        }

        ToNextState();

    }

    public void HandleUserSpoke(Speech spokenResponse){
        
        JobTrainingManager.instance.PerformanceLog.getCurrentTaskData().addResponse(spokenResponse.semantic.reply, true);
        
        string question=JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question;
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech=spokenResponse ;
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question=question ;
        
        

        if(JobTrainingManager.noKinectDebug){//for debug w/ no kinect
            HandleUserInTarget(new(0,0), new(0,0), DateTime.Now);
        }
        SpeechFinished=true;
        if(movementFinished) 
            ToNextState();        
    }

    void ToNextState(){

        TimesUp-=HandleTimeOut;

        bool IsResponsePositive = true; // todo: proper adaptation

        if(IsResponsePositive) {
            
            Debug.Log("Positive response, switching to the next state");
            JobTrainingManager.instance.GetTaskManager().CurrentTask.GetInteractionMachine().ChangeState(new PositiveTurnout());
        } else {
            JobTrainingManager.instance.GetTaskManager().CurrentTask.GetInteractionMachine().ChangeState(new NegativeTurnout());
        }
    }
}


//listener for response from llm (is response acceptable and/or understood) and check if user is in right position
class PositiveTurnout : InteractionState
{
    public override void Dismantle()
    {
        JobTrainingManager.instance.RemoveLLMCustomerResponse(PLayGeneratedResponse);
        JobTrainingManager.instance.RemoveTTShandler(handleTTS);
    }

    public override void Setup()
    {
        Debug.Log("generated response");
        PLayGeneratedResponse("Risposta positiva");
        //JobTrainingManager.instance.GenerateLLMCustomerResponse("last transcript",PLayGeneratedResponse);
    }
    public void PLayGeneratedResponse(string reply){
        JobTrainingManager.instance.PlayDialog("Thanks for your help!",handleTTS);
        
        
    }
    public void handleTTS(float secondsNeeded){
        //set waiting time before change state
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded, new FeedbackState());
    }
}


class NegativeTurnout : InteractionState
{
    public override void Setup(){
       // JobTrainingManager.instance.GenerateLLMCustomerResponse("last transcript",PlayGeneratedResponse);
    }

    public override void Dismantle(){
        JobTrainingManager.instance.RemoveTTShandler(handleTTS);
        JobTrainingManager.instance.RemoveLLMCustomerResponse(PlayGeneratedResponse);
    }
    public void PlayGeneratedResponse(string reply){
        JobTrainingManager.instance.PlayDialog("I didn't quite understand, can you try again?",handleTTS);
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question="I didn't quite understand, can you try again?";
        
    }
    public void handleTTS(float secondsNeeded){
        //set waiting time before change state
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded, new AwaitUserInput());
    }
}

class FeedbackState : InteractionState
{
    public override void Setup(){
        JobTrainingManager.instance.GetEvaluation(JobTrainingManager.instance.getCurrentTasksFeedbackData(),ShowFeedback);
        JobTrainingManager.instance.PlayDialog("Well done, now wait a few seconds for your evaluation",handleTTS);
    }
    void ShowFeedback(EvaluationResponse eval){
        JobTrainingManager.instance.PerformanceLog.getCurrentTaskData().setFeedback(eval);//logs feedback
        JobTrainingManager.instance.ShowFeedbackMessages(eval.Evaluations[0].Description);

        
        
        //CompleteTask();
        //todo on user input or after timer complete task
        
    }

    public override void Dismantle(){
        JobTrainingManager.instance.RemoveEvaluationHandler(ShowFeedback);
        JobTrainingManager.instance.RemoveTTShandler(handleTTS);
    }

    public void handleTTS(float secondsNeeded){
        //JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded+10, null);//todo , i believe this breakes the machine 
    }
}




//additional states: client requests to be directed again because the user gave wrong response 