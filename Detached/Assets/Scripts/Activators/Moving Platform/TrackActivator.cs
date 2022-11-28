using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrackActivator : Activator
{
    [Header("Track fields")]
    [SerializeField] private TrackNode startNode;
    [SerializeField] private TrackNode currentNode;
    [SerializeField] private bool drawConnections;

    private void Awake() => currentNode = startNode;

    protected override void Activate() => startNode.SetActivation(isActivated);
    protected override void Deactivate() => startNode.SetActivation(isActivated);
    //public void Update() => startNode.DrawNodeConnections(Color.red);
    public Vector3 GetStartPosition() => startNode.Position;

    public TrackNode GetNextNode(ref bool isGoingBackwards)
    {
        if (!currentNode.CanContinue(isGoingBackwards))
            isGoingBackwards = !isGoingBackwards;
        currentNode = currentNode.GetNextNode(isGoingBackwards);
        return currentNode;
    }

    private void OnDrawGizmos()
    {
        if (drawConnections)
        {
            startNode.DrawNodeConnections(Color.red);
            startNode.ClearDraw();
        }
    }
}
