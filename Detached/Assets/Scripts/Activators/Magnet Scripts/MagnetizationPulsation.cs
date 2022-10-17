using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MagnetizationPulsation : MonoBehaviour
{
    [SerializeField] Material shaderMaterial;
    [SerializeField] float frequency = 1f;
    private Material material;
    private float time;
    private float max = 0.1f;
    private float min = 0.05f;

    private RenderTexture highlightRenderTexture;
    private RenderTargetIdentifier renderTargetIdentifier;
    private CommandBuffer commandBuffer;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        time += Time.deltaTime;
        Color color = new Color(material.color.r, material.color.g, material.color.b, GetAlpha());
        material.color = color;
    }

    private float GetAlpha() => Mathf.Lerp(max, min, Mathf.Sin(time * frequency));
}