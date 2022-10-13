using UnityEngine;

public class PreventHighlight : MonoBehaviour
{
    void Start()
    {
        var highlighter = GetComponentInParent<HighlightObject>();
        highlighter.TryRemoveRenderer(GetComponent<Renderer>());
    }
}
