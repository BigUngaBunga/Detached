using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanging : NetworkBehaviour
{
    //Maps
    public string[] Maps;
    public int NextMapIndex;

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
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    [Command(requiresAuthority = false)]
    public void cmdServerChangeScene()
    {
         Manager.ServerChangeScene(Maps[NextMapIndex]);
    }
}
