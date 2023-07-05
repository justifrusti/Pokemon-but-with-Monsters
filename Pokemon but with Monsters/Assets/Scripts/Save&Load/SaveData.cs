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
    [Space]
    public string continueLvl;

    //Player Stuff
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float sensitivity = 180f;
    public float turnSpeed = 240f;

    [Header("Jump")]
    public float jumpForce = 5f;

    [Header("Interact")]
    public float pickupRange = 1.0f;

    [Header("Health")]
    public int maxHP;
    public int currentHP;
    [Space]
    public float invisFrameTime = 1.2f;

    [Header("Leaning")]
    public float leanAngle = 20f;
    public float leanSpeed = 10f;

    [Header("Crouching")]
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 10f;

    [Header("Inventory")]
    public Item invItem1;
    public Item invItem2;
    public Item invItem3;

    public Sprite i1s;
    public Sprite i2s;
    public Sprite i3s;

    [Header("AmuletStats")]
    public int amuletHPBoost;
    public float amuletSpeedBoost;
    public float amuletStealthBoost;
}
