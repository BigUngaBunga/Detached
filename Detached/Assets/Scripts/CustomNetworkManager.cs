using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using Mirror.FizzySteam;
using System;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController LobbyPrefab;
    [SerializeField] private GameObject playerSpawnSystem;
    [SerializeField] public GameObject gamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public static event Action<NetworkConnectionToClient> OnServerReadied;

    public override void Awake()
    {
        //When back is pressed (go back from lobby) the networkManager loses it connection to the transport.
        transport = FindObjectOfType<FizzySteamworks>();

        base.Awake();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == "SteamLobby")
        {

            PlayerObjectController GamePlayerInstance = Instantiate(LobbyPrefab);
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

    public void CustomStopServer()
    {
        StopHost();
    }

    public void CustomStopClient()
    {
        StopClient();
    }
}
