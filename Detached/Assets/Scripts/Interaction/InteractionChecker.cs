using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionChecker : NetworkBehaviour
{

    [SerializeField] private float interactionDistance;
    [SerializeField] private GameObject player;
    private LayerMask targetMask;
    private bool interacting = false;
    private InteractableManager interactableManager;

    private bool allowInteraction = true;
    public bool AllowInteraction
    {
        get => allowInteraction;
        set 
        {  
            if(value)
                StartCoroutine(TimedAllowInteractions());
            else
                allowInteraction = false;
        }
    }

    private IEnumerator TimedAllowInteractions()
    {
        yield return new WaitForSeconds(0.25f);
        allowInteraction = true;
    }

    private bool ray1Hit, ray2Hit;

    [Header("Debug values")]
    [Range(-1f, 0f)]
    [SerializeField] private float debugRayAngle = -0.2f;

    private void Awake()
    {
        targetMask = LayerMask.GetMask("Interactable");
    }

    private void Start()
    {
        NetworkClient.localPlayer.TryGetComponent(out interactableManager); //TODO kolla så att det faktiskt fungerar
        player = interactableManager.gameObject;
    }

    private void Update()
    {
        if (!interacting)
            interacting = Input.GetKeyDown(KeyCode.E);
        else
            interacting = !Input.GetKeyUp(KeyCode.E);
    }

    private void FixedUpdate()
    {
        if (!allowInteraction)
            return;
        ray1Hit = ray2Hit = false;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, targetMask))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (CanInteractWith(hitObject))
            {
                ray1Hit = true;
                HighlightObject(hitObject);
                AttemptInteraction(hitObject);
            }
        }
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.yellow);

        var debugDirection = (transform.forward + transform.up * debugRayAngle).normalized;
        if (Physics.Raycast(transform.position, debugDirection, out RaycastHit hit2, interactionDistance, targetMask))
        {
            GameObject hitObject = hit2.transform.gameObject;
            if (CanInteractWith(hitObject))
            {
                ray2Hit = true;
                HighlightObject(hitObject);
                AttemptInteraction(hitObject);
            }
            
        }
        Debug.DrawRay(transform.position, debugDirection * interactionDistance, Color.yellow);
        
        if (!ray1Hit && !ray2Hit)
            AttemptDropItem();
    }

    private bool CanInteractWith(GameObject hitObject)
    {
        if (hitObject.CompareTag("Limb"))
            return true;
        else if (hitObject.TryGetComponent(out IInteractable interactable))
            return interactable.CanInteract(player);
        else
            return hitObject.GetComponentInChildren<IInteractable>().CanInteract(player);
    }

    private void HighlightObject(GameObject hitObject) => hitObject.GetComponent<HighlightObject>().DurationHighlight();

    private void AttemptInteraction(GameObject hitObject)
    {
        if (player == null)
            player = NetworkClient.localPlayer.gameObject;

        if (interacting)
        {
            if (hitObject.CompareTag("Limb"))
            {
                Debug.Log("Hit limb");
                hitObject.GetComponent<SceneObjectItemManager>().TryPickUp();
            }
            else if (hitObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(player);
            }
            else
            {
                hitObject.GetComponentInChildren<IInteractable>().Interact(player);
            }
                
            interacting = false;
        }
    }

    private void AttemptDropItem()
    {
        if (interacting && interactableManager.IsCarryingItem)
        {
            interactableManager.AttemptDropItem();
            interacting = false;
        }           
    }
}
