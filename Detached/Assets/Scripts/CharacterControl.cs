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

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollow;
    [SerializeField] private CinemachineFreeLook cinemaFreelook;


    private bool isGrounded = false;
    

    Vector3 moveDir;

    float horizontalInput;
    float verticalInput;

    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        playerCol = GetComponent<CapsuleCollider>();
        originHeight = playerCol.height;
        ResetJump();
        walkSpeed = movementSpeed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        playerObjectController = GetComponent<PlayerObjectController>();

        //playerBody.SetActive(false); //So the body dosen't load in the steamlobby scene

    }

    private void Update()
    {

        //if (Input.GetMouseButtonDown(0))
        //{
        //    controllingPlayer = !controllingPlayer;
        //    CMFreeLook.SetActive(controllingPlayer);
        //}
            


        if (!isLocalPlayer) return;

        if (SceneManager.GetActiveScene().name == "Game")


        {
            if (rb.useGravity == false)
            {                
                
                camTransform = Camera.main.transform;
                rb.useGravity = true;
                cinemaFreelook = CinemachineFreeLook.FindObjectOfType<CinemachineFreeLook>();
                cinemaFreelook.LookAt = cameraFollow.transform;
                cinemaFreelook.Follow = cameraFollow.transform;
            }

            if (isBeingControlled)
            {
                GroundCheck();
                MyInput();
                Movement();
                Jump();
                Sprint();
                Crouch();

                SpeedControl();

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
        if (isGrounded && Input.GetButton("Jump") && readyToJump && DetachScript.numOfLegs>1)
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
