using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveManager
{
    public static string directory = "/Data/";
    public static string fileName = "SinFile.sin";

    public static void Save(SaveData so)
    {
        string dir = Application.persistentDataPath + directory;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string json = JsonUtility.ToJson(so);
        File.WriteAllText(dir + fileName, json);
    }

    public static SaveData Load()
    {
        string fullPath = Application.persistentDataPath + directory + fileName;

        SaveData so = new SaveData();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            so = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogError("Save file does not exist");
        }

        return so;
    }

    public static void DeleteData()
    {
        string fullPath = Application.persistentDataPath + directory + fileName;

        SaveData so = new SaveData();

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);

            RefreshEditor();

            Debug.Log("Removed Save File...");
        }
        else
        {
            Debug.LogError("No save file to remove present!");

            Save(so);
        }
    }

    public static void RefreshEditor()
    {
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
}