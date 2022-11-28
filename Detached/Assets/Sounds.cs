using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public static class Sounds
{
    public static EventInstance walkSound = RuntimeManager.CreateInstance("event:/SFX/Walking");
    public static EventInstance attachSound = RuntimeManager.CreateInstance("event:/SFX/Attach");
    public static EventInstance detachSound = RuntimeManager.CreateInstance("event:/SFX/Detach");
    public static EventInstance jumpSound = RuntimeManager.CreateInstance("event:/SFX/Jump");
    public static EventInstance pullLeverSound = RuntimeManager.CreateInstance("event:/SFX/PullLever");
    public static EventInstance pushButtonSound = RuntimeManager.CreateInstance("event:/SFX/PushButton");
    public static EventInstance throwSound = RuntimeManager.CreateInstance("event:/SFX/Throw");
}
