using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static partial class ExtralTools 
{
    public static Type typen(string typeName)
    {
        Type type = null;
        Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
        int assemblyArrayLength = assemblyArray.Length;
        for (int i = 0; i < assemblyArrayLength; ++i)
        {
            type = assemblyArray[i].GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        for (int i = 0; (i < assemblyArrayLength); ++i)
        {
            Type[] typeArray = assemblyArray[i].GetTypes();
            int typeArrayLength = typeArray.Length;
            for (int j = 0; j < typeArrayLength; ++j)
            {
                if (typeArray[j].Name.Equals(typeName))
                {
                    return typeArray[j];
                }
            }
        }
        return type;
    }
    public static Type GetConfigType(string className)
    {
        return Type.GetType(className);
    }
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
        public static float FixedAngle(this object obj,float angle)
    {
        if (angle>=180)
        {
            angle -= 360;
        }
        return angle;
    }
    public static  Vector3 CalcuateLocalToWorld(this MonoBehaviour mono,Vector3 local)
    {
        return mono.transform.TransformPoint(local);
    }

    public static T FindBestClosetT<T>(this Transform transform,T[] tList) where T:Component
    {
        var minDisT = Mathf.Infinity;
        T returnT=null;
        foreach (var item in tList)
        {
            var dis = Vector3.Distance(item.transform.position,transform.position);
            if (dis< minDisT)
            {
                returnT = item;
                minDisT = dis;
            }
        }
        return returnT;
    }
    public static Vector3 GetObjectCenter(this Transform trans)
    {
        var center = trans.DeepFind("CenterTrans");
        if (center)
        {
            return center.position;
        }
        else
        {
            Debug.Log("Center为空");
            return Vector3.zero;
        }
        /* return trans.DeepFind("Center")?.position??Vector3.zero;*/
    }
    public static Transform DeepFind(this Transform trans, string childName)
    {
        var obj = trans?.Find(childName);
        if (obj)
        {
            return obj;
        }
        else
        {
            foreach (Transform child in trans)
            {
                Transform res = DeepFind(child, childName);
                if (res != null)
                {
                    return res;
                }

            }
            return null;
        }
    }
    public static T GetComponentOrAdd<T>(this Transform obj) where T : MonoBehaviour
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            Debug.LogWarning("当前对象找不到组件 ，已经为你自动添加对应的组件" + " 组件名" + typeof(T).Name);
            component = obj.gameObject.AddComponent<T>();
        }
        return component;
    }

    public static bool IsSelectThisEnumInMult(this int gameObjectTag, int judgeTag,bool OriginTagMustContainJudgeTag=false)
    {
        if (OriginTagMustContainJudgeTag == true)
        {
            if (gameObjectTag < judgeTag)
            {
                return false;
            }
        }
        if ((((int)judgeTag) & (int)gameObjectTag) != 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="originTag"></param>
    /// <param name="judgeTag"></param>
    /// <param name="JudgeTagMustContainoriginTag">OriginTag是否完全包含JudgeTag,如果为false 表示有交集则匹配成功，如果为true时，OriginTag中一定存在judgeTag中的所有元素</param>
    /// <returns></returns>
    public static bool IsSelectThisEnumInMult(this Enum originTag, Enum judgeTag,bool OriginTagMustContainJudgeTag=false)
    {
        if (OriginTagMustContainJudgeTag==true)
        {
            if (Convert.ToInt32(originTag)<Convert.ToInt32(judgeTag))
            {
                return false;
            }
        }
        if (((Convert.ToInt32(judgeTag)) & Convert.ToInt32(originTag)) != 0)
        {
            return true;
        }
        return false;
    }

    public static bool IsInEnemyBack(this Transform owner,Transform enemy, float limitAngle = 90)
    {
        if (enemy == null)
        {
            return false;
        }

        if (Mathf.Acos(Vector3.Dot(enemy.forward.normalized, (owner.transform.position-enemy.position).normalized)) * Mathf.Rad2Deg > limitAngle)
        {
            return true;
        }
        return false;
    }
    public static int RandomIndex(this List<object> list)
    {
        int index = UnityEngine.Random.Range(0,list.Count);
        return index;
    }
    public static int RandomIndex(this object[] list)
    {
        int index = UnityEngine.Random.Range(0, list.Length);
        Debug.Log(index);
        return index;
    }
}
public static class TransformExtral
{
    /// <summary>
    /// 180 - (-180)
    /// </summary>
    /// <returns></returns>
    public static bool JudgeIsLeft(this Transform trans,Vector3 from,Vector3 to)
    {
        Vector3 cross = Vector3.Cross(from,to);
        float angle=Vector3.Angle(from,to);
       
        angle = cross.y > 0 ?angle:-angle;
        return angle>0;
    }
}

public static class VectorExtral
{
    public static Vector2 SetY(this Vector2 vector2, float value)
    {
        var newVector = vector2;
        newVector.y = value;
        return newVector;
    }
    public static Vector2 SetX(this Vector2 vector2, float value)
    {
        var newVector = vector2;
        newVector.x = value;
        return newVector;
    }
    public static Vector3 SetY(this Vector3 vector3,float value)
    {
        var newVector = vector3;
        newVector.y = value;
        return newVector;
    }
    public static Vector3 SetX(this Vector3 vector3, float value)
    {
        var newVector = vector3;
        newVector.x = value;
        return newVector;
    }
    public static Vector3 SetZ(this Vector3 vector3, float value)
    {
        var newVector = vector3;
        newVector.z = value;
        return newVector;
    }
    public static Vector3 Add(this Vector3 vector3, Vector3 value)
    {
       return vector3 += value;
    }
    public static Vector3 GetCirclePoint(this Transform trans,float angle)
    {
        angle += trans.eulerAngles.y;
        return trans.position+new Vector3(Mathf.Sin(Mathf.Deg2Rad*angle),0,Mathf.Cos(Mathf.Deg2Rad*angle));
    }
    public static Vector3 Abs(this Vector3 vec)
    {
        vec.x = Mathf.Abs(vec.x);
        vec.y = Mathf.Abs(vec.y);
        vec.z = Mathf.Abs(vec.z);
        return vec;
    }
}

public static class ConvertExtral
{
    public static void StringToInt(ref this int intObj,string formatStr)
    {
        if (string.IsNullOrEmpty(formatStr))
        {
            return;
        }
        int temp_int = 0;
        if (int.TryParse(formatStr, out temp_int) == false)
        {
            return;
        }
        intObj = temp_int;
    }
    public static void StringToFloat(ref this float floatObj, string formatStr)
    {
        if (string.IsNullOrEmpty(formatStr))
        {
            return;
        }
        float temp = 0;
        if (float.TryParse(formatStr, out temp) == false)
        {
            return;
        }
        floatObj = temp;
    }
    public static void StringToBoolean(ref this bool boolObj, string formatStr)
    {
        if (string.IsNullOrEmpty(formatStr))
        {
            return;
        }
        bool temp;
        if (bool.TryParse(formatStr, out temp) == false)
        {
            return;
        }
        boolObj = temp;
    }
}
public static class IntExtral
{
    public static int GetMin(params int[] intarray)
    {
        return intarray.Min();
    }
}
public static class StringExtral
{
   
    /// <summary>
    /// 计算开销
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns name="cost">cost 不会超过 MaxLength</returns>
    public static int LevenshteinDistance(string str1, string str2)
    {
        //str1作为首行 str2作为首列 
        int[,] costDS = new int[str1.Length + 1, str2.Length + 1];
        //初始化首列 首行 保证二维数组第一个元素为0
        for (int r = 0; r < costDS.GetLength(0); r++)
        {
            costDS[r, 0] = r;
        }
        for (int c = 0; c < costDS.GetLength(1); c++)
        {
            costDS[0, c] = c;
        }
        //初始化结束
        ///计算开销
        ///先遍历一行中的列 再向下遍历行
        for (int r = 1; r < costDS.GetLength(0); r++)
        {
            char char1 = str1[r - 1];
            for (int c = 1; c < costDS.GetLength(1); c++)
            {
                int isNoEqual = 0;
                char char2 = str2[c - 1];
                if (!char1.Equals(char2))
                {
                    isNoEqual = 1;
                }
                //left、top 、 leftTop
                int cost = IntExtral.GetMin(costDS[r, c - 1] + 1, costDS[r - 1, c] + 1, costDS[r - 1, c - 1] + isNoEqual);
                costDS[r, c] = cost;
            }
        }
        return costDS[costDS.GetLength(0)-1, costDS.GetLength(1)-1];
    }
    /// <summary>
    /// 得到字符串匹配相似度
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns>percent- [0,1]</returns>
    public static double LevenshteinDistanceMacthPercent(string str1, string str2)
    {
        int cost = LevenshteinDistance(str1, str2);
        return 1 - ((double)cost / Mathf.Max(str1.Length, str2.Length));
    }
    public static double LevenshteinDistanceMacthPercent(int cost,string str1, string str2)
    {
        return 1 - ((double)cost / Mathf.Max(str1.Length, str2.Length));
    }
}