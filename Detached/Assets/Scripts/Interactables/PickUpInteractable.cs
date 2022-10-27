using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractable : NetworkBehaviour, IInteractable
{
    [Range(1, 2)]
    [SerializeField] private int requiredArms = 1;
    [SerializeField] private Transform positionTarget;
    [SyncVar] private bool isHeld;

    public int RequiredArms { get { return requiredArms; } }
    public void Interact(GameObject activatingObject)
    {
        activatingObject.GetComponent<InteractableManager>().AttemptPickUpItem(gameObject);
    }

    //public void PickUp(Transform positionTarget) => this.positionTarget = positionTarget;
    [Command(requiresAuthority = false)]
    public void PickUp(Transform positionTarget)
    {
        Debug.Log("New position target: " + positionTarget);
        this.positionTarget = positionTarget;
        isHeld = true;
    }

    [Command(requiresAuthority = false)]
    public void Drop()
    {
        positionTarget = null;
        isHeld = false;
    }

    public void Update()
    {
        if (positionTarget != null)
        {
            transform.position = positionTarget.position;
            transform.rotation = positionTarget.rotation;
        } 
    }

    public bool CanInteract(GameObject activatingObject) => activatingObject.GetComponent<InteractableManager>().CanPickUpItem(gameObject);
}
