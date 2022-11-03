using Mirror;
using System.Collections.Generic;
using UnityEngine;
using LimbType = ItemManager.Limb_enum;


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
        }
    }

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

    protected bool HasEnoughArms(GameObject player, int requiredArms)
    {
        if(player.CompareTag("Player"))
            return player.GetComponent<ItemManager>().NumberOfArms >= requiredArms;
        return false;
    }

    protected bool IsLimbOfType(GameObject limb, LimbType type) => limb.CompareTag("Limb") && limb.GetComponent<SceneObjectItemManager>().thisLimb == type;

    public void Awake()
    {
        foreach (var activator in activators)
            activator.AddConnection();
    }
}
