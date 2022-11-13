using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detach : MonoBehaviour
{
    public GameObject Head, LeftArm, RightArm, LeftLeg, RightLeg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            // Head 
            if(Head.active == false)
            {
                Head.SetActive(true);
            }
            else if(Head.active == true)
            {
                Head.SetActive(false);
            }
        }
        else if (Input.GetKeyDown("2"))
        {
            // Left Hand
            if (LeftArm.active == false)
            {
                LeftArm.SetActive(true);
            }
            else if (LeftArm.active == true)
            {
                LeftArm.SetActive(false);
            }
        }
        else if (Input.GetKeyDown("3"))
        {
            // Right Hand
            if (RightArm.active == false)
            {
                RightArm.SetActive(true);
            }
            else if (RightArm.active == true)
            {
                RightArm.SetActive(false);
            }
        }
        else if (Input.GetKeyDown("4"))
        {
            // Left Leg
            if (LeftLeg.active == false)
            {
                LeftLeg.SetActive(true);
            }
            else if (LeftLeg.active == true)
            {
                LeftLeg.SetActive(false);
            }
        }
        else if (Input.GetKeyDown("5"))
        {
            // Right Leg
            if (RightLeg.active == false)
            {
                RightLeg.SetActive(true);
            }
            else if (RightLeg.active == true)
            {
                RightLeg.SetActive(false);
            }
        }
    }
}
