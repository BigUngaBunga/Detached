using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySocketTrigger : Trigger, IInteractable
{
    [Header("Battery Socket Fields")]
    [SerializeField] Transform batteryPosition;
    [SerializeField] GameObject battery;

    private void MoveToBatteryPosition(GameObject battery)
    {
        battery.transform.position = batteryPosition.position;
        battery.transform.rotation = batteryPosition.rotation;
    }

    public void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();
        if (IsTriggered && HasEnoughArms(activatingObject, 1))
        {
            itemManager.AttemptPickUpItem(battery);
            IsTriggered = false;
        }
        else if(itemManager.IsCarryingTag("Battery") && itemManager.AttemptDropItem(out GameObject battery))//Om spelare h�ller ett batteri
        {
            MoveToBatteryPosition(battery);
            IsTriggered = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            Debug.Log("Attached battery");
            battery = other.gameObject;
            IsTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            Debug.Log("Removed battery");
            IsTriggered = false;
        }
    }

    public bool CanInteract(GameObject activatingObject)
    {
        if (activatingObject.CompareTag("Player"))
            return activatingObject.GetComponent<InteractableManager>().IsCarryingTag("Battery");
        return false;
    }
}
