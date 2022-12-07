using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LoadUI : NetworkBehaviour
{
    public GameObject gameUI;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameUI);
     
    }


    // Update is called once per frame
    void Update()
    {
        if (gameObject == NetworkClient.localPlayer.gameObject)
        {
            gameUI.SetActive(true);
        }
        else
        {
            gameUI.SetActive(false);
        }

    }

}
