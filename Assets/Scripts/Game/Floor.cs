using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField]
    private BrickObjectPool brickObjectPool;
    [SerializeField]
    private Transform[] brickSpawnPointsArray;

    private List<Character> characterOnFloorList;
    private bool opened;
    private float timer;

    private void Awake()
    {
        characterOnFloorList = new List<Character>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5.0f)
        {
            if (opened)
            {
                SpawnBricksAfterInterval();
            }

            timer = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            characterOnFloorList.Add(character);

            if (!opened)
            {
                Invoke(nameof(OpenFloor), 0.5f);

                opened = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            //characterOnFloorList.Remove(character);
        }
    }

    public void OpenFloor()
    {
        SpawnBricks(brickSpawnPointsArray);
    }

    private void SpawnBricksAfterInterval()
    {
        Transform[] freeBrickSpawnPointsArray = GetRandomFreeBrickSpawnPointArray();

        if (freeBrickSpawnPointsArray != null && freeBrickSpawnPointsArray.Length > 0)
        {
            SpawnBricks(freeBrickSpawnPointsArray);
        }
    }

    private void SpawnBricks(Transform[] brickSpawnPointsArray)
    {
        for (int i = 0; i < brickSpawnPointsArray.Length; i++)
        {
            Brick brick = brickObjectPool.GetPrefabInstance();

            ObjectColorType colorType = GetRandomObjectColorType(characterOnFloorList);

            brick.Setup(brickSpawnPointsArray[i], colorType);
        }
    }

    private Transform[] GetRandomFreeBrickSpawnPointArray()
    {
        List<Transform> freeBrickSpawnPointList = new List<Transform>();

        for (int i = 0; i < brickSpawnPointsArray.Length; i++)
        {
            if (brickSpawnPointsArray[i].childCount == 0)
            {
                freeBrickSpawnPointList.Add(brickSpawnPointsArray[i]);
            }
        }

        int middleIndex = freeBrickSpawnPointList.Count / 2;
        if (middleIndex > 0)
        {
            int randomIndexAtFirstHaft = UnityEngine.Random.Range(0, middleIndex);
            freeBrickSpawnPointList.RemoveRange(randomIndexAtFirstHaft, middleIndex);
        }

        return freeBrickSpawnPointList.ToArray();
    }

    private ObjectColorType GetRandomObjectColorType(List<Character> characterList)
    {
        int randomIndex = UnityEngine.Random.Range(0, characterList.Count);

        return characterList[randomIndex].ColorType;
    }
}
