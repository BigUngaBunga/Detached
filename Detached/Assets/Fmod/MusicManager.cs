using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    List<EventInstance> songs = new List<EventInstance>();
    EventInstance currentSound;
    private FMOD.Studio.EVENT_CALLBACK songCallback;

    private FMODUnity.EventReference EventName;

    private int currentIndex = 0;
    private float time;
    private float globalVolume = 0.1f;
    private float timeBetweenSongs = 40.0f; //seconds
    void Start()
    {
        songCallback = new FMOD.Studio.EVENT_CALLBACK(SongEventCallback);

        songs.Add(RuntimeManager.CreateInstance("event:/SoundTrack/ST_MAIN_MENU"));
        songs.Add(RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG2"));
        songs.Add(RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG3"));
        songs.Add(RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG4"));
        songs.Add(RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG6"));
        songs.Add(RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG7"));
        

        currentSound = RuntimeManager.CreateInstance("event:/SoundTrack/ST_MAIN_MENU");
        currentSound.start();
        currentSound.setVolume(globalVolume);
        RuntimeManager.StudioSystem.setParameterByName("Intensity", 10f);
        DontDestroyOnLoad(this);
    }

    void PlayDialogue(string key)
    {
        var dialogueInstance = RuntimeManager.CreateInstance(EventName);

        // Pin the key string in memory and pass a pointer through the user data
        GCHandle stringHandle = GCHandle.Alloc(key);
        dialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

        dialogueInstance.setCallback(SongEventCallback);
        dialogueInstance.start();
        dialogueInstance.release();
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK_TYPE))]
    static FMOD.RESULT SongEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance();
        // Retrieve the user data
        IntPtr stringPtr;
        instance.getUserData(out stringPtr);

        // Get the string object
        GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
        String key = stringHandle.Target as String;

        switch (type)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));

                    if (key.Contains("."))
                    {
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    else
                    {
                        SOUND_INFO dialogueSoundInfo;
                        var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                        if (keyResult != FMOD.RESULT.OK)
                        {
                            break;
                        }
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    break;
                }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new FMOD.Sound(parameter.sound);
                    sound.release();

                    break;
                }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                    stringHandle.Free();

                    break;
                }
        }

        return FMOD.RESULT.OK;
    }

    void Update()
    {
        float newVal = 0;
        RuntimeManager.StudioSystem.getParameterByName("Intensity", out float value);
        if (Input.GetKeyDown(KeyCode.UpArrow) && value < 100f )
        {
            newVal = value + 10f;
            RuntimeManager.StudioSystem.setParameterByName("Intensity", newVal);
            Debug.Log(newVal);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && value > 0f)
        {
            newVal = value - 10f;
            RuntimeManager.StudioSystem.setParameterByName("Intensity", newVal);
            Debug.Log(newVal);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || time >= timeBetweenSongs)
        {
            time = 0;
            currentSound.setParameterByName("EndSong", 1f);
            currentIndex++;
            if (currentIndex >= songs.Count)
            {
                currentIndex = 1;
                currentSound = songs[currentIndex];
                currentSound.start();
                currentSound.setVolume(globalVolume);
            }
            else
            {
                currentSound = songs[currentIndex];
                currentSound.start();
                currentSound.setVolume(globalVolume);
            }
        }
        currentSound.getVolume(out float volume);
        if (volume < 0.1f)
        {

        }
        time += Time.deltaTime;
    }
}
