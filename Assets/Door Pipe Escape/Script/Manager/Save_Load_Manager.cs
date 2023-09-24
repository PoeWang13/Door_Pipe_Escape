using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{
    // Oyunun bilgilerinin tutulduğu class
    public int lastLevel;
    public int score;
}
public class Save_Load_File_Data_Handler
{
    // Nereye ve nasıl kayıt yapıalacağını belirleyen class
    private string directoryPath;
    private string fileName;
    private bool useSifre;
    private readonly string sifreName = "HuseyinEmreCAN";
    public Save_Load_File_Data_Handler(string directoryPath, string fileName, bool useSifre)
    {
        this.directoryPath = directoryPath;
        this.fileName = fileName;
        this.useSifre = useSifre;
    }
    public GameData LoadGame()
    {
        string fullDataPath = Path.Combine(directoryPath, fileName + ".kimex");
        GameData loadedData = null;
        if (File.Exists(fullDataPath))
        {
            try
            {
                string jsonData = "";
                using (FileStream stream = new FileStream(fullDataPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonData = reader.ReadToEnd();
                    }
                }
                if (useSifre)
                {
                    jsonData = SifrelemeYap(jsonData);
                }
                loadedData = JsonUtility.FromJson<GameData>(jsonData);
            }
            catch (Exception e)
            {

                Debug.LogError("Error happining when we try to load in " + fullDataPath + "\n" + "Error is " + e);
                throw;
            }
        }
        return loadedData;
    }
    public void SaveGame(GameData gameData)
    {
        string fullDataPath = Path.Combine(directoryPath, fileName + ".kimex");
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullDataPath));
            string jsonData = JsonUtility.ToJson(gameData, true);
            if (useSifre)
            {
                jsonData = SifrelemeYap(jsonData);
            }
            using (FileStream stream = new FileStream(fullDataPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jsonData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error happining when we try to save in " + fullDataPath + "\n" + "Error is " + e);
        }
    }
    public string SifrelemeYap(string gameData)
    {
        string sifreliData = "";
        for (int e = 0; e < gameData.Length; e++)
        {
            sifreliData += (char)(gameData[e] ^ sifreName[e % sifreName.Length]);
        }
        return sifreliData;
    }
}
public class Save_Load_Manager : Singletion<Save_Load_Manager>
{
    #region Instance
    public override void OnAwake()
    {
        save_Load_File_Data_Handler =
                    new Save_Load_File_Data_Handler(Application.persistentDataPath, fileName, useSifre);
        LoadGame();
    }
    #endregion
    [SerializeField] private string fileName;
    [SerializeField] private bool useSifre;
    public GameData gameData;
    private Save_Load_File_Data_Handler save_Load_File_Data_Handler;

    private void Start()
    {
        Canvas_Manager.Instance.OnLevelWin += Instance_OnLevelWin;
    }
    private void Instance_OnLevelWin(object sender, EventArgs e)
    {
        if (Game_Manager.Instance.LevelNumber == gameData.lastLevel)
        {
            gameData.lastLevel++;
            if (gameData.lastLevel == Game_Manager.Instance.LevelAmount)
            {
                gameData.lastLevel = Game_Manager.Instance.LevelAmount - 1;
            }
            SaveGame();
        }
    }
    #region Save-Load Game Fonksiyon
    private void LoadGame()
    {
        gameData = save_Load_File_Data_Handler.LoadGame();
    }
    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        save_Load_File_Data_Handler.SaveGame(gameData);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    #endregion
}