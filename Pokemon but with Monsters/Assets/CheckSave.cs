using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CheckSave : MonoBehaviour
{
    public static string directory = "/Data/";
    public static string fileName = "SinFile.sin";

    private void Awake()
    {
        string dir = Application.persistentDataPath + directory + fileName;

        if(!File.Exists(dir))
        {
            GetComponent<Button>().interactable = false;
        }else
        {
            GetComponent<Button>().interactable = true;
        }
    }
}
