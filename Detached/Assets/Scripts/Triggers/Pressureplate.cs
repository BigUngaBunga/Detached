using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pressureplate : Trigger
{
    private int TriggeringObjects { 
        get => triggeringObjects; 
        set {
            triggeringObjects = value;
            IsTriggered = triggeringObjects > 0;
        } 
    }
    [SerializeField] private int triggeringObjects;

    private bool IsCollisionObject(string tag) => tag.Equals("Box") || tag.Equals("Leg") || tag.Equals("Torso");

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Something entered");
        if (IsCollisionObject(collision.gameObject.tag))
            ++TriggeringObjects;
    }

    public void OnCollisionExit(Collision collision)
    {
        Debug.Log("Something left");
        if (IsCollisionObject(collision.gameObject.tag))
            --TriggeringObjects;
    }
}
