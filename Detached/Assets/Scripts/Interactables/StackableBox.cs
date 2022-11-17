using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class StackableBox : Carryable
{
    [SerializeField]private Transform stackingTransform;
    [SyncVar] private Vector3 stackingPosition;
    [SyncVar] private Quaternion stackingRotation;
    [SyncVar] private bool hasBoxAbove;
    [SyncVar] private StackableBox boxBelow;
    private HighlightObject highlighter;
    
    [SerializeField]
    private float offsetFactor = 1f;
    private float offset;

    private bool isKinematic;
    private bool IsKinematic 
    { 
        get { return isKinematic; } 
        set { isKinematic = value; 
                rigidbody.isKinematic = IsKinematic; } 
    }

    private bool showPlacement;
    private bool ShowPlacement
    {
        get { return showPlacement; }
        set
        {
            stackingTransform.gameObject.SetActive(value);
            if (showPlacement != value)
                highlighter.UpdateRenderers();
            showPlacement = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        highlighter = GetComponent<HighlightObject>();
        ShowPlacement = false;
        offset = stackingTransform.localPosition.z;
    }

    public override void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();
        if (itemManager.IsCarryingTag("Box"))
        {
            itemManager.AttemptDropItem(out GameObject box);
            StackBox(box.GetComponent<StackableBox>(), stackingPosition, stackingRotation);
        }
        else
        {
            RemoveBox();
            base.Interact(activatingObject);
        }
    }

    public override bool CanInteract(GameObject activatingObject)
    {
        if (!activatingObject.CompareTag("Player"))
            return false;
        bool pickUp = base.CanInteract(activatingObject);
        bool stack = !isHeld && activatingObject.GetComponent<InteractableManager>().IsCarryingTag("Box");
        ShowPlacement = stack;
        if (ShowPlacement)
            UpdateStackingPosition();
        return (pickUp || stack) && !hasBoxAbove;
    }

    [Command(requiresAuthority = false)]
    private void StackBox(StackableBox newBox, Vector3 position, Quaternion rotation)
    {
        newBox.boxBelow = this;
        newBox.IsKinematic = true;
        newBox.RPCMoveObject(position, rotation);
        newBox.Drop();
        hasBoxAbove = true;
        IsKinematic = true;
    }

    [Command(requiresAuthority = false)]
    private void RemoveBox()
    {
        IsKinematic = false;
        if (boxBelow != null)
        {
            if (hasBoxAbove)
                return;
            boxBelow.RemoveBox();
            boxBelow.hasBoxAbove = false;
            boxBelow = null;
        }
    }

    private void UpdateStackingPosition()
    {
        Vector3 lookingAt = InteractionChecker.latestHit.point;
        Vector3 positionDifference = transform.position - lookingAt;
        positionDifference *= offsetFactor;

        int xOffset = Mathf.Clamp((int)(positionDifference.x), -1, 1);
        int zOffset = Mathf.Clamp((int)(positionDifference.z), -1, 1);

        Vector3 localPosition = new Vector3(offset * xOffset, stackingTransform.localPosition.y, offset * zOffset);
        stackingTransform.localPosition = localPosition;
        stackingPosition = stackingTransform.position;
        stackingRotation = stackingTransform.rotation;
    }
}
