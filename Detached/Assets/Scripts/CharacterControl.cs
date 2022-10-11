using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CharacterControl : NetworkBehaviour
{
   
    //TEMPORARY
    [Header("Temporary")]
    [SerializeField] private bool controllingPlayer = true;
    [SerializeField] private GameObject CMFreeLook;
    //TEMPORARY

    [Header("General")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float crouchHeight;
    [SerializeField] public bool isBeingControlled = true;
    CapsuleCollider playerCol;
    float originHeight;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    float walkSpeed;
    [SerializeField] private Transform camTransform;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCD;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float groundDrag;
    bool readyToJump;


    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;

    [Header("NetWorking")]
    [SerializeField] private GameObject playerBody;
    [SerializeField] private PlayerObjectController playerObjectController;
    [SerializeField] ItemManager limbManager;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollow;
    [SerializeField] private CinemachineFreeLook cinemaFreelook;


    private bool isGrounded = false;
    

    Vector3 moveDir;

    float horizontalInput;
    float verticalInput;


    //[Header("PickUp")]
    //[SerializeField] public Transform dest;
    //[SerializeField] public Camera camera;
    // private Transform dropDest;
    // bool holding = false;
    // private Transform objectHit;
    // private string nameObject;
    // private bool hitObject;
    // private GameObject heldItem;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        playerCol = GetComponent<CapsuleCollider>();
        originHeight = playerCol.height;
        ResetJump();
        walkSpeed = movementSpeed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        playerObjectController = GetComponent<PlayerObjectController>();


        if (!isLocalPlayer) return;
        camTransform = Camera.main.transform;

        cinemaFreelook = CinemachineFreeLook.FindObjectOfType<CinemachineFreeLook>();
        cinemaFreelook.LookAt = cameraFollow.transform;
        cinemaFreelook.Follow = cameraFollow.transform;

        //DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    controllingPlayer = !controllingPlayer;
        //    CMFreeLook.SetActive(controllingPlayer);
        //}

        if (!isLocalPlayer) return;
       
        if (SceneManager.GetActiveScene().buildIndex > 1 && controllingPlayer)
        {
               
            if (isBeingControlled) //If player is being actively controlled as oppose to a limb
            {
                GroundCheck();
                MyInput();
                Movement();
                Jump();
                Sprint();
                Crouch();
                //PickUp();

                SpeedControl();
                gameObject.transform.rotation = Quaternion.AngleAxis(camTransform.rotation.eulerAngles.y, Vector3.up);
                //Debug.Log(movementSpeed);

                if (isGrounded)
                    rb.drag = groundDrag;
                else
                    rb.drag = 0;
            }


        }
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void PickUp()
    {

        //    RaycastHit hit;
        //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        //    hitObject = false;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        hitObject = true;
        //        objectHit = hit.transform;
        //        nameObject = hit.transform.gameObject.name;
        //    }


        //    if (hitObject)
        //    {
        //        if (objectHit.transform.gameObject.tag == "Box" || objectHit.transform.gameObject.tag == "Battery" || objectHit.transform.gameObject.tag == "Key")
        //        {
        //            if (Input.GetKeyDown("e") && !holding)
        //            {
        //                heldItem = GameObject.Find(objectHit.transform.gameObject.name);
        //                heldItem.transform.parent = dest.transform;
        //                heldItem.GetComponent<Rigidbody>().useGravity = false;
        //            }
        //        }
        //    }


        //    if (holding)
        //    {
        //        heldItem.transform.position = dest.position;
        //        heldItem.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //        heldItem.transform.eulerAngles = new Vector3(0, 0, 0);
        //        heldItem.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);


        //    }
        //    if (Input.GetKeyDown("e"))
        //    {

        //        if (!holding && hitObject)
        //        {
        //            if (objectHit.transform.gameObject.tag == "Box" || objectHit.transform.gameObject.tag == "Battery" || objectHit.transform.gameObject.tag == "Key")
        //            {
        //                heldItem = GameObject.Find(objectHit.transform.gameObject.name);
        //                heldItem.transform.parent = dest.transform;
        //                heldItem.GetComponent<Rigidbody>().useGravity = false;
        //                holding = true;
        //            }

        //        }
        //        else if (holding)
        //        {

        //            //if(TooFarGone(objectHit) == false)
        //            //{
        //            if (heldItem.transform.gameObject.tag == "Battery")
        //            {
        //                if (objectHit.transform.gameObject.tag == "BatteryBox")
        //                {
        //                    dropDest = GameObject.Find(objectHit.transform.gameObject.name + "/SlotDestination").transform;

        //                }
        //                else
        //                {
        //                    dropDest = dest.transform;
        //                }
        //            }
        //            else if (heldItem.transform.gameObject.tag == "Key")
        //            {
        //                if (objectHit.transform.gameObject.tag == "Lock")
        //                {
        //                    dropDest = GameObject.Find(objectHit.transform.gameObject.name + "/KeyDestination").transform;
        //                }
        //                else
        //                {
        //                    dropDest = dest.transform;
        //                }
        //            }
        //            //}

        //            if (heldItem.transform.gameObject.tag == "Box")
        //            {
        //                dropDest = dest.transform;

        //            }

        //            heldItem.transform.position = dropDest.position;
        //            heldItem.transform.parent = null;
        //            heldItem.GetComponent<Rigidbody>().useGravity = true;
        //            heldItem = null;
        //            holding = false;
        //        }
        //}
    }




bool TooFarGone(Transform oh)
{
    int size = 10;
    if (Mathf.Abs(this.transform.position.x - oh.position.x) > size || Mathf.Abs(this.transform.position.y - oh.position.y) > size || Mathf.Abs(this.transform.position.z - oh.position.z) > size)
    {
        return true;
    }

    return false;
}

void Movement()
    {

        moveDir = new Vector3(horizontalInput, 0, verticalInput);
        moveDir = Quaternion.AngleAxis(camTransform.rotation.eulerAngles.y, Vector3.up) * moveDir;
       
        if (isGrounded)
            rb.AddForce(moveDir.normalized * movementSpeed * 10f, ForceMode.Force);
        else if (!isGrounded)
            rb.AddForce(moveDir.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);

        //transform.position += moveDir * movementSpeed * Time.deltaTime;
    }


    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundMask);
    }

    private void Jump()
    {
        if (isGrounded && Input.GetButton("Jump") && readyToJump && limbManager.HasBothLegs())
        {
            readyToJump = false;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCD); //Hold jump
        }
    }

    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = runSpeed;
        }
        else
            movementSpeed = walkSpeed;
    }

    void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            playerCol.height = crouchHeight;
            movementSpeed = crouchSpeed;
        }
        else
        {
            playerCol.height = originHeight;
        }
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(groundCheckTransform.position, groundCheckRadius);
    }
}
