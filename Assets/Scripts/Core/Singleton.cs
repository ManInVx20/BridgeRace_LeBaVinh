using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Find singleton
                instance = FindObjectOfType<T>();

                // Create new instance if one doesn't already exist.
                if (instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    GameObject singletonGameObject = new GameObject();
                    singletonGameObject.name = typeof(T).ToString() + " (Singleton)";
                    instance = singletonGameObject.AddComponent<T>();
                }

            }

            return instance;
        }
    }

}
