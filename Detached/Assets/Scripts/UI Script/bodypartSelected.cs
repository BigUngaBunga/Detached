using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bodypartSelected : MonoBehaviour
{
    public GameObject pauseUI, gameUI;
    public GameObject Head, LeftHand, LeftLeg, RightHand, RightLeg;
    bool headActive, leftHandactive, rightHandactive , leftLegactive, rightLegactive;
    // Start is called before the first frame update
    void Start()
    {
      
    }
    void PauseGame()
    {
        Time.timeScale = 0;
    }
    void ResumeGame()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            // Head 
            headActive = true;

            leftHandactive = false;
            rightHandactive = false;
            rightLegactive = false;
            leftLegactive = false;
        }
        else if (Input.GetKeyDown("2"))
        {
            // Left Hand
            leftHandactive = true;

            headActive = false;
            rightHandactive = false;
            rightLegactive = false;
            leftLegactive = false;
        }
        else if (Input.GetKeyDown("3"))
        {
            // Right Hand
            rightHandactive = true;

            headActive = false;
            leftHandactive = false;
            rightLegactive = false;
            leftLegactive = false;
        }
        else if (Input.GetKeyDown("4"))
        {
            // Left Leg
            leftLegactive = true;

            leftHandactive = false;
            rightHandactive = false;
            headActive = false;
            rightLegactive = false;

        }
        else if (Input.GetKeyDown("5"))
        {
            // Right Leg

            rightLegactive = true;

            headActive = false;
            leftHandactive = false;
            rightHandactive = false;
            leftLegactive = false;
        }

        if (Input.GetKeyDown("6"))
        {
            headActive = false;
            leftHandactive = false;
            rightHandactive = false;
            leftLegactive = false;
            rightLegactive = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
        }
        if (!headActive && !rightLegactive && !rightHandactive && !leftHandactive && !leftLegactive)
        {
            Head.GetComponent<Image>().color = Color.white;
            RightHand.GetComponent<Image>().color = Color.white;
            RightLeg.GetComponent<Image>().color = Color.white;
            LeftHand.GetComponent<Image>().color = Color.white;
            LeftLeg.GetComponent<Image>().color = Color.white;

        }
        else
        {
            if (headActive)
            {
                Head.GetComponent<Image>().color = Color.red;
            }
            else
            {
                Head.GetComponent<Image>().color = Color.white;
            }

            if (rightHandactive)
            {
                RightHand.GetComponent<Image>().color = Color.red;
            }
            else
            {
                RightHand.GetComponent<Image>().color = Color.white;
            }

            if (leftHandactive)
            {
                LeftHand.GetComponent<Image>().color = Color.red;
            }
            else
            {
                LeftHand.GetComponent<Image>().color = Color.white;
            }
            if (rightLegactive)
            {
                RightLeg.GetComponent<Image>().color = Color.red;
            }
            else
            {
                RightLeg.GetComponent<Image>().color = Color.white;
            }
            if (leftLegactive)
            {
                LeftLeg.GetComponent<Image>().color = Color.red;
            }
            else
            {
                LeftLeg.GetComponent<Image>().color = Color.white;
            }
        } 
    }
}
