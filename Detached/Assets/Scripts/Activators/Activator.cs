using UnityEngine;

public class Activator : MonoBehaviour
{
    enum ActivationRequirement { All, One, None}

    [Header("Default fields")]
    [SerializeField] private ActivationRequirement activationRequirement;
    [SerializeField] private bool isActivated = false;
    protected bool IsActivated { 
        get { return isActivated; }
        set { isActivated = value;
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

    public void AddConnection() => ++totalConnections;
    public void TriggerActive() => ++ActiveConnections;
    public void TriggerInactive() => --ActiveConnections;

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
