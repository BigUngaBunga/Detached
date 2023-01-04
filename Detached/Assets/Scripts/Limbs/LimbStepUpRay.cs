using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbStepUpRay : MonoBehaviour
{
    private LayerMask layerMask = new LayerMask();
    // Start is called before the first frame update
    [Header("Step up")]
    [SerializeField] GameObject[] stepRays;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;
    public float stepRayLengthLow = 1f;
    public float stepRayLengthHigh = 1f;

    void Start()
    {
        layerMask = LayerMask.GetMask("Ground", "Interactable");
    }

    private void Awake()
    {
        for (int i = 2; i < 4; i++)
        {
            stepRays[i].transform.localPosition = new Vector3(stepRays[i].transform.localPosition.x, stepHeight, stepRays[i].transform.localPosition.z); //(upper rays position)
        }
    }

    // Update is called once per frame


    public void ActivateStepClimb(Vector3 input,Rigidbody rb) => rb.position += StepClimb(stepRays[0], stepRays[1], input, rb);

    public Vector3 GetStepClimb(Vector3 input, Rigidbody rb) => StepClimb(stepRays[0], stepRays[1], input, rb);

    Vector3 StepClimb(GameObject rayDirectioLowerLeft, GameObject rayDirectioLowerRight, Vector3 input, Rigidbody rb)
    {
        RaycastHit hitLower;
        Vector3 rbDirection = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        // Debug.DrawRay(rayDirectioLowerMid.transform.position, rbDirection.normalized, Color.green);
        Debug.DrawRay(rayDirectioLowerLeft.transform.position, rbDirection.normalized * stepRayLengthLow, Color.red);
        Debug.DrawRay(rayDirectioLowerRight.transform.position, rbDirection.normalized * stepRayLengthLow, Color.blue);
        /* if (Physics.Raycast(rayDirectioLowerMid.transform.position, rbDirection.normalized, out hitLower, rayLengthMid))
         {
             Debug.Log("mid");
             StepClimbUpperCheck(rbDirection, stepRays[3]);

         }*/
        if (Physics.Raycast(rayDirectioLowerLeft.transform.position, rbDirection.normalized, out hitLower, stepRayLengthLow, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Left");
            if (!hitLower.collider.CompareTag("Leg") && !hitLower.collider.CompareTag("Arm") && !hitLower.collider.CompareTag("Box"))
                return StepClimbUpperCheck(rbDirection, stepRays[2], input, rb);
        }

        if (Physics.Raycast(rayDirectioLowerRight.transform.position, rbDirection.normalized, out hitLower, stepRayLengthLow, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Right");
            if (!hitLower.collider.CompareTag("Leg") && !hitLower.collider.CompareTag("Arm") && !hitLower.collider.CompareTag("Box"))
                return StepClimbUpperCheck(rbDirection, stepRays[3], input, rb);
        }

        return Vector3.zero;

        //Debug.DrawRay(stepRays[3].transform.position, rbDirection.normalized, Color.green);
        //Debug.DrawRay(stepRays[4].transform.position, rbDirection.normalized, Color.red);
        //Debug.DrawRay(stepRays[5].transform.position, rbDirection.normalized, Color.blue);
    }

    private Vector3 StepClimbUpperCheck(Vector3 rbDirection, GameObject rayDirectionUpper,Vector3 input,Rigidbody rb)
    {
        RaycastHit hitUpper;
        if (!Physics.Raycast(rayDirectionUpper.transform.position, rbDirection.normalized, out hitUpper, stepRayLengthHigh)) //upper check
        {
            Debug.Log("Input " + input);
            if (input != Vector3.zero)
               return new Vector3(0f, -stepSmooth, 0f) * -1; //the actual stepClimb
        }
        return Vector3.zero;
    }
}
