using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Serializable]
    private struct CameraData
    {
        public Vector3 Position;
        public Vector3 EulerAngles;
    }

    [SerializeField]
    private CameraData startOffset;
    [SerializeField]
    private CameraData endOffset;
    [SerializeField]
    private float cameraSpeed;

    private Player targetPlayer;
    private CameraData currentOffset = default;

    private void Start()
    {
        LevelManager.Instance.OnLoadLevel += LevelManager_OnLoadLevel;
        LevelManager.Instance.OnStartLevel += LevelManager_OnStartLevel;
        LevelManager.Instance.OnFinishLevel += LevelManager_OnFinishLevel;
    }

    private void LateUpdate()
    {
        if (targetPlayer != null)
        {
            Vector3 newPosition = targetPlayer.transform.position + currentOffset.Position;
            if (targetPlayer.BrickStack.Count > 10)
            {
                Vector3 directionFromPlayerToCamera = (transform.position - targetPlayer.transform.position).normalized;
                float extraOffsetMultiplier = (targetPlayer.BrickStack.Count - 10) * (targetPlayer.BrickStack.Count / 10) * 0.2f;
                newPosition += directionFromPlayerToCamera * extraOffsetMultiplier;
            }
            transform.position = Vector3.Lerp(transform.position, newPosition, cameraSpeed * Time.deltaTime);

            Quaternion newRotation = Quaternion.Euler(currentOffset.EulerAngles);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, cameraSpeed * Time.deltaTime);
        }
    }

    private void LevelManager_OnLoadLevel(object sender, EventArgs e)
    {
        targetPlayer = FindObjectOfType<Player>();

        currentOffset = startOffset;
    }

    private void LevelManager_OnStartLevel(object sender, EventArgs args)
    {
        currentOffset = startOffset;
    }

    private void LevelManager_OnFinishLevel(object sender, LevelManager.OnFinishLevelArgs args)
    {
        currentOffset = endOffset;
    }
}
