using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectColorsSO : ScriptableObject
{
    [System.Serializable]
    public struct ObjectColorData
    {
        public Material Material;
        public ObjectColorType ColorType;
    }

    [SerializeField]
    private ObjectColorData[] objectColorDataArray;

    public Material GetObjectColorMaterial(ObjectColorType type)
    {
        for (int i = 0; i < objectColorDataArray.Length; i++)
        {
            if (type == objectColorDataArray[i].ColorType)
            {
                return objectColorDataArray[i].Material;
            }
        }

        return null;
    }
}
