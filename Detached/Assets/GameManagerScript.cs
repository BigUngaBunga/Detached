using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class GameManagerScript : NetworkBehaviour
{
    private bool PlayersSpawned;
    private GameObject[] spawnLocations;
    private GameObject[] players;

    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayersSpawned = false;
        spawnLocations = new GameObject[2];
        players = new GameObject[2];
        
        DontDestroyOnLoad(gameObject);
    }    

    void Update()
    {
        HandleSpawn();
    }

    private void HandleSpawn()
    {
        if (!PlayersSpawned)
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                spawnLocations = GameObject.FindGameObjectsWithTag("SpawnLocation");
                players = GameObject.FindGameObjectsWithTag("Player");

                for (int i = 0; i < players.Length; i++)
                {
                    players[i].transform.position = spawnLocations[i].transform.position;
                }
                PlayersSpawned = true;

            }
        }
    }
}
