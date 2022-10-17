using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    [Header("Debug variables")]
    [SerializeField] private bool isHighlighted = false;
    [SerializeField] private float highlightMilliseconds = 50;
    private bool wasHighlighted = false;

    private string StopMethod => nameof(EndHighlight);

    private HighlightHandler highlighter;
    [SerializeField] private List<Renderer> renderers;
    public List<Renderer> Renderers { get => renderers; }

    private void Awake()
    {
        highlighter = FindObjectOfType<HighlightHandler>();
        renderers = new List<Renderer>();
        GetRenderers();
    }

    private void GetRenderers()
    {
        renderers.Clear();
        if (gameObject.TryGetComponent<Renderer>(out var renderer))
            renderers.Add(renderer);
        var renderersInChildren = GetComponentsInChildren<Renderer>();
        foreach (var rendererInChild in renderersInChildren)
            renderers.Add(rendererInChild);
    }

    public void TryRemoveRenderer(Renderer renderer)
    {
        if (renderers.Contains(renderer))
            renderers.Remove(renderer);
    }

    public void DurationHighlight()
    {
        if (!isHighlighted)
            highlighter.AddHighlight(this);
        isHighlighted = true;
        if (IsInvoking(StopMethod))
            CancelInvoke(StopMethod);
        Invoke(StopMethod, highlightMilliseconds / 1000);
    }

    public void Highlight()
    {
        if (!isHighlighted)
            highlighter.AddHighlight(this);
        isHighlighted = true;
    }

    public void EndHighlight()
    {
        if (isHighlighted)
            highlighter.RemoveHighlight(this);
        isHighlighted = false;
    }
}
