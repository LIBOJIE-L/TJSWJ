using BJTSurrenSystem.Config_UI.FtpConfig;
using BJTSurrenSystem.Interface_UI;
using BJTSurrenSystem.MES;
using BJTSurrenSystem.RunProcess;
using BJTSurrenSystem.Siemens;
using BJTSurrenSystem_Siemens;
using language;
using Siemens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BJTSurrenSystem.Parameters
{
    /// <summary>
    /// 动态参数
    /// </summary>
    public class DparamParameters
    {
        /// <summary>
        /// 当前语言
        /// </summary>
        public LanguageType Language { get; set; }

        /// <summary>
        /// 流程对象
        /// </summary>
        public FlowClass flowClass { get; set; }

        /// <summary>
        /// plc
        /// </summary>
        public JPIO_OPC siemensS7Net { get; set; }

        /// <summary>
        /// plc_UI
        /// </summary>
        public set_PLC set_PlcUI { get; set; }
        /// <summary>
        /// ftp_UI
        /// </summary>
        public FtpConfig_Ui FtpConfigUI { get; set; }

        /// <summary>
        /// PLC信号灯
        /// </summary>
        public PLCSignalL_UI pLCSignalL_UI { get; set; }

        /// <summary>
        /// 离线设置
        /// </summary>
        public FilePathUI filePathUI { get; set; }

        /// <summary>
        /// 离线设置
        /// </summary>
        public OffLineUI offLineUI { get; set; }

        /// <summary>
        /// plc地址表
        /// </summary>
        public PLCInteractionUI pLCInteractionUI { get; set; }

        /// <summary>
        /// MES进站参数配置
        /// </summary>
        public MesPullInUI mesPullInUI { get; set; }

        /// <summary>
        /// MES贴纸PN及库存校验参数配置
        /// </summary>
        public MesPullInUI mesBomInventoryUI { get; set; }

        /// <summary>
        /// MES组装物料参数配置
        /// </summary>
        public MesPullInUI mesAssembleMaterialUI { get; set; }

        /// <summary>
        /// MES出站参数配置
        /// </summary>
        public MesPullOutUI mesPullOutUI { get; set; }

        /// <summary>
        /// MES电芯校验参数配置
        /// </summary>
        public MesDX_verifyUI mesDX_verifyUI { get; set; }

        /// <summary>
        /// MES首件参数配置
        /// </summary>
        public MesInitialWorkpieceParametersUI mesInitialWorkpieceParametersUI { get; set; }

        /// <summary>
        /// MES出站上传参数配置
        /// </summary>
        public MesPullOutUploadingUI mesPullOutUploadingUI { get; set; }

        /// <summary>
        /// MES侧板上传参数配置
        /// </summary>
        public MesPullOutSide_plateUI mesPullOutSide_plateUI { get; set; }

        /// <summary>
        /// 修改的数据
        /// </summary>
        public List<string> dataAmendParmeters { get; set; }

        /// <summary>
        /// 判断是否离线
        /// </summary>
        public bool JudgeOffLine { get; set; }

        /// <summary>
        /// 获取MES报错代码内容
        /// </summary>
        public Read_MESCodeCSV Read_MESCodeCSV { get; set; } = new Read_MESCodeCSV();

        /// <summary>
        /// MES交互
        /// </summary>
        public MesInteraction MesInteraction { get; set; }

        /// <summary>
        /// 物料配方
        /// </summary>
        public DataTable dataTable { get; set; }

        /// <summary>
        /// 产品模板路径
        /// </summary>
        public string mFormulaFilePath { get; set; }

        /// <summary>
        /// 配方管理
        /// </summary>
        public FormulaUI formulaUI { get; set; }

        /// <summary>
        /// 自动注销线程
        /// </summary>
        public Thread loginThread { get; set; }

    }
}
