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

    [Header("Step up")]
    /*[SerializeField] GameObject stepRayUpperFront;
    [SerializeField] GameObject stepRayLowerFront;
    [SerializeField] GameObject stepRayUpperBack;
    [SerializeField] GameObject stepRayLowerBack;
    [SerializeField] GameObject stepRayLowerBack;
    [SerializeField] GameObject stepRayLowerBack;
    [SerializeField] GameObject stepRayLowerBack;
    [SerializeField] GameObject stepRayLowerBack;*/
    [SerializeField] GameObject[] stepRays;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;


    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCD;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float jumpForceReduction;
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
    Vector3 input;

    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCol = GetComponent<CapsuleCollider>();
        limbManager = GetComponent<ItemManager>();
        originHeight = playerCol.height;
        ResetJump();
        walkSpeed = movementSpeed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        playerObjectController = GetComponent<PlayerObjectController>();


        if (!isLocalPlayer) return;
        camTransform = Camera.main.transform;

        cinemaFreelook = FindObjectOfType<CinemachineFreeLook>();
        SetCameraFocusPlayer();

        //DontDestroyOnLoad(this.gameObject);
    }
    private void Awake()
    {
        for (int i = 3; i < 6; i++)
        {

            stepRays[i].transform.localPosition = new Vector3(stepRays[i].transform.localPosition.x, stepHeight, stepRays[i].transform.localPosition.z); //(upper rays position)
        }
    }

    private void Update()
    {
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

                #region stepClimbs
                StepClimb(stepRays[0], stepRays[1], stepRays[2]);

                #endregion

                SpeedControl();
                gameObject.transform.rotation = Quaternion.AngleAxis(camTransform.rotation.eulerAngles.y, Vector3.up);
                //Debug.Log(movementSpeed);

                if (isGrounded)
                    rb.drag = groundDrag;
                else
                    rb.drag = airDrag;
            }


        }
    }


    #region Camera focus
    public void SetCameraFocusPlayer() => SetCameraFocus(cameraFollow.transform);

    public void SetCameraFocus(Transform transform)
    {
        cinemaFreelook.LookAt = transform;
        cinemaFreelook.Follow = transform;
    }

    #endregion

    #region Input
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        verticalInput = Input.GetAxisRaw("Vertical") * Time.deltaTime;
    }

    public void TogglePlayerControl() => isBeingControlled = !isBeingControlled;

    #endregion


    #region movement

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Movement()
    {

        input = new Vector3(horizontalInput, 0, verticalInput);
        moveDir = Quaternion.AngleAxis(camTransform.rotation.eulerAngles.y, Vector3.up) * input;


        if (isGrounded)
            rb.AddForce(moveDir.normalized * movementSpeed * 10f * Time.deltaTime, ForceMode.Force);
        else if (!isGrounded)
            rb.AddForce(moveDir.normalized * movementSpeed * 10f * airMultiplier * Time.deltaTime, ForceMode.Force);

        /*        Debug.DrawRay(transform.position, transform.TransformDirection(moveDir.normalized), Color.green);*/
        //transform.position += moveDir * movementSpeed * Time.deltaTime;
    }

    void StepClimb(GameObject rayDirectioLowerMid, GameObject rayDirectioLowerLeft, GameObject rayDirectioLowerRight) //lower check
    {

        RaycastHit hitLower;

        Vector3 rbDirection = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (Physics.Raycast(rayDirectioLowerMid.transform.position, rbDirection.normalized, out hitLower, 0.4f))
        {
            Debug.Log("mid");
            StepClimbUpperCheck(rbDirection, stepRays[3]);

        }
        else if (Physics.Raycast(rayDirectioLowerLeft.transform.position, rbDirection.normalized, out hitLower, 0.4f))
        {
            Debug.Log("Left");
            StepClimbUpperCheck(rbDirection, stepRays[4]);
        }
        else if (Physics.Raycast(rayDirectioLowerRight.transform.position, rbDirection.normalized, out hitLower, 0.4f))
        {
            Debug.Log("Right");
            StepClimbUpperCheck(rbDirection, stepRays[5]);
        }

        Debug.DrawRay(rayDirectioLowerMid.transform.position, rbDirection.normalized, Color.green);
        Debug.DrawRay(rayDirectioLowerLeft.transform.position, rbDirection.normalized, Color.red);
        Debug.DrawRay(rayDirectioLowerRight.transform.position, rbDirection.normalized, Color.blue);

        //Debug.DrawRay(stepRays[3].transform.position, rbDirection.normalized, Color.green);
        //Debug.DrawRay(stepRays[4].transform.position, rbDirection.normalized, Color.red);
        //Debug.DrawRay(stepRays[5].transform.position, rbDirection.normalized, Color.blue);

    }

    private void StepClimbUpperCheck(Vector3 rbDirection, GameObject rayDirectionUpper)
    {
        RaycastHit hitUpper;
        if (!Physics.Raycast(rayDirectionUpper.transform.position, rbDirection.normalized, out hitUpper, 0.4f)) //upper check
        {
            if (input != Vector3.zero)
                rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f); //the actual stepClimb
        }
    }



    private void GroundCheck() => isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundMask);

    private void Jump()
    {
        if (isGrounded && Input.GetButton("Jump") && readyToJump && limbManager.HasBothLegs())
        {
            readyToJump = false;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity *= jumpForceReduction;

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCD); //Hold jump
        }
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = runSpeed;
        }
        else
            movementSpeed = walkSpeed;
    }

    private void Crouch()
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

    private void ResetJump()
    {
        readyToJump = true;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(groundCheckTransform.position, groundCheckRadius);
    }
}
