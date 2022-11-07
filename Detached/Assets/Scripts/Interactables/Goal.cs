using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Goal : NetworkBehaviour
{
    [SerializeField] private int playerNumber;
    [SerializeField] private int NextMapIndex;
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
        if (!isLocked && playerNumber >= 2)
        {
            Debug.Log("The players won");
            return true;
        }
        return false;
    }

    public void EvaluateVictory()
    {
        if (CheckVictoryStatus())
            ServerChangeScene(GlobalLevelIndex.levelNames[NextMapIndex]);
    }
}
