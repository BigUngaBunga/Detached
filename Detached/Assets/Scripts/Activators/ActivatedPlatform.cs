using UnityEngine;

public class ActivatedPlatform : Activator
{
    [Header("Platform fields")]
    [SerializeField] Material activeMaterial;
    [SerializeField] Material inactiveMaterial;
    private BoxCollider collider;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<BoxCollider>();
    }

    protected override void Activate()
    {
        base.Activate();
        collider.enabled = true;
        meshRenderer.material = activeMaterial;
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        collider.enabled = false;
        meshRenderer.material = inactiveMaterial;
    }
}
