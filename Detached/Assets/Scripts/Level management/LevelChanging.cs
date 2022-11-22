using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanging : NetworkBehaviour
{
    //Add level names in GlobalLevelIndex instead
    [SerializeField] private int NextMapIndex;

    //Manager
    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    [Command(requiresAuthority = false)]
    public void cmdServerChangeScene()
    {
         Manager.ServerChangeScene(GlobalLevelIndex.GetLevelZeroIndex(NextMapIndex));
    }
}
