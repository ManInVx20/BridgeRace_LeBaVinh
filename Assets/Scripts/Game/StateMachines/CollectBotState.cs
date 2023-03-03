using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectBotState : IBotState
{
    public void Enter(Bot bot)
    {
        bot.ChangeAnim(bot.RunAnimHash);
    }

    public void Execute(Bot bot)
    {
        if (bot.TargetBrick != null)
        {
            if (bot.IsValidBrick(bot.TargetBrick))
            {
                bot.MoveDirection = (bot.TargetBrick.transform.position - bot.transform.position).normalized;
            }
            else
            {
                bot.ChangeState(new IdleBotState());
            }
        }
    }

    public void Exit(Bot bot)
    {
        
    }
}
