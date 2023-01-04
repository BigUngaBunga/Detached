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

    LimbStepUpRay limbStepUp;

    private void Start()
    {

        sceneObjectItemManagerScript = gameObject.GetComponent<SceneObjectItemManager>();
        initialRotationY = GetInitialRotation(sceneObjectItemManagerScript.thisLimb);
        camTransform = Camera.main.transform;
        //rb = gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        limbStepUp = GetComponentInChildren<LimbStepUpRay>();
    }


    private void Update()
    {
        if (!hasAuthority || !sceneObjectItemManagerScript.IsBeingControlled) return;
        //Vector3 newRotation = new Vector3(0, camTransform.rotation.eulerAngles.y + initialRotationY, 0);
        
    }

    void FixedUpdate()
    {
        if (!hasAuthority || !sceneObjectItemManagerScript.IsBeingControlled) return;

        if (sceneObjectItemManagerScript.thisLimb != LimbType.Head)
        {

            MyInput();
            Movement();
            #region stepClimbs
            
            if (limbStepUp == null) limbStepUp = GetComponentInChildren<LimbStepUpRay>();
            limbStepUp.ActivateStepClimb(input, rb);

            #endregion
            SpeedControl();

            float angle = (camTransform.rotation.eulerAngles.y + initialRotationY);// % 360f;
            RotateLimb(angle);
            
            //rb.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            //CmdMoveObject(input);
            //rb.AddForce(moveDir.normalized * movementSpeed * 10f * Time.deltaTime, ForceMode.Force);
            //if (moveDir.normalized != Vector3.zero)
            //    rb.AddForce(moveDir.normalized * speed * Time.deltaTime, ForceMode.Force);
        }
    }

    [Command]
    private void RotateLimb(float angle)
    {
        rb.MoveRotation(Quaternion.AngleAxis(angle, Vector3.up));
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
