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

    [Header("LimbsParents")]
    [SerializeField] public Transform headParent;
    [SerializeField] public Transform leftArmParent;
    [SerializeField] public Transform rightArmParent;

    [Header("Syncvars")]
    [SyncVar(hook = nameof(OnChangeHeadDetached))]
    public bool headDetached;
    [SyncVar(hook = nameof(OnChangeLeftArmDetached))]
    public bool leftArmDetached;
    [SyncVar(hook = nameof(OnChangeRightArmDetached))]
    public bool rightArmDetached;

    public enum Limb_enum
    {
        Head,
        Leg,
        Arm
    }

    [SerializeField] public GameObject wrapperSceneObject;

    private void OnChangeLeftArmDetached(bool oldValue, bool newValue)
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
    private void OnChangeRightArmDetached(bool oldValue, bool newValue)
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
    private void OnChangeHeadDetached(bool oldValue, bool newValue)
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

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(detachKeyHead) && headDetached == false)
            CmdDropLimb(Limb_enum.Head);
        if(Input.GetKeyDown(detachKeyHead) && (leftArmDetached == false || rightArmDetached == false)){
            CmdDropLimb(Limb_enum.Arm);
        }
        
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
                NetworkServer.Spawn(newSceneObject);
                SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                SceneObjectScript.detached = true;
                SceneObjectScript.thisLimb = Limb_enum.Head;
                headDetached = true;
                break;

            case Limb_enum.Arm:
                if (!leftArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, headObject.transform.position, headObject.transform.rotation);
                    NetworkServer.Spawn(newSceneObject);
                    SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                    SceneObjectScript.detached = true;
                    SceneObjectScript.thisLimb = Limb_enum.Arm;
                    leftArmDetached = true;
                }
                else if(!rightArmDetached)
                {
                    newSceneObject = Instantiate(wrapperSceneObject, headObject.transform.position, headObject.transform.rotation);
                    NetworkServer.Spawn(newSceneObject);
                    SceneObjectScript = newSceneObject.GetComponent<SceneObjectItemManager>();
                    SceneObjectScript.detached = true;
                    SceneObjectScript.thisLimb = Limb_enum.Arm;
                    rightArmDetached = true;
                }
                else
                {
                    Debug.Log("No arm to detach");
                }
                break;
            case Limb_enum.Leg:
                break;
        }
         
    }

    [Command]
    public void CmdPickUpLimb(GameObject sceneObject)
    {
        switch (sceneObject.GetComponent<SceneObjectItemManager>().thisLimb)
        {
            case Limb_enum.Head:
                headDetached = false;
                NetworkServer.Destroy(sceneObject);
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

                break;
        }

        //Destroy(Limb.GetComponent<Rigidbody>());
        //Limb.transform.parent = limbParent;
        //Limb.transform.localPosition = Vector3.zero;
        //Limb.transform.localEulerAngles = Vector3.zero;
        //Limb.transform.localScale = Vector3.one;
        headDetached = false;
        NetworkServer.Destroy(sceneObject);
    }
}
