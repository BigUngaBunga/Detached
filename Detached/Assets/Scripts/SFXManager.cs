using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

public class SFXManager
{
    public float SFXVolume { get; private set; }
    public string WalkSound => "event:/SFX/Walking";
    public string AttachSound => "event:/SFX/Attach";
    public string DetachSound => "event:/SFX/Detach";
    public string JumpSound => "event:/SFX/Jump";
    public string PullLeverSound => "event:/SFX/PullLever";
    public string PushButtonSound => "event:/SFX/PushButton";
    public string ThrowSound => "event:/SFX/Throw";

    /// <summary>
    /// Converts the path to a GUID and then calls another method to play the event.
    /// </summary>
    /// <param name="path">Path to the FMOD event</param>
    /// <param name="position">The position of the object that is going to be emitting the sound</param>
    public void PlayOneShot(string path, Vector3 position)
    {
        try
        {
            PlayOneShot(RuntimeManager.PathToGUID(path), position);
        }
        catch (EventNotFoundException)
        {
            RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
        }
    }

    /// <summary>
    /// Plays a sound once and then releases it
    /// </summary>
    /// <param name="guid">Reference to the event in FMOD</param>
    /// <param name="position">The position of the object that is going to be emitting the sound</param>
    public void PlayOneShot(FMOD.GUID guid, Vector3 position)
    {
        var instance = RuntimeManager.CreateInstance(guid);
        instance.setVolume(SFXVolume);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        instance.release();
    }

    /// <summary>
    /// Changes the volume of all future SFX
    /// </summary>
    /// <param name="volume">value between 0-1</param>
    public void ChangeSFXVolume(float volume)
    {
        SFXVolume = volume;
    }
}
