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
        if(Input.GetKeyDown(detachKeyHead) && leftArmDetached == false){

        }
        else if(Input.GetKeyDown(detachKeyHead) && rightArmDetached == false)
        {

        }
    }


    [Command]
    void CmdDropLimb(Limb_enum limb)
    {       
        GameObject newSceneObject = Instantiate(wrapperSceneObject, headObject.transform.position, headObject.transform.rotation);
        NetworkServer.Spawn(newSceneObject);
        newSceneObject.GetComponent<SceneObjectItemManager>().detached = true;      
        headDetached = true;    
    }

    [Command]
    public void CmdPickUpLimb(GameObject sceneObject)
    {
        //Destroy(Limb.GetComponent<Rigidbody>());
        //Limb.transform.parent = limbParent;
        //Limb.transform.localPosition = Vector3.zero;
        //Limb.transform.localEulerAngles = Vector3.zero;
        //Limb.transform.localScale = Vector3.one;
        headDetached = false;
        NetworkServer.Destroy(sceneObject);
    }
}
