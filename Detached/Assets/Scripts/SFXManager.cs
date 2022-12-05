using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using Mirror;

public static class SFXManager
{
    public static float SFXVolume = 0.3f;
    public static string WalkSound => "event:/SFX/Walking";
    public static string AttachSound => "event:/SFX/Attach";
    public static string DetachSound => "event:/SFX/Detach";
    public static string JumpSound => "event:/SFX/Jump";
    public static string PullLeverSound => "event:/SFX/PullLever";
    public static string PushButtonSound => "event:/SFX/PushButton";
    public static string ThrowSound => "event:/SFX/Throw";

    /// <summary>
    /// Converts the path to a GUID and then calls another method to play the event.
    /// </summary>
    /// <param name="path">Path to the FMOD event</param>
    /// <param name="position">The position of the object that is going to be emitting the sound</param>
    public static void PlayOneShot(string path, float volume, Vector3 position)
    {
        try
        {
            PlayOneShot(RuntimeManager.PathToGUID(path), volume, position);
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
    public static void PlayOneShot(FMOD.GUID guid, float volume, Vector3 position)
    {
        var instance = RuntimeManager.CreateInstance(guid);
        instance.setVolume(SFXVolume);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        Debug.Log($"transform position: X: {position.x} Y: {position.y} Z: {position.z}");
        //instance.release();
    }

    public static void PlayOneShotAttached(EventReference eventReference, float volume, GameObject gameObject)
    {
        try
        {
            PlayOneShotAttached(eventReference.Guid, volume, gameObject);
        }
        catch (EventNotFoundException)
        {
            RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + eventReference);
        }
    }

    public static void PlayOneShotAttached(string path, float volume, GameObject gameObject)
    {
        try
        {
            PlayOneShotAttached(RuntimeManager.PathToGUID(path), volume, gameObject);
        }
        catch (EventNotFoundException)
        {
            RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
        }
    }
    public static void PlayOneShotAttached(FMOD.GUID guid, float volume, GameObject gameObject)
    {
        var instance = RuntimeManager.CreateInstance(guid);
#if UNITY_PHYSICS_EXIST
            AttachInstanceToGameObject(instance, gameObject.transform, gameObject.GetComponent<Rigidbody>());
#elif UNITY_PHYSICS2D_EXIST
            AttachInstanceToGameObject(instance, gameObject.transform, gameObject.GetComponent<Rigidbody2D>());
#else
        RuntimeManager.AttachInstanceToGameObject(instance, gameObject.transform);
#endif
        instance.start();
        //instance.release();
    }
}
