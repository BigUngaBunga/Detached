using UnityEngine;

public class BigButtonTrigger : Trigger, IInteractable
{
    [Header("Box interaction")]
    [SerializeField] private Transform boxPosition;
    [SerializeField] private GameObject box;
    private bool HasBox => box != null;

    private int TriggeringObjects { 
        get => triggeringObjects; 
        set {
            triggeringObjects = value;
            if (triggeringObjects == 1)
                IsTriggered = true;
            else if (triggeringObjects == 0)
                IsTriggered = false;
        } 
    }
    [SerializeField] private int triggeringObjects;

    private bool IsCollisionObject(string tag) => tag.Equals("Box") || tag.Equals("Leg") || tag.Equals("Player");

    public void OnTriggerEnter(Collider other)
    {
        if (IsCollisionObject(other.gameObject.tag))
            ++TriggeringObjects;
        if (other.gameObject.CompareTag("Box"))
            box = other.gameObject;
    }

    public void OnTriggerExit(Collider other)
    {
        if (IsCollisionObject(other.gameObject.tag))
            --TriggeringObjects;
        if (other.gameObject.Equals(box))
            box = null;
    }

    public void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();
        if (HasBox && IsTriggered && HasEnoughArms(activatingObject, 1))
        {
            itemManager.AttemptPickUpItem(box);
        }
        else if (itemManager.IsCarryingTag("Box") && itemManager.AttemptDropItem(out GameObject box))
        {
            MoveToBoxPosition(box);
        }
    }

    private void MoveToBoxPosition(GameObject box)
    {
        box.transform.position = boxPosition.position;
        box.transform.rotation = boxPosition.rotation;
    }

    public bool CanInteract(GameObject activatingObject)
    {
        if (activatingObject.CompareTag("Player"))
        {
            bool canPlace = activatingObject.GetComponent<InteractableManager>().IsCarryingTag("Box");
            bool canPickUp = HasBox && activatingObject.GetComponent<InteractableManager>().CanPickUpItem(box);
            return canPlace || canPickUp;
        }
        return false;
    }
}
