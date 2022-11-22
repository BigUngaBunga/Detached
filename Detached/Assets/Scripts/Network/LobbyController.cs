using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyController : NetworkBehaviour
{
    public static LobbyController Instance;

    //UI Elements
    //public Text LobbyNameText;
    public Button StartGameButton;
    public Text ReadyButtonText;
    public Button Invite;
    public Text LobbyNameText;

    //PlayerData
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;
    public GameObject friendListPrefab;
    public GameObject friendListViewContent;
    private List<GameObject> friendsInContentView = new List<GameObject>();

    //OtherData
    public ulong CurrentLobbyId;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController localPlayerController;

    //Manager
    private CustomNetworkManager manager;
    public string GameScene;

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

    public void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    public void Start()
    {
        InvokeRepeating("GetFriendsPlaying", 1, 1); //Updates the invite friendslist
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyId = Manager.GetComponent<SteamLobby>().currentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyId), "name");
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated) { CreateHostPlayerItem(); } //host
        if (PlayerListItems.Count < Manager.GamePlayers.Count) { CreateClientPlayerItem(); }
        if (PlayerListItems.Count > Manager.GamePlayers.Count) { RemovePlayerItem(); }
        if (PlayerListItems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem(); }

    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.SetPlayerValues();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }
    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }
    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers) // error här
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.SetPlayerValues();
                    if(player == localPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }

        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerlistItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerlistItem.ConnectionID))
            {
                playerListItemToRemove.Add(playerlistItem);
            }
        }
        if (playerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove) //throws error here
            {
                GameObject objectToRemove = playerlistItemToRemove.gameObject;
                PlayerListItems.Remove(playerlistItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void StartGame(string SceneName)
    {
        localPlayerController.canStartGame(SceneName);
    }

    public void StartGame(int levelNumber) => StartGame(GlobalLevelIndex.GetLevel(levelNumber));

    public void ReadyPlayer()
    {
        localPlayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (localPlayerController.Ready)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;

        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (player.Ready)
            {
                AllReady = true;
            }
            else
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if (localPlayerController.PlayerIdNumber == 1)
            {
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }

    public void GetFriendsPlaying()
    {
        ClearPlayerInviteList();

        int numberOfFriends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate); //Gets int of "reguler" friends
        CGameID thisGameId = new CGameID(SteamUtils.GetAppID());

        if (numberOfFriends == -1) //Good practice according to documentation
        {
            numberOfFriends = 0;
        }

        for (int i = 0; i < numberOfFriends; i++)
        {
            FriendGameInfo_t friendGameInfo;
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            SteamFriends.GetFriendGamePlayed(friendSteamId, out friendGameInfo);

            if(friendGameInfo.m_gameID == thisGameId)
            {
                AddPlayerToInviteList(friendSteamId);
            }
        }
    }
    private void ClearPlayerInviteList()
    {
        foreach(GameObject friend in friendsInContentView)
        {
            Destroy(friend);
        }
    }

    private void AddPlayerToInviteList(CSteamID friendSteamId)
    {
        GameObject friend = Instantiate(friendListPrefab, friendListViewContent.transform);
        FriendObjectScript friendObjectScript = friend.GetComponent<FriendObjectScript>();
        friendObjectScript.ID = friendSteamId;
        friendObjectScript.Name = SteamFriends.GetFriendPersonaName(friendSteamId);
        friendsInContentView.Add(friend);

    }

    public void InvitePlayerToGame(CSteamID friendSteamId)
    {
        if (friendSteamId == null) return;
      
        CSteamID lobby = new CSteamID(Manager.GetComponent<SteamLobby>().currentLobbyID);  
        SteamMatchmaking.InviteUserToLobby(lobby, friendSteamId);
    }

    public void BackButton()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(localPlayerController.PlayerSteamID));

        if (isServer)
            CustomNetworkManager.singleton.StopHost();

        else if (isClient)
        {
            CmdPlayerDisconnected();
            CustomNetworkManager.singleton.StopClient();
        }
    }

    [Command]
    public void CmdPlayerDisconnected()
    {
        RpcPlayerDisconnected();
    }

    [ClientRpc]
    public void RpcPlayerDisconnected()
    {
        UpdatePlayerList();
    }


    public void StopServer()
    {
        if (isServer)
        {
            Manager.CustomStopServer();
        }
        if (isClient)
        {
            Manager.CustomStopClient();
        }

        UpdatePlayerList();
    }  
}
