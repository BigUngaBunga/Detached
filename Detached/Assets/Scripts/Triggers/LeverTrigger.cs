using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : Trigger
{
    enum LeverType { OneHanded, TwoHanded };

    [SerializeField] private LeverType type;
    [Range(1, 2)]
    [SerializeField] private int requiredArms = 1;

    [SerializeField] private Vector3 activeLeverPosition;
    [SerializeField] private Vector3 activeLeverRotation;
    [SerializeField] private GameObject lever;

    private void Start()
    {
        SetLeverType();
        //UpdateLeverPosition();
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
        if (IsTriggered)
        {
            lever.transform.position = activeLeverPosition;
            lever.transform.eulerAngles = activeLeverRotation;
        }

        else
        {
            lever.transform.position = Vector3.zero;
            lever.transform.rotation = Quaternion.identity;
        }
    }

    private void SetLeverType()
    {
        switch (type)
        {
            case LeverType.OneHanded:
                Debug.Log("Setting activeTransform");
                requiredArms = 1;
                activeLeverPosition = new Vector3(0, 1.6f, 3);
                activeLeverRotation = new Vector3(303, 0, 0);
                break;
            case LeverType.TwoHanded:
                requiredArms = 2;
                activeLeverPosition = new Vector3(0, -0.25f, 0.75f);
                activeLeverRotation = new Vector3(47, 0, 0);
                break;
        }
    }
}
