using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private LineRenderer lineRend;

    [SerializeField]
    [Range(3, 30)]
    private int lineSegmentCount = 20;

    private List<Vector3> linePoints;

    #region singleton
    public static DrawTrajectory instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion
    void Start()
    {
        linePoints = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateTrajectory(Vector3 forceVector,/*, Rigidbody rigidbody,*/ Vector3 startingPoint, float dir)
    {
        Vector3 vel = forceVector /*/ rigidbody.mass)*/ * Time.fixedDeltaTime;

        float flightDuration = (2 * vel.y) / Physics.gravity.y;

        float stepTime = flightDuration / lineSegmentCount;

        linePoints.Clear();

        for (int i = 0; i < lineSegmentCount; i++)
        {

            float stepTimePassed = stepTime * i;

            Vector3 movementVector = new Vector3(
                vel.x * stepTimePassed,
                vel.y * stepTimePassed - 0.5f *  Physics.gravity.y * Mathf.Pow(stepTimePassed, 2),
                vel.z * stepTimePassed );

            RaycastHit hit;
            if (Physics.Raycast(startingPoint, -movementVector, out hit, movementVector.magnitude) )
            {
                break;
            }
            linePoints.Add(-movementVector + startingPoint);
        }
        lineRend.positionCount = linePoints.Count;
        lineRend.SetPositions(linePoints.ToArray());
    }

    public void HideLine()
    {
        lineRend.positionCount = 0;
    }
}
