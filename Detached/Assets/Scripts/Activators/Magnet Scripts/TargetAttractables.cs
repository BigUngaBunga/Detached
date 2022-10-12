using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAttractables : MonoBehaviour
{
    private MagnetActivator magnet;

    void Start()
    {
        magnet = GetComponentInParent<MagnetActivator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasAMagneticTag(other.gameObject) && other.gameObject.TryGetComponent(out Rigidbody rigidbody))
        {
            Debug.Log("Encountered magnetic object");
            magnet.AddMagnetizedObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (HasAMagneticTag(other.gameObject))
        {
            Debug.Log("Lost magnetic object");
            magnet.RemoveMagnetizedObject(other.gameObject);
        }
    }

    private bool HasAMagneticTag(GameObject gameObject)
    {
        string[] tags = { "Leg", "Torso", "Player", "Battery", "Key", "Magnetic"};
        foreach (var tag in tags)
            if (gameObject.CompareTag(tag))
                return true;
        return false;
    }

}
