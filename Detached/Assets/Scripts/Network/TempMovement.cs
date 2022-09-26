using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TempMovement : NetworkBehaviour
{
    float movementSpeed = 1f;
   
    void Update()
    {
        if (!hasAuthority) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            gameObject.transform.position += new Vector3(0, 0, movementSpeed);
        }
    }
}
