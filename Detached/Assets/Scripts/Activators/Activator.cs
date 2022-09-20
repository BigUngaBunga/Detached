using UnityEngine;

public class Activator : MonoBehaviour
{
    enum ActivationRequirement { All, One, None}
    [SerializeField] private ActivationRequirement activationRequirement;
    [SerializeField] protected bool isActivated = false;

    [SerializeField] private int totalConnections;
    private int ActiveConnections
    {
        get => activeConnections;
        set
        {
            activeConnections = value;
            isActivated = GetActivationStatus();
        }
    }
    [SerializeField] private int activeConnections;

    public void AddConnection() => ++totalConnections;
    public void TriggerActive() => ++ActiveConnections;
    public void TriggerInactive() => --ActiveConnections;

    private bool GetActivationStatus()
    {
        switch (activationRequirement)
        {
            case ActivationRequirement.All:
                return totalConnections <= ActiveConnections;
            case ActivationRequirement.One:
                return ActiveConnections > 0;
            case ActivationRequirement.None:
                return ActiveConnections.Equals(0);
            default:
                return false;
        }
    }

    void Start()
    {
        isActivated = GetActivationStatus();
    }
}
