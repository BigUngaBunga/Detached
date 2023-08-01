using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerAuthentication : NetworkBehaviour
{
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
    public bool CheckIfPlayerIsLocalPlayer()
    {
        return isLocalPlayer;
    }

    public bool CheckIfPlayerIsHost()
    {
        return isServer;
    }
    [Command]
    public void CmdServerChangeScene(string newSceneName)
    {
        Manager.ServerChangeScene(newSceneName);
    }
}
