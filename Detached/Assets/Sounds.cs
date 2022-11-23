using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{

    [Header("Audio")]
    [SerializeField] private AudioSource walkSound;
    // Start is called before the first frame update
   public void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Leg")
        {
            walkSound.Play();
        }
    }
}
