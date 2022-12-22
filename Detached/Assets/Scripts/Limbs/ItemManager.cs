using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Events;
using Cinemachine;
using FMODUnity;

public class ItemManager : NetworkBehaviour
{
    [Header("Keybindings")]
    [SerializeField] public KeyCode detachKeyHead;
    [SerializeField] public KeyCode detachKeyArm;
    [SerializeField] public KeyCode detachKeyLeg;
    [SerializeField] public KeyCode keySwitchBetweenLimbs;
    [SerializeField] public KeyCode throwKey;
    [SerializeField] public KeyCode selectHeadKey;
    [SerializeField] public KeyCode selectArmKey;
    [SerializeField] public KeyCode selectLegKey;
    [SerializeField] public KeyCode changeSelectionMode;


    [Header("LimbsPrefabs")]
    [SerializeField] public GameObject headPrefab;
    [SerializeField] public GameObject leftArmPrefab;
    [SerializeField] public GameObject rightArmPrefab;
    [SerializeField] public GameObject leftLegPrefab;
    [SerializeField] public GameObject rightLegPrefab;
    [SerializeField] public GameObject wrapperSceneObject;


    [Header("Origin Transform")]
    [SerializeField] public Transform headParent;
    [SerializeField] public Transform leftArmParent;
    [SerializeField] public Transform rightArmParent;
    [SerializeField] public Transform leftLegParent;
    [SerializeField] public Transform rightLegParent;
    [SerializeField] public Transform camFocusOrigin;

    //OrginalPositions
    private Vector3 orginalHeadPosition;
    private Vector3 orginalLeftArmPosition;
    private Vector3 orginalRightArmPosition;
    private Vector3 orginalLeftLegPosition;
    private Vector3 orginalRightLegPosition;

    //BooleanForColorOnLimbs


    public List<GameObject> limbs = new List<GameObject>();
    public List<GameObject> limbsOnGround;
    public int indexControll;
    private bool isControllingLimb;

    [Header("Throwing")]
    [SerializeField] public float throwForce;
    [SerializeField] public float throwUpwardForce;
    [SerializeField] public float throwCD;
    [SerializeField] public Transform camPoint;
    [SerializeField] public Transform throwPoint;
    [SerializeField] public Transform camFocus;
    [SerializeField] public CinemachineFreeLook cinemachine;
    [SerializeField] public Vector3 throwCamOffset;
    [SerializeField] GameObject indicator;

    private Vector3 noLegCamPos;
    public bool readyToThrow;
    private Limb_enum selectedLimbToThrow = Limb_enum.Head;
    private GameObject headObj;
    private bool dragging;
    private Vector3 mousePressDownPos;
    private Vector3 dir;
    private Vector3 mouseReleasePos;
    private GameObject sceneObjectHoldingToThrow;
    private Vector3 orignalPosition = Vector3.zero;
    public int numberOfLimbs;
    private List<GameObject> objsToDelete = new List<GameObject>();

    CharacterControl characterControlScript;

    //Color and selectionMode
    [SyncVar] public bool isDeta;
    [SyncVar] private bool rightLegIsDeta;
    [SyncVar] private bool leftLegIsDeta;
    [SyncVar] private bool rightArmIsDeta;
    [SyncVar] private bool leftArmIsDeta;
    [SyncVar] private int selectionMode; //0 == limbSelection mode, 1 == out on map limb selection mode
    private LimbTextureManager limbTextureManager;
    public bool groundMode;

    bool leftLegExist, rightLegExist;
    bool leftArmExist, rightArmExist;

    public readonly UnityEvent pickupLimbEvent = new UnityEvent();
    public readonly UnityEvent changeSelectedLimbEvent = new UnityEvent();
    public readonly UnityEvent dropLimbEvent = new UnityEvent();
    public bool testControlOwnLimb = true;

    private InteractionChecker interactionChecker;
    public bool allowInteraction = true;
    public bool AllowInteraction
    {
        get => allowInteraction;
        set
        {
            allowInteraction = value;
            if (interactionChecker == null)
                interactionChecker = FindObjectOfType<Camera>().GetComponent<InteractionChecker>();
            interactionChecker.AllowInteraction = value;
        }
    }


    /// <summary>
    /// 0 == limbSelection mode, 1 == out on map limb selection mode
    /// </summary>
    public int SelectionMode
    {
        get => selectionMode;
    }

    public Limb_enum SelectedLimbToThrow
    {
        get => selectedLimbToThrow;
    }

    #region Syncvars with hooks

    [Header("Syncvars")]
    [SyncVar(hook = nameof(OnChangeHeadDetachedHook))]
    public bool headDetached;

    [SyncVar(hook = nameof(OnChangeLeftArmDetachedHook))]
    public bool leftArmDetached;

    [SyncVar(hook = nameof(OnChangeRightArmDetachedHook))]
    public bool rightArmDetached;

    [SyncVar(hook = nameof(OnChangeRightLegDetachedHook))]
    public bool rightLegDetached;

    [SyncVar(hook = nameof(OnChangeLeftLegDetachedHook))]
    public bool leftLegDetached;

    #endregion

    public enum Limb_enum
    {
        Head,
        Leg,
        Arm
    }

    #region Hook Functions

    private void OnChangeLeftArmDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            leftArmPrefab.SetActive(false);
            numberOfLimbs--;

        }
        else // if Detached == False
        {
            leftArmPrefab.SetActive(true);
            numberOfLimbs++;
            limbTextureManager.ChangeColorOfLimb(Limb_enum.Arm, leftArmPrefab, leftArmIsDeta);

        }
    }
    private void OnChangeRightArmDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            rightArmPrefab.SetActive(false);
            numberOfLimbs--;

        }
        else // if Detached == False
        {
            rightArmPrefab.SetActive(true);
            numberOfLimbs++;
            limbTextureManager.ChangeColorOfLimb(Limb_enum.Arm, rightArmPrefab, rightArmIsDeta);

        }
    }
    private void OnChangeHeadDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            headPrefab.SetActive(false);
            numberOfLimbs--;

        }
        else // if Detached == False
        {
            headPrefab.SetActive(true);
            numberOfLimbs++;


        }
    }
    private void OnChangeLeftLegDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            leftLegPrefab.SetActive(false);
            numberOfLimbs--;

        }
        else // if Detached == False
        {
            leftLegPrefab.SetActive(true);
            numberOfLimbs++;
            limbTextureManager.ChangeColorOfLimb(Limb_enum.Leg, leftLegPrefab, leftLegIsDeta);
        }
    }
    private void OnChangeRightLegDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            rightLegPrefab.SetActive(false);
            numberOfLimbs--;

        }
        else // if Detached == False
        {
            rightLegPrefab.SetActive(true);
            numberOfLimbs++;
            limbTextureManager.ChangeColorOfLimb(Limb_enum.Leg, rightLegPrefab, rightLegIsDeta);
        }
    }

    #endregion

    private void Awake()
    {
        limbTextureManager = gameObject.GetComponent<LimbTextureManager>();
        numberOfLimbs = 5;
        selectionMode = 0;
        characterControlScript = gameObject.GetComponent<CharacterControl>();
        noLegCamPos = new Vector3(camFocus.localPosition.x + characterControlScript.noLegCamOffset.x, camFocus.localPosition.y + characterControlScript.noLegCamOffset.y, camFocus.localPosition.z + characterControlScript.noLegCamOffset.z);

        /* originalCamTransform.position = camFocus.localPosition;
         originalCamTransform.eulerAngles = camFocus.localEulerAngles;
         originalCamTransform.rotation = camFocus.localRotation;*/
        cinemachine = FindObjectOfType<CinemachineFreeLook>();
        ///TODO
        ///Check what maxCamHeight is?

        //MAX CAM HEIGHT IS HOW HIGH UP PLAYER CAN LOOK, 0 = look at sky, 1 = look at ground, MinValue = 0.2 prevents player from look all the way up to the sky

        //maxCamHeight = 0.2f;
        //cinemachine.m_YAxis.m_MinValue = maxCamHeight; 
    }
    /* All drop/throw updates happens below.
     * All pickup checks happen on each object in script: SceneObjectManager  
     */
    private void Start()
    {
        camPoint = Camera.main.transform;
        rightLegIsDeta = isDeta;
        leftLegIsDeta = isDeta;
        rightArmIsDeta = isDeta;
        leftArmIsDeta = isDeta;
    }
    void Update()
    {
        if (!isLocalPlayer) return;

        HandleSelectionModeChange();
        HandleScrollWheelInput();

        if (selectionMode == 0)
            groundMode = false;
        else
            groundMode = true;
        if (!AllowInteraction) return;

        //Below not needed anymore, only  used for testing purposes
        if (selectionMode == 0)
        {

            if (Input.GetKeyDown(detachKeyHead) && !headDetached)
            {
                CmdDropLimb(Limb_enum.Head, gameObject);
                Transform body = transform.Find("group1");
                SFXManager.PlayOneShotAttached(SFXManager.ThrowSound, VolumeManager.GetSFXVolume(), body.gameObject);
            }
            if (Input.GetKeyDown(detachKeyArm) && (!leftArmDetached || !rightArmDetached))
            {
                CmdDropLimb(Limb_enum.Arm, gameObject);
                Transform body = transform.Find("group1");
                SFXManager.PlayOneShotAttached(SFXManager.ThrowSound, VolumeManager.GetSFXVolume(), body.gameObject);
            }
            if (Input.GetKeyDown(detachKeyLeg) && (!leftLegDetached || !rightLegDetached))
            {
                CmdDropLimb(Limb_enum.Leg, gameObject);
                Transform body = transform.Find("group1");
                SFXManager.PlayOneShotAttached(SFXManager.ThrowSound, VolumeManager.GetSFXVolume(), body.gameObject);
            }
        }


        UpdateThrowButton();

        if (dragging)
            TrajectoryCal();

        #region desperate af
        if (!leftLegDetached)
            leftLegExist = false;

        if (!rightLegDetached)
            rightLegExist = false;

        if (!leftArmDetached)
            leftArmExist = false;

        if (!rightArmDetached)
            rightArmExist = false;
        #endregion
    }

    private void HandleScrollWheelInput()
    {
        if (Input.mouseScrollDelta.y < 0 || Input.mouseScrollDelta.y > 0)
        {
            if (selectionMode == 0)
            {
                ChangeSelectedLimbToThrow(Input.mouseScrollDelta.y);
            }
            else if (selectionMode == 1)
            {

                ChangeLimbControll(Input.mouseScrollDelta.y); //Change this to handle the scroll up and down
            }
            changeSelectedLimbEvent.Invoke();
        }
    }

    private void ChangeSelectedLimbToThrow(float scrollDelta)
    {
        int change = (int)scrollDelta;
        int currentlySelected = (int)selectedLimbToThrow;
        int allEnums = Enum.GetNames(typeof(Limb_enum)).Length;
        int goingToSelect = (currentlySelected + -change) % allEnums;
        if (goingToSelect < 0) goingToSelect = allEnums - 1; //Modulo in c# is remainder not modulo...
        selectedLimbToThrow = (Limb_enum)goingToSelect;
        if (numberOfLimbs > 0 && !CanDropSelected())
            ChangeSelectedLimbToThrow(1);
    }

    /// <summary>
    /// Changes Selection mode for player, 0 == limbs on player, 1 == limbs on ground.
    /// if player isn't controlling the "main" body then he cant switch to limb selection
    /// </summary>
    private void HandleSelectionModeChange()
    {
        if (Input.GetKeyDown(changeSelectionMode))
        {
            //limbsOnGround = GameObject.FindGameObjectsWithTag("Limb");
            if (selectionMode == 0)
            {
                selectionMode = 1;
                GetAllPlayerLimbsInScene();
            }


            else if (characterControlScript.isBeingControlled == false && selectionMode == 1) //0 == limbSelection mode, 1 == on ground limb selection mode
            {
                selectionMode = 0;
                ChangeControllingforLimbAndPlayer(limbs[indexControll], false);
                ChangeControllingforLimbAndPlayer(gameObject, true);
            }
            else if (selectionMode == 1)
            {
                selectionMode = 0;
                // indexControll = 0;
            }

            if (selectionMode == 0)
            {
                CamPositionReset();
                // indexControll = 0;
            }
        }
    }

    #region Check status of players limbs

    public bool HasBothLegs() => NumberOfLegs > 1;

    public bool HasBothArms() => NumberOfArms > 1;

    public bool HasHead()
    {
        return headDetached;
    }

    public int NumberOfLegs
    {
        get
        {
            int i = 0;
            if (!rightLegDetached) ++i;
            if (!leftLegDetached) ++i;
            return i;
        }
    }
    public int NumberOfArms
    {
        get
        {
            int i = 0;
            if (!rightArmDetached) ++i;
            if (!leftArmDetached) ++i;
            return i;
        }
    }



    public bool CheckIfMissingLimb(Limb_enum limb)
    {

        return limb switch
        {
            Limb_enum.Arm => rightArmDetached || leftArmDetached,
            Limb_enum.Leg => rightLegDetached || leftLegDetached,
            Limb_enum.Head => headDetached,
            _ => false,
        };
    }

    #endregion

    #region Spawn Functions
    public void SetAmountOfLimbsToSpawn(int arms, int legs)
    {
        switch (arms)
        {
            case 0:
                leftArmDetached = true;
                rightArmDetached = true;
                break;

            case 1:
                leftArmDetached = true;
                break;
            case 2:
                break;
        }

        switch (legs)
        {
            case 0:
                leftLegDetached = true;
                rightLegDetached = true;
                break;

            case 1:
                leftLegDetached = true;
                break;
            case 2:
                break;
        }
    }
    #endregion

    #region LimbControll
    void ChangeLimbControll(float scrollDelta)
    {
        //If no limbs is out, this is failsafe to return control over body.
        if (limbs.Count == 0)
        {
            gameObject.GetComponent<CharacterControl>().isBeingControlled = true;
            return;
        }

        ChangeControllingforLimbAndPlayer(limbs[indexControll], false);
        CheckIfRemoveClientAuthority(limbs[indexControll]);

        indexControll += scrollDelta > 0 ? 1 : -1;
        indexControll %= limbs.Count;
        indexControll = Math.Abs(indexControll);

        if (limbs[indexControll] != gameObject)
        {
            ChangeControllingforLimbAndPlayer(gameObject, false); //If this check dosen't happen, player can control limbs and body at the same time
        }

        //Checks if other player is controlling, if true: move index one further
        if (CheckIfOtherPlayerIsControllingLimb(limbs[indexControll]))
        {
            indexControll++;
            indexControll %= limbs.Count;
        }
        ChangeControllingforLimbAndPlayer(limbs[indexControll], true);
        camFocus.parent = limbs[indexControll].transform; //bug

        //if (limbs[indexControll] == (rightArmObject || leftArmObject || headObj))
        //    cinemachine.m_YAxis.m_MinValue = 0.25f;
        //else
        ///TODO
        ///Check what maxCamHeight is?
            //cinemachine.m_YAxis.m_MinValue = maxCamHeight; //check in Awake()

        camFocus.localPosition = Vector3.zero;
        camFocus.localEulerAngles = Vector3.zero;
        camFocus.localScale = Vector3.one;
        CheckIfAddClientAuthority(limbs[indexControll]);
    }

    private void SetCamFocusOnPlayer()
    {
        camFocus.parent = gameObject.transform;
        camFocus.localPosition = Vector3.zero;
        camFocus.localEulerAngles = Vector3.zero;
        camFocus.localScale = Vector3.one;
    }

    public void ReturnControllToPlayer()
    {
        //Expected that all handling of other limb controll removement is done
        gameObject.GetComponent<CharacterControl>().isBeingControlled = true;
        SetCamFocusOnPlayer();
    }

    private bool CheckIfOtherPlayerIsControllingLimb(GameObject objToCheck)
    {
        if (objToCheck != gameObject)
        {
            if (objToCheck.GetComponent<SceneObjectItemManager>().IsBeingControlled)
            {
                return true;
            }
        }
        return false;
    }
    private void ChangeControllingforLimbAndPlayer(GameObject objToCheck, bool value)
    {
        if (objToCheck == gameObject)
        {
            gameObject.GetComponent<CharacterControl>().isBeingControlled = value;
            isControllingLimb = !value;
            AllowInteraction = value;
        }
        else
        {
            var sceneObject = objToCheck.GetComponent<SceneObjectItemManager>();
            sceneObject.IsBeingControlled = value;
            sceneObject.itemManager = value ? this : null;
        }
    }

    private void CheckIfRemoveClientAuthority(GameObject objToCheck)
    {
        if (objToCheck != gameObject)
        {
            if (objToCheck.GetComponent<SceneObjectItemManager>().thisLimb != Limb_enum.Head)
            {
                CmdRemoveClientAutohrity(objToCheck);
            }
        }
    }
    private void CheckIfAddClientAuthority(GameObject objToCheck)
    {
        if (objToCheck != gameObject)
        {
            if (objToCheck.GetComponent<SceneObjectItemManager>().thisLimb != Limb_enum.Head)
            {
                CmdAssignClientAuthority(limbs[indexControll]);
            }
        }
    }

    private void GetAllPlayerLimbsInScene()
    {
        try
        {
            limbs.Clear();
            GameObject[] limbsInScene = GameObject.FindGameObjectsWithTag("Limb");
            //limbs.AddRange(GameObject.FindGameObjectsWithTag("Limb"));
            //foreach (GameObject limb in limbsInScene)

            for (int i = 0; i <= limbsInScene.Length; i++)
            {

                if (limbsInScene[i].GetComponent<SceneObjectItemManager>().originalOwner == gameObject)
                {
                    //blood, tears, sweat and other body fluid
                    #region onGround limb names
                    if (limbsInScene[i].transform.GetChild(0).gameObject.tag == "Leg" && leftLegDetached && !leftLegExist)
                    {
                        if (!limbs.Exists(x => limbsInScene[i].name == "RightLeg"))
                        {
                            limbsInScene[i].name = "LeftLeg";
                            limbsInScene[i].transform.GetChild(0).gameObject.name = "LeftLeg";
                            leftLegExist = true;
                        }
                    }
                    else if (limbsInScene[i].transform.GetChild(0).gameObject.tag == "Leg" && rightLegDetached && leftLegExist && !rightLegExist)
                    {
                        if (!limbs.Exists(x => limbsInScene[i].name == "LeftLeg"))
                        {
                            limbsInScene[i].name = "RightLeg";
                            limbsInScene[i].transform.GetChild(0).gameObject.name = "RightLeg";
                            rightLegExist = true;
                        }
                    }

                    if (limbsInScene[i].transform.GetChild(0).gameObject.tag == "Arm" && leftArmDetached && !leftArmExist)
                    {
                        if (!limbs.Exists(x => limbsInScene[i].name == "RightArm"))
                        {
                            limbsInScene[i].name = "LeftArm";
                            limbsInScene[i].transform.GetChild(0).gameObject.name = "LeftArm";
                            leftArmExist = true;
                        }
                    }
                    else if (limbsInScene[i].transform.GetChild(0).gameObject.tag == "Arm" && rightArmDetached && leftArmExist && !rightArmExist)
                    {
                        if (!limbs.Exists(x => limbsInScene[i].name == "LeftArm"))
                        {
                            limbsInScene[i].name = "RightArm";
                            limbsInScene[i].transform.GetChild(0).gameObject.name = "RightArm";
                            rightArmExist = true;
                        }
                    }
                    #endregion 

                    limbs.Add(limbsInScene[i]);
                }
            }
            if (limbs.Count == 0)
            {
                return;
            }

            limbs.Add(gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("Tag dosen't exist, Have you forgotten to add \"limb\" to your tags?");
            Debug.Log(e.Message);
        }
    }

    private void GetAllLimbsInScene()

    {
        try
        {
            limbs.Clear();
            GameObject[] limbsInScene = GameObject.FindGameObjectsWithTag("Limb");
            //limbs.AddRange(GameObject.FindGameObjectsWithTag("Limb"));
            foreach (GameObject limb in limbsInScene)
            {
                if (limb.GetComponent<SceneObjectItemManager>().originalOwner == gameObject)
                {
                    limbs.Add(limb);
                }
            }
            if (limbs.Count == 0)
            {
                return;
            }

            limbs.Add(gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("Tag dosen't exist, Have you forgotten to add \"limb\" to your tags?");
            Debug.Log(e.Message);
        }
    }

    [Command]
    void CmdAssignClientAuthority(GameObject sceneObject)
    {
        sceneObject.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }

    [Command]
    void CmdRemoveClientAutohrity(GameObject sceneObject)
    {
        sceneObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
    }


    #endregion

    [Command]
    void CmdDropLimb(Limb_enum limb, GameObject orginalOwner)
    {
        DropLimb(limb, orginalOwner);
    }
    [Command]
    void CmdThrowLimb(Limb_enum limb, Vector3 force, Vector3 throwPoint, GameObject orignalOwner)
    {
        //sceneObject.GetComponent<Rigidbody>().useGravity = true;
        //sceneObject.GetComponent<SceneObjectItemManager>().IsBeingControlled = false;
        ThrowLimb(force, CmdThrowDropLimb(limb, throwPoint, orignalOwner));
    }


    #region DropLimb/ThrowLimb

    bool CheckIfSelectedCanBeThrown()
    {
        if (rightArmDetached)
            return false;
        switch (selectedLimbToThrow)
        {
            case Limb_enum.Head:
                if (!headDetached)
                    return true;
                break;
            case Limb_enum.Arm:
                if (!leftArmDetached)
                    return true;
                break;
            case Limb_enum.Leg:
                if (!rightLegDetached || !leftLegDetached)
                    return true;
                break;
        }
        return false;
    }
    private bool CanDropSelected()
    {
        return selectedLimbToThrow switch
        {
            Limb_enum.Head => !headDetached,
            Limb_enum.Arm => !rightArmDetached || !leftArmDetached,
            Limb_enum.Leg => !rightLegDetached || !leftLegDetached,
            _ => false,
        };
    }
    [Server]
    GameObject DropLimb(Limb_enum limb, GameObject originalOwner)
    {
        GameObject newSceneObject = null;
        switch (limb)
        {
            case Limb_enum.Head:
                newSceneObject = Instantiate(wrapperSceneObject, throwPoint.position, headPrefab.transform.rotation);
                var SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                SceneObjectScript.thisLimb = limb;  //This must come before detached = true and networkServer.spawn               
                SceneObjectScript.originalOwner = originalOwner;
                NetworkServer.Spawn(newSceneObject, connectionToClient); //Set Authority to client att spawn since no other player should be able to control it.
                SceneObjectScript.detached = true;
                headDetached = true;
                break;

            case Limb_enum.Arm:
                Vector3 extraRotation = new Vector3(0, 90);
                if (!leftArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwPoint.position, Quaternion.Euler(leftArmParent.eulerAngles + extraRotation));
                    DropGenericLimb(newSceneObject, limb, originalOwner, leftArmIsDeta);
                    leftArmDetached = true;
                }
                else if (!rightArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwPoint.position, Quaternion.Euler(rightArmParent.eulerAngles + extraRotation));
                    DropGenericLimb(newSceneObject, limb, originalOwner, rightArmIsDeta);
                    rightArmDetached = true;
                }
                else
                {
                    Debug.Log("No arm to detach");
                }
                break;
            case Limb_enum.Leg:
                if (!leftLegDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwPoint.position, leftLegParent.rotation);
                    DropGenericLimb(newSceneObject, limb, originalOwner, leftLegIsDeta);
                    leftLegDetached = true;
                }
                else if (!rightLegDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwPoint.position, rightLegParent.rotation);
                    DropGenericLimb(newSceneObject, limb, originalOwner, rightLegIsDeta);
                    rightLegDetached = true;
                }
                else
                {
                    Debug.Log("No leg to detach");
                }
                break;
            default:
                return null;
        }

        dropLimbEvent.Invoke();
        return newSceneObject;
    }

    [Server]
    GameObject CmdThrowDropLimb(Limb_enum limb, Vector3 throwpoint, GameObject originalOwner)
    {
        GameObject newSceneObject = null;
        switch (limb)
        {
            case Limb_enum.Head:
                newSceneObject = Instantiate(wrapperSceneObject, throwpoint, headPrefab.transform.rotation);
                var SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                SceneObjectScript.thisLimb = limb;  //This must come before detached = true and networkServer.spawn
                SceneObjectScript.originalOwner = originalOwner;
                SceneObjectScript.isDeta = isDeta;
                NetworkServer.Spawn(newSceneObject, connectionToClient); //Set Authority to client att spawn since no other player should be able to control it.
                SceneObjectScript.detached = true;
                headDetached = true;
                //camFocus.parent = SceneObjectScript.transform;
                TargetRpcUpdateCameraPositionForHeadOnClient(originalOwner.GetComponent<NetworkIdentity>().connectionToClient, SceneObjectScript);
                ResetHeadPosCam(SceneObjectScript);
                SceneObjectScript.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                break;

            case Limb_enum.Arm:
                if (!leftArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwpoint, leftArmParent.transform.rotation);
                    DropGenericLimb(newSceneObject, limb, originalOwner, leftArmIsDeta);
                    leftArmDetached = true;

                    // camFocus.parent = SceneObjectScript.transform;
                    /*  SceneObjectScript.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;*/
                }
                else
                {
                    Debug.Log("No arm to detach");
                }
                break;
            case Limb_enum.Leg:
                if (!leftLegDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwpoint, leftLegParent.transform.rotation);
                    DropGenericLimb(newSceneObject, limb, originalOwner, leftLegIsDeta);
                    leftLegDetached = true;
                }
                else if (!rightLegDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwpoint, rightLegParent.transform.rotation);
                    DropGenericLimb(newSceneObject, limb, originalOwner, rightLegIsDeta);
                    rightLegDetached = true;
                }
                else
                {
                    Debug.Log("No leg to detach");
                }
                break;
            default:
                return null;



        }
        //SceneObjectScript.IsBeingControlled = true;
        //newSceneObject.GetComponent<Rigidbody>().useGravity = false;
        //TargetRpcGetThrowingGameObject(identity, newSceneObject);
        dropLimbEvent.Invoke();
        return newSceneObject;
    }

    [Command]
    private void LateNetworkDestroy()
    {
        foreach(GameObject obj in objsToDelete)
        {
            NetworkServer.Destroy(obj);
        }
    }

    private void ResetHeadPosCam(SceneObjectItemManager SceneObjectScript)
    {
        SceneObjectScript.transform.rotation = Quaternion.Euler(new Vector3(0, camPoint.rotation.eulerAngles.y, 0));
        camFocus.localPosition = Vector3.zero;
        camFocus.localEulerAngles = Vector3.zero;
        camFocus.localScale = Vector3.one;
    }

    private void TrajectoryCal()
    {
        #region trash code
        ////Quaternion dir = Quaternion.AngleAxis(camPoint.rotation.eulerAngles.y, Vector3.up);
        //Vector3 forceInit = Input.mousePosition - mousePressDownPos /*+ camPoint.transform.forward * throwForce + transform.up * throwUpwardForce*/; //idek what im doing anymore
        //Vector3 dir = Quaternion.AngleAxis(camPoint.rotation.eulerAngles.y, Vector3.up) * forceInit;
        //Vector3 forceV = new Vector3(dir.x, dir.y, z: dir.y);
        ////Vector3 forceV = new Vector3(forceInit.x, forceInit.y, z: forceInit.y);


        ////dir = (Input.mousePosition - mousePressDownPos).normalized;
        ///
        #endregion

        /* Vector3 upForce = (Input.mousePosition - mousePressDownPos).normalized;
         throwUpwardForce = upForce.y * 4;*/
        DrawTrajectory.instance.DrawProjection(camPoint.transform.forward, transform.up, throwPoint.position, throwForce, throwUpwardForce);
    }

    private GameObject GetGameObjectLimbFromSelect()
    {
        switch (selectedLimbToThrow)
        {
            case Limb_enum.Arm:
                return leftArmPrefab;
            case Limb_enum.Head:
                return headPrefab;
            case Limb_enum.Leg:
                if (!leftLegDetached)
                    return leftLegPrefab;
                else if (!rightLegDetached)
                    return rightLegPrefab;
                break;
        }
        return null;
    }

    private void UpdateThrowButton()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!CheckIfSelectedCanBeThrown()) return;

            // mousePressDownPos = Input.mousePosition;

            readyToThrow = true;
            dragging = true;
            indicator.SetActive(true);
            //cam when aiming
            camFocus.localPosition = new Vector3(camFocus.localPosition.x + throwCamOffset.x, camFocus.localPosition.y + throwCamOffset.y, camFocus.localPosition.z + throwCamOffset.z);

            sceneObjectHoldingToThrow = Instantiate(GetGameObjectLimbFromSelect());
            sceneObjectHoldingToThrow.transform.localPosition = throwPoint.position;

            SFXManager.PlayOneShotAttached(SFXManager.DetachSound, VolumeManager.GetSFXVolume(), transform.gameObject);

            float chargeUpSpeed = 3f;
            cinemachine.m_YAxis.m_MaxSpeed = chargeUpSpeed;
            //if (cinemachine.m_YAxis.m_MaxValue <= 0.35f)

            float maxThrowHeight = 0.28f; //from cam perspective
            cinemachine.m_YAxis.m_MinValue = maxThrowHeight;
           // camFocus.localPosition = new Vector3(camFocus.localPosition.x + throwCamOffset.x, camFocus.localPosition.y + throwCamOffset.y, camFocus.localPosition.z + throwCamOffset.z);

        }
        else if (Input.GetMouseButtonUp(1))
        {
            readyToThrow = false;
            dragging = false;
            DrawTrajectory.instance.HideLine();
            indicator.SetActive(false);

            //if (SelectedLimbToThrow == Limb_enum.Head)
            ResetCamCondition();


            cinemachine.m_YAxis.m_MaxSpeed = 10;
            cinemachine.m_YAxis.m_MinValue = 0;
            if (sceneObjectHoldingToThrow != null)
            {
                Destroy(sceneObjectHoldingToThrow);
            }

        }
        if (Input.GetMouseButtonUp(0) && readyToThrow && sceneObjectHoldingToThrow != null)
        {
            dragging = false;
            DrawTrajectory.instance.HideLine();
            indicator.SetActive(false);
            // mouseReleasePos = Input.mousePosition;
            Destroy(sceneObjectHoldingToThrow);

            cinemachine.m_YAxis.m_MaxSpeed = 10;
            cinemachine.m_YAxis.m_MinValue = 0;


            //ending point - starting point + cam movement
            // dir = (Input.mousePosition - mousePressDownPos).normalized;
            // CmdThrowLimb(selectedLimbToThrow, force: camPoint.transform.forward * throwForce + transform.up * throwUpwardForce, throwPoint.position);
            CmdThrowLimb(selectedLimbToThrow, force: camPoint.transform.forward * throwForce + transform.up * throwUpwardForce, throwPoint.position, gameObject);
            if (SelectedLimbToThrow == Limb_enum.Head)
            {
                characterControlScript.isBeingControlled = false;
                selectionMode = 1;
            }

            Transform body = transform.Find("group1");
            SFXManager.PlayOneShotAttached(SFXManager.ThrowSound, VolumeManager.GetSFXVolume(), body.gameObject);

        }
    }

    [Server]
    void ThrowLimb(Vector3 force, GameObject sceneObject)
    {

        readyToThrow = false;

        Rigidbody objectRb;

        objectRb = sceneObject.GetComponent<Rigidbody>();

        /*  Vector3 forceToAdd = new Vector3(force.x, force.y, z: force.z);*/
        //Vector3 dir = Quaternion.AngleAxis(camPoint.rotation.eulerAngles.y, Vector3.up).normalized * forceToAdd;
        objectRb.AddForce(force, ForceMode.Impulse);

        Invoke(nameof(ResetThrow), throwCD);
    }

    [Server]
    void ResetThrow()
    {
        readyToThrow = true;
    }



    [Server]
    private void DropGenericLimb(GameObject newSceneObject, Limb_enum limb, GameObject orignalOwner, bool limbIsDeta)
    {
        var SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
        SceneObjectScript.thisLimb = limb;  //This must come before detached = true and networkServer.spawn
        SceneObjectScript.isDeta = limbIsDeta;
        NetworkServer.Spawn(newSceneObject);
        SceneObjectScript.detached = true;
        SceneObjectScript.originalOwner = orignalOwner;
    }

    #endregion

    #region PickUp Limbs

    //Destroys the object in the scene and changes the syncvar of the object which then updates it on all clients.
    [Command]
    public void CmdPickUpLimb(GameObject sceneObject)
    {
        if (sceneObject == null) return;
        bool lateDelete = false;

        sceneObject.GetComponent<HighlightObject>().ForceStopHighlight();
        bool keepSceneObject = true;
        SceneObjectItemManager sceneObjectItemManager = sceneObject.GetComponent<SceneObjectItemManager>();
        allowInteraction = true;
        NetworkConnection connectionClient = sceneObject.GetComponent<NetworkIdentity>().connectionToClient;
        switch (sceneObject.GetComponent<SceneObjectItemManager>().thisLimb)
        {
            case Limb_enum.Head:

                if (headDetached && sceneObjectItemManager.originalOwner == gameObject)
                    keepSceneObject = headDetached = false;
                lateDelete = true;
                objsToDelete.Add(sceneObject);
                //camFocus.parent = camFocusOrigin;

                //camFocus.localPosition = Vector3.zero;
                //camFocus.localEulerAngles = Vector3.zero;
                //camFocus.localScale = Vector3.one;
                //CamPositionReset();

                //Head Can be picked up even if you are controlling as the head, need a check to fix this
                if (sceneObjectItemManager.isBeingControlled)
                {
                    TargetRpcLetGoHeadControll(connectionClient, sceneObjectItemManager.originalOwner);
                }

                TargetRpcSetSelectionMode(connectionClient, 0);
                TargetRpcSetCharaterIsBeingControlled(connectionClient, characterControlScript, true);
                TargetRpcCamPositionReset(connectionClient); //This methods also calls for deletion of sceneobject

                //characterControlScript.isBeingControlled = true;
                /*  camFocus = originalCamTransform;
                  Debug.Log(originalCamTransform);*/

                break;
            case Limb_enum.Arm:
                if (rightArmDetached)
                {
                    rightArmIsDeta = sceneObjectItemManager.isDeta;
                    keepSceneObject = rightArmDetached = false;

                }

                else if (leftArmDetached)
                {
                    leftArmIsDeta = sceneObjectItemManager.isDeta;
                    keepSceneObject = leftArmDetached = false;

                }

                //Change bool of syncvars. When hook 
                else
                    Debug.Log("No Spots to attach arm to");
                break;
            case Limb_enum.Leg:
                if (rightLegDetached)
                {
                    rightLegIsDeta = sceneObjectItemManager.isDeta;
                    keepSceneObject = rightLegDetached = false;

                    //TODO implement better fix for preventing getting stuck
                    float moveHeight = 5f;
                    MovePlayer(transform.up * moveHeight);
                }

                else if (leftLegDetached)
                {
                    leftLegIsDeta = sceneObjectItemManager.isDeta;
                    keepSceneObject = leftLegDetached = false;
                }
                else
                    Debug.Log("No Spots to attach leg to");
                break;
            default:
                return;
        }

        if (!keepSceneObject && !lateDelete)
            NetworkServer.Destroy(sceneObject);
        pickupLimbEvent.Invoke();
    }



    [Command(requiresAuthority = false)]
    private void MovePlayer(Vector3 displacement) => RPCMovePlayer(displacement);

    [ClientRpc]
    private void RPCMovePlayer(Vector3 displacement) => transform.position += displacement;

    public void CamPositionReset()
    {
        camFocus.parent = camFocusOrigin;

        ResetCamCondition();

        camFocus.localEulerAngles = Vector3.zero;
        camFocus.localScale = Vector3.one;
    }
    #endregion


    #region TargetRpcFunctions

    [TargetRpc]
    private void TargetRpcLetGoHeadControll(NetworkConnection connectionClient, GameObject orginalOwner)
    {
        ChangeControllingforLimbAndPlayer(orginalOwner, true);
    }

    [TargetRpc]
    private void TargetRpcUpdateCameraPositionForHeadOnClient(NetworkConnection client, SceneObjectItemManager SceneObjectScript)
    {
        camFocus.parent = SceneObjectScript.transform;
    }

    [TargetRpc]
    private void TargetRpcSetCharaterIsBeingControlled(NetworkConnection connection, CharacterControl characterControlScript, bool isBeingControlled)
    {
        characterControlScript.isBeingControlled = isBeingControlled;
    }
    [TargetRpc]
    private void TargetRpcSetSelectionMode(NetworkConnection connection, int mode)
    {
        selectionMode = mode;
    }

    [TargetRpc]
    private void TargetRpcCamPositionReset(NetworkConnection connectionToClient)
    {
        
        camFocus.parent = camFocusOrigin;

        ResetCamCondition();

        camFocus.localEulerAngles = Vector3.zero;
        camFocus.localScale = Vector3.one;

        LateNetworkDestroy(); 
    }

    #endregion
    void ResetCamCondition()
    {
        if (NumberOfLegs >= 1)
            camFocus.localPosition = Vector3.zero;
        else
            camFocus.localPosition = noLegCamPos;
    }
}