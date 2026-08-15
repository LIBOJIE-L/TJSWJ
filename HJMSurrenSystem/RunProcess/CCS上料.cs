using System;
using System.Windows.Forms;
using System.Drawing;
using HJMSurrenSystem.Parameters;
using Siemens;
using HJMSurrenSystem.Siemens;
using DataModel;
using MachineIntegrationServiceService;
using HJMSurrenSystem.MES;
using System.Web.Services.Description;
using System.Reflection;
using HJMSurrenSystem_Siemens;

namespace HJMSurrenSystem.RunProcess
{
    public class CCS上料 : FlowClass
    {
        #region 定义 

        #endregion

        public CCS上料(Main main)
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
            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "出站2触发"), new OPC_Event.Value_Set_Event(PullOutTrigger), new OPC_Event.Value_Reset_Event(PullOutResset)));

            ResourceHandler.dparamParameters.siemensS7Net.OPC_AddEventList(new OPC_Event(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "上料进站触发"), new OPC_Event.Value_Set_Event(loading), new OPC_Event.Value_Reset_Event(Charging_reset)));

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

                main.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = new string[] { moduleCode, ResourceHandler.listSystemParameters[0].PNCode, ResourceHandler.listSystemParameters[0].ModuleType, $"进站{index}", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);
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

            dataCollectForSfcEx dataCollectForSfcEx = new dataCollectForSfcEx();
            dataCollectForSfcEx.SfcDcExRequest = new sfcDcExRequest();
            dataCollectForSfcEx.SfcDcExRequest.parametricDataArray = new MachineIntegrationServiceService.machineIntegrationParametricData[2000];
            bool state = true;
            string[] temp = new string[main.dataGridView2.Columns.Count];

            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")] = moduleCode;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "班次")] = ClassesJudge();
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "是否首件")] = "否";

            //获取表头名称
            string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[0].Cells[0].Value.ToString();
            //获取PLC地址
            string parametersAddress = DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"CCS条码{index}");
            //获取PLC地址类型
            string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[0].Cells[4].Value.ToString();

            main.Invoke(new MethodInvoker(delegate
            {
                object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);
                bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[0].Cells[7].Value);
                int index1 = 0;
                if (whetherUploading)
                {
                    //获取MES名称
                    string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[0].Cells[1].Value.ToString();

                    //获取MES类型
                    string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[0].Cells[2].Value.ToString();

                    dataCollectForSfcEx.SfcDcExRequest.parametricDataArray[index1] = GetParametricData(
                                                        ParametersMESName, // 标识符
                                                        ParametersMESType, // 数据类型
                                                        parametersPrice.ToString()// 数据值
                                                        );
                    index1++;
                }
                temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
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

                        temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "结果")] = "NG";
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

        /// <summary>
        /// 上料进站触发
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void loading(string RegAddress)
        {
            int index = 0;
            index = 1;

            //PullInOpenSignal("进站信号", index);
            main.outDiary($"接受到上料进站触发信号", "信息");

            string moduleCode = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"上料进站条码"));

            if (string.IsNullOrEmpty(moduleCode) || moduleCode.Equals(" "))
            {
                main.outDiary($"上料进站条码为空，条码:{moduleCode}", "警告");
                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"上料进站NG"), true);
                //PullInOpenSignal("进站NG", index);
                return;
            }

            main.outDiary($"上料进站条码:{moduleCode}", "信息");

            if (ResourceHandler.dparamParameters.JudgeOffLine)
            {
                main.outDiary($"上料进站进入离线模式", "信息");

                //ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"上料进站PN码"), ResourceHandler.listSystemParameters[0].PNCode);
                //main.outDiary($"上料进站PN码：{ResourceHandler.listSystemParameters[0].PNCode}", "信息");

                //ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}模组类型"), ResourceHandler.listSystemParameters[0].ModuleType);
                //main.outDiary($"上料进站模组类型：{ResourceHandler.listSystemParameters[0].ModuleType}", "信息");

                main.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = new string[] { moduleCode, "", "", $"上料进站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"上料进站OK"), true);
                //PullInOpenSignal("进站OK", index);
                main.outDiary($"上料进站OK", "信息");
                return;
            }

            main.outDiary($"上料进站进入在线模式", "信息");
            ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.PullIn(moduleCode);
            if (responseData.code != 0)
            {
                main.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = new string[] { moduleCode, "", "", $"上料进站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                    DataGridViewClass.AddRows(main.dataGridView1, temp, Color.Red);
                }));

                ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"上料进站NG"), true);
                //PullInOpenSignal("上料进站NG", index);
                main.outDiary($"上料进站失败", "报警");
                MessageBox.Show($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");

                return;
            }
            main.Invoke(new MethodInvoker(delegate
            {
                string[] temp = new string[] { moduleCode, "", "", $"上料进站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") };
                DataGridViewClass.AddRows(main.dataGridView1, temp, Color.White);
            }));

            //ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}NP"), responseData.pnCode);
            //ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_string(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"进站{index}模组类型"), responseData.type);

            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, $"上料进站OK"), true);
            //PullInOpenSignal("进站OK", index);
            main.outDiary($"上料进站成功", "信号");

        }

        /// <summary>
        /// 上料进站复位
        /// </summary>
        /// <param name="RegAddress"></param>
        public async void Charging_reset(string RegAddress)
        {

            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "上料进站OK"), false);
            ResourceHandler.dparamParameters.siemensS7Net.PLC_Write_bool(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, "上料进站NG"), false);
            //PullInOpenSignal("进站复位", 1);

        }

    }
}
