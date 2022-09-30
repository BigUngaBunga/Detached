using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropperActivator : Activator
{
    [Header("Item dropper fields")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject currentInstantiation;
    [SerializeField] private Transform dropLocation;
    [SerializeField] private Transform interactableFolder;
    protected override void Activate()
    {
        base.Activate();
        InstantiateObject();
    }

    private void InstantiateObject()
    {
        if (currentInstantiation != null)
            Destroy(currentInstantiation);
        currentInstantiation = Instantiate(prefab, interactableFolder, true);
        currentInstantiation.transform.position = dropLocation.position;
    }
}
