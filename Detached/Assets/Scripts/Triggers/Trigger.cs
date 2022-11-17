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
            if (value)
                StartTrigger();
            else
                StopTrigger();
        }
    }

    [Command(requiresAuthority = false)]
    private void StartTrigger()
    {
        if (IsTriggered)
            return;
        isTriggered = true;
        data.Activations++;
        foreach (var activator in activators)
            activator.TriggerActive();
        PlaySoundOnTrigger();
    }
    [Command(requiresAuthority = false)]
    private void StopTrigger()
    {
        if (!IsTriggered)
            return;
        isTriggered = false;
        foreach (var activator in activators)
            activator.TriggerInactive();
        PlaySoundOnStopTrigger();
    }

    protected virtual void PlaySoundOnTrigger(){}
    protected virtual void PlaySoundOnStopTrigger(){}

    protected bool IsLimbOfType(GameObject limb, LimbType type) => limb.CompareTag("Limb") && limb.GetComponent<SceneObjectItemManager>().thisLimb == type;

    protected bool HasEnoughArms(GameObject activatingObject, int requiredArms)
    {
        if (activatingObject.TryGetComponent(out ItemManager manager))
            return manager.NumberOfArms >= requiredArms;
        return IsLimbOfType(activatingObject, LimbType.Arm) && requiredArms < 2;
    }

    public void Awake()
    {
        foreach (var activator in activators)
            activator.AddConnection();
    }
}
