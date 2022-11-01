using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Goal : NetworkBehaviour
{
    [SerializeField] private int playerNumber;
    [SerializeField] private int NextMapIndex;
    [SerializeField] private int numOfLimbsRequired;
    [SerializeField] private bool sameNumLimbInAsOut;
    public bool isLocked;

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

    private void Start()
    {
        if (sameNumLimbInAsOut)
        {
            GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnLocation");

            if (spawnLocations.Length != 2) Debug.Log("Missing one or more spawnLocations, possibly missing tags.");

            foreach(GameObject spawnLocation in spawnLocations)
            {
                numOfLimbsRequired += spawnLocation.GetComponent<SpawnPoint>().numOfLimbsAtSpawn;
            }
        }
        
        Debug.Log("Number of levels: " + GlobalLevelIndex.levelNames.Length);
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

    private bool CheckVictoryStatus()
    {
        if (!isLocked && playerNumber >= 2 && EvaluateLimbsOnPlayers())
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

    public void EvaluateVictory()
    {
        if (CheckVictoryStatus())
            ServerChangeScene(GlobalLevelIndex.levelNames[NextMapIndex]);
    }
}
