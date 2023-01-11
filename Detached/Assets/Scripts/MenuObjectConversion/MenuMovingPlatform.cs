using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMovingPlatform : MonoBehaviour
{
    [Header("Moving platform variables")]
    [SerializeField] private float stopWaitTime;
    [SerializeField] private float platformSpeed;
    [Range(0f, 1f)]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float targetDistance;
    [Header("Moving platform information")]
    [SerializeField] private GameObject train;
    [SerializeField] private GameObject platform;
    [SerializeField] private bool goingBackwards;
    [SerializeField] private TrackNode targetNode;
    private Quaternion initialRotation;

    private MenuTrack track;
    private Transform Transform => platform.transform;
    private Transform Train => train.transform;

    private float Speed => platformSpeed * Time.deltaTime;
    private bool isMoving;

    protected void Start()
    {
        track = GetComponent<MenuTrack>();
        PickNextStop();
        Transform.position = track.GetStartPosition();
        initialRotation = Quaternion.Euler(train.transform.localEulerAngles);
        Train.rotation = Quaternion.LookRotation(GetDirectionTo(track.PeekNextNode(false).Position)) * initialRotation;
    }

    private bool IsCloseToTarget(Vector3 target) => Vector3.Distance(Transform.position, target) <= targetDistance;
    private Vector3 GetDirectionTo(Vector3 target) => (target - Transform.position).normalized;

    private void PickNextStop()
    {
        if (isMoving)
            return;
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
            Debug.DrawRay(Transform.position, direction * 10f, Color.red);
            UpdateTrainTransform(direction);
        }
        else
            Invoke(nameof(PickNextStop), stopWaitTime);
    }

    private void UpdateTrainTransform(Vector3 direction)
    {
        Transform.position += direction * Speed;

        int goingBackwardsModifier = goingBackwards ? -1 : 1;
        Quaternion targetRotation = Quaternion.LookRotation(GetDirectionTo(targetNode.Position) * goingBackwardsModifier);
        targetRotation *= initialRotation;
        train.transform.rotation = Quaternion.Lerp(train.transform.rotation, targetRotation, rotationSpeed);
        //platform.transform.localRotation = Quaternion.Euler(new Vector3(-Transform.eulerAngles.x, -Transform.eulerAngles.y, -Transform.eulerAngles.z));
    }
}
