using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private GameObject mover;


    Vector2 turn;
    Vector3 deltaMove;

    void Update()
    {

        if (Input.GetMouseButton(1))
        {
            turn.x += Input.GetAxis("Mouse X");
            turn.y += Input.GetAxis("Mouse Y");
        }

        mover.transform.localRotation = Quaternion.Euler(0, turn.x, 0);
        transform.localRotation = Quaternion.Euler(-turn.y, 0, 0);

        deltaMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * movementSpeed * Time.deltaTime;
        mover.transform.Translate(deltaMove);

        if (Input.GetKey("space"))
        {
            mover.transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey("left ctrl"))
        {
            mover.transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);
        }

    }
}
