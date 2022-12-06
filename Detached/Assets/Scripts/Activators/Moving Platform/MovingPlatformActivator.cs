using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class MovingPlatformActivator : Activator
{
    [Header("Moving platform variables")]
    [SerializeField] private float stopWaitTime;
    [SerializeField] private float platformSpeed;
    [Range(0f, 1f)]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float targetDistance;
    [Header("Moving platform information")]
    [SerializeField] private GameObject platform;
    [SerializeField] private bool goingBackwards;
    [SerializeField] private TrackNode targetNode;
    [SerializeField] private List<GameObject> connectedObjects = new List<GameObject>();

    private TrackActivator track;
    private Transform Transform => platform.transform;

    private float Speed => platformSpeed * Time.deltaTime;
    [SyncVar] private bool isMoving;

    protected override void Start()
    {
        base.Start();
        track = GetComponent<TrackActivator>();
        Transform.position = track.GetStartPosition();
        Transform.rotation = Quaternion.LookRotation(GetDirectionTo(track.PeekNextNode(false).Position));
    }

    private bool IsCloseToTarget(Vector3 target) => Vector3.Distance(Transform.position, target) <= targetDistance;
    private Vector3 GetDirectionTo(Vector3 target) => (target - Transform.position).normalized;

    protected override void Activate()
    {
        base.Activate();
        RPCPickNextStop();
    }


    private void PickNextStop()
    {
        if (isMoving)
            return;
        isMoving = true;
        targetNode = track.GetNextNode(ref goingBackwards);
    }

    [ClientRpc]
    private void RPCPickNextStop() => PickNextStop();

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
            Debug.DrawRay(Transform.position, direction * 10f, Color.red);

            UpdatePlatformTransform(direction);
            foreach (var gameObject in connectedObjects)
            {
                if (gameObject != null)
                {
                    gameObject.transform.position += direction * Speed;//TODO använd krafter istället
                    //gameObject.transform.rotation = Transform.rotation;
                }
            }
            
        }
        else if (isActivated)
            Invoke(nameof(PickNextStop), stopWaitTime);
    }

    private void UpdatePlatformTransform(Vector3 direction)
    {
        Transform.position += direction * Speed;

        int goingBackwardsModifier = goingBackwards ? -1 : 1;
        Quaternion targetRotation = Quaternion.LookRotation(GetDirectionTo(targetNode.Position) * goingBackwardsModifier);
        Transform.rotation = Quaternion.Lerp(Transform.rotation, targetRotation, rotationSpeed);
        
    }

    [Server]
    public void Attach(GameObject connectingObject)
    {
        EditConnectedObjects(connectingObject, true);
        Debug.Log("Attached " + connectingObject.name);
    }

    [Server]
    public void Detach(GameObject connectingObject)
    {
        EditConnectedObjects(connectingObject, false);
        Debug.Log("Detached " + connectingObject.name);
    }

    [ClientRpc]
    private void EditConnectedObjects(GameObject gameObject, bool add)
    {
        if (add)
        {
            if (!connectedObjects.Contains(gameObject))
                connectedObjects.Add(gameObject);
        }
        else if(connectedObjects.Contains(gameObject))
        {
            if (targetNode != null)
                    gameObject.GetComponent<Rigidbody>().velocity = platformSpeed * GetDirectionTo(targetNode.Position);
            connectedObjects.Remove(gameObject);
        }
    }
}
