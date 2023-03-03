using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : View
{
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private GameObject joystickGameObject;

    protected override void Start()
    {
        base.Start();

        backButton.onClick.AddListener(OnBackButtonClicked);

        LevelManager.Instance.OnLoadLevel += LevelManager_OnLoadLevel;
        LevelManager.Instance.OnStartLevel += LevelManager_OnStartLevel;
        LevelManager.Instance.OnFinishLevel += LevelManager_OnFinishLevel;
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButtonClicked);

        LevelManager.Instance.OnLoadLevel -= LevelManager_OnLoadLevel;
        LevelManager.Instance.OnStartLevel -= LevelManager_OnStartLevel;
        LevelManager.Instance.OnFinishLevel -= LevelManager_OnFinishLevel;
    }

    private void OnBackButtonClicked()
    {
        GameManager.Instance.ExitGame();
    }

    private void LevelManager_OnLoadLevel(object sender, EventArgs args)
    {
        Hide();
    }

    private void LevelManager_OnStartLevel(object sender, EventArgs args)
    {
        Show();
    }

    private void LevelManager_OnFinishLevel(object sender, LevelManager.OnFinishLevelArgs args)
    {
        Hide();
    }
}
