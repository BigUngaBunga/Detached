using Mirror;
using UnityEngine;

public class ActivatedPlatform : Activator
{
    [Header("Platform fields")]
    [Range(0.05f, 1f)]
    [SerializeField] float inactiveAlpha = 0.33f;
    [Range(0.05f, 0.33f)]
    [SerializeField] float minimumAlpha = 0.1f;

    [SyncVar(hook  = nameof(UpdateMaterial))]
    private float alpha = 0.0f;

    private Color color;
    private new BoxCollider collider;
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
        if (isServer)
            UpdateAlpha();

    }

    protected override void Deactivate()
    {
        base.Deactivate();
        collider.enabled = false;
        if (NetworkClient.isHostClient)
            UpdateAlpha();

    }

    public override void TriggerActive()
    {
        base.TriggerActive();
        if (NetworkClient.isHostClient)
            UpdateAlpha();
    }

    public override void TriggerInactive()
    {
        base.TriggerInactive();
        if (NetworkClient.isHostClient)
            UpdateAlpha();
    }

    [Server]
    private void UpdateAlpha()
    {
        if (isActivated)
            alpha = 1f;
        else if (activationRequirement.Equals(ActivationRequirement.All))
            alpha = Mathf.Max(PercentageActive / 2f, minimumAlpha);
        else
            alpha = inactiveAlpha;
    }


    private void UpdateMaterial(float oldValue, float newValue)
    {

        color.a = newValue;
        meshRenderer.material.color = color;
    }
}
