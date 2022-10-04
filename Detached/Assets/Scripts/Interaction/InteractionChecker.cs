using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionChecker : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    private LayerMask targetMask;

    private void Awake()
    {
        targetMask = LayerMask.GetMask("Interactable");
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, targetMask))
        {
            if (Input.GetKeyDown(KeyCode.E))
                hit.transform.gameObject.GetComponent<IInteractable>().Interact();
            var highlightThinger = hit.transform.gameObject.GetComponent<HighlightObject>();
            highlightThinger.HighlightItem();
        }
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.yellow);
    }
}
