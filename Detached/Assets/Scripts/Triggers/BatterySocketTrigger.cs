using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySocketTrigger : Trigger, IInteractable
{
    [Header("Battery Socket Fields")]
    [SerializeField] Transform batteryPosition;
    [SerializeField] GameObject battery;


    protected override void PlaySoundOnTrigger()
    {
        RuntimeManager.PlayOneShot(Sounds.attachSound, transform.position);
    }

    public void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();
        if (IsTriggered && HasEnoughArms(activatingObject, 1))
        {
            itemManager.AttemptPickUpItem(battery);
            IsTriggered = false;
        }
        else if(itemManager.IsCarryingTag("Battery") && itemManager.AttemptDropItemTo(batteryPosition, out _))//Om spelare håller ett batteri
        {
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
            battery.GetComponent<Carryable>().destroyEvent.AddListener(HandleDestroyBattery);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            Debug.Log("Removed battery");
            IsTriggered = false;
            battery.GetComponent<Carryable>().destroyEvent.RemoveListener(HandleDestroyBattery);
        }
    }

    public bool CanInteract(GameObject activatingObject)
    {
        if (activatingObject.CompareTag("Player"))
        {
            var interactableManager = activatingObject.GetComponent<InteractableManager>();
            return interactableManager.IsCarryingTag("Battery") || (!interactableManager.IsCarryingItem && IsTriggered);
        }
        return false;
    }

    private void HandleDestroyBattery() => IsTriggered = false;
}
