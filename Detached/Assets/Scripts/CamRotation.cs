using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotation : MonoBehaviour
{

    public Vector2 mouse;
    [SerializeField] private float mouseSensitivity = 0.5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        mouse.x = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouse.y = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.localRotation = Quaternion.Euler(-mouse.y, mouse.x, 0);
    }
}
