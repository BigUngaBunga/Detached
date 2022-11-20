using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private int numOfLimbsAtSpawn;
    public int NumOfLimbsAtSpawn
    {
        get { return numOfLimbsAtSpawn; }      
    }

    [Range(0, 2)]
    [SerializeField] public int numOfLegs = 2;
    [Range(0, 2)]
    [SerializeField] public int numOfArms = 2;


    private void Awake()
    {
        PlayerSpawnSystem.AddSpawnPoints(gameObject);
        numOfLimbsAtSpawn += numOfArms + numOfLegs + 1; // 1 for head
    }
    private void OnDestroy()
    {
        PlayerSpawnSystem.RemoveSpawnPoints(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1,1,1));
    }
}
