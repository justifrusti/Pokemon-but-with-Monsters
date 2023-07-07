using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using TMPro;
using UnityEngine.Audio;
using static PlayerController;

[System.Serializable]
public class SaveData
{
    [Header("Settings")]
    public int width = 1920;
    public int height = 1080;
    public int resolutionIndex;
    [Space]
    public float masterVolume = 1;
    public float musicVolume = 1;
    public float sfxVolume = 1;
    [Space]
    public bool isFullscreen = true;
    public bool vSync = false;
    [Space]
    public string continueLvl;

    [Header("Health")]
    public int maxHP = 100;
}
