using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using System;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    [SerializeField] private GameObject playerSpawnSystem;
    [SerializeField] private String[] levelNames;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public static event Action<NetworkConnectionToClient> OnServerReadied;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == "SteamLobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.connThis = conn;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID) SteamLobby.Instance.currentLobbyID, GamePlayers.Count) ;

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);

        }
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);        
    }

    public void ChangeScene(int LevelID)
    {
        ServerChangeScene(levelNames[LevelID]);
    }

    public override void ServerChangeScene(string newSceneName)
    {       
        base.ServerChangeScene(newSceneName);          
    }

    [Server]
    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance); //server owns this
        }
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

}
