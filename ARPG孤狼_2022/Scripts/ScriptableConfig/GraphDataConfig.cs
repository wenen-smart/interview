using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="GraphDataConfig",menuName = "CreateConfig/CreateGraphDataConfig",order = 3)]
public class GraphDataConfig : ScriptableConfigTon<GraphDataConfig>
{
    new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.GraphDataConfigPath;
    }
    [SerializeField]
    private GraphItemData[] graphItemDatas;

    public Dictionary<int, GraphItemData> graphItemDataDic = new Dictionary<int, GraphItemData>();
    public override void OnEnable()
    {
        base.OnEnable();
        foreach (var item in graphItemDatas)
        {
            graphItemDataDic.Add(item.id,item);
        }
    }
    public GraphItemData GetGraphItemData(int id)
    {
        GraphItemData item;
        graphItemDataDic.TryGetValue(id,out item);
        return item;
    }
}
[System.Serializable]

public class GraphItemData
{
    public string description;
    public int id;
    public Vector3[] graphPoints;
    [Header("容差角度"),Range(0,180)]
    public int faultAngle;//容差角度
    public bool isLoopGraph;
    public Gradient colorGradient;
    public bool isCircle;
    public FaultTolerantWay faultTolerantWay;
    public int Corner
    {
        get
        {
            return isLoopGraph ?graphPoints.Length:graphPoints.Length-1;
        }
    }
    [Header("最低相似度"),Range(0,1)]
    public float MinimumMatch=0.5f;//0-1 相似度大于多少算成功的
}
public enum FaultTolerantWay
{
    Default=1,//根据偏离的容差角度判断是否拐点。
    Circle=1<<1
}