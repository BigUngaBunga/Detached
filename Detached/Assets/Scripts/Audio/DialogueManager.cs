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
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class DialogueManager : NetworkBehaviour
{
    enum RequiredPlayers { None, One, Both };
    enum SayingLine { Deta, Ched, Other };

    [Header("Requirements to play")]
    [SerializeField] private RequiredPlayers requiredPlayers;
    [SerializeField] private bool playAtStart;
    [SerializeField] private float playAtStartDelay;
    [SerializeField] private float replyDelay;
    [SerializeField] private bool wasPlayed;
    [SerializeField] private int playerNumber;

    private int currentIndex = 0, lastIndex = 0;

    public float DialogueVolume { get; set;}

    private int PlayerNumber 
    { get { return playerNumber; } 
      set { playerNumber = value; EvaluateIfPlaySound(); } 
    }
    //[SyncVar(hook = nameof(OnChangePlayerNumber))]
    [SerializeField] private LineOfDialogue[] audioChain;

    private GameObject deta, ched;
    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public StringWrapper lastMarker = new StringWrapper();
    }
    EVENT_CALLBACK beatCallback;
    private List<TimelineInfo> timelineInfoList = new List<TimelineInfo>();
    private List<GCHandle> timelineHandleList = new List<GCHandle>();

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

    void Start()
    {
        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new EVENT_CALLBACK(BeatEventCallback);

        // Pin the classes that will store the data modified during the callback 
        for (int i = 0; i < audioChain.Length; i++)
        {
            timelineInfoList.Add(new TimelineInfo());
            timelineHandleList.Add(GCHandle.Alloc(timelineInfoList[i]));
        }



        for (int i = 0; i < audioChain.Length; i++)
        {
            IntializeDialougeInstance(i);
        }

        deta = GameObject.Find("Deta(Clone)");
        ched = GameObject.Find("Ched(Clone)");
        if (playAtStart)
            PlayAudioChain();
    }


    private void Update()
    {
        if (audioChain.Length == 0 || currentIndex == audioChain.Length) return;

        // If the song has reached the end of the looping region then start the next song in queue.
        if ((string)timelineInfoList[currentIndex].lastMarker == "End")
        {
            //audioChain[lastIndex].eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            lastIndex = currentIndex;
            currentIndex++;
            Debug.Log("Playing dialogue");
            audioChain[currentIndex].eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(GetSource(audioChain[currentIndex].sayingLine).transform.position));
            //RuntimeManager.AttachInstanceToGameObject(audioChain[currentIndex].eventInstance, GetSource(audioChain[currentIndex].sayingLine).transform);
            audioChain[currentIndex].eventInstance.setVolume(VolumeManager.GetDialogueVolume());
            audioChain[currentIndex].eventInstance.start();
        }
    }

    private void EvaluateIfPlaySound()
    {
        if (CorrectNumberOfPlayers(playerNumber))
            PlayAudioChain();
    }

    private bool CorrectNumberOfPlayers(int numberOfPlayers) => !wasPlayed && (int)requiredPlayers == numberOfPlayers;

    private void PlayAudioChain()
    {
        Debug.Log("Playing dialogue");
        audioChain[currentIndex].eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(GetSource(audioChain[currentIndex].sayingLine).transform.position));
        //RuntimeManager.AttachInstanceToGameObject(audioChain[currentIndex].eventInstance, GetSource(audioChain[currentIndex].sayingLine).transform);
        audioChain[currentIndex].eventInstance.setVolume(VolumeManager.GetDialogueVolume());
        audioChain[currentIndex].eventInstance.start();
        //SFXManager.PlayOneShot(audioChain[currentIndex].path, VolumeManager.GetDialogueVolume(), GetSource(audioChain[currentIndex].sayingLine).transform.position);
        currentIndex++;
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
            PlayerNumber++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            PlayerNumber--;
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
            eventInstance = RuntimeManager.CreateInstance(RuntimeManager.PathToGUID(path));
        }
    }

    private void IntializeDialougeInstance(int index)
    {
        // Pass the object through the userdata of the instance
        audioChain[index].eventInstance.setUserData(GCHandle.ToIntPtr(timelineHandleList[index]));

        audioChain[index].eventInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        audioChain[index].eventInstance.setVolume(DialogueVolume);
    }

    void OnGUI()
    {
        GUILayout.Box(String.Format("Current Bar = {0}, Last Marker = {1}, Song index {2}, Last index {3}",
        timelineInfoList[currentIndex].currentMusicBar, (string)timelineInfoList[currentIndex].lastMarker, currentIndex, lastIndex));
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBar = parameter.bar;
                    }
                    break;
                case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }
        return RESULT.OK;
    }

    void OnDestroy()
    {
        for (int i = 0; i < audioChain.Length; i++)
        {
            audioChain[i].eventInstance.setUserData(IntPtr.Zero);
            audioChain[i].eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            audioChain[i].eventInstance.release();
        }
        foreach (GCHandle timelineHandle in timelineHandleList)
        {
            timelineHandle.Free();
        }

    }
}
