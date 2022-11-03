using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;

public class LeverTrigger : Trigger, IInteractable
{
    [SerializeField] private int requiredArms = 1;

    [SerializeField] private GameObject triggeredLever;
    [SerializeField] private GameObject normalLever;
    private HighlightObject highlight;

    protected override void Start()
    {
        base.Start();
        normalLever = transform.Find("InactiveLever").gameObject;
        triggeredLever = transform.Find("TriggeredLever").gameObject;
        highlight = GetComponent<HighlightObject>();
        UpdateLeverRPC();
    }
    
    [Command(requiresAuthority = false)]
    private void UpdateLeverRPC() => RPCSetLeverActivation(IsTriggered);

    private void SetRecursiveActivation(bool isActive, GameObject gameObject)
    {
        //Debug.Log("Setting object to " + (isActive ? "active" : "inactive"));
        int children = gameObject.transform.childCount;
        for (int i = 0; i < children; i++)
            SetRecursiveActivation(isActive, gameObject.transform.GetChild(i).gameObject);
        gameObject.SetActive(isActive);
    }

    public void Interact(GameObject activatingObject)
    {
        if (CanInteract(activatingObject))
            PullLever();
    }

    [Command(requiresAuthority = false)]
    private void PullLever()
    {
        IsTriggered = !IsTriggered;
        RPCSetLeverActivation(IsTriggered);
        highlight.UpdateRenderers();
    }

    [ClientRpc]
    private void RPCSetLeverActivation(bool isTriggered)
    {
        SetRecursiveActivation(!isTriggered, normalLever);
        SetRecursiveActivation(isTriggered, triggeredLever);
    }

    public bool CanInteract(GameObject activatingObject)
    {
        if (activatingObject.CompareTag("Player"))
            return HasEnoughArms(activatingObject, requiredArms);
        else if (IsLimbOfType(activatingObject, ItemManager.Limb_enum.Arm))
            return requiredArms < 2;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isServer && !collision.gameObject.CompareTag("Player") && CanInteract(collision.gameObject))
            PullLever();
    }
}
