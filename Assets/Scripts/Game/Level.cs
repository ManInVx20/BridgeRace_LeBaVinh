using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Level : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface navMeshSurface;
    [SerializeField]
    private NavMeshData navMeshData;

    private void Awake()
    {
        navMeshSurface.UpdateNavMesh(navMeshData);
    }
}
