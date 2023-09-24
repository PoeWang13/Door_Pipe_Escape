using UnityEngine;
using System.Collections.Generic;

public class Game_Manager : Singletion<Game_Manager>
{
    [SerializeField] private List<GameObject> level_Managers = new List<GameObject>();
    private GameObject loadedLevel;
    private int levelNumber;
    public int LevelAmount { get { return level_Managers.Count; } }
    public int LevelNumber { get { return levelNumber; } }
    private void Start()
    {
        Canvas_Manager.Instance.OnLevelLost += Instance_OnLevelLost;
        Canvas_Manager.Instance.OnLevelWin += Instance_OnLevelWin;
    }
    private void Instance_OnLevelLost(object sender, System.EventArgs e)
    {
        DestroyLevelLoad();
    }
    private void Instance_OnLevelWin(object sender, System.EventArgs e)
    {
        DestroyLevelLoad();
    }
    [ContextMenu("Level Restart")]
    public void LevelRestart()
    {
        Destroy(loadedLevel);
        LevelLoad(levelNumber);
    }
    private void DestroyLevelLoad()
    {
        if (loadedLevel != null)
        {
            Destroy(loadedLevel);
        }
    }
    public void NextLevelLoad()
    {
        Destroy(loadedLevel);
        LevelLoad(Save_Load_Manager.Instance.gameData.lastLevel);
    }
    public void LevelLoad(int loadingLevelNumber)
    {
        loadedLevel = Instantiate(level_Managers[loadingLevelNumber]);
        levelNumber = loadingLevelNumber;
    }
}