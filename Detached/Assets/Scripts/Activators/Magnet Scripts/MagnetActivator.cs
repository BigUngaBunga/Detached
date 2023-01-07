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
    [SerializeField] private float forwardsStrength;
    [SerializeField] private Transform attractionPosition;
    [SerializeField] private GameObject magnetizationField;
    private List<Rigidbody> magnetizedObjects = new List<Rigidbody>();

    private Vector3 Forward => attractionPosition.up * -1f;

    public void AddMagnetizedObject(GameObject gameObject)
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (!magnetizedObjects.Contains(rigidbody))
            magnetizedObjects.Add(rigidbody);
    }
    public void RemoveMagnetizedObject(GameObject gameObject)
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (magnetizedObjects.Contains(rigidbody))
            magnetizedObjects.Remove(rigidbody);
    }

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
        for (int i = magnetizedObjects.Count - 1; i >= 0; i--)
        {

            if (magnetizedObjects[i] == null)
            {
                magnetizedObjects.RemoveAt(i);
                return;
            }
            Rigidbody rigidbody = magnetizedObjects[i];
            Vector3 direction = GetDirection(rigidbody.transform.position);
            var forwardsForce = GetForce(direction.normalized, attractionSpeed * forwardsStrength);
            var centralForce = GetForce(GetCentralDirection(rigidbody.transform.position), attractionSpeed * centralizationStrength);

            Debug.DrawRay(rigidbody.transform.position, forwardsForce, Color.red);
            Debug.DrawRay(rigidbody.transform.position, centralForce, Color.yellow);

            rigidbody.AddForce(forwardsForce * rigidbody.mass);
            rigidbody.AddForce(centralForce * rigidbody.mass);
        }
        Debug.DrawRay(attractionPosition.position, Forward * 10f, Color.green);
    }

    protected override void Activate()
    {
        base.Activate();
        SetMagnetizationActivation(true);
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        SetMagnetizationActivation(false);
    }

    protected override void Start()
    {
        base.Start();
        magnetizationField.SetActive(false);
    }

    [ClientRpc]
    private void SetMagnetizationActivation(bool isActive) => magnetizationField.SetActive(isActive);
}
