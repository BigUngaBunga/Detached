using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SceneObjectItemManager : NetworkBehaviour
{
    public GameObject limb;
    [SyncVar(hook = nameof(OnChangeDetached))]
    public bool detached = false;

    private void OnChangeDetached(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            Instantiate(limb, transform.position, transform.rotation, transform);           
        }    
    }

    void Update()
    {     
        
        if(Input.GetKeyDown(KeyCode.T))
        NetworkClient.localPlayer.GetComponent<ItemManager>().CmdPickUpLimb(gameObject);
    }

}
