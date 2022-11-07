using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField] private LineRenderer lineRend;

    //[SerializeField]
    //[Range(3, 30)]
    //private int lineSegmentCount = 20;

    [SerializeField]
    private LineRenderer LineRenderer;

    [SerializeField]
    LayerMask collidable;

    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    private int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float TimeBetweenPoints = 0.1f;

  //  private List<Vector3> linePoints;

    #region singleton
    public static DrawTrajectory instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion
    void Start()
    {
        LineRenderer = GameObject.Find("Line").GetComponent<LineRenderer>();
        /*        linePoints = new List<Vector3>();*/
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer.startWidth=0.15f;
        LineRenderer.endWidth = 0.15f;
        //LineRenderer.GetPosition(LineRenderer.positionCount);
    }

    public void DrawProjection(Vector3 forward, Vector3 up, Vector3 startingPoint, float throwForce,float upforce)
    {
        LineRenderer.enabled = true;
        LineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;
        Vector3 startPosition = startingPoint;
        Vector3 startVelocity = throwForce * forward + up * upforce;
        int i = 0;
        LineRenderer.SetPosition(i, startPosition);
        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            LineRenderer.SetPosition(i, point);

            Vector3 lastPosition = LineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition, (point - lastPosition).normalized, out RaycastHit hit, (point - lastPosition).magnitude, collidable))
            {
                LineRenderer.SetPosition(i, hit.point);
                LineRenderer.positionCount = i + 1;
                return;
            }
        }
    }

    //public void UpdateTrajectory(Vector3 forceVector,/*, Rigidbody rigidbody,*/ Vector3 startingPoint)
    //{

    //    Vector3 vel = forceVector /*/ rigidbody.mass)*/ * Time.fixedDeltaTime;

    //    float flightDuration = (2 * vel.y) / Physics.gravity.y;

    //    float stepTime = flightDuration / lineSegmentCount;

    //    linePoints.Clear();

    //    for (int i = 0; i < lineSegmentCount; i++)
    //    {

    //        float stepTimePassed = stepTime * i;

    //        Vector3 movementVector = new Vector3(
    //            vel.x * stepTimePassed,
    //            vel.y * stepTimePassed - 0.5f *  Physics.gravity.y * Mathf.Pow(stepTimePassed, 2),
    //            vel.z * stepTimePassed);

    //        RaycastHit hit;
    //        if (Physics.Raycast(startingPoint, -movementVector, out hit, movementVector.magnitude) )
    //        {
    //            break;
    //        }
    //        linePoints.Add(-movementVector + startingPoint);
    //    }
    //    lineRend.positionCount = linePoints.Count;
    //    lineRend.SetPositions(linePoints.ToArray());
    //}

    public void HideLine()
    {
        LineRenderer.positionCount = 0;
    }
}
