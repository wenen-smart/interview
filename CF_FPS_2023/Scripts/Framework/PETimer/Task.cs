using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PETime
{
    public abstract class Task
    {
        public int requireLoopCount = 0;
        public int _excuteCount = 0;
        public int TID;
        public bool autoLogStatus = false;
        public Action callBack;
        public Action complete;

        protected Task()
        {

        }
        protected Task(bool autoLogStatus)
        {
            this.autoLogStatus = autoLogStatus;
        }

        protected Task(int tID, Action callBack, Action fp_complete, int loopCount, bool autoLogStatus)
        {
            if (loopCount == 0)
            {
                loopCount = 1;
            }
            this.requireLoopCount = loopCount;
            TID = tID;
            this.autoLogStatus = autoLogStatus;
            this.callBack = callBack;
            this.complete = fp_complete;
          
        }

        public abstract TaskStatus RunStatus { get; }
        public abstract TaskStatus RunTask();
        public abstract void RefreshDest();
    }

    public class PETimeTask : Task
    {
        private double _lastDestTime;
        private double delay;
        protected double destTime;
        private PETimeUnit delayUnit;
        #region  Properly
        public double LastDestTime { get => _lastDestTime; private set => _lastDestTime = value; }
        public double DestTime { get => destTime; private set => destTime = value; }
        public override TaskStatus RunStatus
        {
            get
            {
                if (callBack == null || DestTime <= 0 || requireLoopCount == 0)
                {
                    return TaskStatus.InVaild;
                }
                else if (requireLoopCount <= _excuteCount && requireLoopCount != -1)
                {
                    return TaskStatus.Complete;
                }
                else
                {
                    return TaskStatus.RunningTimer;
                }
            }
        }
        #endregion

        public PETimeTask(int tid, double t_delay, double nowTime, PETimeUnit t_delayUnit,Action callBack, Action fp_complete, int loopCount = 1,bool _autoLogStatus=false) :base(tid, callBack, fp_complete,loopCount,_autoLogStatus)
        {
            this.TID = tid;
            delay = t_delay;
            delayUnit = t_delayUnit;
            RefreshDest(nowTime);
        }
        public override void RefreshDest()
        {
            LastDestTime = DestTime;
            DestTime += PETimeTools.ConvertTo(delay, delayUnit, PETimeUnit.MillSeconds);
        }
        public  void RefreshDest(double nowTime)
        {
            LastDestTime = DestTime;
            DestTime= nowTime+PETimeTools.ConvertTo(delay,delayUnit,PETimeUnit.MillSeconds);
        }
        public override TaskStatus RunTask()
        {
            callBack?.Invoke();
            _excuteCount++;

            return RunStatus;
        }
    }
    public class PEFrameTask : Task
    {
        private int frameCounter = 0;
        private int delayFrame;

        #region  Properly
        public int FrameCounter { get => frameCounter; set => frameCounter = value; }
        public override TaskStatus RunStatus
        {
            get
            {
                if (callBack == null || DelayFrame < 0 || requireLoopCount == 0)
                {
                    return TaskStatus.InVaild;
                }
                else if (requireLoopCount <= _excuteCount && requireLoopCount != -1)
                {
                    return TaskStatus.Complete;
                }
                else
                {
                    return TaskStatus.RunningTimer;
                }
            }
        }

        public int DelayFrame { get => delayFrame; private set => delayFrame = value; }
        #endregion

        public PEFrameTask(int tid, int t_delay,Action callBack,Action fp_complete, int loopCount = 1, bool _autoLogStatus = false) : base(tid, callBack, fp_complete, loopCount, _autoLogStatus)
        {
            this.TID = tid;
            DelayFrame = t_delay;
            RefreshDest();
        }
        public override void RefreshDest()
        {
            frameCounter = 0;
        }
        public override TaskStatus RunTask()
        {
            callBack?.Invoke();
            return RunStatus;
        }
    }
    public enum TaskStatus
    {
        //Idle,
        RunningTimer,
        Complete,
        InVaild,
    }
}