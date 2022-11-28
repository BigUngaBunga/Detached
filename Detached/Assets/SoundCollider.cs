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
            Sounds.walkSound.start();
        }
        else if (other.tag == "Head")
        {
            Sounds.walkSound.start();
        }
        else if (other.tag == "Arm")
        {
            Sounds.walkSound.start();
        }
        else if (other.tag == "Battery")
        {
            Sounds.walkSound.start();
        }
        else if (other.tag == "Box")
        {
            Sounds.walkSound.start();
        }
    }

}
