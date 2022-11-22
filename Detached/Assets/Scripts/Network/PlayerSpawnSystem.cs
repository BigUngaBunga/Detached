using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using System;

public class PlayerSpawnSystem : NetworkBehaviour {

    [SerializeField] private GameObject playerPrefabChed = null;
    [SerializeField] private GameObject playerPrefabDeta = null;
    

    private GameObject playerObjToSpawn = null;

    private static List<Transform> spawnPoints = new List<Transform>();
    private static List<GameObject> spawnPointsObj = new List<GameObject>();
    private List<GameObject> playerPrefabs = new List<GameObject>();


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
    }

    public static void RemoveSpawnPoints(GameObject spawnObj)
    {
        spawnPoints.Remove(spawnObj.transform);
        spawnPointsObj.Remove(spawnObj);
    }

    public override void OnStartServer()
    {
        playerPrefabs.Add(playerPrefabDeta);
        playerPrefabs.Add(playerPrefabChed);       
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
        playerObjToSpawn = playerPrefabs.ElementAtOrDefault(nextIndex);
        SpawnPoint spawnPointScript = spawnPointsObj.ElementAtOrDefault(nextIndex).GetComponent<SpawnPoint>();

        GameObject playerInstance = Instantiate(playerObjToSpawn, spawnPoint.position, spawnPoint.rotation);
        ItemManager playerItemManager = playerInstance.GetComponent<ItemManager>();
        playerItemManager.SetAmountOfLimbsToSpawn(spawnPointScript.numOfArms, spawnPointScript.numOfLegs);
        playerItemManager.isDeta = Convert.ToBoolean((nextIndex) % 2); // 1 = true = deta, 0 = false = ched

        if (conn.identity != null)
        NetworkServer.Destroy(conn.identity.gameObject);
        NetworkServer.ReplacePlayerForConnection(conn, playerInstance);

        nextIndex++;
    }
}
