using DataModel;
using HJMSurrenSystem;
using HJMSurrenSystem.Parameters;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using language;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Siemens
{
    public class JPIO_OPC
    {
        public string RegAddress;
        public delegate void ThreadEvent(bool status);
        private string heartBeatAddress;
        public static ThreadEvent heartBeatCallBack;
        private bool heartBeatFlag;
        public static ThreadEvent IoCheckCallBack;
        private List<OPC_Event> oPC_EventList;
        private volatile bool PLC_Connect_Status;
        private SiemensS7Net siemensTcpNet;
        private Thread tHeartBeatThread;
        public OPC_Event opcEvent;
        private Thread tThread;
        public Main main;
        private readonly object connectionLock = new object();
        private const int ReconnectIntervalMilliseconds = 3000;
        private volatile bool isClosing;
        private int reconnecting;
        private int heartbeatAlarmActive;

        public JPIO_OPC(string station,string heartBeatAddress, Main main)
        {

            XmlHelper xmlHelper = new XmlHelper("xml/" + station + "/PLCParameterSetting.xml");
            bool languageBool = xmlHelper.Read(ref ResourceHandler.listPlcParameters);
            if (!languageBool)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "PLC参数文件读取异常"), "异常");
            }

            ResourceHandler.dparamParameters.set_PlcUI.PLC_IP.Text = ResourceHandler.listPlcParameters[0].IP;

            ResourceHandler.dparamParameters.set_PlcUI.PLC_model.Text = ResourceHandler.listPlcParameters[0].PLCmodel;


            if (ResourceHandler.listPlcParameters[0].PLCmodel.Equals("S1200"))
                this.siemensTcpNet = new SiemensS7Net(SiemensPLCS.S1200);
            else if (ResourceHandler.listPlcParameters[0].PLCmodel.Equals("S300"))
                this.siemensTcpNet = new SiemensS7Net(SiemensPLCS.S300);
            else if (ResourceHandler.listPlcParameters[0].PLCmodel.Equals("S400"))
                this.siemensTcpNet = new SiemensS7Net(SiemensPLCS.S400);
            else if (ResourceHandler.listPlcParameters[0].PLCmodel.Equals("S200Smart"))
                this.siemensTcpNet = new SiemensS7Net(SiemensPLCS.S200Smart);
            else if (ResourceHandler.listPlcParameters[0].PLCmodel.Equals("S200"))
                this.siemensTcpNet = new SiemensS7Net(SiemensPLCS.S1500);
            else
                this.siemensTcpNet = new SiemensS7Net(SiemensPLCS.S1200);

            this.main = main;
            this.siemensTcpNet.IpAddress = ResourceHandler.listPlcParameters[0].IP;
            this.PLC_Connect_Status = false;
            this.tThread = new Thread(new ThreadStart(this.OPC_CheckEventAsync));
            this.tHeartBeatThread = new Thread(new ThreadStart(this.HeartBeatEventAsync));
            this.tThread.IsBackground = true;
            this.tHeartBeatThread.IsBackground = true;
            this.oPC_EventList = new List<OPC_Event>();
            this.heartBeatAddress = heartBeatAddress;
            this.heartBeatFlag = false;
            this.isClosing = false;


        }
        private async Task CheckIo()
        {
            List<Task> taskList = new List<Task>();
            int i = 0;
            while (i < this.oPC_EventList.Count)
            {
                isRunAll = false;//确保了大循环不会多次发送
                bool result = false;
                opcEvent = oPC_EventList[i];
                RegAddress = opcEvent.RegAddress;
                if (!this.PLC_Connect_Status)
                {
                    break;
                }
                try
                {
                    await Task.Run(() =>
                    {
                        OperateResult<bool> content = siemensTcpNet.ReadBool(this.RegAddress);
                        if (!content.IsSuccess)
                        {
                            result = false;

                            main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读取PLC地址失败")+ RegAddress, "错误");
                        }
                        result = content.Content;
                    });
                }
                catch (Exception ex)
                {
                    HandleConnectionLost("OPC读取异常：" + ex.Message);
                }
                try
                {
                    if (result && !opcEvent.isRun)
                    {
                        opcEvent.isRun = true;

                        taskList.Add(Task.Run(() =>
                        {
                            opcEvent.value_Set_Event(RegAddress);
                        }));

                    }
                    else if (!result && opcEvent.isRun)
                    {
                        opcEvent.isRun = false;
                        taskList.Add(Task.Run(() =>
                        {
                            this.opcEvent.value_Reset_Event(this.RegAddress);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "OPC Event处理异常") + ex.Message, "致命错误");
                }
                Task t = Task.WhenAll(taskList.ToArray());
                await t;
                await Task.Delay(10);
                i++;
            }
            isRunAll = true;
        }


        /// <summary>
        /// isRun确保大循环正常
        /// </summary>
        volatile bool isRunAll = true;
        private void OPC_CheckEventAsync()
        {
            while (!isClosing)
            {
                if (PLC_Connect_Status && isRunAll)
                {
                    Task task = CheckIo();
                    Task[] tasks = new Task[] { task };
                    Task.WhenAll(tasks);
                    GC.Collect();
                }
                Thread.Sleep(10);
            }
        }

        private void HeartBeatEventAsync()
        {
            while (!isClosing)
            {
                if (!this.PLC_Connect_Status)
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (this.heartBeatAddress != "")
                {
                    try
                    {
                        bool nextHeartBeatFlag = !this.heartBeatFlag;
                        OperateResult writeResult = this.siemensTcpNet.Write(this.heartBeatAddress, nextHeartBeatFlag);
                        if (writeResult == null || !writeResult.IsSuccess)
                        {
                            string failureMessage = writeResult == null ? "PLC无返回结果" : writeResult.Message;
                            HandleConnectionLost("PLC心跳写入失败：" + failureMessage);
                            continue;
                        }

                        this.heartBeatFlag = nextHeartBeatFlag;
                        if (this.heartBeatFlag)
                        {
                            ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox1.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
                        }
                        else
                        {
                            ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox1.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                        }
                        heartBeatCallBack?.Invoke(this.heartBeatFlag);
                    }
                    catch (Exception ex)
                    {
                        HandleConnectionLost("PLC心跳发送异常：" + ex.Message);
                    }
                }
                Thread.Sleep(0x3e8);
            }
        }

        private void HandleConnectionLost(string reason)
        {
            if (isClosing)
            {
                return;
            }

            PLC_Connect_Status = false;
            if (Interlocked.Exchange(ref heartbeatAlarmActive, 1) == 0)
            {
                main.outDiary("PLC心跳断开：" + reason, "错误");
                UpdateConnectionState(false);
                ShowHeartbeatAlarm();
            }

            StartReconnectLoop();
        }

        private void ShowHeartbeatAlarm()
        {
            try
            {
                MethodInvoker showAlarm = delegate
                {
                    MessageBox.Show(main, "PLC心跳已断开，程序正在自动重连PLC，请检查PLC电源及网络连接。", "PLC心跳报警", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                if (main.InvokeRequired)
                {
                    main.BeginInvoke(showAlarm);
                }
                else
                {
                    showAlarm();
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void StartReconnectLoop()
        {
            if (isClosing || PLC_Connect_Status || Interlocked.CompareExchange(ref reconnecting, 1, 0) != 0)
            {
                return;
            }

            Task.Run(() => ReconnectLoop());
        }

        private void ReconnectLoop()
        {
            try
            {
                while (!isClosing && !PLC_Connect_Status)
                {
                    while (!isClosing && !isRunAll)
                    {
                        Thread.Sleep(50);
                    }

                    if (isClosing)
                    {
                        break;
                    }

                    bool reconnectSuccess = false;
                    string failureMessage = "未知错误";
                    try
                    {
                        lock (connectionLock)
                        {
                            if (isClosing || PLC_Connect_Status)
                            {
                                break;
                            }

                            this.siemensTcpNet.ConnectClose();
                            connect = this.siemensTcpNet.ConnectServer();
                            reconnectSuccess = connect != null && connect.IsSuccess;
                            if (!reconnectSuccess && connect != null && !string.IsNullOrEmpty(connect.Message))
                            {
                                failureMessage = connect.Message;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        failureMessage = ex.Message;
                    }

                    if (reconnectSuccess)
                    {
                        foreach (OPC_Event item in oPC_EventList)
                        {
                            item.isRun = false;
                        }
                        this.heartBeatFlag = false;
                        PLC_Connect_Status = true;
                        Interlocked.Exchange(ref heartbeatAlarmActive, 0);
                        UpdateConnectionState(true);
                        main.outDiary("PLC自动重连成功", "信息");
                        break;
                    }

                    main.outDiary("PLC自动重连失败：" + failureMessage + "，3秒后继续重试", "错误");
                    for (int i = 0; i < ReconnectIntervalMilliseconds / 100 && !isClosing; i++)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref reconnecting, 0);
                if (!isClosing && !PLC_Connect_Status)
                {
                    StartReconnectLoop();
                }
            }
        }

        private void UpdateConnectionState(bool connectedState)
        {
            try
            {
                MethodInvoker updateState = delegate
                {
                    main.plcConnectState.Text = connectedState ? "连接成功" : "连接断开";
                    main.plcConnectState.ForeColor = connectedState ? Color.Green : Color.Red;
                };

                if (main.InvokeRequired)
                {
                    main.BeginInvoke(updateState);
                }
                else
                {
                    updateState();
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void OPC_AddEventList(OPC_Event oOPC_Event)
        {
            this.oPC_EventList.Add(oOPC_Event);
        }

        public void OPC_CloseServer()
        {
            try
            {
                isClosing = true;
                this.PLC_Connect_Status = false;
                lock (connectionLock)
                {
                    this.siemensTcpNet.ConnectClose();
                }
                if (this.tThread != null && this.tThread.IsAlive && Thread.CurrentThread != this.tThread)
                {
                    this.tThread.Join(1000);
                }
                if (this.tHeartBeatThread != null && this.tHeartBeatThread.IsAlive && Thread.CurrentThread != this.tHeartBeatThread)
                {
                    this.tHeartBeatThread.Join(1000);
                }
            }
            catch (Exception exception)
            {
                throw new Exception("关闭PLC错误:" + exception.Message);
            }
        }
        public OperateResult connect;
        public bool OPC_ConnectServer()
        {
            try
            {
                siemensTcpNet.ConnectTimeOut = 2000;
                lock (connectionLock)
                {
                    siemensTcpNet.ConnectClose();//连接PLC
                    connect = siemensTcpNet.ConnectServer();
                }
                if (connect.IsSuccess == true)
                {
                    PLC_Connect_Status = true;
                }
                else
                {
                    PLC_Connect_Status = false;
                }
            }
            catch (Exception exception)
            {
                PLC_Connect_Status = false;
                main.outDiary("PLC初次连接异常：" + exception.Message, "错误");
            }

            if ((tThread.ThreadState & System.Threading.ThreadState.Unstarted) == System.Threading.ThreadState.Unstarted)
            {
                tThread.Start();
            }
            if ((tHeartBeatThread.ThreadState & System.Threading.ThreadState.Unstarted) == System.Threading.ThreadState.Unstarted)
            {
                tHeartBeatThread.Start();
            }
            if (!PLC_Connect_Status)
            {
                StartReconnectLoop();
            }
            return PLC_Connect_Status;
        }

        public bool PLC_Read_bool(string address)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    return this.siemensTcpNet.ReadBool(address).Content;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return false;
        }

        public byte PLC_Read_byte(string address)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    return this.siemensTcpNet.ReadByte(address).Content;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return 0;
        }

        public float PLC_Read_float(string address)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    return this.siemensTcpNet.ReadFloat(address).Content;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return 0f;
        }

        public int PLC_Read_int(string address)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    return this.siemensTcpNet.ReadInt32(address).Content;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return 0;
        }

        public uint PLC_Read_uint(string address)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    return this.siemensTcpNet.ReadUInt16(address).Content;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return 0;
        }

        public short PLC_Read_short(string address)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    return this.siemensTcpNet.ReadInt16(address).Content;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return 0;
        }

        public ushort[] PLC_Read_short(string address, ushort length)
        {
            ushort[] numArray = new ushort[length];
            try
            {
                if (this.PLC_Connect_Status)
                {
                    byte[] content = this.siemensTcpNet.Read(address, (ushort)(length * 2)).Content;
                    for (ushort i = 0; i < length; i = (ushort)(i + 1))
                    {
                        numArray[i] = (ushort)((content[i * 2] << 8) | content[(i * 2) + 1]);
                    }
                    return numArray;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return numArray;
        }

        public string PLC_Read_String(string address)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    var head = this.siemensTcpNet.ReadUInt16(address).Content;
                    byte recID = (byte)(head >> 8 & 0xff);
                    byte len = (byte)(head & 0xff);

                    string value = this.siemensTcpNet.ReadString(address, (ushort)(len + 2)).Content.Substring(2, len);
                    value = value == null ? "" : value;
                    return value == "" ? " " : value;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return "";
        }

        public void PLC_Write_bool(string adderss, bool value)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    this.siemensTcpNet.Write(adderss, value);
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
        }

        public void PLC_Write_byte(string adderss, byte value)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    this.siemensTcpNet.Write(adderss, value);
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
        }

        public void PLC_Write_bytes(string adderss, byte[] value)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    this.siemensTcpNet.Write(adderss, value);
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
        }

        public void PLC_Write_float(string adderss, float value)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    this.siemensTcpNet.Write(adderss, value);
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
        }

        public void PLC_Write_int(string adderss, int value)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    this.siemensTcpNet.Write(adderss, value);
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
        }

        public void PLC_Write_string(string adderss, string value)
        {
            PLC_TryWrite_string(adderss, value);
        }

        public bool PLC_TryWrite_string(string adderss, string value)
        {
            try
            {
                if (!this.PLC_Connect_Status)
                {
                    return false;
                }

                byte[] head = new byte[2];
                head[0] = (byte)97;
                head[1] = (byte)value.Length;
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                byte[] sendData = new byte[value.Length + 2];
                head.CopyTo(sendData, 0);
                data.CopyTo(sendData, 2);

                OperateResult writeResult = this.siemensTcpNet.Write(adderss, sendData);
                if (writeResult == null || !writeResult.IsSuccess)
                {
                    string failureMessage = writeResult == null ? "PLC无返回结果" : writeResult.Message;
                    main.outDiary(
                        Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" +
                        main.user_Name.Text + "     " +
                        Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "写PLC错误") +
                        failureMessage,
                        "错误");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "写PLC错误") + ex.Message, "错误");
                return false;
            }
        }

        public void PLC_Write_ushort(string adderss, ushort value)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    this.siemensTcpNet.Write(adderss, value);
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
        }

        public byte[] PLC_Read_all(string[] address, ushort[] lengths)
        {
            try
            {
                if (this.PLC_Connect_Status)
                {
                    return this.siemensTcpNet.Read(address, lengths).Content;
                }
            }
            catch (Exception ex)
            {
                main.outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + main.user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "读PLC错误") + ex.Message, "错误");
            }
            return null;
        }
    }
}
