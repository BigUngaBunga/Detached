using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 0.05f;



    void Update()
    {

        Vector3 move = new Vector3(Input.GetAxis("Horizontal") * movementSpeed, 0, Input.GetAxis("Vertical") * movementSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.position += move;
        }
    }
}
