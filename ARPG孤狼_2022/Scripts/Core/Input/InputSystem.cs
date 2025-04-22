using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : InstanceMono<InputSystem>
{
    public bool pressShiftBtn =>shiftEvent.isPressing&&GameRoot.Instance.DisableRoleMonoInput==false;
    public bool pressShiftBtnExtend=>shiftEvent.isExtending&&GameRoot.Instance.DisableRoleMonoInput==false;
    public bool pressDownShiftBtn=>shiftEvent.OnPressed&&GameRoot.Instance.DisableRoleMonoInput==false;
    public bool pressDownJumpBtn=>jumpEvent.OnPressed&&GameRoot.Instance.DisableRoleMonoInput==false;

    public bool pressDownMouseLBtn => key_MouseLeft_Event.OnPressed  && GameRoot.Instance.DisableRoleMonoInput == false;
    public bool pressMouseLBtn=>key_MouseLeft_Event.isPressing&&GameRoot.Instance.DisableRoleMonoInput==false;

    
    public KeyCodeEvent jumpEvent;
    public KeyCodeEvent shiftEvent;
    public KeyCodeEvent key_E_MainEvent;
    public KeyCodeEvent key_MouseMiddle_Event;
    public KeyCodeEvent key_MouseLeft_Event;
    public KeyCodeEvent key_MouseRight_Event;
    public KeyCodeEvent key_AltL_Event;
    public KeyCodeEvent key_B_Event;
    public KeyCodeEvent key_I_Event;

    public KeyCodeEvent key_TestEvent;

    public bool pressJumpBtn=>jumpEvent.isPressing&&GameRoot.Instance.DisableRoleMonoInput==false;
    public float jumpValue;//KeyCode E
    public Func<bool> mainEventSaver;

    public bool mouseRightIsPressed =>key_MouseRight_Event.isPressing&&GameRoot.Instance.DisableRoleMonoInput==false;
    public bool pressDownMouseRBtn => key_MouseRight_Event.OnPressed&&GameRoot.Instance.DisableRoleMonoInput==false;
    public bool pressUpMouseLBtn=>key_MouseLeft_Event.onReleased&&GameRoot.Instance.DisableRoleMonoInput==false;
    public bool pressUpMouseRBtn=>key_MouseRight_Event.onReleased&&GameRoot.Instance.DisableRoleMonoInput==false;
    public bool pressUpMouseRExtend => key_MouseRight_Event.isExtending&&GameRoot.Instance.DisableRoleMonoInput==false;
    public bool pressDownAltLBtn => key_AltL_Event.OnPressed&&GameRoot.Instance.DisableRoleMonoInput==false;
    public Dictionary<KeyCode, KeyCodeEvent> keyHandleEventDic=new Dictionary<KeyCode, KeyCodeEvent>();//存储 按键监听者 可用于处理键盘对应的事件
    public Vector2 inputDir;
    public float scrollWheel = 0;

    public void Start()
    {

        jumpEvent = new KeyCodeEvent(0,true);
        shiftEvent = new KeyCodeEvent(0.2f,true);
        key_E_MainEvent = new KeyCodeEvent(0);
        key_MouseMiddle_Event = new KeyCodeEvent(0,false);
        key_MouseLeft_Event= new KeyCodeEvent(0, false);
        key_MouseRight_Event = new KeyCodeEvent(0.2f,false);
        key_AltL_Event = new KeyCodeEvent(0,false);
        key_B_Event = new KeyCodeEvent(0,false);
        key_TestEvent = new KeyCodeEvent(0,false);
        key_I_Event = new KeyCodeEvent(0,false);

        AddKeyCodeHandlerEvent(KeyCode.E,key_E_MainEvent);
        AddKeyCodeHandlerEvent(KeyCode.Space, jumpEvent);
        AddKeyCodeHandlerEvent(KeyCode.LeftShift,shiftEvent);
        AddKeyCodeHandlerEvent(KeyCode.B,key_B_Event);
        AddKeyCodeHandlerEvent(KeyCode.N,key_TestEvent);
        AddKeyCodeHandlerEvent(KeyCode.I,key_I_Event);
        
    }
    // Update is called once per frame
    void Update()
    {
        if (GameRoot.GameState_!=GameState.GAMEING)
        {
            return;
        }
        jumpEvent.Tick(Input.GetKey(KeyCode.Space));
        shiftEvent.Tick(Input.GetKey(KeyCode.LeftShift));
        key_E_MainEvent.Tick(Input.GetKey(KeyCode.E));
        key_MouseMiddle_Event.Tick(Input.GetMouseButton(2));
        key_MouseLeft_Event.Tick(Input.GetMouseButton(0));
        key_MouseRight_Event.Tick(Input.GetMouseButton(1));
        key_AltL_Event.Tick(Input.GetKey(KeyCode.LeftAlt));
        key_B_Event.Tick(Input.GetKey(KeyCode.B));
        key_TestEvent.Tick(Input.GetKey(KeyCode.N));
        key_I_Event.Tick(Input.GetKey(KeyCode.I));

        jumpValue = jumpEvent.pressAxis;
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        inputDir = GameRoot.Instance.DisableRoleMonoInput==false?new Vector2(h, v):Vector2.zero;
        scrollWheel = GameRoot.Instance.DisableRoleMonoInput==false?Input.GetAxis("Mouse ScrollWheel"):0;
    }
    //为对应按键添加监听类
    public void AddKeyCodeHandlerEvent(KeyCode keyCode, KeyCodeEvent keyCodeEvent)
    {
        if (keyHandleEventDic!=null&&keyHandleEventDic.ContainsKey(keyCode)==false)
        {
            keyHandleEventDic.Add(keyCode,keyCodeEvent);

        }
    }
    public void Sign_KeyHandleEvent(KeyCode key, EventCode eventCode, Func<bool> action)
    {
        if (keyHandleEventDic!=null&& keyHandleEventDic.ContainsKey(key))
        {
            keyHandleEventDic[key].SignHandlerEvent(eventCode, action);
        }
        else
        {
            Debug.LogError("keyHandleEventDic中没有对应按键的监听者。"+key.ToString());
        }
    }
    public void Leave_KeyHandlerEvent(KeyCode key, EventCode eventCode)
    {
        if (keyHandleEventDic != null && keyHandleEventDic.ContainsKey(key))
        {
            keyHandleEventDic[key].LeaveHandlerEvent(eventCode);
        }
        else
        {
            Debug.Log("keyHandleEventDic中没有对应按键的监听者。");
        }
    }
        public bool CurrentPressMoveKey()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            return true;
        }
        return false;
    }
    public bool CurrentPressDownMoveKey()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            return true;
        }
        return false;
    }
}
