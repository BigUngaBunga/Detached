using UnityEngine;
using Mirror;

public class Activator : NetworkBehaviour
{
    public enum ActivationRequirement { All, One, None}

    [Header("Default fields")]
    [SerializeField] protected ActivationRequirement activationRequirement;
    [SerializeField] private bool isActivated = false;

    public bool locked;
    private bool active;

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

    [SerializeField] private int totalConnections;
    [SerializeField] private int activeConnections;
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
    public void TriggerActive() => ++ActiveConnections;
    public void TriggerInactive() => --ActiveConnections;
    public void ReevaluateActivation() => IsActivated = active;

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

    void Start()
    {
        IsActivated = GetActivationStatus();
    }
}

