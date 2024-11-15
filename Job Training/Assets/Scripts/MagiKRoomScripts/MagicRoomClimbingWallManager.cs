using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class MagicRoomClimbingWallManager : MonoBehaviour
{

    public List<HoldData> holds;
    public bool? isClimbingWallActive = null;
    private const string endpoint = "ClimbingWallPointer";

    private readonly string address = "http://localhost:7083";

    public WallConfig wallParams;

    public delegate void TractionMeasured(int col, int row, Vector3 traction);
    public event TractionMeasured TractionEvent;

    /// <summary>
    /// TODO:
    /// AGGIUNGERE IL SIMULATORE
    /// </summary>


    private void Awake()
    {
        holds = new List<HoldData>();
    }

    void Start()
    {
        //MagicRoomManager.instance.Logger.AddToLogNewLine("ServerClimbingWall", "Searching Climbing Wall Server");
        MagicRoomManager.instance.HttpListenerForMagiKRoom.RequestHandlers.Add(new Regex(@"^/" + endpoint + @"$"), ManageHttpRequest);
        SendHealthCheck();
        SendConfigurationRequest();
    }

    private void ManageHttpRequest(string message, NameValueCollection query)
    {
        ParseHolds(message);
    }

    public void ParseHolds(string message)
    {
        Debug.LogWarning(message);
        try
        {
            JArray arr = JArray.Parse(message);
            foreach (JObject o in arr)
            {
                HoldData hd = o.ToObject<HoldData>();
                HoldData hdold = holds.Where(x => x.column == hd.column && x.row == hd.row).FirstOrDefault();
                if (hdold != null)
                {
                    hdold.column = hd.column;
                    hdold.row = hd.row;
                    hdold.traction = hd.traction;
                    hdold.isSensorized = hd.isSensorized;
                    setTraction(hd.column, hd.row, hd.traction);
                }
                else
                {
                    Debug.LogError("No hold found");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) {
            StartStreamingHolds(500);
        }
    }

    private void SendConfigurationRequest()
    {
        ClimbibngWallCommand cmd = new ClimbibngWallCommand
        {
            action = "getConfiguration"
        };
        StartCoroutine(SendCommand(cmd, (body) =>
        {
            Debug.Log(body);
            if (body != "not Found")
            {
                HoldsConfig conf = JsonUtility.FromJson<HoldsConfig>(body);
                holds.Clear();
                List<HoldData> hd = new List<HoldData>();

                foreach (HoldConf c in conf.configuration)
                {
                    HoldData d = new HoldData();
                    d.fromConfig(c);
                    hd.Add(d);
                }

                holds.AddRange(hd);

                wallParams = conf.wallParams;
            }
        }));
    }

    

    private void SendHealthCheck()
    {
        ClimbibngWallCommand cmd = new ClimbibngWallCommand
        {
            action = "healthcheck"
        };
        StartCoroutine(SendCommand(cmd, (body) =>
        {
            Debug.Log("body " + body);
            if (body != "not Found") { 
            JObject b = JObject.Parse(body);
            Debug.Log(b);
            isClimbingWallActive = (bool)b["healthcheck"];
            }
            else
            {
                isClimbingWallActive = false;
            }
        }));
    }

    public void StartStreamingHolds(int interval)
    {
        if (isClimbingWallActive == true)
        {
            string listeningAddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpoint;
            ClimbibngWallCommand command = new ClimbibngWallCommand
            {
                action = "WallCommand",
                command = "startStream",
                interval = interval,
                address = listeningAddress
            };
            //MagicRoomManager.instance.Logger.AddToLogNewLine("ServerClimbingWall", "Start Streaming from Climbing Wall");
            StartCoroutine(SendCommand(command));
        }
        else
        {
            /*if (playerSimulator == null)
            {
                skeletons = new Dictionary<ulong, Skeleton>();
                playerSimulator = new PlayerMovementSimultor[6];
                for (int i = 0; i < 6; i++)
                {
                    GameObject g = GameObject.Instantiate(Resources.Load("Simulation/PlayerSimulator") as GameObject);
                    playerSimulator[i] = g.GetComponent<PlayerMovementSimultor>();
                    playerSimulator[i].Setup(i + 1);
                    skeletons.Add((ulong)i, playerSimulator[i].skeleton);
                }
            }
            this.interval = interval / 1000;
            Debug.Log(this.interval);
            timer = interval / 1000;*/
        }
    }

    public void StopStreamingHolds()
    {
        if (isClimbingWallActive == true)
        {
            string listeningaddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpoint;
            ClimbibngWallCommand command = new ClimbibngWallCommand
            {
                action = "WallCommand",
                command = "stopStream",
                address = listeningaddress
            };
            //MagicRoomManager.instance.Logger.AddToLogNewLine("ServerClimbingWall", "Stop streaming Climbing Wall");
            StartCoroutine(SendCommand(command));
        }
    }

    public void GetData(int row = -1, int col = -1)
    {
        if (isClimbingWallActive == true)
        {
            ClimbibngWallCommand command = new ClimbibngWallCommand
            {
                action = "WallCommand",
                command = "getAllData"
            };

            if(row != -1 && col != - 1){
                command.command = "getData";
                command.row = row;
                command.col = col;
            }
            //MagicRoomManager.instance.Logger.AddToLogNewLine("ServerClimbingWall", "Get Data manually");
            StartCoroutine(SendCommand(command, (body) =>
            {
                Debug.Log(body);
                HoldsDataArray data = JsonUtility.FromJson<HoldsDataArray>(body);
                Debug.Log(data.results[0].traction);
                Debug.Log(holds[2].traction);
                foreach (HoldData hd in data.results)
                {
                    setTraction(hd.column, hd.row, hd.traction);
                }
            }));
        }
    }

    public void setTraction(int col, int row, Vector3 traction) {

        HoldData hdold = holds.Where(x => x.column == col && x.row == row).FirstOrDefault();
        if (hdold != null)
        {
            hdold.column = col;
            hdold.row = row;
            /*string tr = hd.traction;
            tr = tr.Substring(1, tr.Length-2);*/
            hdold.traction = traction;
            TractionEvent?.Invoke(col, row, traction);
        }
        else
        {
            Debug.LogError("No hold found");
        }
    }

    private IEnumerator SendCommand(ClimbibngWallCommand command, MagicRoomManager.WebCallback callback = null)
    {
        string json = JsonUtility.ToJson(command);
        byte[] body = new System.Text.UTF8Encoding().GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(address, "POST")
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (!request.isNetworkError)
        {
            callback?.Invoke(request.downloadHandler.text);
        }
        else {
            callback?.Invoke("not Found");
        }

    }
}

[Serializable]
internal class HoldsConfig
{
    public HoldConf[] configuration;
    public WallConfig wallParams;
}

[Serializable]
public class WallConfig {

    public int distanceX;
    public int distanceY;
    public int wallWidth;
    public int wallHeight;
    public int numCol;
    public int numRow;
    public int initialshiftX;
    public int initialshiftY;
}

[Serializable]
internal class HoldsDataArray
{
    public HoldData[] results;
}
[Serializable]
public class HoldData
{
    public int row;
    public int column;
    public Vector3 traction;
    public bool isSensorized;

    public void fromConfig(HoldConf hc)
    {
        row = hc.row;
        column = hc.column;
        traction = Vector3.zero;
        isSensorized = hc.isSensorized;
    }

    public HoldData(int row, int column, bool isSensorized) {
        this.row = row;
        this.column = column;
        traction = Vector3.zero;
        this.isSensorized = isSensorized;
    }
    public HoldData(int row, int column, bool isSensorized, Vector3 traction)
    {
        this.row = row;
        this.column = column;
        this.traction = traction;
        this.isSensorized = isSensorized;
    }
    public HoldData()
    {
        row = 0;
        column = 0;
        traction = Vector3.zero;
        isSensorized = true;
    }
}

[Serializable]
public class HoldConf
{
    public int row;
    public int column;
    public bool isSensorized;
}

[Serializable]
public class ClimbibngWallCommand
{
    public string action;
    public string command;
    public string address;
    public int interval;
    public int col;
    public int row;
}