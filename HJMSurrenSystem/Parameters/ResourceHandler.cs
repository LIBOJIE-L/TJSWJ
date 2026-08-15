using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HJMSurrenSystem.Parameters
{
    class ResourceHandler
    {

        /// <summary>
        /// 系统参数
        /// </summary>
        public static List<SystemParameters> listSystemParameters = new List<SystemParameters>();

        /// <summary>
        /// plc参数
        /// </summary>
        public static List<PlcParameters> listPlcParameters = new List<PlcParameters>();

        /// <summary>
        /// plc地址参数
        /// </summary>
        public static List<PLCInteractiveAddress> listPLCInteractiveAddress = new List<PLCInteractiveAddress>();

        /// <summary>
        /// MES进站配置参数
        /// </summary>
        public static List<MesPullInParameters> listMesPullInParameters = new List<MesPullInParameters>();

        /// <summary>
        /// MES出站配置参数
        /// </summary>
        public static List<MesPullOutParameters> listMesPullOutParameters = new List<MesPullOutParameters>();

        /// <summary>
        /// MES首件配置参数
        /// </summary>
        public static List<MesInitialWorkpieceParameters> listMesInitialWorkpieceParameters = new List<MesInitialWorkpieceParameters>();

        /// <summary>
        /// MES电芯校验配置参数
        /// </summary>
        public static List<MesDX_verifyParameters> listMesDX_verifyParameters = new List<MesDX_verifyParameters>();

        /// <summary>
        /// MES出站上传参数
        /// </summary>
        public static List<MesPullOutUploadingParameters> listMesPullOutUploadingParameters = new List<MesPullOutUploadingParameters>();

        /// <summary>
        /// MES侧板上传参数
        /// </summary>
        public static List<MesPullOutSide_plateParameters> listMesPullOutSide_plateParameters = new List<MesPullOutSide_plateParameters>();

        /// <summary>
        /// 表头
        /// </summary>
        public static List<Header> listHeader = new List<Header>();

        /// <summary>
        /// 动态参数
        /// </summary>
        public static DparamParameters dparamParameters = new DparamParameters();

        /// <summary>
        /// PLC地址
        /// </summary>
        public static PLCInteractiveAddress pLCInteractiveAddress = new PLCInteractiveAddress();

        /// <summary>
        /// MES进站配置参数
        /// </summary>
        public static MesPullInParameters mesPullInParameters = new MesPullInParameters();

        /// <summary>
        /// MES出站配置参数
        /// </summary>
        public static MesPullOutParameters mesPullOutParameters = new MesPullOutParameters();

        /// <summary>
        /// MES首件配置参数
        /// </summary>
        public static MesInitialWorkpieceParameters mesInitialWorkpieceParameters = new MesInitialWorkpieceParameters();

        /// <summary>
        /// MES电芯校验配置参数
        /// </summary>
        public static MesDX_verifyParameters mesDX_verifyParameters = new MesDX_verifyParameters();

        /// <summary>
        /// MES出站上传参数
        /// </summary>
        public static MesPullOutUploadingParameters mesPullOutUploadingParameters = new MesPullOutUploadingParameters();

        /// <summary>
        /// 侧板出站上传参数
        /// </summary>
        public static MesPullOutSide_plateParameters mesPullOutSide_plateParameters = new MesPullOutSide_plateParameters();
    }
}
