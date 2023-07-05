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

    public string lvlToInitialize;

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

    private void Start()
    {
        if(includeSettings)
        {
            GetResolutions();
            
            LoadSettings();
        }
    }

    public void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;

        lvlToInitialize = GameManager.instance.saveData.continueLvl;

        LoadLevel(lvlToInitialize);
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
        GameManager.instance.Save();

        SceneManager.LoadScene(levelName);
    }

    public void SaveSettings()
    {
        GameManager.instance.saveData.width = width;
        GameManager.instance.saveData.height = height;
        GameManager.instance.saveData.resolutionIndex = resolutionIndex;
        GameManager.instance.saveData.masterVolume = masterVolume;
        GameManager.instance.saveData.musicVolume = musicVolume;
        GameManager.instance.saveData.sfxVolume = sfxVolume;
        GameManager.instance.saveData.isFullscreen = isFullscreen;
        GameManager.instance.saveData.vSync = vSync;
        GameManager.instance.saveData.continueLvl = lvlToInitialize;

        GameManager.instance.Save();
    }

    public void LoadSettings()
    {
        GameManager.instance.Load();

        width = GameManager.instance.saveData.width;
        height = GameManager.instance.saveData.height;
        resolutionIndex = GameManager.instance.saveData.resolutionIndex;

        masterVolume = GameManager.instance.saveData.masterVolume;
        musicVolume = GameManager.instance.saveData.musicVolume;
        sfxVolume = GameManager.instance.saveData.sfxVolume;

        isFullscreen = GameManager.instance.saveData.isFullscreen;
        vSync = GameManager.instance.saveData.vSync;

        lvlToInitialize = GameManager.instance.saveData.continueLvl;

        ApplySettings();
    }

    public void ApplySettings()
    {
        fullscreenT.isOn = isFullscreen;
        vSyncT.isOn = vSync;

        masterVolS.value = masterVolume;
        musicVolS.value = musicVolume;
        sfxVolS.value = sfxVolume;

        SetResolution(resolutionIndex);
    }
}
