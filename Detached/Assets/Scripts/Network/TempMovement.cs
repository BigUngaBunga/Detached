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
        if (!hasAuthority) return;

        if (!sceneObjectItemManagerScript.isBeingControlled) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            CmdMoveForward();
        }
    }

    [Command]
    void CmdMoveForward()
    {
        RpcMoveForward();
    }

    [ClientRpc]
    void RpcMoveForward()
    {
        gameObject.transform.position += new Vector3(0, 0, movementSpeed);
    }
}
