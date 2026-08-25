using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 读取MES报错文档，用于查询报错代码的意思
namespace BJTSurrenSystem.MES
{
    public class Read_MESCodeCSV
    {
        List<CSV_Style> array = new List<CSV_Style>();
        public Read_MESCodeCSV() {
            ReadCSV();// 读取CSV件里的内容
        }

        public void ReadCSV() {
            List<string> temp = DataGridViewClass.Read_CSV(System.Windows.Forms.Application.StartupPath + "/MES报错文档.CSV");

            for (int i = 0; i < temp.Count; i++)
            {
                string[] temp2 = temp[i].Split(',');

                string Code = "";
                try {
                    Code = temp2[0];
                } catch (Exception) { Code = ""; }

                string Message = "";
                try
                {
                    Message = temp2[1];
                }
                catch (Exception) { Message = ""; }

                string way = "";
                try
                {
                    way = temp2[2];
                }
                catch (Exception) { way = ""; }

                string PersoninCharge = "";
                try
                {
                    PersoninCharge = temp2[3];
                }
                catch (Exception) { PersoninCharge = ""; }
                array.Add(new CSV_Style(Code, Message, way, PersoninCharge));
                
            }
        }

        public CSV_Style Find_Code(string Code) {
            foreach (CSV_Style temp in array) {
                if (temp.Code == Code) {
                    return temp;
                }
            }
            return null;
        }
    }

    public class CSV_Style {
        public CSV_Style(string Code, string Message, string way, string PersoninCharge) {
            this.Code = Code;
            this.Message = Message;
            this.way = way;
            this.PersoninCharge = PersoninCharge;
        }
        /// <summary>
        /// 报错代码
        /// </summary>
        public string Code;
        /// <summary>
        /// 报错原因
        /// </summary>
        public string Message;
        /// <summary>
        /// 解决办法
        /// </summary>
        public string way;
        /// <summary>
        /// 负责人员
        /// </summary>
        public string PersoninCharge;
    }
}
