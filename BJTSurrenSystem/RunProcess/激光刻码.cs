using System;
using System.Windows.Forms;
using System.Drawing;
using BJTSurrenSystem.Parameters;
using Siemens;
using BJTSurrenSystem.Siemens;
using DataModel;
using MachineIntegrationServiceService;
using BJTSurrenSystem.MES;
using System.Threading;
using BJTSurrenSystem_Siemens;

namespace BJTSurrenSystem.RunProcess
{
    public class 激光刻码 : FlowClass
    {
        #region 定义 

        #endregion

        public 激光刻码(Main main)
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
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站1触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));

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

            string moduleCode = GetElectricCoreCode();

            if (string.IsNullOrEmpty(moduleCode) || moduleCode.Equals(" "))
            {
                main.outDiary($"进站{index}条码为空，条码:{moduleCode}", "警告");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                Thread.Sleep(50);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                Thread.Sleep(50);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                PullInOpenSignal("进站NG", index);
                return;
            }

            string[] temp2 = new string[main.dataGridView2.Columns.Count];
            temp2[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")] = moduleCode;
            temp2[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = ClassesJudge();
            temp2[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
            temp2[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "是否首件")] = "否";

            miSFCAttriDataEntryRequest miSFCAttriDataEntryRequest = new miSFCAttriDataEntryRequest();
            main.Invoke(new MethodInvoker(delegate
            {
                DX_verify(temp2, ref miSFCAttriDataEntryRequest);
            }));

            main.outDiary($"进站{index}条码:{moduleCode}", "信息");

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"进站{index}进入离线模式", "信息");

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"模组条码{index}"), ResourceHandler.listSystemParameters[0].ModuleCode);
                main.outDiary($"进站{index}模组码：{ResourceHandler.listSystemParameters[0].ModuleCode}", "信息");

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NP"), ResourceHandler.listSystemParameters[0].PNCode);
                main.outDiary($"进站{index}PN码：{ResourceHandler.listSystemParameters[0].PNCode}", "信息");

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}模组类型"), ResourceHandler.listSystemParameters[0].ModuleType);
                main.outDiary($"进站{index}模组类型：{ResourceHandler.listSystemParameters[0].ModuleType}", "信息");

                string buff = "#CHG;1," + ResourceHandler.listSystemParameters[0].ModuleCode + ";2," + ResourceHandler.listSystemParameters[0].PNCode + ";3," + ResourceHandler.listSystemParameters[0].ModuleType;
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"刻码内容组合{index}"), buff);
                main.outDiary($"刻码内容组合{index}：{buff}", "信息");


                temp2[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
                main.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = new string[] { moduleCode, ResourceHandler.listSystemParameters[0].PNCode, ResourceHandler.listSystemParameters[0].ModuleType, $"进站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);
                }));

                main.Invoke(new MethodInvoker(delegate
                {
                    DataGridViewClass.AddRows(main.dataGridView2, temp2, Color.White);
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
                main.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = new string[] { moduleCode, "", "", $"进站{index}", "NG", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.Red);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                PullInOpenSignal("进站NG", index);
                main.outDiary($"进站{index}失败", "报警");
                MessageBox.Show($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");

                return;
            }
            main.Invoke(new MethodInvoker(delegate
            {
                string[] temp = new string[] { moduleCode, "", "", $"进站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);
            }));

            ResponseData responseData2 = ResourceHandler.dparamParameters.MesInteraction.DX_verify(moduleCode, miSFCAttriDataEntryRequest);

            if (responseData2.code != 0)
            {
                temp2[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "NG";

                main.Invoke(new MethodInvoker(delegate
                {
                    DataGridViewClass.AddRows(main.dataGridView2, temp2, Color.Red);
                }));

                main.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = new string[] { moduleCode, "", "", "电芯校验", "NG", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.Red);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                main.outDiary("电芯校验失败", "报警");
                MessageBox.Show($"Code:{responseData2.code} \r\nMessage:{responseData2.message}\r\n原因:{responseData2.Message}\r\n解决办法:{responseData2.way}\r\n处理人员:{responseData2.personinCharge}\r\nMES审核失败", "警告");

                return;
            }

            temp2[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
            main.Invoke(new MethodInvoker(delegate
            {
                string[] temp = new string[] { moduleCode, "", "", "电芯校验", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);
            }));

            main.Invoke(new MethodInvoker(delegate
            {
                DataGridViewClass.AddRows(main.dataGridView2, temp2, Color.White);
            }));


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
                Thread.Sleep(50);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}NG"), true);
                Thread.Sleep(50);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}NG"), true);
                PullOutOpenSignal("出站NG", index);
                return;
            }

            main.outDiary($"出站{index}条码:{moduleCode}", "信息");

            dataCollectForSfcEx dataCollectForSfcEx = new dataCollectForSfcEx();
            dataCollectForSfcEx.SfcDcExRequest = new sfcDcExRequest();
            dataCollectForSfcEx.SfcDcExRequest.parametricDataArray = new MachineIntegrationServiceService.machineIntegrationParametricData[2000];
            bool state = true;
            string[] temp = new string[main.dataGridView2.Columns.Count];

            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")] = moduleCode;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = ClassesJudge();
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "是否首件")] = "否";

            main.Invoke(new MethodInvoker(delegate
            {
                PullOutDataGain(temp, ref state, ref dataCollectForSfcEx);
            }));

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"出站{index}进入离线模式", "信息");

                main.Invoke(new MethodInvoker(delegate
                {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"出站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.White);
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView2, temp, Color.White);
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
                    main.Invoke(new MethodInvoker(delegate
                    {
                        string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"出站{index}", "NG", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                        DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.Red);

                        temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = "NG";
                        DataGridViewClass.AddRows(main.dataGridView2, temp, Color.Red);
                    }));

                    ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}NG"), true);
                    PullInOpenSignal("出站NG", index);
                    main.outDiary($"出站{index}失败", "报警");
                    MessageBox.Show($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");
                    return;
                }
                main.Invoke(new MethodInvoker(delegate
                {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"出站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);

                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView2, temp, Color.White);
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
            return;
        }

        /// <summary>
        /// 首件复位
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void InitialWorkpiecePullOutResset(string RegAddress)
        {
            return;
        }

    }
}
