using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundCollider : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Leg")
        {
            RuntimeManager.PlayOneShot(SFXManager.walkSound, transform.position);
        }
        else if (other.tag == "Head")
        {
            RuntimeManager.PlayOneShot(SFXManager.walkSound, transform.position);
        }
        else if (other.tag == "Arm")
        {
            RuntimeManager.PlayOneShot(SFXManager.walkSound, transform.position);
        }
        else if (other.tag == "Battery")
        {
            RuntimeManager.PlayOneShot(SFXManager.walkSound, transform.position);
        }
        else if (other.tag == "Box")
        {
            RuntimeManager.PlayOneShot(SFXManager.walkSound, transform.position);
        }
    }

}
