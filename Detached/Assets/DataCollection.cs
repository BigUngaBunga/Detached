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


    private void Start()
    {
        level = SceneManager.GetActiveScene().name;
        filePath = "Assets/Resources/test.txt";
        triggerStructArray = new TriggerActivationStruct[triggers.Length];
        for (int i = 0; i < triggers.Length; i++)
        {
            triggerStructArray[i].name = triggers[i].gameObject.GetComponent<Data>().name;
            triggerStructArray[i].activations = triggers[i].gameObject.GetComponent<Data>().Activations;
        }
        conn = Manager.GamePlayers[0].connectionToClient;
        gameVersion = 1;
    }
    void Update()
    {
        time += Time.deltaTime;
        player = conn.identity.gameObject;
        playerPosition = player.transform.position;
        for (int i = 0; i < triggers.Length; i++)
        {
            triggerStructArray[i].name = triggers[i].gameObject.GetComponent<Data>().name;
            triggerStructArray[i].activations = triggers[i].gameObject.GetComponent<Data>().Activations;
        }
        WriteString();
    }

    void WriteString()
    {
        //filePath = "Assets/Resources/test" + level + gameVersion.ToString() +".txt";
        StreamWriter writer = new StreamWriter(filePath, true);
        string output = "";
        string triggerActivations = "";
        foreach (TriggerActivationStruct trigger in triggerStructArray)
        {
            triggerActivations += trigger.name + trigger.activations.ToString() + ";";
        }
        output = level + ";" + gameVersion + ";" + time + ";" + triggerActivations + playerPosition.x.ToString() + ";" + playerPosition.y.ToString() + ";" + playerPosition.z.ToString() + ";";
        writer.WriteLine(output);
        writer.Close();
    }
}
