using System.Collections.Generic;
using UnityEngine;

public class InteractionLimiter : MonoBehaviour
{
    [SerializeField] private List<GameObject> interactors = new List<GameObject>();
    public bool ContainsObject(GameObject gameObject) => interactors.Contains(gameObject);

    private void OnTriggerEnter(Collider other)
    {
        HandlePlayer(other, true);
        HandleArm(other, true);
    }

    private void OnTriggerExit(Collider other)
    {

        HandlePlayer(other, false);
        HandleArm(other, false);
    }

    private void HandlePlayer(Collider other, bool add)
    {
        if (other.gameObject.CompareTag("Torso"))
        {
            var playerObject = other.gameObject.GetComponentInParent<CharacterControl>().gameObject;
            if (add)
                AddIfNotAdded(playerObject);
            else
                RemoveIfAdded(playerObject);
        }
    }

    private void HandleArm(Collider other, bool add)
    {
        if (other.gameObject.CompareTag("Arm"))
        {
            var armObject = other.gameObject.GetComponentInParent<LimbMovement>().gameObject;
            
            if (add)
                AddIfNotAdded(armObject);
            else
                RemoveIfAdded(armObject);
        }
    }

    private void AddIfNotAdded(GameObject gameObject)
    {
        if (gameObject != null && !interactors.Contains(gameObject))
            interactors.Add(gameObject);
    }

    private void RemoveIfAdded(GameObject gameObject)
    {
        if (interactors.Contains(gameObject))
            interactors.Remove(gameObject);
    }

}
