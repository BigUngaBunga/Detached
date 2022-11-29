using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    enum FadeType { FadeIn, FadeOut }
    [SerializeField] private FadeType fadeType;
    [SerializeField] float fadeSpeed;
    private Image image;
    private Color color;

    private void Start()
    {
        color = Color.black;
        if (fadeType == FadeType.FadeOut)
            color.a = 0;
        image = GetComponent<Image>();
        image.color = color;
    }

    public void StartFade()
    {
        float alphaTarget = 0;
        if (fadeType == FadeType.FadeOut)
            alphaTarget = 1;
        StartCoroutine(Fade(alphaTarget));
    }

    private IEnumerator Fade(float alphaTarget)
    {
        while (image.color.a != alphaTarget)
        {
            color.a = Mathf.Lerp(color.a, alphaTarget, fadeSpeed);
            image.color = color;
            yield return null;
        }
    }

    private bool IsCloseToTarget(float target, float value)
    {
        return fadeType switch
        {
            FadeType.FadeIn => value <= target,
            FadeType.FadeOut => value >= target,
            _ => false,
        };
    }
}
