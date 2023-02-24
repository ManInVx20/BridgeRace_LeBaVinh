using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour, IHasColor
{
    public ObjectColorType ColorType { get; set; }
    public int BrickStackCount => brickStack.Count;

    [Header("Core")]
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform brickHolderTransform;
    [SerializeField]
    private ObjectColorType colorType;

    [Header("Movement")]
    [SerializeField]
    private float maxSpeed = 6.0f;
    [SerializeField]
    private float maxAcceleration = 10.0f;
    [SerializeField]
    private float turnSpeed = 10.0f;

    [Header("Ground Check")]
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private float groundCheckRadius = 0.15f;
    [SerializeField]
    private float groundCheckDistance = 1.0f;
    [SerializeField]
    private LayerMask groundLayerMask;

    [Header("Slope")]
    [SerializeField]
    private float maxGroundAngle = 60.0f;
    [SerializeField]
    private float maxSnapSpeed = 100.0f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private Vector3 currentVelocity;
    private Vector3 targetVelocity;
    private Vector3 contactNormal;
    private float minGroundDotProduct;
    private int stepsSinceLastGrounded;
    private string currentAnim;
    private Stack<Brick> brickStack;


    private void Awake()
    {
        OnCharacterAwake();
    }

    private void Start()
    {
        OnCharacterStart();
    }

    private void Update()
    {
        HandleInput();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        UpdateState();
        AdjustVelocity();
        HandleMovement();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent<Brick>(out Brick brick))
        {
            if (CanGetBrick(brick))
            {
                //PushBrick(brick);
            }
        }

        if (other.transform.TryGetComponent<Character>(out Character character))
        {
            HandleCharracterCollision(character);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
    }

    public void ChangeObjectColorType(ObjectColorType type)
    {
        ColorType = type;

        skinnedMeshRenderer.material = LevelManager.Instance.ObjectColorsSO.GetObjectColorMaterial(ColorType);
    }

    public void SetTargetVelocityByInput(Vector2 moveInput)
    {
        targetVelocity = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized * maxSpeed;

        if (targetVelocity.sqrMagnitude > 0.0f)
        {
            ChangeAnim("Run");
        }
        else
        {
            ChangeAnim("Idle");
        }
    }

    public void ChangeAnim(string newAnim)
    {
        if (newAnim == currentAnim)
        {
            return;
        }

        currentAnim = newAnim;

        animator.SetTrigger(currentAnim);
    }

    protected virtual void OnCharacterAwake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        brickStack = new Stack<Brick>();

        ChangeObjectColorType(colorType);
    }

    protected virtual void OnCharacterStart()
    {
        ChangeAnim("Idle");
    }

    protected virtual void HandleInput()
    {

    }

    private void HandleRotation()
    {
        if (targetVelocity.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void UpdateState()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);

        currentVelocity = rb.velocity;

        stepsSinceLastGrounded += 1;

        if (isGrounded || SnapToGround())
        {
            stepsSinceLastGrounded = 0;
        }
    }

    private void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right, contactNormal).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward, contactNormal).normalized;

        float currentX = Vector3.Dot(currentVelocity, xAxis);
        float currentZ = Vector3.Dot(currentVelocity, zAxis);

        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentX, targetVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, targetVelocity.z, maxSpeedChange);

        currentVelocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void HandleMovement()
    {
        rb.velocity = currentVelocity;
    }

    private void GroundCheck()
    {
        rb.useGravity = true;

        Vector3 sphereCastOrigin = transform.position + Vector3.up * capsuleCollider.height * 0.5f;

        if (Physics.SphereCast(sphereCastOrigin, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayerMask))
        {
            if (hit.normal.y >= minGroundDotProduct)
            {
                isGrounded = true;
                contactNormal = hit.normal;
            }

            if (hit.normal.y < 1.0f)
            {
                rb.useGravity = false;
            }
        }
        else
        {
            isGrounded = false;
            contactNormal = Vector3.up;
        }
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 contactNormal)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    private bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1)
        {
            return false;
        }

        float speed = currentVelocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }

        if (!Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayerMask))
        {
            return false;
        }

        if (hit.normal.y < minGroundDotProduct)
        {
            return false;
        }

        contactNormal = hit.normal;
        float dot = Vector3.Dot(currentVelocity, hit.normal);

        if (dot > 0.0f)
        {
            currentVelocity = (currentVelocity - hit.normal * dot).normalized * speed;
        }

        return true;
    }

    private void PushBrick(Brick brick)
    {
        Vector3 brickPosition = Vector3.up * brick.BrickHeight * brickStack.Count;
        brick.Collect(brickHolderTransform, brickPosition, ColorType);

        brickStack.Push(brick);
    }

    private void PopBrickToStair(Stair stair)
    {
        Brick brick = brickStack.Pop();
        brick.BuildStair();

        stair.Activate(ColorType);
    }

    private void HandleCharracterCollision(Character character)
    {
        float ratio = 0.0f;

        if (BrickStackCount > character.BrickStackCount)
        {
            ratio = 0.0f;
        }
        else if(BrickStackCount < character.BrickStackCount)
        {
            ratio = 1.0f;
        }
        else
        {
            ratio = 0.5f;
        }

        int bricksLostCount = Mathf.RoundToInt(brickStack.Count * ratio);

        for (int i = 0; i < bricksLostCount; i++)
        {
            float x = UnityEngine.Random.Range(-2.0f, 2.0f);
            float z = UnityEngine.Random.Range(-2.0f, 2.0f);
            Vector3 offset = new Vector3(x, 0.0f, z);

            brickStack.Pop().Drop(transform.position + offset);
        }
    }

    private bool CanGetBrick(Brick brick)
    {
        return brick.ColorType == ObjectColorType.Default || brick.ColorType == ColorType;
    }

    private bool CanBuildStair(Stair stair)
    {
        return brickStack.Count > 0 && (stair.ColorType == ObjectColorType.Default || stair.ColorType != ColorType);
    }
}
