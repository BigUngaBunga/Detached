using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using System;

public class PlayerSpawnSystem : NetworkBehaviour {

    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

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

    public static void AddSpawnPoints(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoints(Transform transform)
    {
        spawnPoints.Remove(transform);
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

        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        if(conn.identity != null)
        NetworkServer.Destroy(conn.identity.gameObject);
        NetworkServer.ReplacePlayerForConnection(conn, playerInstance);

        nextIndex++;
    }
}
