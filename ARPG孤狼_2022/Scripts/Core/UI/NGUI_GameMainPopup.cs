using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGUI_GameMainPopup : BasePanel
{
    private static NGUI_GameMainPopup instance;
    public static NGUI_GameMainPopup Instance=>instance;
    [SerializeField]
    private Transform CrosshairTrans;
    [SerializeField]
    private UIWidget foreSinglePoint;
    [SerializeField]
    private GainItemContainer gainItemContainer;
    private Stack<string> gainItemStack=new Stack<string>();
    [SerializeField]
    private KeyEventTip mainEventTip;
    
    public HpBarEntity playerHPBarEntity;
    public override void Enable()
    {
        base.Enable();
        instance = this;
    }
    /// <summary>
    /// 打开十字准星
    /// </summary>
    public void OpenCrosshairUI()
    {
        CrosshairTrans.gameObject.SetActive(true);
    }
    /// <summary>
    /// 关闭十字准星
    /// </summary>
    public void CloseCrosshairUI()
    {
        CrosshairTrans.gameObject.SetActive(false);
    }
    /// <summary>
    /// 打开圆点准星
    /// </summary>
    public void OpenForeSinglePoint()
    {
        foreSinglePoint.gameObject.SetActive(true);
    }
    /// <summary>
    /// 关闭圆点准星
    /// </summary>
    public void CloseForeSinglePoint()
    {
        foreSinglePoint.gameObject.SetActive(false);
    }
    public void UpdateForeSinglePointPos(Vector3 worldPoint)
    {
        if (foreSinglePoint.gameObject.activeSelf)
        {
            foreSinglePoint.transform.position=CameraMgr.WorldPointToNGUIWorldPoint(worldPoint);
        }
    }
    public void ShowGainItemContainer(string info)
    {
        gainItemStack.Push(info);
    }
    public void ShowMainEventTip(string info)
    {
        mainEventTip.ShowTipContainer(info);
    }
    public void HideTipContainer()
    {
        mainEventTip.HideTipContainer();
    }
    public void Update()
    {
        if (gainItemStack.Count>0)
        {
            if (gainItemContainer.gameObject.activeSelf==false)
            {
                gainItemContainer.ShowGainItemContainer(gainItemStack.Pop());
            }
        }
    }
}
