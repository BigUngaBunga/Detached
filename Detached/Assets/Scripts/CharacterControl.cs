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
        originHeight = playerCol.height;
        ResetJump();
        walkSpeed = movementSpeed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        playerObjectController = GetComponent<PlayerObjectController>();

        /*stepRays[0].transform.localPosition = new Vector3(stepRays[0].transform.localPosition.x, stepHeight, stepRays[0].transform.localPosition.z);
        stepRays[2].transform.localPosition = new Vector3(stepRays[2].transform.localPosition.x, stepHeight, stepRays[2].transform.localPosition.z);
        stepRays[2].transform.localPosition = new Vector3(stepRays[2].transform.localPosition.x, stepHeight, stepRays[2].transform.localPosition.z);
        stepRays[2].transform.localPosition = new Vector3(stepRays[2].transform.localPosition.x, stepHeight, stepRays[2].transform.localPosition.z);*/

        for (int i = 0; i < stepRays.Length / 2; i++)
        {

            stepRays[i * 2].transform.localPosition = new Vector3(stepRays[i * 2].transform.localPosition.x, stepHeight, stepRays[i * 2].transform.localPosition.z); //i*2 to get 0,2,4,6 (upper rays position)
        }

        if (!isLocalPlayer) return;
        camTransform = Camera.main.transform;

        cinemaFreelook = FindObjectOfType<CinemachineFreeLook>();
        SetCameraFocusPlayer();

        //DontDestroyOnLoad(this.gameObject);
    }
    private void Awake()
    {

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

                #region stepClimbbs
                stepClimb(stepRays[1], stepRays[0], Vector3.forward);
                stepClimb(stepRays[3], stepRays[2], Vector3.back);
                stepClimb(stepRays[5], stepRays[4], Vector3.left);
                stepClimb(stepRays[7], stepRays[6], Vector3.right);
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

        //transform.position += moveDir * movementSpeed * Time.deltaTime;
    }

    void stepClimb(GameObject rayDirectioLower, GameObject rayDirectionUpper, Vector3 direction)
    {
        RaycastHit hitLower;
        RaycastHit hitLower45;

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(rayDirectioLower.transform.position, transform.TransformDirection(direction), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(rayDirectionUpper.transform.localPosition, transform.TransformDirection(direction), out hitUpper, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }


        /*  else if (Physics.Raycast(rayDirectioLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f))
          {

              RaycastHit hitUpper45;
              if (!Physics.Raycast(rayDirectionUpper.transform.localPosition, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f))
              {
                  rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
              }
          }

          else if (Physics.Raycast(rayDirectioLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
          {

              RaycastHit hitUpperMinus45;
              if (!Physics.Raycast(rayDirectionUpper.transform.localPosition, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
              {
                  rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
              }
          }*/
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
