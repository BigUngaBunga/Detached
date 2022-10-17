using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetActivator : Activator
{
    [Header("Magnet fields")]
    [SerializeField] private float attractionSpeed;
    [SerializeField] private float centralizationSpeed;
    [SerializeField] private Transform attractionPosition;
    [SerializeField] private GameObject magnetizationField;
    private List<Rigidbody> magnetizedObjects = new List<Rigidbody>();

    public void AddMagnetizedObject(GameObject gameObject) => magnetizedObjects.Add(gameObject.GetComponent<Rigidbody>());
    public void RemoveMagnetizedObject(GameObject gameObject) => magnetizedObjects.Remove(gameObject.GetComponent<Rigidbody>());

    public Vector3 GetDirection(Vector3 target) => attractionPosition.position - target;

    public Vector3 GetCentralDirection(Vector3 direction) => direction.normalized - attractionPosition.forward;

    public void FixedUpdate()
    {
        if (IsActivated)
        {
            foreach (var gameObject in magnetizedObjects)
            {
                Vector3 direction = GetDirection(gameObject.transform.position);
                gameObject.AddForce(direction.normalized * attractionSpeed * Time.deltaTime);
                gameObject.AddForce(GetCentralDirection(direction) * centralizationSpeed * Time.deltaTime);
            }
                
        }
    }

    protected override void Activate()
    {
        base.Activate();
        magnetizationField.SetActive(true);
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        magnetizationField.SetActive(false);
    }
}
