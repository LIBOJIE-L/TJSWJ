using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJTSurrenSystem.Parameters
{
    /// <summary>
    /// MES进站配置参数
    /// </summary>
    public struct MesPullInParameters
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParametersName { get; set; }

        /// <summary>
        /// 参数说明
        /// </summary>
        public string ParametersExplain { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ParametersPrice { get; set; }

    }

    /// <summary>
    /// MES出站配置参数
    /// </summary>
    public struct MesPullOutParameters 
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParametersName { get; set; }

        /// <summary>
        /// 参数说明
        /// </summary>
        public string ParametersExplain { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ParametersPrice { get; set; }
    }

    /// <summary>
    /// MES首件配置参数
    /// </summary>
    public struct MesInitialWorkpieceParameters 
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParametersName { get; set; }

        /// <summary>
        /// 参数说明
        /// </summary>
        public string ParametersExplain { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ParametersPrice { get; set; }
    }

    /// <summary>
    /// MES电芯校验配置参数
    /// </summary>
    public struct MesDX_verifyParameters
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParametersName { get; set; }

        /// <summary>
        /// 参数说明
        /// </summary>
        public string ParametersExplain { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ParametersPrice { get; set; }
    }

    /// <summary>
    /// MES出站上传参数
    /// </summary>
    public struct MesPullOutUploadingParameters 
    {
        /// <summary>
        /// 表头
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// MES名称
        /// </summary>
        public string ParametersMESName { get; set; }

        /// <summary>
        /// MES类型
        /// </summary>
        public string ParametersMESType { get; set; }

        /// <summary>
        /// PLC地址
        /// </summary>
        public string PLCAddres { get; set; }


        /// <summary>
        /// PLC类型
        /// </summary>
        public string ParametersPLCType { get; set; }

        /// <summary>
        /// 参数上限
        /// </summary>
        public string ParametersUpperLimit { get; set; }

        /// <summary>
        /// 参数下限
        /// </summary>
        public string ParametersLowerLimit { get; set; }

        /// <summary>
        /// 是否上传
        /// </summary>
        public bool WhetherUploading { get; set; }

        /// <summary>
        /// 是否首件
        /// </summary>
        public bool WhetherInitial { get; set; }

        /// <summary>
        /// 重复校验
        /// </summary>
        public bool RepetitionVerify { get; set; }

        /// <summary>
        /// 数据数量地址
        /// </summary>
        public string DataQuantityPath { get; set; }
        /// <summary>
        /// 数据数量PLC类型
        /// </summary>
        public string DataQuantityType { get; set; }
        /// <summary>
        /// 递增值
        /// </summary>
        public string DataIncrementValue { get; set; }

    }

    /// <summary>
    /// MES侧板上传参数
    /// </summary>
    public struct MesPullOutSide_plateParameters
    {
        /// <summary>
        /// 表头
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// MES名称
        /// </summary>
        public string ParametersMESName { get; set; }

        /// <summary>
        /// MES类型
        /// </summary>
        public string ParametersMESType { get; set; }

        /// <summary>
        /// PLC地址
        /// </summary>
        public string PLCAddres { get; set; }


        /// <summary>
        /// PLC类型
        /// </summary>
        public string ParametersPLCType { get; set; }

        /// <summary>
        /// 参数上限
        /// </summary>
        public string ParametersUpperLimit { get; set; }

        /// <summary>
        /// 参数下限
        /// </summary>
        public string ParametersLowerLimit { get; set; }

        /// <summary>
        /// 是否上传
        /// </summary>
        public bool WhetherUploading { get; set; }

        /// <summary>
        /// 是否首件
        /// </summary>
        public bool WhetherInitial { get; set; }

        /// <summary>
        /// 重复校验
        /// </summary>
        public bool RepetitionVerify { get; set; }

    }

}
