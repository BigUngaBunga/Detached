using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;


public class CustomNetworkManager : NetworkManager
{
    [SerializeField]private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == "SteamLobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(playerPrefab.GetComponent<PlayerObjectController>());
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
        foreach(PlayerObjectController player in GamePlayers)
        {
            
            var gamePlayerInstance = Instantiate(GamePlayerPrefab);

            NetworkServer.Destroy(player.connThis.identity.gameObject);
            NetworkServer.ReplacePlayerForConnection(player.connThis, gamePlayerInstance.gameObject);
        }

        base.ServerChangeScene(newSceneName);
    }
}
