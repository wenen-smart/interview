using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIControl:InstanceMono<UIControl>
{
    public KeyCode ctrlInventoryKeycode;
    public KeyCode ctrlTestKeyCode;
    private void Start()
    {
        GameRoot.Instance.Sign_Key_HandleEvent(ctrlInventoryKeycode,new EventInfo("打开背包",EventCode.OpenInventory,KeyBehaviourType.UIBehaviour),OpenInventory);
        GameRoot.Instance.Sign_Key_HandleEvent(KeyCode.I,new EventInfo("打开图形识别界面",EventCode.OpenCharacter,KeyBehaviourType.UIBehaviour),OpenCharacterPopup);
        GameRoot.Instance.Sign_Key_HandleEvent(ctrlTestKeyCode,new EventInfo("打开图形识别界面",EventCode.TestGraphRecognition,KeyBehaviourType.UIBehaviour),TestGraphRecognition);
    }
    public bool OpenInventory()
    {
        bool isShow = GetPopupShowState(UIPanelIdentity.InventoryPanel);
        if (isShow)
        {
            UIManager.Instance.TryPopPanel(UIPanelIdentity.InventoryPanel);
        }
        else
        {
            UIManager.Instance.PushPanel(new InventoryPopupDefinition(PlayerFacade.Instance.actorSystem.roleData.InventoryData));
        }
        return false;
    }
    public static bool GetPopupShowState(UIPanelIdentity identity)
    {
        bool isShow = false;
        isShow = UIManager.Instance.GetTopPopup() != null && UIManager.Instance.GetTopPopup().Definition.GetUIPanelIdentity() == identity;
        return isShow;
    }

    public bool OpenCharacterPopup()
    {
        bool isShow = GetPopupShowState(UIPanelIdentity.CharacterPopup);
        if (isShow)
        {
            UIManager.Instance.TryPopPanel(UIPanelIdentity.CharacterPopup);
        }
        else
        {
            UIManager.Instance.PushPanel(new PopupDefinition(UIPanelIdentity.CharacterPopup));
        }
        return false;
    }

    int i=-1;
    public bool TestGraphRecognition()
    {

        bool isShow = GetPopupShowState(UIPanelIdentity.GraphRecognitionPanel);
        if (isShow)
        {
            UIManager.Instance.TryPopPanel(UIPanelIdentity.GraphRecognitionPanel);
        }
        else
        {
            i += 1;
            i %= GraphDataConfig.Instance.graphItemDataDic.Count;
            var graphItemData = GraphDataConfig.Instance.GetGraphItemData(i);
            UIManager.Instance.PushPanel(new GraphRecognitionPopupDefinition(graphItemData, (matchPercent) =>
            {
                int[] failedDialogID = new int[3];
                if (i == 0)
                {
                    failedDialogID[0] = 3;
                    failedDialogID[1] = 2;
                    failedDialogID[2] = 1;

                }
                else
                {
                    failedDialogID[0] = 4;
                    failedDialogID[1] = 2;
                    failedDialogID[2] = 1;
                }
                double onceMatchLv = graphItemData.MinimumMatch / 3;
                if (matchPercent <= onceMatchLv)
                {
                    GameRoot.Instance.ShowFloatDialog(failedDialogID[0], 1.5f);
                }
                else if (matchPercent <= onceMatchLv * 2)
                {
                    GameRoot.Instance.ShowFloatDialog(failedDialogID[1], 1.5f);

                }
                else if (matchPercent <= onceMatchLv * 3)
                {
                    GameRoot.Instance.ShowFloatDialog(failedDialogID[2], 1.5f);
                }
            },
            (matchPer) =>
            {
                string suc = $"画出了{graphItemData.description}，匹配度竟然高达{(int)(matchPer * 100)}%";
                GameRoot.Instance.ShowFloatDialog(new string[] { suc }, 1.5f);
                UIManager.Instance.TryPopPanel(UIPanelIdentity.GraphRecognitionPanel);
            }
            ));


        }
        return false;
    }
}

