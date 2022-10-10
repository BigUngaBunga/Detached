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
    }

    public bool TryCompareTag(string tag)
    {
        if (isCarryingItem)
            return carriedItem.CompareTag(tag);
        return false;
    }

    public void AttemptPickUpItem(GameObject item)
    {
        Debug.Log("Picking up item");
        var interactable = item.GetComponent<PickUpInteractable>();
        if (!isCarryingItem && interactable.RequiredArms <= itemManager.NumberOfArms())
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
    }
}
