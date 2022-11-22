using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UIElements;

public class Goal : NetworkBehaviour
{
    public bool isLocked;
    [SerializeField] private int playerNumber;
    [Header("Override variables")]
    [SerializeField] private bool overrideNextMap;
    [SerializeField] private int overrideMapIndex;

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
        {
            string nextScene = GlobalLevelIndex.GetNextLevel();
            if (overrideNextMap)
                nextScene = GlobalLevelIndex.GetLevel(overrideMapIndex);
            ServerChangeScene(nextScene);
        }
    }
}
