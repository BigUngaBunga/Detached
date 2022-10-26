using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LeverTrigger : Trigger, IInteractable
{
    [SerializeField] private int requiredArms = 1;

    [SerializeField] private GameObject triggeredLever;
    [SerializeField] private GameObject normalLever;

    protected override void Start()
    {
        base.Start();
        UpdateLeverPosition();
    }

    public void TriggerLever()
    {
        IsTriggered = !IsTriggered;
        UpdateLeverPosition();
    }

    private void UpdateLeverPosition()
    {
        SetRecursiveActivation(!IsTriggered, normalLever);
        SetRecursiveActivation(IsTriggered, triggeredLever);
    }

    [Command(requiresAuthority = false)]
    private void SetRecursiveActivation(bool isActive, GameObject gameObject)
    {
        int children = gameObject.transform.childCount;
        gameObject.SetActive(isActive);
        if (children <= 0)
            return;
        for (int i = 0; i < children; i++)
            SetRecursiveActivation(isActive, gameObject.transform.GetChild(i).gameObject);
    }

    public void Interact(GameObject activatingObject)
    {
        if (HasEnoughArms(activatingObject, requiredArms))
            TriggerLever();

    }

    public bool CanInteract(GameObject activatingObject) => HasEnoughArms(activatingObject, requiredArms);
}
