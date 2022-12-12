using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private string SceneName;
    private int currentIndex = 0, lastIndex = 0;
    private bool hasChangedSong = true;

    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }


    [SerializeField]
    private FMODUnity.EventReference eventName = new FMODUnity.EventReference();

    FMOD.Studio.EVENT_CALLBACK beatCallback;

    private FMOD.Studio.EventInstance[] musicInstances;
    private TimelineInfo[] timelineInfoArray;
    private GCHandle[] timelineHandleArray;

    void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("MusicManager");

        // This just destroys itself if there already is a MusicManager in the scene.
        if (objects.Length > 1)
        {
            Destroy(this.gameObject);
        }


        musicInstances = new FMOD.Studio.EventInstance[6]
        {
            FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_MAIN_MENU"),
            FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG2"),
            FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG3"),
            FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG4"),
            FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG6"),
            FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG7"),
        };

        timelineInfoArray = new TimelineInfo[6]
        {
            new TimelineInfo(),
            new TimelineInfo(),
            new TimelineInfo(),
            new TimelineInfo(),
            new TimelineInfo(),
            new TimelineInfo()
        };


        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        // Pin the classes that will store the data modified during the callback 
        timelineHandleArray = new GCHandle[6]
        {
            GCHandle.Alloc(timelineInfoArray[0]),
            GCHandle.Alloc(timelineInfoArray[1]),
            GCHandle.Alloc(timelineInfoArray[2]),
            GCHandle.Alloc(timelineInfoArray[3]),
            GCHandle.Alloc(timelineInfoArray[4]),
            GCHandle.Alloc(timelineInfoArray[5])

        };

        for (int i = 0; i < musicInstances.Length; i++)
        {
            IntializeMusicInstance(i);
        }
        musicInstances[currentIndex].start();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Intensity", 100f);
        SceneName = SceneManager.GetActiveScene().name;
        DontDestroyOnLoad(this);
    }

    private void IntializeMusicInstance(int index)
    {
        // Pass the object through the userdata of the instance
        musicInstances[index].setUserData(GCHandle.ToIntPtr(timelineHandleArray[index]));

        musicInstances[index].setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        musicInstances[index].setVolume(VolumeManager.GetMusicVolume());

    }

    void OnDestroy()
    {
        foreach (FMOD.Studio.EventInstance song in musicInstances)
        {
            song.setUserData(IntPtr.Zero);
            song.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            song.release();
        }
        foreach (GCHandle timelineHandle in timelineHandleArray)
        {
            timelineHandle.Free();
        }

    }

    public void UpdateVolume(float volume)
    {
        foreach (EventInstance song in musicInstances)
        {
            song.setVolume(volume);
        }
    }

    private void Update()
    {
        if (musicInstances.Length == 0) return;

        // Check too see if the last song has reached its endpoint if it has stop it
        musicInstances[lastIndex].getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED && !hasChangedSong)
        {
            hasChangedSong = true;
            musicInstances[lastIndex].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        // Change song whenever the scenes switch
        if (SceneName != SceneManager.GetActiveScene().name)
        {
            SceneName = SceneManager.GetActiveScene().name;
            musicInstances[currentIndex].setParameterByName("EndSong", 100);
        }

        // If the song has reached the end of the looping region then start the next song in queue.
        if ((string)timelineInfoArray[currentIndex].lastMarker == "End" && hasChangedSong )
        {
            hasChangedSong = false;
            if (currentIndex >= musicInstances.Length-1)
            {
                lastIndex = currentIndex;
                currentIndex = 1;
                musicInstances[currentIndex].start();
            }
            else
            {
                lastIndex = currentIndex;
                currentIndex++;
                musicInstances[currentIndex].start();
            }
            
        }
        musicInstances[currentIndex].getVolume(out float volume);
        if (volume != VolumeManager.MusicVolume)
        {
            UpdateVolume(VolumeManager.MusicVolume);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
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
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBar = parameter.bar;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
}
