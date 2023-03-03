using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public class OnFinishLevelArgs : EventArgs
    {
        public Character Winner;
    }
    public EventHandler<OnFinishLevelArgs> OnFinishLevel;
    public EventHandler OnStartLevel;
    public EventHandler OnLoadLevel;

    [field: SerializeField]
    public ObjectColorsSO ObjectColorsSO { get; private set; }
    [field: SerializeField]
    public GameObject[] LevelPrefabArray { get; private set; }

    private GameObject currentLevelInstance;
    private bool triggered = false;
    private float timer = 3.0f;
    private float maxTimer = 3.0f;

    private void Update()
    {
        if (!triggered)
        {
            timer -= Time.deltaTime;
            if (timer > 0.0f)
            {
                UIManager.Instance.GetView<PreGameView>().SetCountdownText(timer);
            }
            else
            {
                UIManager.Instance.GetView<PreGameView>().SetCountdownText(timer);

                StartLevel();

                triggered = true;
            }
        }
    }

    public void LoadCurrentLevel()
    {
        LoadLevel(PlayerPrefs.GetInt("Level", 1));
    }

    public void LoadNextLevel()
    {
        LoadLevel(PlayerPrefs.GetInt("Level", 1) + 1);
    }

    public void StartLevel()
    {
        OnStartLevel?.Invoke(this, EventArgs.Empty);
    }

    public void FinishLevel(Character character)
    {
        OnFinishLevel?.Invoke(this, new OnFinishLevelArgs()
        {
            Winner = character
        });
    }

    public bool IsLastLevel()
    {
        return PlayerPrefs.GetInt("Level") == LevelPrefabArray.Length;
    }

    private void LoadLevel(int level)
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        currentLevelInstance = Instantiate(LevelPrefabArray[level - 1]);

        timer = maxTimer;

        triggered = false;

        PlayerPrefs.SetInt("Level", level);

        OnLoadLevel?.Invoke(this, EventArgs.Empty);
    }
}
