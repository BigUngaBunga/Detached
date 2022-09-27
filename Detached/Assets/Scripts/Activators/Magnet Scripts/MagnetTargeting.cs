using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetTargeting : MonoBehaviour
{
    private List<GameObject> presentPlayers = new List<GameObject>();
    private bool PlayerIsPresent => presentPlayers.Count > 0;

    public void Update()
    {
        if (PlayerIsPresent)
        {
            transform.LookAt(presentPlayers[0].transform);
            transform.eulerAngles += new Vector3(90, 0, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            presentPlayers.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            presentPlayers.Remove(other.gameObject);
    }
}
