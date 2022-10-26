using UnityEngine;

public class RotatingCylinderActivator : Activator
{
    enum Direction { Clockwise, CounterClockwise}
    [Header("Cylinder fields")]
    [SerializeField] private Direction direction;
    [SerializeField] private float degreesPerActivation = 90f;
    [SerializeField] private float lerpIncrement;
    [SerializeField] private Vector3 initialRotation;
    private float TargetRotation => initialRotation.y + degreesPerActivation * ActiveConnections;

    private void Awake()
    {
        initialRotation = transform.eulerAngles;
        if (direction.Equals(Direction.CounterClockwise))
            degreesPerActivation *= -1f;
    }
    private void Update()
    {
        transform.eulerAngles = new Vector3(initialRotation.x, Mathf.LerpAngle(transform.eulerAngles.y, TargetRotation, lerpIncrement * Time.deltaTime));
    }
}
