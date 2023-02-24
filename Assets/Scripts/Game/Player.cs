using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private Joystick joystick;

    protected override void OnCharacterAwake()
    {
        base.OnCharacterAwake();

        joystick = FindObjectOfType<Joystick>();
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        if (joystick == null)
        {
            return;
        }

        SetTargetVelocityByInput(joystick.Input);
    }
}
