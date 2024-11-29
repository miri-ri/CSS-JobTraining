using UnityEngine;
using UnityEngine.UI;
public class EventDebugSummoner : MonoBehaviour{

    public void EvokeOnTaskCompleteEvent(){
        //logic to throw the event
                Debug.Log("event xxxx sent");

    }
    //For when you need to manually call an event when debugging!!


    //   you can also attach to the button object (on the editor: Button->Button[the Component]-> OnCLick()  )
    //  one of the managers to Manually call public methodsssss!!!!!!!!!!!

}