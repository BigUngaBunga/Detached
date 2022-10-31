using Mirror;
using UnityEngine;

public class Unlock : NetworkBehaviour, IInteractable
{
    enum LockType { Door, Activator}

    [SerializeField] private GameObject lockedObject;
    private LockType lockType;
    private Activator activator;
    private Goal door;


    void Start()
    {
        if (lockedObject.TryGetComponent(out Activator activator))
        {
            lockType = LockType.Activator;
            activator.locked = true;
            this.activator = activator;
        }
        else if (lockedObject.TryGetComponent(out Goal door))
        {
            lockType=LockType.Door;
            door.isLocked = true;
            this.door = door;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            UnlockObject(other.gameObject);
        }

    }

    [Command(requiresAuthority = false)]
    private void UnlockObject(GameObject key)
    {
        switch (lockType)
        {
            case LockType.Door:
                door.isLocked = false;
                door.EvaluateVictory();
                break;
            case LockType.Activator:
                activator.locked = false;
                activator.ReevaluateActivation();
                break;
            default:
                break;
        }
        gameObject.GetComponent<HighlightObject>().ForceStopHighlight();
        NetworkServer.Destroy(key);
        NetworkServer.Destroy(gameObject);
    }

    public void Interact(GameObject activatingObject)
    {
        var manager = activatingObject.GetComponent<InteractableManager>();
        if (manager.IsCarryingTag("Key") && manager.AttemptDropItem(out GameObject key))
            UnlockObject(key);
    }

    public bool CanInteract(GameObject activatingObject) => activatingObject.GetComponent<InteractableManager>().IsCarryingTag("Key");
}
