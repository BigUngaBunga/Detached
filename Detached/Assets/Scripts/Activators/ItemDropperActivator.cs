using Mirror;
using UnityEngine;

public class ItemDropperActivator : Activator
{
    [Header("Item dropper fields")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject currentInstantiation;
    [SerializeField] private Transform dropLocation;
    [SerializeField] private Transform interactableFolder;

    protected override void Start()
    {
        base.Start();
        if (interactableFolder == null)
            interactableFolder = GameObject.Find("Interactables").transform;
    }

    protected override void Activate()
    {
        base.Activate();
        InstantiateObject();
    }

    [Command(requiresAuthority = false)]
    private void InstantiateObject()
    {
        if (currentInstantiation != null)
            NetworkServer.Destroy(currentInstantiation);
        currentInstantiation = Instantiate(prefab, interactableFolder);
        currentInstantiation.transform.position = dropLocation.position;
        NetworkServer.Spawn(currentInstantiation);
    }
}
