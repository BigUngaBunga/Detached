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
        if (newValue)
        {       
            
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(detachKey) && detached == false)
            CmdChangeAttachmentLimb(!detached);
        
    }
    [Command]
    void CmdChangeAttachmentLimb(bool value)
    {
        detached = true;
        Limb.AddComponent<Rigidbody>();
        GameObject newSceneObject = Instantiate(sceneObject, Limb.transform.position, Limb.transform.rotation);
        NetworkServer.Spawn(newSceneObject);
        Limb.transform.parent = newSceneObject.transform;
    }

    [Command]
    public void pickUpLimb(GameObject sceneObject)
    {
        Destroy(Limb.GetComponent<Rigidbody>());
        Limb.transform.parent = limbParent;
        Limb.transform.localPosition = Vector3.zero;
        Limb.transform.localEulerAngles = Vector3.zero;
        Limb.transform.localScale = Vector3.one;
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
