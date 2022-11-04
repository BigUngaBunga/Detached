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
        GameObject otherObject = other.gameObject;
        if (other.gameObject.CompareTag("Player"))
        {
            ServerChangeScene(SceneManager.GetActiveScene().name);
        }
        if (otherObject.CompareTag("Leg") || otherObject.CompareTag("Head") || otherObject.CompareTag("Arm"))
        {
            if (otherObject.transform.parent.gameObject.TryGetComponent(out SceneObjectItemManager sceneObject))
                sceneObject.HandleFallOutOfWorld();
        }
    }

    [Server]
    void ServerChangeScene(string sceneName)
    {
        Manager.ServerChangeScene(sceneName);
    }
}
