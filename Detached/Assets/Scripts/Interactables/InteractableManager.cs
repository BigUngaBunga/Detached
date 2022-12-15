using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress; // this was causing build errors.

public class InteractableManager : NetworkBehaviour
{
    [SyncVar][SerializeField] private bool isCarryingItem;
    [SerializeField] private Transform holdingPosition;
    [SyncVar][SerializeField] private Carryable carriedItem;
    private ItemManager itemManager;
    public bool IsCarryingItem { get { return isCarryingItem && carriedItem != null; } }

    private void Start()
    {
        itemManager = GetComponent<ItemManager>();
        itemManager.dropLimbEvent.AddListener(DropIfCantCarry);
    }

    public bool IsCarryingTag(string tag) => IsCarryingItem && carriedItem.CompareTag(tag);

    public bool CanPickUpItem(GameObject item) => item != null && !isCarryingItem && item.GetComponent<Carryable>().RequiredArms <= itemManager.NumberOfArms;

    public bool AttemptPickUpItem(GameObject item)
    {
        if (CanPickUpItem(item))
        {
            PickUpItem(item);
            return true;
        }
        return false;
    }

    [Command(requiresAuthority = false)]
    private void PickUpItem(GameObject item)
    {
        isCarryingItem = true;
        carriedItem = item.GetComponent<Carryable>();
        carriedItem.destroyEvent.AddListener(DropItem);
        carriedItem.PickUp(holdingPosition);
    }

    public bool AttemptDropItem(out GameObject item)
    {
        if (IsCarryingItem)
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
        if (IsCarryingItem)
        {
            DropItem();
            return true;
        }
        return false;
    }

    public bool AttemptDropItemTo(Transform targetPosition, out GameObject item)
    {
        DropItemTo(targetPosition.position, targetPosition.rotation);
        return AttemptDropItem(out item);
    }


    [Command(requiresAuthority = false)]
    private void DropItem() => RPCDropItem();

    [ClientRpc]
    private void RPCDropItem() 
    {
        if(carriedItem == null)
            return;
        carriedItem.destroyEvent.RemoveListener(DropItem);
        isCarryingItem = false;
        if(carriedItem != null)
            carriedItem.Drop();

    }

    [Command(requiresAuthority = false)]
    private void DropItemTo(Vector3 position, Quaternion rotation) => RPCDropItemTo(position, rotation);
    
    [ClientRpc]
    private void RPCDropItemTo(Vector3 position, Quaternion rotation)
    {
        isCarryingItem = false;
        if (carriedItem != null)
            carriedItem.DropTo(position, rotation);
    }

    private void DropIfCantCarry()
    {
        if (IsCarryingItem && carriedItem.RequiredArms > itemManager.NumberOfArms)
            AttemptDropItem();
    }
}
