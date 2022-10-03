using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

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


    [Header("LimbsParents")]
    [SerializeField] public Transform headParent;
    [SerializeField] public Transform leftArmParent;
    [SerializeField] public Transform rightArmParent;
    [SerializeField] public Transform leftLegParent;
    [SerializeField] public Transform rightLegParent;

    private List<GameObject> limbs = new List<GameObject>();
    private int indexControll;
    private bool isControllingLimb;

    [Header("Throwing")]
    [SerializeField] public float throwForce;
    [SerializeField] public float throwUpwardForce;
    [SerializeField] public float throwCD;
    [SerializeField] public Transform cam;

    private bool readyToThrow;
    private Limb_enum selectedLimbToThrow;

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

    private void Start()
    {
        cam = Camera.main.transform;
    }
    public enum Limb_enum
    {
        Head,
        Leg,
        Arm
    }

    [SerializeField] public GameObject wrapperSceneObject;

    #region Hook Functions

    private void OnChangeLeftArmDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            leftArmObject.active = false;

        }
        else // if Detached == False
        {
            leftArmObject.active = true;
        }
    }
    private void OnChangeRightArmDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            rightArmObject.active = false;

        }
        else // if Detached == False
        {
            rightArmObject.active = true;
        }
    }
    private void OnChangeHeadDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            headObject.active = false;

        }
        else // if Detached == False
        {
            headObject.active = true;

        }
    }
    private void OnChangeLeftLegDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            leftLegObject.active = false;

        }
        else // if Detached == False
        {
            leftLegObject.active = true;

        }
    }
    private void OnChangeRightLegDetachedHook(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            rightLegObject.active = false;

        }
        else // if Detached == False
        {
            rightLegObject.active = true;

        }
    }

    #endregion

    void Update()
    {
        if (!isLocalPlayer) return;

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
        if (Input.GetKeyDown(throwKey) && CheckIfSelectedCanBeThrown())
            CmdThrowLimb(selectedLimbToThrow);

    }

    bool CheckIfSelectedCanBeThrown()
    {
        switch (selectedLimbToThrow)
        {
            case Limb_enum.Head:
                if (!headDetached)
                    return true;
                break;
            case Limb_enum.Arm:
                if (!rightArmDetached || !leftArmDetached)
                    return true;
                break;
            case Limb_enum.Leg:
                if (!rightLegDetached || !leftLegDetached)
                    return true;
                break;
        }
        return false;
    }

    //Todo this function needs to improved, Many pontential bugs from here
    void ChangeLimbControll()
    {
        if (limbs.Count == 0) return;

        //Checks wether client controlled a limb and removes client autohority from it
        if (isControllingLimb)
        {
            limbs[indexControll].GetComponent<SceneObjectItemManager>().isBeingControlled = false;
            CmdRemoveClientAutohrity(limbs[indexControll]);
            isControllingLimb = false;
        }
        indexControll++;
        indexControll %= limbs.Count;
        //Finds next limb in limb list and Gains control of it aslong as its not already being controlled
        if (limbs[indexControll] != gameObject && !limbs[indexControll].GetComponent<SceneObjectItemManager>().isBeingControlled)
        {
            limbs[indexControll].GetComponent<SceneObjectItemManager>().isBeingControlled = true;
            CmdAssignClientAuthority(limbs[indexControll]);
            isControllingLimb = true;
        }
        //If next limb in list is gameobject then player is not controlling limb. 
        if (limbs[indexControll] == gameObject)
        {
            isControllingLimb = false;
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
        }
    }

    #region ThrowLimb

    //Exaxtly the same as CmdDropLimb but it has a throw function call at the end
    //Todo see if CmdDropLimb can be reused.
    [Command]
    void CmdThrowLimb(Limb_enum limb)
    {
        GameObject newSceneObject = null;
        SceneObjectItemManager SceneObjectScript = null;
        switch (limb)
        {
            case Limb_enum.Head:
                newSceneObject = Instantiate(wrapperSceneObject, headObject.transform.position, headObject.transform.rotation);
                DropGenericLimb(newSceneObject, SceneObjectScript, limb);
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
                return;
                break;
        }
        Throw(newSceneObject);
    }

    void Throw(GameObject obj)
    {

        readyToThrow = false;
        Rigidbody objectRb;

        objectRb = obj.GetComponent<Rigidbody>();
        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwardForce;

        objectRb.AddForce(forceToAdd, ForceMode.Impulse);

        Invoke(nameof(ResetThrow), throwCD);
    }

    void ResetThrow()
    {
        readyToThrow = true;
    }

    #endregion

    #region DropLimb

    [Command]
    void CmdDropLimb(Limb_enum limb)
    {
        GameObject newSceneObject = null;
        SceneObjectItemManager SceneObjectScript = null;
        switch (limb)
        {
            case Limb_enum.Head:
                newSceneObject = Instantiate(wrapperSceneObject, headObject.transform.position, headObject.transform.rotation);
                DropGenericLimb(newSceneObject, SceneObjectScript, limb);
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
                return;
                break;
        }
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

    #region PickUp Limb

    [Command]
    public void CmdPickUpLimb(GameObject sceneObject)
    {
        switch (sceneObject.GetComponent<SceneObjectItemManager>().thisLimb)
        {
            case Limb_enum.Head:
                if (headDetached)
                {
                    headDetached = false;
                    NetworkServer.Destroy(sceneObject);
                }
                break;
            case Limb_enum.Arm:
                if (rightArmDetached)
                {
                    rightArmDetached = false;
                    NetworkServer.Destroy(sceneObject);
                }
                else if (leftArmDetached)
                {
                    leftArmDetached = false;
                    NetworkServer.Destroy(sceneObject);
                }
                else
                {
                    Debug.Log("No Spots to attach arm too");
                }
                break;
            case Limb_enum.Leg:
                if (rightLegDetached)
                {
                    rightLegDetached = false;
                    NetworkServer.Destroy(sceneObject);
                }
                else if (leftLegDetached)
                {
                    leftLegDetached = false;
                    NetworkServer.Destroy(sceneObject);
                }
                else
                {
                    Debug.Log("No Spots to attach arm too");
                }
                break;
            default:
                return;
                break;
        }


    }

    #endregion
}
