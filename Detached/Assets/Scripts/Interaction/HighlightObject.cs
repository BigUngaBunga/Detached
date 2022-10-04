using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    [Header("Debug variables")]
    [SerializeField] private bool isHighlighted = false;
    private bool wasHighlighted = false;


    private ObjectHighlighter highlighter;
    [SerializeField] private List<Renderer> renderers;

    private void Update()
    {
        if (isHighlighted && !wasHighlighted)
        {
            HighlightItem();
            wasHighlighted = true;
        }
        else if (!isHighlighted && wasHighlighted)
        {
            EndHighlight();
            wasHighlighted = false;
        }
    }

    void Start()
    {
        highlighter = FindObjectOfType<ObjectHighlighter>();
        renderers = new List<Renderer>{gameObject.GetComponent<Renderer>()};

        var renderersInChildren = GetComponentsInChildren<Renderer>();
        foreach (var rendererInChild in renderersInChildren)
            renderers.Add(rendererInChild);
    }

    public void HighlightItem() => highlighter.AddRenderers(renderers);
    public void EndHighlight() => highlighter.RemoveRenderers(renderers);
}
