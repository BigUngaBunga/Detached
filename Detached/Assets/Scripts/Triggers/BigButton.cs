using UnityEngine;

public class BigButton : Trigger
{
    private int TriggeringObjects { 
        get => triggeringObjects; 
        set {
            triggeringObjects = value;
            IsTriggered = triggeringObjects > 0;
        } 
    }
    [SerializeField] private int triggeringObjects;

    private bool IsCollisionObject(string tag) => tag.Equals("Box") || tag.Equals("Leg") || tag.Equals("Player");

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Something entered");
        if (IsCollisionObject(other.gameObject.tag))
            ++TriggeringObjects;
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("Something left");
        if (IsCollisionObject(other.gameObject.tag))
            --TriggeringObjects;
    }
}
