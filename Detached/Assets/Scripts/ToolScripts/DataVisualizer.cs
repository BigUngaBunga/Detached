using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    //The colors and the ranges in where those colors will apply
    //Color[] colors;
    //float[] pointranges;

    public LineRenderer line;

    void Start()
    {
        DontDestroyOnLoad(this);
        filePath = "Assets/Resources/test.txt";
        ReadFile();
        HeatmapDraw();
        line.positionCount = dataList.Count;

        //colors = new Color[5];
        //pointranges = new float[5];

        //colors[0] = new Color(0, 0, 0, 1);//Black
        //colors[1] = new Color(0, 0.9f, 0.2f, 1);//Green
        //colors[2] = new Color(0.9f, 1, 0.3f, 1);//Yellowish green
        //colors[3] = new Color(0.9f, 0.7f, 0.1f, 1);//Orange
        //colors[4] = new Color(1, 0, 0, 1);//Red

        //pointranges[0] = 0;
        //pointranges[1] = 0.15f;
        //pointranges[2] = 0.35f;
        //pointranges[3] = 0.50f;
        //pointranges[4] = 0.75f;
        LineDraw();
    }

    // Update is called once per frame
    void Update()
    {
        //LineDraw();
    }

    void LineDraw()
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            line.SetPosition(i, dataList[i].playerPosition);
        }
        
    }

    Color GetColorForLine(float weight)
    {
        //if (weight <= pointranges[0])
        //{
        //    return colors[0];
        //}
        //if (weight >= pointranges[4])
        //{
        //    return colors[4];
        //}

        //for (int i = 1; i < 5; i++)
        //{
        //    if (weight < pointranges[i])
        //    {
        //        float dist_from_lower_point = weight - pointranges[i - 1];
        //        float size_of_point_range = pointranges[i] - pointranges[i - 1];

        //        float ratio_over_lower_point = dist_from_lower_point / size_of_point_range;

        //        Color color_range = colors[i] - colors[i - 1];
        //        Color color_contribution = color_range * ratio_over_lower_point;

        //        Color new_color = colors[i - 1] + color_contribution;

        //        return new_color;
        //    }
        //}

        //return colors[0];
        return Color.black;
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

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
