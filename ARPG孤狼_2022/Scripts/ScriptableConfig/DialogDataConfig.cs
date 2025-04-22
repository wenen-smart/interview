using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName ="DialogDataConfig",menuName = "CreateConfig/CreateDialogDataConfig",order = 6)]
public class DialogDataConfig:ScriptableConfigTon<DialogDataConfig>
{
    new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.DialogDataConfigPath;
    }
    [SerializeField]
    private DialogItemData[] dialogItemDatas;
    public Dictionary<int, DialogItemData> DialogItemDataDic = new Dictionary<int, DialogItemData>();
    public override void OnEnable()
    {
        base.OnEnable();
        foreach (var item in dialogItemDatas)
        {
            DialogItemDataDic.Add(item.dialogID, item);
        }
    }
    public DialogItemData GetDialogItemData(int id)
    {
        DialogItemData item;
        DialogItemDataDic.TryGetValue(id, out item);
        return item;
    }
}
[Serializable]
public class DialogItemData
{
    public string description;
    public int dialogID;
    public string[] dialogs;
    public int Count => dialogs.Length;
}

public enum DialogType
{
    Board,
    FloatLabel
}

