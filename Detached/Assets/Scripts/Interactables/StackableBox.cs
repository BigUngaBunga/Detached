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
    [SerializeField]private Transform stackingPosition;
    [SyncVar] private bool hasBoxAbove;
    private StackableBox boxBelow;
    private new Rigidbody rigidbody;
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
            stackingPosition.gameObject.SetActive(value);
            if (showPlacement != value)
                highlighter.UpdateRenderers();
            showPlacement = value;
            
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        highlighter = GetComponent<HighlightObject>();
        ShowPlacement = false;
        offset = stackingPosition.localPosition.z;
    }

    public override void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();
        if (itemManager.IsCarryingTag("Box"))
        {
            itemManager.AttemptDropItem(out GameObject box);
            StackBox(box.GetComponent<StackableBox>());
        }
        else
        {
            RemoveBox();
            base.Interact(activatingObject);
        }
    }

    public override bool CanInteract(GameObject activatingObject)
    {
        bool pickUp = base.CanInteract(activatingObject) && !hasBoxAbove;
        bool stack = !hasBoxAbove && activatingObject.GetComponent<InteractableManager>().IsCarryingTag("Box");
        ShowPlacement = stack;
        if (ShowPlacement)
            UpdateStackingPosition();
        return pickUp || stack;
    }

    private void StackBox(StackableBox newBox)
    {
        newBox.transform.position = stackingPosition.position;
        newBox.transform.rotation = stackingPosition.rotation;
        newBox.boxBelow = this;
        newBox.IsKinematic = true;
        hasBoxAbove = true;
        IsKinematic = true;
    }

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

        int xOffset = Mathf.Clamp((int)positionDifference.x, -1, 1);
        int zOffset = Mathf.Clamp((int)positionDifference.z, -1, 1);

        Vector3 localPosition = new Vector3(offset * xOffset, stackingPosition.localPosition.y, offset * zOffset);
        stackingPosition.localPosition = localPosition;         
    }
}
