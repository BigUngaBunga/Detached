using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemManager : NetworkBehaviour
{

    [Header("Keybindings")]
    public KeyCode detachKey;
    
    public GameObject Limb;
    public GameObject sceneObject;
    

    [SyncVar(hook = nameof(OnChangeDetached))]
    public bool detached;

    public Transform limbParent;
    public Transform detachedList;
    public Transform body;
    

    private void OnChangeDetached(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            
            Limb.active = false;
        }
        else // if Detached == False
        {
            Limb.active = true;

        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(detachKey) && detached == false)
            CmdDropLimb();   
    }
    [Command]
    void CmdDropLimb()
    {
        //GameObject newLimbObject = Instantiate(Limb, Limb.transform.position, Limb.transform.rotation);
        //newLimbObject.AddComponent<Rigidbody>();
        //Limb.transform.parent = newSceneObject.transform;

        GameObject newSceneObject = Instantiate(sceneObject, Limb.transform.position, Limb.transform.rotation);
        newSceneObject.GetComponent<SceneObjectItemManager>().limb = Limb;
        NetworkServer.Spawn(newSceneObject);

        newSceneObject.GetComponent<SceneObjectItemManager>().detached = true;
        

        detached = true;     
    }

    [Command]
    public void CmdPickUpLimb(GameObject sceneObject)
    {

        //Destroy(Limb.GetComponent<Rigidbody>());
        //Limb.transform.parent = limbParent;
        //Limb.transform.localPosition = Vector3.zero;
        //Limb.transform.localEulerAngles = Vector3.zero;
        //Limb.transform.localScale = Vector3.one;
        detached = false;

        NetworkServer.Destroy(sceneObject);
    }



    /*
     *  Destroy(Limb.GetComponent<Rigidbody>());
        Limb.transform.parent = limbParent;
        Limb.transform.localPosition = Vector3.zero;
        Limb.transform.localEulerAngles = Vector3.zero;
        Limb.transform.localScale = Vector3.one;
        detached = false;
     * 
     * 
     * Limb.AddComponent<Rigidbody>();
        Limb.transform.parent = detachedList;
        detached = true;
     * 
     * 
     * 
     * 
     */
}
