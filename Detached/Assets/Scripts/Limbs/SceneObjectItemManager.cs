using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEngine.Events;
using LimbType = ItemManager.Limb_enum;

public class SceneObjectItemManager : NetworkBehaviour
{
    [SerializeField] private GameObject headLimb;
    [SerializeField] private GameObject armLimb;
    [SerializeField] private GameObject legLimb;
    [SerializeField] private int NumOfTimeLimbCanFallOut;

    //Ground Checks
    [SerializeField] private Vector3 safeLocation;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] float groundCheckRadius;

    private KeyCode detachKeyHead;
    private KeyCode detachKeyArm;
    private KeyCode detachKeyLeg;

    private HighlightObject highlight;
    private ArmInteraction armInteractor;

    [SyncVar(hook = nameof(OnChangeDetached))] public bool detached = false;
    [SyncVar] public LimbType thisLimb;
    [SyncVar] public Vector3 initalPosition;
    [SyncVar] public bool isBeingControlled = false;
    [SyncVar] public GameObject originalOwner;
    [SyncVar] public bool isDeta;
    [SyncVar] private GameObject thisGameObject;
    [SyncVar] public bool isBeingPickedUp = false;

    public UnityEvent pickUpLimbEvent = new UnityEvent();
    private int numOfFallOutOfWorld = 0;

    public bool IsBeingControlled
    {
        get { return isBeingControlled; }
        set
        {
            SetControlledStatus(value);
            if (value)
            {
                highlight.ForceHighlight();
            }
            else
                highlight.ForceStopHighlight();
        }
    }

    [Command(requiresAuthority = false)]
    private void SetControlledStatus(bool value) => isBeingControlled = value;

    public ItemManager itemManager;
    private TextureManager textureManager;

    private void Awake()
    {
        textureManager = GetComponent<TextureManager>();
    }

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
        if (newValue ) // if Detached == true
        {
            GameObject limb = null;
            switch (thisLimb)
            {
                case LimbType.Head:
                    limb = headLimb;
                    break;
                case LimbType.Arm:
                    limb = armLimb;
                    armInteractor = gameObject.AddComponent<ArmInteraction>();
                    break;
                case LimbType.Leg:
                    limb = legLimb;
                    break;
            }
            Instantiate(limb, transform.position, transform.rotation, transform);
            textureManager.UpdateColor();
        }
    }

    void Update()
    {
        if (thisLimb == LimbType.Head && hasAuthority)
        {
            if (hasAuthority && Input.GetKeyDown(detachKeyHead))
            {
                NetworkClient.localPlayer.GetComponent<ItemManager>().CmdPickUpLimb(gameObject);
            }
        }
        if (thisLimb == LimbType.Arm && IsBeingControlled && itemManager != null && itemManager.isLocalPlayer)
            armInteractor.UpdateInteractor(Input.GetKeyDown(KeyCode.E));
        //Todo Needs to be changed to a more specific pickup action
        if (Input.GetKeyDown(KeyCode.T) && !IsBeingControlled)
        {
            var itemManager = NetworkClient.localPlayer.GetComponent<ItemManager>();
            switch (thisLimb)
            {
                case LimbType.Arm:
                    if (itemManager.rightArmDetached || itemManager.leftArmDetached)
                    {
                        itemManager.CmdPickUpLimb(gameObject);
                    }
                    break;
                case LimbType.Leg:
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
        var itemManager = NetworkClient.localPlayer.GetComponent<ItemManager>();
        if (!IsBeingControlled && itemManager.CheckIfMissingLimb(thisLimb) && !isBeingPickedUp)
        {
            Debug.Log("Picking it up");
            isBeingPickedUp = true;
            itemManager.CmdPickUpLimb(gameObject);
        }
    }

    public void TryPickUpWithOrginalOwner()
    {
        Debug.Log("Attempting pickup");
        var itemManager = originalOwner.GetComponent<ItemManager>();
        if (itemManager.CheckIfMissingLimb(thisLimb) && !isBeingPickedUp)
        {
            Debug.Log("Picking it up");
            isBeingPickedUp = true;
            itemManager.CmdPickUpLimb(gameObject);
        }
    }

    public void HandleFallOutOfWorld()
    {

        if (safeLocation != null && safeLocation != Vector3.zero && NumOfTimeLimbCanFallOut >= numOfFallOutOfWorld)
        {
            if (isServer)
            {
                numOfFallOutOfWorld++;
                RpcUpdatePosition(safeLocation);
            }
        }
        else
        {
            if (originalOwner == NetworkClient.localPlayer.gameObject)
            {

                var itemManager = originalOwner.GetComponent<ItemManager>();
                itemManager.ReturnControllToPlayer();                
                TryPickUpWithOrginalOwner();
            }
            else //If it is the client, the client can send a command from a server, therefore we need a targetrpc
            {
                TargetRpcPickUpLimb(originalOwner.GetComponent<NetworkIdentity>().connectionToServer);
            }
        }
    }

    [TargetRpc]
    private void TargetRpcPickUpLimb(NetworkConnection target)
    {
        Debug.Log("Attempting pickup");
        var itemManager = originalOwner.GetComponent<ItemManager>();
        if (itemManager.CheckIfMissingLimb(thisLimb) && !isBeingPickedUp)
        {
            Debug.Log("Picking it up");
            isBeingPickedUp = true;
            itemManager.CmdPickUpLimb(gameObject);
        }
        itemManager.ReturnControllToPlayer();
    }



    [ClientRpc]
    public void RpcUpdatePosition(Vector3 safeLocation)
    {
        gameObject.transform.position = safeLocation + new Vector3(0, 2, 0); //So it dosne't collide with ground or other limbs
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (Physics.CheckSphere(transform.position, groundCheckRadius, groundMask))
        {
            safeLocation = transform.position;
        }
    }

    private void OnDestroy()
    {
        pickUpLimbEvent.Invoke();
    }
}
