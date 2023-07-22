using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UIElements;
using System.Security.Cryptography;
using FMODUnity;

public class Goal : NetworkBehaviour
{
    [Header("Debug")]
    [SerializeField] private int playerNumber;
    [SerializeField] private bool sameNumLimbInAsOut = true;
    [SerializeField] private bool skippLevel = false;
    private int numOfLimbsRequired = 0;
    [SyncVar] public bool isLocked;
    [Header("Override variables")]
    [SerializeField] private bool overrideNextMap;
    [SerializeField] private int overrideMapIndex;
    [SerializeField] private bool debugDoor;

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

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.P))
        {   
            string nextScene = GlobalLevelIndex.GetNextLevel();
            ServerChangeScene(nextScene);
        }
    }
#endif

    protected virtual void Start()
    {
        //Change the music intensity
        // Calculate how many tenths of the levels that are completed
        int tenth = (GlobalLevelIndex.GetLevelIndex() * 10) / (SceneManager.sceneCountInBuildSettings - 2);
        int intensity = tenth * 10; // Multiply by 10 to get the intensity. (This should make the (intensity % 10 = 0) I believe)
        if (tenth > 7) { intensity = 70; } // Reserve 30 intensity for when you press a timed button.
        RuntimeManager.StudioSystem.setParameterByName("Intensity", intensity);

        Debug.Log(intensity + "Intensity");

        if (sameNumLimbInAsOut)
        {
            GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnLocation");

            if (spawnLocations.Length != 2) Debug.Log("Missing one or more spawnLocations, possible missing tag.");

            foreach (GameObject spawnLocation in spawnLocations)
            {
                numOfLimbsRequired += spawnLocation.GetComponent<SpawnPoint>().NumOfLimbsAtSpawn;
            }
        }

        //Debug.Log("Number of levels: " + GlobalLevelIndex.levelNames.Length);
    }

    [Server]
    void ServerChangeScene(string sceneName)
    {
        Manager.ServerChangeScene(sceneName);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Torso"))
        {
            playerNumber++;
            EvaluateVictory();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Torso"))
        {
             EvaluateVictory();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Torso"))
            playerNumber--;
    }

    protected bool CheckVictoryStatus()
    {
        if (debugDoor || !isLocked && playerNumber >= 2 && EvaluateLimbsOnPlayers())
        {
            Debug.Log("The players won");
            return true;
        }
        return false;
    }

    private bool EvaluateLimbsOnPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int limbs = 0;
        foreach (GameObject player in players)
        {
            limbs += player.GetComponent<ItemManager>().numberOfLimbs;
        }

        return limbs == numOfLimbsRequired;
    }

    public virtual void EvaluateVictory()
    {
        if (CheckVictoryStatus())
        {
            string nextScene = GlobalLevelIndex.GetNextLevel();
            if (overrideNextMap)
                nextScene = GlobalLevelIndex.GetLevel(overrideMapIndex);
            ServerChangeScene(nextScene);
        }
    }
}
