using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerAuthentication : NetworkBehaviour
{
   
    public bool CheckIfPlayerIsLocalPlayer()
    {
        return isLocalPlayer;
    }

    public bool CheckIfPlayerIsHost()
    {
        return isServer;
    }
}
