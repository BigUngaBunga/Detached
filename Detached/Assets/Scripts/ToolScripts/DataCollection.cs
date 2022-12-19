using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataCollection : MonoBehaviour
{
    //Triggers
    public GameObject[] triggers;

    //Player
    private GameObject player;
    private NetworkConnectionToClient conn;

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

    private struct TriggerStruct
    {
        public string name;
        public int activations;
        public float x, y, z;
    }

    private string filePath;
    private string level = "";
    private float time = 0;
    private TriggerStruct[] triggerStructArray;
    private int gameVersion; // ???
    private Vector3 playerPosition;


    private void Start()
    {
        // Check if a folder exists in the current project's Assets folder
        bool folderExists = Directory.Exists(Application.dataPath + "/DebugTool");

        if (!folderExists)
        {
            Directory.CreateDirectory(Application.dataPath + "/DebugTool");
        }


        level = SceneManager.GetActiveScene().name;
        gameVersion = 1;
        filePath = Application.dataPath + "/DebugTool/" + level + " " + gameVersion.ToString() + ".txt";
        while (File.Exists(filePath))
        {
            gameVersion++;
            filePath = Application.dataPath + "/DebugTool/" + level + " " + gameVersion.ToString() + ".txt";
        }
        triggerStructArray = new TriggerStruct[triggers.Length];
        for (int i = 0; i < triggers.Length; i++)
        {
            triggerStructArray[i].name = triggers[i].gameObject.GetComponent<Data>().name;
            triggerStructArray[i].activations = triggers[i].gameObject.GetComponent<Data>().Activations;
            triggerStructArray[i].x = triggers[i].gameObject.GetComponent<Data>().position.x;
            triggerStructArray[i].y = triggers[i].gameObject.GetComponent<Data>().position.y;
            triggerStructArray[i].z = triggers[i].gameObject.GetComponent<Data>().position.z;
        }
        conn = Manager.GamePlayers[0].connectionToClient;

    }
    void Update()
    {
        ///TODO
        /// Change this from checking the players position to just checking the camera position since the camera will now be 
        /// attached to limbs and in turn will better represent where the player is actually getting stuck.
        time += Time.deltaTime;
        if (conn != null)
        {
            player = conn.identity.gameObject;
            playerPosition = player.transform.position;
            for (int i = 0; i < triggers.Length; i++)
            {
                triggerStructArray[i].name = triggers[i].gameObject.GetComponent<Data>().name;
                triggerStructArray[i].activations = triggers[i].gameObject.GetComponent<Data>().Activations;
                triggerStructArray[i].x = triggers[i].gameObject.GetComponent<Data>().position.x;
                triggerStructArray[i].y = triggers[i].gameObject.GetComponent<Data>().position.y;
                triggerStructArray[i].z = triggers[i].gameObject.GetComponent<Data>().position.z;
            }
            WriteString();
        }
    }

    void WriteString()
    {
        Debug.Log("Writing to: " + filePath);
        StreamWriter writer = new StreamWriter(filePath, true);
        string output = "";
        string triggerValues = "";
        foreach (TriggerStruct trigger in triggerStructArray)
        {
            triggerValues += trigger.name + ";" + trigger.activations.ToString() + ";" + trigger.z + ";" + trigger.y + ";" + trigger.z + ";";
        }
        output = level + ";" + gameVersion + ";" + time + ";" + triggerValues + playerPosition.x.ToString() + ";" + playerPosition.y.ToString() + ";" + playerPosition.z.ToString();
        writer.WriteLine(output);
        writer.Close();
    }
}