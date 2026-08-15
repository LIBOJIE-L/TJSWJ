using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJMSurrenSystem.Config_UI.FtpConfig
{
    public class FtpParam
    {

        public bool IsEnabled { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        public string FtpName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string FtpPwd { get; set; }
        /// <summary>
        /// FtpIp
        /// </summary>
        public string FtpIP { get; set;}
        /// <summary>
        /// FtpPath
        /// </summary>
        public string FtpPath { get; set;}

    }
}
