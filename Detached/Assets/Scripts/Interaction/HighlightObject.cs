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

    private void Update()
    {
        //if (isHighlighted && !wasHighlighted)
        //{
        //    HighlightItem();
        //    wasHighlighted = true;
        //}
        //else if (!isHighlighted && wasHighlighted)
        //{
        //    EndHighlight();
        //    wasHighlighted = false;
        //}
    }

    void Start()
    {
        highlighter = FindObjectOfType<HighlightHandler>();
        renderers = new List<Renderer>();
        if (gameObject.TryGetComponent<Renderer>(out var renderer))
            renderers.Add(renderer);

        var renderersInChildren = GetComponentsInChildren<Renderer>();
        foreach (var rendererInChild in renderersInChildren)
            renderers.Add(rendererInChild);
    }

    public void DurationHighlight()
    {
        if (!isHighlighted)
            highlighter.AddRenderers(renderers);
        isHighlighted = true;
        if (IsInvoking(StopMethod))
            CancelInvoke(StopMethod);
        Invoke(StopMethod, highlightMilliseconds / 1000);
    }

    public void Highlight()
    {
        if (!isHighlighted)
            highlighter.AddRenderers(renderers);
        isHighlighted = true;
    }

    public void EndHighlight()
    {
        if (isHighlighted)
            highlighter.RemoveRenderers(renderers);
        isHighlighted =false;
    }
}
