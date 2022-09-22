using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] List<Activator> activators = new List<Activator>();
    [SerializeField] private bool isTriggered;
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
        foreach (var activator in activators)
            activator.TriggerActive();
    }

    private void StopTrigger()
    {
        foreach (var activator in activators)
            activator.TriggerInactive();
    }

    public void Awake()
    {
        foreach (var activator in activators)
            activator.AddConnection();
    }

}
