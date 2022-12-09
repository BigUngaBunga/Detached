
using Mirror;
using Mirror.Examples.AdditiveLevels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingSystem : NetworkBehaviour
{
    [Header("GameObject")]

    [SerializeField] GameObject pingPrefab;
    [SerializeField] float duration;
    [SerializeField] Transform raySource;

    void Start()
    {
        raySource = GameObject.Find("Camera").transform;
    }


    void PlaySoundOnTrigger(Vector3 position)
    {
        SFXManager.PlayOneShot(SFXManager.PingSound, SFXManager.SFXVolume, position);
    }

    void Update()
    {
        if (isLocalPlayer && Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
            Ping(raySource.position, raySource.forward);
    }

    [Command]
    private void Ping(Vector3 origin, Vector3 direction)
    {
        if (!Physics.Raycast(origin, direction, out RaycastHit hit))
            return;

        Vector3 position = hit.point + new Vector3(0, 3f, 0);
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
}
