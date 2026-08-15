namespace NxLog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;

    internal class LogWriter
    {
        private StreamWriter mWriter = null;
        private FileStream mFileStream = null;
        private string mLogPath = string.Empty;
        private object mLocker = new object();
        private Queue<string> mQueLog = new Queue<string>(0x3e8);
        private Thread mThread = null;
        private EventWaitHandle mExitFlushThread = new EventWaitHandle(false, EventResetMode.ManualReset);
        private long mMaxLogLength = 0x5f5e100L;
        private int mMaxLogFileCount = 5;
        private int mFlushInterval = 100;
        private int mSuspendInterval = 0xea60;
        private string mLogPrefixName = string.Empty;
        private int mMaxFlushSegmentSize = 0x7fff;
        private bool mExceptionSuspend = false;

        public LogWriter(LogParameterInfo para, string categoryName, string logPath)
        {
            this.mLogPath = logPath;
            this.mLogPrefixName = categoryName;
            if (!Directory.Exists(this.mLogPath))
            {
                Directory.CreateDirectory(this.mLogPath);
            }
            this.mMaxLogLength = para.LogFileMaxByteLen;
            this.mFlushInterval = para.FlushInterval;
            this.mMaxLogFileCount = para.MaxLogFileCount;
            this.mMaxFlushSegmentSize = para.MaxFlushSegmentSize;
            this.CreateLogWriter();
            this.mThread = new Thread(new ThreadStart(this.FlushLog));
            this.mThread.IsBackground = true;
            this.mThread.Start();
        }
        DateTime createTime = DateTime.Now;
        private void CreateLogWriter()
        {
            createTime = DateTime.Now;
            if (this.mWriter != null)
            {
                this.mWriter.Close();
                this.mWriter.Dispose();
            }
            if (this.mFileStream != null)
            {
                this.mFileStream.Close();
                this.mFileStream.Dispose();
            }
            if (!Directory.Exists(this.mLogPath))
            {
                Directory.CreateDirectory(this.mLogPath);
            }
            this.DeleteOldFile(this.mLogPath);
            string path = Path.Combine(this.mLogPath, $"{this.mLogPrefixName}_{DateTime.Now.ToString("yyyyMMdd")}.Log");
            this.mFileStream = new FileStream(path, FileMode.Append);
            this.mWriter = new StreamWriter(this.mFileStream, Encoding.UTF8);
            this.mWriter.AutoFlush = false;
        }

        private void DeleteOldFile(string dir)
        {
            try
            {
                string[] files = Directory.GetFiles(dir);
                List<FileInfo> list = new List<FileInfo>();
                foreach (string str in files)
                {
                    FileInfo item = new FileInfo(str);
                    if (item.Name.StartsWith(this.mLogPrefixName))
                    {
                        list.Add(item);
                    }
                }
                if (list.Count >= this.mMaxLogFileCount)
                {
                    string path = string.Empty;
                    DateTime maxValue = DateTime.MaxValue;
                    foreach (FileInfo info2 in list)
                    {
                        if (info2.CreationTime < maxValue)
                        {
                            maxValue = info2.CreationTime;
                            path = info2.FullName;
                        }
                    }
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            try
            {
                this.mExitFlushThread.Set();
                if ((this.mThread != null) && this.mThread.IsAlive)
                {
                    this.mThread.Abort();
                }
                if (this.mWriter != null)
                {
                    this.mWriter.Close();
                    this.mWriter.Dispose();
                }
                if (this.mFileStream != null)
                {
                    this.mFileStream.Close();
                    this.mFileStream.Dispose();
                }
            }
            catch
            {
            }
        }

        private void FlushLog()
        {
            while (true)
            {
                if (this.mExitFlushThread.WaitOne(0, false))
                {
                    this.mExitFlushThread.Reset();
                    return;
                }
                string[] array = null;
                lock (this.mLocker)
                {
                    int count = this.mQueLog.Count;
                    if (count > 0)
                    {
                        array = new string[count];
                        this.mQueLog.CopyTo(array, 0);
                        this.mQueLog.Clear();
                    }
                }
                this.FlushWriterStream(this.mWriter, array);
                if (((this.mFileStream != null) && (this.mFileStream.Length > this.mMaxLogLength)) || DateTime.Now.Day != createTime.Day)
                {
                    this.CreateLogWriter();
                }
                if (this.mExceptionSuspend)
                {
                    Thread.Sleep(this.mSuspendInterval);
                    this.mExceptionSuspend = false;
                }
                else
                {
                    Thread.Sleep(this.mFlushInterval);
                }
            }
        }

        private void FlushWriterStream(StreamWriter writer, string[] infos)
        {
            if ((writer != null) && (infos != null))
            {
                int index = 0;
                int num2 = 0;
                try
                {
                    int length = infos.Length;
                    for (index = 0; index < length; index++)
                    {
                        writer.WriteLine(infos[index]);
                        num2 += infos[index].Length;
                        if (num2 >= this.mMaxFlushSegmentSize)
                        {
                            writer.Flush();
                            num2 = 0;
                        }
                    }
                    if (num2 > 0)
                    {
                        writer.Flush();
                        num2 = 0;
                    }
                }
                catch (IOException)
                {
                    try
                    {
                        DriveInfo[] drives = DriveInfo.GetDrives();
                        DirectoryInfo info = new DirectoryInfo(this.mLogPath);
                        foreach (DriveInfo info2 in drives)
                        {
                            if (info2.RootDirectory.Name == info.Root.Name)
                            {
                                if (info2.TotalFreeSpace <= this.mMaxLogLength)
                                {
                                    this.mExceptionSuspend = true;
                                }
                                return;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        this.mExceptionSuspend = true;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void WriteLog(string info)
        {
            if (!this.mExceptionSuspend)
            {
                lock (this.mLocker)
                {
                    this.mQueLog.Enqueue(info);
                }
            }
        }
    }
}

