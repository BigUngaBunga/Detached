using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : NetworkBehaviour
{
    [SerializeField] private bool isCarryingItem;
    [SerializeField] private GameObject carriedItem;
    [SerializeField] private Transform holdingParent;
    [SerializeField] private Transform previousParent;
    private ItemManager itemManager;
    public bool IsCarryingItem { get { return isCarryingItem; } }

    private void Start()
    {
        itemManager = GetComponent<ItemManager>();
        itemManager.dropLimbEvent.AddListener(CanStillCarryItem);
    }

    public bool IsCarryingTag(string tag)
    {
        if (isCarryingItem)
            return carriedItem.CompareTag(tag);
        return false;
    }

    public bool CanPickUpItem(GameObject item) => !isCarryingItem && item.GetComponent<PickUpInteractable>().RequiredArms <= itemManager.NumberOfArms;

    public void AttemptPickUpItem(GameObject item)
    {
        Debug.Log("Picking up item");
        if (CanPickUpItem(item))
        {
            isCarryingItem = true;
            carriedItem = item;
            previousParent = item.transform.parent;
            MoveTo(carriedItem, holdingParent);
            ToggleGravity(carriedItem);
            carriedItem.GetComponent<PickUpInteractable>().PickUp(holdingParent);
        }
    }

    public bool AttemptDropItem(out GameObject item)
    {
        item = null;
        if (isCarryingItem)
        {
            isCarryingItem = false;
            MoveTo(carriedItem, previousParent, false);
            ToggleGravity(carriedItem);
            carriedItem.GetComponent<PickUpInteractable>().Drop();
            item = carriedItem;
            return true;
        }
        return false;
    }

    public void AttemptDropItem()
    {
        if (isCarryingItem)
        {
            isCarryingItem = false;
            MoveTo(carriedItem, previousParent, false);
            ToggleGravity(carriedItem);
            carriedItem.GetComponent<PickUpInteractable>().Drop();
        }
    }

    private void CanStillCarryItem()
    {
        if (isCarryingItem && carriedItem.GetComponent<PickUpInteractable>().RequiredArms > itemManager.NumberOfArms)
            AttemptDropItem();
    }

    private void MoveTo(GameObject item, Transform target, bool moveItem = true)
    {
        if (target == null)
            return;
        item.transform.parent = target;
        if (moveItem)
            item.transform.position = target.position;

    }

    private void ToggleGravity(GameObject item)
    {
        item.TryGetComponent(out Rigidbody rigidbody);
        rigidbody.useGravity = !rigidbody.useGravity;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.angularDrag = rigidbody.useGravity ? 0.05f : 0.5f;
    }
}
