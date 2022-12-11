using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using Cinemachine;
using FMODUnity;
using FMOD.Studio;
using System;

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
    [SerializeField] GameObject[] stepRays;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;
    public float stepRayLength = 1f;


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
    [SerializeField] private Transform secondaryGroundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;

    [Header("NetWorking")]
    [SerializeField] private GameObject playerBody;
    [SerializeField] private PlayerObjectController playerObjectController;
    [SerializeField] ItemManager limbManager;

    [Header("Camera")]
    [SerializeField] private GameObject cameraFollow;
    [SerializeField] private CinemachineFreeLook cinemaFreelook;
    [SerializeField] private Transform camFocus;
    [SerializeField] public Vector3 noLegCamOffset;
    bool camUpdated;

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
        originHeight = 0;
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
               // Crouch();

                #region stepClimbs
                StepClimb(stepRays[0], stepRays[1], stepRays[2]);

                #endregion
                NoLegCam();
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

    private void NoLegCam()
    {
        if (limbManager.NumberOfLegs >= 1)
        {
            camFocus.localPosition = Vector3.zero;
            camUpdated = false;
            return;
        }

        if (limbManager.NumberOfLegs <= 0 && !camUpdated)
        {
            camFocus.localPosition = new Vector3(camFocus.localPosition.x + noLegCamOffset.x, camFocus.localPosition.y + noLegCamOffset.y, camFocus.localPosition.z + noLegCamOffset.z);
            camUpdated = true;
        }


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
        if (limbManager.NumberOfArms <= 0 && limbManager.NumberOfLegs <= 0)
            return;
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
        // Debug.DrawRay(rayDirectioLowerMid.transform.position, rbDirection.normalized, Color.green);
        Debug.DrawRay(rayDirectioLowerLeft.transform.position, rbDirection.normalized * stepRayLength, Color.red);
        Debug.DrawRay(rayDirectioLowerRight.transform.position, rbDirection.normalized * stepRayLength, Color.blue);
        /* if (Physics.Raycast(rayDirectioLowerMid.transform.position, rbDirection.normalized, out hitLower, rayLengthMid))
         {
             Debug.Log("mid");
             StepClimbUpperCheck(rbDirection, stepRays[3]);

         }*/
        if (Physics.Raycast(rayDirectioLowerLeft.transform.position, rbDirection.normalized, out hitLower, stepRayLength))
        {
            Debug.Log("Left");
            if (hitLower.collider.CompareTag("Leg") || hitLower.collider.CompareTag("Box"))
                return;
            StepClimbUpperCheck(rbDirection, stepRays[4]);
            return;
        }

        if (Physics.Raycast(rayDirectioLowerRight.transform.position, rbDirection.normalized, out hitLower, stepRayLength))
        {
            Debug.Log("Right");
            if (hitLower.collider.CompareTag("Leg") || hitLower.collider.CompareTag("Box"))
                return;
            StepClimbUpperCheck(rbDirection, stepRays[5]);
        }



        //Debug.DrawRay(stepRays[3].transform.position, rbDirection.normalized, Color.green);
        //Debug.DrawRay(stepRays[4].transform.position, rbDirection.normalized, Color.red);
        //Debug.DrawRay(stepRays[5].transform.position, rbDirection.normalized, Color.blue);

    }

    private void StepClimbUpperCheck(Vector3 rbDirection, GameObject rayDirectionUpper)
    {
        RaycastHit hitUpper;
        if (!Physics.Raycast(rayDirectionUpper.transform.position, rbDirection.normalized, out hitUpper, stepRayLength)) //upper check
        {
            if (input != Vector3.zero && isGrounded)
                rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f); //the actual stepClimb
        }
    }



    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundMask);
        isGrounded = isGrounded || (secondaryGroundCheck.gameObject.activeSelf && Physics.CheckSphere(secondaryGroundCheck.position, groundCheckRadius, groundMask));
    }

    private void Jump()
    {
        if (isGrounded && Input.GetButton("Jump") && readyToJump && limbManager.HasBothLegs())
        {
            readyToJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity *= jumpForceReduction;

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Transform body = transform.Find("group1");
            SFXManager.PlayOneShotAttached(SFXManager.JumpSound, SFXManager.SFXVolume, body.gameObject);
            Invoke(nameof(ResetJump), jumpCD); //Hold jump
        }
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && limbManager.NumberOfLegs >= 1)
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
        Gizmos.DrawSphere(secondaryGroundCheck.position, groundCheckRadius);
    }
}
