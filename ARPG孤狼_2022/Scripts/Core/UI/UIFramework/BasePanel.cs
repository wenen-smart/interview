using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePanel : MonoBehaviour {
    public PopupDefinition Definition;
    [HideInInspector]
    private bool isInitialized = false;

    public bool IsInitialized { get => isInitialized; set => isInitialized = value; }

    public virtual void Init()
    {
        MyDebug.DebugPrint($"{Definition.GetUIPanelIdentity()}界面完成初始化");
    }
    /// <summary>
    /// 打开界面时触发 重启界面不触发。
    /// </summary>
	public virtual void Enable()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 关闭界面时触发 但界面暂停时不触发该函数
    /// </summary>
   public virtual void Exit()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 界面暂停时使触发
    /// </summary>
    public virtual void  Pause()
    {

    }
    /// <summary>
    /// 界面恢复时触发
    /// </summary>
    public virtual void Resume()
    {

    }
    /// <summary>
    /// 调用本函数 隐藏本界面
    /// </summary>
    public virtual void Hide()
    {
        if (UIManager.Instance.TryPopPanel(Definition.GetUIPanelIdentity())==false)
        {
            MyDebug.DebugSupperError($"{Definition.GetUIPanelIdentity()}-界面隐藏失败");
        }
        else
        {
            MyDebug.DebugPrint($"[UIManager:]触发{Definition.GetUIPanelIdentity()}隐藏");
        }
    }
    public void PlayAudio(string path)
    {
        AudioManager.Instance.PlayAudio(path);
    }
}
