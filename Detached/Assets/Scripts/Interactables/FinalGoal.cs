using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalGoal : Goal
{
    private FadeScreen fadeToBlack;
    private bool startedFade;

    protected override void Start()
    {
        base.Start();
        fadeToBlack = GameObject.Find("Screen fader").GetComponentInChildren<FadeScreen>();
    }

    public override void EvaluateVictory()
    {
        if (CheckVictoryStatus() && !startedFade)
            FadeToBlack();
    }

    private void FadeToBlack()
    {
        startedFade = true;
        fadeToBlack.StartFade();
    }

    private void InactivateInteraction()
    {
        GameObject camera = GameObject.Find("Camera");
        camera.GetComponent<InteractionChecker>().AllowInteraction = false;
    }
}
