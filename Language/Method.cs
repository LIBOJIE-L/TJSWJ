using language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace language
{
    public class Method
    {
        public static List<Language> languages = new List<Language>();

        /// <summary>
        /// 字符串对应当前语言
        /// </summary>
        /// <param name="value">输入字符串</param>
        /// <returns></returns>
        public static string StringToLanguage(string Language, string value)
        {
            try
            {
                if (Language == "英")
                {
                    string content = languages.Find(language => language.Chinese == value)?.English;
                    return string.IsNullOrEmpty(content) ? value : content;
                }
                if (Language == "德")
                {
                    string content = languages.Find(language => language.Chinese == value)?.German;
                    return string.IsNullOrEmpty(content) ? value : content;
                }
                return value;
            }
            catch (Exception)
            {
                try
                {
                    if (Encoding.Default.GetByteCount(value) != value.Length)
                    {
                        File.AppendAllText("language/logforlang.txt", value + "\n");
                    }
                }
                catch { }
                return value;
            }
        }
    }
}
