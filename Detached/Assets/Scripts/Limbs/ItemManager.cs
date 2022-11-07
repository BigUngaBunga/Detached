using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Events;

public class ItemManager : NetworkBehaviour
{
    //Todo, Make ui manager that handles input from player instead of hardcoded keybindings
    [Header("Keybindings")]
    [SerializeField] public KeyCode detachKeyHead;
    [SerializeField] public KeyCode detachKeyArm;
    [SerializeField] public KeyCode detachKeyLeg;
    [SerializeField] public KeyCode keySwitchBetweenLimbs;
    [SerializeField] public KeyCode throwKey;
    [SerializeField] public KeyCode selectHeadKey;
    [SerializeField] public KeyCode selectArmKey;
    [SerializeField] public KeyCode selectLegKey;

    [Header("LimbsPrefabs")]
    [SerializeField] public GameObject headObject;
    [SerializeField] public GameObject leftArmObject;
    [SerializeField] public GameObject rightArmObject;
    [SerializeField] public GameObject leftLegObject;
    [SerializeField] public GameObject rightLegObject;
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


    private List<GameObject> limbs = new List<GameObject>();
    private int indexControll;
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

    private bool readyToThrow;
    private Limb_enum selectedLimbToThrow = Limb_enum.Head;
    private GameObject headObj;
    private bool dragging;
    private Vector3 mousePressDownPos;
    private Vector3 dir;
    private Vector3 mouseReleasePos;
    private GameObject sceneObjectHoldingToThrow;
    private Vector3 orignalPosition = Vector3.zero;

    public readonly UnityEvent dropLimbEvent = new UnityEvent();

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
            leftArmObject.SetActive(false);

        }
        else // if Detached == False
        {
            leftArmObject.SetActive(true);
        }
    }
    private void OnChangeRightArmDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            rightArmObject.SetActive(false);

        }
        else // if Detached == False
        {
            rightArmObject.SetActive(true);
        }
    }
    private void OnChangeHeadDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            headObject.SetActive(false);

        }
        else // if Detached == False
        {
            headObject.SetActive(true);

        }
    }
    private void OnChangeLeftLegDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            leftLegObject.SetActive(false);

        }
        else // if Detached == False
        {
            leftLegObject.SetActive(true);

        }
    }
    private void OnChangeRightLegDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            rightLegObject.SetActive(false);

        }
        else // if Detached == False
        {
            rightLegObject.SetActive(true);

        }
    }

    #endregion

    private void Awake()
    {
        /* originalCamTransform.position = camFocus.localPosition;
         originalCamTransform.eulerAngles = camFocus.localEulerAngles;
         originalCamTransform.rotation = camFocus.localRotation;*/
    }
    /* All drop/throw updates happens below.
     * All pickup checks happen on each object in script: SceneObjectManager
     * 
     */
    private void Start()
    {
        camPoint = Camera.main.transform;


    }
    void Update()
    {
        if (!isLocalPlayer || !AllowInteraction) return;

        if (Input.GetKeyDown(detachKeyHead) && headDetached == false)
            CmdDropLimb(Limb_enum.Head);
        if (Input.GetKeyDown(detachKeyArm) && (leftArmDetached == false || rightArmDetached == false))
            CmdDropLimb(Limb_enum.Arm);
        if (Input.GetKeyDown(detachKeyLeg) && (leftLegDetached == false || rightLegDetached == false))
            CmdDropLimb(Limb_enum.Leg);
        if (Input.GetKeyDown(keySwitchBetweenLimbs))
        {
            GetAllLimbsInScene();
            ChangeLimbControll();
        }
        if (Input.GetKeyDown(selectHeadKey))
            selectedLimbToThrow = Limb_enum.Head;
        if (Input.GetKeyDown(selectArmKey))
            selectedLimbToThrow = Limb_enum.Arm;
        if (Input.GetKeyDown(selectLegKey))
            selectedLimbToThrow = Limb_enum.Leg;

        UpdateThrowButton();

        if (dragging)
            TrajectoryCal();

    }

    #region Check status of players limbs

    public bool HasBothLegs() => NumberOfLegs > 1;

    public bool HasBothArms() => NumberOfArms > 1;

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

    #region LimbControll

    void ChangeLimbControll()
    {
        //If no limbs is out, this is failsafe to return control over body.
        if (limbs.Count == 0)
        {
            gameObject.GetComponent<CharacterControl>().isBeingControlled = true;
            return;
        }

        ChangeControllingforLimbAndPlayer(limbs[indexControll], false);
        CheckIfRemoveClientAuthority(limbs[indexControll]);
        indexControll++;
        indexControll %= limbs.Count;

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
        CheckIfAddClientAuthority(limbs[indexControll]);
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
        }
        else
        {
            objToCheck.GetComponent<SceneObjectItemManager>().IsBeingControlled = value;
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

    void GetAllLimbsInScene()
    {
        try
        {
            limbs.Clear();
            limbs.AddRange(GameObject.FindGameObjectsWithTag("Limb"));
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
    void CmdDropLimb(Limb_enum limb)
    {
        DropLimb(limb);
    }
    [Command]
    void CmdThrowLimb(Limb_enum limb, Vector3 force, Vector3 throwPoint)
    {
        //sceneObject.GetComponent<Rigidbody>().useGravity = true;
        //sceneObject.GetComponent<SceneObjectItemManager>().IsBeingControlled = false;

        ThrowLimb(force, CmdThrowDropLimb(limb, throwPoint));
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

    [Server]
    GameObject DropLimb(Limb_enum limb)
    {
        GameObject newSceneObject = null;
        SceneObjectItemManager SceneObjectScript = null;
        switch (limb)
        {
            case Limb_enum.Head:
                newSceneObject = Instantiate(wrapperSceneObject, headObject.transform.position, headObject.transform.rotation);
                SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                SceneObjectScript.thisLimb = limb;  //This must come before detached = true and networkServer.spawn               
                NetworkServer.Spawn(newSceneObject, connectionToClient); //Set Authority to client att spawn since no other player should be able to control it.
                SceneObjectScript.detached = true;
                headDetached = true;
                break;

            case Limb_enum.Arm:
                if (!leftArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, leftArmParent.transform.position, leftArmParent.transform.rotation);
                    DropGenericLimb(newSceneObject, SceneObjectScript, limb);
                    leftArmDetached = true;
                }
                else if (!rightArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, rightArmParent.transform.position, rightArmParent.transform.rotation);
                    DropGenericLimb(newSceneObject, SceneObjectScript, limb);
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
                    newSceneObject = Instantiate(wrapperSceneObject, leftLegParent.transform.position, leftLegParent.transform.rotation);
                    DropGenericLimb(newSceneObject, SceneObjectScript, limb);
                    leftLegDetached = true;
                }
                else if (!rightLegDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, rightArmParent.transform.position, rightArmParent.transform.rotation);
                    DropGenericLimb(newSceneObject, SceneObjectScript, limb);
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
    GameObject CmdThrowDropLimb(Limb_enum limb, Vector3 throwpoint)
    {
        GameObject newSceneObject = null;
        SceneObjectItemManager SceneObjectScript = null;
        switch (limb)
        {
            case Limb_enum.Head:
                newSceneObject = Instantiate(wrapperSceneObject, throwpoint, headObject.transform.rotation);
                SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                SceneObjectScript.thisLimb = limb;  //This must come before detached = true and networkServer.spawn               
                NetworkServer.Spawn(newSceneObject, connectionToClient); //Set Authority to client att spawn since no other player should be able to control it.
                SceneObjectScript.detached = true;
                headDetached = true;
                camFocus.parent = SceneObjectScript.transform;
                SceneObjectScript.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                break;

            case Limb_enum.Arm:
                if (!leftArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwpoint, leftArmParent.transform.rotation);
                    DropGenericLimb(newSceneObject, SceneObjectScript, limb);
                    leftArmDetached = true;
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
                    DropGenericLimb(newSceneObject, SceneObjectScript, limb);
                    leftLegDetached = true;
                }
                else if (!rightLegDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, throwpoint, rightArmParent.transform.rotation);
                    DropGenericLimb(newSceneObject, SceneObjectScript, limb);
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
        return newSceneObject;
    }



    //[TargetRpc]
    //public void TargetRpcGetThrowingGameObject(NetworkIdentity identity, GameObject sceneObject)
    //{
    //    sceneObjectHoldingToThrow = sceneObject;
    //}

    private void TrajectoryCal()
    {
        ////Quaternion dir = Quaternion.AngleAxis(camPoint.rotation.eulerAngles.y, Vector3.up);
        //Vector3 forceInit = Input.mousePosition - mousePressDownPos /*+ camPoint.transform.forward * throwForce + transform.up * throwUpwardForce*/; //idek what im doing anymore
        //Vector3 dir = Quaternion.AngleAxis(camPoint.rotation.eulerAngles.y, Vector3.up) * forceInit;
        //Vector3 forceV = new Vector3(dir.x, dir.y, z: dir.y);
        ////Vector3 forceV = new Vector3(forceInit.x, forceInit.y, z: forceInit.y);


        ////dir = (Input.mousePosition - mousePressDownPos).normalized;
        Vector3 upForce = (Input.mousePosition - mousePressDownPos).normalized;
        throwUpwardForce = upForce.y;
        DrawTrajectory.instance.DrawProjection(camPoint.transform.forward,transform.up, throwPoint.position,throwForce,throwUpwardForce);  
    }

    private GameObject GetGameObjectLimbFromSelect()
    {
        switch (selectedLimbToThrow)
        {
            case Limb_enum.Arm:
                return leftArmObject;
            case Limb_enum.Head:
                return headObject;
            case Limb_enum.Leg:
                if (!leftLegDetached)
                    return leftLegObject;
                else if (!rightLegDetached)
                    return rightLegObject;
                break;
        }
        return null;
    }

    private void UpdateThrowButton()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!CheckIfSelectedCanBeThrown()) return;

            mousePressDownPos = Input.mousePosition;

            readyToThrow = true;
            dragging = true;

            sceneObjectHoldingToThrow = GetGameObjectLimbFromSelect();
            sceneObjectHoldingToThrow.transform.localPosition = throwPoint.position;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            readyToThrow = false;
            dragging = false;

            DrawTrajectory.instance.HideLine();

            if (sceneObjectHoldingToThrow != null)
            {
                sceneObjectHoldingToThrow.transform.localPosition = Vector3.zero;
                sceneObjectHoldingToThrow = null;
            }

        }
        if (Input.GetMouseButtonUp(0) && readyToThrow && sceneObjectHoldingToThrow != null)
        {

            DrawTrajectory.instance.HideLine();
            mouseReleasePos = Input.mousePosition;
            sceneObjectHoldingToThrow.transform.localPosition = Vector3.zero;
            sceneObjectHoldingToThrow = null;

            //ending point - starting point + cam movement
            // dir = (Input.mousePosition - mousePressDownPos).normalized;
            CmdThrowLimb(selectedLimbToThrow, force: camPoint.transform.forward * throwForce + transform.up * throwUpwardForce, throwPoint.position);


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
        objectRb.AddForce(force,ForceMode.Impulse);

        Invoke(nameof(ResetThrow), throwCD);
    }

    [Server]
    void ResetThrow()
    {
        readyToThrow = true;
    }

    [Server]
    private void DropGenericLimb(GameObject newSceneObject, SceneObjectItemManager SceneObjectScript, Limb_enum limb)
    {
        SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
        SceneObjectScript.thisLimb = limb;  //This must come before detached = true and networkServer.spawn
        NetworkServer.Spawn(newSceneObject);
        SceneObjectScript.detached = true;
    }

    #endregion

    #region PickUp Limbs

    //Destroys the object in the scene and changes the syncvar of the object which then updates it on all clients.
    [Command]
    public void CmdPickUpLimb(GameObject sceneObject)
    {
        sceneObject.GetComponent<HighlightObject>().ForceStopHighlight();
        bool keepSceneObject = true;
        switch (sceneObject.GetComponent<SceneObjectItemManager>().thisLimb)
        {
            case Limb_enum.Head:
                if (headDetached)
                    keepSceneObject = headDetached = false;

                camFocus.parent = camFocusOrigin;

                camFocus.localPosition = Vector3.zero;
                camFocus.localEulerAngles = Vector3.zero;
                camFocus.localScale = Vector3.one;

                /*  camFocus = originalCamTransform;
                  Debug.Log(originalCamTransform);*/
                break;
            case Limb_enum.Arm:
                if (rightArmDetached)
                    keepSceneObject = rightArmDetached = false;
                else if (leftArmDetached)
                    keepSceneObject = leftArmDetached = false;
                else
                    Debug.Log("No Spots to attach arm to");
                break;
            case Limb_enum.Leg:
                if (rightLegDetached)
                    keepSceneObject = rightLegDetached = false;
                else if (leftLegDetached)
                    keepSceneObject = leftLegDetached = false;
                else
                    Debug.Log("No Spots to attach leg to");
                break;
            default:
                return;
        }
        if (!keepSceneObject)
            NetworkServer.Destroy(sceneObject);
    }

    #endregion
}
