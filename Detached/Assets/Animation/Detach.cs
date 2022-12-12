using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detach : MonoBehaviour
{
    public GameObject Head, LeftArm, RightArm, LeftLeg, RightLeg;

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            // Head 
            Head.SetActive(!Head.activeSelf);
        }
        else if (Input.GetKeyDown("2"))
        {
            // Left Hand
            LeftArm.SetActive(!LeftArm.activeSelf);
        }
        else if (Input.GetKeyDown("3"))
        {
            // Right Hand
            RightArm.SetActive(!RightArm.activeSelf);
        }
        else if (Input.GetKeyDown("4"))
        {
            // Left Leg
            LeftLeg.SetActive(!LeftLeg.activeSelf);
        }
        else if (Input.GetKeyDown("5"))
        {
            // Right Leg
            RightLeg.SetActive(!RightLeg.activeSelf);
        }
    }
}
