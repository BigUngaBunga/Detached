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
        battery.transform.position = batteryPosition.transform.position;
        battery.transform.rotation = batteryPosition.transform.rotation;
    }

    public void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();
        if (IsTriggered && HasEnoughArms(activatingObject, 1))
        {
            itemManager.AttemptPickUpItem(battery);
            IsTriggered = false;
        }
        else if(itemManager.TryCompareTag("Battery") && itemManager.AttemptDropItem(out GameObject battery))//Om spelare håller ett batteri
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
}
