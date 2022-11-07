using Mirror;
using UnityEngine;

public class ActivatedWall : ActivatedPlatform
{
    
    private float PercentageActive => (TotalConnections - ActiveConnections) / (float)TotalConnections;

    protected override void Activate()
    {
        base.Activate();
        collider.enabled = false;
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        collider.enabled = true;
    }

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
