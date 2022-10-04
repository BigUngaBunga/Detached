using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectHighlighter : MonoBehaviour
{
    //Made with help from https://xroft666.blogspot.com/2015/07/glow-highlighting-in-unity.html?view=classic
    //GaussianBlur shader from https://github.com/keijiro/GaussianBlur

    [Range(1, 25)]
    [SerializeField] private int blurIntensity;
    [SerializeField] private bool displayOnlyHighlights;

    [Header("Materials")]
    [SerializeField] private Material drawMaterial;
    [SerializeField] private Material blurMaterial;
    [SerializeField] private Material highlightMaterial;


    private RenderTexture highlightRenderTexture;
    private RenderTargetIdentifier renderTargetIdentifier;
    private CommandBuffer commandBuffer;
    private int sortingType;

    private List<List<Renderer>> rendererCollections;

    private void Awake()
    {
        highlightRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTargetIdentifier = new RenderTargetIdentifier(highlightRenderTexture);
        commandBuffer = new CommandBuffer();
        rendererCollections = new List<List<Renderer>>();
    }

    public void AddRenderers(List<Renderer> renderers)
    {
        rendererCollections.Add(renderers);
        Debug.Log("Added item, now highligting " + rendererCollections.Count + " items");
    }
    public void RemoveRenderers(List<Renderer> renderers)
    {
        rendererCollections.Remove(renderers);
        Debug.Log("Removed item, now highligting " + rendererCollections.Count + " items");
    }


    private void ClearCommandBuffers()
    {
        commandBuffer.Clear();
        RenderTexture.active = highlightRenderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }

    //Renders all active renderers to a static monochrome texture
    private void RenderHighlights()
    {
        commandBuffer.SetRenderTarget(renderTargetIdentifier);
        
        foreach (var rendererCollection in rendererCollections)
            foreach (var renderer in rendererCollection)
                if (renderer.gameObject.activeSelf)
                    commandBuffer.DrawRenderer(renderer, drawMaterial, 0, sortingType);

        RenderTexture.active = highlightRenderTexture;
        Graphics.ExecuteCommandBuffer(commandBuffer);
        RenderTexture.active = null;
    }

    //TODO refactor och skriv kommentarer
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        ClearCommandBuffers();
        RenderHighlights();
        RenderTexture blurTexture1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        RenderTexture blurTexture2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        RenderTexture renderTextureTarget = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        Graphics.Blit(highlightRenderTexture, blurTexture1);

        for (int i = 0; i < blurIntensity; i++)
        {
            Graphics.Blit(blurTexture1, blurTexture2, blurMaterial, 1);
            Graphics.Blit(blurTexture2, blurTexture1, blurMaterial, 2);
        }

        highlightMaterial.SetTexture("_OccludeMap", highlightRenderTexture);
        Graphics.Blit(blurTexture2, renderTextureTarget, highlightMaterial, 0);
        highlightMaterial.SetTexture("_OccludeMap", renderTextureTarget);

        if (displayOnlyHighlights)
            Graphics.Blit(renderTextureTarget, destination, highlightMaterial, 1);
        else
            Graphics.Blit(source, destination, highlightMaterial, 1);

        RenderTexture.ReleaseTemporary(renderTextureTarget);
        RenderTexture.ReleaseTemporary(blurTexture1);
        RenderTexture.ReleaseTemporary(blurTexture2);
    }
}
