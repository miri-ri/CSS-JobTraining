using TMPro;
using UnityEngine;

public class FeedbackUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI FeedbackText;

    [SerializeField] GameObject canvas;
    bool visible;
    public void ToggleHide(){ 
        int i=0;
        if(visible) i=1;
         gameObject.GetComponent<CanvasGroup>().alpha= i;
    }
    

    public void SetText(string message){
        text.text=message;
    }
    public void setFeedback(string message){
        FeedbackText.text=message;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        visible=true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
