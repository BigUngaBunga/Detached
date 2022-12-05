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
            SFXManager.PlayOneShot(SFXManager.WalkSound, SFXManager.SFXVolume, transform.position);
        }
        else if (other.tag == "Head")
        {
            SFXManager.PlayOneShot(SFXManager.WalkSound, SFXManager.SFXVolume, transform.position);
        }
        else if (other.tag == "Arm")
        {
            SFXManager.PlayOneShot(SFXManager.WalkSound, SFXManager.SFXVolume, transform.position);
        }
        else if (other.tag == "Battery")
        {
            SFXManager.PlayOneShot(SFXManager.WalkSound, SFXManager.SFXVolume, transform.position);
        }
        else if (other.tag == "Box")
        {
            SFXManager.PlayOneShot(SFXManager.WalkSound, SFXManager.SFXVolume, transform.position);
        }
    }

}
