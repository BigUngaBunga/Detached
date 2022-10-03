using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SceneObjectItemManager : NetworkBehaviour
{
    [SerializeField] private GameObject headLimb;
    [SerializeField] private GameObject armLimb;
    [SerializeField] private GameObject legLimb;

    private KeyCode detachKeyHead;
    private KeyCode detachKeyArm;
    private KeyCode detachKeyLeg;

    [SyncVar(hook = nameof(OnChangeDetached))]
    public bool detached = false;
    [SyncVar]
    public ItemManager.Limb_enum thisLimb;

    [SyncVar]
    public bool isBeingControlled = false;

    public bool test = true;

    public ItemManager itemManager;

    private void Start()
    {
        itemManager = NetworkClient.localPlayer.GetComponent<ItemManager>();
        detachKeyHead = itemManager.detachKeyHead;
        detachKeyArm = itemManager.detachKeyArm;
        detachKeyLeg = itemManager.detachKeyLeg;

    }

    //Instantiates the limb as a child on the SceneObject 
    private void OnChangeDetached(bool oldValue, bool newValue)
    {
        if (newValue) // if Detached == true
        {
            switch (thisLimb)
            {
                case ItemManager.Limb_enum.Head:
                    Instantiate(headLimb, transform.position, transform.rotation, transform);
                    break;
                case ItemManager.Limb_enum.Arm:
                    Instantiate(armLimb, transform.position, transform.rotation, transform);
                    break;
                case ItemManager.Limb_enum.Leg:
                    Instantiate(legLimb, transform.position, transform.rotation, transform);
                    break;
            }
        }
    }

    void Update()
    {       
        if (thisLimb == ItemManager.Limb_enum.Head && hasAuthority)
        {
            if (hasAuthority && Input.GetKeyDown(detachKeyHead))
            {
                NetworkClient.localPlayer.GetComponent<ItemManager>().CmdPickUpLimb(gameObject);
            }
        }

        //Todo Needs to be changed to a more specific pickup action
        if (Input.GetKeyDown(KeyCode.T) && !isBeingControlled)
        {
            switch (thisLimb)
            {
                case ItemManager.Limb_enum.Arm:
                    if (NetworkClient.localPlayer.GetComponent<ItemManager>().rightArmDetached || NetworkClient.localPlayer.GetComponent<ItemManager>().leftArmDetached)
                    {
                        NetworkClient.localPlayer.GetComponent<ItemManager>().CmdPickUpLimb(gameObject);
                    }
                    break;
                case ItemManager.Limb_enum.Leg:
                    if (NetworkClient.localPlayer.GetComponent<ItemManager>().rightLegDetached || NetworkClient.localPlayer.GetComponent<ItemManager>().leftLegDetached)
                    {
                        NetworkClient.localPlayer.GetComponent<ItemManager>().CmdPickUpLimb(gameObject);
                    }
                    break;
            }
        }
    }
}
