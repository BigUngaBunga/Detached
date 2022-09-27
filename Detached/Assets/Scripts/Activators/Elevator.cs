using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Activator
{
    [Header("Elevator fields")]
    [SerializeField] private GameObject elevatorFloor;
    [SerializeField] private List<Transform> stops;
    [SerializeField] private float elevatorSpeed;
    private float Speed => elevatorSpeed * Time.deltaTime;
    private int currentStop = 0;
    private bool isMoving;

    protected override void Activate()
    {
        base.Activate();
        PickNextStop();
    }

    private void PickNextStop()
    {
        if (isMoving)
            return;
        ++currentStop;
        currentStop %= stops.Count;
        StartCoroutine(MoveToNextStop());
    }

    private IEnumerator MoveToNextStop()
    {
        isMoving = true;
        if (currentStop == 0)
        {
            while (stops[currentStop].position.y < elevatorFloor.transform.position.y)
            {
                elevatorFloor.transform.position += elevatorFloor.transform.up * Speed * (-1);
                yield return new WaitForEndOfFrame();
            }   
        }
        else
        {
            while (stops[currentStop].position.y > elevatorFloor.transform.position.y)
            {
                elevatorFloor.transform.position += elevatorFloor.transform.up * Speed;
                yield return new WaitForEndOfFrame();
            }
        }

        isMoving = false;
        if (IsActivated)
            Invoke(nameof(PickNextStop), 0.1f);

        yield return null;
    }
}
