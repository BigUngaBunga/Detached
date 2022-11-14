using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEngine.Events;

public class SceneObjectItemManager : NetworkBehaviour
{
    [SerializeField] private GameObject headLimb;
    [SerializeField] private GameObject armLimb;
    [SerializeField] private GameObject legLimb;

    //Ground Checks
    private Vector3 safeLocation;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] float groundCheckRadius;


    private KeyCode detachKeyHead;
    private KeyCode detachKeyArm;
    private KeyCode detachKeyLeg;

    private HighlightObject highlight;

    [SyncVar(hook = nameof(OnChangeDetached))]
    public bool detached = false;
    [SyncVar]
    public ItemManager.Limb_enum thisLimb;

    [SyncVar]
    public bool isBeingControlled = false;

    public UnityEvent pickUpLimbEvent = new UnityEvent();

    public GameObject orignalOwner;

    
    public bool IsBeingControlled
    {
        get { return isBeingControlled; }
        set { 
            SetControlledStatus(value);
            if (value)
                highlight.ForceHighlight();
            else
                highlight.ForceStopHighlight();
        }
    }

    [Command(requiresAuthority = false)]
    private void SetControlledStatus(bool value) => isBeingControlled = value;


    public bool test = true;

    public ItemManager itemManager;
    

    private void Start()
    {
        itemManager = NetworkClient.localPlayer.GetComponent<ItemManager>();
        highlight = GetComponent<HighlightObject>();
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
        if (Input.GetKeyDown(KeyCode.T) && !IsBeingControlled)
        {
            var itemManager = NetworkClient.localPlayer.GetComponent<ItemManager>();
            switch (thisLimb)
            {
                case ItemManager.Limb_enum.Arm:
                    if (itemManager.rightArmDetached || itemManager.leftArmDetached)
                    {
                        itemManager.CmdPickUpLimb(gameObject);
                    }
                    break;
                case ItemManager.Limb_enum.Leg:
                    if (itemManager.rightLegDetached || itemManager.leftLegDetached)
                    {
                        itemManager.CmdPickUpLimb(gameObject);
                    }
                    break;
            }
        }
    }

    public void TryPickUp()
    {
        Debug.Log("Attempting pickup");
        var itemManager = orignalOwner.GetComponent<ItemManager>();
        if (!IsBeingControlled && itemManager.CheckIfMissingLimb(thisLimb))
        {
            Debug.Log("Picking it up");
            itemManager.CmdPickUpLimb(gameObject);
        }      
    }

    public void HandleFallOutOfWorld()
    {
        if (safeLocation != null && safeLocation != Vector3.zero)
        {
            if (isServer)
            {
                RpcUpdatePosition(safeLocation);
            }            
        }
        else
        {
            itemManager.CmdPickUpLimb(gameObject);
            itemManager.ReturnControllToPlayer();
        }
    }

    [ClientRpc]
    public void RpcUpdatePosition(Vector3 safeLocation)
    {
        //So it dosne't collide with ground or other limbs
        gameObject.transform.position = safeLocation + new Vector3(0,2,0);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (Physics.CheckSphere(transform.position, groundCheckRadius, groundMask))
            {
                safeLocation = transform.position;
            }
        }
        
    }

    private void OnDestroy()
    {
        pickUpLimbEvent.Invoke();
    }
}
