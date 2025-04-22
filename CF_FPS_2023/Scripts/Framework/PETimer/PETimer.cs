using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PETime
{
    public class PETimer:IDisposable
    {
        public List<PETimeTask> timerTaskList = new List<PETimeTask>();
        public HashSet<PETimeTask> tempTimeTasks = new HashSet<PETimeTask>();//缓存列表

        public List<PEFrameTask> timerFrameList = new List<PEFrameTask>();
        public HashSet<PEFrameTask> tempFrameTasks = new HashSet<PEFrameTask>();//缓存列表

        private List<int> tidList = new List<int>();
        private int tid = 1000;
        private object asyncLockRoot = new object();
        private List<int> deleteTidList = new List<int>();

        private Action<string> logHandler;
        private Action<Task, int> mainThreadCallback;
        private Timer threadTimer;
        private Timer logWriteThreadTimer;
        private Queue<Task> waitMainThreadExcute = new Queue<Task>();
        private double nowTime;
        private static string lockTimeTaskList = "lockTimeTaskList";
        private static string lockTempTimeTask = "lockTempTimeTask";
        private static string lockFrameTaskList = "lockFrameTaskList";
        private static string lockLogQueue = "lockLogQueue";
        private double minDestTime;
        private static bool logStatus;
        private bool m_DependOnMinDestTime=true;
        public bool EnableLogFile = false;
        private int logLine = 0;
        private Queue<string> m_logQueue = new Queue<string>();
        private int logWriterInterval = 5000;//mill
        private double frameRate = 1000f/50;//ms
        private int frame_mistake = 1;
        public double SetFrameRate { get { return frameRate; } set { frameRate = value; frame_mistake = (int)(threadTimer.Interval / frameRate); if (frame_mistake <= 0) { frame_mistake = 1; } LogInfo("实际帧纠正"+(frame_mistake-1)+"帧"); } }
        //private double writerLogDestTime = 0;
        public int SetHowLongOnceWriteLog
        {
            get { return logWriterInterval; }
            set
            {
                logWriteThreadTimer = new Timer(value);
                logWriteThreadTimer.Elapsed += (sender,args) => { LogTick(); };
                logWriteThreadTimer.AutoReset = true;
                logWriteThreadTimer.Start();
                logWriterInterval = value; /*writerLogDestTime = nowTime + value;*/ LogInfo($"设置{value}毫秒批量写入一次日志");
            }
        }
        /// <summary>
        /// 是否依赖最小目标时间进行计算，高性能（测试阶段）
        /// </summary>
        public bool DependOnMinDestTime
        {
            get { return m_DependOnMinDestTime; }
            set
            {
                m_DependOnMinDestTime = value; 
                if (value)
                {
                    LogInfo("开启了依赖最小目标时间模式，此模式在测试阶段，不确定未来其稳定性");
                } 
                else
                {
                    LogInfo("关闭了依赖最小目标时间模式，恢复正常模式。");
                }
            }
        }
        public PETimer(int interval)
        {
            if (interval > 0)
            {
                threadTimer = new Timer(interval);//---靠™ 在这条语句声明同时并不会运行线程。直接开始计时 满足时间才算一次。
                threadTimer.AutoReset = true;
                threadTimer.Elapsed += (obj, args) => { OnTick(); };
                threadTimer.Start();
            }
            nowTime = GetUTCMillSeconds();
        }
        #region Add or Remove
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="delay"></param>
        /// <param name="peTimeUnit"></param>
        /// <param name="loop" description="loop==-1 is Infinity Loop "></param>
        public int AddTimeTask(Action callback,Action fp_complete, float delay, PETimeUnit peTimeUnit = PETimeUnit.MillSeconds, int loop = 1,bool autoLogStatus= false)
        {
            int tid = GetTid();
            nowTime = GetUTCMillSeconds();
            lock (asyncLockRoot)
            {
                tidList.Add(tid);
            }
            lock (lockTimeTaskList)
            {
                PETimeTask timeTask = new PETimeTask(tid, delay, nowTime, peTimeUnit, callback, fp_complete, loop, autoLogStatus);
                tempTimeTasks.Add(timeTask);
                RefreshMinDesTime(timeTask.DestTime);
            }
            return tid;
        }
        public int AddFrameTask(Action callback, Action fp_complete, int delay, int loop = 1, bool autoLogStatus = false)
        {
            int tid = GetTid();
            PEFrameTask frameTask = new PEFrameTask(tid, delay, callback, fp_complete,loop, autoLogStatus);
            lock (asyncLockRoot)
            {
                tidList.Add(tid);
            }
            tempFrameTasks.Add(frameTask);
            return tid;
        }
        public bool DeleteTimeTask(int tid)
        {
            lock (lockTimeTaskList)
            {
                PETimeTask timeTask = timerTaskList.SingleOrDefault(task => task.TID == tid);
                if (timeTask != null)
                {
                    timerTaskList.Remove(timeTask);
                }
                else
                {
                    timeTask = tempTimeTasks.SingleOrDefault(task => task.TID == tid);
                    if (timeTask != null)
                    {
                        tempTimeTasks.Remove(timeTask);
                    }
                }
                if (timeTask != null)
                {
                    tidList.Remove(timeTask.TID);
                    return true;
                }
            }

            return false;
        }
        public bool DeleteFrameTask(int tid)
        {
            PEFrameTask frameTask = timerFrameList.SingleOrDefault(task => task.TID == tid);
            lock (lockTimeTaskList)
            {
                if (frameTask != null)
                {
                    timerFrameList.Remove(frameTask);
                }
                else
                {
                    frameTask = tempFrameTasks.SingleOrDefault(task => task.TID == tid);
                    if (frameTask != null)
                    {
                        tempFrameTasks.Remove(frameTask);
                    }
                }
            }
            lock (asyncLockRoot)
            {
                if (frameTask != null)
                {
                    tidList.Remove(frameTask.TID);
                    return true;
                }
            }
            return false;
        }
        public bool ReplaceTimeTask(int tid, Action callback, Action fp_complete, float delay, PETimeUnit peTimeUnit = PETimeUnit.MillSeconds, int loop = 1)
        {
            bool deleteSuccess = DeleteTimeTask(tid);
            if (deleteSuccess == false)
            {
                return false;
            }
            nowTime = GetUTCMillSeconds();
            lock (lockTimeTaskList)
            {
                PETimeTask timeTask = new PETimeTask(tid, delay, nowTime, peTimeUnit, callback, fp_complete, loop);
                tidList.Add(tid);
                tempTimeTasks.Add(timeTask);
                return true;
            }
        }
        public bool ReplaceFrameTask(int tid, Action callback, Action fp_complete, int delay, int loop = 1)
        {
            bool deleteSuccess = DeleteFrameTask(tid);
            if (deleteSuccess == false)
            {
                return false;
            }
            lock (lockTimeTaskList)
            {
                PEFrameTask frameTask = new PEFrameTask(tid, delay, callback, fp_complete, loop);
                tempFrameTasks.Add(frameTask);
            }
            lock (asyncLockRoot)
            {
                tidList.Add(tid);
            }
            return true;
        }
        #endregion
        #region Update
        public void OnTick()
        {
            CheckTimeTask();
            CheckFrameTask();
            RecycleTID();
        }
        public void CheckTimeTask()
        {
            if (tempTimeTasks.Count > 0)
            {
                lock (lockTimeTaskList)
                {
                    foreach (var task in tempTimeTasks)
                    {
                        timerTaskList.Add(task);
                    }
                    tempTimeTasks.Clear();
                }
            }

            if (timerTaskList.Count > 0)
            {
                nowTime = GetUTCMillSeconds();
                if (DependOnMinDestTime == false||minDestTime < nowTime)
                {
                    lock (asyncLockRoot)
                    {
                        for (int i = 0; i < timerTaskList.Count; i++)
                        {
                            var taskItem = timerTaskList[i];
                            if (taskItem.DestTime <= nowTime)
                            {
                                if (mainThreadCallback != null)
                                {
                                    mainThreadCallback.Invoke(taskItem, taskItem.TID);
                                }
                                else
                                {
                                    TaskStatus taskStatus = taskItem.RunTask();
                                    //Fsm
                                    if (TimeTaskResultHandler(taskStatus, taskItem))
                                    {
                                        i--;
                                    }
                                }
                            }
                            else if (taskItem.RunStatus == TaskStatus.InVaild)
                            {
                                RecordTaskStatus(taskItem);
                            }
                        }

                    }
                }
            }
            else
            {
                if (minDestTime != -1)
                {
                    minDestTime = -1;
                }
            }
        }
        public void CheckFrameTask()
        {
            if (tempFrameTasks != null && tempFrameTasks.Count > 0)
            {
                lock (lockTimeTaskList)
                {
                    foreach (var task in tempFrameTasks)
                    {
                        timerFrameList.Add(task);
                    }
                    tempFrameTasks.Clear();
                }
            }

            if (timerFrameList != null)
            {
                for (int i = 0; i < timerFrameList.Count; i++)
                {
                    var taskItem = timerFrameList[i];
                    taskItem.FrameCounter += frame_mistake;
                    if (taskItem.DelayFrame <= taskItem.FrameCounter)
                    {

                        if (mainThreadCallback != null)
                        {
                            int excuteCount = taskItem.FrameCounter /= taskItem.DelayFrame;
                            taskItem.RefreshDest();
                            for (int e = 0; e < excuteCount; e++)
                            {
                                taskItem._excuteCount++;
                                if (taskItem._excuteCount > taskItem.requireLoopCount)
                                {
                                    timerFrameList.Remove(taskItem);
                                    i--;
                                    break;
                                }
                                else
                                {
                                    mainThreadCallback.Invoke(taskItem, taskItem.TID);
                                }
                               
                            }
                        }
                        else
                        {
                            taskItem._excuteCount++;
                            TaskStatus taskStatus = taskItem.RunTask();
                            if (FrameTaskResultHandler(taskStatus, taskItem))
                            {
                                i--;
                            }
                            taskItem.RefreshDest();
                        }

                    }
                }
            }
        }
        #endregion
        public void SetLogger(Action<string> logger)
        {
            logHandler = logger;
        }
        public void SetMainHandler(Action<Task, int> handler)
        {
            mainThreadCallback = handler;
        }
        /// <summary>
        ///  设置任务推迟到主线程的事件
        /// </summary>
        public void SetMainHandler()
        {
            mainThreadCallback = (task, tid) =>
            {
                lock (asyncLockRoot)
                {
                    waitMainThreadExcute.Enqueue(task);
                }
            };
        }
        /// <summary>
        /// 主线程执行任务
        /// </summary>
        /// <param name="extralAction">额外的事件执行</param>
        /// <param name="replaceDefaultAct">代替默认逻辑</param>
        /// <returns></returns>
        public bool MainThreadExcuteTask(Action<Task> extralAction = null, Action replaceDefaultAct = null)
        {
            Task task = null;
            bool taskDestory = false;
            lock (asyncLockRoot)
            {
                if (replaceDefaultAct != null)
                {
                    replaceDefaultAct.Invoke();
                }
                else if (waitMainThreadExcute.Count > 0)
                {
                    task = waitMainThreadExcute.Dequeue();
                    if (task!=null)
                    {
                        TaskStatus taskStatus = task.RunTask();
                        taskDestory = TaskResuleHandler(taskStatus, task);
                    }

                }
            }
            extralAction?.Invoke(task);
            return taskDestory;
        }
        public bool TaskResuleHandler(TaskStatus taskStatus, Task taskItem)
        {
            bool taskDestory = false;
            if (taskItem is PETimeTask)
            {
                taskDestory = TimeTaskResultHandler(taskStatus, taskItem as PETimeTask);
            }
            else if (taskItem is PEFrameTask)
            {
                taskDestory = FrameTaskResultHandler(taskStatus, taskItem as PEFrameTask);
            }
            return taskDestory;
        }
        public bool TimeTaskResultHandler(TaskStatus taskStatus, PETimeTask taskItem)
        {
            bool taskDestroy = false;
            lock (lockTimeTaskList)
            {
                RecordTaskStatus(taskItem);

                if (taskStatus == TaskStatus.Complete)
                {
                    //Complete
                    //timerTaskList.RemoveAt(i);
                    taskItem.complete?.Invoke();
                    taskItem.callBack = null;
                    timerTaskList.Remove(taskItem);
                    deleteTidList.Add(taskItem.TID);
                    taskDestroy = true;
                }
                else if (taskStatus == TaskStatus.InVaild)
                {
                    //Invaid Task
                    timerTaskList.Remove(taskItem);
                    deleteTidList.Add(taskItem.TID);
                    taskDestroy = true;
                }
                else if (taskStatus == TaskStatus.RunningTimer)
                {
                    //Refresh Task DestinationTime
                    RefreshDestTime(taskItem);
                    //taskItem.RefreshDest();
                    //if (taskItem.DestTime == taskItem.LastDestTime)
                    //{
                    //    timerTaskList.Remove(taskItem);
                    //    deleteTidList.Add(taskItem.TID);
                    //    taskDestroy = true;
                    //}
                }
            }
            return taskDestroy;
        }
        public bool FrameTaskResultHandler(TaskStatus taskStatus, PEFrameTask taskItem)
        {
            bool taskDestroy = false;
            lock (lockTimeTaskList)
            {
                RecordTaskStatus(taskItem);
                //Fsm
                if (taskStatus == TaskStatus.Complete)
                {
                    //Complete
                    //timerFrameList.RemoveAt(i);
                    taskItem.complete?.Invoke();
                    taskItem.callBack = null;
                    timerFrameList.Remove(taskItem);
                    deleteTidList.Add(taskItem.TID);
                    taskDestroy = true;
                }
                else if (taskStatus == TaskStatus.InVaild)
                {
                    //Invaid Task
                    timerFrameList.Remove(taskItem);
                    deleteTidList.Add(taskItem.TID);
                    taskDestroy = true;
                }
                else if (taskStatus == TaskStatus.RunningTimer)
                {
                    //Refresh Task DestinationTime
                    //taskItem.RefreshDest();
                }

            }
            return taskDestroy;
        }
        public void LogInfo(string message)
        {
            logHandler?.Invoke(message);
            PushLogFile($"[PETime--Logger {GetLocalTimeStr()}]:"+message);
        }
        public void EnableLogTaskStatus(bool fp_logStatus)
        {
            logStatus = fp_logStatus;
        }
        public void RecordTaskStatus(PETimeTask task)
        {
            if (logStatus == false||task.autoLogStatus==false)
            {
                return;
            }
            LogInfo($"TaskID:{task.TID}--TaskStatus: " + task.RunStatus.ToString());
        }
        public void RecordTaskStatus(PETime.PEFrameTask task)
        {
            if (logStatus == false||task.autoLogStatus == false)
            {
                return;
            }
            LogInfo($"TaskID:{task.TID}--TaskStatus: " + task.RunStatus.ToString());
        }
        /// <summary>
        /// 生成唯一的任务ID  需要考虑多线程
        /// </summary>
        /// <returns></returns>
        private int GetTid()
        {
            lock (asyncLockRoot)
            {
                tid++;
                //Safe Check
                while (true)
                {
                    if (tid == int.MaxValue)
                    {
                        tid = 0;
                    }
                    bool used = false;
                    for (int i = 0; i < tidList.Count; i++)
                    {
                        if (tid == tidList[i])
                        {
                            used = true;
                            break;
                        }
                    }
                    if (used == false)
                    {
                        break;
                    }
                    tid++;
                }
                return tid;
            }
        }
        private void RecycleTID()
        {
            lock (asyncLockRoot)
            {
                if (deleteTidList != null)
                {
                    for (int i = 0; i < deleteTidList.Count; i++)
                    {
                        tidList.Remove(deleteTidList[i]);
                    }
                    deleteTidList.Clear();
                }
            }
        }

        public void Reset()
        {
            timerTaskList.Clear();
            tempTimeTasks.Clear();
            timerFrameList.Clear();
            tempFrameTasks.Clear();
            tidList.Clear();
            tid = 1000;
            deleteTidList.Clear();
        }
        #region Time
        public double GetUTCMillSeconds()
        {
            return PETimeTools.GetUTCMillSeconds();
        }


        public DateTime GetLocalDateTime()
        {
            DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(PETimeTools.unixStartTime.AddMilliseconds(nowTime));
            return dt;
        }
        public int GetYear()
        {
            return GetLocalDateTime().Year;
        }
        public int GetMonth()
        {
            return GetLocalDateTime().Month;
        }
        public int GetDay()
        {
            return GetLocalDateTime().Day;
        }
        public int GetWeek()
        {
            return (int)(GetLocalDateTime().DayOfWeek);
        }
        public string GetLocalTimeStr()
        {
            DateTime dt = GetLocalDateTime();
            return PETimeTools.GetTimeStr(dt);
        }
        public static string GetTimeStr(DateTime dt)
        {
            dt = TimeZone.CurrentTimeZone.ToLocalTime(PETimeTools.unixStartTime.AddMilliseconds((dt-PETimeTools.unixStartTime).TotalMilliseconds));
            return PETimeTools.GetTimeStr(dt);
        }
        #endregion
        public void RefreshDestTime(PETimeTask task)
        {
            task.RefreshDest();
            RefreshMinDesTime(task);
        }
        public void RefreshMinDesTime()
        {
            for (int i = 0; i < timerTaskList.Count; i++)
            {
                if (timerTaskList[i].RunStatus == TaskStatus.InVaild)
                {
                    continue;
                }
                double destTime = timerTaskList[i].DestTime;
                if (destTime < minDestTime || minDestTime == -1)
                {
                    minDestTime = destTime;
                }
            }
        }
        public void RefreshMinDesTime(double destTime)
        {
            if (destTime < minDestTime || minDestTime == -1)
            {
                minDestTime = destTime;
            }
        }
        public void RefreshMinDesTime(PETimeTask task)
        {
            if (task.RunStatus == TaskStatus.InVaild)
            {
                return;
            }
            double destTime = task.DestTime;
            if (destTime < minDestTime || minDestTime == -1)
            {
                minDestTime = destTime;
            }
        }

        #region LogWriter
        public void PushLogFile(string message)
        {
            lock (lockLogQueue)
            {
                m_logQueue.Enqueue(message);
            }
        }

        private void LogTick()
        {
            lock (lockLogQueue)
            {
                WriteToLogFile(m_logQueue);
            }
        }
        public void ImmdietalyWriteLogFile()
        {
            lock (lockLogQueue)
            {
                WriteToLogFile(m_logQueue);
            }
        }
        public void WriteToLogFile(Queue<string> fp_logQueue)
        {
            if (fp_logQueue == null|| fp_logQueue.Count==0)
            {
                return;
            }
            if (PEDefine.MaxLogLine <= logLine)
            {
                logLine = 0;
            }
            StreamWriter sw = null;
            string[] files = null;
            if (logLine==0)
            {
                if (Directory.Exists(PEDefine.logFileDirectory)==false)
                {
                    Directory.CreateDirectory(PEDefine.logFileDirectory);
                }
                files = Directory.GetFiles(PEDefine.logFileDirectory, "*"+PEDefine.logFileFront+"_*");
                if (files.Length > 0)
                {
                    //DateTime minTime = GetLocalDateTime();
                    files = files.OrderBy((path) =>
                      {
                          return File.GetLastAccessTimeUtc(path);
                      }).ToArray();
                    if (files.Length>=PEDefine.MaxTempLogFile&&File.Exists(files[0]))
                    {
                        File.Delete(files[0]);
                        LogInfo("清空" + files[0] + " 日志文件");
                    }
                }
            }
            if (logLine==0)
            {
                string newPath = PEDefine.logFileDirectory + PEDefine.logFileFront + "_" + DateTime.Now.Year+"_"+DateTime.Now.Month+"_"+DateTime.Now.Day+"_"+DateTime.Now.Hour+"_"+DateTime.Now.Minute+"_"+DateTime.Now.Second;
                sw = File.AppendText(newPath);
                File.WriteAllText(PEDefine.logFileDirectory + PEDefine.recordLastLogPath, newPath);
            }
            else
            {
                try
                {
                    string backup = File.ReadAllText(PEDefine.logFileDirectory+PEDefine.recordLastLogPath);
                    backup = backup.Trim();
                    sw = File.AppendText(backup);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            //sw.AutoFlush=true;
            while (fp_logQueue.Count>0)
            {
                string log= fp_logQueue.Dequeue();
                sw.WriteLine(log);
                logLine++;
                if (logLine>= PEDefine.MaxLogLine)
                {
                    logLine = 0;
                    break;
                }
            }
            sw.Close();
        }
        #endregion

        public void Dispose()
        {
            LogInfo("应用程序退出");
            WriteToLogFile(m_logQueue);
        }

        ~ PETimer()
        {
            //---控制台退出后未执行
            LogInfo("应用程序退出");
            WriteToLogFile(m_logQueue);
        }
    }
}




