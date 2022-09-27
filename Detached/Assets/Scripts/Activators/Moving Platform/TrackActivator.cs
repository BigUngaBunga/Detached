using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrackActivator : Activator
{
    [Header("Track fields")]
    //[SerializeField] private List<Transform> path;
    //[Tooltip("Index values of path transforms")]
    //[SerializeField] private List<int> stops;
    [SerializeField] private TrackNode startNode;
    [SerializeField] private TrackNode currentNode;

    private void Awake()
    {
        currentNode = startNode;
    }

    protected override void Activate()
    {
        base.Activate();
        startNode.SetActivation(IsActivated);
    }
    protected override void Deactivate()
    {
        base.Deactivate();
        startNode.SetActivation(IsActivated);
    }
    public void Update()
    {
        //for (int i = 1; i < path.Count; i++)
        //    Debug.DrawLine(path[i - 1].position, path[i].position);
        startNode.DrawNodeConnections(Color.red);
    }
    public Vector3 GetStartPosition() => startNode.Position;

    public TrackNode GetNextNode(ref bool isGoingBackwards)
    {
        if (!currentNode.CanContinue(isGoingBackwards))
            isGoingBackwards = !isGoingBackwards;
        currentNode = currentNode.GetNextNode(isGoingBackwards);
        return currentNode;
    }

    //public float StopsCount => stops.Count;

    //public Transform GetStop(int i) => path[stops[i]];

    //public Transform GetPath(int i) => path[i];
}
