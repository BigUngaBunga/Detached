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
        if (HasAMagneticTag(other.gameObject))
        {
            magnet.AddMagnetizedObject(GetObjectWithRigidbody(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (HasAMagneticTag(other.gameObject))
        {
            magnet.RemoveMagnetizedObject(GetObjectWithRigidbody(other.gameObject));
        }
    }

    private bool HasAMagneticTag(GameObject gameObject)
    {
        string[] tags = { "Leg", "Arm", "Head", "Torso", "Player", "Battery", "Key", "Magnetic", "Box" };
        foreach (var tag in tags)
            if (gameObject.CompareTag(tag))
                return true;
        return false;
    }

    private GameObject GetObjectWithRigidbody(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Rigidbody _))
            return gameObject;
        return gameObject.GetComponentInParent<Rigidbody>().gameObject;
    }
}
