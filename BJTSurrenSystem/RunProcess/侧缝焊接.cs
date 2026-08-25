using System;
using System.Windows.Forms;
using System.Drawing;
using BJTSurrenSystem.Parameters;
using Siemens;
using DataModel;
using BJTSurrenSystem.MES;
using System.Web.Services.Description;
using System.Reflection;
using System.Threading;
using BJTSurrenSystem_Siemens;
using BJTSurrenSystem.Siemens;

namespace BJTSurrenSystem.RunProcess
{
    public class 侧缝焊接 : FlowClass
    {
        #region 定义 
       
        #endregion

        public 侧缝焊接(Main main)
        {
            this.main = main;

            ResourceHandler.dparamParameters.set_PlcUI = new set_PLC(main);
            ResourceHandler.dparamParameters.pLCSignalL_UI = new PLCSignalL_UI();

            ResourceHandler.dparamParameters.MesInteraction = new MesInteraction(main);
            main.MesPullOutSide_plateParameter();//加载侧板上传的表格，只有侧缝焊才有
            添加侧板出站表头();
            plcParameterSet();

        }

        public void plcParameterSet()
        {
            ResourceHandler.dparamParameters.siemensS7Net = new JPIO_OPC(main.workstationName.Text,DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "心跳"), main);
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "MES屏蔽"), new OPC_Event.Value_Set_Event(MesShielding), new OPC_Event.Value_Reset_Event(MesShieldingReset)));

            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站1触发"), new OPC_Event.Value_Set_Event(PullInTrigger), new OPC_Event.Value_Reset_Event(PullInReset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站2触发"), new OPC_Event.Value_Set_Event(PullInTrigger), new OPC_Event.Value_Reset_Event(PullInReset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站3触发"), new OPC_Event.Value_Set_Event(PullInTrigger), new OPC_Event.Value_Reset_Event(PullInReset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "进站4触发"), new OPC_Event.Value_Set_Event(PullInTrigger), new OPC_Event.Value_Reset_Event(PullInReset)));

            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站1触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站2触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站3触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站4触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));

            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站1触发"), new OPC_Event.Value_Set_Event(Side_board_exit), new OPC_Event.Value_Reset_Event(Side_board_exitResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站2触发"), new OPC_Event.Value_Set_Event(Side_board_exit), new OPC_Event.Value_Reset_Event(Side_board_exitResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站3触发"), new OPC_Event.Value_Set_Event(Side_board_exit), new OPC_Event.Value_Reset_Event(Side_board_exitResset)));
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站4触发"), new OPC_Event.Value_Set_Event(Side_board_exit), new OPC_Event.Value_Reset_Event(Side_board_exitResset)));

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
        public async void PullInTrigger(string RegAddress) {
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

            string moduleCode = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站模组条码{index}"));

            if (string.IsNullOrEmpty(moduleCode) || moduleCode.Equals(" ")) {
                main.outDiary($"进站{index}条码为空，条码:{moduleCode}", "警告");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                Thread.Sleep(50);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                Thread.Sleep(50);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NG"), true);
                PullInOpenSignal("进站NG", index);
                return;
            }

            main.outDiary($"进站{index}条码:{moduleCode}","信息");

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"进站{index}进入离线模式","信息");

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NP"), ResourceHandler.listSystemParameters[0].PNCode);
                main.outDiary($"进站{index}PN码：{ResourceHandler.listSystemParameters[0].PNCode}", "信息");

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}模组类型"), ResourceHandler.listSystemParameters[0].ModuleType);
                main.outDiary($"进站{index}模组类型：{ResourceHandler.listSystemParameters[0].ModuleType}", "信息");

                main.Invoke(new MethodInvoker(delegate {
                    string[] temp = new string[] { moduleCode, ResourceHandler.listSystemParameters[0].PNCode, ResourceHandler.listSystemParameters[0].ModuleType, $"进站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath +  "\\进站数据\\"+ DateTime.Now.ToString("yyyy年MM月dd日")+"\\", main.dataGridView1, moduleCode);
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

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath +  "\\进站数据\\"+ DateTime.Now.ToString("yyyy年MM月dd日")+"\\", main.dataGridView1, moduleCode);
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

                DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath +  "\\进站数据\\"+ DateTime.Now.ToString("yyyy年MM月dd日")+"\\", main.dataGridView1, moduleCode);
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
        public async void PullOutTrigger(string RegAddress) {
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

            MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx = new MachineIntegrationServiceService.dataCollectForSfcEx();
            dataCollectForSfcEx.SfcDcExRequest = new MachineIntegrationServiceService.sfcDcExRequest();
            dataCollectForSfcEx.SfcDcExRequest.parametricDataArray = new MachineIntegrationServiceService.machineIntegrationParametricData[2000];
            bool state = true;
            string[] temp = new string[main.dataGridView2.Columns.Count];

            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")] = moduleCode;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = ClassesJudge();
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "是否首件")] = "否";

            main.Invoke(new MethodInvoker(delegate {
                PullOutDataGain(temp, ref state, ref dataCollectForSfcEx);
            }));

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"出站{index}进入离线模式", "信息");

                main.Invoke(new MethodInvoker(delegate {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"出站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.White);
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView2, temp, Color.White);

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath +  "\\出站数据\\"+ DateTime.Now.ToString("yyyy年MM月dd日")+"\\", main.dataGridView2, moduleCode);

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

                        DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath +  "\\出站数据\\"+ DateTime.Now.ToString("yyyy年MM月dd日")+"\\", main.dataGridView2, moduleCode);
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

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath +  "\\出站数据\\"+ DateTime.Now.ToString("yyyy年MM月dd日")+"\\", main.dataGridView2, moduleCode);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"出站{index}OK"), true);
                PullInOpenSignal("出站OK", index);
                main.outDiary($"出站{index}成功", "信号");
                return;
            }else {
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
        public async void PullOutResset(string RegAddress) {
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
        public async void InitialWorkpiecePullOut(string RegAddress) {
            PullOutOpenSignal("首件触发", 1);
            main.outDiary($"接受到首件触发信号", "信息");

            string moduleCode = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"首件模组条码"));

            if (string.IsNullOrEmpty(moduleCode) || moduleCode.Equals(" ") )
            {
                main.outDiary($"首件条码为空，条码:{moduleCode}", "警告");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"首件NG"), true);
                PullOutOpenSignal("首件NG", 1);
                return;
            }

            main.outDiary($"首件条码:{moduleCode}", "信息");

            dataCollectForResourceFAI dataCollectForResourceFAI = new dataCollectForResourceFAI();
            dataCollectForResourceFAI.resourceRequest = new dataCollectForResourceFAIRequest();
            dataCollectForResourceFAI.resourceRequest.parametricDataArray = new machineIntegrationParametricData[2048];
            bool state = true;
            string[] temp = new string[main.dataGridView2.Columns.Count];
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")] = moduleCode;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = ClassesJudge();
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "设备标识")] = ResourceHandler.dparamParameters.offLineUI.textBox1.Text;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "是否首件")] = "是";

            main.Invoke(new MethodInvoker(delegate {
                InitialWorkpieceDataGain(temp, ref state, ref dataCollectForResourceFAI);
            }));

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
        public async void InitialWorkpiecePullOutResset(string RegAddress) {
            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "首件OK"), false);
            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "首件NG"), false);
            PullOutOpenSignal("首件复位", 1);
        }

        /// <summary>
        /// 侧板出站触发
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void Side_board_exit(string RegAddress)
        {
            int index = 0;
            if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站1触发")))
                index = 1;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站2触发")))
                index = 2;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站3触发")))
                index = 3;
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站4触发")))
                index = 4;
            PullOutOpenSignal("出站信号", index);
            main.outDiary($"接受到侧板出站{index}触发信号", "信息");

            string moduleCode = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"侧板条码{index}"));

            if (string.IsNullOrEmpty(moduleCode) || moduleCode.Equals(" "))
            {
                main.outDiary($"侧板出站{index}条码为空，条码:{moduleCode}", "警告");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"侧板出站{index}NG"), true);
                Thread.Sleep(50);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"侧板出站{index}NG"), true);
                Thread.Sleep(50);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"侧板出站{index}NG"), true);
                PullOutOpenSignal("出站NG", index);
                return;
            }

            main.outDiary($"侧板出站{index}条码:{moduleCode}", "信息");

            MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx = new MachineIntegrationServiceService.dataCollectForSfcEx();
            dataCollectForSfcEx.SfcDcExRequest = new MachineIntegrationServiceService.sfcDcExRequest();
            dataCollectForSfcEx.SfcDcExRequest.parametricDataArray = new MachineIntegrationServiceService.machineIntegrationParametricData[2000];
            bool state = true;
            string[] temp = new string[main.dataGridView4.Columns.Count];

            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView4, "模组码")] = moduleCode;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView4, "班次")] = ClassesJudge();
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView4, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView4, "是否首件")] = "否";

            main.Invoke(new MethodInvoker(delegate {
                Side_boardDataGain(temp, ref state, ref dataCollectForSfcEx);
            }));

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"侧板出站{index}进入离线模式", "信息");

                main.Invoke(new MethodInvoker(delegate {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"侧板出站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.White);
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView4, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView4, temp, Color.White);

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\侧板出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView4, moduleCode);

                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"侧板出站{index}OK"), true);
                PullOutOpenSignal("出站Ok", index);
                main.outDiary($"侧板出站{index}OK", "信息");
                return;
            }

            main.outDiary($"侧板出站{index}进入在线模式", "信息");

            if (state)
            {
                ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.PullOut(moduleCode, dataCollectForSfcEx);
                if (responseData.code != 0)
                {
                    main.Invoke(new MethodInvoker(delegate {
                        string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"侧板出站{index}", "NG", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                        DataGridViewClass.AddRows(main.dataGridView1, dataGridViewTemp, Color.Red);

                        temp[DataGridViewClass.GetColumnsIndex(main.dataGridView4, "结果")] = "NG";
                        DataGridViewClass.AddRows(main.dataGridView4, temp, Color.Red);

                        DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\侧板出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView4, moduleCode);
                    }));

                    ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"侧板出站{index}NG"), true);
                    PullInOpenSignal("出站NG", index);
                    main.outDiary($"侧板出站{index}失败", "报警");
                    MessageBox.Show($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");
                    return;
                }
                main.Invoke(new MethodInvoker(delegate {
                    string[] dataGridViewTemp = new string[] { moduleCode, "", "", $"侧板出站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);

                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView4, "结果")] = "OK";
                    DataGridViewClass.AddRows(main.dataGridView4, temp, Color.White);

                    DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\侧板出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", main.dataGridView4, moduleCode);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"侧板出站{index}OK"), true);
                PullInOpenSignal("出站OK", index);
                main.outDiary($"侧板出站{index}成功", "信号");
                return;
            }
            else
            {
                MessageBox.Show("侧板出站失败，参数错误或超上下限", "错误");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"侧板出站{index}NG"), true);
                PullOutOpenSignal("出站NG", index);
                return;
            }
        }

        /// <summary>
        /// 侧板出站复位
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void Side_board_exitResset(string RegAddress)
        {
            if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站1触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站1OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站1NG"), false);
                PullOutOpenSignal("出站复位", 1);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站2触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站2OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站2NG"), false);
                PullOutOpenSignal("出站复位", 2);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站3触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站3OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站3NG"), false);
                PullOutOpenSignal("出站复位", 3);
                return;
            }
            else if (RegAddress.Equals(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站4触发")))
            {
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站4OK"), false);
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "侧板出站4NG"), false);
                PullOutOpenSignal("出站复位", 4);
                return;
            }
        }

        public void 添加侧板出站表头()
        {
            DataGridViewClass.RemoveAllColumns(main.dataGridView4);
            string[] header1 = new string[ResourceHandler.listMesPullOutSide_plateParameters.Count + 5];
            header1[0] = "模组码";
            for (int i = 0; i < ResourceHandler.listMesPullOutSide_plateParameters.Count; i++)
            {
                header1[i + 1] = ResourceHandler.listMesPullOutSide_plateParameters[i].Header;
            }
            header1[ResourceHandler.listMesPullOutSide_plateParameters.Count + 1] = "结果";
            header1[ResourceHandler.listMesPullOutSide_plateParameters.Count + 2] = "班次";
            header1[ResourceHandler.listMesPullOutSide_plateParameters.Count + 3] = "设备标识";
            header1[ResourceHandler.listMesPullOutSide_plateParameters.Count + 4] = "是否首件";
            DataGridViewClass.AddColumns(main.dataGridView4, header1);
        }
    }
}
