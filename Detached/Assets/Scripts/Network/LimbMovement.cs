using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using LimbType = ItemManager.Limb_enum;

public class LimbMovement : NetworkBehaviour
{
    float movementSpeed = 1f;
    SceneObjectItemManager sceneObjectItemManagerScript;


    [Header("Movement")]
    [SerializeField] float speed = 10;
    [SerializeField] private Transform camTransform;
    Vector3 moveDir;
    Vector3 input;

    float horizontalInput;
    float verticalInput;

    private void Start()
    {
        sceneObjectItemManagerScript = gameObject.GetComponent<SceneObjectItemManager>();
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!hasAuthority && !isClient) return;

        if (!sceneObjectItemManagerScript.IsBeingControlled || sceneObjectItemManagerScript.thisLimb== LimbType.Head) return;

        MyInput();
        Movement();

        CmdMoveObject(moveDir);
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

    [Command]
    private void CmdMoveObject(Vector3 move)
    {
        RpcMoveObject(move);
    }

    [ClientRpc]
    private void RpcMoveObject(Vector3 move)
    {
        gameObject.transform.position += move;
    }
}
