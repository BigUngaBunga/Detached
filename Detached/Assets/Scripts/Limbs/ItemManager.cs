using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemManager : NetworkBehaviour
{
    //Todo, Make ui manager that handles input from player instead of hardcoded keybindings
    [Header("Keybindings")]
    [SerializeField] public KeyCode detachKeyHead;
    [SerializeField] public KeyCode detachKeyArm;
    [SerializeField] public KeyCode detachKeyLeg;

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
    }


    [Command]
    void CmdDropLimb(Limb_enum limb)
    {
        GameObject newSceneObject;
        SceneObjectItemManager SceneObjectScript;
        switch (limb)
        {
            case Limb_enum.Head:
                newSceneObject = Instantiate(wrapperSceneObject, headObject.transform.position, headObject.transform.rotation);
                SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                SceneObjectScript.thisLimb = Limb_enum.Head; //This must come before detached = true and networkServer.spawn
                NetworkServer.Spawn(newSceneObject);
                SceneObjectScript.detached = true;
                headDetached = true;
                break;

            case Limb_enum.Arm:
                if (!leftArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, leftArmParent.transform.position, leftArmParent.transform.rotation);
                    SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                    SceneObjectScript.thisLimb = Limb_enum.Arm; //This must come before detached = true and networkServer.spawn
                    NetworkServer.Spawn(newSceneObject);
                    SceneObjectScript.detached = true;
                    leftArmDetached = true;
                }
                else if (!rightArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, rightArmParent.transform.position, rightArmParent.transform.rotation);
                    SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                    SceneObjectScript.thisLimb = Limb_enum.Arm; //This must come before detached = true and networkServer.spawn
                    NetworkServer.Spawn(newSceneObject);
                    SceneObjectScript.detached = true;
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
                    SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                    SceneObjectScript.thisLimb = Limb_enum.Leg;  //This must come before detached = true and networkServer.spawn
                    NetworkServer.Spawn(newSceneObject);
                    SceneObjectScript.detached = true;
                    leftLegDetached = true;
                }
                else if (!rightLegDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, rightArmParent.transform.position, rightArmParent.transform.rotation);
                    SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                    SceneObjectScript.thisLimb = Limb_enum.Leg;  //This must come before detached = true and networkServer.spawn
                    NetworkServer.Spawn(newSceneObject);
                    SceneObjectScript.detached = true;
                    rightLegDetached = true;
                }
                else
                {
                    Debug.Log("No leg to detach");
                }
                break;
        }
    }

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
        }

        ////Destroy(Limb.GetComponent<Rigidbody>());
        ////Limb.transform.parent = limbParent;
        ////Limb.transform.localPosition = Vector3.zero;
        ////Limb.transform.localEulerAngles = Vector3.zero;
        ////Limb.transform.localScale = Vector3.one;
        //headDetached = false;
        //NetworkServer.Destroy(sceneObject);
    }
}
