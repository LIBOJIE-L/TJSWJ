using DataModel;
using HJMSurrenSystem.Parameters;
using MachineIntegrationServiceService;
using MiFindCustomAndSfcDataServiceService;
using System;
using System.Net;

namespace HJMSurrenSystem.MES
{
    public class MesInteraction
    {
        Main main;
        public MesInteraction(Main main ) { 
            this.main = main;
            ResourceHandler.dparamParameters.Read_MESCodeCSV = new Read_MESCodeCSV();
        }

        public modeProcessSFC PullInGetEnum(string tempStr)
        {
            if (tempStr == "MODE_NONE")
                return modeProcessSFC.MODE_NONE;
            else if (tempStr == "MODE_COMPLETE_SFC_POST_DC")
                return modeProcessSFC.MODE_START_COMPLETE_SFC;
            else if (tempStr == "MODE_PASS_FSC_POST_DC")
                return modeProcessSFC.MODE_PASS_SFC;
            else if (tempStr == "MODE_START_SFC")
                return modeProcessSFC.MODE_START_SFC;
            else if (tempStr == "MODE_COMPLETE_SFC")
                return modeProcessSFC.MODE_COMPLETE_SFC;
            return modeProcessSFC.MODE_NONE;
        }

        public MiFindCustomAndSfcDataServiceService.ObjectAliasEnum PullInGetObjectAliasEnum(string tempStr) {
            if (tempStr == "ACTIVITY")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ACTIVITY;
            else if (tempStr == "ACTIVITY_GROUP")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ACTIVITY_GROUP;
            else if (tempStr == "ACTIVITY_LOG")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ACTIVITY_LOG;
            else if (tempStr == "ALARM")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ALARM;
            else if (tempStr == "ALARM_LOG")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ALARM_LOG;
            else if (tempStr == "APPLICATION_SETTING")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.APPLICATION_SETTING;
            else if (tempStr == "ATTACHMENT")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ATTACHMENT;
            else if (tempStr == "ATTENDANCE_LOG")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ATTENDANCE_LOG;
            else if (tempStr == "BACKGROUND_PROCESS")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.BACKGROUND_PROCESS;
            else if (tempStr == "BOM")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.BOM;
            else if (tempStr == "BOM_COMPONENT")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.BOM_COMPONENT;
            else if (tempStr == "BUYOFF")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.BUYOFF;
            else if (tempStr == "BUYOFF_LOG")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.BUYOFF_LOG;
            else if (tempStr == "CERTIFICATION")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.CERTIFICATION;
            else if (tempStr == "CNC_PROGRAM")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.CNC_PROGRAM;
            else if (tempStr == "CONTAINER")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.CONTAINER;
            else if (tempStr == "CONTAINER_DATA")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.CONTAINER_DATA;
            else if (tempStr == "COST_CENTER")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.COST_CENTER;
            else if (tempStr == "CUSTOMER")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.CUSTOMER;
            else if (tempStr == "DATA_FIELD")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.DATA_FIELD;
            else if (tempStr == "DATA_TYPE")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.DATA_TYPE;
            else if (tempStr == "DC_GROUP")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.DC_GROUP;
            else if (tempStr == "DOCUMENT")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.DOCUMENT;
            else if (tempStr == "INVENTORY")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.INVENTORY;
            else if (tempStr == "INVENTORY_LOG")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.INVENTORY_LOG;
            else if (tempStr == "ITEM_GROUP")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ITEM_GROUP;
            else if (tempStr == "LABOR_CHARGE_CODE")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.LABOR_CHARGE_CODE;
            else if (tempStr == "MESSAGE")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.MESSAGE;
            else if (tempStr == "MESSAGE_LOG")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.MESSAGE_LOG;
            else if (tempStr == "MESSAGE_TYPE")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.MESSAGE_TYPE;
            else if (tempStr == "NC_CODE")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.NC_CODE;
            else if (tempStr == "NEXT_NUMBER")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.NEXT_NUMBER;
            else if (tempStr == "OPERATION")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.OPERATION;
            else if (tempStr == "REASON_CODE")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.REASON_CODE;
            else if (tempStr == "SAMPLE_PLAN")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.SAMPLE_PLAN;
            else if (tempStr == "WORK_INSTRUCTION")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.WORK_INSTRUCTION;
            else if (tempStr == "RESOURCE")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.RESOURCE;
            else if (tempStr == "ROUTER")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ROUTER;
            else if (tempStr == "SHOP_ORDER")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.SHOP_ORDER;
            else if (tempStr == "ROUTER_STEP")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ROUTER_STEP;
            else if (tempStr == "ROUTER_OPERATION")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ROUTER_OPERATION;
            else if (tempStr == "SFC")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.SFC;
            else if (tempStr == "USR")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.USR;
            else if (tempStr == "USER_GROUP")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.USER_GROUP;
            else if (tempStr == "WORK_CENTER")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.WORK_CENTER;
            else if (tempStr == "WORKSTATION")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.WORKSTATION;
            else if (tempStr == "RESOURCE_TYPE")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.RESOURCE_TYPE;
            else if (tempStr == "CUSTOMER_ORDER")
                return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.CUSTOMER_ORDER;
            return MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ITEM;
        }

        public ModeProcessSfc PullOutGetEnum(string tempStr)
        {
            if (tempStr == "MODE_NONE")
                return ModeProcessSfc.MODE_NONE;
            else if (tempStr == "MODE_START_SFC_PRE_DC")
                return ModeProcessSfc.MODE_START_SFC_PRE_DC;
            else if (tempStr == "MODE_COMPLETE_SFC_POST_DC")
                return ModeProcessSfc.MODE_COMPLETE_SFC_POST_DC;
            else if (tempStr == "MODE_PASS_SFC_POST_DC")
                return ModeProcessSfc.MODE_PASS_SFC_POST_DC;
            return ModeProcessSfc.MODE_NONE;
        }

        public ResponseData PullIn(string 模组码)
        {
            MiFindCustomAndSfcDataServiceService.MiFindCustomAndSfcDataServiceService serviceInDll = new MiFindCustomAndSfcDataServiceService.MiFindCustomAndSfcDataServiceService();

            NetworkCredential dential = new NetworkCredential();

            dential.UserName = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1,"userName");
            dential.Password = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "password"); 

            serviceInDll.PreAuthenticate = true;
            serviceInDll.Credentials = dential;

            serviceInDll.Url = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "url");
            serviceInDll.Timeout = int.Parse(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "timeout"));

            DataGridViewClass.Write_MESLOG_CSV(new string[] { "网址：," + serviceInDll.Url }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "耗时：," + serviceInDll.Timeout }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "当前时间：," + DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");


            miFindCustomAndSfcData dataIn = new miFindCustomAndSfcData();
            dataIn.FindCustomAndSfcDataRequest = new findCustomAndSfcDataRequest();

            dataIn.FindCustomAndSfcDataRequest.user = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "user");
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "user:," + dataIn.FindCustomAndSfcDataRequest.user }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.resource = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "resource");
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "resource:," + dataIn.FindCustomAndSfcDataRequest.resource }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.site = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "site");
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "site:," + dataIn.FindCustomAndSfcDataRequest.site }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.activity = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "activity");
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "activity:," + dataIn.FindCustomAndSfcDataRequest.activity }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.modeProcessSFC = (PullInGetEnum(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "modeProcessSfc")));
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "modeProcessSfc:," + dataIn.FindCustomAndSfcDataRequest.modeProcessSFC }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.customDataArray = new customDataInParametricData[1];
            dataIn.FindCustomAndSfcDataRequest.customDataArray[0] = new customDataInParametricData();

            dataIn.FindCustomAndSfcDataRequest.customDataArray[0].category = PullInGetObjectAliasEnum(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "category"));
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "category:," + dataIn.FindCustomAndSfcDataRequest.customDataArray[0].category }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.customDataArray[0].dataField = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "dataField");
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "dataField:," + dataIn.FindCustomAndSfcDataRequest.customDataArray[0].dataField }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.operation = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "operation");
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "operation:," + dataIn.FindCustomAndSfcDataRequest.operation }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.operationRevision = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullInUI.dataGridView1, "operationRevision");
             DataGridViewClass.Write_MESLOG_CSV(new string[] { "operationRevision:," + dataIn.FindCustomAndSfcDataRequest.operationRevision }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            dataIn.FindCustomAndSfcDataRequest.findSfcByInventory = false;
            dataIn.FindCustomAndSfcDataRequest.masterDataArray = new MiFindCustomAndSfcDataServiceService.ObjectAliasEnum[] { MiFindCustomAndSfcDataServiceService.ObjectAliasEnum.ITEM };
            dataIn.FindCustomAndSfcDataRequest.sfc = 模组码;
            DataGridViewClass.Write_MESLOG_CSV(new string[] { "sfc:," + dataIn.FindCustomAndSfcDataRequest.sfc }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

            #region 调用接口
            miFindCustomAndSfcDataResponse responseIn;
            ResponseData reData = new ResponseData();
            reData.code = -1;
            reData.startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");

            reData.sfc = 模组码;
            try
            {
                responseIn = serviceInDll.miFindCustomAndSfcData(dataIn);//此处调试所以禁用进站方法 使用延时模拟进站

                reData.code = responseIn.@return.code;
                reData.message = responseIn.@return.message;
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + responseIn.@return.code +
                                                                       "Message:" + responseIn.@return.message +
                    "}"
                                                                }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

                if (responseIn.@return.code != 0 || responseIn.@return.message != "")
                {
                    main.outDiary("【MES】MES进站审核失败\r\n【MES】Code：" + responseIn.@return.code + "\r\n【MES】message:" + responseIn.@return.message.ToString() + "\r\n\r\n", "警告");

                    CSV_Style message = ResourceHandler.dparamParameters.Read_MESCodeCSV.Find_Code(responseIn.@return.code.ToString());
                    if (message != null)
                    {
                        main.outDiary("Code:" + message.Code +
                                    "\r\nMessage:" + responseIn.@return.message.ToString() +
                                    "\r\n原因：" + message.Message +
                                    "\r\n解决办法：" + message.way +
                                    "\r\n处理人员：" + message.PersoninCharge +
                                    "\r\nMES审核失败", "警告");
                        reData.code = Convert.ToInt16(message.Code);
                        reData.Message = message.Message;
                        reData.way = message.way;
                        reData.personinCharge = message.PersoninCharge;
                    }
                    else
                    {
                        main.outDiary("Code:" + responseIn.@return.code.ToString() + "\r\n" + "不存在于当前的报错文档中！请提供文档进行更新" +
                            "\r\nMES审核失败", "警告");
                        reData.code = Convert.ToInt16(responseIn.@return.code);
                        reData.Message = "不存在于当前的报错文档中！请提供文档进行更新";
                    }
                    return reData;
                }
                main.outDiary("【MES】数据已接收！\r\n【MES】MES进站成功!\r\n\r\n", "信息");

                reData.type = responseIn.@return.customDataArray[0].dataAttribute;
                reData.pnCode = responseIn.@return.masterDataArray[0].value;
                reData.sfc = responseIn.@return.sfc;

                return reData;
            }
            catch (Exception ex)
            {
                reData.sfc = "";
                reData.message = ex.Message;
                reData.code = 9999;

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + reData.code +
                                                                       "Message:" + reData.message +
                    "}"
                                                                }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\findCustomAndSfcDataRequest", "进站");

                return reData;
            }
            #endregion
        }

        public ResponseData PullOut(string 模组码, dataCollectForSfcEx dataCollectForSfcEx)
        {
            var serviceOutDll = new MachineIntegrationServiceService.MachineIntegrationServiceService();

            NetworkCredential dential = new NetworkCredential();

            MachineIntegrationServiceService.nonConfirmCodeArray[] codeArray =
               new MachineIntegrationServiceService.nonConfirmCodeArray[1];

            try
            {
                dential.UserName = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "userName");
                dential.Password = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "password");

                serviceOutDll.PreAuthenticate = true;
                serviceOutDll.Credentials = dential;

                serviceOutDll.Url = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "url");
                serviceOutDll.Timeout = int.Parse(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "timeout"));    //服务器连接超时设置，毫秒 

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "网址：," + serviceOutDll.Url }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "耗时：," + serviceOutDll.Timeout }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "当前时间：," + DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.ncCodeArray = codeArray;
                dataCollectForSfcEx.SfcDcExRequest.site = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "site");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "site：," + dataCollectForSfcEx.SfcDcExRequest.site }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.user = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "user");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "user：," + dataCollectForSfcEx.SfcDcExRequest.user }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.operation = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "operation");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "operation：," + dataCollectForSfcEx.SfcDcExRequest.operation }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.operationRevision = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "operationRevision");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "operationRevision：," + dataCollectForSfcEx.SfcDcExRequest.operationRevision }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.dcGroup = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "dcGroup");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "dcGroup：," + dataCollectForSfcEx.SfcDcExRequest.dcGroup }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.resource = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "resource");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "resource：," + dataCollectForSfcEx.SfcDcExRequest.resource }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.sfc = 模组码;//出站条码 
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "sfc：," + 模组码 }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.dcGroupRevision = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "dcGroupRevision");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "dcGroupRevision：," + dataCollectForSfcEx.SfcDcExRequest.dcGroupRevision }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.activityId = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "activityId");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "activityId：," + dataCollectForSfcEx.SfcDcExRequest.activityId }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                dataCollectForSfcEx.SfcDcExRequest.modeProcessSfc = (ModeProcessSfc)PullOutGetEnum(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "modeProcessSfc"));
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "modeProcessSfc：," + dataCollectForSfcEx.SfcDcExRequest.modeProcessSfc }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

            }
            catch (Exception)
            {
                main.outDiary("MES配置参数异常!", "错误");
                return null;
            }
            main.outDiary("出站模组码：" + 模组码, "信息");
                
            string parametr = "";

            DataGridViewClass.Write_MESLOG_CSV(new string[] { "parametr：," + parametr }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");
            foreach (var item in dataCollectForSfcEx.SfcDcExRequest.parametricDataArray)
            {
                if (item == null)
                {
                    break;
                }
                parametr += "{" + $"{item.name}:{item.dataType}:{item.value} " + "}";
            }

            DataGridViewClass.Write_MESLOG_CSV(new string[] { "parametr：," + parametr }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

            dataCollectForSfcExResponse responseIn;
            ResponseData reData = new ResponseData();
            reData.code = -1;
            reData.sfc = 模组码;
            try
            {
                responseIn = serviceOutDll.dataCollectForSfcEx(dataCollectForSfcEx);//此处为调试所以禁用出站方法，使用延时模拟出站

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + responseIn.@return.code +
                                                                       "Message:" + responseIn.@return.message +
                                                                       "}"
                                                                }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");
                reData.code = responseIn.@return.code;
                reData.message = responseIn.@return.message;
                if (responseIn.@return.code != 0 || responseIn.@return.message != "")
                {
                    main.outDiary("【MES】MES出站审核失败\r\n【MES】Code：" + responseIn.@return.code + "\r\n【MES】message:" + responseIn.@return.message.ToString() + "\r\n\r\n", "警告");

                    CSV_Style message = ResourceHandler.dparamParameters.Read_MESCodeCSV.Find_Code(responseIn.@return.code.ToString());
                    if (message != null)
                    {
                        main.outDiary("Code:" + message.Code +
                                    "\r\nMessage:" + responseIn.@return.message.ToString() +
                                    "\r\n原因：" + message.Message +
                                    "\r\n解决办法：" + message.way +
                                    "\r\n处理人员：" + message.PersoninCharge +
                                    "\r\nMES审核失败","警告");

                        reData.code = Convert.ToInt16(message.Code);
                        reData.Message = message.Message;
                        reData.way = message.way;
                        reData.personinCharge = message.PersoninCharge;
                    }
                    else
                    {
                        main.outDiary("Code:" + responseIn.@return.code.ToString() + "\r\n" + "不存在于当前的报错文档中！请提供文档进行更新" +
                            "\r\nMES审核失败", "警告");
                        reData.code = Convert.ToInt16(responseIn.@return.code);
                        reData.Message = "不存在于当前的报错文档中！请提供文档进行更新";
                    }
                    return reData;
                }
                main.outDiary("【MES】数据已接收！\r\n【MES】MES出站成功!\r\n\r\n", "信息");

                return reData;
            }
            catch (Exception ex)
            {
                reData.code = 9999;
                reData.message = ex.Message;
                main.outDiary(ex.Message, "信息");

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + reData.code +
                                                                       "Message:" + reData.message +
                                                                       "}"
                                                                }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiFindCustomAndSfcDataServiceService", "出站");

                return reData;
            }
        }

        public ResponseData InitialWorkpiece(string 模组码, dataCollectForResourceFAI dataCollectForResourceFAI) {
            var serviceOutDll = new DataCollectForResourceFAIServiceService();

            NetworkCredential dential = new NetworkCredential();

            try
            {
                dential.UserName = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "userName");
                dential.Password = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "password");

                serviceOutDll.PreAuthenticate = true;
                serviceOutDll.Credentials = dential;

                serviceOutDll.Url = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "url");
                serviceOutDll.Timeout = int.Parse(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "timeout"));    //服务器连接超时设置，毫秒 

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "网址：," + serviceOutDll.Url }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "耗时：," + serviceOutDll.Timeout }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "当前时间：," + DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.site = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "site");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "site：," + dataCollectForResourceFAI.resourceRequest.site }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.user = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "user");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "user：," + dataCollectForResourceFAI.resourceRequest.user }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.operation = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "operation");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "operation：," + dataCollectForResourceFAI.resourceRequest.operation }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.operationRevision = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "operationRevision");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "operationRevision：," + dataCollectForResourceFAI.resourceRequest.operationRevision }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.dcGroup = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "dcGroup");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "dcGroup：," + dataCollectForResourceFAI.resourceRequest.dcGroup }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.dcGroupRevision = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "dcGroupRevision");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "dcGroupRevision：," + dataCollectForResourceFAI.resourceRequest.dcGroupRevision }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.dcMode = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "dcMode");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "dcMode：," + dataCollectForResourceFAI.resourceRequest.dcMode }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.sfc = 模组码;//出站条码 
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "sfc：," + 模组码 }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.resource = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "resource");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "resource：," + dataCollectForResourceFAI.resourceRequest.resource }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.dcGroupSequence = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "dcGroupSequence");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "dcGroupSequence：," + dataCollectForResourceFAI.resourceRequest.dcGroupSequence}, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.material = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "material");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "material：," + dataCollectForResourceFAI.resourceRequest.material }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

                dataCollectForResourceFAI.resourceRequest.materialRevision = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "materialRevision");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "materialRevision：," + dataCollectForResourceFAI.resourceRequest.materialRevision}, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

            }
            catch (Exception)
            {
                main.outDiary("MES配置参数异常!", "错误");
                return null;
            }
            main.outDiary("首件模组码：" + 模组码, "信息");

            string parametr = "";

            foreach (var item in dataCollectForResourceFAI.resourceRequest.parametricDataArray)
            {
                if (item == null)
                {
                    break;
                }
                parametr += "{" + $"{item.name}:{item.dataType}:{item.value} " + "}";
            }

            DataGridViewClass.Write_MESLOG_CSV(new string[] { "parametr：," + parametr }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");

            dataCollectForResourceFAIResponse responseIn;
            ResponseData reData = new ResponseData();
            reData.code = -1;
            reData.sfc = 模组码;
            try
            {
                responseIn = serviceOutDll.dataCollectForResourceFAI(dataCollectForResourceFAI);//此处为调试所以禁用出站方法，使用延时模拟出站

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + responseIn.@return.code +
                                                                       "Message:" + responseIn.@return.message +
                                                                       "}"
                                                                }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "首件");
                reData.code = responseIn.@return.code;
                reData.message = responseIn.@return.message;
                if (responseIn.@return.code != 0 || responseIn.@return.message != "")
                {
                    main.outDiary("【MES】MES首件出站审核失败\r\n【MES】Code：" + responseIn.@return.code + "\r\n【MES】message:" + responseIn.@return.message.ToString() + "\r\n\r\n", "警告");

                    CSV_Style message = ResourceHandler.dparamParameters.Read_MESCodeCSV.Find_Code(responseIn.@return.code.ToString());
                    if (message != null)
                    {
                        main.outDiary("Code:" + message.Code +
                                    "\r\nMessage:" + responseIn.@return.message.ToString() +
                                    "\r\n原因：" + message.Message +
                                    "\r\n解决办法：" + message.way +
                                    "\r\n处理人员：" + message.PersoninCharge +
                                    "\r\nMES审核失败", "警告");

                        reData.code = Convert.ToInt16(message.Code);
                        reData.Message = message.Message;
                        reData.way = message.way;
                        reData.personinCharge = message.PersoninCharge;
                    }
                    else
                    {
                        main.outDiary("Code:" + responseIn.@return.code.ToString() + "\r\n" + "不存在于当前的报错文档中！请提供文档进行更新" +
                            "\r\nMES审核失败", "警告");
                        reData.code = Convert.ToInt16(responseIn.@return.code);
                        reData.Message = "不存在于当前的报错文档中！请提供文档进行更新";
                    }
                    return reData;
                }
                main.outDiary("【MES】数据已接收！\r\n【MES】MES首件出站成功!\r\n\r\n", "信息");

                return reData;
            }
            catch (Exception ex)
            {
                reData.code = 9999;
                reData.message = ex.Message;
                main.outDiary(ex.Message, "信息");

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + reData.code +
                                                                       "Message:" + reData.message +
                                                                       "}"
                                                                }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "出站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "出站");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\DataCollectForResourceFAIServiceService", "出站");

                return reData;
            }
        }

        public ResponseData DX_verify(string 模组码, miSFCAttriDataEntryRequest miSFCAttriDataEntryRequest)
        {
            var serviceOutDll = new MiSFCAttriDataEntryServiceService();

            NetworkCredential dential = new NetworkCredential();

            try
            {
                dential.UserName = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "userName");
                dential.Password = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "password");

                serviceOutDll.PreAuthenticate = true;
                serviceOutDll.Credentials = dential;

                serviceOutDll.Url = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "url");
                serviceOutDll.Timeout = int.Parse(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "timeout"));    //服务器连接超时设置，毫秒 

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "网址：," + serviceOutDll.Url }, ResourceHandler.listSystemParameters[0].MESLogPath + ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "耗时：," + serviceOutDll.Timeout }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "首件");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "当前时间：," + DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");

                miSFCAttriDataEntryRequest.site = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "site");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "site：," + miSFCAttriDataEntryRequest.site }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");

                miSFCAttriDataEntryRequest.userId = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "userId");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "userId：," + miSFCAttriDataEntryRequest.userId }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");

                miSFCAttriDataEntryRequest.sfcMode = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "sfcMode");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "sfcMode：," + miSFCAttriDataEntryRequest.sfcMode }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");

                miSFCAttriDataEntryRequest.sfc = 模组码;
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "sfc：," + miSFCAttriDataEntryRequest.sfc }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");

                miSFCAttriDataEntryRequest.isCheckSequence = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, "isCheckSequence");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "isCheckSequence：," + miSFCAttriDataEntryRequest.isCheckSequence }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");

                string parametr = "";
                foreach (var item in miSFCAttriDataEntryRequest.sfcDatalist)
                {
                    if (item == null)
                    {
                        break;
                    }
                    parametr += "{" + $"{item.value}:{item.attributes}:{item.sequence} " + "}";
                }

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "sfcDatalist：," + miSFCAttriDataEntryRequest.sfcDatalist }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");

                //itemGroup:电芯物料组
                miSFCAttriDataEntryRequest.itemGroup = "FC-FL;FC-NL";

            }
            catch (Exception)
            {
                main.outDiary("MES配置参数异常!", "错误");
                return null;
            }
            miSfcAttriDataEntry entry = new miSfcAttriDataEntry();
            entry.MiSFCAttriDataEntryRequest = miSFCAttriDataEntryRequest;

            miSfcAttriDataEntryResponse responseIn;
            ResponseData reData = new ResponseData();
            reData.code = -1;
            reData.sfc = 模组码;
            try
            {
                responseIn = serviceOutDll.miSfcAttriDataEntry(entry);//此处为调试所以禁用出站方法，使用延时模拟出站

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + responseIn.@return.code +
                                                                       "Message:" + responseIn.@return.message +
                                                                       "}"
                                                                }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");
                reData.code = responseIn.@return.code;
                reData.message = responseIn.@return.message;
                if (responseIn.@return.code != 0 || responseIn.@return.message != "")
                {
                    main.outDiary("【MES】MES电芯校验失败\r\n【MES】Code：" + responseIn.@return.code + "\r\n【MES】message:" + responseIn.@return.message.ToString() + "\r\n\r\n", "警告");

                    CSV_Style message = ResourceHandler.dparamParameters.Read_MESCodeCSV.Find_Code(responseIn.@return.code.ToString());
                    if (message != null)
                    {
                        main.outDiary("Code:" + message.Code +
                                    "\r\nMessage:" + responseIn.@return.message.ToString() +
                                    "\r\n原因：" + message.Message +
                                    "\r\n解决办法：" + message.way +
                                    "\r\n处理人员：" + message.PersoninCharge +
                                    "\r\nMES审核失败", "警告");

                        reData.code = Convert.ToInt16(message.Code);
                        reData.Message = message.Message;
                        reData.way = message.way;
                        reData.personinCharge = message.PersoninCharge;
                    }
                    else
                    {
                        main.outDiary("Code:" + responseIn.@return.code.ToString() + "\r\n" + "不存在于当前的报错文档中！请提供文档进行更新" +
                            "\r\nMES审核失败", "警告");
                        reData.code = Convert.ToInt16(responseIn.@return.code);
                        reData.Message = "不存在于当前的报错文档中！请提供文档进行更新";
                    }
                    return reData;
                }
                main.outDiary("【MES】数据已接收！\r\n【MES】MES电芯校验成功!\r\n\r\n", "信息");

                return reData;
            }
            catch (Exception ex)
            {
                reData.code = 9999;
                reData.message = ex.Message;
                main.outDiary(ex.Message, "信息");

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + reData.code +
                                                                       "Message:" + reData.message +
                                                                       "}"
                                                                }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiSFCAttriDataEntryServiceService", "电芯校验");

                return reData;
            }
        }
    }

    public class ResponseData
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime;
        /// <summary>
        /// 报错代码
        /// </summary>
        public int code;
        /// <summary>
        /// 错误原因
        /// </summary>
        public string message;
        /// <summary>
        /// 条码
        /// </summary>
        public string sfc;
        /// <summary>
        /// PN码
        /// </summary>
        public string pnCode;
        /// <summary>
        /// 类型
        /// </summary>
        public string type;
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime;
        /// <summary>
        /// 原因
        /// </summary>
        public string Message;
        /// <summary>
        /// 解决方案
        /// </summary>
        public string way;
        /// <summary>
        /// 处理人员
        /// </summary>
        public string personinCharge;
    }

}
