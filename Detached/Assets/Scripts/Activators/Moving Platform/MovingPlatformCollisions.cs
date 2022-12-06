using UnityEngine;

public class MovingPlatformCollisions : MonoBehaviour
{
    private MovingPlatformActivator activator;
    void Start()
    {
        activator = GetComponentInParent<MovingPlatformActivator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (HasRigidbody(other, out GameObject gameObject))
            activator.Attach(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (HasRigidbody(other, out GameObject gameObject))
            activator.Detach(gameObject);
    }

    private bool HasRigidbody(Collider other, out GameObject gameObject)
    {
        gameObject = null;
        Rigidbody rigidbody = other.GetComponent<Rigidbody>();
        if (rigidbody != null )
            gameObject = rigidbody.gameObject;
        else
        {
            rigidbody = other.GetComponentInParent<Rigidbody>();
            if (rigidbody != null)
                gameObject = rigidbody.gameObject;
        }

        return gameObject != null;
    }
}
