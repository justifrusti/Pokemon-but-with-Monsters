using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public bool includeSettings;

    [Header("Settings Menu")]
    public int width;
    public int height;
    public int resolutionIndex;
    [Space]
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    [Space]
    public bool isFullscreen;
    public bool vSync;

    [Header("Settings Attributes")]
    public TMP_Dropdown resolutionDropdown;
    public Resolution[] resolutions;
    [Space]
    public AudioMixer mainMixer;
    [Space]
    public Toggle fullscreenT;
    public Toggle vSyncT;
    [Space]
    public Slider masterVolS;
    public Slider musicVolS;
    public Slider sfxVolS;

    private GameManager manager;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if(includeSettings)
        {
            GetResolutions();
        }
    }

    public void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        this.isFullscreen = isFullscreen;
    }

    public void SetVSync(bool vSyncCount)
    {
        if(vSyncCount)
        {
            QualitySettings.vSyncCount = 1;
            this.vSync = vSyncCount;
        }else
        {
            QualitySettings.vSyncCount = 0;
            this.vSync = vSyncCount;
        }
    }

    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        masterVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        sfxVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        musicVolume = volume;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        width = resolution.width;
        height = resolution.height;
        this.resolutionIndex = resolutionIndex;
    }

    public void GetResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;

            if (resolutions[i].width >= 854)
            {
                if (!options.Contains(option))
                {
                    options.Add(option);
                }
            }

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void SaveSettings()
    {
        manager.saveData.width = width;
        manager.saveData.height = height;
        manager.saveData.resolutionIndex = resolutionIndex;
        manager.saveData.masterVolume = masterVolume;
        manager.saveData.musicVolume = musicVolume;
        manager.saveData.sfxVolume = sfxVolume;
        manager.saveData.isFullscreen = isFullscreen;
        manager.saveData.vSync = vSync;

        manager.Save();
    }

    public void LoadSettings()
    {
        manager.Load();

        width = manager.saveData.width;
        height = manager.saveData.height;
        resolutionIndex = manager.saveData.resolutionIndex;

        masterVolume = manager.saveData.masterVolume;
        musicVolume = manager.saveData.musicVolume;
        sfxVolume = manager.saveData.sfxVolume;

        isFullscreen = manager.saveData.isFullscreen;
        vSync = manager.saveData.vSync;

        ApplySettings();
    }

    public void ApplySettings()
    {
        fullscreenT.isOn = isFullscreen;
        vSyncT.isOn = vSync;

        masterVolS.value = masterVolume;
        musicVolS.value = musicVolume;
        sfxVolS.value = sfxVolume;

        resolutionDropdown.value = resolutionIndex;
    }
}
