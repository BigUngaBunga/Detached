using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private int playerNumber;
    [SerializeField] private string nextScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerNumber++;
            CheckVictoryStatus();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerNumber--;
    }

    private void CheckVictoryStatus()
    {
        if (playerNumber >= 2)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
