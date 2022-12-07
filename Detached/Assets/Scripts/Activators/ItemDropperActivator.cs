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
        if(isServer)
            InstantiateObject();
    }

    [Server]
    private void InstantiateObject()
    {
        if (currentInstantiation != null)
            NetworkServer.Destroy(currentInstantiation);
        currentInstantiation = Instantiate(prefab, dropLocation.position, prefab.transform.rotation, interactableFolder);
        NetworkServer.Spawn(currentInstantiation);
        //RPCMoveToParent(currentInstantiation, dropLocation.position, prefab.transform.rotation);
    }

    //TODO lägg till InteractableFolder som förälder till currentInstantiation
    //[ClientRpc]
    //private void RPCMoveToParent(GameObject gameObject, Vector3 objectPosition, Quaternion rotation)
    //{
    //    currentInstantiation = gameObject;
    //    currentInstantiation.transform.parent = interactableFolder;
    //    currentInstantiation.transform.position = objectPosition;
    //    currentInstantiation.transform.rotation = rotation;
    //}
}
