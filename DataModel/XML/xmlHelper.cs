using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataModel
{
    public class XmlHelper
    {
        private string path { get; set; }

        public XmlHelper(string path)
        {
            this.path = path;
        }

        public bool Read<T>(ref T t)
        {
            try
            {
                using (StreamReader stream = new StreamReader(path, Encoding.Default))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    t = (T)serializer.Deserialize(stream);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Write<T>(T t)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(path, false, Encoding.Default))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stream, t);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
