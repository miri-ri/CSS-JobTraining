using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvalScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    [SerializeField] GameObject partialScore;
    [SerializeField] GameObject Stare;
    // Update is called once per frame
    void Update()
    {
        
    }
    public void FlushPanel(){
        //todo, destroy objs, or set alpha=0
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
      
    }
    List<GameObject> scoreList ;
    public void AddFeedbackIcons(EvaluationResponse eval){
        List<Sprite> iconType, iconVal;
        Sprite star=Resources.Load<Sprite>("icons/star");
        string[] descr={"Qualità Risposta","Tempo di Riflessione","Velocità di Parola","Accuratezza Movimenti","Prontezza Movimenti","Velocità di spostamento"};
        int[] xPos={-360, -240, -120,  120 , 240 ,360};

        iconVal=new();
        iconType=new();
        
        scoreList=new();

        for (int i = 0; i < 3; i++)
        {
            iconVal.Add( Resources.Load<Sprite>("icons/val"+i));
        }
        for (int i = 1; i < 7; i++)
        {
            iconType.Add( Resources.Load<Sprite>("icons/t"+i));
        }


       
        
        int j=0;
        foreach (var item in eval.Evaluations)//with references in case we want to add some kind of animations
        {

            scoreList.Add( Instantiate(partialScore,  transform));
            scoreList[j].transform.localPosition = Vector3.zero;
            scoreList[j].transform.localScale = Vector3.one;
            Sprite temp=iconType[j];
            scoreList[j].transform.Find("type").GetComponent<Image>().sprite=temp;
            string txt=descr[j];
            Transform txtChild=scoreList[j].transform.Find("text");
            txtChild.GetComponent<TextMeshProUGUI>().text=txt;

            int valD;
           if(item.Score>7) valD=2;
           else if(item.Score>3) valD=1;
           else valD=0;
            scoreList[j].transform.Find("val").GetComponent<Image>().sprite=iconVal[valD];

            scoreList[j].transform.localPosition=new(xPos[j],-70, 0);
            j++;
        }

        

        int[] starPosX={-130,-65,0,65,130};
        Debug.Log(eval.Total);
        for (int i = 0; i < (int)Math.Floor(eval.Total/2); i++)
        {
            GameObject st= Instantiate(Stare,  transform);
            st.transform.localPosition = Vector3.zero;
            st.transform.localScale = Vector3.one;
            st.transform.localPosition=new(starPosX[i],58, 0);
        }//58y 


    }
}
