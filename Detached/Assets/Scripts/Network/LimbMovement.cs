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
    Vector3 input;
    public Rigidbody rb;
    float horizontalInput;
    float verticalInput;

    [Header("Step up")]
    [SerializeField] GameObject[] stepRays;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;
    public float stepRayLength = 1f;

    private void Start()
    {

        sceneObjectItemManagerScript = gameObject.GetComponent<SceneObjectItemManager>();
        initialRotationY = GetInitialRotation(sceneObjectItemManagerScript.thisLimb);
        camTransform = Camera.main.transform;
        //rb = gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
 /*       stepRays[0] = GameObject.Find("StepRayLowerFrontMid");
        stepRays[1] = GameObject.Find("StepRayLowerFrontLeft");
        stepRays[2] = GameObject.Find("StepRayLowerFrontRight");
        stepRays[4] = GameObject.Find("StepRayUpperFrontLeft");
        stepRays[5] = GameObject.Find("StepRayUpperFrontLeft");*/
    }

    private void Awake()
    {
      /*  for (int i = 3; i < 6; i++)
        {

            stepRays[i].transform.localPosition = new Vector3(stepRays[i].transform.localPosition.x, stepHeight, stepRays[i].transform.localPosition.z); //(upper rays position)
        }*/
    }

    void Update()
    {
        if (!hasAuthority && !isClient) return;


        if (!sceneObjectItemManagerScript.IsBeingControlled) return;

        if (sceneObjectItemManagerScript.thisLimb != LimbType.Head)
        {

            MyInput();
            Movement();
            #region stepClimbs
           // StepClimb(stepRays[0], stepRays[1], stepRays[2]);

            #endregion

            //CmdMoveObject(input);
            //rb.AddForce(moveDir.normalized * movementSpeed * 10f * Time.deltaTime, ForceMode.Force);
            SpeedControl();
            if (moveDir.normalized != Vector3.zero)
                rb.AddForce(moveDir.normalized * speed * Time.deltaTime, ForceMode.Force);


        }

        Debug.Log(rb.velocity);

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
            if (input != Vector3.zero)
                rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f); //the actual stepClimb
        }
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
