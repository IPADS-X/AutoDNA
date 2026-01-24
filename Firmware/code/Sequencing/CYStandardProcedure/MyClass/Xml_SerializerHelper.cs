using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CYStandardProcedure
{
    /// <summary>
    /// 序列化XML
    /// </summary>
    public class Xml_SerializerHelper
    {
        private static readonly object XmlSerializerObj = new object();

        /// <summary>
        /// 写入XML
        /// </summary>
        /// <param name="obj">写入的对象</param>
        /// <param name="path">文件路径</param>
        public static bool XmlSerializer(object obj, string path)
        {
            lock (XmlSerializerObj)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        var xs = new XmlSerializer(obj.GetType());
                        xs.Serialize(sw, obj);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }

        private static readonly object XmlDeserializeObj = new object();

        /// <summary>
        /// 读取XML
        /// </summary>
        /// <typeparam name="T">返回的对象</typeparam>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string path)
        {
            lock (XmlDeserializeObj)
            {

                try
                {
                    Type type = typeof(T);
                    XmlSerializer serializer = new XmlSerializer(type);
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        return (T)serializer.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }
        }
    }
}
