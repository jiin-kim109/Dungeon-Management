using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoad {

    private static string saveDataPath = Application.persistentDataPath + "/saveDM.gd";

    public static void SavePlayerData(PlayerData playerData)
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);
        }

        string jsonString = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(saveDataPath, jsonString);
    }
    public static PlayerData LoadPlayerData(PlayerData playerData)
    {
        try
        {
            if (File.Exists(saveDataPath))
            {
                string jsonString = File.ReadAllText(saveDataPath);
                JsonUtility.FromJsonOverwrite(jsonString, playerData);
            }
            else
            {
                playerData = new PlayerData();
            }
        }
        catch (FileNotFoundException e) {
        }
        return playerData;
    }

    public static void DeletePlayerData()
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);
        }
    }
}
