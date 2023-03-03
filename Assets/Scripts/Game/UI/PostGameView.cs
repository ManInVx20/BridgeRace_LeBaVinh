using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameView : View
{
    [SerializeField]
    private GameObject victoryPanel;
    [SerializeField]
    private GameObject defeatPanel;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button tryAgainButton;
    [SerializeField]
    private Button exit1Button;
    [SerializeField]
    private Button exit2Button;

    protected override void Start()
    {
        base.Start();

        nextButton.onClick.AddListener(OnNextButtonClicked);
        tryAgainButton.onClick.AddListener(OnTryAgainButtonClicked);
        exit1Button.onClick.AddListener(OnExitButtonClicked);
        exit2Button.onClick.AddListener(OnExitButtonClicked);

        LevelManager.Instance.OnLoadLevel += LevelManager_OnLoadLevel;
        LevelManager.Instance.OnStartLevel += LevelManager_OnStartLevel;
        LevelManager.Instance.OnFinishLevel += LevelManager_OnFinishLevel;
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(OnNextButtonClicked);
        tryAgainButton.onClick.RemoveListener(OnTryAgainButtonClicked);
        exit1Button.onClick.RemoveListener(OnExitButtonClicked);
        exit2Button.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnNextButtonClicked()
    {
        LevelManager.Instance.LoadNextLevel();
    }

    private void OnTryAgainButtonClicked()
    {
        LevelManager.Instance.LoadCurrentLevel();
    }

    private void OnExitButtonClicked()
    {
        GameManager.Instance.ExitGame();
    }

    private void LevelManager_OnLoadLevel(object sender, EventArgs args)
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);

        Hide();
    }

    private void LevelManager_OnStartLevel(object sender, EventArgs args)
    {
        Hide();
    }

    private void LevelManager_OnFinishLevel(object sender, LevelManager.OnFinishLevelArgs args)
    {
        if (args.Winner is Player)
        {
            victoryPanel.SetActive(true);

            if (LevelManager.Instance.IsLastLevel())
            {
                nextButton.gameObject.SetActive(false);
            }
        }
        else
        {
            defeatPanel.SetActive(true);
        }

        Invoke(nameof(Show), 3.0f);
    }
}
