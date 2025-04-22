using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Game : InstanceMono<Game>, I_Init
{
    public void Init()
    {

    }
    public void GainItem(string item)
    {
        NGUI_GameMainPopup gameMainPopup = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.GameMain) as NGUI_GameMainPopup;
        gameMainPopup.ShowGainItemContainer("获得"+item);
    }
    public void EquipItem(string item)
    {
        NGUI_GameMainPopup gameMainPopup = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.GameMain) as NGUI_GameMainPopup;
        gameMainPopup.ShowGainItemContainer("装备" + item);
    }
    public void ShowMainEventTip(string item)
    {
        NGUI_GameMainPopup gameMainPopup = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.GameMain) as NGUI_GameMainPopup;
        gameMainPopup.ShowMainEventTip("按下E" + item);
    }
    public void HideMainEventTip()
    {
        NGUI_GameMainPopup gameMainPopup = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.GameMain) as NGUI_GameMainPopup;
        gameMainPopup.HideTipContainer();
    }
    public void StartGame(bool isContinue)
    {
        UIManager.Instance.PopPanel();
        if (isContinue==false)
        {
            GameLoader.LoadScene(SCENE_NAME.Map);
        }
    }
}

