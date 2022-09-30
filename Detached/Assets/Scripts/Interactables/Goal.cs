using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private int playerNumber;

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
            Debug.Log("The players won");
            Debug.Break();
        }
            
    }
}
