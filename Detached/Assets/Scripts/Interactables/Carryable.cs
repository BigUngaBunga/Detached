using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : NetworkBehaviour, IInteractable
{
    [Range(1, 2)]
    [SerializeField] protected int requiredArms = 1;
    [SerializeField] private Transform positionTarget;
    [SyncVar] private bool isHeld;

    public int RequiredArms { get { return requiredArms; } }
    public virtual void Interact(GameObject activatingObject)
    {
        activatingObject.GetComponent<InteractableManager>().AttemptPickUpItem(gameObject);
    }

    public void PickUp(Transform positionTarget)
    {
        Debug.Log("New position target: " + positionTarget);
        this.positionTarget = positionTarget;
        isHeld = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void Drop()
    {
        positionTarget = null;
        isHeld = false;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void Update() 
    {
        if (isHeld && positionTarget != null)
            MoveObject();

    }

    [Command(requiresAuthority = false)]
    private void MoveObject() => RPCMoveObject();

    [ClientRpc]
    private void RPCMoveObject()
    {
        if (positionTarget == null)
            return;
        transform.position = positionTarget.position;
        transform.rotation = positionTarget.rotation;
    }

    public virtual bool CanInteract(GameObject activatingObject) => !isHeld && activatingObject.GetComponent<InteractableManager>().CanPickUpItem(gameObject);
}
