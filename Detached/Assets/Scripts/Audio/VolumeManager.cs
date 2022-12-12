using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static float MasterVolume = 1.0f;
    public static float SFXVolume = 1.0f;
    public static float DialogueVolume = 1.0f;
    public static float MusicVolume = 1.0f;
    

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

    public static void SetMasterVolume(float value)
    {
        MasterVolume = value; 
        Debug.Log("Master volume: " + MasterVolume);
    }
    public static void SetSFXVolume(float value)
    {
        SFXVolume= value;
        Debug.Log("SFX volume: " + SFXVolume);
    }
    public static void SetDialogueVolume(float value)
    {
        DialogueVolume= value;
        Debug.Log("Dialogue volume: " + DialogueVolume);
    }
    public static void SetMusicVolume(float value)
    {
        MusicVolume= value;
        Debug.Log("Music volume: " + MusicVolume);
    }
}
