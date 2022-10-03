using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySocketTrigger : Trigger
{
    [Header("Battery Socket Fields")]
    [SerializeField] Transform batteryPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            Debug.Log("Attached battery");
            IsTriggered = true;
            other.transform.position = batteryPosition.transform.position;
            other.transform.rotation = batteryPosition.transform.rotation;
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
