using DataModel;
using HJMSurrenSystem.MES.MiCheckBOMInventoryProxy;
using HJMSurrenSystem.Parameters;
using MachineIntegrationServiceService;
using MiFindCustomAndSfcDataServiceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace HJMSurrenSystem.MES
{
    extern alias MiAssembleService;

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

        public dataCollectForSfcModeProcessSfc PullOutGetEnum(string tempStr)
        {
            if (tempStr == "MODE_NONE")
                return dataCollectForSfcModeProcessSfc.MODE_NONE;
            else if (tempStr == "MODE_START_SFC_PRE_DC")
                return dataCollectForSfcModeProcessSfc.MODE_START_SFC_PRE_DC;
            else if (tempStr == "MODE_COMPLETE_SFC_POST_DC")
                return dataCollectForSfcModeProcessSfc.MODE_COMPLETE_SFC_POST_DC;
            else if (tempStr == "MODE_PASS_SFC_POST_DC")
                return dataCollectForSfcModeProcessSfc.MODE_PASS_SFC_POST_DC;
            return dataCollectForSfcModeProcessSfc.MODE_NONE;
        }

        private static string ReadInterfaceParameter(
            List<MesPullInParameters> parameters,
            string parameterName,
            string defaultValue = "")
        {
            MesPullInParameters parameter = parameters.FirstOrDefault(item =>
                string.Equals(item.ParametersName, parameterName, StringComparison.Ordinal));
            return string.IsNullOrWhiteSpace(parameter.ParametersPrice)
                ? defaultValue
                : parameter.ParametersPrice.Trim();
        }

        private static bool ReadBooleanParameter(
            List<MesPullInParameters> parameters,
            string parameterName,
            bool defaultValue)
        {
            string value = ReadInterfaceParameter(parameters, parameterName, defaultValue ? "true" : "false");
            if (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("是", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (value.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("0", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("否", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new InvalidOperationException(parameterName + "必须配置为true或false");
        }

        private bool IsBomInventoryCheckEnabled()
        {
            return ReadBooleanParameter(ResourceHandler.listMesBomInventoryParameters, "enabled", true);
        }

        private bool IsAssembleMaterialEnabled()
        {
            return ReadBooleanParameter(ResourceHandler.listMesAssembleMaterialParameters, "enabled", true);
        }

        private static string ToServiceUrl(string wsdlUrl)
        {
            string url = (wsdlUrl ?? "").Trim();
            int wsdlIndex = url.IndexOf("?wsdl", StringComparison.OrdinalIgnoreCase);
            return wsdlIndex >= 0 ? url.Substring(0, wsdlIndex) : url;
        }

        private static CheckBOMInventoryParameter[] ParseBomParameterArray(List<MesPullInParameters> parameters)
        {
            string configuredArray = ReadInterfaceParameter(parameters, "parameterArray[]");
            List<CheckBOMInventoryParameter> result = new List<CheckBOMInventoryParameter>();

            if (!string.IsNullOrWhiteSpace(configuredArray))
            {
                foreach (string item in configuredArray.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] parts = item.Split(new[] { '|' }, 3);
                    if (parts.Length != 3 || parts.Any(string.IsNullOrWhiteSpace))
                    {
                        throw new InvalidOperationException("parameterArray[]格式必须为 usage|category|dataField");
                    }

                    result.Add(new CheckBOMInventoryParameter
                    {
                        usage = parts[0].Trim(),
                        category = parts[1].Trim(),
                        dataField = parts[2].Trim()
                    });
                }
                return result.ToArray();
            }

            string[] usages = ReadInterfaceParameter(parameters, "usage")
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] categories = ReadInterfaceParameter(parameters, "category")
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] dataFields = ReadInterfaceParameter(parameters, "dataField")
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (usages.Length == 0 || usages.Length != categories.Length || usages.Length != dataFields.Length)
            {
                throw new InvalidOperationException("usage、category、dataField的项目数量必须一致且不能为空");
            }

            for (int i = 0; i < usages.Length; i++)
            {
                result.Add(new CheckBOMInventoryParameter
                {
                    usage = usages[i].Trim(),
                    category = categories[i].Trim(),
                    dataField = dataFields[i].Trim()
                });
            }
            return result.ToArray();
        }

        private static string ResolveAssemblyValue(string value, string moduleCode)
        {
            return (value ?? "").Replace("{SFC}", moduleCode ?? "");
        }

        private MiAssembleService::miInventoryData[] ParseAssemblyInventoryArray(string configuredValue, string moduleCode)
        {
            List<MiAssembleService::miInventoryData> inventoryList = new List<MiAssembleService::miInventoryData>();
            if (string.IsNullOrWhiteSpace(configuredValue))
            {
                return inventoryList.ToArray();
            }

            foreach (string rawItem in configuredValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = rawItem.Split(new[] { '|' }, 3);
                string inventory = ResolveAssemblyValue(parts[0].Trim(), moduleCode);
                if (string.IsNullOrWhiteSpace(inventory))
                {
                    throw new InvalidOperationException("inventoryArray[]中存在空库存号");
                }

                List<MiAssembleService::AssemblyDataField> fields = new List<MiAssembleService::AssemblyDataField>();
                if (parts.Length >= 3 && !string.IsNullOrWhiteSpace(parts[2]))
                {
                    int sequence = 1;
                    foreach (string rawField in parts[2].Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] fieldParts = rawField.Split(new[] { '=' }, 2);
                        if (fieldParts.Length != 2 || string.IsNullOrWhiteSpace(fieldParts[0]))
                        {
                            throw new InvalidOperationException("库存属性格式必须为 属性名=属性值");
                        }

                        fields.Add(new MiAssembleService::AssemblyDataField
                        {
                            sequence = sequence++,
                            sequenceSpecified = true,
                            attribute = fieldParts[0].Trim(),
                            value = ResolveAssemblyValue(fieldParts[1].Trim(), moduleCode)
                        });
                    }
                }

                inventoryList.Add(new MiAssembleService::miInventoryData
                {
                    inventory = inventory,
                    qty = parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[1]) ? parts[1].Trim() : "1",
                    assemblyDataFields = fields.ToArray()
                });
            }

            return inventoryList.ToArray();
        }

        private MiAssembleService::machineIntegrationParametricData[] ParseAssemblyParameterArray(string configuredValue, string moduleCode)
        {
            List<MiAssembleService::machineIntegrationParametricData> parameterList = new List<MiAssembleService::machineIntegrationParametricData>();
            if (string.IsNullOrWhiteSpace(configuredValue))
            {
                return parameterList.ToArray();
            }

            foreach (string rawItem in configuredValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = rawItem.Split(new[] { '|' }, 3);
                if (parts.Length != 3 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    throw new InvalidOperationException("parameterArray[]格式必须为 名称|类型|值");
                }

                MiAssembleService::ParameterDataType dataType;
                if (!Enum.TryParse(parts[1].Trim(), true, out dataType) || !Enum.IsDefined(typeof(MiAssembleService::ParameterDataType), dataType))
                {
                    throw new InvalidOperationException("组装物料DC参数类型仅支持NUMBER、TEXT、FORMULA、BOOLEAN");
                }

                parameterList.Add(new MiAssembleService::machineIntegrationParametricData
                {
                    name = parts[0].Trim(),
                    dataType = dataType,
                    value = ResolveAssemblyValue(parts[2].Trim(), moduleCode)
                });
            }

            return parameterList.ToArray();
        }

        private MiAssembleService::nonConfirmCodeArray[] ParseAssemblyNcCodeArray(
            string ncCodeValue,
            string hasNcValue,
            string moduleCode)
        {
            List<MiAssembleService::nonConfirmCodeArray> ncCodeList = new List<MiAssembleService::nonConfirmCodeArray>();
            if (string.IsNullOrWhiteSpace(ncCodeValue))
            {
                return ncCodeList.ToArray();
            }

            string[] ncCodes = ncCodeValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] hasNcValues = (hasNcValue ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (ncCodes.Length != hasNcValues.Length)
            {
                throw new InvalidOperationException("ncCode与hasNc的项目数量必须一致");
            }

            for (int i = 0; i < ncCodes.Length; i++)
            {
                string hasNcText = hasNcValues[i].Trim();
                bool hasNc;
                if (hasNcText.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    hasNcText.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                    hasNcText.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                    hasNcText.Equals("是", StringComparison.OrdinalIgnoreCase))
                {
                    hasNc = true;
                }
                else if (hasNcText.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                         hasNcText.Equals("0", StringComparison.OrdinalIgnoreCase) ||
                         hasNcText.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                         hasNcText.Equals("否", StringComparison.OrdinalIgnoreCase))
                {
                    hasNc = false;
                }
                else
                {
                    throw new InvalidOperationException("hasNc必须配置为true或false");
                }

                ncCodeList.Add(new MiAssembleService::nonConfirmCodeArray
                {
                    ncCode = ResolveAssemblyValue(ncCodes[i].Trim(), moduleCode),
                    hasNc = hasNc
                });
            }

            return ncCodeList.ToArray();
        }

        private void FillMesErrorDetails(ResponseData responseData, int code)
        {
            CSV_Style message = ResourceHandler.dparamParameters.Read_MESCodeCSV.Find_Code(code.ToString());
            if (message == null)
            {
                responseData.Message = "不存在于当前的报错文档中！请提供文档进行更新";
                return;
            }

            responseData.code = Convert.ToInt16(message.Code);
            responseData.Message = message.Message;
            responseData.way = message.way;
            responseData.personinCharge = message.PersoninCharge;
        }

        public ResponseData CheckStickerPnAndInventory(string moduleCode, bool forceCheck = false)
        {
            ResponseData result = new ResponseData
            {
                code = -1,
                sfc = moduleCode,
                startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
            };

            if (!forceCheck && !IsBomInventoryCheckEnabled())
            {
                result.code = 0;
                result.message = "贴纸PN及库存校验已停用";
                main.outDiary("【MES】贴纸PN及库存校验已停用", "信息");
                return result;
            }

            string logPath = ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiCheckBOMInventoryServiceService";
            MiCheckBOMInventoryServiceService service = new MiCheckBOMInventoryServiceService();

            try
            {
                List<MesPullInParameters> parameters = ResourceHandler.listMesBomInventoryParameters;
                string url = ToServiceUrl(ReadInterfaceParameter(parameters, "MES WSDL"));
                string timeoutText = ReadInterfaceParameter(parameters, "TimeOut(ms)", "10000");
                string site = ReadInterfaceParameter(parameters, "site");
                string user = ReadInterfaceParameter(parameters, "user");
                string operation = ReadInterfaceParameter(parameters, "operation");
                string resource = ReadInterfaceParameter(parameters, "Resource");
                string modeCheckOperation = ReadInterfaceParameter(parameters, "modeCheckOperation");
                string sfc = ResolveAssemblyValue(ReadInterfaceParameter(parameters, "sfc", "{SFC}"), moduleCode);

                int timeout;
                if (!int.TryParse(timeoutText, out timeout) || timeout <= 0)
                {
                    throw new InvalidOperationException("TimeOut(ms)必须是大于0的整数");
                }

                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(site) ||
                    string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(operation) ||
                    string.IsNullOrWhiteSpace(resource) || string.IsNullOrWhiteSpace(sfc))
                {
                    throw new InvalidOperationException("贴纸PN及库存校验的MES WSDL、site、user、operation、Resource、sfc不能为空");
                }

                service.Url = url;
                service.Timeout = timeout;
                service.PreAuthenticate = true;
                service.Credentials = new NetworkCredential(
                    ReadInterfaceParameter(parameters, "User"),
                    ReadInterfaceParameter(parameters, "Password"));

                CheckBOMInventoryRequest requestData = new CheckBOMInventoryRequest
                {
                    site = site,
                    operation = operation,
                    operationRevision = ReadInterfaceParameter(parameters, "operationRevision", "#"),
                    resource = resource,
                    parameterArray = ParseBomParameterArray(parameters),
                    user = user,
                    activity = ReadInterfaceParameter(parameters, "activity", "EAP_WS"),
                    sfc = sfc,
                    modeCheckOperation = string.IsNullOrWhiteSpace(modeCheckOperation) ? null : modeCheckOperation,
                    modeProcessSFC = ReadInterfaceParameter(parameters, "modeProcessSfc", "MODE_COMPLETE_SFC_POST_DC")
                };

                miCheckBOMInventory request = new miCheckBOMInventory
                {
                    CheckBOMInventoryRequest = requestData
                };

                DataGridViewClass.Write_MESLOG_CSV(new[] { "网址：," + service.Url }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "耗时：," + service.Timeout }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "site：," + requestData.site }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "user：," + requestData.user }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "operation：," + requestData.operation }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "operationRevision：," + requestData.operationRevision }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "activity：," + requestData.activity }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "resource：," + requestData.resource }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "modeCheckOperation：," + requestData.modeCheckOperation }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "modeProcessSfc：," + requestData.modeProcessSFC }, logPath, "贴纸PN及库存校验");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "sfc：," + requestData.sfc }, logPath, "贴纸PN及库存校验");
                foreach (CheckBOMInventoryParameter parameter in requestData.parameterArray)
                {
                    DataGridViewClass.Write_MESLOG_CSV(
                        new[] { "parameterArray：,{" + parameter.usage + ":" + parameter.category + ":" + parameter.dataField + "}" },
                        logPath,
                        "贴纸PN及库存校验");
                }

                miCheckBOMInventoryResponse response = service.miCheckBOMInventory(request);
                if (response == null || response.@return == null || !response.@return.codeSpecified)
                {
                    throw new InvalidOperationException("贴纸PN及库存校验接口未返回有效code");
                }

                result.code = response.@return.code;
                result.message = response.@return.message ?? "";
                DataGridViewClass.Write_MESLOG_CSV(
                    new[] { "从MES收集的数据:,{Code:" + result.code + " Message:" + result.message + "}" },
                    logPath,
                    "贴纸PN及库存校验");

                if (result.code != 0)
                {
                    FillMesErrorDetails(result, response.@return.code);
                    main.outDiary(
                        "【MES】贴纸PN及库存校验失败\r\n【MES】Code：" + result.code +
                        "\r\n【MES】message:" + result.message,
                        "警告");
                    return result;
                }

                main.outDiary("【MES】贴纸PN及库存校验成功", "信息");
                return result;
            }
            catch (Exception ex)
            {
                result.code = 9999;
                result.message = ex.Message;
                result.Message = "贴纸PN及库存校验接口调用异常";
                DataGridViewClass.Write_MESLOG_CSV(
                    new[] { "接口异常：," + ex.Message },
                    logPath,
                    "贴纸PN及库存校验");
                main.outDiary("【MES】贴纸PN及库存校验异常：" + ex.Message, "错误");
                return result;
            }
        }

        public ResponseData AssembleMaterial(string moduleCode, bool forceCheck = false)
        {
            ResponseData result = new ResponseData
            {
                code = -1,
                sfc = moduleCode,
                startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
            };
            string logPath = ResourceHandler.listSystemParameters[0].MESLogPath + "\\MiAssembleAndCollectDataForSfcServiceService";

            try
            {
                if (!forceCheck && !IsAssembleMaterialEnabled())
                {
                    result.code = 0;
                    result.message = "组装物料已停用";
                    main.outDiary("【MES】组装物料已停用", "信息");
                    return result;
                }

                MiAssembleService::MiAssembleAndCollectDataForSfcServiceService service = new MiAssembleService::MiAssembleAndCollectDataForSfcServiceService();
                List<MesPullInParameters> parameters = ResourceHandler.listMesAssembleMaterialParameters;
                string url = ToServiceUrl(ReadInterfaceParameter(parameters, "MES WSDL"));
                string timeoutText = ReadInterfaceParameter(parameters, "TimeOut(ms)", "10000");
                string site = ReadInterfaceParameter(parameters, "site");
                string user = ReadInterfaceParameter(parameters, "user");
                string operation = ReadInterfaceParameter(parameters, "operation");
                string resource = ReadInterfaceParameter(parameters, "Resource");
                string dcGroup = ReadInterfaceParameter(parameters, "dcGroup", "*");
                string sfc = ResolveAssemblyValue(ReadInterfaceParameter(parameters, "sfc", "{SFC}"), moduleCode);

                int timeout;
                if (!int.TryParse(timeoutText, out timeout) || timeout <= 0)
                {
                    throw new InvalidOperationException("TimeOut(ms)必须是大于0的整数");
                }

                if (string.IsNullOrWhiteSpace(sfc) || string.IsNullOrWhiteSpace(url) ||
                    string.IsNullOrWhiteSpace(site) || string.IsNullOrWhiteSpace(user) ||
                    string.IsNullOrWhiteSpace(operation) || string.IsNullOrWhiteSpace(resource) ||
                    string.IsNullOrWhiteSpace(dcGroup))
                {
                    throw new InvalidOperationException("组装物料的MES WSDL、site、user、operation、Resource、dcGroup、sfc不能为空");
                }

                MiAssembleService::dataCollectForSfcModeProcessSfc modeProcessSfc;
                string modeText = ReadInterfaceParameter(parameters, "modeProcessSfc", "MODE_NONE");
                if (!Enum.TryParse(modeText, true, out modeProcessSfc) ||
                    !Enum.IsDefined(typeof(MiAssembleService::dataCollectForSfcModeProcessSfc), modeProcessSfc))
                {
                    throw new InvalidOperationException("modeProcessSfc不是接口支持的过站模式");
                }

                service.Url = url;
                service.Timeout = timeout;
                service.PreAuthenticate = true;
                service.Credentials = new NetworkCredential(
                    ReadInterfaceParameter(parameters, "User"),
                    ReadInterfaceParameter(parameters, "Password"));

                MiAssembleService::assembleAndCollectDataForSfcRequest requestData = new MiAssembleService::assembleAndCollectDataForSfcRequest
                {
                    site = site,
                    sfc = sfc,
                    dcGroup = dcGroup,
                    dcGroupRevision = ReadInterfaceParameter(parameters, "dcGroupRevision", "#"),
                    operation = operation,
                    operationRevision = ReadInterfaceParameter(parameters, "operationRevision", "#"),
                    resource = resource,
                    user = user,
                    activityId = ReadInterfaceParameter(parameters, "activityId", "EAP_WS"),
                    modeProcessSFC = modeProcessSfc,
                    partialAssembly = ReadBooleanParameter(parameters, "partialAssembly", true),
                    inventoryArray = ParseAssemblyInventoryArray(
                        ReadInterfaceParameter(parameters, "inventoryArray[]"), moduleCode),
                    parametricDataArray = ParseAssemblyParameterArray(
                        ReadInterfaceParameter(parameters, "parameterArray[]"), moduleCode),
                    ncCodeArray = ParseAssemblyNcCodeArray(
                        ReadInterfaceParameter(parameters, "ncCode"),
                        ReadInterfaceParameter(parameters, "hasNc"),
                        moduleCode),
                    remark = null
                };

                MiAssembleService::miAssmebleAndCollectDataForSfc request = new MiAssembleService::miAssmebleAndCollectDataForSfc
                {
                    AssembleAndCollectDataForSfcRequest = requestData
                };

                DataGridViewClass.Write_MESLOG_CSV(new[] { "网址：," + service.Url }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "耗时：," + service.Timeout }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "site：," + requestData.site }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "user：," + requestData.user }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "operation：," + requestData.operation }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "resource：," + requestData.resource }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "dcGroup：," + requestData.dcGroup }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "modeProcessSfc：," + requestData.modeProcessSFC }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "partialAssembly：," + requestData.partialAssembly }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "sfc：," + requestData.sfc }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "inventoryArray数量：," + requestData.inventoryArray.Length }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "parameterArray数量：," + requestData.parametricDataArray.Length }, logPath, "组装物料");
                DataGridViewClass.Write_MESLOG_CSV(new[] { "ncCodeArray数量：," + requestData.ncCodeArray.Length }, logPath, "组装物料");

                MiAssembleService::miAssmebleAndCollectDataForSfcResponse response = service.miAssmebleAndCollectDataForSfc(request);
                if (response == null || response.@return == null)
                {
                    throw new InvalidOperationException("组装物料接口未返回有效响应");
                }

                result.code = response.@return.code;
                result.message = response.@return.message ?? "";
                result.sfc = string.IsNullOrWhiteSpace(response.@return.sfc) ? requestData.sfc : response.@return.sfc;
                DataGridViewClass.Write_MESLOG_CSV(
                    new[] { "从MES收集的数据:,{Code:" + result.code + " Message:" + result.message +
                            " Sfc:" + result.sfc + " FailedInventory:" + response.@return.failedInventory + "}" },
                    logPath,
                    "组装物料");

                if (result.code != 0)
                {
                    FillMesErrorDetails(result, result.code);
                    main.outDiary("【MES】组装物料失败\r\n【MES】Code：" + result.code +
                                  "\r\n【MES】message:" + result.message, "警告");
                    return result;
                }

                main.outDiary("【MES】组装物料成功", "信息");
                return result;
            }
            catch (Exception ex)
            {
                result.code = 9999;
                result.message = ex.Message;
                result.Message = "组装物料接口调用异常";
                DataGridViewClass.Write_MESLOG_CSV(
                    new[] { "接口异常：," + ex.Message },
                    logPath,
                    "组装物料");
                main.outDiary("【MES】组装物料异常：" + ex.Message, "错误");
                return result;
            }
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

        public ResponseData PullOut(string 模组码, dataCollectForSfcEx collectedData)
        {
            var serviceOutDll = new MachineIntegrationServiceService.MachineIntegrationServiceService();
            NetworkCredential dential = new NetworkCredential();
            dataCollectForModuleTest moduleTestRequest = new dataCollectForModuleTest();
            moduleTestRequest.DcForModuleTestRequest = new dcForModuleTestRequest();
            string moduleTestLogPath = ResourceHandler.listSystemParameters[0].MESLogPath + "\\dataCollectForModuleTest";

            try
            {
                dential.UserName = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "userName");
                dential.Password = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "password");

                serviceOutDll.PreAuthenticate = true;
                serviceOutDll.Credentials = dential;

                serviceOutDll.Url = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "url");
                serviceOutDll.Timeout = int.Parse(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "timeout"));    //服务器连接超时设置，毫秒 

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "网址：," + serviceOutDll.Url }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "耗时：," + serviceOutDll.Timeout }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "当前时间：," + DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") }, moduleTestLogPath, "出站收数");

                if (collectedData == null || collectedData.SfcDcExRequest == null || collectedData.SfcDcExRequest.parametricDataArray == null)
                {
                    throw new InvalidOperationException("MES收数参数数组未初始化");
                }

                dcForModuleTestRequest requestData = moduleTestRequest.DcForModuleTestRequest;
                requestData.site = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "site");
                requestData.user = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "user");
                requestData.operation = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "operation");
                requestData.operationRevision = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "operationRevision");
                requestData.activityId = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "activityId");
                requestData.resource = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "resource");
                requestData.dcGroup = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "dcGroup");
                requestData.dcGroupRevision = DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "dcGroupRevision");
                requestData.modeProcessSfc = PullOutGetEnum(DataGridViewClass.MesFindDataGridViewRead(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, "modeProcessSfc"));
                requestData.sfc = 模组码;
                requestData.parametricDataArray = collectedData.SfcDcExRequest.parametricDataArray;

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "site：," + requestData.site }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "user：," + requestData.user }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "operation：," + requestData.operation }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "operationRevision：," + requestData.operationRevision }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "activityId：," + requestData.activityId }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "resource：," + requestData.resource }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "dcGroup：," + requestData.dcGroup }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "dcGroupRevision：," + requestData.dcGroupRevision }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "modeProcessSfc：," + requestData.modeProcessSfc }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { "sfc：," + requestData.sfc }, moduleTestLogPath, "出站收数");

            }
            catch (Exception ex)
            {
                main.outDiary("MES收数请求参数异常：" + ex.Message, "错误");
                return new ResponseData
                {
                    code = 9999,
                    message = ex.Message,
                    sfc = 模组码
                };
            }
            main.outDiary("出站模组码：" + 模组码, "信息");
                
            string parametr = "";

            foreach (var item in moduleTestRequest.DcForModuleTestRequest.parametricDataArray)
            {
                if (item == null)
                {
                    break;
                }
                parametr += "{" + $"{item.name}:{item.dataType}:{item.value} " + "}";
            }

            DataGridViewClass.Write_MESLOG_CSV(new string[] { "parameterArray：," + parametr }, moduleTestLogPath, "出站收数");

            dataCollectForModuleTestResponse responseIn;
            ResponseData reData = new ResponseData();
            reData.code = -1;
            reData.sfc = 模组码;
            try
            {
                responseIn = serviceOutDll.dataCollectForModuleTest(moduleTestRequest);
                if (responseIn == null || responseIn.@return == null)
                {
                    throw new InvalidOperationException("MES收数接口未返回有效响应");
                }

                DataGridViewClass.Write_MESLOG_CSV(new string[] { "从MES收集的数据:,{" +
                                                                       "Code:" + responseIn.@return.code +
                                                                       "Message:" + responseIn.@return.message +
                                                                       "}"
                                                                }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, moduleTestLogPath, "出站收数");
                reData.code = responseIn.@return.code;
                reData.message = responseIn.@return.message;
                if (responseIn.@return.code != 0)
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
                                                                }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, moduleTestLogPath, "出站收数");
                DataGridViewClass.Write_MESLOG_CSV(new string[] { " " }, moduleTestLogPath, "出站收数");

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
