using Buff.Base;
using PETime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using static UICamera;

public enum GameState
{
    NONE=0,
    LOADING=1,
    GAMEING=1<<1,
    Pause=1<<2
}
[RequireComponent(typeof(GameTimeScaleCtrl))]
public class GameRoot : InstanceMono<GameRoot>
{
    public static GameState GameState_;
    public static bool scirptLoadCompleted=false;
    public Action updateHandler;
    public PETimer peTimer=new PETimer(0);
    public UIRoot UIROOT;
    [HideInInspector]
    public UICamera UICAMERA;
    [HideInInspector]
    public UIManager UIManagerRoot;
    [HideInInspector]public float loadScriptProgress=0;//0-1f
    [HideInInspector] public GameObject currentClickUIGO;
    public bool DisableRoleMonoInput { get { return UIManager.Instance.stateWhenUIOpen.IsSelectThisEnumInMult(GameStateWhenUIOpen.DisInput,true) || (UIManager.Instance.stateWhenUIOpen.IsSelectThisEnumInMult(GameStateWhenUIOpen.DisInputWhenMouseClickUI) == true &&currentClickUIGO!=null); } }
    private Game _Game;
    public override void Awake()
    {
        base.Awake();
        UIManagerRoot=UIManager.Instance;
        UICAMERA = UIROOT.transform.Find("Camera").GetComponent<UICamera>();
        CoreEventSystem.Instance.RegisterEvent(EventCode.Debug,new CoreEventHandler((arg)=> { Debug.Log(((BuffBase)arg.value[0]).buffData.currency); }));
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UIROOT.gameObject);
        UICamera.onPress += OnNGUIClick;
        if (Game.Instance == null)
        {
            _Game = gameObject.AddComponent<Game>();
            Game.Instance.Init();
        }
    }
    public void OnNGUIClick(GameObject selectGo,bool state)
    {
        if (state==false)
        {
            currentClickUIGO = null;
            Debug.Log("点击完成:" + selectGo.name+"Frame:"+Time.frameCount);
        }
        if (selectGo!=UIROOT.gameObject&&state==true)
        {
            currentClickUIGO = selectGo;
            Debug.Log("Click:" + selectGo.name+"Frame:"+Time.frameCount);
        }
    }
    public void Update()
    {
        updateHandler?.Invoke();
        peTimer.OnTick();
#if UNITY_EDITOR
        Test();
#endif
    }
    public void LateUpdate()
    {
        if (UICAMERA.processEventsIn == ProcessEventsIn.Update) currentClickUIGO=null;
    }
    public void ShowEventBtnPressTip(KeyCode keycode,string actionTip)
    {
        MyDebug.DebugPrint($"注册键盘事件{keycode}"+actionTip);
        if (keycode==KeyCode.E)
        {
            Game.Instance.ShowMainEventTip(actionTip);
        }
    }
    public void HideEventBtnPressTip(KeyCode keycode)
    {
        if (keycode == KeyCode.E)
        {
            Game.Instance.HideMainEventTip();
        }
    }
    public void SignHandlerMainEvent(EventInfo eventInfo,Func<bool> action)
    {
        Sign_Key_HandleEvent(KeyCode.E, eventInfo, action);
    }
    public void LeaveHandlerMainEvent(EventCode eventCode)
    {
        Leave_Key_HandlerEvent(KeyCode.E,eventCode);
    }

    public void Sign_Key_HandleEvent(KeyCode keyCode,EventInfo eventInfo,Func<bool> action)
    {
        if (DisableRoleMonoInput==true&&eventInfo.keyBehaviourType==KeyBehaviourType.RoleBehaviour)
        {
            return;
        }
        InputSystem.Instance.Sign_KeyHandleEvent(keyCode, eventInfo.eventCode, action);
        ShowEventBtnPressTip(keyCode,eventInfo.eventTips);
        
    }
    public void Leave_Key_HandlerEvent(KeyCode keyCode,EventCode eventCode)
    {
        if (DisableRoleMonoInput == true)
        {
            return;
        }
        InputSystem.Instance.Leave_KeyHandlerEvent(keyCode, eventCode);
        HideEventBtnPressTip(keyCode);
    }
    public IEnumerator StayExcuteOnMuchFrame(int frame,Action action)
    {
        int all = 0;
        while (all<frame)
        {
            action?.Invoke();
            all++;
            yield return 0;
        }
    }

    public void AddTask(int ms,Action action)
    {
        AddTimeTask(ms,action);
    }
    public bool DelTimeTask(int tid)
    {
        return peTimer.DeleteTimeTask(tid);
    }
    public int AddTimeTask(float delay,Action callBack,PETimeUnit unit=PETimeUnit.MillSeconds,int loop=1,Action lifeDestoryCallback=null)
    {
        return peTimer.AddTimeTask(callBack,lifeDestoryCallback,delay,unit,loop,false);
    }
    public int AddFrameTask(int frame,Action callBack,int loop=1,Action lifeDestoryCallback=null)
    {
        return peTimer.AddFrameTask(callBack,lifeDestoryCallback,frame,loop);
    }

    public void StartLoadScript()
    {
       ExcelDataObject excelDataObject=ConfigResFactory.LoadConifg<ExcelDataObject>(PathDefine.scriptManagerConfigPath);
        List<ExcelRowData> rows = excelDataObject._Rows;
        List<LoadScriptConfigData> loadScriptConfigDatas = new List<LoadScriptConfigData>();
        foreach (var row in rows)
        {
            string scriptName = row[(int)_Data_ScriptManagerConfig.Script_Name];
            if (string.IsNullOrEmpty(scriptName) ==false)
            {
                loadScriptConfigDatas.Add(new LoadScriptConfigData(scriptName,Convert.ToInt32(row[(int)_Data_ScriptManagerConfig.Load_Sequence]), Convert.ToBoolean(row[(int)_Data_ScriptManagerConfig.IsNewContrust])));
            }
        }
        loadScriptConfigDatas.Sort((l,l2)=> { return l.sequence > l2.sequence?1:0; });
        StartCoroutine(LoadScript(loadScriptConfigDatas));
    }
    private IEnumerator LoadScript(List<LoadScriptConfigData> datas)
    {
        loadScriptProgress = 0;
        if (datas==null)
        {
            yield break;
        }
        for (int i = 0; i < datas.Count; i++)
        {
            LoadScriptConfigData scriptConfig=datas[i];
            ILoadScript loadScript=null;
            Type type = Type.GetType(scriptConfig.scriptName);
            if (scriptConfig.isContrust)
            {
                loadScript=Activator.CreateInstance(type) as ILoadScript;
                Type genericType = Type.GetType("CommonSingleTon"+"`1");
                if (genericType!=null)
                {
                    genericType=genericType.MakeGenericType(type);
                    if (type.IsSubclassOf(genericType))
                    {
                        object instance =loadScript /*TypeDescriptor.GetConverter(typeof(ILoadScript)).ConvertTo(loadScript, type)*/;
                        FieldInfo fieldInfo = genericType.GetField("_Instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        fieldInfo.SetValue(instance, instance);
                    }
                }
            }
            else if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                loadScript = FindObjectOfType(type) as  ILoadScript;
            }
            if (loadScript!=null)
            {
                loadScript.LoadScript(null);
            }
            else
            {
                Debug.LogError("Load Script Error: "+scriptConfig.scriptName+" 脚本加载初始化失败");
            }
            yield return 0;
            loadScriptProgress = i / (datas.Count-1.0f);
        }
        LoadScriptComplete();
    }
    private void LoadScriptComplete()
    {
        scirptLoadCompleted = true;
        loadScriptProgress = 1;
        BattleManager.Instance.Init();
    }
    private void ShowDialog(int id,DialogType dialogType,float showTime=-1,bool isPauseOtherPanel=true)
    {
        DialogItemData dialogItemData = DialogDataConfig.Instance.GetDialogItemData(id);
        if (dialogItemData==null)
        {
            MyDebug.DebugError($"获取不到对话数据 id:{id}");
            return;
        }
        switch (dialogType)
        {
            case DialogType.Board:
                break;
            case DialogType.FloatLabel:
                if (UIControl.GetPopupShowState(UIPanelIdentity.GraphRecognitionPanel))
                {
                    BasePanel basePanel = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.GraphRecognitionPanel);
                }
                UIManager.Instance.PushPanel(new FloatDialogPopupDefinition(dialogItemData,showTime,UILayerType.TopPopup,isPauseOtherPanel));
                break;
            default:
                break;
        }
    }
    private void ShowDialog(string[] dialogs, DialogType dialogType, float showTime = -1, bool isPauseOtherPanel = true)
    {
        DialogItemData dialogItemData = new DialogItemData() { dialogID = -1, dialogs = dialogs };
        if (dialogItemData == null)
        {
            MyDebug.DebugError($"获取不到对话数据");
            return;
        }
        switch (dialogType)
        {
            case DialogType.Board:
                break;
            case DialogType.FloatLabel:
                if (UIControl.GetPopupShowState(UIPanelIdentity.GraphRecognitionPanel))
                {
                    BasePanel basePanel = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.GraphRecognitionPanel);
                }
                UIManager.Instance.PushPanel(new FloatDialogPopupDefinition(dialogItemData, showTime, UILayerType.TopPopup, isPauseOtherPanel));
                break;
            default:
                break;
        }
    }
    public void ShowDialog(int id)
    {
        ShowDialog(id,DialogType.Board);
    }
    public void ShowFloatDialog(int id,float showTime,bool stillPushWhenHaveDialog=true,bool isPauseOtherPanel=true)
    {
        if (stillPushWhenHaveDialog==false)
        {
            BasePanel dialog = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.FloatDialogPanel);
            if (dialog)
            {
                if (dialog.gameObject.activeSelf==true)
                {
                    return;
                }
            }
        }
        ShowDialog(id,DialogType.FloatLabel,showTime,isPauseOtherPanel);
    }
    public void ShowFloatDialog(string[] dialogs, float showTime, bool isPauseOtherPanel = true)
    {
        ShowDialog(dialogs, DialogType.FloatLabel, showTime, isPauseOtherPanel);
    }


    #region Test
    public void Test()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //TimeLineManager.Instance.AnimatorToTimeLine(PlayerFacade.Instance.playerAnim.anim, "APose_SPAttack05", PlayerFacade.Instance.playerAnim.stateInfo,EndKillTimeLineConfig.Instance.);

        }
    }
    public void LoadGame()
    {
        //检测是否有存档
        //---无
        //----开始新游戏
        //---加载存档
    }
    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void LoadNewGame()
    {

    }
    public void GameLoadComplete()
    {
        MyDebug.DebugPrint("[GameLoad]:-GameLoadComplete");
        UIManager.Instance.TryPopAllPanel();
        UIManager.Instance.PushPanel(new PopupDefinition(UIPanelIdentity.GameMain));
    }
#endregion
}

public class LoadScriptConfigData
{
    public string scriptName;
    public int sequence;
    public bool isContrust;

    public LoadScriptConfigData(string scriptName, int sequence, bool isContrust)
    {
        this.scriptName = scriptName;
        this.sequence = sequence;
        this.isContrust = isContrust;
    }
}