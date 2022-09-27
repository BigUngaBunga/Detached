using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovingPlatformActivator : Activator
{
    [Header("Moving platform fields")]
    [SerializeField] private float platformSpeed;
    [SerializeField] private float targetDistance;
    [SerializeField] private bool goingBackwards;
    [SerializeField] private TrackActivator track;
    [SerializeField] private TrackNode targetNode;

    private float Speed => platformSpeed * Time.deltaTime;
    private bool isMoving;
    private readonly List<GameObject> connectedObjects = new List<GameObject>();

    private bool IsCloseToTarget(Vector3 target) => Vector3.Distance(transform.position, target) <= targetDistance;
    private Vector3 GetDirectionTo(Vector3 target) => (target - transform.position).normalized;

    protected override void Activate()
    {
        base.Activate();
        PickNextStop();
    }

    private void PickNextStop()
    {
        if (isMoving)
            return;
        StartCoroutine(MoveToNextStop());
    }

    private IEnumerator MoveToNextStop()
    {
        isMoving = true;

        do
        {
            targetNode = track.GetNextNode(ref goingBackwards);
            Vector3 direction = GetDirectionTo(targetNode.Position);
            while (!IsCloseToTarget(targetNode.Position))
            {
                transform.position += direction * Speed;
                foreach (var gameObject in connectedObjects)
                    gameObject.transform.position += direction * Speed;//TODO använd krafter istället

                yield return new WaitForFixedUpdate();
            }
            yield return null;
        } while (!targetNode.IsStop);

        isMoving = false;
        if (IsActivated)
            Invoke(nameof(PickNextStop), 0.1f);

        yield return null;
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

    private void Start()
    {
        transform.position = track.GetStartPosition();
    }
}
