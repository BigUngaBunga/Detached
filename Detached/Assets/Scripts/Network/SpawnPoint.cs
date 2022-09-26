using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        PlayerSpawnSystem.AddSpawnPoints(transform);
    }
    private void OnDestroy()
    {
        PlayerSpawnSystem.RemoveSpawnPoints(transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1,1,1));
    }
}
