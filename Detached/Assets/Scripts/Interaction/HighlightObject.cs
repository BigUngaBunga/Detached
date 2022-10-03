using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    private ObjectHighlighter highlighter;
    private Renderer renderer;
    void Start()
    {
        highlighter = GameObject.FindObjectOfType<ObjectHighlighter>();
        renderer = gameObject.GetComponent<Renderer>();
    }


}
