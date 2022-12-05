using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlManager : MonoBehaviour
{
    [SerializeField] private GameObject secondGroundCheck;
    [SerializeField] private GameObject colliders, quickRig;
    [SerializeField] private float stepUpRayLenght = 2;
    private float initialRayLength;
    private CharacterControl characterControl;
    private Vector3 initialColliderPosition;
    private Vector3 initialQuickRigPosition;
    private Vector3 rotation = new Vector3(90, 0, 0);

    private Vector3 Position => transform.position;
    private Vector3 GetAdjustedPosition(float x, float y, float z) => Position + new Vector3(x, y, z);

    private void Start()
    {
        characterControl = GetComponentInParent<CharacterControl>();
        initialRayLength = characterControl.stepRayLength;
        initialColliderPosition = colliders.transform.position;
        initialQuickRigPosition = quickRig.transform.position;
    }

    public void SetCrawl(bool isCrawling)
    {
        secondGroundCheck.SetActive(isCrawling);
        if (isCrawling)
        {
            colliders.transform.position = GetAdjustedPosition(0,0.4f,-0.5f);
            quickRig.transform.position = Position;
            colliders.transform.Rotate(rotation);
            characterControl.stepRayLength = stepUpRayLenght;
        }
        else
        {
            colliders.transform.position = initialColliderPosition;
            quickRig.transform.position = initialQuickRigPosition;
            colliders.transform.Rotate(-rotation);
            characterControl.stepRayLength = initialRayLength;
        }
    }
}
