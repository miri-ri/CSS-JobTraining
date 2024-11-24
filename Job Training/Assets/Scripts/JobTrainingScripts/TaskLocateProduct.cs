
using System.Drawing.Text;

public class TaskLocateProduct : Task
{
     
    void Start(){
        interactionMachine=new InteractionMachine(new FirstDialog());
        
    }
    public override void Feedback()
    {
        //send data logged during states, or only user responses to feedback api
    }

    public override void Interaction()
    {
        
    }

    public override void Introduction()
    {
        throw new System.NotImplementedException();
    }
}

class FirstDialog:InteractionState{
    //play audio from virtual client
    public override void Setup()
    {
        
    }
    public override void Dismantle()
    {
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
        throw new System.NotImplementedException();
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