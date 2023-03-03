using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    private enum GameState
    {
        MainMenu,
        InGame
    }

    private GameState currentState;

    private void Start()
    {
        currentState = GameState.MainMenu;
    }

    public void StartGame()
    {
        StartCoroutine(LoadSceneByNameCoroutine("GameScene", 0.0001f, () =>
        {
            LevelManager.Instance.LoadCurrentLevel();

            currentState = GameState.InGame;
        }));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }

    public void ExitGame()
    {
        StartCoroutine(LoadSceneByNameCoroutine("MainMenuScene", 0.0f, () =>
        {
            currentState = GameState.MainMenu;
        }));
    }

    private IEnumerator LoadSceneByNameCoroutine(string sceneName, float delayTime, Action action)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        yield return asyncOperation;

        if (delayTime > 0.0f)
        {
            yield return new WaitForSeconds(delayTime);
        }

        action?.Invoke();
    }
}
