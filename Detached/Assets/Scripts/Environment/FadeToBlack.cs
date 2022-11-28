using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void StartFade(float fadeTime) => image.CrossFadeAlpha(255, fadeTime, true);
}
