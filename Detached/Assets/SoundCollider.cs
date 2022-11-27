using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollider : MonoBehaviour
{


    [Header("Audio")]
    [SerializeField] private AudioSource metalSound;
    [SerializeField] private AudioSource softSound;
    // Start is called before the first frame update
    
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Leg")
        {
            metalSound.Play();
        }
        else if (other.tag == "Head")
        {
            metalSound.Play();
        }
        else if (other.tag == "Arm")
        {
            softSound.Play();
        }
        else if (other.tag == "Battery")
        {
            metalSound.Play();
        }
        else if (other.tag == "Box")
        {
            softSound.Play();
        }
    }

}
