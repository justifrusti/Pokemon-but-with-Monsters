using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SaveData saveData;

    public PlayerController pc;

    private Material detectorMat1, detectorMat2;
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if(pc == null)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        if(instance == null)
        {
            instance = this;
        }
    }

    public void RegisterDetectorMat(Material mat1,  Material mat2)
    {
        instance.detectorMat1 = mat1;
        instance.detectorMat2 = mat2;
    }

    public void GetDetectorMat(Material mat1, Material mat2)
    {
        mat1 = instance.detectorMat1;
        mat2 = instance.detectorMat2;
    }
}
