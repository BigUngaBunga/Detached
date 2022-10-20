using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataVisualizer : MonoBehaviour
{

    private struct TriggerActivationStruct
    {
        public string name;
        public int activations;
    }

    private string filePath;
    private string level = "";
    private float time = 0;
    private TriggerActivationStruct[] triggerStructArray;
    private int gameVersion; // ???
    private Vector3 playerPosition;
    void Start()
    {
        filePath = "Assets/Resources/test.txt";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadFile()
    {

    }
}
