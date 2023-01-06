using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public string Name = "";
    public int Activations = 0;
    public Vector3 position;
    private void Start()
    {
        position = transform.position;
        Name = gameObject.name;
    }
}