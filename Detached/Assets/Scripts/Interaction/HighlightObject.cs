using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : NetworkBehaviour
{
    [Header("Debug variables")]
    [SerializeField] private bool isHighlighted = false;
    [SerializeField] private float highlightMilliseconds = 50;
    [SyncVar] private bool lockHighlight = false;

    private string StopMethod => nameof(EndHighlight);

    private HighlightHandler highlighter;
    [SerializeField] private List<Renderer> renderers;
    public List<Renderer> Renderers { get => renderers; }

    private void Awake()
    {
        highlighter = FindObjectOfType<HighlightHandler>();
        renderers = new List<Renderer>();
        UpdateRenderers();
    }

    private void Start()
    {
        if (renderers.Count <= 0)
            UpdateRenderers();
    }

    [Command(requiresAuthority = false)]
    public void UpdateRenderers() => RCPUpdateRenderers();

    [ClientRpc]
    private void RCPUpdateRenderers()
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
        int index = -1;
        if (renderers.Contains(renderer))
            index = renderers.IndexOf(renderer);
        if (index == -1 || index >= renderers.Count)
            return;
        RemoveRenderer(index);
    }


    [Command(requiresAuthority = false)]
    public void RemoveRenderer(int index) => RCPRemoveRenderer(index);

    [ClientRpc]
    private void RCPRemoveRenderer(int index) => renderers.RemoveAt(index);

    public void DurationHighlight()
    {
        Highlight();
        if (IsInvoking(StopMethod))
            CancelInvoke(StopMethod);
        Invoke(StopMethod, highlightMilliseconds / 1000);
    }

    public void Highlight()
    {
        if (!isHighlighted)
        {
            if (renderers.Count <= 0)
                UpdateRenderers();
            isHighlighted = true;
            highlighter.AddHighlight(this);
        }       
    }

    public void ForceHighlight()
    {
        lockHighlight = true;
        Highlight();
    }


    public void EndHighlight()
    {
        if (isHighlighted && !lockHighlight)
        {
            isHighlighted = false;
            highlighter.RemoveHighlight(this);
        }
    }

    public void ForceStopHighlight()
    {
        lockHighlight = false;
        EndHighlight();
    }
}
