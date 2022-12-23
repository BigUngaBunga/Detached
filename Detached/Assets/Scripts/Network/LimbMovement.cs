using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using LimbType = ItemManager.Limb_enum;
using Mirror.Experimental;

public class LimbMovement : NetworkBehaviour
{
    private float movementSpeed = 1f;
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
 /*       stepRays[0] = GameObject.Find("StepRayLowerFrontMid");
        stepRays[1] = GameObject.Find("StepRayLowerFrontLeft");
        stepRays[2] = GameObject.Find("StepRayLowerFrontRight");
        stepRays[4] = GameObject.Find("StepRayUpperFrontLeft");
        stepRays[5] = GameObject.Find("StepRayUpperFrontLeft");*/
    }

  

    void Update()
    {
        if (!hasAuthority) return;


        if (!sceneObjectItemManagerScript.IsBeingControlled) return;

        if (sceneObjectItemManagerScript.thisLimb != LimbType.Head)
        {

            MyInput();
            Movement();
            #region stepClimbs
            // 
            if(limbStepUp == null) limbStepUp = GetComponentInChildren<LimbStepUpRay>();
            limbStepUp.ActiveStepClimb(input, rb);
            #endregion

            //CmdMoveObject(input);
            //rb.AddForce(moveDir.normalized * movementSpeed * 10f * Time.deltaTime, ForceMode.Force);
            SpeedControl();
            if (moveDir.normalized != Vector3.zero)
                rb.AddForce(moveDir.normalized * speed * Time.deltaTime, ForceMode.Force);


        }

        gameObject.transform.rotation = Quaternion.AngleAxis(camTransform.rotation.eulerAngles.y + initialRotationY, Vector3.up);
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

    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
        verticalInput = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;
    }

   

    private float GetInitialRotation(LimbType type)
    {
        return type switch
        {
            LimbType.Arm => 90,
            _ => 0,
        };
    }

    [Command]
    private void CmdMoveObject(Vector3 move)
    {
        RpcMoveObject(move);
    }

    [ClientRpc]
    private void RpcMoveObject(Vector3 move)
    {
        //rb.AddForce(move.normalized * movementSpeed * 10f * Time.deltaTime, ForceMode.Force);
        gameObject.transform.position += move;
        //rb.AddForce(move.normalized * speed * Time.deltaTime, ForceMode.Force);
    }
}
