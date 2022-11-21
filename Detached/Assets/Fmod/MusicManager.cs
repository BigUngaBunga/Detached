using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    List<FMOD.Studio.EventInstance> songs = new List<FMOD.Studio.EventInstance>();
    FMOD.Studio.EventInstance currentSound;
    int currentIndex = 0;
    float time;
    void Start()
    {
        songs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_MAIN_MENU"));
        songs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG2"));
        songs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG3"));
        songs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG4"));
        songs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG6"));
        songs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_SONG7"));


        currentSound = FMODUnity.RuntimeManager.CreateInstance("event:/SoundTrack/ST_MAIN_MENU");
        currentSound.start();
        currentSound.setVolume(0.3f);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Intensity", 10f);
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        float newVal = 0;
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName("Intensity", out float value);
        if (Input.GetKeyDown(KeyCode.UpArrow) && value < 100f )
        {
            newVal = value + 10f;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Intensity", newVal);
            Debug.Log(newVal);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && value > 0f)
        {
            newVal = value - 10f;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Intensity", newVal);
            Debug.Log(newVal);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && value > 0f)
        {
            currentSound.setParameterByName("EndSong", 1f);
            currentIndex++;
            if (currentIndex >= songs.Count)
            {
                currentIndex = 1;
                currentSound = songs[currentIndex];
                currentSound.start();
                currentSound.setVolume(0.3f);
            }
            else
            {
                currentSound = songs[currentIndex];
                currentSound.start();
                currentSound.setVolume(0.3f);
            }
        }
        currentSound.getVolume(out float volume);
        if (volume < 0.1f)
        {

        }
    }
}
