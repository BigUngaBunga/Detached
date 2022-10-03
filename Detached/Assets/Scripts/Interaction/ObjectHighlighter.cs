using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectHighlighter : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    private RenderTexture highlightRenderTexture;
    private RenderTargetIdentifier renderTargetIdentifier;
    private CommandBuffer commandBuffer;
    private int sortingType;
    PostProcessAttribute postProcess;

    private List<GameObject> gameObjects;
    private Dictionary<Renderer, bool> renderers;

    private void Awake()
    {
        highlightRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTargetIdentifier = new RenderTargetIdentifier(highlightRenderTexture);
        commandBuffer = new CommandBuffer();
        renderers = new Dictionary<Renderer, bool>();
    }

    public void AddRenderer(Renderer renderer, bool isHighlighted) => renderers.Add(renderer, isHighlighted);
    public void RemoveRenderer(Renderer renderer) => renderers.Remove(renderer);
    public void SetRendererStatus(Renderer renderer, bool isHighlighted) => renderers[renderer] = isHighlighted;

    private void ClearCommandBuffers()
    {
        commandBuffer.Clear();
    }

    private void RenderHighlights()
    {
        commandBuffer.SetRenderTarget(renderTargetIdentifier);

        foreach (var keyValuePair in renderers)
            if (keyValuePair.Value)
                commandBuffer.DrawRenderer(keyValuePair.Key, highlightMaterial, 0, sortingType);

        RenderTexture.active = highlightRenderTexture;
        Graphics.ExecuteCommandBuffer(commandBuffer);
        RenderTexture.active = null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        ClearCommandBuffers();
        RenderHighlights();

        RenderTexture renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        //TODO sätt in en blur effekt 
        //blur.OnRenderImage(highlightRenderTexture, renderTexture);
        
        highlightMaterial.SetTexture("_OccludeMap", highlightRenderTexture);
        Graphics.Blit(renderTexture, renderTexture, highlightMaterial, 0);
        highlightMaterial.SetTexture("_OccludeMap", renderTexture);
        Graphics.Blit (source, destination, highlightMaterial, 1);

        RenderTexture.ReleaseTemporary(renderTexture);
    }
}
