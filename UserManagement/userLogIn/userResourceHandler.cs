using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.userLogIn
{
    public class userResourceHandler
    {
        public static List<UserParameters> listUserParameters = new List<UserParameters>();

        public static UserParameters userParameters = new UserParameters();

        public static dparamParameters dparamParameters = new dparamParameters();

        public static string language = "";

    }

    public class UserParameters {

        /// <summary>
        /// 账号 
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string userPassword { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string userPermissions { get; set; }

        /// <summary>
        /// 注销时间
        /// </summary>
        public string cancellationTime { get; set; }

    }

    public class dparamParameters {
        /// <summary>
        /// 修改的用户名 
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime loginTime { get; set; }
    }



}
