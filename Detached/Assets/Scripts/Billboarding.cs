using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    [SerializeField] bool PreSpawn;
    [SerializeField] private string prompt;
    public GameObject floatingText;
    private Camera camera;
    private Vector3 cameraDir;
    private TextMeshPro text;

    void Start()
    {
        text = null;
        SetTextVisible(PreSpawn);
    }

    void Update()
    {
        if (text == null) return;
        if(camera == null)
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cameraDir = camera.transform.forward;
        cameraDir.y = 0;

        text.transform.rotation = Quaternion.LookRotation(cameraDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "ChedBody" || other.gameObject.name == "DetaBody")
            SetTextVisible(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ChedBody" || other.gameObject.name == "DetaBody")
            SetTextVisible(false);
    }

    private void CreateText()
    {
        text = Instantiate(floatingText, transform.position, Quaternion.identity, transform).GetComponentInChildren<TextMeshPro>();
        text.GetComponent<TextMeshPro>().SetText(prompt);
        int multiplier = 2 + prompt.Length / 40;
        text.GetComponent<TextMeshPro>().gameObject.transform.position += Vector3.up * multiplier;
    }

    private void SetTextVisible(bool setVisible)
    {
        if (text == null)
            CreateText();
        text.gameObject.SetActive(setVisible);
    }
}
