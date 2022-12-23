using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;




public class MainMenu : MonoBehaviour
{
    [SerializeField] private Slider sliderSensitivity;
    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    public float clampAngle = 80.0f;
    float mouseSensitivity = 100f;
    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
/*        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;*/
    }
    public void adjustMouseSensitivity()
    {
         mouseSensitivity = sliderSensitivity.value;
    }
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        //EditorApplication.isPlaying = false; // causing build errors because we're not in editor when playing from an app standpoint
        Application.Quit();

    }
}
