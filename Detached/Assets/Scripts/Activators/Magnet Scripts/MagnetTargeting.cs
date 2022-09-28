using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetTargeting : MonoBehaviour
{
    private List<GameObject> presentPlayers = new List<GameObject>();
    private bool PlayerIsPresent => presentPlayers.Count > 0;
    private float heightTargeting;
    [SerializeField] private float angleWidth;
    [SerializeField] private float inputSpeed;

    public void Update()
    {
        if (PlayerIsPresent)
        {
            AdjustHeight();
            transform.LookAt(presentPlayers[0].transform);
            transform.eulerAngles += new Vector3(90 + heightTargeting, 0, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            presentPlayers.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            presentPlayers.Remove(other.gameObject);
    }

    private void AdjustHeight()
    {
        float input = Input.GetAxis("Mouse ScrollWheel") * -1 * inputSpeed;
        heightTargeting = Mathf.Clamp(heightTargeting + input, -angleWidth, angleWidth);
    }
}
