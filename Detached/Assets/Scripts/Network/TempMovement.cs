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

        if (!sceneObjectItemManagerScript.IsBeingControlled) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            gameObject.transform.position += new Vector3(0, 0, movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            gameObject.transform.position += new Vector3(0, 0, -movementSpeed);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            gameObject.transform.position += new Vector3(-movementSpeed, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            gameObject.transform.position += new Vector3(movementSpeed, 0, 0);
        }
    }

    
}
