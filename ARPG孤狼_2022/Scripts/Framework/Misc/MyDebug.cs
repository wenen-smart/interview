using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MyDebug:InstanceMono<MyDebug>
{
    public List<MyDebugData> debugDataList=new List<MyDebugData>();
    public static void DebugLogger(MyDebugTYpe debugTYpe,string format)
    {
        switch (debugTYpe)
        {
            case MyDebugTYpe.Print:
                DebugPrint(format);
                break;
            case MyDebugTYpe.Warning:
                DebugWarning(format);
                break;
            case MyDebugTYpe.Error:
                DebugError(format);
                break;
            case MyDebugTYpe.SupperError:
                DebugSupperError(format);
                break;
            default:
                break;
        }
    }

    public static void DebugPrint(string format)
    {
        Debug.Log(DebugerFactory.GetDebugerPartFormatStr(MyDebugTYpe.Print)+format);
    }
    public static void DebugWarning(string format)
    {
        Debug.LogWarning(DebugerFactory.GetDebugerPartFormatStr(MyDebugTYpe.Warning)+format);
    }
    public static void DebugError(string format)
    {
        Debug.LogError(DebugerFactory.GetDebugerPartFormatStr(MyDebugTYpe.Error)+format);
    }
    public static void DebugSupperError(string format)
    {
        Debug.LogError(DebugerFactory.GetDebugerPartFormatStr(MyDebugTYpe.SupperError)+format);
    }
    public static void DebugLine(Vector3 start,Vector3 end,Color color,float time)
    {
         Debug.DrawLine(start, end,color, time);
    }
    public static void DebugWireSphere(Vector3[] points,float radius,Color color,float time)
    {
        MyDebug.Instance.debugDataList.Add(new MyDebugData(MyDebugTYpe.WireShpere,radius,time,color,points));
    }
    [RuntimeInitializeOnLoadMethod]
    public static void OnRunTime()
    {
        if (Instance==null)
        {
            GameObject debugGo = new GameObject("MyDebug",typeof(MyDebug));
            DontDestroyOnLoad(debugGo);
        }
    }
    public void Update()
    {
        if (debugDataList.Count > 0)
        {
            for (int i = 0; i < debugDataList.Count; i++)
            {
                var item = debugDataList[i];
                item.timer += Time.deltaTime;
                if (item.timer > item.time)
                {
                    debugDataList.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }
    }
    public void OnDrawGizmos()
    {
        foreach (var item in debugDataList)
        {
            if (item.myDebugTYpe==MyDebugTYpe.WireShpere)
            {
                Gizmos.color = item.color;
                foreach (var point in item.points)
                {
                    Gizmos.DrawWireSphere(point,item.length);
                }
            }
        }
    }
}
public enum MyDebugTYpe
{
    Print,
    Warning,
    Error,
    SupperError,
    WireShpere,
}
public class MyDebugData
{
    public MyDebugTYpe myDebugTYpe;
    public float time;
    public Color color;
    public Vector3[] points;
    public float timer = 0;
    public float length;

    public MyDebugData(MyDebugTYpe myDebugTYpe,float length, float time, Color color,Vector3[] points)
    {
        this.myDebugTYpe = myDebugTYpe;
        this.time = time;
        this.color = color;
        this.length = length;
        this.points = points;
    }
}
public static class DebugerFactory
{
    public static string GetDebugerPartFormatStr(MyDebugTYpe debugTYpe)
    {
        string part = "";
        switch (debugTYpe)
        {
            case MyDebugTYpe.Print:
                part = "[Logger:]";
                break;
            case MyDebugTYpe.Warning:
                part = "[Logger-Warning:]";
                break;
            case MyDebugTYpe.Error:
                part = "[Logger-Error:]";
                break;
            case MyDebugTYpe.SupperError:
                part = "[Logger-SupperError:]";
                break;
            default:
                break;
        }
        return part;
    }
}