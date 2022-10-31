using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InteractableManager : NetworkBehaviour
{
    [SyncVar][SerializeField] private bool isCarryingItem;
    [SerializeField] private Transform holdingPosition;
    [SyncVar][SerializeField] private PickUpInteractable carriedItem;
    private ItemManager itemManager;
    public bool IsCarryingItem { get { return isCarryingItem; } }

    private void Start()
    {
        itemManager = GetComponent<ItemManager>();
        itemManager.dropLimbEvent.AddListener(DropIfCantCarry);
    }

    public bool IsCarryingTag(string tag) => isCarryingItem && carriedItem.CompareTag(tag);

    public bool CanPickUpItem(GameObject item) => !isCarryingItem && item.GetComponent<PickUpInteractable>().RequiredArms <= itemManager.NumberOfArms;

    [Command(requiresAuthority = false)]
    public void AttemptPickUpItem(GameObject item)
    {
        if (CanPickUpItem(item))
        {
            isCarryingItem = true;
            ToggleGravity(item);
            carriedItem = item.GetComponent<PickUpInteractable>();
            carriedItem.PickUp(holdingPosition);
        }
    }

    public bool AttemptDropItem(out GameObject item)
    {
        if (isCarryingItem)
        {
            item = carriedItem.gameObject;
            DropItem();
            return true;
        }
        item = null;
        return false;
    }

    public bool AttemptDropItem()
    {
        if (isCarryingItem)
        {
            DropItem();
            return true;
        }
        return false;
    }

    [Command(requiresAuthority = false)]
    private void DropItem()
    {
        RPCDropItem();
    }

    [ClientRpc]
    private void RPCDropItem() 
    {
        isCarryingItem = false;
        if(carriedItem != null)
        {
            carriedItem.Drop();
            ToggleGravity(carriedItem.gameObject);
        }
        
    }

    private void DropIfCantCarry()
    {
        if (isCarryingItem && carriedItem.GetComponent<PickUpInteractable>().RequiredArms > itemManager.NumberOfArms)
            AttemptDropItem();
    }

    private void ToggleGravity(GameObject item)
    {
        item.TryGetComponent(out Rigidbody rigidbody);
        rigidbody.useGravity = !rigidbody.useGravity;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}
