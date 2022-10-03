using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Goal : NetworkBehaviour
{
    [SerializeField] private int playerNumber;

     public LevelChanging levelChanging;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerNumber++;
            if (CheckVictoryStatus())
            {
                levelChanging.cmdServerChangeScene();
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
