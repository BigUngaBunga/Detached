using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataVisualizer : MonoBehaviour
{

    //Manager
    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    public GameObject[] textArray;
    public GameObject textPrefab;

    public QuadScript Target;
    private struct TriggerActivationStruct
    {
        public string name;
        public int activations;
        public float x, y, z;
    }
    private struct DataStruct
    {
        public string level;
        public float time;
        public List<TriggerActivationStruct> triggerStructList;
        public int gameVersion; // ???
        public Vector3 playerPosition;
    }

    private string filePath;
    private List<DataStruct> dataList = new List<DataStruct>();

    public LineRenderer line;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void DrawStart(string filePath)
    {
        this.filePath = filePath;
        Debug.Log(filePath);
        if (!File.Exists(filePath)) return;
        ReadFile();
        HeatmapDraw();
        line.positionCount = dataList.Count;
        LineDraw();
        TriggerDraw();
    }

    // Update is called once per frame
    void Update()
    {

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera == null) return;
        if (camera.activeInHierarchy)
        {
            camera.SetActive(false);
        }
        
    }

    void TriggerDraw()
    {
        if (textArray.Length > 0 )
        {
            for (int i = 0; i < textArray.Length; i++)
            {
                Destroy(textArray[i]);
            }
        }
        textArray = new GameObject[dataList[dataList.Count-1].triggerStructList.Count];
        Debug.Log("count: " + textArray.Length);

        for (int i = 0; i < textArray.Length; i++)
        {
            textArray[i] = Instantiate(textPrefab, new Vector3(dataList[dataList.Count - 1].triggerStructList[i].x,
                dataList[dataList.Count - 1].triggerStructList[i].y,
                dataList[dataList.Count - 1].triggerStructList[i].z), 
                Quaternion.identity);
            textArray[i].GetComponent<TextMesh>().text = dataList[dataList.Count - 1].triggerStructList[i].activations.ToString();

        }
    }

    void LineDraw()
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            line.SetPosition(i, dataList[i].playerPosition);
        }
        
    }

    void HeatmapDraw()
    {
        //Separate the data for the heatmap into 256 different clumps of data that will be averaged out so that the shader does not store too much data
        float[,] weightArray = new float[16,16];
        Vector3 right = Target.gameObject.transform.position + transform.right * Target.transform.localScale.x / 2;
        Vector3 left = Target.gameObject.transform.position + transform.right * -Target.transform.localScale.x / 2;
        Vector3 down = Target.gameObject.transform.position + Vector3.forward * -Target.transform.localScale.z / 2;
        float sizeOfSquares = (right.x - left.x) / 16;
        for (int i = 0; i < dataList.Count-1; i++)
        {
            int x = (int)((dataList[i].playerPosition.x - left.x)/ sizeOfSquares);
            int y = (int)((dataList[i].playerPosition.z - down.z) / sizeOfSquares);
            if (x <0 || x > 15) continue;
            if (y < 0 || y > 15) continue;

            weightArray[x, y]++;
        }

        //used for calculating the weight of a certain square
        float max = 0;
        foreach (int value in weightArray) if (value > max)max = value;

        Vector3 StartOfRay = Vector3.zero;
        Vector3 RayDir = Vector3.down;
        int count = 0;
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                StartOfRay = new Vector3((i*sizeOfSquares + sizeOfSquares/2) + left.x, 6, (j*sizeOfSquares + sizeOfSquares/2)+down.z);
                Ray ray = new Ray(StartOfRay, RayDir);
                RaycastHit hit;

                bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMap"));
                if (hitit)
                {
                    if (count < 255) // skipping last loop to not overflow and reset array.
                    {
                        float weight = (weightArray[i, j] / max) * 15.0f;
                        //Debug.Log("Hit Texture coordinates" + hit.textureCoord.x + "," + hit.textureCoord.y + "," + weight);
                        Target.AddHitPoint(hit.textureCoord.x * 4 - 2, hit.textureCoord.y * 4 - 2, weight);
                    }
                }
                count++;
            }
        }
    }

    void ReadFile()
    {
        dataList.Clear();
        StreamReader reader = new StreamReader(filePath);
        string ln;

        while ((ln = reader.ReadLine()) != null)
        {
            DataStruct dataStruct = new DataStruct();
            string[] data = ln.Split(';');
            dataStruct.level = data[0];
            dataStruct.gameVersion = int.Parse(data[1]);
            dataStruct.time = float.Parse(data[2]);
            for (int i = 3; i < data.Length - 4; i+=5)
            {
                TriggerActivationStruct trigger = new TriggerActivationStruct();
                trigger.name = data[i];
                trigger.activations = int.Parse(data[i + 1]);
                trigger.x = float.Parse(data[i + 2]);
                trigger.y = float.Parse(data[i + 3]);
                trigger.z = float.Parse(data[i + 4]);
                dataStruct.triggerStructList = new List<TriggerActivationStruct>();
                dataStruct.triggerStructList.Add(trigger);
            }
            dataStruct.playerPosition = new Vector3(float.Parse(data[data.Length - 3]), float.Parse(data[data.Length - 2]), float.Parse(data[data.Length - 1]));

            dataList.Add(dataStruct);
        }
    }

    public void ChangeScene(string sceneName)
    {
        ServerChangeScene(sceneName);
    }

    [Server]
    void ServerChangeScene(string sceneName)
    {
        Manager.ServerChangeScene(sceneName);
    }
}
