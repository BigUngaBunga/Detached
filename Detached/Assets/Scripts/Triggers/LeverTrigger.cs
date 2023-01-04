using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using FMODUnity;

public class LeverTrigger : Trigger, IInteractable
{
    [SerializeField] private int requiredArms = 1;

    [SerializeField] private GameObject triggeredLever;
    [SerializeField] private GameObject normalLever;
    private HighlightObject highlight;

    private bool isOnCooldown = false;

    protected override void PlaySoundOnTrigger()
    {
        SFXManager.PlayOneShot(SFXManager.PullLeverSound, VolumeManager.GetSFXVolume(), transform.position);
    }

    protected override void PlaySoundOnStopTrigger()
    {
        SFXManager.PlayOneShot(SFXManager.PullLeverSound, VolumeManager.GetSFXVolume(), transform.position);
    }

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

    [Command(requiresAuthority = false)]
    public void Interact(GameObject activatingObject)
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

    private void RevokeCooldown() => isOnCooldown = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isServer && !collision.gameObject.CompareTag("Player") && CanInteract(collision.gameObject))
        {
            Interact(collision.gameObject);
            isOnCooldown = true;
            Invoke(nameof(RevokeCooldown), 0.1f);
        }
    }

    public bool CanInteract(GameObject activatingObject) => HasEnoughArms(activatingObject, requiredArms) && !isOnCooldown;
}
