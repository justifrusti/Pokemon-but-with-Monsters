using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelData : MonoBehaviour
{
    private void Start()
    {
        string directory = "/Data/";
        string fileName = "SinFile.sin";

        string dir = Application.persistentDataPath + directory + fileName;

        if (File.Exists(dir))
        {
            GameManager.instance.saveData.continueLvl = SceneManager.GetActiveScene().name;

            GameManager.instance.pc.LoadData();
        }else
        {
            GameManager.instance.saveData.continueLvl = SceneManager.GetActiveScene().name;

            GameManager.instance.Save();
        }
    }
}
