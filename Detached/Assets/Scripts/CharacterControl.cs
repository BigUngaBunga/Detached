using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CharacterControl : NetworkBehaviour
{
    [Header("General")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float crouchHeight;
    [SerializeField] private bool active;
    CapsuleCollider playerCol;
    float originHeight;
    public GameObject PlayerModel;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    float walkSpeed;
    [SerializeField] private Transform orientation;

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

    private bool isGrounded = false;

    Vector3 moveDir;

    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        PlayerModel.SetActive(false);
        rb = GetComponentInChildren<Rigidbody>();
        playerCol = GetComponentInChildren<CapsuleCollider>();
        originHeight = playerCol.height;
        ResetJump();
        walkSpeed = movementSpeed;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MovementScene")
        {
            if (PlayerModel.activeSelf == false)
            {
                PlayerModel.SetActive(true);
            }
        }

        if (hasAuthority)
        {
            if (active)
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
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

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
        if (isGrounded && Input.GetButton("Jump") && readyToJump)
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
