using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private int playerNumber;

     public LevelChanging levelChanging;

    public void Start()
    {
        //levelChanging.cmdServerChangeScene();
    }


    //[Server]
    void ServerChangeScene(string sceneName)
    {
        //Manager.ServerChangeScene(sceneName);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerNumber++;
            if (CheckVictoryStatus())
            {

                //ServerChangeScene(Maps[NextMapIndex]);

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
