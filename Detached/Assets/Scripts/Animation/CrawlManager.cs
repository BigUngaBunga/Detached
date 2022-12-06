using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlManager : MonoBehaviour
{
    [SerializeField] private GameObject secondGroundCheck;
    [SerializeField] private GameObject colliders, quickRig;
    [SerializeField] private Vector3 colliderOfset, moveOfset;
    [SerializeField] private float stepUpRayLenght = 2;
    [SerializeField] private List<Transform> transformsToMove = new List<Transform>();
    private List<Vector3> initialTransforms = new List<Vector3>();
    private float initialRayLength;
    private CharacterControl characterControl;
    private Vector3 initialColliderPosition;
    private Quaternion initialColliderRotation;
    private Vector3 initialQuickRigPosition;
    private Vector3 rotation = new Vector3(90, 0, 0);

    private Vector3 Position => transform.localPosition;
    //private Vector3 AdjustedPosition => Position + colliderOfset;
    private Vector3 AdjustedPosition(Vector3 ofset) => Position + ofset;

    private void Start()
    {
        characterControl = GetComponentInParent<CharacterControl>();
        initialRayLength = characterControl.stepRayLength;
        initialColliderPosition = colliders.transform.localPosition;
        initialColliderRotation = colliders.transform.localRotation;
        initialQuickRigPosition = quickRig.transform.localPosition;
        for (int i = 0; i < transformsToMove.Count; i++)
            initialTransforms.Add(transformsToMove[i].localPosition);
    }

    public void SetCrawl(bool isCrawling)
    {
        secondGroundCheck.SetActive(isCrawling);
        if (isCrawling)
        {
            colliders.transform.localPosition = AdjustedPosition(colliderOfset);
            quickRig.transform.localPosition = Position;
            colliders.transform.localRotation = Quaternion.Euler(initialColliderRotation.eulerAngles + rotation);//ignore head?
            characterControl.stepRayLength = stepUpRayLenght;
            for (int i = 0; i < transformsToMove.Count; i++)
                transformsToMove[i].localPosition = initialTransforms[i] + AdjustedPosition(moveOfset);
        }
        else
        {
            colliders.transform.localPosition = initialColliderPosition;
            quickRig.transform.localPosition = initialQuickRigPosition;
            colliders.transform.localRotation = initialColliderRotation;
            characterControl.stepRayLength = initialRayLength;
            for (int i = 0; i < transformsToMove.Count; i++)
                transformsToMove[i].localPosition = initialTransforms[i];
        }
    }
}
