using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button quitButton;

    protected override void Start()
    {
        base.Start();

        startButton.onClick.AddListener(OnStartButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnStartButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        GameManager.Instance.StartGame();
    }

    private void OnQuitButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }
}
