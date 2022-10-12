using Mirror;
using UnityEngine;

public class MagnetTargeting : NetworkBehaviour, IInteractable
{
    private GameObject controllingPlayer;
    private float heightTargeting;
    [SyncVar] private bool isPlayerPresent;

    [SerializeField] private float angleWidth;
    [SerializeField] private float inputSpeed;
    [SerializeField] private GameObject magnetHead;

    private Transform Transform => magnetHead.transform;

    public void Update()
    {
        if (isPlayerPresent)
        {
            AdjustHeight();
            Transform.LookAt(controllingPlayer.transform);
            Transform.eulerAngles += new Vector3(90 + heightTargeting, 0, 0);
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
        if (!isPlayerPresent && activatingObject.GetComponent<ItemManager>().NumberOfArms >= 1)
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

    public bool CanInteract(GameObject activatingObject) => activatingObject.GetComponent<ItemManager>().NumberOfArms >= 1 && !isPlayerPresent;
}
