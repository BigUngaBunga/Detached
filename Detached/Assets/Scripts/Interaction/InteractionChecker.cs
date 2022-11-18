using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionChecker : NetworkBehaviour
{
    public static RaycastHit latestHit;

    [SerializeField] private float interactionDistance;
    [SerializeField] private GameObject player;
    [SerializeField] private int targetLayer = 15;
    [SerializeField] private Transform sourceTransform;
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

    private bool ray1Hit, ray2Hit;

    [Header("Debug values")]
    [Range(-1f, 0f)]
    [SerializeField] private float debugRayAngle = -0.2f;
    [SerializeField] private bool useDebugRay = false;

    private void Start()
    {
        NetworkClient.localPlayer.TryGetComponent(out interactableManager); //TODO kolla så att det faktiskt fungerar
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
        ray1Hit = ray2Hit = false;
        var hits = Physics.RaycastAll(sourceTransform.position, transform.forward, interactionDistance);
        GameObject closestHit = GetClosestHit(hits);
        ray1Hit = EvaluateHit(closestHit);
        previousObject = closestHit;
        Debug.DrawRay(sourceTransform.position, transform.forward * interactionDistance, Color.yellow);

        //DEBUG
        if (useDebugRay)
        {
            Vector3 debugDirection = (transform.forward + transform.up * debugRayAngle).normalized;
            hits = Physics.RaycastAll(transform.position, debugDirection, interactionDistance);
            ray2Hit = EvaluateHit(GetClosestHit(hits));
            Debug.DrawRay(transform.position, debugDirection * interactionDistance, Color.yellow);
        }

        if (!ray1Hit && !ray2Hit)
            AttemptDropItem();
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

    private bool EvaluateHit(GameObject hitObejct)
    {
        if (hitObejct != null && hitObejct.layer == targetLayer && CanInteractWith(hitObejct))
        {
            HighlightObject(hitObejct);
            AttemptInteraction(hitObejct);
            return true;
        }
        return false;
    }

    private GameObject GetClosestHit(RaycastHit[] hits)
    {
        float bestDistance = float.MaxValue;
        int bestIndex = -1;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == null || hits[i].collider.isTrigger)
                continue;
            float distance = Vector3.Distance(sourceTransform.position, hits[i].point);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestIndex = i;
            }
        }
        if (bestIndex != -1)
        {
            latestHit = hits[bestIndex];
            return hits[bestIndex].transform.gameObject;
        }
        else
            return null;
    }
}
