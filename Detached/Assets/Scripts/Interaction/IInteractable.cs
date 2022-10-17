using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject activatingObject);

    bool CanInteract(GameObject activatingObject);
}
