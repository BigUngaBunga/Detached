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
            OneShotVolume.PlayOneShot(AudioPaths.WalkSound, VolumeManager.GetSFXVolume(), transform.position);
        }
        else if (other.tag == "Head")
        {
            OneShotVolume.PlayOneShot(AudioPaths.WalkSound, VolumeManager.GetSFXVolume(), transform.position);
        }
        else if (other.tag == "Arm")
        {
            OneShotVolume.PlayOneShot(AudioPaths.WalkSound, VolumeManager.GetSFXVolume(), transform.position);
        }
        else if (other.tag == "Battery")
        {
            OneShotVolume.PlayOneShot(AudioPaths.WalkSound, VolumeManager.GetSFXVolume(), transform.position);
        }
        else if (other.tag == "Box")
        {
            OneShotVolume.PlayOneShot(AudioPaths.WalkSound, VolumeManager.GetSFXVolume(), transform.position);
        }
    }

}
