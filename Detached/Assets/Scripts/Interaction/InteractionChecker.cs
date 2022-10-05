using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionChecker : MonoBehaviour
{

    [SerializeField] private float interactionDistance;
    [SerializeField] private GameObject player;
    private LayerMask targetMask;
    private bool interacting = false;

    [Header("Debug values")]
    [SerializeField] private bool allowInteraction = false;
    [Range(-1f, 0f)]
    [SerializeField] private float debugRayAngle = -0.2f;

    private void Awake()
    {
        targetMask = LayerMask.GetMask("Interactable");
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
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, targetMask))
        {
            HighlightObject(hit.transform.gameObject);
            AttemptInteraction(hit.transform.gameObject);
        }
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.yellow);

        var debugDirection = (transform.forward + transform.up * debugRayAngle).normalized;
        if (Physics.Raycast(transform.position, debugDirection, out RaycastHit hit2, interactionDistance, targetMask))
        {
            HighlightObject(hit2.transform.gameObject);
            AttemptInteraction(hit2.transform.gameObject);
        }
        Debug.DrawRay(transform.position, debugDirection * interactionDistance, Color.yellow);
    }

    private void HighlightObject(GameObject hitObject) => hitObject.GetComponent<HighlightObject>().DurationHighlight();

    private void AttemptInteraction(GameObject hitObject)
    {
        if (interacting && allowInteraction)
        {
            if (hitObject.CompareTag("Limb"))
            {
                Debug.Log("Hit limb");
                hitObject.GetComponent<SceneObjectItemManager>().TryPickUp();
            }
            else if (hitObject.TryGetComponent(out IInteractable interactable))
                interactable.Interact(player);
            else
                hitObject.GetComponentInChildren<IInteractable>().Interact(player);
            interacting = false;
        }
    }
}
