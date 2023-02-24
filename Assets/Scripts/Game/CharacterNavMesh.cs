using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterNavMesh : MonoBehaviour, IHasColor
{
    public ObjectColorType ColorType { get; set; }

    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;

    private NavMeshAgent agent;

    private Transform targetTransform;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        ChangeObjectColorType(ObjectColorType.Blue);
    }

    private void Update()
    {
        if (targetTransform == null)
        {
            CheckBricks();
        }
    }

    public void ChangeObjectColorType(ObjectColorType type)
    {
        ColorType = type;

        skinnedMeshRenderer.material = LevelManager.Instance.ObjectColorsSO.GetObjectColorMaterial(ColorType);
    }

    private void CheckBricks()
    {
        RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, 10.0f, Vector3.down);

        for (int i = 0; i < hitArray.Length; i++)
        {
            if (hitArray[i].transform.TryGetComponent<Brick>(out Brick brick))
            {
                if (CanGetBrick(brick))
                {
                    targetTransform = brick.transform;

                    agent.destination = targetTransform.position;

                    break;
                }
            }
        }
    }

    private bool CanGetBrick(Brick brick)
    {
        return brick.ColorType == ObjectColorType.Default || brick.ColorType == ColorType;
    }

    public bool ReachedDestinationOrGaveUp()
    {

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
