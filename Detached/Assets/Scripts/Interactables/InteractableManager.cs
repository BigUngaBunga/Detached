using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InteractableManager : NetworkBehaviour
{
    [SerializeField] private bool isCarryingItem;
    [SerializeField] private Transform holdingPosition;
    [SerializeField] private PickUpInteractable carriedItem;
    private ItemManager itemManager;
    public bool IsCarryingItem { get { return isCarryingItem; } }

    private void Start()
    {
        itemManager = GetComponent<ItemManager>();
        itemManager.dropLimbEvent.AddListener(CanStillCarryItem);
    }

    public bool IsCarryingTag(string tag) => isCarryingItem && carriedItem.CompareTag(tag);

    public bool CanPickUpItem(GameObject item) => !isCarryingItem && item.GetComponent<PickUpInteractable>().RequiredArms <= itemManager.NumberOfArms;

    public void AttemptPickUpItem(GameObject item)
    {
        Debug.Log("Picking up item");
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
        item = null;
        if (isCarryingItem)
        {
            isCarryingItem = false;
            carriedItem.Drop();
            item = carriedItem.gameObject;
            ToggleGravity(item);
            carriedItem = null;
            return true;
        }
        return false;
    }

    public bool AttemptDropItem()
    {
        if (isCarryingItem)
        {
            isCarryingItem = false;
            carriedItem.Drop();
            ToggleGravity(carriedItem.gameObject);
            carriedItem = null;
            return true;
        }
        return false;
    }

    private void CanStillCarryItem()
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
