using System;
using TMPro;
using UnityEngine;

// prototype of task 1: Locate product
// advance by enabling use of syestem's TTS for audio, STT for dialog and kinect capture for movement;
// then advance to using a fully working parametrized manager, with feedback (scoring) system;
// once everything is there, prepare other tasks.

public class ProtoTaskManager : MonoBehaviour
{

    public GameObject UserSimPref;
    private GameObject User;
    private PlayerMovementSimultor userSim;
    private GameObject Trainer;
    private PlayerMovementSimultor trainerSim;


    [SerializeField] AudioSource dialog1;
    [SerializeField] AudioSource dialog2;
    [SerializeField] AudioSource dialog3;





     string introduction = "introduction";
     string instruction = "instruction ";
     string feedback = "goodjob ";



     string[] userDialog;
     string[] trainerDialog;




    //public GameObject bubble;
    //public TextMeshPro dialogText;
    private BubbleBehaviour globalBubble;

    public GameObject bubblePrefab;

    private BubbleBehaviour userBub, clientBub;





    private int currentPhase;  //0 instroduction, 1 interaction, 2 feedback
    public bool userInRightIsle;








    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        currentPhase=0;
        userInRightIsle=false;
        conversationState=0;

        globalBubble= Instantiate(bubblePrefab , new Vector3(0,11,400) , Quaternion.identity).GetComponent<BubbleBehaviour>();
        globalBubble.transform.position=new Vector3(-5,15,400);

        User=Instantiate(UserSimPref, new Vector3(2,7,410), Quaternion.identity);
        Rigidbody trig   =User.AddComponent<Rigidbody>();
        trig.useGravity=false;
        User.AddComponent<BoxCollider>();
        userSim=User.GetComponent<PlayerMovementSimultor>();
        userSim.Setup(1);

        Trainer=Instantiate(UserSimPref, new Vector3(-1,7,410), Quaternion.identity);
        trainerSim=Trainer.GetComponent<PlayerMovementSimultor>();
        trainerSim.Setup(2);

        //
        userBub= Instantiate(bubblePrefab , new Vector3(0,11,400) , Quaternion.identity).GetComponent<BubbleBehaviour>();
        userBub.SetDimension(2);
        userBub.SetFollow(User);

        clientBub= Instantiate(bubblePrefab , new Vector3(0,11,400) , Quaternion.identity).GetComponent<BubbleBehaviour>();
        clientBub.SetDimension(2);
        clientBub.SetFollow(Trainer);
        
        userBub.WriteInBubble( "");
        clientBub.WriteInBubble( "");
        
        WriteOnBubble("prova");
    }


    private void UserSays(string text){
        userBub.WriteInBubble( text);
    }

    private int conversationState;
    // Update is called once per frame
    void Update()
    {
       
        Interaction();

        

    }


    // write stuff somewhere
    private void PlayIntroduction(){
        writeText(introduction); 
    }

    //move users in space toward target position and write smwr; could use events 
     private void Interaction(){//todo x proto :   switch gui text for audio(files on git)
        if(currentPhase==1 && Input.GetKeyUp(KeyCode.Space)){
            if(conversationState==0  ){
                 clientBub.WriteInBubble("Cliente: Buongiorno, sa dirmi dove sono le patate?");
                 conversationState++;
                 }
            if(conversationState==1)
                userBub.WriteInBubble("Utente: Con piacere, seguitemi perfavore. Le patate sono nel reparto verdure");

            if(userInRightIsle){
                clientBub.WriteInBubble("Cliente: La ringrazio");
                conversationState++;
                currentPhase++;
            }
                
        }
    }
    // write smwr
     private void PlayFeedback(){
         writeText(feedback);
    }

// this would be on audio
    private void writeText(string textMsg){
       // Debug.Log(textMsg);
        WriteOnBubble(textMsg);

    }

        void OnGUI() {

            if (GUILayout.Button("Play dialog")){ 
                switch (currentPhase)
                {
                    case 0:
                        PlayIntroduction();
                    break;

                    case 1:
                        Interaction();
                    break;

                    case 2:
                        userBub.WriteInBubble( "");
                        clientBub.WriteInBubble( "");
                        PlayFeedback();
                    break;
                    //default:
                }
        }
        
    
        if (GUILayout.Button("changePhase")){
            currentPhase++;
            writeText("changed Phase");
                if(currentPhase==3)
                    writeText("end of proto");
        }
    }
    public void WriteOnBubble(string text){
        globalBubble.WriteInBubble(text);
    }
   
}
