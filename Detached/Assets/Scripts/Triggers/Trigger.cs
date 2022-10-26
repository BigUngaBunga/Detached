using Mirror;
using System.Collections.Generic;
using Telepathy;
using UnityEditor;
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
            if (!value && IsTriggered)
                StopTrigger();
            else if (value && !IsTriggered)
                StartTrigger();
            //isTriggered = value;
        }
    }

    //private void HandleTrigger(bool value)
    //{
    //        if (NetworkClient.localPlayer.isClientOnly)
    //        {
    //            Debug.Log("Activating as non host client");
    //        if (!value)
    //            StartTrigger();
    //        else
    //            StopTrigger();
    //        }
    //        else
    //        {
    //            Debug.Log("Activating as host client");
    //            if (!value)
    //                StartTrigger();
    //            else
    //                StopTrigger();
    //        }
    //}

    [Command(requiresAuthority = false)]
    private void StartTrigger()
    {
        isTriggered = true;
        data.Activations++;
        foreach (var activator in activators)
            activator.TriggerActive();
    }
    [Command(requiresAuthority = false)]
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
