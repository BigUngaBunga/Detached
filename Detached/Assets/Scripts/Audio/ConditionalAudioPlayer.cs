using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalAudioPlayer : NetworkBehaviour
{
    enum RequiredPlayers { None, One, Both};

    [SerializeField] private RequiredPlayers requiredPlayers;
    [SyncVar(hook = nameof(OnChangePlayerNumber))][SerializeField] private int playerNumber;
    [SerializeField] private bool wasPlayed;
    [SerializeField] private List<AudioSource> audioChain = new List<AudioSource>();


    private void OnChangePlayerNumber(int oldValue, int newValue)
    {
        if (CorrectNumberOfPlayers(newValue))
            StartCoroutine(PlayAudio());
    }

    private bool CorrectNumberOfPlayers(int numberOfPlayers) => !wasPlayed && (int)requiredPlayers == numberOfPlayers;

    //TODO spela upp audioChain sekventiellt
    private IEnumerator PlayAudio()
    {
        wasPlayed = true;
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerNumber++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerNumber--;
    }

}
