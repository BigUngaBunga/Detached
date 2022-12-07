using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class bodypartSelected : MonoBehaviour
{
    public GameObject pauseUI, gameUI;
    public GameObject Head, LeftHand, leftLeg, RightHand, rightLeg;
    private ItemManager iManagerLocalPlayer;
    bool headActive, leftHandactive, rightHandactive, leftLegactive, rightLegactive;
    // Start is called before the first frame update
    void Start()
    {
        iManagerLocalPlayer = NetworkClient.localPlayer.gameObject.GetComponent<ItemManager>();
        iManagerLocalPlayer.changeSelectedLimbEvent.AddListener(GetSelectedOfPlayer);
        iManagerLocalPlayer.dropLimbEvent.AddListener(GetCurrentLimbsOfPlayer);
        iManagerLocalPlayer.pickupLimbEvent.AddListener(GetCurrentLimbsOfPlayer);


        GetSelectedOfPlayer();
    }
    void PauseGame()
    {
        Time.timeScale = 0;
    }
    void ResumeGame()
    {
        Time.timeScale = 1;
    }
    void activate()
    {
        gameUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
        }
        if (!headActive && !rightLegactive && !rightHandactive && !leftHandactive && !leftLegactive)
        {
            Head.GetComponent<Image>().color = Color.white;
            RightHand.GetComponent<Image>().color = Color.white;
            rightLeg.GetComponent<Image>().color = Color.white;
            LeftHand.GetComponent<Image>().color = Color.white;
            leftLeg.GetComponent<Image>().color = Color.white;

        }
        else
        {
            if (headActive)
            {
                Head.GetComponent<Image>().color = Color.white;
            }
            else
            {
                Head.GetComponent<Image>().color = Color.white;
            }

            if (rightHandactive)
            {
                RightHand.GetComponent<Image>().color = Color.white;
            }
            else
            {
                RightHand.GetComponent<Image>().color = Color.white;
            }

            if (leftHandactive)
            {
                LeftHand.GetComponent<Image>().color = Color.white;
            }
            else
            {
                LeftHand.GetComponent<Image>().color = Color.white;
            }
            if (rightLegactive)
            {
                rightLeg.GetComponent<Image>().color = Color.white;
            }
            else
            {
                rightLeg.GetComponent<Image>().color = Color.white;
            }
            if (leftLegactive)
            {
                leftLeg.GetComponent<Image>().color = Color.white;
            }
            else
            {
                leftLeg.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void GetSelectedOfPlayer()
    {
        SetAllInactive();
        if (!Convert.ToBoolean(iManagerLocalPlayer.SelectionMode))
        {
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Arm)
            {
                if (iManagerLocalPlayer.HasBothArms())
                    leftHandactive = true;
                else
                    rightHandactive = true;
            }
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Leg)
            {
                if (iManagerLocalPlayer.HasBothLegs())
                    leftLegactive = true;
                else
                    rightLegactive = true;
            }
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Head)
                headActive = true;
        }
    }

    private void SetAllInactive()
    {
        headActive = false;
        leftHandactive = false;
        rightHandactive = false;
        leftLegactive = false;
        rightLegactive = false;
    }

    private void GetCurrentLimbsOfPlayer()
    {
        int numberOfArms = iManagerLocalPlayer.NumberOfArms;
        int numberOfLegs = iManagerLocalPlayer.NumberOfLegs;

        if (numberOfArms == 2)
        {
            RightHand.SetActive(true);
            LeftHand.SetActive(true);
        }
        else if (numberOfArms == 1)
        {
            RightHand.SetActive(true);
            LeftHand.SetActive(false);
        }
        else if (numberOfArms == 0)
        {
            RightHand.SetActive(false);
            LeftHand.SetActive(false);
        }


        if (numberOfLegs == 2)
        {
            rightLeg.SetActive(true);
            leftLeg.SetActive(true);
        }
        else if (numberOfLegs == 1)
        {
            rightLeg.SetActive(true);
            leftLeg.SetActive(false);
        }
        else if (numberOfLegs == 0)
        {
            rightLeg.SetActive(false);
            leftLeg.SetActive(false);
        }
        GetSelectedOfPlayer();
    }
}
