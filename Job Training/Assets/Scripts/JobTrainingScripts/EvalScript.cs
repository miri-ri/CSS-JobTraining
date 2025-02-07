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
    }
    public void AddFeedbackIcons(EvaluationResponse eval){
        List<Sprite> iconType, iconVal;
        Sprite star=Resources.Load<Sprite>("icons/star");
        string[] descr={"","","","","",""};
        int[] xPos={-360, -240, -120,  120 , 240 ,360};

        iconVal=new();
        iconType=new();
        List<GameObject> scoreList ;
        scoreList=new();

        for (int i = 0; i < 3; i++)
        {
            iconVal.Add( Resources.Load<Sprite>("icons/val"+i));
        }
        for (int i = 0; i < 6; i++)
        {
            iconVal.Add( Resources.Load<Sprite>("icons/t"+i));
        }


       
        
        int j=0;
        foreach (var item in eval.Evaluations)//with references in case we want to add some kind of animations
        {
            scoreList.Add( Instantiate(partialScore));
            scoreList[j].transform.GetChild(0).GetComponent<Image>().sprite=iconType[j];
            scoreList[j].transform.GetChild(2).GetComponent<TextMeshPro>().text=descr[j];

            int valD;
           if(item.Score>7) valD=2;
           else if(item.Score>3) valD=1;
           else valD=0;
            scoreList[j].transform.GetChild(3).GetComponent<Image>().sprite=iconVal[valD];

            scoreList[j].transform.position=new(xPos[j],-70);
        }


        int[] starPosX={-130,-65,0,65,130};
        for (int i = 0; i < eval.Total; i++)
        {
            GameObject st= Instantiate(Stare);
            st.transform.position=new(starPosX[i],58);
        }//58y 


    }
}
