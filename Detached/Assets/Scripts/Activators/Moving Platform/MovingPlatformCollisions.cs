using UnityEngine;

public class MovingPlatformCollisions : MonoBehaviour
{
    private MovingPlatformActivator activator;
    void Start()
    {
        activator = GetComponentInParent<MovingPlatformActivator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            activator.Attach(collision.gameObject);
        }
            
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null)
            activator.Detach(collision.gameObject);
    }
}
