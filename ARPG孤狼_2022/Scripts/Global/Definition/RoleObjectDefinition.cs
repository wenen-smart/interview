using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RoleObjectDefinition:MatchObjectDefinition
{
    public int roleID;
    public bool haveInvertory;//是否拥有背包
    public GameDropItemKindListInfo _roleDropItemKindListInfo;
    public static Dictionary<int, RoleObjectDefinition> Definitions = new Dictionary<int, RoleObjectDefinition>();
    public static RoleObjectDefinition GetRoleDefinition(int key)
    {
        RoleObjectDefinition definition;
        Definitions.TryGetValue(key,out definition);
        if (definition==null)
        {
            MyDebug.DebugError($"根据传入Key：{key} 未获取到对应的角色配置,请检查错误原因");
        }
        return definition;
    }
    public List<DropItemKindInfo> RandomGetItemInRange()
    {
        return _roleDropItemKindListInfo.RandomGetItemInRange();
    }
}
[Serializable]
public class GameDropItemKindListInfo
{
    public DropItemKindInfo[] kindsInfo;
    public int minKindCount;
    public int maxKindCount;
    public bool IsOneCount { get { return minKindCount >= maxKindCount; } }
    private DropItemKindInfo maxWeightItemKind;
    private DropItemKindInfo FindEasyDropItemKind()
    {
        if (maxWeightItemKind!=null)
        {
            return maxWeightItemKind;
        }
        float weight=0;
        DropItemKindInfo max=null;
        foreach (var item in kindsInfo)
        {
            if (item.weight>=weight)
            {
                max = item;
            }
        }
        maxWeightItemKind = max;
        return maxWeightItemKind;
    }
    public int RandomKindCount()
    {
        if (IsOneCount)
        {
            return 1;
        }
        int count = UnityEngine.Random.Range(minKindCount, maxKindCount + 1);
        return count;
    }
    public List<DropItemKindInfo> RandomGetItemInRange()
    {
        if (kindsInfo == null)
        {
            return null;
        }
        int kindCount = RandomKindCount();
        
        List<DropItemKindInfo> itemKindInfos = new List<DropItemKindInfo>();
        
        foreach (var kindInfo in kindsInfo)
        {
            int ran = UnityEngine.Random.Range(0, 100);
            bool hit = false;
            if ((kindInfo.weight * 100) > ran)
            {
                hit = true;
            }
            if (hit)
            {
                itemKindInfos.Add(kindInfo);
            }
        }
        if (itemKindInfos.Count == 0)
        {
            itemKindInfos.Add(FindEasyDropItemKind());
        }
        else
        {
            itemKindInfos.Sort((kindInfo1, kindinfo2) => { return kindinfo2.weight.CompareTo(kindInfo1.weight); });//从大到小 权重越大越容易得到
        }
        
        if (kindCount > itemKindInfos.Count)
        {
            kindCount = itemKindInfos.Count;
        }
        return itemKindInfos.GetRange(0, kindCount);
    }
}

/// <summary>
/// 掉落物打包  掉落物列表显示可以是一个物体对应一个item，也可以是一捆麻袋 里面装着很多item。所以涉及一个包的概念。
/// </summary>
public class DropItemPacker
{
    public List<DropItemData> dropItemKindDatas;
    public GameObject relativeObject;//无论是一群item 还是单个item  都最终关联一个物体(地上的物品)。
    public bool IsHave => dropItemKindDatas != null && dropItemKindDatas.Count > 0;
    public void AddItem(DropItemData dropItemData)
    {
        dropItemData.dropItemPacker = this;
        dropItemKindDatas.Add(dropItemData);
    }
    public void Notice(DropItemData dropItemData)
    {
        if (dropItemData.IsHave==false)
        {
            if (dropItemData.relativeObject)
            {
                GameObjectFactory.Instance.PushItem(dropItemData.relativeObject);
            }
            dropItemKindDatas.Remove(dropItemData);
        }
        if (IsHave==false)
        {
            if (relativeObject)
            {
                GameObjectFactory.Instance.PushItem(relativeObject);
            }
        }
    }
}
public class DropItemData
{
    public DropItemKindInfo dropItemKindInfo;
    private int count;
    public GameObject relativeObject;//如果没有实际物体的话，这个也可以不需要关联
    public DropItemPacker dropItemPacker;
    public DropItemData(DropItemKindInfo dropItemKindInfo, int count)
    {
        this.dropItemKindInfo = dropItemKindInfo;
        Count = count;
    }
    public bool IsHave => Count > 0;

    public int Count { get => count; private set => count = value; }
    public void Reduce(int count=1)
    {
        if (Count<count)
        {
            count = Count;
        }
        Count-=count;
        if (IsHave==false)
        {
            dropItemPacker.Notice(this);
        }
    }
}

