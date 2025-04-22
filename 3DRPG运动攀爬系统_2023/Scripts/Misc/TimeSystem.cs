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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoSingleTon<TimeSystem>
{
    private List<MyTimer> runtimeTimers = new List<MyTimer>();
    private Queue<MyTimer> stopRuntimeQueue = new Queue<MyTimer>();
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
    private void Update()
    {
        while (stopRuntimeQueue.Count > 0)
        {
            runtimeTimers.Remove(stopRuntimeQueue.Dequeue());
        }
        if (runtimeTimers.Count > 0)
        {
            for (int i = 0; i < runtimeTimers.Count; i++)
            {
                runtimeTimers[i].TimerUpdate();
            }
        }
    }
}
