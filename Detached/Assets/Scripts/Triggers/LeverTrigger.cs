using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : Trigger
{
    [SerializeField] private int requiredArms = 1;

    [SerializeField] private GameObject triggeredLever;
    [SerializeField] private GameObject normalLever;

    private void Start()
    {
        UpdateLeverPosition();
    }

    //TODO implementera faktisk aktiveringsfunktionalitet
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            TriggerLever();
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
}
