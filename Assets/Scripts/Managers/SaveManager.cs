using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SAVE_FILE = "highscore.json";
    private string savePath;
    private int highScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE);
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadHighScore()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                highScore = data.highScore;
            }
            catch
            {
                highScore = 0;
            }
        }
    }

    private void SaveHighScore()
    {
        try
        {
            SaveData data = new SaveData { highScore = highScore };
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(savePath, json);
        }
        catch { }
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public bool UpdateHighScore(int newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore;
            SaveHighScore();
            return true;
        }
        return false;
    }

    private void OnApplicationQuit()
    {
        SaveHighScore();
    }
}

[System.Serializable]
public class SaveData
{
    public int highScore;
}
