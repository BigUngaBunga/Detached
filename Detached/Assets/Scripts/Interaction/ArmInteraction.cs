using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArmInteraction : MonoBehaviour
{
    public SceneObjectItemManager SceneObjectItemManager { private get; set; }
    private List<GameObject> interactableObjects = new List<GameObject>();
    private List<HighlightObject> highlighters = new List<HighlightObject>();
    private bool interacting;

    private void Start()
    {
        var collider = gameObject.AddComponent <SphereCollider> ();
        collider.isTrigger = true;
        collider.radius = 3;
    }

    private void Update()
    {
        if (!SceneObjectItemManager.isBeingControlled && !SceneObjectItemManager.isLocalPlayer)
            return;

        foreach (var highlighter in highlighters)
            highlighter.DurationHighlight();

        if (Input.GetKeyDown(KeyCode.E))
            GetClosestInteractable().Interact(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidInteractable(other.gameObject, out IInteractable interactable))
            if (interactable.CanInteract(gameObject))
                AddObject(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsValidInteractable(other.gameObject, out _))
            RemoveObject(other.gameObject);
    }

    private bool IsValidInteractable(GameObject gameObject, out IInteractable interactable)
    {
        interactable = null;
        bool isInteractableLayer = gameObject.layer == 15;
        bool isInteractable = gameObject.TryGetComponent(out interactable);
        return isInteractableLayer && isInteractable;
    }

    private IInteractable GetClosestInteractable()
    {
        GameObject closestInteractable = null;
        float smallestDistance = float.MaxValue;

        foreach (var gameObject in interactableObjects)
        {
            float distance = Vector3.Distance(base.gameObject.transform.position, gameObject.transform.position);
            if (distance < smallestDistance)
            {
                closestInteractable = gameObject;
                smallestDistance = distance;
            }
        }

        return closestInteractable.GetComponent<IInteractable>();
    }


    private void AddObject(GameObject gameObject)
    {
        interactableObjects.Add(gameObject);
        highlighters.Add(gameObject.GetComponent<HighlightObject>());
    }

    private void RemoveObject(GameObject gameObject)
    {
        interactableObjects.Remove(gameObject);
        highlighters.Remove(gameObject.GetComponent<HighlightObject>());
    }
}
