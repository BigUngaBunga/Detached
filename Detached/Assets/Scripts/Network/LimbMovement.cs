using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using LimbType = ItemManager.Limb_enum;
using Mirror.Experimental;

public class LimbMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 3.5f;
    private float initialRotationY;
    private SceneObjectItemManager sceneObjectItemManagerScript;


    [Header("Movement")]
    [SerializeField] float speed = 10;
    [SerializeField] private Transform camTransform;
    Vector3 moveDir;
    [HideInInspector]
    public Vector3 input;
    public Rigidbody rb;
    float horizontalInput;
    float verticalInput;
    float rotationSpeed = 10f;

    bool hasUpdated = true;

    LimbStepUpRay limbStepUp;

    private void Start()
    {

        sceneObjectItemManagerScript = gameObject.GetComponent<SceneObjectItemManager>();
        initialRotationY = GetInitialRotation(sceneObjectItemManagerScript.thisLimb);
        camTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        limbStepUp = GetComponentInChildren<LimbStepUpRay>();
        if (isClientOnly)
        {
            hasUpdated = false;
            if (limbStepUp != null)
                limbStepUp.IncreasePlayerTwoRay();
        }
    }

    void FixedUpdate()
    {
        if (!hasAuthority || !sceneObjectItemManagerScript.IsBeingControlled) return;
        if (!hasUpdated && limbStepUp != null)
        {
            limbStepUp.IncreasePlayerTwoRay();
            hasUpdated = true;
        }

        if (sceneObjectItemManagerScript.thisLimb != LimbType.Head)
        {

            MyInput();
            Movement();

            if (moveDir != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(moveDir, Vector3.up);
                rotation *= Quaternion.AngleAxis(initialRotationY, Vector3.up);
                RotateLimb(Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime));
            }

            if (limbStepUp == null) limbStepUp = GetComponentInChildren<LimbStepUpRay>();
            Vector3 stepUpSize = limbStepUp.GetStepClimb(input, rb);
            if (stepUpSize != Vector3.zero)
            {
                Debug.Log("Stepup size: " + stepUpSize);
                rb.AddForce(stepUpSize * movementSpeed * Time.deltaTime, ForceMode.Impulse);
            }
            SpeedControl();         
        }
    }

    [Command]
    private void RotateLimb(Quaternion angle)
    {
        rb.MoveRotation(angle);
    }

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

        Vector3 force = moveDir.normalized * movementSpeed * 10f * Time.deltaTime;
        rb.AddForce(force, ForceMode.Force);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }



    private float GetInitialRotation(LimbType type)
    {
        return type switch
        {
            LimbType.Arm => 90,
            _ => 0,
        };
    }
}
