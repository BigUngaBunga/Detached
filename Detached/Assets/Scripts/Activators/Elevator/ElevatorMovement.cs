using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMovement : MonoBehaviour
{
    private readonly List<GameObject> connectedObjects = new List<GameObject>();

    public void Move(Vector3 direction)
    {
        transform.position += direction;
        foreach (var connectedObject in connectedObjects)
            connectedObject.transform.position += direction;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entered platform collider");
        if (collision.rigidbody != null)
            connectedObjects.Add(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Left platform collider");
        if (collision.rigidbody != null)
            connectedObjects.Remove(collision.gameObject);
    }
}
