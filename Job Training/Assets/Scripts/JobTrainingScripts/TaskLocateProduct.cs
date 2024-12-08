using System;
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

        // Evaluation
        JobTrainingManager.instance.WriteOnUi("Well done! You solved the task in x seconds");
        // log data
        // send extensive evaluation to trainer
        
        CompleteTask();
    }

    public override void TaskSetup()
    {
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
        // playing speech sound (in API?)
        
        JobTrainingManager.instance.WriteOnUi("FirstDialogInput"); // for dynamic first dialogue input from LLM API
    }
    public override void Dismantle()
    {
        // logging first dialog
        throw new System.NotImplementedException();
    }
}

// add eventListener For UserPosition and Audio user response, on rsponse arrival then send response of user to the llm
class AwaitUserDirectionForProductLocation : InteractionState
{
    public override void Dismantle()
    {
        throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        UserInput.OnUserSpoke += HandleUserSpoke;
        UserInput.OnUserMoved += HandleUserMoved;
    }

    private void HandleUserMoved(Vector3 newPosition)
    {

    }

    private void HandleUserSpoke(string spokenText){

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
        throw new System.NotImplementedException();
    }
}





//additional states: client requests to be directed again because the user gave wrong response 