using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [field: SerializeField]
    public ObjectColorsSO ObjectColorsSO { get; private set; }
    [field: SerializeField]
    public List<Character> CharacterList { get; private set; }

    [SerializeField]
    private Floor[] _floorArray;
}
