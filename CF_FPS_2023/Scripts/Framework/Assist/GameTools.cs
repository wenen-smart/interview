using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static partial  class GameTools
{

}
public static class GameObjectExtral
{
    public static T AddActorComponent<T>(this GameObject go) where T : ActorComponent
    {
        return ActorComponent.AddActorComponent<T>(go);
    }
}
public static class AssemblyMainTool
{
    public static Type GetConfigTypeByAssembley(string className)
    {
        Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        if (assembly == null)
        {
            return null;
        }
        Type _type = assembly.GetType(className);

        return _type;

    }
}