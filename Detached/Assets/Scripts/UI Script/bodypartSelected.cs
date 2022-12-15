using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class BodypartSelected : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI, gameUI;
    [SerializeField] private GameObject head, leftHand, leftLeg, rightHand, rightLeg;
    [SerializeField] private GameObject headSelection, leftHandSelection, leftLegSelection, rightHandSelection, rightLegSelection;
    private ItemManager iManagerLocalPlayer;
    private bool headActive, leftHandActive, rightHandActive, leftLegActive, rightLegActive;
    void Start()
    {
        iManagerLocalPlayer = NetworkClient.localPlayer.gameObject.GetComponent<ItemManager>();
        iManagerLocalPlayer.changeSelectedLimbEvent.AddListener(GetSelectedOfPlayer);
        iManagerLocalPlayer.dropLimbEvent.AddListener(GetCurrentLimbsOfPlayer);
        iManagerLocalPlayer.pickupLimbEvent.AddListener(GetCurrentLimbsOfPlayer);

        GetCurrentLimbsOfPlayer();
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

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    pauseUI.SetActive(true);
        //    gameUI.SetActive(false);
        //}

        //if(pauseUI.activeSelf)
        //{
        //    gameUI.SetActive(true);
        //}
    }

    private void GetSelectedOfPlayer()
    {
        SetAllInactive();
        if (iManagerLocalPlayer.numberOfLimbs > 0 && !Convert.ToBoolean(iManagerLocalPlayer.SelectionMode))
        {
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Arm && iManagerLocalPlayer.NumberOfArms > 0)
            {
                if (iManagerLocalPlayer.HasBothArms())
                    leftHandActive = true;
                else
                    rightHandActive = true;
            }
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Leg && iManagerLocalPlayer.NumberOfLegs > 0)
            {
                if (iManagerLocalPlayer.HasBothLegs())
                    leftLegActive = true;
                else
                    rightLegActive = true;
            }
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Head && head.activeSelf)
                headActive = true;
        }
        IndicateSelectedLimb();
    }

    private void IndicateSelectedLimb()
    {
        headSelection.SetActive(headActive);
        leftHandSelection.SetActive(leftHandActive);
        leftLegSelection.SetActive(leftLegActive);
        rightHandSelection.SetActive(rightHandActive);
        rightLegSelection.SetActive(rightLegActive);
    }

    private void SetAllInactive()
    {
        headActive = false;
        leftHandActive = false;
        rightHandActive = false;
        leftLegActive = false;
        rightLegActive = false;
    }

    private void GetCurrentLimbsOfPlayer()
    {
        int numberOfArms = iManagerLocalPlayer.NumberOfArms;
        int numberOfLegs = iManagerLocalPlayer.NumberOfLegs;

        head.SetActive(!iManagerLocalPlayer.headDetached);
        rightHand.SetActive(numberOfArms > 0);
        leftHand.SetActive(numberOfArms > 1);
        rightLeg.SetActive(numberOfLegs > 0);
        leftLeg.SetActive(numberOfLegs > 1);
        GetSelectedOfPlayer();
    }
}
