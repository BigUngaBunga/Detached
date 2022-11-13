using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : NetworkBehaviour, IInteractable
{
    [Range(1, 2)]
    [SerializeField] protected int requiredArms = 1;
    [SerializeField] protected Transform positionTarget;
    [SyncVar] protected bool isHeld;
    private Collider[] colliders;
    protected new Rigidbody rigidbody;

    public int RequiredArms { get { return requiredArms; } }
    public virtual void Interact(GameObject activatingObject)
    {
        activatingObject.GetComponent<InteractableManager>().AttemptPickUpItem(gameObject);
    }

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
    }

    public void PickUp(Transform positionTarget)
    {
        Debug.Log("New position target: " + positionTarget);
        RPCSetCollision(false);
        RPCSetGravity(false);
        this.positionTarget = positionTarget;
        isHeld = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void Drop()
    {
        isHeld = false;
        if (isClient)
        {
            SetCollision(true);
            SetGravity(true);
        }
        else
        {
            RPCSetCollision(true);
            RPCSetGravity(true);
        }
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void Update() 
    {
        if (isHeld && positionTarget != null)
            MoveObject();

    }

    [ClientRpc]
    private void RPCSetCollision(bool canCollide) => SetCollision(canCollide);

    private void SetCollision(bool canCollide)
    {
        foreach (var collider in colliders)
            collider.enabled = canCollide;
    }

    [Command(requiresAuthority = false)]
    private void MoveObject() => RPCMoveObject(positionTarget.position, positionTarget.rotation);

    [ClientRpc]
    protected void RPCMoveObject(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    [ClientRpc]
    private void RPCSetGravity(bool useGravity) => SetGravity(useGravity);

    private void SetGravity(bool useGravity)
    {
        rigidbody.useGravity = useGravity;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public virtual bool CanInteract(GameObject activatingObject) => !isHeld && activatingObject.GetComponent<InteractableManager>().CanPickUpItem(gameObject);
}
