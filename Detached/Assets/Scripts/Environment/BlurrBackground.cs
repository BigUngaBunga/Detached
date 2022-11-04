using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BlurrBackground : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private MeshFilter[] meshFilters;
    [SerializeField] private Material blurMaterial;
    [SerializeField] private int blurIntensity;
    [SerializeField] private GameObject background;

    private RenderTexture backgroundTexture;
    private RenderTargetIdentifier renderTargetIdentifier;
    private CommandBuffer commandBuffer;

    private void Awake()
    {
        backgroundTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTargetIdentifier = new RenderTargetIdentifier(backgroundTexture);
        commandBuffer = new CommandBuffer();
        background = GameObject.Find("Room interior");
        meshFilters = background.GetComponentsInChildren<MeshFilter>();
        renderers = background.GetComponentsInChildren<Renderer>();
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    private void ClearCommandBuffers()
    {
        commandBuffer.Clear();
        RenderTexture.active = backgroundTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }

    private void RenderBackground()
    {
        commandBuffer.SetRenderTarget(renderTargetIdentifier);

        for (int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < renderers[i].materials.Length; j++)
            {
                commandBuffer.DrawRenderer(renderers[i], renderers[i].materials[j], j);
                //commandBuffer.DrawMesh(meshFilters[i].mesh, renderers[i].worldToLocalMatrix, renderers[i].materials[j], j);
            }
        }
        RenderTexture.active = backgroundTexture;
        Graphics.ExecuteCommandBuffer(commandBuffer);
        RenderTexture.active = null;
    }

    private void BlurTexture()
    {
        RenderTexture blurTexture1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        RenderTexture blurTexture2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        Graphics.Blit(backgroundTexture, blurTexture1);
        for (int i = 0; i < blurIntensity; i++)
        {
            Graphics.Blit(blurTexture1, blurTexture2, blurMaterial, 1);
            Graphics.Blit(blurTexture2, blurTexture1, blurMaterial, 2);
        }

        Graphics.Blit(blurTexture1, backgroundTexture);

        RenderTexture.ReleaseTemporary(blurTexture1);
        RenderTexture.ReleaseTemporary(blurTexture2);
    }

    //TODO Prova rendera en textur av det som är nära och sedan ta bort den från resterande bilden
    //TODO Resten kan då göras suddig och sedan sättas ihop igen

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        ClearCommandBuffers();
        RenderBackground();
        BlurTexture();
        //Graphics.Blit(backgroundTexture, source);
        Graphics.Blit(source, destination);
    }
}
