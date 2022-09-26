using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Renderer renderer;
    private LevelEditorManager editor;

    public int ID;
    void Start()
    {
        renderer = GetComponent<Renderer>();
        editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
    }

    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        var scan = transform;
        while (scan.parent && scan.parent.GetComponent<Target>() != null)
        {
            scan = scan.parent;
        }
        foreach (Target target in scan.GetComponentsInChildren<Target>())
        {
            target.renderer.material.color = Color.red;
        }
    }

    private void OnMouseExit()
    {
        var scan = transform;
        while (scan.parent && scan.parent.GetComponent<Target>() != null)
            scan = scan.parent;
        foreach (var target in scan.GetComponentsInChildren<Target>())
            target.renderer.material.color = Color.white;
    }
}
