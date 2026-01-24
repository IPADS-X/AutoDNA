using ModbusLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CYStandardProcedure
{
    public static class Common
    {
      //  public static ModbusRtu m_ModbusRtu = null;



        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="objectToConvert"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        public static void PositiveSerializerXml(object objectToConvert, string path, Encoding encoding)
        {
            // 对象不为空
            if (objectToConvert != null)
            {
                Type t = objectToConvert.GetType();
                //t = typeof(ArrayList).GetType();
                XmlSerializer ser = new XmlSerializer(t);
                using (StreamWriter writer = new StreamWriter(path, false, encoding))
                {
                    ser.Serialize(writer, objectToConvert);
                    writer.Close();
                }
            }
        }
        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="path">路经加文件名</param>
        /// <param name="objectType">内容类型</param>
        /// <param name="encoding">编码类型</param>
        /// <returns></returns>
        public static object InsteadSerializerXml(string path, Type objectType, Encoding encoding)
        {
            object convertedObject = null;
            // 文件名不为空
            if (!string.IsNullOrEmpty(path))
            {

                XmlSerializer ser = new XmlSerializer(objectType);
                using (StreamReader reader = new StreamReader(path, encoding))
                {
                    convertedObject = ser.Deserialize(reader);
                    reader.Close();
                }
            }
            return convertedObject;
        }

    }
    /// <summary>
    /// 报警含义
    /// </summary>
    public enum Enum_ErrorCode
    {
        正常 = 0,
        位置超差 = -10,
        速度超差 = -20,
        位置超差与速度超差 = 30,
        电机堵转 = -40,
        位置超差加电机堵转 = -50,
        速度超差加电机堵转 = -60,
        位置超差加速度超差加电机堵转 = -70
    }
}
