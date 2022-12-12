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

    private void Start()
    {

        sceneObjectItemManagerScript = gameObject.GetComponent<SceneObjectItemManager>();
        initialRotationY = GetInitialRotation(sceneObjectItemManagerScript.thisLimb);
        camTransform = Camera.main.transform;
        //rb = gameObject.AddComponent<Rigidbody>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!hasAuthority && !isClient) return;


        if (!sceneObjectItemManagerScript.IsBeingControlled) return;

        if (sceneObjectItemManagerScript.thisLimb != LimbType.Head)
        {
           
            MyInput();
            Movement();

            CmdMoveObject(moveDir);
            //rb.AddForce(moveDir.normalized * movementSpeed * 10f * Time.deltaTime, ForceMode.Force);

        }

        Debug.Log(rb.velocity);

        gameObject.transform.rotation = Quaternion.AngleAxis(camTransform.rotation.eulerAngles.y + initialRotationY, Vector3.up);
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
