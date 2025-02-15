using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class inputForJobTraining : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    [SerializeField] TMP_InputField  addr;
    [SerializeField] TMP_InputField  isMagic;
    public void setVals(){
        StartValues.JTSaddress=addr.text;
        StartValues.realMR=isMagic.text.ToLower()=="true";

    }
}
