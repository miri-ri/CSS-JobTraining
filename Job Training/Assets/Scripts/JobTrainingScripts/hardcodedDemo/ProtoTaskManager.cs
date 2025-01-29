using System;
using TMPro;
using UnityEngine;

//Todo: Can be deleted

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





     string introduction = "Intro: A customer will come into the supermarket and ask you to show them a certain product.\n Please, answer them by going to the correct product displayed on the floor. ";
     string instruction = "instructions: Trainer- Ask where you can find potatoes ";
     string feedback = "Very well, the response was correct and coincise...\n Score: 90/100.....etc... ";



     string[] userDialog;
     string[] trainerDialog;




    public GameObject bubblePrefab;
    private BubbleBehaviour globalBubble;

 

    private BubbleBehaviour userBub, clientBub;





    private int currentPhase;  //0 instroduction, 1 interaction, 2 feedback
    public bool userInRightIsle;








    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        currentPhase=0;
        userInRightIsle=false;
        conversationState=0;
        introduced=false;

        CreateDummies();
        CreateBubbles();
       
        
        userBub.WriteInBubble("");
        clientBub.WriteInBubble( "");
        

        WriteOnBubble("Phase: 0");
    }

    private void CreateDummies(){
        User=Instantiate(UserSimPref, new Vector3(6.3f,7.5f,410), new Quaternion(0,280f,0, (float)Space.Self));
        Rigidbody trig   =User.AddComponent<Rigidbody>();
        trig.useGravity=false;
        User.AddComponent<BoxCollider>();
        userSim=User.GetComponent<PlayerMovementSimultor>();
        userSim.Setup(1);
        User.transform.rotation=new Quaternion(0,280f,0, (float)Space.Self);

        Trainer=Instantiate(UserSimPref, new Vector3(-1,7.5f,410), new Quaternion(0,95f,0, (float)Space.Self));
        trainerSim=Trainer.GetComponent<PlayerMovementSimultor>();
        trainerSim.Setup(2);
        Trainer.transform.rotation=new Quaternion(0,95f,0, (float)Space.Self);

    }
    private void CreateBubbles(){
        globalBubble= Instantiate(bubblePrefab , new Vector3(0,11,400) , Quaternion.identity).GetComponent<BubbleBehaviour>();
        globalBubble.transform.position=new Vector3(-5,15,400);

        userBub= Instantiate(bubblePrefab , new Vector3(0,11,400) , Quaternion.identity).GetComponent<BubbleBehaviour>();
        userBub.SetDimension(2);
        userBub.SetFollow(User);

        clientBub= Instantiate(bubblePrefab , new Vector3(0,11,400) , Quaternion.identity).GetComponent<BubbleBehaviour>();
        clientBub.SetDimension(2);
        clientBub.SetFollow(Trainer);
        
    }


    private int conversationState;

    void Update()
    {

        

    }


    private bool introduced;
    private void PlayIntroduction(){
        if(!introduced){
            writeText(introduction); 
            introduced=true;
        }
        else writeText(instruction);
    }

    //move users in space toward target position and write smwr; could use events 
     private void Interaction(){//todo x proto :   switch gui text for audio(files on git)
    // Debug.Log(conversationState);
        
            if(conversationState==0  ){
                 clientBub.WriteInBubble("Cliente: Buongiorno, sa dirmi dove sono le patate?");
                 dialog1.Play();
                 conversationState++;
                 }
            else if(conversationState==1 ){
                userBub.WriteInBubble("Utente: Con piacere, seguitemi perfavore. Le patate sono nel reparto verdure");
                dialog3.Play();
                conversationState++;
            }
            else if(conversationState==2 && userInRightIsle ){
                clientBub.WriteInBubble("Cliente: Grazie mille");
                userBub.WriteInBubble("");
                dialog2.Play();
                conversationState++;
                
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
                        userBub.WriteInBubble("");
                        clientBub.WriteInBubble("");
                        PlayFeedback();
                    break;
                    //default:
                }
        }
        
    
        if (GUILayout.Button("ChangePhase")){
            userBub.WriteInBubble("");
            clientBub.WriteInBubble("");

            currentPhase++;
            conversationState=0;
            writeText("Phase: "+currentPhase);

            if(currentPhase==3)
                writeText("end of proto");
        }
    }
    public void WriteOnBubble(string text){
        globalBubble.WriteInBubble(text);
    }
   
}
