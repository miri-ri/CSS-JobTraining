using TMPro;
using UnityEngine;

public class TextCloud : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    //public GameObject background;
    [SerializeField] TextMeshProUGUI text;
 
    void Update() { 
      
  
    }
    public void ShowTextUI(){
        gameObject.GetComponent<CanvasGroup>().alpha=1;
    }
    public void HideTextUI(){
        gameObject.GetComponent<CanvasGroup>().alpha=0;
    }
    public void WriteText(string txt){
        text.text=txt;
    }
}
