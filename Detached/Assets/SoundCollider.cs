using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundCollider : MonoBehaviour
{

    [Header("Audio")]
    public SFXManager sfx;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Leg")
        {
            RuntimeManager.PlayOneShot(sfx.walkSound, transform.position);
        }
        else if (other.tag == "Head")
        {
            RuntimeManager.PlayOneShot(sfx.walkSound, transform.position);
        }
        else if (other.tag == "Arm")
        {
            RuntimeManager.PlayOneShot(sfx.walkSound, transform.position);
        }
        else if (other.tag == "Battery")
        {
            RuntimeManager.PlayOneShot(sfx.walkSound, transform.position);
        }
        else if (other.tag == "Box")
        {
            RuntimeManager.PlayOneShot(sfx.walkSound, transform.position);
        }
    }

}
