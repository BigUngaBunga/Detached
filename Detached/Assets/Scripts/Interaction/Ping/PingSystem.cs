
using Mirror;
using Mirror.Examples.AdditiveLevels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PingSystem : NetworkBehaviour
{
    [Header("GameObject")]

    [SerializeField] private LayerMask pingMask;
    [SerializeField] private GameObject pingPrefab;
    [SerializeField] private float duration;
    [SerializeField] private float height;
    [SerializeField] private Transform raySource;

    void Start()
    {
        raySource = GameObject.Find("Camera").transform;
    }


    void PlaySoundOnTrigger(Vector3 position)
    {
        OneShotVolume.PlayOneShot(AudioPaths.PingSound, VolumeManager.GetSFXVolume(), position);
    }

    void Update()
    {
        if (isLocalPlayer && Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1) && !UI.gameIsPaused)
        {
            Ping(raySource.position, raySource.forward);
            CMDPingDialogue(transform.position, transform.gameObject.name);
        }

    }

    [Command]
    private void Ping(Vector3 origin, Vector3 direction)
    {
        if (!Physics.Raycast(origin, direction, out RaycastHit hit, float.MaxValue, pingMask, QueryTriggerInteraction.Ignore))
            return;

        Vector3 position = hit.point + new Vector3(0, height, 0);
        GameObject pingPoint = Instantiate(pingPrefab, position, Quaternion.identity);
        NetworkServer.Spawn(pingPoint);
        RPCPing(pingPoint, position);
    }

    [ClientRpc]
    private void RPCPing(GameObject ping, Vector3 position)
    {
        ping.transform.position = position;
        ping.GetComponent<PingPoint>().RemoveAfterDuration(duration);
        PlaySoundOnTrigger(position);
    }

    [Command]
    private void CMDPingDialogue(Vector3 position, string name)
    {

        RPCPingDialogue(position, name);
    }

    [ClientRpc]
    private void RPCPingDialogue(Vector3 position, string name)
    {
        if (name == "Ched(Clone)") OneShotVolume.PlayOneShot(AudioPaths.PingChed, VolumeManager.GetDialogueVolume(), position);
        else OneShotVolume.PlayOneShot(AudioPaths.PingDeta, VolumeManager.GetDialogueVolume(), position);
    }
}
