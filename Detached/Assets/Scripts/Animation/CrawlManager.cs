using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlManager : MonoBehaviour
{
    [SerializeField] private GameObject secondGroundCheck;
    [SerializeField] private GameObject colliders, quickRig;
    [SerializeField] private Vector3 ofset;
    [SerializeField] private float stepUpRayLenght = 2;
    [Range(0, 2f)]
    [SerializeField] private List<Transform> transformsToMove = new List<Transform>();
    private List<Vector3> initialTransforms = new List<Vector3>();
    private float initialRayLength;
    private CharacterControl characterControl;
    private Vector3 initialColliderPosition;
    private Quaternion initialColliderRotation;
    private Vector3 initialQuickRigPosition;
    private Vector3 rotation = new Vector3(90, 0, 0);

    private Vector3 Position => transform.localPosition;
    private Vector3 AdjustedPosition => Position + ofset;

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
            colliders.transform.localPosition = AdjustedPosition;
            quickRig.transform.localPosition = Position;
            colliders.transform.localRotation = Quaternion.Euler(initialColliderRotation.eulerAngles + rotation);
            characterControl.stepRayLength = stepUpRayLenght;
            for (int i = 0; i < transformsToMove.Count; i++)
                transformsToMove[i].localPosition = initialTransforms[i] + Position;
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
