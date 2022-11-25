using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    List<EventInstance> songs = new List<EventInstance>();
    EventInstance currentSound;
    int currentIndex = 0;
    float time;
    float globalVolume = 0.3f;
    float timeBetweenSongs = 40.0f; //seconds
    void Start()
    {
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
