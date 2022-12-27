using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class BodypartSelected : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI, gameUI;
    [SerializeField] private GameObject bodyUI, groundUI;

    #region body objects
    [Header("Body GameObject")]
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject headSelection, leftArmSelection, leftLegSelection, rightArmSelection, rightLegSelection;
    private bool headSelected, leftArmSelected, rightArmSelected, leftLegSelected, rightLegSelected;
    #endregion

    #region ground objects
    [Header("Ground GameObject")]
    [SerializeField] private GameObject headG;
    [SerializeField] private GameObject leftArmG;
    [SerializeField] private GameObject leftLegG;
    [SerializeField] private GameObject rightArmG;
    [SerializeField] private GameObject rightLegG;

    GameObject selectedObject;
    [SerializeField] private GameObject headSelectionG, leftArmSelectionG, leftLegSelectionG, rightArmSelectionG, rightLegSelectionG;
    private bool headActiveG, leftArmActiveG, rightArmActiveG, leftLegActiveG, rightLegActiveG;
    #endregion

    public GameObject cam;
    int numOfLimbOnGround;
    private ItemManager iManagerLocalPlayer;

    public void Setup()
    {
        iManagerLocalPlayer = NetworkClient.localPlayer.gameObject.GetComponent<ItemManager>();
        GetCurrentLimbsOfPlayer();
    }
    public void GetCurrentLimbsOfPlayer(bool headIsDetached, int arms, int legs, float delay = 0)
    {
        if (delay != 0)
            Invoke(nameof(GetCurrentLimbsOfPlayer), delay);
        head.SetActive(!headIsDetached);
        rightArm.SetActive(arms > 0);
        leftArm.SetActive(arms > 1);
        rightLeg.SetActive(legs > 0);
        leftLeg.SetActive(legs > 1);
        GetSelectedOfPlayer();

    }
    public void GetCurrentLimbsOfPlayer()
    {
        int numberOfArms = iManagerLocalPlayer.NumberOfArms;
        int numberOfLegs = iManagerLocalPlayer.NumberOfLegs;

        GetCurrentLimbsOfPlayer(iManagerLocalPlayer.headDetached, numberOfArms, numberOfLegs);
    }
    public void GetCurrentLimbsOfPlayerTest()
    {
        int numberOfArms = iManagerLocalPlayer.NumberOfArms;
        int numberOfLegs = iManagerLocalPlayer.NumberOfLegs;

        head.SetActive(!iManagerLocalPlayer.headDetached);
        rightArm.SetActive(numberOfArms > 0);
        leftArm.SetActive(numberOfArms > 1);
        rightLeg.SetActive(numberOfLegs > 0);
        leftLeg.SetActive(numberOfLegs > 1);
        GetSelectedOfPlayer();

    }
    public void GetSelectedOfPlayer()
    {
        SetBodyAllInactive();
        if (!Convert.ToBoolean(iManagerLocalPlayer.SelectionMode))
        {
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Arm)
            {
                if (leftArm.activeSelf)
                    leftArmSelected = true;
                else if(rightArm.activeSelf)
                    rightArmSelected = true;
            }
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Leg)
            {
                if (leftLeg.activeSelf)
                    leftLegSelected = true;
                else if(rightLeg.activeSelf)
                    rightLegSelected = true;
            }
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Head)
                headSelected = head.activeSelf;
        }
        IndicateSelectedLimb();
    }
    
    private void Update()
    {
        groundUI.SetActive(iManagerLocalPlayer.groundMode);
        bodyUI.SetActive(!iManagerLocalPlayer.groundMode);
        GetCurrentLimbsOfPlayer();

        if (iManagerLocalPlayer.groundMode)
        {
            GetLimbsOnGround();
            GetSelectedOnGround();
        }
        else
        {
            GetSelectedOfPlayer();
        }

        
    }

    private void GetLimbsOnGround()
    {
        headG.SetActive(!head.activeSelf);
        leftArmG.SetActive(!leftArm.activeSelf);
        rightArmG.SetActive(!rightArm.activeSelf);
        leftLegG.SetActive(!leftLeg.activeSelf);
        rightLegG.SetActive(!rightLeg.activeSelf);
    }

    private void IndicateSelectedLimb()
    {
        headSelection.SetActive(headSelected);
        rightArmSelection.SetActive(rightArmSelected);
        rightLegSelection.SetActive(rightLegSelected);
        leftArmSelection.SetActive(leftArmSelected);
        leftLegSelection.SetActive(leftLegSelected);
    }

    private void IndicateSelectedLimbOnGround()
    {
        if (iManagerLocalPlayer.limbs.Count == 0)
            return;

        if (iManagerLocalPlayer.limbs[iManagerLocalPlayer.indexControll] == null)
        {
            return;
        }
        int boundCheck = iManagerLocalPlayer.limbs[iManagerLocalPlayer.indexControll].transform.childCount - 1;
        for (int i = 0; i < boundCheck; i++)
        {
            selectedObject = iManagerLocalPlayer.limbs[iManagerLocalPlayer.indexControll].transform.GetChild(i).gameObject;
            if (iManagerLocalPlayer.limbs[iManagerLocalPlayer.indexControll].transform.GetChild(i + 1) != null)
                cam = iManagerLocalPlayer.limbs[iManagerLocalPlayer.indexControll].transform.GetChild(i + 1).gameObject;
        }

        if (cam == null || selectedObject == null) return;
        
        headSelectionG.SetActive(selectedObject.name == "Head");
        leftLegSelectionG.SetActive(selectedObject.name == "LeftLeg");
        rightLegSelectionG.SetActive(selectedObject.name == "RightLeg");
        leftArmSelectionG.SetActive(selectedObject.name == "LeftArm");
        rightArmSelectionG.SetActive(selectedObject.name == "RigthArm");
    }

    private void GetSelectedOnGround()
    {
        SetGroundAllInactive();
        IndicateSelectedLimbOnGround();
    }

    private void SetBodyAllInactive()
    {
        headSelected = false;
        leftArmSelected = false;
        rightArmSelected = false;
        leftLegSelected = false;
        rightLegSelected = false;
    }

    private void SetGroundAllInactive()
    {
        headSelectionG.SetActive(false);
        leftArmSelectionG.SetActive(false);
        rightArmSelectionG.SetActive(false);
        leftLegSelectionG.SetActive(false);
        rightLegSelectionG.SetActive(false);
    }

}
