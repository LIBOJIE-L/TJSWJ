using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace DataModel.CSV
{
    public class ToolUtils
    {
        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return result;
                string tmpstr = sourse.Substring(startindex + startstr.Length);
                endindex = tmpstr.IndexOf(endstr);
                if (endindex == -1)
                    return result;
                result = tmpstr.Remove(endindex);
            }
            catch (Exception)
            {
                //Log.WriteLog("MidStrEx Err:" + ex.Message);
            }
            return result;
        }

        public static long GetCurrentTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static byte[] StringToByteArr(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static string getFilePath(string startCatalogue, string code, string testResult = "")
        {
            string[] fileNames = null;
            try
            {
                fileNames = Directory.GetFiles($"{startCatalogue}/{DateTime.Now.ToString("yyyyMM")}/{DateTime.Now.ToString("yyyyMMdd")}", "*.*", SearchOption.AllDirectories);
            }
            catch (Exception e) { throw e; }
            List<string> fileNamList = new List<string>(fileNames);
            fileNamList.Reverse();
            List<FileInfo> containFileName = new List<FileInfo>();
            foreach (var item in fileNamList)
            {
                if (item.Contains(code))
                {
                    containFileName.Add(new FileInfo(item));
                }
            }
            if (containFileName.Count == 1)
                return containFileName[0].DirectoryName + "\\" + containFileName[0].Name;
            if (containFileName.Count == 0)
                return "";
            //List<DateTime> fileCreatTiem = new List<DateTime>();
            List<FileInfo> sortList = containFileName.OrderBy(x => x.CreationTime).ToList();
            return sortList[sortList.Count - 1].DirectoryName + "\\" + sortList[sortList.Count - 1].Name;
        }

        #region 全局变量
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);
        #endregion
        /// <summary>
        /// 获取INI文件数据
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <param name="ini_path"></param>
        /// <returns></returns>
        public static string Get_ini_data(string title, string name, string ini_path)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(title, name, "", sb, 255, ini_path);
            return sb.ToString();
        }

        /// <summary>
        /// 设置INI文件数据
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="ini_path"></param>
        public static void Set_ini_data(string title, string name, string value, string ini_path)
        {
            WritePrivateProfileString(title, name, value, ini_path);
        }

        public static void SetObjToIniData(string title, object data, string ini_path)
        {
            if (data == null)
                return;

            foreach (var item in data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Set_ini_data(title, item.Name, item.GetValue(data).ToString(), ini_path);
            }
        }

        public static T GetObjToIniData<T>(string title, Type data, string ini_path)
        {
            if (data == null)
                return default;
            JObject obj = new JObject();
            foreach (var item in data.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string value = Get_ini_data(title, item.Name, ini_path);

                if (string.IsNullOrEmpty(value))
                    continue;
                obj[item.Name] = value;
                //Set_ini_data(title, item.Name, item.GetValue(item).ToString(), ini_path);
            }
            return JsonConvert.DeserializeObject<T>(obj.ToString());
        }

        /// <summary>
        /// 加密字符串，并保存为base64编码格式的字符串
        /// </summary>
        /// <param name="encryptStr"></param>
        /// <returns></returns>
        public static string EncryptStr(string encryptStr)
        {
            string decryptStr = string.Empty;
            try
            {
                //将待加密的明文字符串转为加密所需的字节数组格式
                byte[] plainText = Encoding.UTF8.GetBytes(encryptStr);

                //设定加密参数
                RijndaelManaged rijndaelCipher = Setting();

                //加密字符串
                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
                byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

                //将加密后的字节数组转为base64格式字符串
                decryptStr = Convert.ToBase64String(cipherBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return decryptStr;
        }

        /// <summary>
        /// 解密base64编码格式的字符串
        /// </summary>
        /// <param name="decryptStr"></param>
        /// <returns></returns>
        public static string DecryptStr(string decryptStr)
        {
            string encryptStr = string.Empty;
            try
            {
                //将待解密的base64格式字符串解码为加密所需的字节数组格式
                byte[] plainText = Convert.FromBase64String(decryptStr);

                //设定解密参数
                RijndaelManaged rijndaelCipher = Setting();

                //解密字符串
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

                //将解密后的字节数组转为字符串
                encryptStr = Encoding.UTF8.GetString(cipherBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return encryptStr;
        }


        //设置AES加密解密参数
        private static RijndaelManaged Setting()
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes("JIANGZHIYONG0001"), //加密密钥,自己设置，长度必须为16字节的倍数
                IV = Encoding.UTF8.GetBytes("1234567812345678"),  //加密的iv偏移量,长度必须为16字节的倍数
                Mode = CipherMode.CBC,       //加密模式，ECB、CBC、CFB等
                Padding = PaddingMode.PKCS7, //待加密的明文长度不满足条件时使用的填充模式，PKCS7是python中默认的填充模式
                BlockSize = 32              //加密操作的块大小
            };
            return rijndaelCipher;
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <returns></returns>
        public static string GetMD5(string sDataIn)
        {
            sDataIn += "a35j#o1>!6"; //加盐  防止设置密码过于简单  被暴力破解
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.Substring(0, 12); //取前十二位为密码
        }

        // 写入json数据
        public static bool JsonWrite(object obj, string jsonPath)
        {
            try
            {
                string jsonStr = JsonConvert.SerializeObject(obj);
                System.IO.File.WriteAllText(jsonPath, jsonStr, Encoding.Default);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
        // 读取Json数据转string
        public static string ReadJsonString(string jsonPath)
        {
            if (!File.Exists(jsonPath))
            {
                return string.Empty;
            }
            return File.ReadAllText(jsonPath, Encoding.Default);
        }

        /// <summary>
        /// 字典类型转化为对象
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T DicToObject<T>(Dictionary<string, object> dic) where T : new()
        {
            var md = new T();
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            foreach (var d in dic)
            {
                var filed = textInfo.ToTitleCase(d.Key);
                try
                {
                    var value = d.Value;
                    md.GetType().GetProperty(filed).SetValue(md, value);
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
            return md;
        }

        public static bool IsFileNameValid(string name)
        {
            bool isFilename = true;
            string[] errorStr = new string[] { "/", "\\", ":", ",", "*", "?", "\"", "<", ">", "|" };

            if (string.IsNullOrEmpty(name))
            {
                isFilename = false;
            }
            else
            {
                for (int i = 0; i < errorStr.Length; i++)
                {
                    if (name.Contains(errorStr[i]))
                    {
                        isFilename = false;
                        break;
                    }
                }
            }
            return isFilename;
        }

        /// <summary>
        /// 处理文件名称
        /// </summary>
        /// <param name="fileNameFormat">文件格式</param>
        /// <returns>返回合法的文件名</returns>
        public static string PraseStringToFileName(string fileNameFormat)
        {
            char[] strs = "+#?*\"<>/;,-:%~".ToCharArray();
            foreach (char c in strs)
                fileNameFormat = fileNameFormat.Replace(c.ToString(), "_");

            strs = "：，。；？".ToCharArray();
            foreach (char c in strs)
                fileNameFormat = fileNameFormat.Replace(c.ToString(), "_");

            //去掉空格.
            while (fileNameFormat.Contains(" ") == true)
                fileNameFormat = fileNameFormat.Replace(" ", "");

            //替换特殊字符.
            fileNameFormat = fileNameFormat.Replace("\t\n", "");

            //处理合法的文件名.
            StringBuilder rBuilder = new StringBuilder(fileNameFormat);
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                rBuilder.Replace(rInvalidChar.ToString(), string.Empty);

            fileNameFormat = rBuilder.ToString();

            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace("__", "_");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");
            fileNameFormat = fileNameFormat.Replace(" ", "");

            if (fileNameFormat.Length > 240)
                fileNameFormat = fileNameFormat.Substring(0, 240);

            return fileNameFormat;
        }

        public static string GetStrToNumber(string str)
        {

            Regex reg = new Regex("-?[0-9]+.[0-9]+" , RegexOptions.IgnoreCase | RegexOptions.Singleline,
                        TimeSpan.FromSeconds(3));
            Match m = reg.Match(str);

            if(!string.IsNullOrEmpty(m.Value))
                return m.Value;
            reg = new Regex("-?[0-9]+", RegexOptions.IgnoreCase | RegexOptions.Singleline,
                        TimeSpan.FromSeconds(3));
            m = reg.Match(str);
            return m.Value;
        }
        
    }
}
