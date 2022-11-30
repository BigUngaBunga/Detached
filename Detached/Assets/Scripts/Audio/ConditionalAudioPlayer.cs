using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using FMOD.Studio;
using FMOD;
using System.Reflection;
using UnityEngine.Events;

public class ConditionalAudioPlayer : NetworkBehaviour
{
    enum RequiredPlayers { None, One, Both};
    enum SayingLine { Deta, Ched, Other};

    [Header("Requirements to play")]
    [SerializeField] private RequiredPlayers requiredPlayers;
    [SerializeField] private bool playAtStart;
    [SerializeField] private float playAtStartDelay;
    [SerializeField] private float replyDelay;
    [SerializeField] private bool wasPlayed;
    [SerializeField] private int playerNumber;

    private int PlayerNumber 
    { get { return playerNumber; } 
      set { playerNumber = value; EvaluateIfPlaySound(); } 
    }
    //[SyncVar(hook = nameof(OnChangePlayerNumber))]
    [SerializeField] private LineOfDialogue[] audioChain;

    private GameObject deta, ched;

    private void Start()
    {
        deta = GameObject.Find("Deta(Clone)");
        ched = GameObject.Find("Ched(Clone)");
        if (playAtStart)
            StartCoroutine(PlayAudioDelayed(playAtStartDelay));
    }

    //[Command(requiresAuthority = false)]
    //private void CMDPlayAudio(int index) => RPCPLayAudio(index);

    //[ClientRpc]
    //private void RPCPLayAudio(int index)
    //{
        
    //}

    //private void OnChangePlayerNumber(int oldValue, int newValue)
    //{
    //    if (CorrectNumberOfPlayers(newValue))
    //        StartCoroutine(PlayAudioChain());
    //}

    private void EvaluateIfPlaySound()
    {
        if (CorrectNumberOfPlayers(playerNumber))
            StartCoroutine(PlayAudioChain());
    }

    private bool CorrectNumberOfPlayers(int numberOfPlayers) => !wasPlayed && (int)requiredPlayers == numberOfPlayers;

    
    private IEnumerator PlayAudioChain()
    {
        wasPlayed = true;
        for (int i = 0; i < audioChain.Length; i++)
        {
            //RPCPLayAudio(i);
            
            RuntimeManager.PlayOneShotAttached(audioChain[i].path, GetSource(audioChain[i].sayingLine));
            audioChain[i].eventInstance.getDescription(out EventDescription description);
            description.getLength(out int length);
            yield return new WaitForSeconds(length + replyDelay);
        }
    }

    private IEnumerator PlayAudioDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(PlayAudioChain());
    }

    private GameObject GetSource(SayingLine sayingLine)
    {
        return sayingLine switch
        {
            SayingLine.Deta => deta,
            SayingLine.Ched => ched,
            _ => gameObject,
        };
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

    [Serializable]
    private struct LineOfDialogue
    {
        public string path;
        public SayingLine sayingLine;
        public EventInstance eventInstance;

        public LineOfDialogue(string path, SayingLine sayingLine)
        {
            this.path = path;
            this.sayingLine = sayingLine;
            eventInstance = RuntimeManager.CreateInstance(path);
        }
    }

}
