using UnityEngine;

public class ActivatedPlatform : Activator
{
    [Header("Platform fields")]
    [Range(0.05f, 1f)]
    [SerializeField] float inactiveAlpha = 0.33f;
    [Range(0.05f, 0.33f)]
    [SerializeField] float minimumAlpha = 0.1f;
    private Color color;
    private BoxCollider collider;
    private MeshRenderer meshRenderer;
    private float PercentageActive => (float)ActiveConnections / (float)TotalConnections;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        color = meshRenderer.material.color;
        collider = GetComponent<BoxCollider>();
    }

    protected override void Activate()
    {
        base.Activate();
        collider.enabled = true;
        UpdateMaterial();
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        collider.enabled = false;
        UpdateMaterial();
    }


    private void UpdateMaterial()
    {
        if (IsActivated)
            color.a = 1f;
        else if(activationRequirement.Equals(ActivationRequirement.All))
            color.a = Mathf.Max(PercentageActive / 2f, minimumAlpha);
        else
            color.a = inactiveAlpha;

        meshRenderer.material.color = color;
    }
}
