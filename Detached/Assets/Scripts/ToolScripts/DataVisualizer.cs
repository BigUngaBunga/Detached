using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataVisualizer : MonoBehaviour
{

    public QuadScript Target;
    private struct TriggerActivationStruct
    {
        public string name;
        public int activations;
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

    void Start()
    {
        filePath = "Assets/Resources/test.txt";
        ReadFile();
        HeatmapDraw();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HeatmapDraw()
    {
        //Separate the data for the heatmap into 64 different clumps of data that will be averaged out so that the shader does not store too much data
        Vector3[] array = new Vector3[64];
        int size = dataList.Count/64;

        for (int i = 0; i < 64; i++)
        {
            Vector3 posSum = new Vector3();
            for (int j = 0; j < size; j++)
            {
                posSum += dataList[i * j].playerPosition;
            }
            posSum = posSum / size;
            posSum.z = 5f;
            posSum = dataList[i * size].playerPosition;
            posSum.z = 5f;
            Vector3 StartOfRay = posSum;
            Vector3 RayDir = Vector3.down;

            Ray ray = new Ray(StartOfRay, RayDir);
            RaycastHit hit;

            bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMap"));

            Debug.Log(" here ");

            if (hitit)
            {
                Debug.Log("Hit object " + hit.collider.gameObject.name);
                Debug.Log("Hit Texture coordinates" + hit.textureCoord.x + "," + hit.textureCoord.y);
                Target.AddHitPoint(hit.textureCoord.x * 4 - 2, hit.textureCoord.y * 4 - 2);
            }
        }


    }

    void ReadFile()
    {
        StreamReader reader = new StreamReader(filePath);
        string ln;

        while ((ln = reader.ReadLine()) != null)
        {
            DataStruct dataStruct = new DataStruct();
            string[] data = ln.Split(';');
            int nmbrOfTriggers = data.Length - 5;
            dataStruct.level = data[0];
            dataStruct.gameVersion = int.Parse(data[1]);
            dataStruct.time = float.Parse(data[2]);
            for (int i = 3; i < nmbrOfTriggers; i+=2)
            {
                TriggerActivationStruct trigger = new TriggerActivationStruct();
                trigger.name = data[i];
                trigger.activations = int.Parse(data[i + 1]);
                dataStruct.triggerStructList = new List<TriggerActivationStruct>();
                dataStruct.triggerStructList.Add(trigger);
            }
            dataStruct.playerPosition = new Vector3(float.Parse(data[data.Length - 3]), float.Parse(data[data.Length - 2]), float.Parse(data[data.Length - 1]));

            dataList.Add(dataStruct);
        }
    }
}
