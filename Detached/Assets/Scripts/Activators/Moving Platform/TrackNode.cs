using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackNode : MonoBehaviour
{
    enum NodeType { Path, Stop }

    [SerializeField] private NodeType type = NodeType.Path;
    [SerializeField] private TrackNode nextNode, nextActiveNode;
    [SerializeField] private TrackNode previousNode, previousActiveNode;
    [SerializeField] private bool isActivated;
    public bool IsStop => type.Equals(NodeType.Stop);
    public Vector3 Position { get { return transform.position; } }

    public void SetActivation(bool isActivated)
    {
        this.isActivated = isActivated;
        if (nextActiveNode != null)
            nextActiveNode.SetActivation(isActivated);
        if (nextNode != null)
            nextNode.SetActivation(isActivated);
    }

    void Start()
    {
        if (nextNode == null || previousNode == null)
            type = NodeType.Stop;
    }

    public TrackNode GetNextNode(bool goingBackwards)
    {
        if (goingBackwards)
        {
            if (isActivated && previousActiveNode != null)
                return previousActiveNode;
            return previousNode;
        }
        if (isActivated && nextActiveNode != null)
            return nextActiveNode;
        return nextNode;
    }

    public bool CanContinue(bool goingBackwards)
    {
        if (type.Equals(NodeType.Path))
            return true;

        return goingBackwards ? previousNode != null : nextNode != null;
    }

    public void DrawNodeConnections(Color colour)
    {
        if (nextActiveNode != null)
        {
            Debug.DrawLine(Position, nextActiveNode.Position, Color.green);
            nextActiveNode.DrawNodeConnections(Color.green);
        }
        if (nextNode != null)
        {
            Debug.DrawLine(Position, nextNode.Position, colour);
            nextNode.DrawNodeConnections(colour);
        }
    }
}
