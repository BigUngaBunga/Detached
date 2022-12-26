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
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject headSelection, leftHandSelection, leftLegSelection, rightHandSelection, rightLegSelection;
    private bool headSelected, leftHandSelected, rightHandSelected, leftLegSelected, rightLegSelected;
    #endregion

    #region ground objects
    [Header("Ground GameObject")]
    [SerializeField] private GameObject headG;
    [SerializeField] private GameObject leftHandG;
    [SerializeField] private GameObject leftLegG;
    [SerializeField] private GameObject rightHandG;
    [SerializeField] private GameObject rightLegG;

    GameObject selectedObject;
    [SerializeField] private GameObject headSelectionG, leftHandSelectionG, leftLegSelectionG, rightHandSelectionG, rightLegSelectionG;
    private bool headActiveG, leftHandActiveG, rightHandActiveG, leftLegActiveG, rightLegActiveG;
    #endregion

    public GameObject cam;
    int numOfLimbOnGround;
    private bool performedSetup = false;
    private ItemManager iManagerLocalPlayer;
    void Start()
    {

    }

    public void Setup()
    {
        iManagerLocalPlayer = NetworkClient.localPlayer.gameObject.GetComponent<ItemManager>();
        Debug.Log("Game object name: " + iManagerLocalPlayer.gameObject.name);
        /*      iManagerLocalPlayer.changeSelectedLimbEvent.AddListener(GetSelectedOfPlayer);
              iManagerLocalPlayer.dropLimbEvent.AddListener(GetCurrentLimbsOfPlayer);
              iManagerLocalPlayer.pickupLimbEvent.AddListener(GetCurrentLimbsOfPlayer);*/

        GetCurrentLimbsOfPlayer();
        performedSetup = true;
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

/*        if (!performedSetup)
            Setup();*/
        if (!iManagerLocalPlayer.groundMode)
        {
            groundUI.SetActive(false);
            bodyUI.SetActive(true);
        }
        else
        {
            bodyUI.SetActive(false);
            groundUI.SetActive(true);
        }

        GetLimbsOnGround();
        GetSelectedOnGround();
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

    public void GetSelectedOfPlayer()
    {
        SetBodyAllInactive();

        if (iManagerLocalPlayer.numberOfLimbs > 0 && !Convert.ToBoolean(iManagerLocalPlayer.SelectionMode))
        {
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Arm && iManagerLocalPlayer.NumberOfArms > 0)
            {
                if (iManagerLocalPlayer.HasBothArms())
                    leftHandSelected = true;
                else
                    rightHandSelected = true;
            }
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Leg && iManagerLocalPlayer.NumberOfLegs > 0)
            {
                if (iManagerLocalPlayer.HasBothLegs())
                    leftLegSelected = true;
                else
                    rightLegSelected = true;
            }
            if (iManagerLocalPlayer.SelectedLimbToThrow == ItemManager.Limb_enum.Head && head.activeSelf)
                headSelected = true;

        }
        IndicateSelectedLimb();
    }

    private void GetLimbsOnGround()
    {

        headG.SetActive(!head.activeSelf);

        leftHandG.SetActive(!leftHand.activeSelf);

        rightHandG.SetActive(!rightHand.activeSelf);

        leftLegG.SetActive(!leftLeg.activeSelf);

        rightLegG.SetActive(!rightLeg.activeSelf);
    }

    private void IndicateSelectedLimb()
    {
        headSelection.SetActive(headSelected);
        rightHandSelection.SetActive(rightHandSelected);
        rightLegSelection.SetActive(rightLegSelected);
        leftHandSelection.SetActive(leftHandSelected);
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

        if (cam != null && selectedObject.tag == "Head")
            headSelectionG.SetActive(true);
        else
            headSelectionG.SetActive(false);

        if (cam != null && selectedObject.name == "LeftLeg")
            leftLegSelectionG.SetActive(true);
        else
            leftLegSelectionG.SetActive(false);

        if (cam != null && selectedObject.name == "RightLeg")
            rightLegSelectionG.SetActive(true);
        else
            rightLegSelectionG.SetActive(false);

        if (cam != null && selectedObject.name == "LeftArm")
            leftHandSelectionG.SetActive(true);
        else
            leftHandSelectionG.SetActive(false);

        if (cam != null && selectedObject.name == "RightArm")
            rightHandSelectionG.SetActive(true);
        else
            rightHandSelectionG.SetActive(false);
        /*      
                if (iManagerLocalPlayer.limbs[iManagerLocalPlayer.indexControll].transform.GetChild(1).gameObject != null)
                    leftLegSelectionG.SetActive(true);
                else
                    leftLegSelectionG.SetActive(false);*/
        // test = iManagerLocalPlayer.limbs[iManagerLocalPlayer.indexControll].transform.GetChild(1).gameObject;
        //if ()


        /*rightHandSelectionG.SetActive(rightHandG.activeSelf);
        rightLegSelectionG.SetActive(rightLegG.activeSelf);
        leftHandSelectionG.SetActive(leftHandG.activeSelf);
        leftLegSelectionG.SetActive(leftLegG.activeSelf);*/
    }

    private void GetSelectedOnGround()
    {
        SetGroundAllInactive();

        IndicateSelectedLimbOnGround();
    }

    private void SetBodyAllInactive()
    {
        headSelected = false;
        leftHandSelected = false;
        rightHandSelected = false;
        leftLegSelected = false;
        rightLegSelected = false;
    }

    private void SetGroundAllInactive()
    {
        /*   for (int i = 0; i < groundSelection.Length; i++)
           {
               groundSelection[i].SetActive(false);
           }*/
        headSelectionG.SetActive(false);
        leftHandSelectionG.SetActive(false);
        rightHandSelectionG.SetActive(false);
        leftLegSelectionG.SetActive(false);
        rightLegSelectionG.SetActive(false);
    }

    public void GetCurrentLimbsOfPlayer()
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
