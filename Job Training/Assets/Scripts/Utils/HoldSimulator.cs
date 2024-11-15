using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldSimulator : MonoBehaviour
{

    public float MaxTraction = 30;
    public float CurrentTraction = 0;
    public float streanghtStep = 0.5f;
    public float range;
    public int row;
    public int col;
    public bool isSensorized;
    private Camera cam;
    private bool keep = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("FrontCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam != null && isSensorized)
        {
            Vector3 position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - cam.transform.position.z));
           
            float dist1 = Mathf.Abs(position.x - transform.position.x);
            float dist2 = Mathf.Abs(position.y - transform.position.y);

            if (dist1 < range && dist2 < range)
            {
                if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.LeftShift))
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        Debug.Log(gameObject.name + " " + CurrentTraction);
                        CurrentTraction += streanghtStep;
                        if (CurrentTraction >= MaxTraction)
                        {
                            CurrentTraction = MaxTraction;
                        }
                    }
                    MagicRoomManager.instance.MagicRoomClimbingWallManager.setTraction(col, row, Vector3.forward * CurrentTraction);
                    if (Input.GetKey(KeyCode.LeftShift)) {
                        keep = true;
                    }
                }
                else
                {
                    MagicRoomManager.instance.MagicRoomClimbingWallManager.setTraction(col, row, Vector3.zero);
                    CurrentTraction = 0;
                    keep = false;
                }
            }
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                keep = false;
            }
        }
        else {
            cam = GameObject.FindGameObjectWithTag("FrontCamera").GetComponent<Camera>();
        }
    }
}
