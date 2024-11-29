using TMPro;
using UnityEngine;

public class FeedbackUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject canvas;
    public void ShowFeedbackUI(){
        enabled=true;
    }
    public void HideFeedbackUI(){
        canvas.SetActive(false);
        //enabled=false;
        Debug.Log("nascosta");
    }

    public void SetText(string message){
        text.text=message;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
