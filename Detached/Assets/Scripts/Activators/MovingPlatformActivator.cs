using Steamworks;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class MovingPlatformActivator : Activator
{
    [Header("Moving platform fields")]
    [SerializeField] private GameObject platform;
    [SerializeField] private List<Transform> path;
    [Tooltip("Index values of path transforms")]
    [SerializeField] private List<int> stops = new List<int>{0};
    [SerializeField] private float platformSpeed;
    [SerializeField] private float targetDistance;
    private float Speed => platformSpeed * Time.deltaTime;
    private int currentStop = 0;
    private int currentPath = 0;
    private bool isMoving;
    private Rigidbody platformRigidbody;

    private bool GoingBackwards
    {
        get 
        {
            if (currentStop == 0 && goingBackwards)
                goingBackwards = false;
            else if (currentStop +1 == stops.Count && !goingBackwards)
                goingBackwards = true;
            return goingBackwards;
        }
    }
    private bool goingBackwards;

    private Transform GetStop(int i) => path[stops[i]];
    private int GetNextStop() => GoingBackwards ? --currentStop : ++currentStop;
    private int GetNextPath() => goingBackwards ? --currentPath : ++currentPath;
    private bool IsCloseToTarget(Transform target) => Vector3.Distance(platform.transform.position, target.position) <= targetDistance;

    private Vector3 GetDirectionTo(Transform target) => (target.position - platform.transform.position).normalized;

    private void Awake()
    {
        platformRigidbody = GetComponentInChildren<Rigidbody>();
    }

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
        Debug.Log("Next stop: " + GetNextStop());
        isMoving = true;
        while (!IsCloseToTarget(GetStop(currentStop)))
        {
            Debug.Log("Next path: " + GetNextPath());
            Vector3 direction = GetDirectionTo(path[currentPath]);
            while (!IsCloseToTarget(path[currentPath]))
            {
                //platformRigidbody.AddForce(direction * Speed);
                //platformRigidbody.velocity = direction * Speed;
                platform.transform.position += direction * Speed;
                yield return new WaitForEndOfFrame();
            }
        }
        isMoving = false;
        if (IsActivated)
            Invoke(nameof(PickNextStop), 0.25f);

        yield return null;
    }
}
