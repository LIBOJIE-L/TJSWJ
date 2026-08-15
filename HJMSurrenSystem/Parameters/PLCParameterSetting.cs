using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HJMSurrenSystem.Parameters
{
    /// <summary>
    /// PLC参数
    /// </summary>
    public class PlcParameters
    {
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// PLC型号
        /// </summary>
        public string PLCmodel { get; set; }

    }

    /// <summary>
    /// PLC交互地址
    /// </summary>
    public struct PLCInteractiveAddress {
        /// <summary>
        /// 地址说明
        /// </summary>
        public string AddressThat { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 地址类型
        /// </summary>
        public string AddressType { get; set; }
    }
}
