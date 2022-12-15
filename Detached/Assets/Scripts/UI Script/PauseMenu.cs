using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEditor;

public class PauseMenu : MonoBehaviour
{

    [SerializeField]
    Dropdown resolutionDropdown;

    [SerializeField] private Toggle fullscreenToggle;
    public Toggle vSyncTog;

    Resolution[] resolutions;

    void Start()
    {
        ScreenResolution();

        if (QualitySettings.vSyncCount == 0)
        {
            vSyncTog.isOn = false;
        }
        else { vSyncTog.isOn = true; }
    }
    private void ScreenResolution()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscren()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }
    public void SetVolume(float volume)
    {
        //AudioManager.instance.SetVolume(volume);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
