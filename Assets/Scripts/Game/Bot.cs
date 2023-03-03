using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Bot : Character
{
    public Brick TargetBrick = null;
    public Floor CurrentFloor = null;
    public Staircase TargetStaircase = null;

    [SerializeField]
    private int minBricksToBuild = 5;

    private IBotState currentState;

    public void ChangeState(IBotState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void CheckAvailableBricks()
    {
        if (TargetBrick == null)
        {
            Vector3 halfExtends = new Vector3(20.0f, 2.0f, 20.0f);
            RaycastHit[] hitArray = Physics.BoxCastAll(transform.position, halfExtends, Vector3.down);

            Brick nearestBrick = null;

            for (int i = 0; i < hitArray.Length; i++)
            {
                if (hitArray[i].transform.TryGetComponent<Brick>(out Brick brick))
                {
                    if (IsValidBrick(brick))
                    {
                        if (nearestBrick == null 
                            || Vector3.Distance(brick.transform.position, transform.position) < Vector3.Distance(nearestBrick.transform.position, transform.position))
                        {
                            nearestBrick = brick;
                        }
                    }
                }
            }

            TargetBrick = nearestBrick;
        }
        //else
        //{
        //    if (!IsValidBrick(TargetBrick))
        //    {
        //        TargetBrick = null;
        //    }
        //}
    }

    public bool IsValidBrick(Brick brick)
    {
        return !BrickStack.Contains(brick) && CanGetBrick(brick);
    }

    public bool IsEnoughBricks()
    {
        return BrickStack.Count >= minBricksToBuild;
    }

    public bool ReachedDestinationOrGaveUp()
    {
        if (!Agent.pathPending)
        {
            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (!Agent.hasPath || Agent.velocity.magnitude < 0.01f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected override void StartCharacter()
    {
        base.StartCharacter();

        ChangeAnim(IdleAnimHash);

        ChangeState(null);
    }

    protected override void UpdateState()
    {
        base.UpdateState();

        CheckAvailableBricks();

        currentState?.Execute(this);
    }

    protected override void HandleOnTriggerEnter(Collider other)
    {
        base.HandleOnTriggerEnter(other);

        if (other.TryGetComponent<Floor>(out Floor floor))
        {
            if (floor == null || CurrentFloor != floor)
            {
                CurrentFloor = floor;

                TargetStaircase = floor.GetNearestStaircase(transform.position);

                TargetBrick = null;
            }
        }
    }

    protected override void Hit()
    {
        base.Hit();

        ChangeState(null);
    }

    protected override void Restore()
    {
        base.Restore();

        ChangeState(new IdleBotState());
    }

    protected override void ReachFinishFloor()
    {
        base.ReachFinishFloor();

        ChangeState(null);
    }

    protected override void LevelManager_OnStartLevel(object sender, EventArgs args)
    {
        base.LevelManager_OnStartLevel(sender, args);

        ChangeState(new IdleBotState());
    }
}
