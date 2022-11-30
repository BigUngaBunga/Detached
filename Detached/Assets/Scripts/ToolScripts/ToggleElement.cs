using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleElement : MonoBehaviour
{
    public void Toggle(GameObject objectToToggle)
    {
        objectToToggle.SetActive(!objectToToggle.activeInHierarchy);
    }
    public void Toggle(LineRenderer objectToToggle)
    {
        objectToToggle.enabled = objectToToggle.enabled ? false : true;
    }
}