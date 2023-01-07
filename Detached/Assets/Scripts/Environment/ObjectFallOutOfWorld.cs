using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFallOutOfWorld : NetworkBehaviour
{
    private int fellOutCounter = 0;
    [SyncVar] private Vector3 startPosition;
    [SerializeField] private Vector3 safeLocation;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private int fallOutMaxAmount = 3;


    private void Start()
    {
        startPosition = gameObject.transform.position;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (Physics.CheckSphere(transform.position, groundCheckRadius, groundMask))
        {
            safeLocation = transform.position;
        }
    }

    public void HandleFallOutOfWorld()
    {

        if (safeLocation != null && safeLocation != Vector3.zero && isServer)
        {
            if (fellOutCounter < fallOutMaxAmount)
            {
                fellOutCounter++;
                RpcUpdatePosition(safeLocation);
            }
            else
            {
                fellOutCounter = 0;
                RpcUpdatePosition(startPosition);
            }
        }
    }

    public void RpcUpdatePosition(Vector3 safeLocation)
    {
        gameObject.transform.position = safeLocation + new Vector3(0, 2, 0); //So it dosne't collide with ground or other limbs
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
