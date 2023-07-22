using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using TMPro;

public class FriendObjectScript : MonoBehaviour
{
    private CSteamID id;
    private string name;
    [SerializeField] private TextMeshProUGUI textObjname;


    public CSteamID ID { get { return id; } set { id = value; } }
    public string Name { get { return name;  } set { name = value; textObjname.text = value; } }

    public void OnClick()
    {
        LobbyController.Instance.InvitePlayerToGame(ID);
    }
}
