namespace NxLog
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class LoggingServiceText
    {
        private static LogWriter mLog = null;
        private static LogParameterInfo mLogPara = null;

        public static void setPath(string path) {
            string processName = Process.GetCurrentProcess().ProcessName;
            mLogPara = new LogParameterInfo(Path.Combine(Environment.CurrentDirectory, ""));
            if (!mLogPara.Load())
            {
                EventLog.WriteEntry("LoggingServiceText", "日志参数文件加载错误", EventLogEntryType.Error);
            }
            else if (mLogPara.EnableLogger)
            {
                mLog = new LogWriter(mLogPara, processName, path + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + "\\Logger");
            }
        }

        /// <summary>
        /// 致命错误
        /// </summary>
        public static void CriticalError(string content, string type)
        {
            if (mLogPara.LogLevel <= NxLog.LogLevel.Critical)
            {
                WriteByType(content, type + ":");
            }
        }

        /// <summary>
        /// 调试
        /// </summary>
        public static void Debug(string content, string type)
        {
            if (mLogPara.LogLevel <= NxLog.LogLevel.Debug)
            {
                WriteByType(content, type + ":");
            }
        }

        public static void Dispose()
        {
            if ((mLog != null) && mLogPara.EnableLogger)
            {
                mLog.Dispose();
            }
        }

        /// <summary>
        /// 错误
        /// </summary>
        public static void Error(string content, string type)
        {
            if (mLogPara.LogLevel <= NxLog.LogLevel.Error)
            {
                WriteByType(content, type + ":");
            }
        }

        /// <summary>
        /// 信息
        /// </summary>
        public static void Info(string content, string type)
        {
            if (mLogPara.LogLevel <= NxLog.LogLevel.Info)
            {
                WriteByType(content, type + ":");
            }
        }

        /// <summary>
        /// 警告
        /// </summary>
        public static void Warn(string content, string type)
        {
            if (mLogPara.LogLevel <= NxLog.LogLevel.Warn)
            {
                WriteByType(content, type + ":");
            }
        }

        private static void WriteByType(string content, string type)
        {
            if ((mLog != null) && mLogPara.EnableLogger)
            {
                string info = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {type} {content}";
                if (info.Length > mLogPara.MaxFlushSegmentSize)
                {
                    info = info.Substring(0, mLogPara.MaxFlushSegmentSize);
                }
                mLog.WriteLog(info);
            }
        }
    }
}

