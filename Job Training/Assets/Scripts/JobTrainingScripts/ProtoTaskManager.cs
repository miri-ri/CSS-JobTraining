using System;
using Mono.Cecil.Cil;
using UnityEngine;

// prototype of task 1: Locate product
// advance by enabling use of syestem's TTS for audio, STT for dialog and kinect capture for movement;
// then advance to using a fully working parametrized manager, with feedback (scoring) system;
// once everything is there, prepare other tasks.

public class ProtoTaskManager : MonoBehaviour
{

    public GameObject UserSimPref;
    public GameObject User;
    public PlayerMovementSimultor userSim;
    public GameObject Trainer;
    public PlayerMovementSimultor trainerSim;


    public string introduction ="introduction lorem ipsummsmsmssmsmsm";
    public string instruction = "instruction lorem ipspspspsspspsp";
    public string feedback = "goodjob lorem ipsummmmm";



    public string[] userDialog;
    public string[] trainerDialog;


    private int currentPhase;  //0 instroduction, 1 interaction, 2 feedback








    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPhase=0;


        User=Instantiate(UserSimPref, new Vector3(2,12,410), Quaternion.identity);
        userSim=User.GetComponent<PlayerMovementSimultor>();
        userSim.Setup(1);

        Trainer=Instantiate(UserSimPref, new Vector3(-1,12,410), Quaternion.identity);
        trainerSim=Trainer.GetComponent<PlayerMovementSimultor>();
        trainerSim.Setup(2);


    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPhase)
        {
            case 0:
                playIntroduction();
            break;

            case 1:
                interaction();
            break;

            case 2:
                playFeedback();
            break;
            //default:
        }
    }


    // write stuff somewhere
    private void playIntroduction(){
        writeText(introduction);
    }

    //move users in space toward target position and write smwr; could use events 
     private void interaction(){

    }
    // write smwr
     private void playFeedback(){

    }

// this would be on audio
    private void writeText(string textMsg){

    }
}
