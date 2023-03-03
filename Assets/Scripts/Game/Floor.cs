using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField]
    private BrickObjectPool brickObjectPool;
    [SerializeField]
    private Transform[] brickSpawnPointsArray;
    [SerializeField]
    private Staircase[] staircaseArray;

    private List<Character> characterOnFloorList;
    private bool opened = false;
    private bool canSpawnBricks = true;
    private float timer = 0.0f;

    private void Awake()
    {
        characterOnFloorList = new List<Character>();
    }

    private void Start()
    {
        LevelManager.Instance.OnFinishLevel += LevelManager_OnFinishLevel;
    }

    private void Update()
    {
        if (opened && canSpawnBricks)
        {
            timer += Time.deltaTime;
            if (timer >= 5.0f)
            {
                SpawnBricksAfterInterval();

                timer = 0.0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            if (!characterOnFloorList.Contains(character))
            {
                characterOnFloorList.Add(character);

                if (!opened)
                {
                    opened = true;

                    Invoke(nameof(OpenFloor), 0.1f);
                }
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

    public Staircase GetNearestStaircase(Vector3 position)
    {
        Staircase nearestStaircase = null;

        for (int i = 0; i < staircaseArray.Length; i++)
        {
            if (nearestStaircase == null 
                || Vector3.Distance(nearestStaircase.transform.position, position) >= Vector3.Distance(staircaseArray[i].transform.position, position))
            {
                nearestStaircase = staircaseArray[i];
            }
        }

        return nearestStaircase;
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

    private void LevelManager_OnFinishLevel(object sender, LevelManager.OnFinishLevelArgs args)
    {
        canSpawnBricks = false;
    }
}
