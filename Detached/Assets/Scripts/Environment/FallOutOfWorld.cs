using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallOutOfWorld : NetworkBehaviour
{
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ServerChangeScene(SceneManager.GetActiveScene().name);
        }
    }

    [Server]
    void ServerChangeScene(string sceneName)
    {
        Manager.ServerChangeScene(sceneName);
    }
}
