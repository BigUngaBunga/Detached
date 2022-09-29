using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemManager : NetworkBehaviour
{

    [Header("Keybindings")]
    [SerializeField] public KeyCode detachKeyHead;
    
    [Header("LimbsPrefabs")]
    [SerializeField] public GameObject headObject;
    
    
    [Header("LimbsParents")]
    [SerializeField] public Transform headParent;

    [Header("Syncvars")]
    [SyncVar(hook = nameof(OnChangeHeadDetached))]
    public bool headDetached;
    [SyncVar(hook = nameof(OnChangeArmDetached))]
    public bool leftArmDetached;
    [SyncVar(hook = nameof(OnChangeArmDetached))]
    public bool rightArmDetached;

    [SyncVar]
    public Limbs test;
    public enum Limbs
    {
        Head,
        Leg,
        Arm
    }

    [SerializeField] public GameObject wrapperSceneObject;



    private void OnChangeArmDetached(bool oldValue, bool newValue)
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
            CmdDropLimb();
        if(Input.GetKeyDown(detachKeyHead) && leftArmDetached == false){

        }
        else if(Input.GetKeyDown(detachKeyHead) && rightArmDetached == false)
        {

        }
    }


    [Command]
    void CmdDropLimb()
    {       
        GameObject newSceneObject = Instantiate(wrapperSceneObject, headObject.transform.position, headObject.transform.rotation);
        NetworkServer.Spawn(newSceneObject);
        newSceneObject.GetComponent<SceneObjectItemManager>().detached = true;      
        headDetached = true;

        test = Limbs.Arm;
        Debug.Log(test.ToString());
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

        test = Limbs.Head;
        Debug.Log(test.ToString());
    }
}
