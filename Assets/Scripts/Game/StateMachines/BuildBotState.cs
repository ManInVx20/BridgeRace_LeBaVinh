using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildBotState : IBotState
{
    private bool reachedStaircase;

    public void Enter(Bot bot)
    {
        bot.ChangeAnim(bot.RunAnimHash);

        reachedStaircase = false;

        bot.Agent.SetDestination(bot.TargetStaircase.StartPoint.position);
    }

    public void Execute(Bot bot)
    {
        if (!reachedStaircase)
        {
            if (bot.ReachedDestinationOrGaveUp())
            {
                reachedStaircase = true;

                bot.Agent.SetDestination(bot.TargetStaircase.EndPoint.position);
            }
        }
        else
        {
            if (bot.ReachedDestinationOrGaveUp())
            {
                bot.ChangeState(new IdleBotState());
            }
            else if (bot.BrickStack.Count == 0)
            {
                bot.ChangeState(new CollectBotState());
            }
        }
    }

    public void Exit(Bot bot)
    {
        bot.Agent.ResetPath();
    }
}
