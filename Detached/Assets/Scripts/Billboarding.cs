using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    [SerializeField] bool PreSpawn;
    public string prompt;
    public GameObject floatingText;
    public Camera camera;
    Vector3 cameraDir;

    TextMeshPro text;


    void Start()
    {
        if (PreSpawn)
        {
            text = Instantiate(floatingText, transform.position, Quaternion.identity, transform).GetComponentInChildren<TextMeshPro>();
            text.GetComponent<TextMeshPro>().SetText(prompt);
            int multiplier = 2 + prompt.Length / 40;
            text.GetComponent<TextMeshPro>().gameObject.transform.position += Vector3.up * multiplier;
        }
    }

    void Update()
    {
        if (text == null)
        {
            return;
        }
        cameraDir = camera.transform.forward;
        cameraDir.y = 0;

        text.transform.rotation = Quaternion.LookRotation(cameraDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (text != null) { return; }
        if (other.gameObject.name == "ChedBody" || other.gameObject.name == "DetaBody")
        {
            text = Instantiate(floatingText, transform.position, Quaternion.identity, transform).GetComponentInChildren<TextMeshPro>();
            text.GetComponent<TextMeshPro>().SetText(prompt);
            text.GetComponent<TextMeshPro>().fontSize = 16;
            int multiplier = 2 + prompt.Length / 40;
            text.GetComponent<TextMeshPro>().gameObject.transform.position += Vector3.up*multiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ChedBody" || other.gameObject.name == "DetaBody")
        {
            if (text == null)
            {
                return;
            }
            Destroy(text);
        }
    }
}
