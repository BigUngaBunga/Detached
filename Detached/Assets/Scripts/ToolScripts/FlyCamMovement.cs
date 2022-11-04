using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamMovement : MonoBehaviour
{
    float mainSpeed = 100.0f;
    float mouseSensitivity = 100f;
    float yaw = 0.0f;
    float pitch = 0.0f;

    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        transform.eulerAngles = new Vector3(pitch, yaw, 0);


        Vector3 p = GetBaseInput();
        p = p * mainSpeed * Time.deltaTime;
        transform.Translate(p);

    }

    private Vector3 GetBaseInput()
    {   
        //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}

