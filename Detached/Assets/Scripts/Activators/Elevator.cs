using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Activator
{
    // Update is called once per frame
    void Update()
    {
        if (IsActivated)
        {
            transform.Rotate(new Vector3(0.5f, 0, 0));
        }
    }
}
