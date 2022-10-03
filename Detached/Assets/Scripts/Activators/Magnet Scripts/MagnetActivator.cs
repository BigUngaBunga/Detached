using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetActivator : Activator
{
    [Header("Magnet fields")]
    [SerializeField] private float attractionSpeed;
    [SerializeField] private Transform attractionPosition;
    private List<Rigidbody> magnetizedObjects = new List<Rigidbody>();

    public void AddMagnetizedObject(GameObject gameObject) => magnetizedObjects.Add(gameObject.GetComponent<Rigidbody>());
    public void RemoveMagnetizedObject(GameObject gameObject) => magnetizedObjects.Remove(gameObject.GetComponent<Rigidbody>());

    public Vector3 GetDirection(Vector3 target) => attractionPosition.position - target;

    public void FixedUpdate()
    {
        if (IsActivated)
        {
            foreach (var gameObject in magnetizedObjects)
                gameObject.AddForce(GetDirection(gameObject.transform.position) * attractionSpeed * Time.deltaTime);
        }
    }


}
