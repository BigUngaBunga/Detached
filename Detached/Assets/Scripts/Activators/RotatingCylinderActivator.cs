using UnityEngine;

public class RotatingCylinderActivator : Activator
{
    enum Direction { Clockwise, CounterClockwise}
    [Header("Cylinder fields")]
    [SerializeField] private Direction direction;
    [SerializeField] private float degreesPerActivation = 90f;
    [Range(0.01f, 1f)]
    [SerializeField] private float secondsToRotate = 0.25f;
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
        transform.eulerAngles = new Vector3(initialRotation.x, Mathf.LerpAngle(transform.eulerAngles.y, TargetRotation, 1f / secondsToRotate * Time.deltaTime));
    }
}
