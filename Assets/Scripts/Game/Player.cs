using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Character
{
    private Joystick joystick;

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void UpdateState()
    {
        base.UpdateState();

        if (joystick == null)
        {
            return;
        }

        MoveDirection = new Vector3(joystick.Input.x, 0.0f, joystick.Input.y).normalized;

        if (MoveDirection.magnitude > 0.01f)
        {
            ChangeAnim(RunAnimHash);
        }
        else
        {
            ChangeAnim(IdleAnimHash);
        }
    }

    protected override void LevelManager_OnStartLevel(object sender, EventArgs args)
    {
        base.LevelManager_OnStartLevel(sender, args);

        joystick = FindObjectOfType<Joystick>();
    }
}
