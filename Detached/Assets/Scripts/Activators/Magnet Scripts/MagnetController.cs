using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class MagnetController : NetworkBehaviour, IInteractable
{
    private GameObject controllingPlayer;
    private float heightTargeting;
    [SyncVar] private bool isPlayerPresent;

    [SerializeField] private GameObject magnetHead;
    [SerializeField] private Transform cameraFocus;

    private Transform Transform => magnetHead.transform;
    private Vector3 rotation = new Vector3(90, 180);

    public void Update()
    {
        if (isPlayerPresent && NetworkClient.localPlayer.gameObject == controllingPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
                Exit();

            Rotate();
        }
    }

    private void Rotate()
    {
        rotation.x += Input.GetAxis("Mouse Y");
        Vector3 camera = Camera.main.transform.eulerAngles;
        Vector3 eulerAngles =  camera + rotation;
        Transform.eulerAngles = eulerAngles;
    }

    public void Interact(GameObject activatingObject)
    {
        if (!isPlayerPresent && activatingObject.GetComponent<ItemManager>().NumberOfArms >= 1)
            Enter(activatingObject);
    }

    [Command(requiresAuthority = false)]
    private void Enter(GameObject player)
    {
        isPlayerPresent = true;
        controllingPlayer = player;
        Debug.Log("Updated variables");
        controllingPlayer.GetComponent<ItemManager>().allowLimbInteraction = false;
        if (controllingPlayer.TryGetComponent(out CharacterControl controller))
        {
            Debug.Log("Got components successfully");
            Debug.Log("Attempting camera focus change");
            controller.SetCameraFocus(cameraFocus);
            Debug.Log("Attempting player control toggle");
            controller.TogglePlayerControl();
        }
        else
        {
            Debug.Log("Could not get characterController");
            controllingPlayer = null;
            isPlayerPresent = false;
        }
        
        
    }

    [Command(requiresAuthority = false)]
    private void Exit()
    {
        controllingPlayer.GetComponent<ItemManager>().allowLimbInteraction = true;
        controllingPlayer.TryGetComponent(out CharacterControl controller);
        controller.SetCameraFocusPlayer();
        controller.TogglePlayerControl();
        isPlayerPresent = false;
        controllingPlayer = null;
    }



    public bool CanInteract(GameObject activatingObject) => activatingObject.GetComponent<ItemManager>().NumberOfArms >= 1 && !isPlayerPresent;
}
