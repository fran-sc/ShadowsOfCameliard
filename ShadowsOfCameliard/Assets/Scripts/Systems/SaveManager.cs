using System;
using System.IO;
using UnityEngine;

public class SaveManager : PersistentSingleton<SaveManager>
{
    const string SaveFileName = "savegame.json";

    SaveData saveData;
    string saveFilePath;

    public int LastUnlockedChapter => saveData.lastUnlockedChapter;

    protected override void Awake()
    {
        base.Awake();

        saveFilePath = Path.Combine(
            Application.persistentDataPath,
            SaveFileName
        );

        LoadGame();
    }

    public void UnlockChapter(int chapter)
    {
        if (chapter <= saveData.lastUnlockedChapter)
        {
            return;
        }

        saveData.lastUnlockedChapter = chapter;
        SaveGame();
    }

    public bool IsChapterUnlocked(int chapter)
    {
        return chapter <= saveData.lastUnlockedChapter;
    }

    public void SaveGame()
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);

            Debug.Log($"Partida guardada en: {saveFilePath}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"No se pudo guardar la partida: {exception.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            CreateNewGame();
            return;
        }

        try
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData == null)
            {
                CreateNewGame();
            }
        }
        catch (Exception exception)
        {
            Debug.LogError($"No se pudo cargar la partida: {exception.Message}");
            CreateNewGame();
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        CreateNewGame();
    }

    private void CreateNewGame()
    {
        saveData = new SaveData
        {
            lastUnlockedChapter = 1
        };

        SaveGame();
    }
}

[Serializable]
public class SaveData
{
    public int lastUnlockedChapter = 1;
}