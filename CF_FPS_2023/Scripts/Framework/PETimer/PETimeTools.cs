using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PETime
{
    public static class PETimeTools
    {
        public readonly static DateTime unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0);//计算机元年
        public static double ConvertToSeconds(double time, PETimeUnit timeUnit)
        {
            return ConvertTo(time, timeUnit, PETimeUnit.Seconds);
        }
        public static double ConvertTo(double time, PETimeUnit timeUnit, PETimeUnit targetTimeUnit)
        {
            if (timeUnit == targetTimeUnit)
            {
                return time;
            }
            double mult = (int)targetTimeUnit * 1.0d / (int)timeUnit;
            time /= mult;
            return time;
        }
        public static double CalcuateTaskDestTime(double delay, PETimeUnit peTimeUnit = PETimeUnit.MillSeconds)
        {
            return GetUTCMillSeconds() + ConvertTo(delay, peTimeUnit, PETimeUnit.MillSeconds);
        }
        public static int CalcuateTaskDestFrame(int delay)
        {
            return 0;
        }
        public static double GetUTCMillSeconds()
        {
            TimeSpan timeSpan = DateTime.UtcNow - unixStartTime;
            return timeSpan.TotalMilliseconds;
        }
        public static string GetTimeStr(DateTime dateTime)
        {
            string str = HandlerTimeStr(dateTime.Hour) + ":" + HandlerTimeStr(dateTime.Minute) + ":" + HandlerTimeStr(dateTime.Second);
            return str;
        }
        private static string HandlerTimeStr(int timer)
        {
            string str = "";
            if (timer < 10)
            {
                str = "0" + timer;
            }
            else
            {
                str = timer.ToString();
            }
            return str;
        }
    }
    public class TaskPack
    {
        public int tid;
        public Action handler;

        public TaskPack(int tid, Action handler)
        {
            this.tid = tid;
            this.handler = handler;
        }
    }
}