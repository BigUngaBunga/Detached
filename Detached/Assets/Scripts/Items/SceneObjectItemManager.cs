using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SceneObjectItemManager : NetworkBehaviour
{
    void Update()
    {     
        
        if(Input.GetKeyDown(KeyCode.T))
        NetworkClient.localPlayer.GetComponent<ItemManager>().CmdPickUpLimb(gameObject);
    }

}
