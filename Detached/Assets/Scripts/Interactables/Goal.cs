using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Goal : NetworkBehaviour
{
    [SerializeField] private int playerNumber;

    //Manager
    private CustomNetworkManager manager;
    public string GameScene;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerNumber++;
            if (CheckVictoryStatus())
            {
                //manager.ChangeScene(SceneManager.GetActiveScene().buildIndex + 1);
                manager.ServerChangeScene("Through The Paine - Level 2");
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
