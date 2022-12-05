using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetActivator : Activator
{
    [Header("Magnet fields")]
    [SerializeField] private float attractionSpeed;
    [Range(0.5f, 2f)]
    [SerializeField] private float centralizationStrength;
    [SerializeField] private Transform attractionPosition;
    [SerializeField] private GameObject magnetizationField;
    private List<Rigidbody> magnetizedObjects = new List<Rigidbody>();

    private Vector3 Forward => attractionPosition.up * -1f;

    public void AddMagnetizedObject(GameObject gameObject) => magnetizedObjects.Add(gameObject.GetComponent<Rigidbody>());
    public void RemoveMagnetizedObject(GameObject gameObject) => magnetizedObjects.Remove(gameObject.GetComponent<Rigidbody>());

    private Vector3 GetDirection(Vector3 position) => attractionPosition.position - position;

    private Vector3 GetCentralDirection(Vector3 position)
    {

        float projectionLength = Vector3.Project(GetDirection(position), Forward).magnitude;
        Vector3 centralPosition = attractionPosition.position + Forward * projectionLength;
        return centralPosition - position;
    }

    private Vector3 GetForce(Vector3 direction, float speed) => direction.normalized * speed * Time.deltaTime;

    public void FixedUpdate()
    {
        if (isActivated)
            ApplyForce();
    }

    private void ApplyForce()
    {
        foreach (var gameObject in magnetizedObjects)
        {
            Vector3 direction = GetDirection(gameObject.transform.position);
            var forwardsForce = GetForce(direction.normalized, attractionSpeed * 0.25f);
            var centralForce = GetForce(GetCentralDirection(gameObject.transform.position), attractionSpeed * centralizationStrength);

            Debug.DrawRay(gameObject.transform.position, forwardsForce, Color.red);
            Debug.DrawRay(gameObject.transform.position, centralForce, Color.yellow);

            gameObject.AddForce(forwardsForce * gameObject.mass);
            gameObject.AddForce(centralForce * gameObject.mass);
        }
        Debug.DrawRay(attractionPosition.position, Forward * 10f, Color.green);
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
