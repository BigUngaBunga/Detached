using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class LegoColourRandomizer : NetworkBehaviour
{
    [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();
    [SyncVar] private int seed;
    private Object[] materials;

    private void Awake()
    {
        materials = Resources.LoadAll("Lego Materials", typeof(Material));
        if (gameObjects.Count <= 0)
            for (int i = 0; i < gameObject.transform.childCount; i++)
                if (gameObject.transform.GetChild(i).gameObject.activeSelf)
                    gameObjects.Add(gameObject.transform.GetChild(i).gameObject);
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
            UpdateMaterial(Random.Range(0, materials.Length), i);
    }

    private void UpdateMaterial(int materialIndex, int objectIndex)
    {
        var renderers = gameObjects[objectIndex].GetComponentsInChildren<Renderer>();
        Material material = materials[materialIndex] as Material;
        if (gameObjects[objectIndex].TryGetComponent(out Renderer renderer))
            SetMaterials(renderer);

        for (int i = 0; i < renderers.Length; i++)
            SetMaterials(renderers[i]);

        void SetMaterials(Renderer renderer)
        {
            var renderMaterials = renderer.materials;
            for (int i = 0; i < renderMaterials.Length; i++)
                renderMaterials[i] = material;
            renderer.materials = renderMaterials;
        }
    }
}
