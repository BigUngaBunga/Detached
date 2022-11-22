using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class MagnetController : NetworkBehaviour, IInteractable
{
    [SyncVar(hook = nameof(HookControllingPLayer))] private GameObject controllingPlayer;
    [SyncVar(hook = nameof(HookIsPlayerPresent))] private bool isPlayerPresent;

    [SerializeField] private GameObject magnetHead;
    [SerializeField] private Transform cameraFocus;
    [Range(0, 180)]
    [SerializeField] private float rotationLimit; 

    private Transform MagnetHead => magnetHead.transform;
    private Vector3 initialRotation = new Vector3(90, 180);

    [Command (requiresAuthority = false)]
    private void HookIsPlayerPresent(bool oldValue, bool newValue) => isPlayerPresent = newValue;
    [Command(requiresAuthority = false)]
    private void HookControllingPLayer(GameObject oldValue, GameObject newValue) => controllingPlayer = newValue;
    [Command(requiresAuthority = false)]
    private void SetMagnetRotation(Vector3 angles) => MagnetHead.eulerAngles = angles;

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
        Vector3 camera = Camera.main.transform.eulerAngles;
        Vector3 eulerAngles =  camera;
        if (camera.x > 180f)
            eulerAngles.x = camera.x - 360f;

        eulerAngles.x = Mathf.Clamp(-eulerAngles.x, -rotationLimit, rotationLimit);
        SetMagnetRotation(eulerAngles + initialRotation);
    }

    public void Interact(GameObject activatingObject)
    {
        if (!isPlayerPresent && activatingObject.GetComponent<ItemManager>().NumberOfArms >= 1)
            Enter(activatingObject);
    }

    private void Enter(GameObject player)
    {
        isPlayerPresent = true;
        controllingPlayer = player;
        controllingPlayer.GetComponent<ItemManager>().AllowInteraction = false;
        if (controllingPlayer.TryGetComponent(out CharacterControl controller))
        {
            controller.SetCameraFocus(cameraFocus);
            controller.TogglePlayerControl();
        }
        else
        {
            controllingPlayer = null;
            isPlayerPresent = false;
        }
    }

    private void Exit()
    {
        controllingPlayer.GetComponent<ItemManager>().AllowInteraction = true;
        controllingPlayer.TryGetComponent(out CharacterControl controller);
        controller.SetCameraFocusPlayer();
        controller.TogglePlayerControl();
        isPlayerPresent = false;
        controllingPlayer = null;
    }

    public bool CanInteract(GameObject activatingObject)
    {
        if (activatingObject.CompareTag("Player"))
            return !isPlayerPresent && activatingObject.GetComponent<ItemManager>().NumberOfArms >= 1;
        return false;
    }
}
