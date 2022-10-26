using Mirror;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger : NetworkBehaviour
{
    [Header("Default fields")]
    [SerializeField] List<Activator> activators = new List<Activator>();
    [SyncVar] [SerializeField] private bool isTriggered;
    private Data data;

    protected virtual void Start()
    {
        data = GetComponent<Data>();
        Debug.Log("Activations " + data.Activations);
    }
    protected bool IsTriggered
    {
        get => isTriggered;
        set
        {
            //if (!value && IsTriggered)
            //    StopTrigger();
            //else if (value && !IsTriggered)
            //    StartTrigger();

            if ((!value && IsTriggered) || (value && !IsTriggered))
                HandleTrigger();
            //isTriggered = value;
        }
    }

    private void HandleTrigger()
    {
        if (isClient)
        {
            if (IsTriggered)
                CommandStartTrigger();
            else
                CommandStopTrigger();
        }
        else
        {
            if (IsTriggered)
                StartTrigger();
            else
                StopTrigger();
        }
    }

    [Command]
    private void CommandStartTrigger() => StartTrigger();

    [Command]
    private void CommandStopTrigger() => StopTrigger();

    [Server]
    private void StartTrigger()
    {
        isTriggered = true;
        data.Activations++;
        foreach (var activator in activators)
            activator.TriggerActive();
    }

    [Server]
    private void StopTrigger()
    {
        isTriggered = false;
        foreach (var activator in activators)
            activator.TriggerInactive();
    }

    protected bool HasEnoughArms(GameObject player, int requiredArms) => player.GetComponent<ItemManager>().NumberOfArms >= requiredArms;

    public void Awake()
    {
        foreach (var activator in activators)
            activator.AddConnection();
    }
}
