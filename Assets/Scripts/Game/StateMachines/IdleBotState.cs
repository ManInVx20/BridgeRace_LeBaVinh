using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBotState : IBotState
{
    private float timer;
    private float maxTimer;

    public void Enter(Bot bot)
    {
        bot.MoveDirection = Vector3.zero;

        bot.TargetBrick = null;

        bot.ChangeAnim(bot.IdleAnimHash);

        timer = 0.0f;
        maxTimer = Random.Range(0.2f, 0.5f);
    }

    public void Execute(Bot bot)
    {
        timer += Time.deltaTime;
        if (timer >= maxTimer)
        {
            if (bot.IsEnoughBricks())
            {
                bot.ChangeState(new BuildBotState());
            }
            else if (bot.TargetBrick != null)
            {
                bot.TargetBrick = null;

                bot.ChangeState(new CollectBotState());
            }
        }
    }

    public void Exit(Bot bot)
    {

    }
}
