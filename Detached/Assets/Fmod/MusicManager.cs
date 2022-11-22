using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    FMOD.Studio.EventInstance LooseScrews;
    void Start()
    {
        LooseScrews = FMODUnity.RuntimeManager.CreateInstance("event:/Loose Screws");
        LooseScrews.start();
        LooseScrews.setParameterByName("Intensity", 5f);
    }

    void Update()
    {
        LooseScrews.getParameterByName("Intensity", out float value);
        LooseScrews.setVolume(value / 100);
    }
}
