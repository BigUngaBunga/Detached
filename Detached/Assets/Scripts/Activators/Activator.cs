using UnityEngine;
using Mirror;

public class Activator : NetworkBehaviour
{
    public enum ActivationRequirement { All, One, None}

    [Header("Default fields")]
    [SerializeField] protected ActivationRequirement activationRequirement;
    //[SerializeField] private bool isActivated = false;

    [SyncVar(hook = nameof(OnChangeActivation))][SerializeField] protected bool isActivated = false;

    [SyncVar] public bool locked;
    [SyncVar] protected bool active;

    protected bool IsActivated
    {
        get { return isActivated; }
        set
        {
            active = value;
            isActivated = active && !locked;
            if (isActivated)
                Activate();
            else
                Deactivate();
        }
    }

    private void OnChangeActivation(bool oldValue, bool newValue)
    {
        active = newValue;
        isActivated = active && !locked;
        if (isActivated)
            Activate();
        else
            Deactivate();
    }

    [SerializeField] private int totalConnections;
    [SyncVar] [SerializeField] private int activeConnections;
    protected int ActiveConnections
    {
        get => activeConnections;
        set
        {
            activeConnections = value;
            isActivated = GetActivationStatus();
        }
    }

    protected int TotalConnections { get { return totalConnections; } }

    public void AddConnection() => ++totalConnections;
    public virtual void TriggerActive() => ++ActiveConnections;
    public virtual void TriggerInactive() => --ActiveConnections;
    public void ReevaluateActivation() => isActivated = active;

    protected virtual void Activate() { }
    protected virtual void Deactivate() { }
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

    [Server]
    protected virtual void Start()
    {
        IsActivated = GetActivationStatus();
    }
}

