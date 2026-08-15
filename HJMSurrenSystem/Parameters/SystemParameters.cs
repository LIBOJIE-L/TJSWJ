using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJMSurrenSystem.Parameters
{
    /// <summary>
    /// 系统参数
    /// </summary>
    public class SystemParameters
    {
        /// <summary>
        /// 当前工位
        /// </summary>
        public string currentLocation { get; set; }

        /// <summary>
        /// 程序日记
        /// </summary>
        public string ProgramLogPath { get; set; }

        /// <summary>
        /// MES文件保存
        /// </summary>
        public string MESLogPath { get; set; }

        /// <summary>
        /// 日记清除时间
        /// </summary>
        public string LogClearTime { get; set; }

        /// <summary>
        /// 自动退出时间
        /// </summary>
        public string VoluntarilyLogOut { get; set; }

        /// <summary>
        /// 设备标识
        /// </summary>
        public string deviceIdentification { get; set; }

        /// <summary>
        /// 白班
        /// </summary>
        public string dayShift { get; set; }

        /// <summary>
        /// 晚班
        /// </summary>
        public string nightShift { get; set; }

        /// <summary>
        /// 模组码
        /// </summary>
        public string ModuleCode { get; set; }

        /// <summary>
        /// PN码
        /// </summary>
        public string PNCode { get; set; }

        /// <summary>
        /// 模组类型
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// 扫码条码长度
        /// </summary>
        public string BarcodeLength { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 上次打开的配方名称
        /// </summary>
        public string FormulaName { get; set; }

    }
}
