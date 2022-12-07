using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LegoColourRandomizer : NetworkBehaviour
{
    [SerializeField] private List<Material> materials = new List<Material>();
    [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();


    [Server]
    void Start()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            RPCUpdateMaterial(Random.Range(0, materials.Count), i);
        }
    }

    [ClientRpc]
    private void RPCUpdateMaterial(int materialIndex, int objectIndex)
    {
        var renderers = gameObjects[objectIndex].GetComponentsInChildren<Renderer>();
        if (gameObjects[objectIndex].TryGetComponent(out Renderer renderer))
            renderer.material = materials[materialIndex];
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = materials[materialIndex];
    }
}
