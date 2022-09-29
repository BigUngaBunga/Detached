using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SceneObjectItemManager : NetworkBehaviour
{
    public GameObject limb;
    [SyncVar(hook = nameof(OnChangeDetached))]
    public bool detached = false;
    public ItemManager.Limb_enum thisLimb;

    //Instantiates the limb as a child on the SceneObject 
    private void OnChangeDetached(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            Instantiate(limb, transform.position, transform.rotation, transform);           
        }    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            
        switch (thisLimb)
            {
                case ItemManager.Limb_enum.Head:
                    if (NetworkClient.localPlayer.GetComponent<ItemManager>().headDetached)
                    {
                        NetworkClient.localPlayer.GetComponent<ItemManager>().CmdPickUpLimb(gameObject);
                    }
                    break;
                case ItemManager.Limb_enum.Arm:
                    if (NetworkClient.localPlayer.GetComponent<ItemManager>().rightArmDetached || NetworkClient.localPlayer.GetComponent<ItemManager>().leftArmDetached)
                    {
                        NetworkClient.localPlayer.GetComponent<ItemManager>().CmdPickUpLimb(gameObject);
                    }
                    break;
                case ItemManager.Limb_enum.Leg:
                    break;
            }
    }

}
