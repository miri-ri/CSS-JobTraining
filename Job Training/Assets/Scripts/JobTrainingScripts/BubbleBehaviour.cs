using TMPro;
using UnityEngine;

public class BubbleBehaviour : MonoBehaviour
{

    public GameObject follows;
    private bool attachedToCharacter=false;
    void Start()
    {
        //attachedToCharacter=false;
    }
    public void SetFollow(GameObject follow){
        follows=follow;
        attachedToCharacter=true;
    }
    public void SetDimension(int fontSize){
        GetComponentInChildren<TextMeshPro>().fontSize=fontSize;
    }
    public void WriteInBubble(string text){
        GetComponentInChildren<TextMeshPro>().text=text;
    }
    // Update is called once per frame
    void Update()
    {
        if(attachedToCharacter)
            transform.position=new Vector3(follows.transform.position.x,11,follows.transform.position.z+0.5f) ;
    }
}
