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
        SFXManager.PlayOneShot(SFXManager.AttachSound, VolumeManager.GetSFXVolume(), transform.position);
    }

    protected override void PlaySoundOnStopTrigger()
    {
        SFXManager.PlayOneShot(SFXManager.DetachSound, VolumeManager.GetSFXVolume(), transform.position);
    }

    public void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();
        if (IsTriggered && itemManager.AttemptPickUpItem(battery))
            IsTriggered = false;
        else if(itemManager.IsCarryingTag("Battery") && itemManager.AttemptDropItemTo(batteryPosition, out _))
            IsTriggered = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            Debug.Log("Attached battery");
            battery = other.gameObject;
            IsTriggered = true;
            battery.GetComponent<Carryable>().destroyEvent.AddListener(HandleDestroyBattery);
            PlaySoundOnTrigger();
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
            bool canPlaceBattery = !IsTriggered && interactableManager.IsCarryingTag("Battery");
            bool canPickUpBattery = IsTriggered && interactableManager.CanPickUpItem(battery);
            return canPlaceBattery || canPickUpBattery;
        }
        return false;
    }

    private void HandleDestroyBattery() => IsTriggered = false;
}
