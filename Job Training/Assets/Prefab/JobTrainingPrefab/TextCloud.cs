using TMPro;
using UnityEngine;

public class TextCloud : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public RectTransform background;
    //public GameObject background;
    public TextMeshPro text;
 
    void Update() { 
      
  
    }
    public void WriteText(string txt){
        text.text=txt;
    }
}
