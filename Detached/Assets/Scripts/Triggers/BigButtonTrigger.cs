using FMODUnity;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class BigButtonTrigger : Trigger, IInteractable
{
    [Header("Box interaction")]
    [SerializeField] private Transform boxPosition;
    [SerializeField] private GameObject box;
    [SerializeField] private GameObject button;
    [SerializeField] private float pressedButtonHeightDifference;
    private Vector3 initialButtonPosition;
    private Carryable boxInteractable;
    [SerializeField] private List<GameObject> objectsOnButton = new List<GameObject>();
    private bool HasBox => box != null;

    private int TriggeringObjects { 
        get => triggeringObjects; 
        set {
            triggeringObjects = value;
            if (triggeringObjects >= 1)
            {
                IsTriggered = true;
                MoveButton(true);
            }
            else if (triggeringObjects <= 0)
            {
                IsTriggered = false;
                MoveButton(false);
            }
            if (triggeringObjects < 0)
                triggeringObjects = 0;
        } 
    }
    [SerializeField] private int triggeringObjects;

    protected override void Start()
    {
        base.Start();
        initialButtonPosition = button.transform.localPosition;
    }

    private bool IsCollisionObject(string tag) => tag.Equals("Box") || tag.Equals("Leg") || tag.Equals("Player") || tag.Equals("Torso");

    public void OnTriggerEnter(Collider other)
    {
        if (IsCollisionObject(other.gameObject.tag))
        {
            ++TriggeringObjects;
            objectsOnButton.Add(other.gameObject);
        }
        if (other.gameObject.CompareTag("Box"))
        {
            box = other.gameObject;
            boxInteractable = box.GetComponent<Carryable>();
            boxInteractable.destroyEvent.AddListener(HandleDestroyBox);
        }
        else if (other.gameObject.CompareTag("Leg"))
        {
            if (other.transform.parent.gameObject.TryGetComponent(out SceneObjectItemManager item))
                item.pickUpLimbEvent.AddListener(RemoveTrigger);
        }
        CheckObjectsOnButton();
    }

    protected override void PlaySoundOnTrigger()
    {
        OneShotVolume.PlayOneShot(AudioPaths.PushButtonSound, VolumeManager.GetSFXVolume(), transform.position);
    }

    protected override void PlaySoundOnStopTrigger()
    {
        OneShotVolume.PlayOneShot(AudioPaths.PushButtonSound, VolumeManager.GetSFXVolume(), transform.position);
    }

    public void OnTriggerExit(Collider other)
    {
        if (IsCollisionObject(other.gameObject.tag))
        {
            --TriggeringObjects;
            objectsOnButton.Remove(other.gameObject);
        }
        if (other.gameObject.Equals(box))
        {
            boxInteractable.destroyEvent.RemoveListener(HandleDestroyBox);
            box = null;
            boxInteractable = null;
        }
        else if (other.gameObject.CompareTag("Leg") && other.transform.parent.gameObject.TryGetComponent(out SceneObjectItemManager item))
                item.pickUpLimbEvent.RemoveListener(RemoveTrigger);

        CheckObjectsOnButton();
    }

    public void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();

        if (HasBox && IsTriggered && itemManager.AttemptPickUpItem(box)) { }
        else if (itemManager.IsCarryingTag("Box"))
            itemManager.AttemptDropItemTo(boxPosition, out _);
    }

    public bool CanInteract(GameObject activatingObject)
    {
        if (activatingObject.CompareTag("Player"))
        {
            bool canPlace = activatingObject.GetComponent<InteractableManager>().IsCarryingTag("Box");
            bool canPickUp = HasBox && boxInteractable.CanInteract(activatingObject);
            return canPlace || canPickUp;
        }
        return false;
    }

    private void RemoveTrigger()
    {
        TriggeringObjects--;
    }

    private void CheckObjectsOnButton()
    {
        for (int i = objectsOnButton.Count - 1; i >= 0; i--)
        {
            if (objectsOnButton[i] == null || !objectsOnButton[i].activeSelf)
            {
                RemoveTrigger();
                objectsOnButton.RemoveAt(i);
            }
        }
    }

    private void HandleDestroyBox()
    {
        Invoke(nameof(CheckObjectsOnButton), 0.1f);
    }

    private void MoveButton(bool itTriggered)
    {
        button.transform.localPosition = itTriggered ? initialButtonPosition + new Vector3(0, pressedButtonHeightDifference) : initialButtonPosition;
    }
}
