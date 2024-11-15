using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToClimbingWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// message sent by HoldManager on Hit after grasp
    /// </summary>
    void TractionIdentified()
    {
        Debug.Log("I was hit by a Ray");
    }
}
