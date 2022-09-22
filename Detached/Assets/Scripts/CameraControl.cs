using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 10f;
    float aroundXRotation = 0f;
    float aroundYRotation = 0f;


    [Header("Transform")]
    [SerializeField] private Transform orientation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

 
        
        aroundXRotation -= mouseY;

        aroundXRotation = Mathf.Clamp(aroundXRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(aroundXRotation, 0f, 0f);

        //orientation.rotation = Quaternion.Euler(0f, aroundYRotation, 0f);

        // playerTransform.Rotate(Vector3.up * mouseX);// Vector3.up = y-axis, rotating around y-axis -> looking right/left
    }
}
