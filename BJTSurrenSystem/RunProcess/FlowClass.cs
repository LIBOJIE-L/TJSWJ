using DataModel;
using BJTSurrenSystem.Parameters;
using MachineIntegrationServiceService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJTSurrenSystem.RunProcess
{
    public abstract class FlowClass
    {
        public Main main;

        /// <summary>
        /// 离线触发
        /// </summary>
        /// <param name="RegAddress"></param>
        public void MesShielding(string RegAddress)
        {
            main.gatherPattern.Text = "离线";
            main.gatherPattern.ForeColor = Color.Red;
            main.outDiary($"MES进入离线模式!", "信息");
            ResourceHandler.dparamParameters.JudgeOffLine = true;

            ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox2.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
        }

        /// <summary>
        /// 在线触发
        /// </summary>
        /// <param name="RegAddress"></param>
        public void MesShieldingReset(string RegAddress)
        {
            main.gatherPattern.Text = "在线";
            main.gatherPattern.ForeColor = Color.Green;
            main.outDiary($"MES进入在线模式!", "信息");
            ResourceHandler.dparamParameters.JudgeOffLine = false;

            ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox2.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
        }

        /// <summary>
        /// 进站信号灯改变
        /// </summary>
        /// <param name="strSignal"></param>
        /// <param name="index"></param>
        public void PullInOpenSignal(string strSignal, int index)
        {
            if (strSignal == "进站信号" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox3.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站信号" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox14.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站信号" && index == 3)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox20.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站信号" && index == 4)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox26.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站NG" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox5.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站NG" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox12.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站NG" && index == 3)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox18.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站NG" && index == 4)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox24.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站OK" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox4.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站OK" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox13.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站OK" && index == 3)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox19.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站OK" && index == 4)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox25.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "进站复位" && index == 1)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox3.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox4.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox5.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "进站复位" && index == 2)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox15.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox14.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox13.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "进站复位" && index == 3)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox20.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox19.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox18.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "进站复位" && index == 4)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox26.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox25.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox24.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "回流信号" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox38.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "回流OK" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox37.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "回流NG" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox36.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "回流信号" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox41.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "回流OK" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox40.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "回流NG" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox39.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "回流复位" && index == 1)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox38.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox37.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox36.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "回流复位" && index == 2)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox41.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox40.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox39.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
        }

        /// <summary>
        /// 出站信号灯改变
        /// </summary>
        /// <param name="strSignal"></param>
        /// <param name="index"></param>
        public void PullOutOpenSignal(string strSignal, int index)
        {
            if (strSignal == "出站信号" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox6.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站信号" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox11.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站信号" && index == 3)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox17.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站信号" && index == 4)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox23.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站NG" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox8.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站NG" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox9.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站NG" && index == 3)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox15.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站NG" && index == 4)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox21.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站OK" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox7.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站OK" && index == 2)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox10.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站OK" && index == 3)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox16.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站OK" && index == 4)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox22.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "出站复位" && index == 1)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox6.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox7.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox8.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "出站复位" && index == 2)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox9.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox10.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox11.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "出站复位" && index == 3)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox15.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox16.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox17.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "出站复位" && index == 4)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox21.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox22.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox23.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
            else if (strSignal == "首件触发" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox35.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "首件OK" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox34.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "首件NG" && index == 1)
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox33.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯绿.png");
            else if (strSignal == "首件复位" && index == 1)
            {
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox35.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox35.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
                ResourceHandler.dparamParameters.pLCSignalL_UI.pictureBox35.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/信号灯红.png");
            }
        }

        /// <summary>
        /// 出站数据收集
        /// </summary>
        /// <param name="temp">表头数组</param>
        /// <param name="state">参数判断</param>
        /// <param name="dataCollectForSfcEx">上传MES数组</param>
        public void PullOutDataGain(string[] temp, ref bool state, ref MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();

                object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);


                bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                if (whetherUploading)
                {
                    if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                    {
                        if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                        {
                            state = false;
                            main.outDiary($"参数值超限：{parametersName}", "警告");
                        }
                    }
                    //获取MES名称
                    string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                    //获取MES类型
                    string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                    dataCollectForSfcEx.SfcDcExRequest.parametricDataArray[index] = GetParametricData(
                                                      ParametersMESName, // 标识符
                                                      ParametersMESType, // 数据类型
                                                      parametersPrice.ToString()// 数据值
                                                      );
                    index++;
                }
                temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
            }
        }

        /// <summary>
        /// 侧板出站数据收集
        /// </summary>
        /// <param name="temp">表头数组</param>
        /// <param name="state">参数判断</param>
        /// <param name="dataCollectForSfcEx">上传MES数组</param>
        public void Side_boardDataGain(string[] temp, ref bool state, ref MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows[i].Cells[6].Value.ToString();

                object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);


                bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows[i].Cells[7].Value);

                if (whetherUploading)
                {
                    if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                    {
                        if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                        {
                            state = false;
                            main.outDiary($"参数值超限：{parametersName}", "警告");
                        }
                    }
                    //获取MES名称
                    string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                    //获取MES类型
                    string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                    dataCollectForSfcEx.SfcDcExRequest.parametricDataArray[index] = GetParametricData(
                                                      ParametersMESName, // 标识符
                                                      ParametersMESType, // 数据类型
                                                      parametersPrice.ToString()// 数据值
                                                      );
                    index++;
                }
                temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
            }
        }

        /// <summary>
        /// 首件
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="state"></param>
        /// <param name="dataCollectForSfcEx"></param>
        public void InitialWorkpieceDataGain(string[] temp, ref bool state, ref dataCollectForResourceFAI dataCollectForResourceFAI)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();

                object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);


                bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                if (whetherUploading)
                {
                    if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                    {
                        if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                        {
                            state = false;
                            main.outDiary($"参数值超限：{parametersName}", "警告");
                        }
                    }
                    //获取MES名称
                    string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                    //获取MES类型
                    string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                    dataCollectForResourceFAI.resourceRequest.parametricDataArray[index] = GetParametricDataInitialWorkpiece(
                                                      ParametersMESName, // 标识符
                                                      ParametersMESType, // 数据类型
                                                      parametersPrice.ToString()// 数据值
                                                      );
                    index++;
                }
                temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
            }
        }

        ///// <summary>
        ///// 补焊出站数据收集
        ///// </summary>
        ///// <param name="temp">表头数组</param>
        ///// <param name="state">参数判断</param>
        ///// <param name="dataCollectForSfcEx">上传MES数组</param>
        //public void BHPullOutDataGain(string[] temp, ref bool state, ref MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx)
        //{
        //    try
        //    {
        //        //读取补焊出站保存的数据

        //        string modeCode = temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")];
        //        main.outDiary($"补焊出站数据收集模组码：{modeCode}", "消息");
        //        string fileName = ResourceHandler.listSystemParameters[0].ProgramLogPath + $"\\停机出站数据\\" + Convert.ToDateTime(DateTime.Now).ToString("yyyy年MM月dd日") + "\\" + modeCode + ".CSV";

        //        List<string> list2 = DataGridViewClass.Read_CSV(fileName);
        //        //储存补焊数据
        //        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

        //        if (list2 == null || list2.Count <= 1)
        //        {
        //            MessageBox.Show("没有匹配的补焊数据");
        //            return;
        //            //int i = DataGridViewClass.GetColumnsIndex(list2, modeCode);
        //            //string[] data = list2[i].Split(',');
        //            //DataGridViewClass.RemoveAllRow(main.dataGridView3);
        //            //DataGridViewClass.AddRows(main.dataGridView3, data, Color.White);
        //        }
        //        //拆分表头名称
        //        string[] dataName = list2[0].Split(',');
        //        //拆分最后一次的数据
        //        string[] dataList = list2[list2.Count - 1].Split(',');
        //        //循环放进list集合里
        //        for (int i = 0; i < dataName.Length; i++)
        //        {
        //            keyValuePairs.Add(dataName[i], dataList[i]);
        //        }



        //        int index = 0;
        //        for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
        //        {
        //            //CBTJKSSJ
        //            //CBTJZJYKSSJ
        //            //获取表头名称
        //            string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
        //            //获取PLC地址
        //            string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
        //            //获取PLC地址类型
        //            string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
        //            //获取上限值
        //            string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
        //            //获取下限值
        //            string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();

        //            /*                object parametersPrice = main.dataGridView3.Rows[0].Cells[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)].Value.ToString();*/

        //            object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);
        //            //判断数据是否为0，为0则赋值旧数据
        //            if ((parametersAddressType.ToLower() == "int"
        //                || parametersAddressType.ToLower() == "float"
        //                || parametersAddressType.ToLower() == "short")
        //                && Convert.ToDouble(parametersPrice) == 0)
        //            {
        //                if (keyValuePairs.ContainsKey(parametersName))
        //                {
        //                    parametersPrice = keyValuePairs[parametersName];
        //                }
        //                else
        //                {
        //                    main.outDiary($"未在旧数据中查到相关数据名称【{parametersName}】，请查看名称是否一致！", "警告");
        //                }
        //            }

        //            bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

        //            if (whetherUploading)
        //            {
        //                if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
        //                {
        //                    if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
        //                    {
        //                        state = false;
        //                        main.outDiary($"参数值超限：{parametersName}", "警告");
        //                    }
        //                }

        //                //获取MES名称
        //                string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

        //                //获取MES类型
        //                string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

        //                dataCollectForSfcEx.SfcDcExRequest.parametricDataArray[index] = GetParametricData(
        //                                                  ParametersMESName, // 标识符
        //                                                  ParametersMESType, // 数据类型
        //                                                  parametersPrice.ToString()// 数据值
        //                                                  );
        //                index++;
        //            }

        //            temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        main.outDiary("补焊出站收集数据产生错误：" + ex.Message, "信息");
        //        throw;
        //    }


        //}

        /// <summary>
        /// 补焊出站数据收集
        /// </summary>
        /// <param name="temp">表头数组</param>
        /// <param name="state">参数判断</param>
        /// <param name="dataCollectForSfcEx">上传MES数组</param>
        public void BHPullOutDataGain(string[] temp, ref bool state, ref MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx)
        {
            try
            {
                //读取补焊出站保存的数据

                string modeCode = temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, "模组码")];
                main.outDiary($"补焊出站数据收集模组码：{modeCode}", "消息");
                string fileName = ResourceHandler.listSystemParameters[0].ProgramLogPath + $"\\停机出站数据\\" + Convert.ToDateTime(DateTime.Now).ToString("yyyy年MM月dd日") + "\\" + modeCode + ".CSV";

                List<string> list2 = DataGridViewClass.Read_CSV(fileName);
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                if (list2 != null && list2.Count > 1)
                {
                    int i = DataGridViewClass.GetColumnsIndex(list2, modeCode);
                    string[] data = list2[i].Split(',');
                    DataGridViewClass.RemoveAllRow(main.dataGridView3);
                    DataGridViewClass.AddRows(main.dataGridView3, data, Color.White);
                }
                else
                {
                    MessageBox.Show("没有匹配的补焊数据");
                    return;
                }
                int index = 0;
                for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
                {
                    //获取表头名称
                    string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                    //获取PLC地址
                    string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                    //获取PLC地址类型
                    string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                    //获取上限值
                    string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                    //获取下限值
                    string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();
                    //获取数据数量地址
                    string DataQuantityPath = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[10].Value.ToString();
                    //获取数据数量PLC类型
                    string DataQuantityPathType = "";
                    //获取递增值
                    string DataIncrementValue = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[12].Value.ToString();


                    if (null != ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value)
                    {
                        DataQuantityPathType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value.ToString();
                    }

                    //object TempParameters = "";
                    object parametersPrice = main.dataGridView3.Rows[0].Cells[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)].Value.ToString();
                    //判断数据是否为0，为0则赋值旧数据
                    if ((parametersAddressType.ToLower() == "int"
                        || parametersAddressType.ToLower() == "float"
                        || parametersAddressType.ToLower() == "short"
                        || parametersAddressType.ToLower() == "string")
                        //&& Convert.ToDouble(parametersPrice) == 0)
                        && parametersPrice.ToString() == "0")
                    {
                        if (keyValuePairs.ContainsKey(parametersName))
                        {
                            parametersPrice = keyValuePairs[parametersName];
                        }
                        else
                        {
                            main.outDiary($"未在旧数据中查到相关数据名称【{parametersName}】，请查看名称是否一致！", "警告");
                        }
                    }

                    if (parametersName.Contains("至"))
                    {
                        /*parametersPrice = Convert.ToDouble(parametersPrice) / 1000;*/
                        main.outDiary($"：{ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString()}{parametersName}---{parametersPrice}", "信息");

                    }
                    if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                    {
                        if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                        {
                            state = false;
                            main.outDiary($"参数值超限：{parametersName}", "警告");
                        }
                    }

                    bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                    if (whetherUploading)
                    {
                        //获取MES名称
                        string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                        //获取MES类型
                        string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                        dataCollectForSfcEx.SfcDcExRequest.parametricDataArray[index] = GetParametricData(
                                                          ParametersMESName, // 标识符
                                                          ParametersMESType, // 数据类型
                                                          parametersPrice.ToString()// 数据值
                                                          );
                        index++;
                    }
                    if (!string.IsNullOrEmpty(DataQuantityPath) && !string.IsNullOrEmpty(DataIncrementValue))
                    {
                        parametersPrice += parametersPrice.ToString() + "，";
                        // 获取下一个数据的地址
                        string[] parametersAddressArray = parametersAddress.Split('.');

                        if (2 > parametersAddressArray.Length)
                        {
                            //Thread.Sleep(50);
                            continue;
                        }
                        parametersAddress = GetNewPath(parametersAddressArray, DataIncrementValue);
                        temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
                }
            }
            catch (Exception ex)
            {
                main.outDiary("补焊出站收集数据产生错误：" + ex.Message, "信息");
                throw;
            }


        }

        /// <summary>
        /// 电芯校验
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="state"></param>
        /// <param name="dataCollectForResourceFAI"></param>
        public void DX_verify(string[] temp, ref miSFCAttriDataEntryRequest miSFCAttriDataEntryRequest)
        {
            int index = 0;
            sfcData[] sArr = new sfcData[ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count];

            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();


                object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);


                bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                if (whetherUploading)
                {
                    sfcData sd = new sfcData();
                    sd.value = "" + parametersPrice;
                    sd.attributes = "M_CELL_SEQUENCE";
                    sd.sequence = null;
                    sArr[index] = sd;
                    main.outDiary("【电芯装配校验】进站电芯码依次：" + parametersPrice, "信息");
                    index++;
                }
                temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
            }
            miSFCAttriDataEntryRequest.sfcDatalist = sArr;
        }

        /// <summary>
        /// 手动出站数据收集
        /// </summary>
        /// <param name="temp">表头数组</param>
        /// <param name="state">参数判断</param>
        /// <param name="dataCollectForSfcEx">上传MES数组</param>
        public void ManuallyUploadDataCollection2(string[] temp, int row, ref bool state, List<MachineIntegrationServiceService.machineIntegrationParametricData> list)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                string parametersValue = DataGridViewClass.GetRowsData(main.dataGridView3, row)[DataGridViewClass.GetColumnsIndex(main.dataGridView3, parametersName)];
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();

                for (int j = 0; j < main.dataGridView3.ColumnCount; j++)
                {
                    if (main.dataGridView3.Columns[j].HeaderText.Equals(parametersName))//对比和mes参数表的名字是否一致
                    {
                        string[] arrayValue = main.dataGridView3.Rows[row].Cells[j].Value.ToString().Split('，');//分割单元格里的数据
                        int dataCount = arrayValue.Length;

                        if (dataCount <= 0)
                        { continue; }

                        object TempParameters = "";
                        for (int k = 0; k < dataCount; k++)
                        {
                            object parametersPrice = arrayValue[k];
                            //if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                            //{
                            //    if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                            //    {
                            //        state = false;
                            //        main.outDiary($"参数值超限：{parametersName}", "警告");
                            //    }
                            //}

                            bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                            if (whetherUploading)
                            {
                                //获取MES名称
                                string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString() + (k + 1);
                                if (dataCount == 1)
                                {
                                    ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();
                                }
                                //获取MES类型
                                string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                                list.Add(GetParametricData(
                                                                  ParametersMESName, // 标识符
                                                                  ParametersMESType, // 数据类型
                                                                  parametersPrice.ToString().Replace(";", "").Replace("\"", "")// 数据值"
                                                                  ));
                                index++;
                            }
                            TempParameters += parametersPrice.ToString() + "，";

                        }
                        if (!string.IsNullOrEmpty(TempParameters.ToString()))
                        {
                            TempParameters = TempParameters.ToString().Substring(0, TempParameters.ToString().Length - 1);
                        }
                        temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = TempParameters.ToString();

                    }
                }


            }
        }

        /// <summary>
        /// 手动出站数据上传
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="row"></param>
        /// <param name="state"></param>
        /// <param name="dataCollectForSfcEx"></param>
        public void ManuallyUploadDataCollection(string[] temp, int row, ref bool state, ref MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取本地文件数据
                string parametersValue = DataGridViewClass.GetRowsData(main.dataGridView3, row)[DataGridViewClass.GetColumnsIndex(main.dataGridView3, parametersName)];
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();

                if (parametersUpperLimit != "-" && parametersLowerLimit != "-" && !string.IsNullOrEmpty(parametersValue))
                {
                    if (Convert.ToDouble(parametersValue) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersValue) < Convert.ToDouble(parametersLowerLimit))
                    {
                        state = false;
                        main.outDiary($"参数值超限：{parametersName}", "警告");
                    }
                }

                bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                if (whetherUploading)
                {
                    //获取MES名称
                    string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                    //获取MES类型
                    string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                    dataCollectForSfcEx.SfcDcExRequest.parametricDataArray[index] = GetParametricData(
                                                      ParametersMESName, // 标识符
                                                      ParametersMESType, // 数据类型
                                                      parametersValue// 数据值
                                                      );
                    index++;
                }
                temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersValue;
            }
        }

        /// <summary>
        /// 手动首件数据上传
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="row"></param>
        /// <param name="state"></param>
        /// <param name="dataCollectForSfcEx"></param>
        public void ManuallyUploadDataCollection(string[] temp, int row, ref bool state, ref dataCollectForResourceFAI dataCollectForResourceFAI)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取本地文件数据
                string parametersValue = DataGridViewClass.GetRowsData(main.dataGridView3, row)[DataGridViewClass.GetColumnsIndex(main.dataGridView3, parametersName)];
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();

                //if (parametersUpperLimit != "-" && parametersLowerLimit != "-" && !string.IsNullOrEmpty(parametersValue))
                //{
                //    if (Convert.ToDouble(parametersValue) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersValue) < Convert.ToDouble(parametersLowerLimit))
                //    {
                //        state = false;
                //        main.outDiary($"参数值超限：{parametersName}", "警告");
                //    }
                //}

                //bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                //if (whetherUploading)
                //{
                //    //获取MES名称
                //    string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                //    //获取MES类型
                //    string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                //    dataCollectForResourceFAI.resourceRequest.parametricDataArray[index] = GetParametricDataInitialWorkpiece(
                //                                      ParametersMESName, // 标识符
                //                                      ParametersMESType, // 数据类型
                //                                      parametersValue.ToString().Replace(";", "").Replace("\"", "")// 数据值
                //                                      );
                //    index++;
                //}
                //temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersValue;
                string parametersValueS = parametersValue.Replace(";", "").Replace("\"", "");
                if (!parametersValueS.Contains("，"))
                {
                    if (parametersUpperLimit != "-" && parametersLowerLimit != "-" && !string.IsNullOrEmpty(parametersValueS))
                    {
                        if (Convert.ToDouble(parametersValueS) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersValueS) < Convert.ToDouble(parametersLowerLimit))
                        {
                            state = false;
                            main.outDiary($"参数值超限：{parametersName}", "警告");
                        }
                    }
                }

                if (parametersValueS.Contains("，"))
                {
                    string[] datas = parametersValueS.Split('，').ToList().Where(e => e != "").ToArray(); ;
                    int dataCount = datas.Length;

                    if (dataCount <= 0)
                    { continue; }
                    object TempParameters = ";";
                    for (int j = 0; j < dataCount; j++)
                    {
                        object parametersPrice = datas[j];
                        

                        bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[8].Value);

                        if (whetherUploading)
                        {
                            if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                            {
                                if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                                {
                                    state = false;
                                    main.outDiary($"参数值超限：{parametersName}", "警告");
                                }
                            }
                            //获取MES名称
                            string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString() + (j + 1);

                            //获取MES类型
                            string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                            dataCollectForResourceFAI.resourceRequest.parametricDataArray[index] = GetParametricDataInitialWorkpiece(
                                                         ParametersMESName, // 标识符
                                                         ParametersMESType, // 数据类型
                                                         parametersPrice.ToString().Replace(";", "").Replace("\"", "")// 数据值
                                                         );

                            index++;
                        }
                        TempParameters += parametersPrice.ToString() + "，";

                    }
                    if (!string.IsNullOrEmpty(TempParameters.ToString()))
                    {
                        TempParameters = TempParameters.ToString().Substring(0, TempParameters.ToString().Length - 1);
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = TempParameters.ToString();
                }
                else
                {
                    bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[8].Value);

                    if (whetherUploading)
                    {
                        //获取MES名称
                        string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                        //获取MES类型
                        string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                        dataCollectForResourceFAI.resourceRequest.parametricDataArray[index] = GetParametricDataInitialWorkpiece(
                                                          ParametersMESName, // 标识符
                                                          ParametersMESType, // 数据类型
                                                          parametersValue.ToString().Replace(";", "").Replace("\"", "")// 数据值
                                                          );
                        index++;
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersValue;
                }
            }
        }

        /// <summary>
        /// 传值到MES数组
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MachineIntegrationServiceService.machineIntegrationParametricData GetParametricData(string name, string type, string value)
        {
            MachineIntegrationServiceService.machineIntegrationParametricData dataArray = new MachineIntegrationServiceService.machineIntegrationParametricData();
            dataArray.name = name;// 相应的值名
            dataArray.value = value;// 传输的值

            // 选择数据传输类型
            switch (type)
            {
                case "NUMBER":
                    dataArray.dataType = MachineIntegrationServiceService.ParameterDataType.NUMBER;
                    break;
                case "TEXT":
                    dataArray.dataType = MachineIntegrationServiceService.ParameterDataType.TEXT;
                    break;
                case "FORMULA":
                    dataArray.dataType = MachineIntegrationServiceService.ParameterDataType.FORMULA;
                    break;
                case "BOOLEAN":
                    dataArray.dataType = MachineIntegrationServiceService.ParameterDataType.BOOLEAN;
                    break;
                default:
                    dataArray.dataType = MachineIntegrationServiceService.ParameterDataType.NUMBER;
                    break;
            }
            return dataArray;
        }

        /// <summary>
        /// 首件数据收集
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public machineIntegrationParametricData GetParametricDataInitialWorkpiece(string name, string type, string value)
        {
            machineIntegrationParametricData dataArray = new machineIntegrationParametricData();
            dataArray.name = name;// 相应的值名
            dataArray.value = value;// 传输的值

            // 选择数据传输类型
            switch (type)
            {
                case "NUMBER":
                    dataArray.dataType = ParameterDataType.NUMBER;
                    break;
                case "TEXT":
                    dataArray.dataType = ParameterDataType.TEXT;
                    break;
                case "FORMULA":
                    dataArray.dataType = ParameterDataType.FORMULA;
                    break;
                case "BOOLEAN":
                    dataArray.dataType = ParameterDataType.BOOLEAN;
                    break;
                default:
                    dataArray.dataType = ParameterDataType.NUMBER;
                    break;
            }

            return dataArray;
        }

        /// <summary>
        /// 判断数据类型，读取PLC值
        /// </summary>
        /// <param name="parametersAddress"></param>
        /// <param name="parametersAddressType"></param>
        /// <returns></returns>
        public object ParametersJudge(string parametersAddress, string parametersAddressType)
        {
            if (parametersAddressType.Equals("BOOL"))
            {
                return ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_bool(parametersAddress);
            }
            else if (parametersAddressType.Equals("BYTE"))
            {
                return ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_byte(parametersAddress);
            }
            else if (parametersAddressType.Equals("FLOAT"))
            {
                return ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_float(parametersAddress);
            }
            else if (parametersAddressType.Equals("INT"))
            {
                return ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_int(parametersAddress);
            }
            else if (parametersAddressType.Equals("SHORT"))
            {
                return ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_short(parametersAddress);
            }
            else if (parametersAddressType.Equals("STRING"))
            {
                return ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(parametersAddress);
            }
            return "";
        }

        /// <summary>
        /// 判断班次
        /// </summary>
        /// <returns></returns>
        public string ClassesJudge()
        {
            int time = DateTime.Now.Hour;
            if (time < Convert.ToInt16(ResourceHandler.listSystemParameters[0].nightShift) && time >= Convert.ToInt16(ResourceHandler.listSystemParameters[0].dayShift))
                return ResourceHandler.dparamParameters.offLineUI.label3.Text;

            else
                return ResourceHandler.dparamParameters.offLineUI.label8.Text;
        }

        public string GetElectricCoreCode()
        {

            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                if (ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("电芯条码"))
                {
                    string moduleCode = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_String(DataGridViewClass.FindIniPath(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString()));
                    if (moduleCode != "" && moduleCode != " ")
                    {
                        return moduleCode;
                    }
                }
            }
            return "";
        }


        /// <summary>
        /// 出站数据收集
        /// </summary>
        /// <param name="temp">表头数组</param>
        /// <param name="state">参数判断</param>
        /// <param name="dataCollectForSfcEx">上传MES数组</param>
        public void PullOutDatasGain(string[] temp, ref bool state, List<MachineIntegrationServiceService.machineIntegrationParametricData> list)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();
                //获取数据数量地址
                string DataQuantityPath = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[10].Value.ToString();
                //获取数据数量PLC类型
                string DataQuantityPathType = "";
                if (null != ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value)
                {
                    DataQuantityPathType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value.ToString();
                }
                //获取递增值
                string DataIncrementValue = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[12].Value.ToString();

                if (!string.IsNullOrEmpty(DataQuantityPath) && !string.IsNullOrEmpty(DataIncrementValue))
                {
                    uint dataCount;
                    switch (DataQuantityPathType)
                    {
                        case "usint":
                        default:
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_byte(DataQuantityPath);
                            break;
                        case "uint":
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_uint(DataQuantityPath);
                            break;
                    }
                    if (dataCount <= 0)
                    { continue; }
                    object TempParameters = ";";
                    for (int j = 0; j < dataCount; j++)
                    {
                        //Thread.Sleep(10);
                        object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);
                        if (parametersPrice.ToString() == "0")
                        {
                            parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);
                        }


                        bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                        if (whetherUploading)
                        {
                            if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                            {
                                if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                                {
                                    state = false;
                                    main.outDiary($"参数值超限：{parametersName}", "警告");

                                }

                            }
                            //获取MES名称
                            string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString() + (j + 1);

                            //获取MES类型
                            string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                            list.Add(GetParametricData(
                                                              ParametersMESName, // 标识符
                                                              ParametersMESType, // 数据类型
                                                              parametersPrice.ToString().Replace("\"","") // 数据值
                                                              ));
                            index++;
                        }
                        TempParameters += parametersPrice.ToString() + "，";
                        // 获取下一个数据的地址
                        string[] parametersAddressArray = parametersAddress.Split('.');

                        if (2 > parametersAddressArray.Length)
                        {
                            //Thread.Sleep(50);
                            continue;
                        }
                        parametersAddress = GetNewPath(parametersAddressArray, DataIncrementValue);
                    }
                    if (!string.IsNullOrEmpty(TempParameters.ToString()))
                    {
                        TempParameters = TempParameters.ToString().Substring(0, TempParameters.ToString().Length - 1);
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = TempParameters.ToString();
                }
                else
                {
                    object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);

                    //if (parametersName.Contains("焊中检测结果"))
                    //{
                    //    parametersPrice = parametersPrice.ToString() == "false" ? "0" : "1";
                    //}
                    bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                    if (whetherUploading)
                    {
                        if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                        {
                            if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                            {
                                state = false;
                                main.outDiary($"参数值超限：{parametersName}", "警告");
                            }
                        }
                        //获取MES名称
                        string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                        //获取MES类型
                        string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                        list.Add(GetParametricData(
                                                          ParametersMESName, // 标识符
                                                          ParametersMESType, // 数据类型
                                                          parametersPrice.ToString()// 数据值
                                                          ));
                        index++;
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
                }

            }
        }


        /// <summary>
        /// 出站数据收集
        /// </summary>
        /// <param name="temp">表头数组</param>
        /// <param name="state">参数判断</param>
        /// <param name="dataCollectForSfcEx">上传MES数组</param>
        public void PullOutDatasGain2(string[] temp, ref bool state, List<MachineIntegrationServiceService.machineIntegrationParametricData> list)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();
                //获取数据数量地址
                string DataQuantityPath = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[10].Value.ToString();
                //获取数据数量PLC类型
                string DataQuantityPathType = "";
                if (null != ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value)
                {
                    DataQuantityPathType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value.ToString();
                }
                //获取递增值
                string DataIncrementValue = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[12].Value.ToString();

                if (!string.IsNullOrEmpty(DataQuantityPath) && !string.IsNullOrEmpty(DataIncrementValue))
                {
                    uint dataCount;
                    switch (DataQuantityPathType)
                    {
                        case "usint":
                        default:
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_byte(DataQuantityPath);
                            break;
                        case "uint":
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_uint(DataQuantityPath);
                            break;
                    }
                    if (dataCount <= 0)
                    { continue; }
                    object TempParameters = ";";
                    for (int j = 0; j < dataCount; j++)
                    {
                        //Thread.Sleep(10);
                        object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);
                        if (parametersPrice.ToString() == "0")
                        {
                            parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);
                        }


                        bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                        if (whetherUploading)
                        {
                            if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                            {
                                if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                                {
                                    state = false;
                                    main.outDiary($"参数值超限：{parametersName}", "警告");

                                }

                            }
                            //获取MES名称
                            string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString() + (j + 1);

                            //获取MES类型
                            string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                            list.Add(GetParametricData(
                                                              ParametersMESName, // 标识符
                                                              ParametersMESType, // 数据类型
                                                              parametersPrice.ToString()// 数据值
                                                              ));
                            index++;
                        }
                        TempParameters += parametersPrice.ToString() + "，";
                        // 获取下一个数据的地址
                        string[] parametersAddressArray = parametersAddress.Split('.');

                        if (2 > parametersAddressArray.Length)
                        {
                            //Thread.Sleep(50);
                            continue;
                        }
                        parametersAddress = GetNewPath(parametersAddressArray, DataIncrementValue);
                    }
                    if (!string.IsNullOrEmpty(TempParameters.ToString()))
                    {
                        TempParameters = TempParameters.ToString().Substring(0, TempParameters.ToString().Length - 1);
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = TempParameters.ToString();
                }
                else
                {
                    object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);

                    if (parametersName.Contains("焊中检测结果"))
                    {
                        parametersPrice = parametersPrice.ToString() == "false" ? "0" : "1";
                    }
                    bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                    if (whetherUploading)
                    {
                        if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                        {
                            if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                            {
                                state = false;
                                main.outDiary($"参数值超限：{parametersName}", "警告");
                            }
                        }
                        //获取MES名称
                        string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                        //获取MES类型
                        string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                        list.Add(GetParametricData(
                                                          ParametersMESName, // 标识符
                                                          ParametersMESType, // 数据类型
                                                          parametersPrice.ToString()// 数据值
                                                          ));
                        index++;
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
                }

            }
        }

        /// <summary>
        /// 出站数据收集
        /// </summary>
        /// <param name="temp">表头数组</param>
        /// <param name="state">参数判断</param>
        /// <param name="dataCollectForSfcEx">上传MES数组</param>
        public void PullOutDatasGainReplace(string[] temp, ref bool state, Dictionary<string, string> keyValuePairs, List<MachineIntegrationServiceService.machineIntegrationParametricData> list)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();
                //获取数据数量地址
                string DataQuantityPath = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[10].Value.ToString();
                //获取数据数量PLC类型
                string DataQuantityPathType = "";
                if (null != ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value)
                {
                    DataQuantityPathType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value.ToString();
                }
                //获取递增值
                string DataIncrementValue = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[12].Value.ToString();

                if (!string.IsNullOrEmpty(DataQuantityPath) && !string.IsNullOrEmpty(DataIncrementValue))
                {
                    uint dataCount;
                    switch (DataQuantityPathType)
                    {
                        case "usint":
                        default:
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_byte(DataQuantityPath);
                            break;
                        case "uint":
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_uint(DataQuantityPath);
                            break;
                    }
                    if (dataCount <= 0)
                    { continue; }
                    // 替换本地值
                    string[] localValues = new string[dataCount];
                    if (keyValuePairs.ContainsKey(parametersName))
                    {
                        localValues = keyValuePairs[parametersName].Split('，');
                    }
                    object TempParameters = ";";
                    for (int j = 0; j < localValues.Length; j++)
                    {
                        object parametersPrice = localValues[j];
                        if (string.IsNullOrEmpty(localValues[j]) || "0".Equals(localValues[j]))
                        {
                            parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);
                            // 获取下一个数据的地址
                            string[] parametersAddressArray = parametersAddress.Split('.');

                            if (2 > parametersAddressArray.Length)
                            {
                                //Thread.Sleep(50);
                                continue;
                            }
                            parametersAddress = GetNewPath(parametersAddressArray, DataIncrementValue);
                        }
                        bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                        if (whetherUploading)
                        {
                            //获取MES名称
                            string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString() + (j + 1);

                            //获取MES类型
                            string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                            list.Add(GetParametricData(
                                                              ParametersMESName, // 标识符
                                                              ParametersMESType, // 数据类型
                                                              parametersPrice.ToString()// 数据值
                                                              ));
                            index++;
                        }
                        TempParameters += parametersPrice.ToString() + "，";
                    }
                    if (!string.IsNullOrEmpty(TempParameters.ToString()))
                    {
                        TempParameters = TempParameters.ToString().Substring(0, TempParameters.ToString().Length - 1);
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = TempParameters.ToString();
                }
                else
                {
                    object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);
                    // 替换本地值
                    if (keyValuePairs.ContainsKey(parametersName))
                    {
                        if (!string.IsNullOrEmpty(keyValuePairs[parametersName]) && !"0".Equals(keyValuePairs[parametersName]))
                        {
                            parametersPrice = keyValuePairs[parametersName];
                        }
                    }
                    if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                    {
                        if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                        {
                            state = false;
                            main.outDiary($"参数值超限：{parametersName}", "警告");
                        }
                    }

                    bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value);

                    if (whetherUploading)
                    {
                        //获取MES名称
                        string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                        //获取MES类型
                        string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                        list.Add(GetParametricData(
                                                          ParametersMESName, // 标识符
                                                          ParametersMESType, // 数据类型
                                                          parametersPrice.ToString()// 数据值
                                                          ));
                        index++;
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
                }

            }
        }

        public void InitialWorkpieceDatasGain(string[] temp, ref bool state, List<machineIntegrationParametricData> list)
        {
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();
                //获取数据地址
                var DataQuantityPath = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[10].Value.ToString();
                //获取数据数量PLC类型
                string DataQuantityPathType = "";
                if (null != ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value)
                {
                    DataQuantityPathType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value.ToString();
                }
                //获取递增值
                string DataIncrementValue = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[12].Value.ToString();

                if (!string.IsNullOrEmpty(DataQuantityPath) && !string.IsNullOrEmpty(DataIncrementValue))
                {
                    uint dataCount;
                    switch (DataQuantityPathType)
                    {
                        case "usint":
                        default:
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_byte(DataQuantityPath);
                            break;
                        case "uint":
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_uint(DataQuantityPath);
                            break;
                    }
                    if (dataCount <= 0)
                    { continue; }
                    object TempParameters = ";";
                    for (int j = 0; j < dataCount; j++)
                    {
                        object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);

                        bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[8].Value);

                        if (whetherUploading)
                        {
                            if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                            {
                                if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                                {
                                    state = false;
                                    main.outDiary($"参数值超限：{parametersName}", "警告");
                                }
                            }

                            //获取MES名称
                            string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString() + (j + 1);

                            //获取MES类型
                            string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                            list.Add(GetParametricDataInitialWorkpiece(
                                                              ParametersMESName, // 标识符
                                                              ParametersMESType, // 数据类型
                                                              parametersPrice.ToString()// 数据值
                                                              ));
                            index++;
                        }
                        TempParameters += parametersPrice.ToString() + "，";
                        // 获取下一个数据的地址
                        string[] pathArray = parametersAddress.Split('.');
                        if (2 > pathArray.Length)
                        {
                            continue;
                        }
                        parametersAddress = GetNewPath(pathArray, DataIncrementValue);
                    }
                    if (!string.IsNullOrEmpty(TempParameters.ToString()))
                    {
                        TempParameters = TempParameters.ToString().Substring(0, TempParameters.ToString().Length - 1);
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = TempParameters.ToString();
                }
                else
                {
                    object parametersPrice = ParametersJudge(parametersAddress, parametersAddressType);


                    bool whetherUploading = Convert.ToBoolean(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[8].Value);

                    if (whetherUploading)
                    {
                        if (parametersUpperLimit != "-" && parametersLowerLimit != "-")
                        {
                            if (Convert.ToDouble(parametersPrice) > Convert.ToDouble(parametersUpperLimit) || Convert.ToDouble(parametersPrice) < Convert.ToDouble(parametersLowerLimit))
                            {
                                state = false;
                                main.outDiary($"参数值超限：{parametersName}", "警告");
                            }
                        }
                        //获取MES名称
                        string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();

                        //获取MES类型
                        string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();

                        list.Add(GetParametricDataInitialWorkpiece(
                                                          ParametersMESName, // 标识符
                                                          ParametersMESType, // 数据类型
                                                          parametersPrice.ToString()// 数据值
                                                          ));
                        index++;
                    }
                    temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, parametersName)] = parametersPrice.ToString();
                }
            }
        }
        public string GetNewPath(string[] pathArray, string incrementValue)
        {
            int value = Convert.ToInt32(pathArray[1]) + Convert.ToInt32(incrementValue);
            return pathArray[0] + "." + value + "." + pathArray[2];
        }

        public void PullOutDatasAllGain(string[] temp, ref bool state, List<MachineIntegrationServiceService.machineIntegrationParametricData> list)
        {
            List<MES.TypeInfo> typeList = new List<MES.TypeInfo>();
            List<string> pathList = new List<string>();
            List<ushort> lengthList = new List<ushort>();
            int index = 0;
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                //获取表头名称
                string parametersName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString();
                //获取MES名称
                string ParametersMESName = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[1].Value.ToString();
                //获取MES类型
                string ParametersMESType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[2].Value.ToString();
                //获取PLC地址
                string parametersAddress = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[3].Value.ToString();
                //获取PLC地址类型
                string parametersAddressType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[4].Value.ToString();
                //获取上限值
                string parametersUpperLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[5].Value.ToString();
                //获取下限值
                string parametersLowerLimit = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[6].Value.ToString();
                //获取是否上传
                string isUploading = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[7].Value.ToString();
                //获取是否首件
                string isFrist = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[8].Value.ToString();
                //获取重复校验
                string isValidation = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[9].Value.ToString();
                //获取数据数量地址
                string DataQuantityPath = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[10].Value.ToString();
                //获取数据数量PLC类型
                string DataQuantityPathType = "";
                if (null != ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value)
                {
                    DataQuantityPathType = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[11].Value.ToString();
                }
                //获取递增值
                string DataIncrementValue = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[12].Value.ToString();

                if (!string.IsNullOrEmpty(DataQuantityPath) && !string.IsNullOrEmpty(DataIncrementValue))
                {
                    uint dataCount;
                    switch (DataQuantityPathType)
                    {
                        case "usint":
                        default:
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_byte(DataQuantityPath);
                            break;
                        case "uint":
                            dataCount = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_uint(DataQuantityPath);
                            break;
                    }
                    if (dataCount <= 0)
                    { continue; }

                    for (int j = 0; j < dataCount; j++)
                    {

                        MES.TypeInfo info = SetTypeInfo(parametersName, ParametersMESName, ParametersMESType, parametersAddress, parametersAddressType, parametersUpperLimit, parametersLowerLimit, isUploading, isFrist, isValidation);
                        typeList.Add(info);
                        pathList.Add(info.address);
                        lengthList.Add(info.Length);

                        // 获取下一个数据的地址
                        string[] parametersAddressArray = parametersAddress.Split('.');
                        if (2 > parametersAddressArray.Length)
                        {
                            continue;
                        }
                        parametersAddress = GetNewPath(parametersAddressArray, DataIncrementValue);
                    }
                }
                else
                {
                    MES.TypeInfo info = SetTypeInfo(parametersName, ParametersMESName, ParametersMESType, parametersAddress, parametersAddressType, parametersUpperLimit, parametersLowerLimit, isUploading, isFrist, isValidation);
                    typeList.Add(info);
                    pathList.Add(info.address);
                    lengthList.Add(info.Length);
                }

            }
            // 将所有PLC地址以及对应地址的数据类型长度一起发送给PLC，一次性回去出来，再做解析
            byte[] bytes = ResourceHandler.dparamParameters.siemensS7Net.PLC_Read_all(pathList.ToArray(), lengthList.ToArray());
            int offset = 0;
            foreach (var info in typeList)
            {
                info.value = ParseData(bytes, offset, info.Type);
                if (null == info.value)
                {
                    main.outDiary($"解析PLC数据失败，数据名称：{info.name}，数据地址：{info.address}，数据类型：{info.Type.Name}", "警告");
                }
                offset += info.Length;
            }
            // 根据名称分组，将名称一样的值汇总到一起
            var typeGroups = typeList.GroupBy(p => p.name);
            foreach (var groupInfo in typeGroups)
            {
                string tempValue = "";
                for (int i = 0; i < groupInfo.Count(); i++)
                {
                    var info = groupInfo.ElementAt(i);
                    if (info.upperLimit != "-" && info.lowerLimit != "-")
                    {
                        if (Convert.ToDouble(info.value) > Convert.ToDouble(info.upperLimit) || Convert.ToDouble(info.value) < Convert.ToDouble(info.lowerLimit))
                        {
                            state = false;
                            main.outDiary($"参数值超限：{info.name}", "警告");
                        }
                    }
                    bool whetherUploading = Convert.ToBoolean(info.isUploading);
                    if (whetherUploading)
                    {
                        list.Add(GetParametricData(
                                                          info.name_mes + (i + 1), // 标识符
                                                          info.type_mes, // 数据类型
                                                          info.value.ToString()// 数据值
                                                          ));
                        index++;
                    }
                    tempValue += info.value + "，";
                }
                if (!string.IsNullOrEmpty(tempValue))
                {
                    tempValue = tempValue.Substring(0, tempValue.Length - 1);
                }
                temp[DataGridViewClass.GetColumnsIndex(main.dataGridView2, groupInfo.Key)] = tempValue;
            }
        }

        static MES.TypeInfo SetTypeInfo(string parametersName, string ParametersMESName, string ParametersMESType, string parametersAddress, string parametersAddressType, string parametersUpperLimit, string parametersLowerLimit, string isUploading, string isFrist, string isValidation)
        {
            MES.TypeInfo info = new MES.TypeInfo();
            info.name = parametersName;
            info.address = parametersAddress;
            info.upperLimit = parametersUpperLimit;
            info.lowerLimit = parametersLowerLimit;
            info.name_mes = ParametersMESName;
            info.type_mes = ParametersMESType;
            info.isUploading = isUploading;
            info.isFrist = isFrist;
            info.isValidation = isValidation;

            switch (parametersAddressType)
            {
                case "BOOL":
                    info.Type = typeof(bool);
                    info.Length = 1;
                    break;
                case "BYTE":
                default:
                    info.Type = typeof(byte);
                    info.Length = 1;
                    break;
                case "FLOAT":
                    info.Type = typeof(float);
                    info.Length = 4;
                    break;
                case "INT":
                    info.Type = typeof(int);
                    info.Length = 4;
                    break;
                case "SHORT":
                    info.Type = typeof(short);
                    info.Length = 2;
                    break;
                case "STRING":
                    info.Type = typeof(string);
                    info.Length = 50;
                    break;
            }
            return info;
        }
        object ParseData(byte[] bytes, int offset, Type type)
        {
            if (type == typeof(bool))
            {
                return Convert.ChangeType(BitConverter.ToBoolean(bytes, offset), type);
            }
            else if (type == typeof(byte))
            {
                return Convert.ChangeType(bytes[offset], type);
            }
            else if (type == typeof(float))
            {
                return Convert.ChangeType(BitConverter.ToSingle(bytes, offset), type);
            }
            else if (type == typeof(int))
            {
                return Convert.ChangeType(BitConverter.ToInt32(bytes, offset), type);
            }
            else if (type == typeof(short))
            {
                return Convert.ChangeType(BitConverter.ToInt16(bytes, offset), type);
            }
            else if (type == typeof(string))
            {
                var head = BitConverter.ToUInt16(bytes, offset + 1);
                byte recID = (byte)(head >> 8 & 0xff);
                byte len = (byte)(head & 0xff);
                string value = Encoding.ASCII.GetString(bytes, offset, len + 2).Substring(2, len);
                value = value == null ? "" : value;
                return value;
            }
            return null;
        }

    }
}
