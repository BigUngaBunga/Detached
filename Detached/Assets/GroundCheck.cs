using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header ("GroundCheck")]
    [SerializeField] private LayerMask groundMask;

    private bool isGrounded = false;
    private BoxCollider boxCollider;
    private QueryTriggerInteraction collideWithTrigger = QueryTriggerInteraction.Ignore;


    public bool IsGrounded => isGrounded;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    void Update()
    {
        // Get the bounds of the box collider
        Bounds colliderBounds = boxCollider.bounds;

        // Expand the bounds slightly to account for the object's height
        colliderBounds.Expand(new Vector3(0, -0.5f, 0));
        if (Physics.CheckBox(colliderBounds.center, colliderBounds.extents, Quaternion.identity, groundMask, collideWithTrigger)) isGrounded = true;
        else isGrounded = false;
    }
    private void OnDrawGizmos()
    {
        if (isGrounded) Gizmos.color = Color.magenta;
        else Gizmos.color = Color.yellow;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawCube(Vector3.zero, transform.gameObject.GetComponent<BoxCollider>().size);
    }
}
