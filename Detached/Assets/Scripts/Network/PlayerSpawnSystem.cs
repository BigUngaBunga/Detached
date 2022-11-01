using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using System;

public class PlayerSpawnSystem : NetworkBehaviour {

    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();
    private static List<GameObject> spawnPointsObj = new List<GameObject>();

    private int nextIndex = 0;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    public static void AddSpawnPoints(GameObject spawnObj)
    {
        spawnPoints.Add(spawnObj.transform);

        spawnPointsObj.Add(spawnObj);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        spawnPointsObj = spawnPointsObj.OrderBy(x => x.transform.GetSiblingIndex()).ToList();

    }

    public static void RemoveSpawnPoints(GameObject spawnObj)
    {
        spawnPoints.Remove(spawnObj.transform);
        spawnPointsObj.Remove(spawnObj);

    }

    public override void OnStartServer()
    {
        playerPrefab = Manager.gamePlayerPrefab;
        CustomNetworkManager.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        CustomNetworkManager.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnectionToClient conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
        SpawnPoint spawnPointScript = spawnPointsObj.ElementAtOrDefault(nextIndex).GetComponent<SpawnPoint>();



        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        playerInstance.GetComponent<ItemManager>().SetAmountOfLimbsToSpawn(spawnPointScript.numOfArms, spawnPointScript.numOfLegs);
        if (conn.identity != null)
        NetworkServer.Destroy(conn.identity.gameObject);
        NetworkServer.ReplacePlayerForConnection(conn, playerInstance);

        nextIndex++;
    }   
}
