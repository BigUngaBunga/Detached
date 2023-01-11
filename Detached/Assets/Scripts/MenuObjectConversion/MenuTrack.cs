using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrack : MonoBehaviour
{
    [Header("Track fields")]
    [SerializeField] private TrackNode startNode;
    [SerializeField] private TrackNode currentNode;
    [SerializeField] private bool drawConnections;
    [Header("Rail prefab")]
    [SerializeField] private GameObject railPrefab;
    private void Awake() => currentNode = startNode;
    public Vector3 GetStartPosition() => startNode.Position;

    public TrackNode PeekNextNode(bool isGoingBackwards)
    {
        if (!currentNode.CanContinue(isGoingBackwards))
            isGoingBackwards = !isGoingBackwards;
        return currentNode.GetNextNode(isGoingBackwards);
    }

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
        }
    }
}
