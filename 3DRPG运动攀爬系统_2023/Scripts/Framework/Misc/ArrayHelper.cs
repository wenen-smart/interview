using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrayHelper
{
    public static List<T> ToComponents<T>(GameObject[] gameObjects) where T:Component
    {
        List<T> result = new List<T>();
        foreach (var item in gameObjects)
        {
            T c = item.GetComponent<T>();
            if (c != null)
            {
                result.Add(c);
            }
        }
        return result;
    }
    public static List<T> ToComponents<T>(List<GameObject> gameObjects) where T : Component
    {
        List<T> result = new List<T>();
        foreach (var item in gameObjects)
        {
            T c = item.GetComponent<T>();
            if (c != null)
            {
                result.Add(c);
            }
        }
        return result;
    }
    public static List<T> ToComponents<MonoT,T>(List<MonoT> gameObjects) where T : Component where MonoT:MonoBehaviour
    {
        List<T> result = new List<T>();
        foreach (var item in gameObjects)
        {
            T c = item.GetComponent<T>();
            if (c != null)
            {
                result.Add(c);
            }
        }
        return result;
    }

}
