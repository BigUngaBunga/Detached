using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UIElements;

public class Goal : NetworkBehaviour
{

    [SerializeField] private int playerNumber;
    [SerializeField] private bool sameNumLimbInAsOut = true;
    private int numOfLimbsRequired = 0;
    public bool isLocked;
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


    protected virtual void Start()
    {
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
        if (other.gameObject.CompareTag("Player"))
        {
            playerNumber++;
            EvaluateVictory();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
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
