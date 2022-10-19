using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    public bool debug;

    //Callbacks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<GameRichPresenceJoinRequested_t> gameRichJoinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyInvite_t> lobbyInvited;
    

    public ulong currentLobbyID;
    public const string HostAdressKey = "HostAddress";
    private CustomNetworkManager manager;
   

    private void Start()
    {
        if (!SteamManager.Initialized) { return; }
        if(Instance == null) { Instance = this; }

        manager = GetComponent<CustomNetworkManager>();

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        gameRichJoinRequest = Callback<GameRichPresenceJoinRequested_t>.Create(onJoinGame);


    }   

    public void HostLobby()
    {
        if (debug)
        {
            Debug.Log("Hosting lobby");
        }
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        if (debug)
        {
            Debug.Log("Lobby creates susccesfully");
        }

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s lobby");
    }

    private void onJoinGame(GameRichPresenceJoinRequested_t callback)
    {
        Debug.Log("Request join game");
        
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Everyone
        currentLobbyID = callback.m_ulSteamIDLobby;     

        //Checks wether this is server
        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey);
        manager.StartClient();
    }   
}
