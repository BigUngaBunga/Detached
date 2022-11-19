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
    [SerializeField] private float targetDistance;
    [Header("Moving platform information")]
    [SerializeField] private GameObject platform;
    [SerializeField] private bool goingBackwards;
    [SerializeField] private TrackNode targetNode;
    [SyncVar][SerializeField] private List<GameObject> connectedObjects = new List<GameObject>();

    private TrackActivator track;
    private Transform Transform => platform.transform;

    private float Speed => platformSpeed * Time.deltaTime;
    [SyncVar] private bool isMoving;
    //private readonly List<GameObject> connectedObjects = new List<GameObject>();
    //[SyncVar] private readonly Dictionary<GameObject, int> connectedObjects = new Dictionary<GameObject, int>();

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
            
            foreach (var gameObject in connectedObjects)
            {
                if (gameObject != null)
                    gameObject.transform.position += direction * Speed;//TODO använd krafter istället
            }
            Transform.position += direction * Speed;
        }
        else if (isActivated)
            Invoke(nameof(PickNextStop), stopWaitTime);
    }

    [Server]
    public void Attach(GameObject connectingObject)
    {
        if (connectedObjects.Contains(connectingObject))
            return;
        else
        {
            EditConnectedObjects(connectingObject, true);
            Debug.Log("Attached " + connectingObject.name);
        }

    }

    [Server]
    public void Detach(GameObject connectingObject)
    {
        if (!connectedObjects.Contains(connectingObject))
            return;
        EditConnectedObjects(connectingObject, false);
        Debug.Log("Detached " + connectingObject.name);
    }

    [ClientRpc]
    private void EditConnectedObjects(GameObject gameObject, bool add)
    {
        if (add)
            connectedObjects.Add(gameObject);
        else
            connectedObjects.Remove(gameObject);
    }
}
