using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackerClimbingWall : MonoBehaviour
{

    public static TrackerClimbingWall instance;

    private Vector2 distance;
    private Vector2 walldimension;
    private Vector2 gridsize;
    private Vector2 initialshift;
    private GameObject HoldSpawn;
    public Vector2 GameWorldSize;
    private Vector3 origin;

    public bool inCameraSpace;

    public delegate void GraspedHold(int col, int row);
    public delegate void ReleaseddHold(int col, int row);
    public delegate void TractionMeasured(int col, int row, Vector3 traction);
    public static event TractionMeasured TractionEvent;
    public static event GraspedHold GraspedHoldEvent;
    public static event ReleaseddHold ReleasedHoldEvent;

    public int maxthreshold;
    public int minthreshold;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            DestroyImmediate(this);
        }
        HoldSpawn = new GameObject("Climbing Wall Holds");
        HoldSpawn.transform.parent = transform;

        MagicRoomManager.instance.MagicRoomClimbingWallManager.TractionEvent += manageTractionEvents;

        StartCoroutine(waitConfigOk());
    }

    private IEnumerator waitConfigOk() {
        Debug.Log(DateTime.Now + " started");
        yield return new WaitUntil(() => MagicRoomManager.instance.MagicRoomClimbingWallManager.isClimbingWallActive!= null);
        Debug.Log(DateTime.Now + " unlocked");
        SetUpClimbingWall();
    }

    private List<Vector2> alreadyGrasped = new List<Vector2>();

    private void manageTractionEvents(int col, int row, Vector3 traction)
    {
        if (traction.magnitude > maxthreshold)
        {
            GraspedHoldEvent?.Invoke(col, row);
            if (!alreadyGrasped.Contains(new Vector2(col, row)))
            {
                alreadyGrasped.Add(new Vector2(col, row));
            }
        }
        if (traction.magnitude <= minthreshold)
        {
            Debug.LogWarning("(" + row + ", " + col + ") released");
            ReleasedHoldEvent?.Invoke(col, row);
            Vector2 ind = alreadyGrasped.Where(x => x.x == col && x.y == row).FirstOrDefault();
            alreadyGrasped.Remove(ind);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpClimbingWall() {

        if (MagicRoomManager.instance.MagicRoomClimbingWallManager.isClimbingWallActive == true)
        {
            distance = new Vector2(MagicRoomManager.instance.MagicRoomClimbingWallManager.wallParams.distanceX, MagicRoomManager.instance.MagicRoomClimbingWallManager.wallParams.distanceY);
            walldimension = new Vector2(MagicRoomManager.instance.MagicRoomClimbingWallManager.wallParams.wallWidth, MagicRoomManager.instance.MagicRoomClimbingWallManager.wallParams.wallHeight);
            gridsize = new Vector2(MagicRoomManager.instance.MagicRoomClimbingWallManager.wallParams.numRow, MagicRoomManager.instance.MagicRoomClimbingWallManager.wallParams.numCol);
            initialshift = new Vector2(MagicRoomManager.instance.MagicRoomClimbingWallManager.wallParams.initialshiftX, MagicRoomManager.instance.MagicRoomClimbingWallManager.wallParams.initialshiftY);
        }
        else
        {
            //SIMULATOR
            distance = new Vector2(23, 18);
            walldimension = new Vector2(366, 244);
            gridsize = new Vector2(13, 15);
            initialshift = new Vector2(15, 16);

            List<HoldData> simulationHolds = new List<HoldData>() {
                new HoldData(0,7, true),
                new HoldData(2,6, true),
                new HoldData(2,8, true),
                new HoldData(3,4, true),
                new HoldData(3,10, true),
                new HoldData(4,6, true),
                new HoldData(4,8, true),
                new HoldData(5,7, true),
                new HoldData(5,5, true),
                new HoldData(5,9, true),
                new HoldData(7,7, true),
                new HoldData(7,6, true),
                new HoldData(7,8, true),
                new HoldData(9,6, true),
                new HoldData(9,8, true),
                new HoldData(11,5, true),
                new HoldData(11,7, true),
                new HoldData(11,9, true),
                new HoldData(2,13, false),
                new HoldData(2,1, false),
                new HoldData(3,12, false),
                new HoldData(3,2, false),
                new HoldData(5,11, false),
                new HoldData(5,3, false),
                new HoldData(7,3, false),
                new HoldData(7,11, false),
                new HoldData(10,3, false),
                new HoldData(10,11, false)
            };
            MagicRoomManager.instance.MagicRoomClimbingWallManager.holds = simulationHolds;
        }
        GenerateRuntimeWall();
    }

    private void GenerateRuntimeWall()
    {
        if (inCameraSpace)
        {
            Camera camera = GameObject.FindGameObjectWithTag("FrontCamera").GetComponent<Camera>();
            Vector3 upperCorner = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, origin.z - camera.transform.position.z));
            Vector3 lowerCorner = camera.ScreenToWorldPoint(new Vector3(0, 0, origin.z - camera.transform.position.z)); //camera.nearClipPlane));
            GameWorldSize = new Vector3(upperCorner.x - lowerCorner.x, upperCorner.y - lowerCorner.y, 1);
            origin = camera.ScreenToWorldPoint(new Vector3(0, camera.pixelHeight, origin.z - camera.transform.position.z));
        }
        Vector3 scaledimension = new Vector3((float)GameWorldSize.x / (float)walldimension.x, (float)GameWorldSize.y / (float)walldimension.y, 1);
        Vector3 scaledelta = Vector3.Scale(scaledimension, new Vector3(distance.x, distance.y, 1));
        Vector3 scaleinitialshift = Vector3.Scale(scaledimension, new Vector3(initialshift.x, -initialshift.y, 0));

        for (int i = 0; i < gridsize.x; i++)
        {
            for (int j = 0; j < gridsize.y; j++)
            {
                //forse si potrebbeor non spawnaer proprio?
                /*GameObject spawned = new GameObject("Hold (" + i + ", " + j + ")");
                spawned.transform.parent = HoldSpawn.transform;

                spawned.transform.position = origin + scaleinitialshift + Vector3.Scale(new Vector3(j, -i, 0), scaledelta);*/
                HoldData hd = MagicRoomManager.instance.MagicRoomClimbingWallManager.holds.Where(x => x.row == i && x.column == j).FirstOrDefault();
                if (hd != null)
                {
                    GameObject spawned = new GameObject("Hold (" + i + ", " + j + ")");
                    spawned.transform.parent = HoldSpawn.transform;

                    spawned.transform.position = origin + scaleinitialshift + Vector3.Scale(new Vector3(j, -i, 0), scaledelta);
                    HoldManager hm = (HoldManager)spawned.AddComponent(typeof(HoldManager));
                    hm.Setup(j, i, hd.isSensorized);

                    if (MagicRoomManager.instance.MagicRoomClimbingWallManager.isClimbingWallActive == false) {
                        GameObject g = GameObject.Instantiate(Resources.Load("Simulation/HoldSimulator") as GameObject);
                        g.transform.position = spawned.transform.position - Vector3.forward * 1;
                        g.name = "HoldSimulation (" + j + "," + i + ")";
                        g.GetComponent<HoldSimulator>().col = j;
                        g.GetComponent<HoldSimulator>().row = i;
                        g.GetComponent<HoldSimulator>().isSensorized = hd.isSensorized;
                    }
                }
            }
        }
    }
}
