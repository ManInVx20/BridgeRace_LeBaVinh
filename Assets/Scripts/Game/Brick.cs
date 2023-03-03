using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Brick : PoolableObject, IHasColor
{
    private enum BrickState
    {
        Idle,
        Spawn,
        Collect,
        Drop
    }

    public ObjectColorType ColorType { get; set; }
    public bool CanBeCollected { get; private set; }
    public float BrickHeight => brickHeight;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private BoxCollider boxCollider;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float brickHeight;
    [SerializeField]
    private float spawnEffectSpeed;
    [SerializeField]
    private float flySpeed;
    [SerializeField]
    private float dropSpeed;

    private BrickState state = BrickState.Idle;
    private Vector3 target;
    private float timer;
    private float maxTimer;

    private void Update()
    {
        switch (state)
        {
            case BrickState.Idle:
                break;
            case BrickState.Spawn:
                if (Vector3.Distance(transform.localScale, Vector3.one) > 0.01f)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, spawnEffectSpeed * Time.deltaTime);
                }
                else
                {
                    transform.localScale = Vector3.one;

                    CanBeCollected = true;

                    rb.isKinematic = false;
                    boxCollider.enabled = true;

                    state = BrickState.Idle;
                }
                break;
            case BrickState.Collect:
                if (Vector3.Distance(transform.localPosition, target) > 0.01f)
                {
                    transform.localPosition = Vector3.Slerp(transform.localPosition, target, flySpeed * Time.deltaTime);
                    transform.localRotation = Quaternion.identity;
                }
                else
                {
                    transform.localPosition = target;
                    transform.localRotation = Quaternion.identity;

                    state = BrickState.Idle;
                }
                break;
            case BrickState.Drop:
                timer += Time.deltaTime;
                if (timer >= maxTimer)
                {
                    CanBeCollected = true;

                    //rb.isKinematic = true;

                    state = BrickState.Idle;

                    timer = 0.0f;
                }
                break;
        }
    }

    public override void ReturnToPool()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        base.ReturnToPool();
    }

    public void Setup(Transform parent, ObjectColorType type)
    {
        ChangeObjectColorType(type);

        CanBeCollected = false;

        rb.isKinematic = true;
        boxCollider.enabled = false;

        transform.SetParent(parent);

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * 0.5f;

        state = BrickState.Spawn;
    }

    public void Collect(Transform parent, Vector3 position, ObjectColorType type)
    {
        ChangeObjectColorType(type);

        CanBeCollected = false;

        rb.isKinematic = true;
        boxCollider.enabled = false;

        transform.SetParent(parent);

        target = position;

        state = BrickState.Collect;
    }

    public void BuildStair()
    {
        ReturnToPool();
    }

    public void Drop(Vector3 position)
    {
        ChangeObjectColorType(ObjectColorType.Default);

        CanBeCollected = false;

        transform.SetParent(null);

        rb.isKinematic = false;

        boxCollider.enabled = true;

        target = position;

        maxTimer = 1.0f; //Mathf.Abs(transform.position.y - target.y);

        Vector3 force = new Vector3(target.x, 0.0f, target.z) - new Vector3(transform.position.x, 0.0f, transform.position.z);
        force.Normalize();

        rb.AddForce(force);

        state = BrickState.Drop;
    }

    public void ChangeObjectColorType(ObjectColorType type)
    {
        ColorType = type;

        meshRenderer.material = LevelManager.Instance.ObjectColorsSO.GetObjectColorMaterial(ColorType);
    }
}
