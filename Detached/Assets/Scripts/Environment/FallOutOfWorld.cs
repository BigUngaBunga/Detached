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
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Torso"))
        {
            ServerChangeScene(SceneManager.GetActiveScene().name);
        }
        else if (other.gameObject.CompareTag("Leg") || other.gameObject.CompareTag("Head") || other.gameObject.CompareTag("Arm"))
        {
            var SceneObject = other.gameObject.GetComponentInParent<SceneObjectItemManager>();
            if (SceneObject != null)
                SceneObject.HandleFallOutOfWorld();

        }
    }

    [Server]
    void ServerChangeScene(string sceneName)
    {
        Manager.ServerChangeScene(sceneName);
    }
}
