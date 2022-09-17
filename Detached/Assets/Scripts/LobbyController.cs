using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    //UI Elements
    public Text LobbyNameText;

    //PlayerData
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

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
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach(PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if(PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.SetPlayerValues();
                }
            }
        }

    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach(PlayerListItem playerlistItem in PlayerListItems)
        {
            if(!Manager.GamePlayers.Any(b => b.ConnectionID == playerlistItem.ConnectionID))
            {
                playerListItemToRemove.Add(playerlistItem);
            }
        }
        if(playerListItemToRemove.Count > 0)
        {
            foreach(PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject objectToRemove = playerlistItemToRemove.gameObject;
                PlayerListItems.Remove(playerlistItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

    public void Start()
    {
        FindLocalPlayer();
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = localPlayerController.GetComponent<PlayerObjectController>();
    }

    public void StartGame()
    {
        LocalPlayerObject.GetComponent<PlayerObjectController>().canStartGame(GameScene);
    }
}
