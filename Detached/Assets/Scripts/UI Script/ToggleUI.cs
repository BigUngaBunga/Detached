using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects;
    private bool isActive = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            isActive = !isActive;
            for (int i = 0; i < gameObjects.Length; i++)
                gameObjects[i].SetActive(isActive);
        }
    }
}
