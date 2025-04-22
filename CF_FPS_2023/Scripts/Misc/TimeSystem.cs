/**
 $ @Author       : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @Date         : 2023-01-10 18:53:29
 $ @LastEditors  : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @LastEditTime : 2023-01-10 21:05:09
 $ @FilePath     : \MovementProject\Assets\Resolution\Scripts\TimeSystem.cs
 $ @Description  : 
 $ @
 $ @Copyright (c) 2023 by unity-mircale 9944586+unity-mircale@user.noreply.gitee.com, All Rights Reserved. 
 **/
using PETime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-2000)]
public class TimeSystem : MonoSingleTon<TimeSystem>
{
    private List<MyTimer> runtimeTimers = new List<MyTimer>();
    private Queue<MyTimer> preRuntimeTimers = new Queue<MyTimer>();
    private Queue<MyTimer> stopRuntimeQueue = new Queue<MyTimer>();
    public PETimer peTimer=new PETimer(0);



    public MyTimer CreateTimer()
    {
        // MyTimer timer = myTimerPool.GetObject();
        //timer休眠后变量值仍需保留，所以不能用对象池
        return new MyTimer();
    }
    public void TimerUpdateFinish(MyTimer myTimer)
    {
        stopRuntimeQueue.Enqueue(myTimer);
    }
    public void RegisterTimer(MyTimer myTimer)
    {
        preRuntimeTimers.Enqueue(myTimer);
    }
    private void Update()
    {
        peTimer.OnTick();
        while (stopRuntimeQueue.Count > 0)
        {
            runtimeTimers.Remove(stopRuntimeQueue.Dequeue());
        }
        while (preRuntimeTimers.Count>0)
        {
            runtimeTimers.Add(preRuntimeTimers.Dequeue());
        }
        if (runtimeTimers.Count > 0)
        {
            for (int i = 0; i < runtimeTimers.Count; i++)
            {
                runtimeTimers[i].TimerUpdate();
            }
        }
    }
    public bool DelTimeTask(int tid)
    {
        return peTimer.DeleteTimeTask(tid);
    }
    public int AddTimeTask(float delay, Action callBack, PETimeUnit unit = PETimeUnit.MillSeconds, int loop = 1, Action lifeDestoryCallback = null)
    {
        return peTimer.AddTimeTask(callBack, lifeDestoryCallback, delay, unit, loop, false);
    }
    public int AddFrameTask(int frame, Action callBack, int loop = 1, Action lifeDestoryCallback = null)
    {
        return peTimer.AddFrameTask(callBack, lifeDestoryCallback, frame, loop);
    }
    public void AddTask(int ms, Action action)
    {
        AddTimeTask(ms, action);
    }

}

