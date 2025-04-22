using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PETime
{
    public enum PETimeUnit
    {
        MillSeconds = 1,
        Seconds = 1000,
        Minutes = 60000,
        Hours = 3600000,
        Days = 3600000 * 24
    }
    public class PEDefine
    {
        public readonly static string logFileFront = "log_file";
        public readonly static string logFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Log/");
        public readonly static int MaxLogLine = 100;
        public readonly static int MaxTempLogFile = 5;
        public readonly static string recordLastLogPath = "backup";
    }
}
