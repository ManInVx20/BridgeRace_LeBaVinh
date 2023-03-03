using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour, IHasColor
{
    [field: SerializeField]
    public ObjectColorType ColorType { get; set; }
    [field: SerializeField]
    public NavMeshAgent Agent { get; private set; }

    public Vector3 MoveDirection { get; set; }
    public Stack<Brick> BrickStack { get; private set; } = new Stack<Brick>();
    public int IdleAnimHash { get; private set; }
    public int RunAnimHash { get; private set; }
    public int HitAnimHash { get; private set; }
    public int VictoryAnimHash { get; private set; }
    public int DefeatAnimHash { get; private set; }

    [SerializeField]
    private CapsuleCollider capsuleCollider;
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform brickHolderTransform;


    private int currentAnimHash;

    private bool canControl = true;
    private Vector3 newPositionAfterHit;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        StartCharacter();

        LevelManager.Instance.OnStartLevel += LevelManager_OnStartLevel;
        LevelManager.Instance.OnFinishLevel += LevelManager_OnFinishLevel;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnStartLevel -= LevelManager_OnStartLevel;
        LevelManager.Instance.OnFinishLevel -= LevelManager_OnFinishLevel;
    }

    private void Update()
    {
        if (!canControl)
        {
            return;
        }

        UpdateState();

        if (MoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(MoveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Agent.angularSpeed * Time.deltaTime);
        }

        Agent.Move(MoveDirection * Agent.speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleOnTriggerEnter(other);
    }

    public void ChangeObjectColorType(ObjectColorType type)
    {
        ColorType = type;

        skinnedMeshRenderer.material = LevelManager.Instance.ObjectColorsSO.GetObjectColorMaterial(ColorType);
    }

    public void ChangeAnim(int newAnimHash)
    {
        if (newAnimHash == currentAnimHash)
        {
            return;
        }

        currentAnimHash = newAnimHash;

        animator.SetTrigger(currentAnimHash);
    }

    protected virtual void Initialize()
    {
        IdleAnimHash = Animator.StringToHash("Idle");
        RunAnimHash = Animator.StringToHash("Run");
        HitAnimHash = Animator.StringToHash("Hit");
        VictoryAnimHash = Animator.StringToHash("Victory");
        DefeatAnimHash = Animator.StringToHash("Defeat");

        ChangeObjectColorType(ColorType);
    }

    protected virtual void StartCharacter()
    {

    }

    protected virtual void UpdateState()
    {
        MoveDirection = Vector3.zero;
    }

    protected virtual void HandleOnTriggerEnter(Collider other)
    {
        if (!canControl)
        {
            return;
        }

        if (other.TryGetComponent<Brick>(out Brick brick))
        {
            if (CanGetBrick(brick))
            {
                PushBrick(brick);
            }
        }

        if (other.TryGetComponent<Stair>(out Stair stair))
        {
            if (CanBuildStair(stair))
            {
                PopBrickToStair(stair);
            }
        }

        if (other.TryGetComponent<Character>(out Character character))
        {
            HandleCharracterCollision(character);
        }

        if (other.TryGetComponent<FinishFloor>(out FinishFloor finishFloor))
        {
            ReachFinishFloor();
        }
    }

    protected virtual void ReachFinishFloor()
    {
        ChangeAnim(VictoryAnimHash);

        LevelManager.Instance.FinishLevel(this);
    }

    protected virtual void Hit()
    {
        ChangeAnim(HitAnimHash);

        canControl = false;

        capsuleCollider.enabled = false;

        Invoke(nameof(Restore), 1.1f);
    }

    protected virtual void Restore()
    {
        ChangeAnim(IdleAnimHash);

        canControl = true;

        capsuleCollider.enabled = true;

        transform.position = newPositionAfterHit;
    }

    protected virtual void LevelManager_OnStartLevel(object sender, EventArgs args)
    {
        canControl = true;
    }

    protected virtual void LevelManager_OnFinishLevel(object sender, LevelManager.OnFinishLevelArgs args)
    {
        canControl = false;

        if (args.Winner != this)
        {
            ChangeAnim(DefeatAnimHash);
        }
    }

    protected bool CanGetBrick(Brick brick)
    {
        return brick.CanBeCollected && (brick.ColorType == ObjectColorType.Default || brick.ColorType == ColorType);
    }

    protected bool CanBuildStair(Stair stair)
    {
        return BrickStack.Count > 0 && (stair.ColorType == ObjectColorType.Default || stair.ColorType != ColorType);
    }

    private void PushBrick(Brick brick)
    {
        Vector3 brickPosition = Vector3.up * brick.BrickHeight * BrickStack.Count;
        brick.Collect(brickHolderTransform, brickPosition, ColorType);

        BrickStack.Push(brick);
    }

    private void PopBrickToStair(Stair stair)
    {
        Brick brick = BrickStack.Pop();
        brick.BuildStair();

        stair.Activate(ColorType);
    }

    private void HandleCharracterCollision(Character character)
    {
        //Debug.Log($"{name}: {BrickStack.Count}, {character.name}: {character.BrickStack.Count}");

        float ratio = 0.0f;

        if (BrickStack.Count > character.BrickStack.Count)
        {
            ratio = 0.0f;
        }
        else if (BrickStack.Count < character.BrickStack.Count)
        {
            ratio = 1.0f;
        }
        //else
        //{
        //    ratio = 0.5f;
        //}

        if (ratio > 0.0f)
        {
            int bricksLostCount = Mathf.RoundToInt(BrickStack.Count * ratio);

            for (int i = 0; i < bricksLostCount; i++)
            {
                float x = UnityEngine.Random.Range(-2.0f, 2.0f);
                float z = UnityEngine.Random.Range(-2.0f, 2.0f);
                Vector3 offset = new Vector3(x, 0.0f, z);

                BrickStack.Pop().Drop(transform.position + offset);
            }

            newPositionAfterHit = transform.position + (transform.position - character.transform.position).normalized;

            Hit();
        }
    }
}
