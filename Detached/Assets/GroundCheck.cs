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
    private float raycastLength = 0.2f;
    private bool hitThisLoop = false;
    Vector3[] corners = new Vector3[4];



    public bool IsGrounded => isGrounded;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    void Update()
    {
        // Get the bounds of the box collider
        Bounds colliderBounds = boxCollider.bounds;
        corners = GetSquareCorners(transform.position, transform.rotation, boxCollider.size);

        foreach (Vector3 corner in corners)
        {
            Vector3 direction = Vector3.down; // direction to cast the ray in

            RaycastHit hit;
            if (Physics.Raycast(corner, direction, out hit, raycastLength, groundMask, collideWithTrigger))
            {
                hitThisLoop = true;
                break;
            }
        }

        if (hitThisLoop) isGrounded = true;
        else isGrounded = false;

        hitThisLoop = false;
    }
    private void OnDrawGizmos()
    {
        if (isGrounded) Gizmos.color = Color.magenta;
        else Gizmos.color = Color.yellow;

        Gizmos.DrawCube(transform.position, transform.gameObject.GetComponent<BoxCollider>().size);
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], new Vector3(corners[i].x, corners[i].y - raycastLength, corners[i].z));
        }
    }

    Vector3[] GetSquareCorners(Vector3 position, Quaternion rotation, Vector3 size)
    {
        // create an array to store the corner positions
        Vector3[] corners = new Vector3[4];

        // calculate the local space positions of the corners
        Vector3 halfSize = size * 0.5f;
        Vector3 bottomLeft = new Vector3(-halfSize.x, -halfSize.y, 0);
        Vector3 bottomRight = new Vector3(halfSize.x, -halfSize.y, 0);
        Vector3 topLeft = new Vector3(-halfSize.x, halfSize.y, 0);
        Vector3 topRight = new Vector3(halfSize.x, halfSize.y, 0);

        // transform the corner positions to world space
        corners[0] = position + rotation * bottomLeft;
        corners[1] = position + rotation * bottomRight;
        corners[2] = position + rotation * topLeft;
        corners[3] = position + rotation * topRight;

        return corners;
    }

}
