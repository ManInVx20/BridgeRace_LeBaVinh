using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour, IHasColor
{
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private GameObject wallGameObject;

    public ObjectColorType ColorType { get; set; }

    private void Start()
    {
        Setup();
    }

    public void ChangeObjectColorType(ObjectColorType type)
    {
        ColorType = type;

        meshRenderer.material = LevelManager.Instance.ObjectColorsSO.GetObjectColorMaterial(ColorType);
    }

    public void Setup()
    {
        ChangeObjectColorType(ObjectColorType.Default);

        meshRenderer.enabled = false;

        wallGameObject.SetActive(true);
    }

    public void Activate(ObjectColorType type)
    {
        ChangeObjectColorType(type);

        meshRenderer.enabled = true;

        wallGameObject.SetActive(false);
    }
}
