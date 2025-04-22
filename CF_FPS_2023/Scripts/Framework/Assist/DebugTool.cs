using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public class DebugTool:MonoSingleTon<DebugTool>
{
    public List<MyDebugGizmosEntity> debugGizmosEntityList=new List<MyDebugGizmosEntity>();
    //public static bool allowEditorNoRunStateDebug = false;
    //public static void AllowDebugInNoRunState(bool isAllow)
    //{
    //    allowEditorNoRunStateDebug = isAllow;
    //}
    private static string DebugToolEntityBaseName = "[DebugTool-Entity]";
    private GameObject debugEntity_prefab;
    public bool DrawDebugToggle = true;

    public override void Awake()
    {
        base.Awake();
        debugEntity_prefab = new GameObject(DebugToolEntityBaseName);
         DontDestroyOnLoad(debugEntity_prefab);
        GameObject group = new GameObject(DebugToolEntityBaseName);
        DontDestroyOnLoad(group);
        #if UNITY_EDITOR
        MyDebugGizmosEntity.Config(debugEntity_prefab, group);
        #endif
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLogger(MyDebugType debugTYpe,string format)
    {
        switch (debugTYpe)
        {
            case MyDebugType.Print:
                DebugPrint(format);
                break;
            case MyDebugType.Warning:
                DebugWarning(format);
                break;
            case MyDebugType.Error:
                DebugError(format);
                break;
            case MyDebugType.SupperError:
                DebugSupperError(format);
                break;
            default:
                break;
        }
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugPrint(string format)
    {
        Debug.Log(DebugerFactory.GetDebugerPartFormatStr(MyDebugType.Print)+format);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugWarning(string format)
    {
        Debug.LogWarning(DebugerFactory.GetDebugerPartFormatStr(MyDebugType.Warning)+format);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugError(string format)
    {
        Debug.LogError(DebugerFactory.GetDebugerPartFormatStr(MyDebugType.Error)+format);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugSupperError(string format)
    {
        Debug.LogError(DebugerFactory.GetDebugerPartFormatStr(MyDebugType.SupperError)+format);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start,Vector3 end,Color color,float time)
    {
		if (DebugTool.Instance.DrawDebugToggle == false)
		{
			return;
		}
		Debug.DrawLine(start, end,color, time);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 direction, float duration, Color color, float time)
    {
		if (DebugTool.Instance.DrawDebugToggle == false)
		{
			return;
		}
		Debug.DrawLine(start, start+direction.normalized*duration, color, time);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawWireSphere(Vector3 point,float radius,Color color,float time,string _signName="")
    {
		if (DebugTool.Instance.DrawDebugToggle==false)
		{
            return;
		}
        if (!string.IsNullOrEmpty(_signName))
        {
            int index = DebugTool.Instance.GetGizmosEntity(_signName);
            if (index != -1)
            {
                DebugTool.Instance.RemoveGizmosEntity(index);
            }
        }
        
        DebugTool.Instance.debugGizmosEntityList.Add(new WireSphereGizmosEntity(point,radius,color,time,_signName));
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawBox(Vector3 point,Vector3 halfExtent,Color color,float time,string _signName="")
    {
		if (DebugTool.Instance.DrawDebugToggle == false)
		{
			return;
		}
		if (!string.IsNullOrEmpty(_signName))
        {
            int index = DebugTool.Instance.GetGizmosEntity(_signName);
            if (index != -1)
            {
                DebugTool.Instance.RemoveGizmosEntity(index);
            }
        }
        DebugTool.Instance.debugGizmosEntityList.Add(new WireBoxGizmosEntity(point,halfExtent,color,time,_signName));
    }
    
    public  int GetGizmosEntity(string _signName)
    {
        return debugGizmosEntityList.FindIndex((item)=> { return item.signName == _signName; });
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void RemoveGizmosEntity(int index)
    {
        debugGizmosEntityList[index].LifeTimeOver();
        debugGizmosEntityList.RemoveAt(index);
    }
    //[RuntimeInitializeOnLoadMethod]
    //public static void OnRunTime()
    //{
    //    if (Instance==null)
    //    {
    //        Debug.LogWarning("未在根场景添加DebugTool游戏对象。建议添加。");
    //    }

    //}
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Update()
    {
		if (DrawDebugToggle == false)
		{
			return;
		}
		if (debugGizmosEntityList.Count > 0)
        {
            for (int i = 0; i < debugGizmosEntityList.Count; i++)
            {
                var item = debugGizmosEntityList[i];
                if (item.timer > item.time)
                {
                    RemoveGizmosEntity(i);
                    i--;
                    continue;
                }
                item.timer += Time.deltaTime;
            }
        }
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void OnDrawGizmos()
    {
		if (DrawDebugToggle == false)
		{
			return;
		}
		foreach (var item in debugGizmosEntityList)
        {
            item.Debug();
        }
    }
}
public enum MyDebugType
{
    Print,
    Warning,
    Error,
    SupperError,
    Gizmos,
}
public abstract class MyDebugGizmosEntity
{
    public MyDebugType myDebugType;
    public float time;
    public Color color;
    public float timer = 0;
    public string signName;
    private static GameObject parent;
    private static GameObject debugEntity_prefab;
    protected GameObject debug_entity;
    public Vector3 point { get; private set; }
    protected void SetPoint(Vector3 _point)
    {
        point = _point;
        SetupGizmos();
    }
    public MyDebugGizmosEntity(Color color,float time,string _signName)
    {
        myDebugType = MyDebugType.Gizmos;
        this.time = time;
        this.color = color;
        this.signName = _signName;
        if (!string.IsNullOrEmpty(signName))
        {
            BindGameObject();
        }
    }
    public static void Config(GameObject _prefab,GameObject entity_Group)
    {
        debugEntity_prefab = _prefab;
        parent = entity_Group;
    }
    public abstract void Debug();
    public void BindGameObject()
    {
        //UnityEngine.Debug.Log("debugEntity_prefab: "+(debugEntity_prefab==null));

        GameObject entity = GameObjectFactory.Instance.PopItem(debugEntity_prefab);
        
        if (debug_entity!=null)
        {
            GameObjectFactory.Instance.PushItem(debug_entity);
        }
        if (entity)
        {
            debug_entity = entity;
            debug_entity.name = signName;
            debug_entity.SetActive(true);
            debug_entity.transform.SetParent(parent.transform,true);
        }
    }
    public virtual void SetupGizmos()
    {
        #if UNITY_EDITOR
        if (debug_entity)
        {
            debug_entity.transform.position = point;
            EditorGizmosIconManager.SetIcon(debug_entity, EditorGizmosIconManager.LabelIcon.Blue);
        }
        #endif
    }
    public virtual void LifeTimeOver()
    {
        if (debug_entity != null)
        {
            GameObjectFactory.Instance.PushItem(debug_entity);
        }
    }
}
public class WireSphereGizmosEntity:MyDebugGizmosEntity
{
    public float radius;

    public WireSphereGizmosEntity(Vector3 point, float radius,Color color,float time, string _signName):base(color,time,_signName)
    {
        SetPoint(point);
        this.radius = radius;
    }

    public override void Debug()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(point, radius);
    }
}
public class WireBoxGizmosEntity:MyDebugGizmosEntity
{
    public Vector3 halfExtent;

    public WireBoxGizmosEntity(Vector3 point, Vector3 halfExtent, Color color, float time, string _signName) : base(color, time,_signName)
    {
        SetPoint(point);
        this.halfExtent = new Vector3(halfExtent.x*2,halfExtent.y*2,halfExtent.z*2);
    }

    public override void Debug()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(point, halfExtent);
    }
}
public static class DebugerFactory
{
    public static string GetDebugerPartFormatStr(MyDebugType debugTYpe)
    {
        string part = "";
        switch (debugTYpe)
        {
            case MyDebugType.Print:
                part = "[Logger:]";
                break;
            case MyDebugType.Warning:
                part = "[Logger-Warning:]";
                break;
            case MyDebugType.Error:
                part = "[Logger-Error:]";
                break;
            case MyDebugType.SupperError:
                part = "[Logger-SupperError:]";
                break;
            default:
                break;
        }
        return part;
    }
}
