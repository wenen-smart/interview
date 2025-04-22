using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Internal;

[CreateAssetMenu(fileName = "DropItemListContainer", menuName = "CreateConfig/CreateDropItemListContainer", order = 1)]
public class DropItemListContainer:ScriptableConfigTon<DropItemListContainer>
{
    new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.DropItemllistContainerPath;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        foreach (DropItemKindListInfo kindListInfo in dropItemKindListInfos)
        {
            if (kindListInfo==null)
            {
                continue;
            }
            dropItemKindInfoDics.Add(kindListInfo.ID,kindListInfo.kindsInfo);
        }
    }
    public List<DropItemKindListInfo> dropItemKindListInfos;
    public Dictionary<int, DropItemKindInfo[]> dropItemKindInfoDics = new Dictionary<int, DropItemKindInfo[]>();
    public DropItemKindInfo[] GetItemKindInfo(int dropItemListID)
    {
        DropItemKindInfo[] dropItemKindInfo;
        dropItemKindInfoDics.TryGetValue(dropItemListID,out dropItemKindInfo);
        return dropItemKindInfo;
    }
}
[Serializable]
public class DropItemKindInfo
{
    public ItemDataConfig itemDataConfig;
    /// <summary>
    /// 概率
    /// </summary>
    [Range(0,1),Header("易掉落率")]
    public float weight;
    [Header("最大数量"),SerializeField]
    private int count = 1;//Default is 1
    [Range(0, 1), Header("数量最低概率")]
    public float countPercent=1;
    public int RandomCount()
    {
        if (count == 1)
        {
            return 1;
        }
        float per = UnityEngine.Random.Range(countPercent,1);
        return Mathf.Clamp((int)(count* per),1,count);
    }
}
[Serializable]
public class DropItemKindListInfo
{
    public string description;
    public int ID;
    public DropItemKindInfo[] kindsInfo;
}

