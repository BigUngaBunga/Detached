using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Activator
{
    [Header("Elevator fields")]
    [SerializeField] private GameObject elevatorFloor;
    [SerializeField] private List<Transform> stops;
    [SerializeField] private float elevatorSpeed;

    private ElevatorMovement movement;
    private float Speed => elevatorSpeed * Time.deltaTime;
    private Vector3 Velocity => goingUp ? elevatorFloor.transform.up * Speed : elevatorFloor.transform.up * Speed * (-1);
    private int currentStop = 0;
    private bool isMoving;

    [SerializeField] private bool goingUp = true;

    private void Start()
    {
        movement = elevatorFloor.GetComponent<ElevatorMovement>();
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

        if (goingUp)
            goingUp = currentStop + 1 < stops.Count;
        else
            goingUp = currentStop <= 0;


        currentStop = goingUp ? currentStop + 1 : currentStop - 1;
        StartCoroutine(MoveToNextStop());
    }

    private IEnumerator MoveToNextStop()
    {
        isMoving = true;
        while (FurtherToGo())
        {
            movement.Move(Velocity);
            yield return new WaitForFixedUpdate();
        }
        isMoving = false;
        if (IsActivated)
            Invoke(nameof(PickNextStop), 0.1f);

        yield return null;
    }

    private bool FurtherToGo()
    {
        if (goingUp)
            return stops[currentStop].position.y > elevatorFloor.transform.position.y; ;

        return stops[currentStop].position.y < elevatorFloor.transform.position.y;
            
    }
}
