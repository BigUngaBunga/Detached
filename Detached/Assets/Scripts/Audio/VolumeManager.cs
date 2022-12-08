using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static float MasterVolume = 0.3f;
    public static float SFXVolume = 0.3f;
    public static float DialogueVolume = 1.0f;
    public static float MusicVolume = 0.02f;

    public static float GetSFXVolume()
    {
        return MasterVolume * SFXVolume;
    }

    public static float GetDialogueVolume()
    {
        return MasterVolume * DialogueVolume;
    }

    public static float GetMusicVolume()
    {
        return MasterVolume * MusicVolume;
    }
}
