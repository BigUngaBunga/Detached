using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [Header("Default fields")]
    [SerializeField] List<Activator> activators = new List<Activator>();
    [SerializeField] private bool isTriggered;
    private Data data;

    private void Start()
    {
        data = GetComponent<Data>();
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
            isTriggered = value;
        }
    }

    private void StartTrigger()
    {
        data.Activations++;
        foreach (var activator in activators)
            activator.TriggerActive();
    }

    private void StopTrigger()
    {
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
