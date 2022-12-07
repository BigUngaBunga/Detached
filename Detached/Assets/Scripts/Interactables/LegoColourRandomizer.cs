using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LegoColourRandomizer : NetworkBehaviour
{
    [SerializeField] private List<Material> materials = new List<Material>();
    [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();
    [SyncVar] private int seed;

    private void Awake()
    {
        if (!isClientOnly)
            SetSeed();
    }

    [Server]
    private void SetSeed()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
    }

    void Start()
    {
        Random.InitState(seed);
        for (int i = 0; i < gameObjects.Count; i++)
            UpdateMaterial(Random.Range(0, materials.Count), i);
    }

    private void UpdateMaterial(int materialIndex, int objectIndex)
    {
        var renderers = gameObjects[objectIndex].GetComponentsInChildren<Renderer>();
        if (gameObjects[objectIndex].TryGetComponent(out Renderer renderer))
            renderer.material = materials[materialIndex];
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = materials[materialIndex];
    }
}
