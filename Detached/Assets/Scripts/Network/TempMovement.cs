using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TempMovement : NetworkBehaviour
{
    float movementSpeed = 1f;
    SceneObjectItemManager sceneObjectItemManagerScript;

    private void Start()
    {
        sceneObjectItemManagerScript = gameObject.GetComponent<SceneObjectItemManager>();
    }

    void Update()
    {
        if (!hasAuthority && !isClient) return;

        if (!sceneObjectItemManagerScript.IsBeingControlled) return;

        Vector3 move = new Vector3();


        if (Input.GetKeyDown(KeyCode.W))
        {
            move += new Vector3(0, 0, movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            move += new Vector3(0, 0, -movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            move += new Vector3(-movementSpeed, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            move += new Vector3(movementSpeed, 0, 0);
        }

        CmdMoveObject(move);
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
