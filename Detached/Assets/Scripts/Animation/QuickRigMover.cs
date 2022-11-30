using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRigMover : MonoBehaviour
{
    [SerializeField] private Transform crawlingTransform;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private Vector3 crawlDisplacement;
    public void SetPosition(bool isCrawling)
    {
        if (isCrawling)
            transform.position =  crawlingTransform.position + crawlDisplacement;
        else
            transform.position = initialTransform.position;
    }
}
