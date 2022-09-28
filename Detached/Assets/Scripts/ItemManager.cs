using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemManager : NetworkBehaviour
{

    [Header("Keybindings")]
    public KeyCode detachKey;
    
    public GameObject Limb;
    

    [SyncVar(hook = nameof(OnChangeDetached))]
    public bool detached;

    public Transform limbParent;
    public Transform detachedList;
    public Transform body;
    

    private void OnChangeDetached(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            Destroy(Limb.GetComponent<Rigidbody>());
            Limb.transform.parent = limbParent;
            Limb.transform.localPosition = Vector3.zero;
            Limb.transform.localEulerAngles = Vector3.zero;
            Limb.transform.localScale = Vector3.one;
        }
        else
        {
            Limb.AddComponent<Rigidbody>();
            Limb.transform.parent = detachedList;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(detachKey) && detached == false)
            CmdChangeAttachmentLimb(!detached);
        else if (Input.GetKeyDown(detachKey) && detached == true)
            CmdChangeAttachmentLimb(!detached);
    }
    [Command]
    void CmdChangeAttachmentLimb(bool value)
    {
        detached = value;
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
