using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Goal : NetworkBehaviour
{
    [SerializeField] private int playerNumber;
    [SerializeField] private string nextScene;

    public string[] Maps;
    public int NextMapIndex;

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
            if (CheckVictoryStatus())
            {
                ServerChangeScene(Maps[NextMapIndex]);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerNumber--;
    }

    private bool CheckVictoryStatus()
    {
        if (playerNumber >= 2)
        {
            Debug.Log("The players won");
            return true;
        }
        return false;
    }
}
