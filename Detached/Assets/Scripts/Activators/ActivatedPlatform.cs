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

    [SerializeField]private Color color;
    protected new BoxCollider collider;
    private MeshRenderer meshRenderer;
    private float PercentageActive => (float)ActiveConnections / (float)TotalConnections;

    protected virtual void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        color = meshRenderer.material.color;
        collider = GetComponent<BoxCollider>();
    }

    protected override void Start()
    {
        base.Start();
        UpdateMaterialOnServer();
    }

    protected override void Activate()
    {
        base.Activate();
        UpdateCollider(true);
        UpdateMaterialOnServer();
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        UpdateCollider(false);
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

    protected void UpdateCollider(bool isActive)
    {
        if (NetworkClient.isHostClient)
            RPCUpdateCollider(isActive);
    }

    [ClientRpc]
    protected virtual void RPCUpdateCollider(bool isActive) => collider.enabled = isActive;

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


    protected void UpdateMaterial(float oldValue, float newValue)
    {
        color.a = newValue;
        meshRenderer.material.color = color;
        meshRenderer.material.SetFloat("_Alpha", alpha);
    }
}
