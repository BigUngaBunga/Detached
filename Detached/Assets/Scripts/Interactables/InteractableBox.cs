using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class InteractableBox : PickUpInteractable
{
    [SyncVar] private bool hasBoxAbove;
    private InteractableBox boxBelow;
    private new Rigidbody rigidbody;
    private bool isKinematic;
    private bool IsKinematic 
    { 
        get { return isKinematic; } 
        set { IsKinematic = value; 
                rigidbody.isKinematic = IsKinematic; } 
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void Interact(GameObject activatingObject)
    {
        var itemManager = activatingObject.GetComponent<InteractableManager>();
        if (itemManager.IsCarryingTag("box"))
        {
            itemManager.AttemptDropItem(out GameObject box);
            StackBox(box.GetComponent<InteractableBox>());
        }
        else
        {
            IsKinematic = false;
            RemoveBox();
            base.Interact(activatingObject);
        }
    }

    public override bool CanInteract(GameObject activatingObject)
    {
        return base.CanInteract(activatingObject) && !hasBoxAbove 
            || !hasBoxAbove && activatingObject.GetComponent<InteractableManager>().IsCarryingTag("box");
    }

    private void StackBox(InteractableBox newBox)
    {
        newBox.boxBelow = this;
        newBox.IsKinematic = true;
        hasBoxAbove = true;
        IsKinematic = true;
    }

    private void RemoveBox()
    {
        
        if (boxBelow != null)
        {
            if (hasBoxAbove)
                return;
            boxBelow.RemoveBox();
            boxBelow.hasBoxAbove = false;
            boxBelow = null;
        }
        else
        {
            IsKinematic = false;
        }
    }
}
