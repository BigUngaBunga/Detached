using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TrackNode : MonoBehaviour
{
    enum NodeType { Path, Stop }

    [Header("Node fields")]
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
            Debug.DrawLine(Position, nextActiveNode.Position, GetActiveColour());
            nextActiveNode.DrawNodeConnections(GetActiveColour());
        }
        if (nextNode != null)
        {
            Debug.DrawLine(Position, nextNode.Position, colour);
            nextNode.DrawNodeConnections(colour);
        }

        Color GetActiveColour()
        {
            if (colour.Equals(Color.red))
                return Color.yellow;
            if (colour.Equals(Color.yellow))
                return Color.green;
            else if (colour.Equals(Color.green))
                return Color.blue;
            else if (colour.Equals(Color.blue))
                return Color.white;
            return colour;
        }
    }
}
