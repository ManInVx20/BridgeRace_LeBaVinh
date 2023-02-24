using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private View[] viewArray;

    private Dictionary<Type, View> viewDictionary = new Dictionary<Type, View>();

    public T GetView<T>() where T : View
    {
        if (viewDictionary.ContainsKey(typeof(T)))
        {
            return viewDictionary[typeof(T)] as T;
        }
        else
        {
            if (viewArray != null)
            {
                foreach (View view in viewArray)
                {
                    if (view is T)
                    {
                        viewDictionary[typeof(View)] = view;

                        return view as T;
                    }
                }
            }

        }

        return null;
    }
}
