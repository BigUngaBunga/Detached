using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InteractableManager : NetworkBehaviour
{
    [SyncVar][SerializeField] private bool isCarryingItem;
    [SerializeField] private Transform holdingPosition;
    [SyncVar][SerializeField] private Carryable carriedItem;
    private ItemManager itemManager;
    public bool IsCarryingItem { get { return isCarryingItem; } }

    private void Start()
    {
        itemManager = GetComponent<ItemManager>();
        itemManager.dropLimbEvent.AddListener(DropIfCantCarry);
    }

    public bool IsCarryingTag(string tag) => isCarryingItem && carriedItem.CompareTag(tag);

    public bool CanPickUpItem(GameObject item) => !isCarryingItem && item.GetComponent<Carryable>().RequiredArms <= itemManager.NumberOfArms;

    [Command(requiresAuthority = false)]
    public void AttemptPickUpItem(GameObject item)
    {
        if (CanPickUpItem(item))
        {
            isCarryingItem = true;
            carriedItem = item.GetComponent<Carryable>();
            carriedItem.destroyEvent.AddListener(DropItem);
            carriedItem.PickUp(holdingPosition);
        }
    }

    public bool AttemptDropItem(out GameObject item)
    {
        if (isCarryingItem)
        {
            item = carriedItem.gameObject;
            carriedItem.destroyEvent.RemoveListener(DropItem);
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
        if (isCarryingItem && carriedItem.GetComponent<Carryable>().RequiredArms > itemManager.NumberOfArms)
            AttemptDropItem();
    }
}
