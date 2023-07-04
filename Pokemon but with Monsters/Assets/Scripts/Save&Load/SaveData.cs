using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using TMPro;
using UnityEngine.Audio;

[System.Serializable]
public class SaveData
{
    [Header("Settings")]
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
}
