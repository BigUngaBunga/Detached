using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class MovingPlatformActivator : Activator
{
    [Header("Moving platform fields")]
    [SerializeField] private GameObject platform;
    [SerializeField] private float platformSpeed;
    [SerializeField] private float targetDistance;
    [SerializeField] private bool goingBackwards;
    [SerializeField] private TrackNode targetNode;

    private TrackActivator track;
    private Transform Transform => platform.transform;

    private float Speed => platformSpeed * Time.deltaTime;
    private bool isMoving;
    private readonly List<GameObject> connectedObjects = new List<GameObject>();
    
    protected override void Start()
    {
        base.Start();
        track = GetComponent<TrackActivator>();
        Transform.position = track.GetStartPosition();
    }

    private bool IsCloseToTarget(Vector3 target) => Vector3.Distance(Transform.position, target) <= targetDistance;
    private Vector3 GetDirectionTo(Vector3 target) => (target - Transform.position).normalized;

    protected override void Activate()
    {
        base.Activate();
        PickNextStop();
    }

    private void PickNextStop()
    {
        //if (isMoving)
        //    return;
        //StartCoroutine(MoveToNextStop());
        isMoving = true;
        targetNode = track.GetNextNode(ref goingBackwards);
    }

    //TODO gör att den återvänder vid krock med vägg||platform

    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (IsCloseToTarget(targetNode.Position))
            {
                if (targetNode.IsStop)
                    isMoving = false;
                else
                    targetNode = track.GetNextNode(ref goingBackwards);
            }
            
            Vector3 direction = GetDirectionTo(targetNode.Position);

            Transform.position += direction * Speed;
            foreach (var gameObject in connectedObjects)
            {
                if (gameObject != null)
                    gameObject.transform.position += direction * Speed;//TODO använd krafter istället
            }
        }
        else if (isActivated)
            Invoke(nameof(PickNextStop), 0.1f);
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
                Transform.position += direction * Speed;
                foreach (var gameObject in connectedObjects)
                {
                    if (gameObject != null)
                        gameObject.transform.position += direction * Speed;//TODO använd krafter istället
                }
                    

                yield return new WaitForFixedUpdate();
            }
            yield return null;
        } while (!targetNode.IsStop);

        isMoving = false;
        if (isActivated)
            Invoke(nameof(PickNextStop), 0.1f);

        yield return null;
    }

    public void Attach(GameObject connectingObject) => connectedObjects.Add(connectingObject);
    public void Detach(GameObject connectingObject) => connectedObjects.Remove(connectingObject);
}
