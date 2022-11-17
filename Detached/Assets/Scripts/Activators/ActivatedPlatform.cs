using Mirror;
using UnityEngine;

public class ActivatedPlatform : Activator
{
    [Header("Platform fields")]
    [Range(0.05f, 1f)]
    [SerializeField] float inactiveAlpha = 0.33f;
    [Range(0.05f, 0.33f)]
    [SerializeField] protected float minimumAlpha = 0.1f;

    [SyncVar(hook  = nameof(UpdateMaterial))]
    protected float alpha = 0.0f;

    private Color color;
    protected new BoxCollider collider;
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
        UpdateColliderOnServer(true);
        UpdateMaterialOnServer();
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        UpdateColliderOnServer(false);
        UpdateMaterialOnServer();
    }

    public override void TriggerActive()
    {
        base.TriggerActive();
        UpdateMaterialOnServer();
    }

    public override void TriggerInactive()
    {
        base.TriggerInactive();
        UpdateMaterialOnServer();
    }

    protected void UpdateColliderOnServer(bool isActive)
    {
        if (NetworkClient.isHostClient)
            UpdateCollider(isActive);
    }

    [Server]
    protected virtual void UpdateCollider(bool isActive) => collider.enabled = isActive;

    private void UpdateMaterialOnServer()
    {
        if (NetworkClient.isHostClient)
            UpdateAlpha();
    }

    [Server]
    protected virtual void UpdateAlpha()
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
