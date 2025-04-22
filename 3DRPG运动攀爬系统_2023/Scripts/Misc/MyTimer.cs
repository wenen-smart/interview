/**
 $ @Author       : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @Date         : 2023-01-10 18:48:53
 $ @LastEditors  : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @LastEditTime : 2023-01-10 20:57:28
 $ @FilePath     : \MovementProject\Assets\Resolution\Scripts\MyTimer.cs
 $ @Description  : 
 $ @
 $ @Copyright (c) 2023 by unity-mircale 9944586+unity-mircale@user.noreply.gitee.com, All Rights Reserved. 
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MyTimer
{
    public TimerState timerState;
    private float timer;
    private float time;
    private bool isRegister;//是否注册了GameFacade中的TimerHAndler事件
    public enum TimerState
    {
        Idle,
        Run,
        Finish,
    }
    /// <summary>
    /// 是否完成结束之后的间隔时间
    /// </summary>
    public TimerState isFinishIntervalState;
    private float finishIntervalTimer = 0;
    private float finishIntervalTime = 0;
    public MyTimer()
    {
        isRegister = false;
    }
    public void TimerUpdate()
    {
        if (timerState == TimerState.Run)
        {
            timer += Time.deltaTime;
            if (timer >= time)
            {
                Finish();
            }
        }
        else if (timerState==TimerState.Finish)
        {
            if (isFinishIntervalState==TimerState.Idle)
            {
                isFinishIntervalState = TimerState.Run;
            }
            else
            {
                if (isFinishIntervalState == TimerState.Run)
                {
                    finishIntervalTimer += Time.deltaTime;
                    if (finishIntervalTimer >= finishIntervalTime)
                    {
                        finishIntervalTimer = 0;
                        isFinishIntervalState = TimerState.Finish;
                        TimeSystem.Instance.TimerUpdateFinish(this);
                    }
                }
            }
            
        }
    }
    public void Go(float _time,float finsihInterval=0)
    {
        if (_time<=0)
        {
            return;
        }
        if (isRegister == false)
        {
            isRegister = true;
            // if (GameRoot.Instance)
            // {
            //     GameRoot.Instance.updateHandler += TimerUpdate;
            // }
        }
        timerState = TimerState.Run;
        time = _time;
        timer = 0;
        finishIntervalTime = finsihInterval;
        finishIntervalTimer = 0;
        isFinishIntervalState = TimerState.Idle;
    }
    private void Finish()
    {
        timerState = TimerState.Finish;
    }
}

