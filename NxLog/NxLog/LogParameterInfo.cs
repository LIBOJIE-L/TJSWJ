namespace NxLog
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Linq;
    using System.Xml;

    internal class LogParameterInfo
    {
        private bool mEnableLogger = false;
        private NxLog.LogLevel mLogLevel = NxLog.LogLevel.Debug;
        private int mFlushInterval = 100;
        private int mLogFileMaxByteLen = 0x5f5e100;
        private int mMaxLogFileCount = 5;
        private string mPathName = string.Empty;

        public LogParameterInfo(string path)
        {
            this.mPathName = path;
        }

        private NxLog.LogLevel ConvertLogLevel(int value)
        {
            if (value > 4)
            {
                return NxLog.LogLevel.Critical;
            }
            if (value < 0)
            {
                return NxLog.LogLevel.Debug;
            }
            return (NxLog.LogLevel) value;
        }

        public bool Load()
        {
            string path = Path.Combine(this.mPathName, this.FileName);
            if (File.Exists(path))
            {
                XDocument doc = XDocument.Load(path);
                return this.ReadXml(doc);
            }
            EventLog.WriteEntry("LoggingServiceText", "日志参数文件不存在", EventLogEntryType.Error);
            return false;
        }

        private bool ReadXml(XDocument doc)
        {
            Debug.Assert(doc != null);
            Debug.Assert(doc.Root != null);
            try
            {
                XElement root = doc.Root;
                this.ReadXml(root);
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LoggingServiceText", exception.ToString(), EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        private void ReadXml(XElement node)
        {
            XElement element = node.Element("LoggerText");
            if (element != null)
            {
                this.mEnableLogger = bool.Parse(element.Attribute("EnableLogger").Value);
                this.mLogLevel = this.ConvertLogLevel(int.Parse(element.Attribute("LogLevel").Value));
                this.mLogFileMaxByteLen = int.Parse(element.Attribute("LogFileMaxByteLen").Value);
                this.mMaxLogFileCount = int.Parse(element.Attribute("MaxLogFileCount").Value);
                this.mFlushInterval = int.Parse(element.Attribute("FlushInterval").Value);
            }
        }

        private string FileName =>
            "LoggingServiceTextParameter.config";

        public bool EnableLogger =>
            this.mEnableLogger;

        public NxLog.LogLevel LogLevel =>
            this.mLogLevel;

        public int FlushInterval =>
            this.mFlushInterval;

        public int LogFileMaxByteLen =>
            this.mLogFileMaxByteLen;

        public int MaxLogFileCount =>
            this.mMaxLogFileCount;

        public int MaxFlushSegmentSize =>
            0x7fff;
    }
}

