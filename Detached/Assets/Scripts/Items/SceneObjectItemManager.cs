using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SceneObjectItemManager : NetworkBehaviour
{
    void OnMouseDown()
    {       
        NetworkClient.localPlayer.GetComponent<ItemManager>().pickUpLimb(gameObject);
    }

}
