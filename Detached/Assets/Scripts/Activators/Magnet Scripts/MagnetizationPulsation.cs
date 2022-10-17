using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MagnetizationPulsation : MonoBehaviour
{
    [SerializeField] Transform magnet;
    private Material material;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        material.SetVector("_TargetPosition", magnet.position);
        Shader.SetGlobalFloat("_GameTime", Time.timeSinceLevelLoad);
    }
}
