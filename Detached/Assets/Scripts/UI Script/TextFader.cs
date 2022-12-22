using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextFader : MonoBehaviour
{    
    [SerializeField] float fadeTimeOutTime;
    [SerializeField] float fadeHoldTime;
    [SerializeField] float fadeTimeInTime;
    [SerializeField] bool fadeAtStart;
    [SerializeField] Text text;
    private float fadeIncrementIn;
    private float fadeIncrementOut;
    private float fadeInTarget = 1;
    private float fadeOutTarget = 0;
    



    void Start()
    {
        string stringToDisplay = "";
        stringToDisplay += "Level " + (GlobalLevelIndex.GetLevelIndex() + 1) + ": " + GlobalLevelIndex.GetLevelZeroIndex(GlobalLevelIndex.GetLevelIndex());
        text.text = stringToDisplay;

        fadeIncrementIn = 1 / fadeTimeInTime;
        fadeIncrementOut = -1 / fadeTimeOutTime;

        if (fadeAtStart) StartCoroutine(StartFading());
    }

    private IEnumerator StartFading()
    {
        yield return StartCoroutine(Fade(fadeInTarget, fadeIncrementIn, true));
        yield return new WaitForSeconds(fadeHoldTime);
        yield return StartCoroutine(Fade(fadeOutTarget, fadeIncrementOut, false));
    }    

    private IEnumerator Fade(float targetAlpha, float fadeIncrement, bool fadeIn)
    {
        while (true)
        {
            Color color = text.color;
            color.a += fadeIncrement * Time.deltaTime;
            text.color = color;
            if (CheckFadeLevel(text.color.a, targetAlpha, fadeIn)) break;
            yield return null;
        }
    }

    private bool CheckFadeLevel(float alpha, float targetAlpha, bool fadeIn)
    {
        if (fadeIn) return alpha > targetAlpha;
        else return alpha < targetAlpha;
    }
}
