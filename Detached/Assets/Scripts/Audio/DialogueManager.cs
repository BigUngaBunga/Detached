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

public class DialogueManager : MonoBehaviour
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

    private int currentIndex = 0;

    public float DialogueVolume { get; set;}

    private int PlayerNumber 
    { get { return playerNumber; } 
      set { playerNumber = value; EvaluateIfPlaySound(); } 
    }
    //[SyncVar(hook = nameof(OnChangePlayerNumber))]
    [SerializeField] private LineOfDialogue[] audioChain;
    private List<EventInstance> events = new List<EventInstance>();

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
            events.Add(RuntimeManager.CreateInstance(audioChain[i].path));
        }
        for (int i = 0; i < audioChain.Length; i++)
        {
            IntializeDialougeInstance(i);
        }
    }


    private void Update()
    {
        //Check if deta and ched are assigned before playing any sounds.
        if (!DetaChedAssignmentCheck()) return;

        if (audioChain.Length == 0 || currentIndex >= audioChain.Length) return;

        if (playAtStart)
            PlayAudioChain();

        Debug.Log(deta == null);
        // If the dialouge has reached the end then start the next dialogue option in queue.
        if ((string)timelineInfoList[currentIndex].lastMarker == "End")
        {
            currentIndex++;
            RuntimeManager.AttachInstanceToGameObject(events[currentIndex], GetSource(audioChain[currentIndex].sayingLine).transform);
            events[currentIndex].setVolume(VolumeManager.GetDialogueVolume());
            events[currentIndex].start();
        }
    }

    private bool DetaChedAssignmentCheck()
    {
        if (deta == null) deta = GameObject.Find("Deta(Clone)");
        if (ched == null) ched = GameObject.Find("Ched(Clone)");
        if (ched == null || deta == null) return false;
        return true;
    }

    private void EvaluateIfPlaySound()
    {
        if (CorrectNumberOfPlayers(playerNumber))
            PlayAudioChain();
    }

    private bool CorrectNumberOfPlayers(int numberOfPlayers) => !wasPlayed && (int)requiredPlayers == numberOfPlayers;

    private void PlayAudioChain()
    {
        playAtStart = false;
        RuntimeManager.AttachInstanceToGameObject(events[currentIndex], GetSource(audioChain[currentIndex].sayingLine).transform);
        events[currentIndex].setVolume(VolumeManager.GetDialogueVolume());
        events[currentIndex].start();
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
        events[index].setUserData(GCHandle.ToIntPtr(timelineHandleList[index]));

        events[index].setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        events[index].setVolume(DialogueVolume);
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
        for (int i = 0; i < events.Count; i++)
        {
            events[i].setUserData(IntPtr.Zero);
            events[i].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            events[i].release();
        }
        foreach (GCHandle timelineHandle in timelineHandleList)
        {
            timelineHandle.Free();
        }

    }
}
