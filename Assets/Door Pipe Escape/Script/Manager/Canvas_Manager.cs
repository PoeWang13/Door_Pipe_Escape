using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Canvas_Manager : Singletion<Canvas_Manager>
{
    public event EventHandler OnLevelWin;
    public event EventHandler OnLevelLost;

    [Header("Keys")]
    [SerializeField] private GameObject objLevelFinish;
    [SerializeField] private GameObject objLevelSecretRoom;
    [Header("Keys")]
    [SerializeField] private GameObject objScorePanel;
    [SerializeField] private TextMeshProUGUI textScore;
    [Header("Level Finish")]
    [SerializeField] private TextMeshProUGUI textLevelFinish;
    [SerializeField] private GameObject levelFinish;
    [SerializeField] private GameObject buttonRestartLevel;
    [SerializeField] private GameObject buttonNextLevel;
    [Header("Level List")]
    [SerializeField] private Button buttonLevelPrefab;
    [SerializeField] private Transform levelListButtonParent;
    [SerializeField] private GameObject levelList;
    [Header("Genel")]
    [SerializeField] private Animation dustAnimation;

    private void Start()
    {
        for (int e = 0; e < Game_Manager.Instance.LevelAmount; e++)
        {
            Button levelButton = Instantiate(buttonLevelPrefab, levelListButtonParent);
            levelButton.interactable = e <= Save_Load_Manager.Instance.gameData.lastLevel;
            levelButton.name = "Button-Level-" + (e + 1);
            levelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Level-" + (e + 1);
            int levelNumber = e;
            levelButton.onClick.AddListener(() =>
            {
                Game_Manager.Instance.LevelLoad(levelNumber);
                levelList.SetActive(false);
            });
        }
        textScore.text = "Score : " + Save_Load_Manager.Instance.gameData.score;
    }
    public void HasLevelFinishKey(bool isActive)
    {
        objLevelFinish.SetActive(isActive);
    }
    public void HasLevelSecretRoomKey(bool isActive)
    {
        objLevelFinish.SetActive(isActive);
    }
    public void Dusted()
    {
        dustAnimation.Play();
    }
    public void LevelFinish(bool isWin)
    {
        Player.Instance.gameObject.SetActive(false);
        HasLevelFinishKey(false);
        HasLevelSecretRoomKey(false);
        levelFinish.SetActive(true);
        objScorePanel.SetActive(false);
        buttonRestartLevel.SetActive(!isWin);
        buttonNextLevel.SetActive(isWin);
        textLevelFinish.text = isWin ? "You finish level." : "You dead";
    }
    public void LevelWin()
    {
        if (Game_Manager.Instance.LevelNumber == Save_Load_Manager.Instance.gameData.lastLevel)
        {
            if (Save_Load_Manager.Instance.gameData.lastLevel + 1 < levelListButtonParent.childCount)
            {
                levelListButtonParent.GetChild(Save_Load_Manager.Instance.gameData.lastLevel + 1).GetComponent<Button>().interactable
                    = true;
            }
        }
        LevelFinish(true);
        
        OnLevelWin?.Invoke(this, EventArgs.Empty);
    }
    public void LevelLost()
    {
        LevelFinish(false);
        OnLevelLost?.Invoke(this, EventArgs.Empty);
    }
    // Assigned to Button_Level_Restart in Canvas.
    public void LevelRestart()
    {
        HasLevelFinishKey(false);
        HasLevelSecretRoomKey(false);
        levelFinish.SetActive(false);
        objScorePanel.SetActive(true);
        Game_Manager.Instance.LevelRestart();
    }
    // Assigned to Button_List_Level in Canvas.
    public void LevelListLoad()
    {
        levelFinish.SetActive(false);
        levelList.SetActive(true);
    }
    // Assigned to Button_Next_Level_Load in Canvas.
    public void NextLevelLoad()
    {
        levelFinish.SetActive(false);
        objScorePanel.SetActive(true);
        Game_Manager.Instance.NextLevelLoad();
    }
    private void SetScoreText()
    {
        DOTween.To(value =>
        {
            textScore.text = "Score : " + (Save_Load_Manager.Instance.gameData.score + (int)value);
        },
            startValue: 0,
            endValue: scoreAdd, duration: 0.25f)
            .OnComplete(() => { ClearMatchControlling(); });
    }
    private int scoreAdd;
    public void AddScore(int point)
    {
        scoreAdd += point;
        SetScoreText();
    }
    private void ClearMatchControlling()
    {
        Save_Load_Manager.Instance.gameData.score += scoreAdd;
        scoreAdd = 0;
    }
}