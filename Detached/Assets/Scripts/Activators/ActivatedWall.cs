using Mirror;
using UnityEngine;

public class ActivatedWall : ActivatedPlatform
{
    
    private float PercentageActive => (TotalConnections - ActiveConnections) / (float)TotalConnections;

    protected override void Awake()
    {
        base.Awake();
        //Debug
    }

    protected override void Activate()
    {
        base.Activate();
    }

    protected override void Deactivate()
    {
        base.Deactivate();
    }

    [ClientRpc]
    protected override void RPCUpdateCollider(bool isActive) => collider.enabled = !isActive;



    [Server]
    protected override void UpdateAlpha()
    {
        if (isActivated)
            alpha = 0f;
        else if (activationRequirement.Equals(ActivationRequirement.All))
            alpha = PercentageActive;
        else
            alpha = 1f;
    }
}
