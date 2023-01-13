using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIButtons : MonoBehaviour
{
    [SerializeField]
    private GameObject[] buttons;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(!buttons[i].activeSelf);
            }
        }
    }
}
