using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class KeyCodeEvent
    {

    public bool isPressing = false;//长按
    public bool OnPressed;//单次按下
    public bool onReleased;//松手
    private bool currentState;
    private bool lastState;
    private MyTimer exitTimer = new MyTimer();
    public bool isExtending;
    private float extIntervalTime;
    private bool m_enableAxis;
    public float pressAxis;
    private float target;
    private float speed = 2;

    public Func<bool> mainEventSaver;
    public KeyCodeEvent(float _extIntervalTime,bool enableAxis = false)
    {
        m_enableAxis = enableAxis;
        extIntervalTime = _extIntervalTime;
    }




    public float GetAxis()
    {
        return target;
    }


    public void Tick(bool input)
    {
        currentState = input;

        isPressing = currentState;
        OnPressed = false;
        onReleased = false;
        if (currentState != lastState)
        {
            if (currentState)
            {
                OnPressed = true;
            }
            else
            {
                onReleased = true;
                exitTimer.Go(extIntervalTime);
            }
        }

        if (OnPressed)
        {
            isPressing = false;
        }
        if (exitTimer.timerState == MyTimer.TimerState.Run)
        {
            isExtending = true;
        }
        else
            isExtending = false;
        lastState = currentState;
        if (m_enableAxis)
        {
            target = currentState ? 1 : 0;
            pressAxis = Mathf.MoveTowards(pressAxis, target, Time.deltaTime * speed);
        }
        ListenerPressHandlerEvent();
    }
    public void ListenerPressHandlerEvent()
    {
        if (OnPressed&&mainEventSaver != null)
        {
            HandleEvent();
        }
        if (tempSaver != null)
        {
            mainEventSaver = tempSaver;
            tempSaver = null;
        }
    }
    public void HandleEvent()
    {
        bool isVaildExcute = mainEventSaver.Invoke();
        if (isVaildExcute)
        {
            mainEventSaver = null;

        }

    }
    public EventCode lastEventCode;
    public Func<bool> tempSaver;//通过这个临时变量暂时存放 委托，防止监听注册操作与 按下键盘操作是同一帧。
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="action">return false that the event Loop,The Event finish When Return Result is True</param>
    public async void SignHandlerEvent(EventCode eventCode, Func<bool> action)
    {
        tempSaver = action;

        lastEventCode = eventCode;
    }
    public void LeaveHandlerEvent(EventCode eventCode)
    {
        if (lastEventCode == eventCode)
        {
            mainEventSaver = null;
        }
    }
}




   

