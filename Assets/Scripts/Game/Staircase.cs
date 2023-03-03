using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staircase : MonoBehaviour
{
    [field: SerializeField]
    public Transform StartPoint { get; private set; }
    [field: SerializeField]
    public Transform EndPoint { get; private set; }
}
