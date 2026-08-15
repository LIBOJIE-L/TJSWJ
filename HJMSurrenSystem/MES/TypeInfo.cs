using HJMSurrenSystem.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJMSurrenSystem.MES
{
    class TypeInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public Object value { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 值上线
        /// </summary>
        public string upperLimit { get; set; }
        /// <summary>
        /// 值下线
        /// </summary>
        public string lowerLimit { get; set; }
        /// <summary>
        /// 是否上传MES
        /// </summary>
        public string isUploading { get; set; }
        /// <summary>
        /// 是否首件
        /// </summary>
        public string isFrist { get; set; }
        /// <summary>
        /// 是否重复校验
        /// </summary>
        public string isValidation { get; set; }
        /// <summary>
        /// MES名称
        /// </summary>
        public string name_mes { get; set; }
        /// <summary>
        /// MES类型
        /// </summary>
        public string type_mes { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 数据所占字节长度
        /// </summary>
        public ushort Length { get; set; }

        public TypeInfo()
        {
        }

        public TypeInfo(Type thisType, ushort thisLength, string name, string upperLimit, string lowerLimit)
        {
            this.Type = thisType;
            this.Length = thisLength;
            this.name = name;
            this.upperLimit = upperLimit;
            this.lowerLimit = lowerLimit;
        }
    }
}
