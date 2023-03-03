using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreGameView : View
{
    [SerializeField]
    private TextMeshProUGUI countdownText;

    protected override void Start()
    {
        base.Start();

        LevelManager.Instance.OnLoadLevel += LevelManager_OnLoadLevel;
        LevelManager.Instance.OnStartLevel += LevelManager_OnStartLevel;
    }

    public void SetCountdownText(float value)
    {
        value = Mathf.CeilToInt(value);
        countdownText.text = value.ToString();
    }

    private void LevelManager_OnLoadLevel(object sender, EventArgs args)
    {
        Show();
    }

    private void LevelManager_OnStartLevel(object sender, EventArgs e)
    {
        Hide();
    }
}
