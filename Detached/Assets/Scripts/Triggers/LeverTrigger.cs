using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : Trigger, IInteractable
{
    [SerializeField] private int requiredArms = 1;

    [SerializeField] private GameObject triggeredLever;
    [SerializeField] private GameObject normalLever;

    private void Start()
    {
        UpdateLeverPosition();
    }

    public void TriggerLever()
    {
        IsTriggered = !IsTriggered;
        UpdateLeverPosition();
    }

    private void UpdateLeverPosition()
    {
        normalLever.SetActive(!IsTriggered);
        triggeredLever.SetActive(IsTriggered);
    }

    public void Interact(GameObject activatingObject)
    {
        //if (HasEnoughArms(activatingObject, requiredArms))
        //    TriggerLever();
        TriggerLever();
    }
}
