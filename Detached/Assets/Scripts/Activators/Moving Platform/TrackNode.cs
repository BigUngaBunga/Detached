using UnityEngine;

public class TrackNode : MonoBehaviour
{
    enum NodeType { Path, Stop }

    [Header("Node fields")]
    [SerializeField] private NodeType type = NodeType.Path;
    [SerializeField] private TrackNode nextNode, nextActiveNode;
    [SerializeField] private TrackNode previousNode, previousActiveNode;
    [SerializeField] private bool isActivated;
    private bool hasBeenDrawn, hasBeenSearched;
    [SerializeField] private GameObject trackPrefab;

    public bool IsStop => type.Equals(NodeType.Stop);
    public Vector3 Position { get { return transform.position; } }

    public void SetActivation(bool isActivated)
    {
        if (hasBeenSearched)
            return;
        hasBeenSearched = true;
        this.isActivated = isActivated;
        if (nextActiveNode != null)
            nextActiveNode.SetActivation(isActivated);
        if (nextNode != null)
            nextNode.SetActivation(isActivated);
        hasBeenSearched = false;
    }

    void Start()
    {
        if (nextNode == null || previousNode == null)
            type = NodeType.Stop;

        if (nextNode != null && trackPrefab != null)
            AddTrack();
    }
    private void AddTrack()
    {
        float trackHeight = 6;
        float forwardDistance = 5;
        Quaternion rotation = Quaternion.LookRotation(GetDirection(nextNode.Position, Position));
        rotation *= Quaternion.Euler(new Vector3(0, -90, 0));
        Vector3 trackPosition = transform.position - new Vector3(0, trackHeight, 0);
        var track = Instantiate(trackPrefab, trackPosition, rotation);
        track.transform.position += track.transform.right * forwardDistance;
    }
    private Vector3 GetDirection(Vector3 target, Vector3 position) => (target - position).normalized;

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
        if (hasBeenDrawn)
            return;
        hasBeenDrawn = true;
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

        hasBeenDrawn = false;

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
