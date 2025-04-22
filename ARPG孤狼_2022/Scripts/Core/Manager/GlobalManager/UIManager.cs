using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager {
    private Dictionary<UIPanelIdentity, string> pathDict;
    private Dictionary<UIPanelIdentity, BasePanel> panelDict;
    private Stack<BasePanel> _panelStack;//Default Layer
    private Stack<BasePanel> _topPopupStack;//Top Layer;


    private Transform canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform==null)
            {
                canvasTransform = GameObject.Find(Constants.NGUIROOT).transform;
            }
            return canvasTransform;
        }
    }
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }
    private UIPanelJson jsonList;
    public GameStateWhenUIOpen stateWhenUIOpen { get{return CheckGameStateInUI(); }}

	private UIManager()
    {
        ParseUIPanelTypeJson();
    }

    public class UIPanelJson
    {
        public List<UIPanelInfo> infoList;
    }
    private void ParseUIPanelTypeJson()
    {
        if (pathDict == null) pathDict = new Dictionary<UIPanelIdentity, string>();
        TextAsset tajson = Resources.Load<TextAsset>(PathDefine.UIPanelJsonInfo);
        jsonList=  JsonUtility.FromJson<UIPanelJson>(tajson.text);

        foreach (UIPanelInfo item in jsonList.infoList)
        {
            pathDict.Add(item.panelIdentity,item.path);
        }
        PreLoadPrefab();
    }
    private BasePanel GetPanel(PopupDefinition definition)
    {
        BasePanel panel = GetPanel(definition.GetUIPanelIdentity());
        panel.Definition = definition;
        return panel;
    }
    /// <summary>
    /// 根据面板标识返回 栈中的具体的面板。
    /// </summary>
    /// <param name="paneltype"></param>
    /// <returns></returns>
    private BasePanel GetPanel(UIPanelIdentity paneltype)
    {
        if (panelDict == null) panelDict = new Dictionary<UIPanelIdentity, BasePanel>();
        BasePanel panel;
        panelDict.TryGetValue(paneltype, out panel);
        if (panel==null)
        {
            string path;
            pathDict.TryGetValue(paneltype,out path);
            if (path==null)
            {
                pathDict.Clear();
                ParseUIPanelTypeJson();
                pathDict.TryGetValue(paneltype,out path);
            }
             GameObject goPanel=GameObjectFactory.Instance.PopItem(path);
            goPanel.transform.SetParent(CanvasTransform,false);
            panel = goPanel.GetComponent<BasePanel>();
            panelDict.Add(paneltype,panel);
        }
        return panel;
    }

    public BasePanel TryGetAExistPanel(UIPanelIdentity paneltype)
    {
        if (panelDict == null) panelDict = new Dictionary<UIPanelIdentity, BasePanel>();
        BasePanel panel;
        panelDict.TryGetValue(paneltype, out panel);
        return panel;
    }

    public void PushPanel(PopupDefinition definition)
    {
        Stack<BasePanel> panelStack = GetPanelStack(definition.LayerType);
        if (panelStack == null) panelStack = new Stack<BasePanel>();

        if (panelStack.Count>0)
        {
            BasePanel topPanel = panelStack.Peek();
            if (definition.PauseOtherWhenPopup)
            {
                topPanel.Pause();
            }
        }
        BasePanel panel = GetPanel(definition);
        if (definition.panelInfo==null)
        {
            foreach (var info in jsonList.infoList)
            {
                if (info.panelIdentity == definition.GetUIPanelIdentity())
                {
                    definition.panelInfo = info;
                }
            }
        }
        if (panel.IsInitialized == false)
        {
            panel.Init();
            panel.IsInitialized = true;
        }
        panel.Enable();
        panelStack.Push(panel);
    }
    public void PopPanel(UILayerType layerType=UILayerType.Default)
    {
        Stack<BasePanel> panelStack = GetPanelStack(layerType);
        if (panelStack == null) panelStack = new Stack<BasePanel>();

        if (panelStack.Count <= 0) return;

      BasePanel thisPanel=  panelStack.Pop();
        thisPanel.Exit();
        if (panelStack.Count <= 0) return;
        BasePanel lastPanel = panelStack.Peek();
        if (thisPanel.Definition.PauseOtherWhenPopup)
        {
            lastPanel.Resume();
        }
    }
    public bool TryPopPanel(UIPanelIdentity panelIdentity)
    {
        BasePanel panel = UIManager.Instance.GetTopPopup();
        bool isPop = false;
        if (panel!=null)
        {
            isPop=PanelIsSure(panel,panelIdentity);
        }
        if (panel==null||isPop==false)
        {
            panel=UIManager.Instance.GetTopPopup(UILayerType.TopPopup);
            isPop=PanelIsSure(panel,panelIdentity);
        }
        if (isPop)
        {
            UIManager.Instance.PopPanel(panel.Definition.LayerType);
        }
        return isPop;
    }
    public bool PanelIsSure(BasePanel panel,UIPanelIdentity panelIdentity)
    {
        if (panel.Definition.GetUIPanelIdentity() == panelIdentity)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 返回当前栈顶界面
    /// </summary>
    /// <returns></returns>
    public BasePanel GetTopPopup(UILayerType layerType=UILayerType.Default)
    {
        Stack<BasePanel> panelStack = GetPanelStack(layerType);
        if (panelStack == null) return null;

        if (panelStack.Count <= 0) return null;
        return panelStack.Peek();
    }
    public void TryPopAllPanel()
    {
        TryPopALayerPanel(UILayerType.Default);
        TryPopALayerPanel(UILayerType.TopPopup);
    }
    public void TryPopALayerPanel(UILayerType layerType)
    {
        while (UIManager.Instance.GetTopPopup(layerType) != null)
        {
            BasePanel panel = UIManager.Instance.GetTopPopup(layerType);
            if (panel != null)
            {
                UIManager.Instance.PopPanel(layerType);
            }
        }
    }
    /// <summary>
    /// 预读取预制件
    /// </summary>
    public void PreLoadPrefab()
    {
        foreach (var path in pathDict.Values)
        {
            GameObjectFactory.Instance.LoadPrefab(path);
        }
    }

    public GameStateWhenUIOpen CheckGameStateInUI()
    {
        if (_panelStack == null&&_topPopupStack==null) return GameStateWhenUIOpen.None;

        if (_panelStack.Count <= 0&&_topPopupStack.Count<=0) return  GameStateWhenUIOpen.None;
        List<BasePanel> panelStackArray = new List<BasePanel>();
        if (_panelStack!=null&&_panelStack.Count>0)
        {
            BasePanel[] defaultPanelStackArray = _panelStack.ToArray();
            foreach (var item in defaultPanelStackArray)
            {
                panelStackArray.Add(item);
            }
        }
        if (_topPopupStack!=null&&_topPopupStack.Count>0)
        {
            BasePanel[] topPanelStackArray = _topPopupStack.ToArray();
            foreach (var item in topPanelStackArray)
            {
                panelStackArray.Add(item);
            }
        }
        GameStateWhenUIOpen state = GameStateWhenUIOpen.None;
        bool isBreakFor = false;
        foreach (var basePanel in panelStackArray)
        {
            
            switch (basePanel.Definition.panelInfo.popupType)
            {
                case UIPopupType.Common:
                    state = GameStateWhenUIOpen.AlwaysPause;
                    isBreakFor = true;
                    break;
                case UIPopupType.FloatPanel:
                    state = GameStateWhenUIOpen.DisInputWhenMouseClickUI;
                    break;
                case UIPopupType.Main:
                    state = GameStateWhenUIOpen.DisInputWhenMouseClickUI;
                    break;
                default:
                    break;
            }
            if (isBreakFor)
            {
                break;
            }
        }
        return state;
    }
    public Stack<BasePanel> GetPanelStack(UILayerType layerType)
    {
        switch (layerType)
        {
            case UILayerType.Default:
                if (_panelStack==null)
                {
                    _panelStack = new Stack<BasePanel>();
                }
                return _panelStack;
            case UILayerType.TopPopup:
                if (_topPopupStack == null)
                {
                    _topPopupStack = new Stack<BasePanel>();
                }
                return _topPopupStack;
            default:
                break;
        }
        return null;
    }
}
public enum GameStateWhenUIOpen
{
    None=0,//无限制
    DisInputWhenMouseClickUI=1,//在点击ui时 屏蔽玩家输入等交互输入。
    AlwaysPause=1<<1|DisInput,//游戏暂停
    DisInput=1<<2|DisInputWhenMouseClickUI,
}
