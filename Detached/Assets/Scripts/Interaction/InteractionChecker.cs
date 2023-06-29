using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionChecker : NetworkBehaviour
{
    public static RaycastHit latestHit;

    [SerializeField] private float interactionDistance;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Transform sourceTransform;
    private int interactableLayer;
    private bool interacting = false;
    private InteractableManager interactableManager;

    private GameObject previousObject;
    private IInteractable objectInteractable;
    private HighlightObject objectHighlighter;

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

    private void Start()
    {
        targetLayers = LayerMask.GetMask("Interactable", "Default", "Ground");
        interactableLayer = LayerMask.NameToLayer("Interactable");
        NetworkClient.localPlayer.TryGetComponent(out interactableManager);
        player = interactableManager.gameObject;
        sourceTransform = transform;
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
        bool hit = Physics.Raycast(sourceTransform.position, transform.forward, out RaycastHit raycastHit, interactionDistance, targetLayers, QueryTriggerInteraction.Ignore);
        if (!hit || !IsValidObject(raycastHit.transform.gameObject))
        {
            AttemptDropItem();
            return;
        }
        
        PerformHit(raycastHit.transform.gameObject);
        latestHit = raycastHit;
        previousObject = raycastHit.transform.gameObject;
        
        Debug.DrawRay(sourceTransform.position, transform.forward * interactionDistance, Color.yellow); 
    }

    private bool CanInteractWith(GameObject hitObject)
    {
        if (hitObject.CompareTag("Limb"))
            return true;

        if (hitObject != previousObject)
        {
            if (hitObject.TryGetComponent(out IInteractable interactable))
                objectInteractable = interactable;
            else
                objectInteractable = hitObject.GetComponentInParent<IInteractable>();
        }
        return objectInteractable.CanInteract(player);
    }

    private void HighlightObject(GameObject hitObject)
    {
        if (hitObject != previousObject)
        {
            if (hitObject.TryGetComponent(out HighlightObject highlighter))
                objectHighlighter = highlighter;
            else
                objectHighlighter = hitObject.GetComponentInParent<HighlightObject>();
        }
            
        objectHighlighter.DurationHighlight();
    }

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
            else
                objectInteractable.Interact(player);
                
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

    private bool IsValidObject(GameObject hitObject)
    {
        return hitObject != null && hitObject.layer == interactableLayer && CanInteractWith(hitObject);
    }

    private void PerformHit(GameObject hitObject)
    {
        HighlightObject(hitObject);
        AttemptInteraction(hitObject);
    }
}
