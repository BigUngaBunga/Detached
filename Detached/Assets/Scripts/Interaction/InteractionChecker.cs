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
    private bool Interacting => interacting && allowInteraction;
    private InteractableManager interactableManager;

    private bool ray1Hit, ray2Hit;

    [Header("Debug values")]
    [SerializeField] private bool allowInteraction = false;
    [Range(-1f, 0f)]
    [SerializeField] private float debugRayAngle = -0.2f;

    private void Awake()
    {
        //player.TryGetComponent(out interactableManager);
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
        else if (interacting)
            interacting = !Input.GetKeyUp(KeyCode.E);
    }

    private void FixedUpdate()
    {
        ray1Hit = ray2Hit = false;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, targetMask))
        {
            ray1Hit = true;
            HighlightObject(hit.transform.gameObject);
            AttemptInteraction(hit.transform.gameObject);
        }
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.yellow);

        var debugDirection = (transform.forward + transform.up * debugRayAngle).normalized;
        if (Physics.Raycast(transform.position, debugDirection, out RaycastHit hit2, interactionDistance, targetMask))
        {
            ray2Hit = true;
            HighlightObject(hit2.transform.gameObject);
            AttemptInteraction(hit2.transform.gameObject);
        }
        Debug.DrawRay(transform.position, debugDirection * interactionDistance, Color.yellow);
        
        if (!ray1Hit && !ray2Hit)
            AttemptDropItem();
    }

    private void HighlightObject(GameObject hitObject) => hitObject.GetComponent<HighlightObject>().DurationHighlight();

    private void AttemptInteraction(GameObject hitObject)
    {//TODO fixa så att båda spelare läggs till korrekt i skriptet
        if (player == null)
            player = NetworkClient.localPlayer.gameObject;

        if (Interacting && allowInteraction)
        {
            if (hitObject.CompareTag("Limb"))
            {
                Debug.Log("Hit limb");
                hitObject.GetComponent<SceneObjectItemManager>().TryPickUp();
            }
            else if (hitObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(player);
                hitObject.GetComponent<HighlightObject>().UpdateHighlight();
            }

            else
            {
                hitObject.GetComponentInChildren<IInteractable>().Interact(player);
                hitObject.GetComponent<HighlightObject>().UpdateHighlight();
            }
                
            interacting = false;
        }
    }

    private void AttemptDropItem()
    {
        if (Interacting)
        {
            interactableManager.AttemptDropItem();
        }
            
    }
}
