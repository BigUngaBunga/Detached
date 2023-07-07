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
    //DEBUG
    [SerializeField] private Vector3 positionDifferenceVector;
    [SerializeField] private Vector3 offsetVector = Vector3.zero;
    //DEBUG

    [SerializeField] private Transform stackingTransform;
    [SyncVar] private Vector3 stackingPosition;
    [SyncVar] private Quaternion stackingRotation;
    [SyncVar] private StackableBox boxAbove;
    [SyncVar] private StackableBox boxBelow;
    private bool HasBoxBelow => boxBelow != null;
    private bool HasBoxAbove => boxAbove != null;
    private HighlightObject highlighter;
    
    [SerializeField] private float offsetFactor = 1f;
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
        destroyEvent.AddListener(HandleRemoval);
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
        if (true)
        {

        }
        return (pickUp || stack) && !HasBoxAbove;
    }

    [Command(requiresAuthority = false)]
    private void StackBox(StackableBox newBox, Vector3 position, Quaternion rotation)
    {
        newBox.boxBelow = this;
        newBox.IsKinematic = true;
        newBox.RPCMoveObject(position, rotation);
        newBox.Drop();
        boxAbove = newBox;
        IsKinematic = true;
    }

    [Command(requiresAuthority = false)]
    private void RemoveBox()
    {
        HandleRemoval();
    }

    private void HandleRemoval()
    {
        IsKinematic = false;
        if (HasBoxAbove)
            boxAbove.HandleRemoval();
        if (HasBoxBelow)
        {
            if (!boxBelow.HasBoxBelow)
                boxBelow.IsKinematic = false;
            boxBelow.boxAbove = null;
            boxBelow = null;
        }
    }

    private void UpdateStackingPosition()
    {
        Vector3 lookingAt = InteractionChecker.latestHit.point;
        Vector3 positionDifference = transform.position - lookingAt;
        positionDifference *= offsetFactor;
        positionDifference = Quaternion.Euler(-transform.rotation.eulerAngles) * positionDifference;
        int xOffset = Mathf.Clamp((int)(positionDifference.x), -1, 1);
        int zOffset = Mathf.Clamp((int)(positionDifference.z), -1, 1);

        positionDifferenceVector = positionDifference;
        offsetVector.x = xOffset;
        offsetVector.z = zOffset;

        Vector3 localPosition = new Vector3(offset * xOffset, stackingTransform.localPosition.y, offset * zOffset);
        stackingTransform.localPosition = localPosition;
        stackingPosition = stackingTransform.position;
        stackingRotation = stackingTransform.rotation;
    }
}
