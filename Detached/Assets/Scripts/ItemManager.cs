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

    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(detachKey) && detached == false)
            CmdDetachLimb();
        else if (Input.GetKeyDown(detachKey) && detached == true)
            CmdAttachLimb();
    }

    [Command]
    void CmdAttachLimb()
    {
        Destroy(Limb.GetComponent<Rigidbody>());
        Limb.transform.parent = limbParent;
        Limb.transform.localPosition = Vector3.zero;
        Limb.transform.localEulerAngles = Vector3.zero;
        Limb.transform.localScale = Vector3.one;
        detached = false;
    }

    [Command]
    void CmdDetachLimb()
    {
        Limb.AddComponent<Rigidbody>();
        Limb.transform.parent = detachedList;
        detached = true;
    }
}
