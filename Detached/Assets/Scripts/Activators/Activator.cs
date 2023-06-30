using UnityEngine;
using Mirror;

public class Activator : NetworkBehaviour
{
    public enum ActivationRequirement { All, One, None}

    [Header("Default fields")]
    [SerializeField] protected ActivationRequirement activationRequirement;

    [SyncVar][SerializeField] protected bool isActivated = false;

    [SyncVar] public bool locked;
    [SyncVar] protected bool active;

    protected bool IsActivated
    {
        get { return isActivated; }
        set { SetActivation(value); }
    }

    [SerializeField] private int totalConnections;
    [SyncVar] [SerializeField] private int activeConnections;
    protected int ActiveConnections
    {
        get => activeConnections;
        set
        {
            activeConnections = value;
            IsActivated = GetActivationStatus();
        }
    }

    protected int TotalConnections { get { return totalConnections; } }

    public void AddConnection() => ++totalConnections;
    public virtual void TriggerActive() => ++ActiveConnections;
    public virtual void TriggerInactive() => --ActiveConnections;
    public void ReevaluateActivation() => IsActivated = active;

    [Command(requiresAuthority = false)]
    private void SetActivation(bool value)
    {
        active = value;
        isActivated = active && !locked;
        if (isActivated)
            Activate();
        else
            Deactivate();
    }

    [Command(requiresAuthority = false)]
    protected virtual void Activate() {}

    [Command(requiresAuthority = false)]
    protected virtual void Deactivate() {}
    private bool GetActivationStatus()
    {
        return activationRequirement switch
        {
            ActivationRequirement.All => totalConnections <= ActiveConnections && activeConnections > 0,
            ActivationRequirement.One => ActiveConnections > 0,
            ActivationRequirement.None => ActiveConnections.Equals(0),
            _ => false,
        };
    }

    [Command(requiresAuthority = false)]
    protected virtual void Start()
    {
        IsActivated = GetActivationStatus();
    }
}

