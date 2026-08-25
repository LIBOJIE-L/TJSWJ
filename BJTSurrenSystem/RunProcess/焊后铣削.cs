using DataModel;
using DataModel.CSV;
using BJTSurrenSystem.Config_UI.FtpConfig;
using BJTSurrenSystem.MES;
using BJTSurrenSystem.Parameters;
using BJTSurrenSystem.Siemens;
using BJTSurrenSystem_Siemens;
using Siemens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BJTSurrenSystem.RunProcess
{
    public class 焊后铣削 : FlowClass
    {

        public 焊后铣削(Main main)
        {
            this.main = main;

            ResourceHandler.dparamParameters.set_PlcUI = new set_PLC(main);
            ResourceHandler.dparamParameters.pLCSignalL_UI = new PLCSignalL_UI();

            ResourceHandler.dparamParameters.MesInteraction = new MesInteraction(main);

            plcParameterSet();

        }

        public void plcParameterSet()
        {
            ResourceHandler.dparamParameters.siemensS7Net = new JPIO_OPC(main.workstationName.Text, DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "心跳"), main);
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "MES屏蔽"), new OPC_Event.Value_Set_Event(MesShielding), new OPC_Event.Value_Reset_Event(MesShieldingReset)));

            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站1触发"), new OPC_Event.Value_Set_Event(PullInTrigger), new OPC_Event.Value_Reset_Event(PullInReset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站2触发"), new OPC_Event.Value_Set_Event(PullInTrigger), new OPC_Event.Value_Reset_Event(PullInReset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站3触发"), new OPC_Event.Value_Set_Event(PullInTrigger), new OPC_Event.Value_Reset_Event(PullInReset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站4触发"), new OPC_Event.Value_Set_Event(PullInTrigger), new OPC_Event.Value_Reset_Event(PullInReset)));

            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站1触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站2触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站3触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站4触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "首件触发"), new OPC_Event.Value_Set_Event(InitialWorkpiecePullOut), new OPC_Event.Value_Reset_Event(InitialWorkpiecePullOutResset)));

            if (ResourceHandler.dparamParameters.siemensS7Net.OPC_ConnectServer())
            {
                main.outDiary("PLC连接成功", "信息");
                main.plcConnectState.Text = "连接成功";
                main.plcConnectState.ForeColor = Color.Green;
            }
            else
            {
                main.outDiary("PLC连接失败", "错误");
                main.plcConnectState.Text = "连接失败";
                main.plcConnectState.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// 进站触发
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void PullInTrigger(string RegAddress)
        {
            int index = 0;
            if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站1触发")))
                index = 1;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站2触发")))
                index = 2;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站3触发")))
                index = 3;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站4触发")))
                index = 4;
            PullInOpenSignal("进站信号", index);
            main.outDiary($"接受到进站{index}触发信号", "信息");

            string moduleCode = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"模组条码{index}"));

            if (string.IsNullOrEmpty(moduleCode) || moduleCode.Equals(" "))
            {
                main.outDiary($"进站{index}条码为空，条码:{moduleCode}", "警告");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                PullInOpenSignal("进站NG", index);
                return;
            }

            main.outDiary($"进站{index}条码:{moduleCode}", "信息");

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"进站{index}进入离线模式", "信息");

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NP"), ResourceHandler.listSystemParameters[0].PNCode);
                main.outDiary($"进站{index}PN码：{ResourceHandler.listSystemParameters[0].PNCode}", "信息");

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}模组类型"), ResourceHandler.listSystemParameters[0].ModuleType);
                main.outDiary($"进站{index}模组类型：{ResourceHandler.listSystemParameters[0].ModuleType}", "信息");
                
                main.outDiary($"进站{index}铣削数据：", "信息");

                main.Invoke(new MethodInvoker(delegate {
                    string[] temp = new string[] { moduleCode, ResourceHandler.listSystemParameters[0].PNCode, ResourceHandler.listSystemParameters[0].ModuleType, $"进站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\进站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView1, moduleCode);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}OK"), true);
                PullInOpenSignal("进站OK", index);
                main.outDiary($"进站{index}OK", "信息");
                return;
            }

            main.outDiary($"进站{index}进入在线模式", "信息");
            ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.PullIn(moduleCode);
            if (responseData.code != 0)
            {
                main.Invoke(new MethodInvoker(delegate {
                    string[] temp = new string[] { moduleCode, "", "", $"进站{index}", "NG", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.Red);

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\进站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView1, moduleCode);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                PullInOpenSignal("进站NG", index);
                main.outDiary($"进站{index}失败", "报警");
                MessageBox.Show($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");

                return;
            }
            main.Invoke(new MethodInvoker(delegate {
                string[] temp = new string[] { moduleCode, "", "", $"进站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);

                DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\进站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView1, moduleCode);
            }));

            // 这里写铣削数据给PLC（是从mes那边获取的铣削数据给到PLC）
            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NP"), responseData.pnCode);
            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}模组类型"), responseData.type);

            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}OK"), true);
            PullInOpenSignal("进站OK", index);
            main.outDiary($"进站{index}成功", "信号");

        }

        /// <summary>
        /// 进站复位
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void PullInReset(string RegAddress)
        {
            if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站1触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站1OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站1NG"), false);
                PullInOpenSignal("进站复位", 1);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站2触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站2OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站2NG"), false);
                PullInOpenSignal("进站复位", 2);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站3触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站3OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站3NG"), false);
                PullInOpenSignal("进站复位", 3);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站4触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站4OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站4NG"), false);
                PullInOpenSignal("进站复位", 4);
                return;
            }
        }

        /// <summary>
        /// 出站触发
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void PullOutTrigger(string RegAddress)
        {
            int index = 0;
            if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站1触发")))
                index = 1;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站2触发")))
                index = 2;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站3触发")))
                index = 3;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站4触发")))
                index = 4;
            PullOutOpenSignal("出站信号", index);
            main.outDiary($"接受到出站{index}触发信号", "信息");

            string moduleCode = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"模组条码{index}"));

            if (string.IsNullOrEmpty(moduleCode) || moduleCode.Equals(" "))
            {
                main.outDiary($"出站{index}条码为空，条码:{moduleCode}", "警告");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}NG"), true);
                PullOutOpenSignal("出站NG", index);
                return;
            }

            main.outDiary($"出站{index}条码:{moduleCode}", "信息");

            MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx = new MachineIntegrationServiceService.dataCollectForSfcEx();
            dataCollectForSfcEx.SfcDcExRequest = new MachineIntegrationServiceService.sfcDcExRequest();
            List<MachineIntegrationServiceService.machineIntegrationParametricData> parametricDataList = new List<MachineIntegrationServiceService.machineIntegrationParametricData>();

            bool state = true;
            string[] temp = new string[main.dataGridView2.Columns.Count];

            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")] = moduleCode;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = ClassesJudge();
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "是否首件")] = "否";

            main.Invoke(new MethodInvoker(delegate {
                PullOutDatasGain(temp, ref state, parametricDataList);
            }));
            dataCollectForSfcEx.SfcDcExRequest.parametricDataArray = parametricDataList.ToArray();

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"出站{index}进入离线模式", "信息");

                main.Invoke(new MethodInvoker(delegate {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"出站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.White);
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView2, temp, Color.White);

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView2, moduleCode);

                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}OK"), true);
                PullOutOpenSignal("出站Ok", index);
                main.outDiary($"出站{index}OK", "信息");
                return;
            }

            main.outDiary($"出站{index}进入在线模式", "信息");

            if (state)
            {
                ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.PullOut(moduleCode, dataCollectForSfcEx);
                if (responseData.code != 0)
                {
                    main.Invoke(new MethodInvoker(delegate {
                        string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"出站{index}", "NG", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                        DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.Red);

                        temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = "NG";
                        DataGridViewClass.AddRows(main.dataGridView2, temp, Color.Red);

                        DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView2, moduleCode);
                    }));

                    ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}NG"), true);
                    PullInOpenSignal("出站NG", index);
                    main.outDiary($"出站{index}失败", "报警");
                    MessageBox.Show($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");
                    return;
                }
                main.Invoke(new MethodInvoker(delegate {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"出站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);

                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView2, temp, Color.White);

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView2, moduleCode);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}OK"), true);
                PullInOpenSignal("出站OK", index);
                main.outDiary($"出站{index}成功", "信号");
                return;
            }
            else
            {
                MessageBox.Show("出站失败，参数错误或超上下限", "错误");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}NG"), true);
                PullOutOpenSignal("出站NG", index);
                return;
            }
        }

        /// <summary>
        /// 出站复位
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void PullOutResset(string RegAddress)
        {
            if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站1触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站1OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站1NG"), false);
                PullOutOpenSignal("出站复位", 1);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站2触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站2OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站2NG"), false);
                PullOutOpenSignal("出站复位", 2);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站3触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站3OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站3NG"), false);
                PullOutOpenSignal("出站复位", 3);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站4触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站4OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站4NG"), false);
                PullOutOpenSignal("出站复位", 4);
                return;
            }
        }

        /// <summary>
        /// 首件出站
        /// </summary>
        public async void InitialWorkpiecePullOut(string RegAddress)
        {
            PullOutOpenSignal("首件触发", 1);
            main.outDiary($"接受到首件触发信号", "信息");

            string moduleCode = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"首件模组条码"));

            if (string.IsNullOrEmpty(moduleCode) || moduleCode.Equals(" "))
            {
                main.outDiary($"首件条码为空，条码:{moduleCode}", "警告");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"首件NG"), true);
                PullOutOpenSignal("首件NG", 1);
                return;
            }

            main.outDiary($"首件条码:{moduleCode}", "信息");

            dataCollectForResourceFAI dataCollectForResourceFAI = new dataCollectForResourceFAI();
            dataCollectForResourceFAI.resourceRequest = new dataCollectForResourceFAIRequest();
            List<machineIntegrationParametricData> parametricDataList = new List<machineIntegrationParametricData>();

            bool state = true;
            string[] temp = new string[main.dataGridView2.Columns.Count];
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")] = moduleCode;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = ClassesJudge();
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "设备标识")] = ResourceHandler.dparamParameters.offLineUI.textBox1.Text;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "是否首件")] = "是";

            main.Invoke(new MethodInvoker(delegate {
                InitialWorkpieceDatasGain(temp, ref state, parametricDataList);
            }));
            dataCollectForResourceFAI.resourceRequest.parametricDataArray = parametricDataList.ToArray();

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"首件进入离线模式", "信息");

                main.Invoke(new MethodInvoker(delegate {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"首件", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.White);
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView2, temp, Color.White);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"首件OK"), true);
                PullOutOpenSignal("首件OK", 1);
                return;
            }

            main.outDiary($"首件进入在线模式", "信息");

            if (state)
            {
                ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.InitialWorkpiece(moduleCode, dataCollectForResourceFAI);
                if (responseData.code != 0)
                {
                    main.Invoke(new MethodInvoker(delegate {
                        string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"首件", "NG", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                        DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.Red);

                        temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = "NG";
                        DataGridViewClass.AddRows(main.dataGridView2, temp, Color.Red);
                    }));

                    ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"首件NG"), true);
                    PullInOpenSignal("首件NG", 1);
                    main.outDiary($"首件失败", "报警");
                    MessageBox.Show($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");
                    return;
                }
                main.Invoke(new MethodInvoker(delegate {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"首件", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);

                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView2, temp, Color.White);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"首件OK"), true);
                PullInOpenSignal("首件OK", 1);
                main.outDiary($"首件成功", "信号");
                return;
            }
            else
            {
                MessageBox.Show("首件失败，参数错误或超上下限", "错误");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"首件NG"), true);
                PullOutOpenSignal("首件NG", 1);
                return;
            }
        }

        /// <summary>
        /// 首件复位
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void InitialWorkpiecePullOutResset(string RegAddress)
        {
            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "首件OK"), false);
            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "首件NG"), false);
            PullOutOpenSignal("首件复位", 1);
        }

    }
}
