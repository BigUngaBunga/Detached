using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlManager : MonoBehaviour
{
    [SerializeField] private GameObject secondGroundCheck;
    [SerializeField] private GameObject colliders, quickRig;
    [SerializeField] private Vector3 ofset;
    [SerializeField] private float stepUpRayLenght = 2;
    private float initialRayLength;
    private CharacterControl characterControl;
    private Vector3 initialColliderPosition;
    private Vector3 initialQuickRigPosition;
    private Vector3 rotation = new Vector3(90, 0, 0);

    private Vector3 Position => transform.localPosition;
    private Vector3 AdjustedPosition => Position + ofset;

    private void Start()
    {
        characterControl = GetComponentInParent<CharacterControl>();
        initialRayLength = characterControl.stepRayLength;
        initialColliderPosition = colliders.transform.localPosition;
        initialQuickRigPosition = quickRig.transform.localPosition;
    }

    public void SetCrawl(bool isCrawling)
    {
        secondGroundCheck.SetActive(isCrawling);
        if (isCrawling)
        {
            colliders.transform.localPosition = AdjustedPosition;
            quickRig.transform.localPosition = Position;
            colliders.transform.Rotate(rotation);
            characterControl.stepRayLength = stepUpRayLenght;
        }
        else
        {
            colliders.transform.localPosition = initialColliderPosition;
            quickRig.transform.localPosition = initialQuickRigPosition;
            colliders.transform.Rotate(-rotation);
            characterControl.stepRayLength = initialRayLength;
        }
    }
}
