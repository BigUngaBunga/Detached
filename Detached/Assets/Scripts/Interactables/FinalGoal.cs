using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalGoal : Goal
{
    [SerializeField] float fadeDuration;
    private FadeToBlack fadeToBlack;
    private GameObject camera;
    private bool startedFade;

    protected override void Start()
    {
        base.Start();
        fadeToBlack = GameObject.Find("Screen fader").GetComponentInChildren<FadeToBlack>();
        camera = GameObject.Find("Camera");
        //DEBUG
        FadeToBlack();
    }

    public override void EvaluateVictory()
    {
        if (CheckVictoryStatus() && !startedFade)
        {
            camera.GetComponent<InteractionChecker>().AllowInteraction = false;
            FadeToBlack();
        }
    }

    private void FadeToBlack()
    {
        startedFade = true;
        fadeToBlack.StartFade(fadeDuration);
    }
}
