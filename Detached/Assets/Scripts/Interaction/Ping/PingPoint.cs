using UnityEngine;

public class PingPoint : MonoBehaviour
{
    private HighlightObject highlight;

    void Start()
    {
        highlight = GetComponent<HighlightObject>();
        highlight.Highlight();
    }

    public void RemoveAfterDuration(float duration) => Invoke(nameof(RemovePing), duration);

    private void RemovePing()
    {
        highlight.EndHighlight();
        Destroy(gameObject);
    }
}
