using System;
using UnityEngine;

public class TaskLocateProduct : Task
{
    protected override TaskList GetTaskType() => TaskList.LocateProduct;
    protected override string GetInitialDialog() => "Ciao! Puoi mostrarmi dove sono i pomodori?";
    protected override string GetIntroduction() => "In questo task dovrai aiutare un cliente rispondendo alla sua richiesta e mostrandogli dove si trovano i pomodori in vendita";
    public override string GetAreaTrigger() => "locateTask";

    public TaskLocateProduct(){
        if(JobTrainingManager.instance==null){
            throw new Exception("The job training manager isn't instantiated yet");
        }
    }
    
    public override void Feedback()
    {
    }

    public override string GetBackgroundImage()=>"Senza titolo";
}
class TaskDescription : InteractionState
{
    string FirstDialog, instructions;
    public override void Dismantle()
    {
               JobTrainingManager.instance.RemoveTTShandler(handleTTS);

    }

    public override void Setup()
    {
        JobTrainingManager.instance.ToggleTextUi(true);
        JobTrainingManager.instance.ChangeFrontWallBackground("instructions");
        JobTrainingManager.instance.PlayDialog(instructions,handleTTS);
    }
    public TaskDescription(string introduction, string FirstDialog){
        instructions=introduction;
        this.FirstDialog=FirstDialog;
    }
    public void handleTTS(float secondsNeeded){
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded,new FirstDialog(FirstDialog));
    }
}
class FirstDialog:InteractionState{
    //play audio from virtual client

    private string dialogText;

    public FirstDialog(string dialogText){
        this.dialogText = dialogText;
    }
    public override void Setup()
    {
        if(JobTrainingManager.noKinectDebug){
            JobTrainingManager.instance.ToggleTextUi(true);
        }else JobTrainingManager.instance.ToggleTextUi(false);
        
        JobTrainingManager.instance.ChangeFrontWallBackground(JobTrainingManager.instance.GetTaskManager().CurrentTask.GetBackgroundImage());
        JobTrainingManager.instance.PlaySound("supermarket-17823");
        JobTrainingManager.instance.PlayDialog(dialogText,handleTTS);// for dynamic first dialogue input from LLM API
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question=dialogText;// here we have to insert the question not FirstDialogInput
        JobTrainingManager.instance.PerformanceLog.getCurrentTaskData().addResponse(dialogText, false);
    }
    public void handleTTS(float secondsNeeded){
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded+2.5f,new AwaitUserInput());
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
        JobTrainingManager.instance.DismantleTimer(HandleTimeOut);
        JobTrainingManager.instance.UnsubscribeToAreaTrigger(areaTrigger,HandleUserInTarget);
    }
    private string areaTrigger;
    private Movement actionForFeedBack;
    private DateTime startMovement;
    private bool movementFinished,SpeechFinished;

    public AwaitUserInput(){
        areaTrigger = JobTrainingManager.instance.GetTaskManager().CurrentTask.GetAreaTrigger();
    }
    public override void Setup()
    {
        actionForFeedBack=new();
        startMovement=DateTime.Now;
        JobTrainingManager.instance.GetUserDialog(HandleUserSpoke);
        JobTrainingManager.instance.SubscribeToAreaTrigger(areaTrigger,HandleUserInTarget);
        actionForFeedBack.positioning.start_pos=new(JobTrainingManager.instance.getUserPos());
        if (JobTrainingManager.noKinectDebug)
            movementFinished = true;//this should be false. true to bypass waiting WARNING
        else movementFinished = false;
        SpeechFinished=false;
        JobTrainingManager.instance.SetTimer(25,HandleTimeOut);
        
       
    }
    public void HandleUserInTarget(Vector2 userPosition, Vector2 targetPosition, DateTime arrival){//todo fix up start pos and area
        actionForFeedBack.positioning.user_pos=new(userPosition);

        actionForFeedBack.positioning.target_pos=new(targetPosition);
        
        actionForFeedBack.timing.s_before_action=0;//unclear
        actionForFeedBack.timing.s_duration=arrival.Subtract(startMovement).Seconds;
        JobTrainingManager.instance.getCurrentTasksFeedbackData().movement=actionForFeedBack;
        JobTrainingManager.instance.PerformanceLog.MovementDataLogger(JobTrainingManager.instance.getCurrentTasksFeedbackData().movement);



        movementFinished=true;
        if(SpeechFinished) 
            ToNextState();
    }

    public void HandleTimeOut(){
        Debug.Log("TIMEOUT");
        if(!movementFinished){

        }
        if(!SpeechFinished){
            //todo graphics, may add a different animation to the microphone if timeout;
            //warning with no words server crashes
        }
       // JobTrainingManager.instance.PlaySound("timeoutSound");//todo add sound
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

        JobTrainingManager.instance.DismantleTimer(HandleTimeOut);

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
        PLayGeneratedResponse("Risposta positiva");
        //JobTrainingManager.instance.GenerateLLMCustomerResponse("last transcript",PLayGeneratedResponse);
    }
    public void PLayGeneratedResponse(string reply){
        JobTrainingManager.instance.PlayDialog("Grazie mille!",handleTTS);
        
        
    }
    public void handleTTS(float secondsNeeded){
        //set waiting time before change state
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded+2, new FeedbackState());
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
        JobTrainingManager.instance.PlayDialog("Non ho capito, puoi ripeter?",handleTTS);
        JobTrainingManager.instance.getCurrentTasksFeedbackData().speech.semantic.question= "Non ho capito, puoi ripeter?";
        
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
        JobTrainingManager.instance.ChangeFrontWallBackground("waiting_eval");
        JobTrainingManager.instance.PlaySound("waiting-music");
        JobTrainingManager.instance.ToggleTextUi(false);
        //JobTrainingManager.instance.PlayDialog("Ben fatto, ora attendi qualche secondo per la valutazione ",handleTTS);
    }
    void ShowFeedback(EvaluationResponse eval){
        
        JobTrainingManager.instance.PerformanceLog.getCurrentTaskData().setFeedback(eval);//logs feedback
        //JobTrainingManager.instance.ShowFeedbackMessages(eval.Evaluations[0].Description);
        JobTrainingManager.instance.ChangeFrontWallBackground("evaluation");
        
        JobTrainingManager.instance.showEvaluation(eval);
        string evalmin="";
        string evalmax="";
        double min = 11;
        double max = -1;
        foreach(Evaluation ee in eval.Evaluations){
            Debug.Log(ee.Description);
            if(ee.Score < min) {
                min = ee.Score;
                evalmin = ee.Description;
            } 
            else if(ee.Score > max) {
                max = ee.Score;
                evalmax = ee.Description;
            }
        }
           
        string result = System.Text.RegularExpressions.Regex.Replace(evalmax+" "+evalmin, @"\d", "");
        result = result.Replace("\n", " ");
        Debug.Log(result);
        JobTrainingManager.instance.PlayDialog(result,handleTTS2);
        
        //
        //todo on user input or after timer complete task
        
    }
    void handleTTS2(float sec){
        Debug.Log(sec);
        JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(sec, new EndingState());
    }
    
    public override void Dismantle(){
        JobTrainingManager.instance.RemoveEvaluationHandler(ShowFeedback);
        JobTrainingManager.instance.RemoveTTShandler(handleTTS);
        JobTrainingManager.instance.RemoveTTShandler(handleTTS2);
        JobTrainingManager.instance.hideEvaluation();
        JobTrainingManager.instance.StopAudioCurrentClip();
    }

    public void handleTTS(float secondsNeeded){
        //JobTrainingManager.instance.GetTaskManager().ChangeStateOnTimer(secondsNeeded+10, null);
    }
}
class EndingState : InteractionState
{
    public override void Dismantle()
    {
        
    }

    public override void Setup()
    {
        Debug.Log("end of task LOCATE");
        JobTrainingManager.instance.GetTaskManager().CurrentTask.CompleteTask();
    }
}




//additional states: client requests to be directed again because the user gave wrong response 