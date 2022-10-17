using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetTargeting : NetworkBehaviour, IInteractable
{
    private GameObject controllingPlayer;
    private float heightTargeting;
    [SyncVar] private bool isPlayerPresent;

    [SerializeField] private float angleWidth;
    [SerializeField] private float inputSpeed;

    public void Update()
    {
        if (isPlayerPresent)
        {
            AdjustHeight();
            transform.LookAt(controllingPlayer.transform);
            transform.eulerAngles += new Vector3(90 + heightTargeting, 0, 0);
        }
    }

    //TODO kontrollera med WASD/piltangenter
    private void AdjustHeight()
    {
        float input = Input.GetAxis("Mouse ScrollWheel") * -1 * inputSpeed;
        heightTargeting = Mathf.Clamp(heightTargeting + input, -angleWidth, angleWidth);
    }

    public void Interact(GameObject activatingObject)
    {
        if (!isPlayerPresent && activatingObject.GetComponent<ItemManager>().NumberOfArms() >= 1)
        {
            isPlayerPresent = true;
            controllingPlayer = activatingObject;
        }
        else if (isPlayerPresent && controllingPlayer.Equals(activatingObject))
        {
            isPlayerPresent = false;
            controllingPlayer = null;
        }
    }
}
