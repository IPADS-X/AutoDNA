using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using CYAutoFramework;
using System.Drawing;
using ModbusLib;

namespace CYStandardProcedure
{
    class SerializeClass
    {
        /// <summary>
        /// 实例记忆类
        /// </summary>
        public static MemoryClass mMemory = new MemoryClass();
        /// <summary>
        /// 实例测序启动参数(搬运工位)
        /// </summary>
        public static SequencingStartParam startParam_carryStation = new SequencingStartParam();
        /// <summary>
        /// 实例测序启动参数(测序仪工位)
        /// </summary>
        public static SequencingStartParam startParam_sequencingStation = new SequencingStartParam();
        /// <summary>
        /// 实例测序当前ID号(测序仪工位)
        /// </summary>
        public static RunID id_sequencingStation = new RunID();
        /// <summary>
        /// 实例测序当前ID号（文件处理线程）
        /// </summary>
        public static RunID id_dataProcessingStation = new RunID();
        /// <summary>
        /// 实例芯片质检ID号
        /// </summary>
        public static ProductCode mProductCode = new ProductCode();
        /// <summary>
        /// 实例数字孪生参数
        /// </summary>
        public static AnimationClass animationParam = new AnimationClass();

        public static RobClawParam m_RobClawParam = new RobClawParam();
        public static ModbusRtu m_ModbusRtuRob = null;


        /// <summary>
        /// 实例IDNA引物信息类
        /// </summary>
        public static CompleteReportingFromControl completeParam = new CompleteReportingFromControl();



        /// <summary>
        /// 实例请求总控开始实验类
        /// </summary>
        public static StartReportingToControl mStartReportingToControl = new StartReportingToControl();
        /// <summary>
        /// 实例测序结束上报总控类
        /// </summary>
        public static CompleteReportingToControl mCompleteReportingToControl = new CompleteReportingToControl();
        /// <summary>
        /// 实例测序结果上报总控类
        /// </summary>
        public static ChipDataReportingToControl mChipDataReportingToControl = new ChipDataReportingToControl();
        /// <summary>
        /// 实例查询测序任务类
        /// </summary>
        public static SearchFolloUpTaskToControl mSearchFolloUpTaskToControl = new SearchFolloUpTaskToControl();

        /// <summary>
        /// 反序列化参数
        /// </summary>
        /// <returns></returns>
        public static bool ReadRobClawParame()
        {
            try
            {
                // 设备全部记忆参数路径
                if (!File.Exists(Application.StartupPath + "\\ExeFile\\RobotClaws\\RobotClawConfig.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(Application.StartupPath + "\\ExeFile\\RobotClaws\\RobotClawConfig.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(RobClawParam));
                        m_RobClawParam = (RobClawParam)serial.Deserialize(fs);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 序列化记忆文件
        /// </summary>
        /// 
        public static bool WriteRobClawParame()
        {
            try
            {
                if (!Directory.Exists(Application.StartupPath + "\\ExeFile\\RobotClaws\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\ExeFile\\RobotClaws\\");
                }
                // 设备全部记忆参数路径
                if (File.Exists(Application.StartupPath + "\\ExeFile\\RobotClaws\\RobotClawConfig.xml"))
                {
                    File.Delete(Application.StartupPath + "\\ExeFile\\RobotClaws\\RobotClawConfig.xml");
                }
                using (FileStream fs = new FileStream(Application.StartupPath + "\\ExeFile\\RobotClaws\\RobotClawConfig.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(RobClawParam));
                    serial.Serialize(fs, m_RobClawParam);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 反序列化记忆参数
        /// </summary>
        /// <returns></returns>
        public static bool ReadMemoryParame()
        {

            try
            {
                // 设备全部记忆参数路径
                if (!File.Exists(@"Serialize\\MachineStatusMemory.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(@"Serialize\\MachineStatusMemory.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(MemoryClass));
                        mMemory = (MemoryClass)serial.Deserialize(fs);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 序列化记忆文件
        /// </summary>
        /// 
        public static bool WriteMemoryParame()
        {
            try
            {
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                // 设备全部记忆参数路径
                if (File.Exists(@"Serialize\\MachineStatusMemory.xml"))
                {
                    File.Delete(@"Serialize\\MachineStatusMemory.xml");
                }
                using (FileStream fs = new FileStream(@"Serialize\\MachineStatusMemory.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(MemoryClass));
                    serial.Serialize(fs, mMemory);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 反序列化测序参数
        /// </summary>
        /// <returns></returns>
        public static bool ReadSequenceParame()
        {

            try
            {
                //读取测序启动参数(搬运工位)
                if (!File.Exists(@"Serialize\\SequenceParame_carryStation.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(@"Serialize\\SequenceParame_carryStation.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(SequencingStartParam));
                        startParam_carryStation = (SequencingStartParam)serial.Deserialize(fs);
                    }
                }
                //读取测序启动参数（测序仪工位）
                if (!File.Exists(@"Serialize\\SequenceParame_sequencingStation.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(@"Serialize\\SequenceParame_sequencingStation.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(SequencingStartParam));
                        startParam_sequencingStation = (SequencingStartParam)serial.Deserialize(fs);
                    }
                }
                //读取当前测序当前ID号(测序仪工位)
                if (!File.Exists(@"Serialize\\SequenceRunID_sequencingStation.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(@"Serialize\\SequenceRunID_sequencingStation.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(RunID));
                        id_sequencingStation = (RunID)serial.Deserialize(fs);
                    }
                }
                //读取当前测序当前ID号（数据处理线程）
                if (!File.Exists(@"Serialize\\SequenceRunID_dataProcessingStation.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(@"Serialize\\SequenceRunID_dataProcessingStation.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(RunID));
                        id_dataProcessingStation = (RunID)serial.Deserialize(fs);
                    }
                }

                //读取当前芯片质检ID号
                if (!File.Exists(@"Serialize\\SequenceProductCode.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(@"Serialize\\SequenceProductCode.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(ProductCode));
                        mProductCode = (ProductCode)serial.Deserialize(fs);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 序列化测序文件
        /// </summary>
        /// 
        public static bool WriteSequenceParame()
        {
            try
            {
                //保存测序启动参数(搬运工位)
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                if (File.Exists(@"Serialize\\SequenceParame_carryStation.xml"))
                {
                    File.Delete(@"Serialize\\SequenceParame_carryStation.xml");
                }
                using (FileStream fs = new FileStream(@"Serialize\\SequenceParame_carryStation.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(SequencingStartParam));
                    serial.Serialize(fs, startParam_carryStation);
                }
                //保存测序启动参数（测序仪工位）
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                if (File.Exists(@"Serialize\\SequenceParame_sequencingStation.xml"))
                {
                    File.Delete(@"Serialize\\SequenceParame_sequencingStation.xml");
                }
                using (FileStream fs = new FileStream(@"Serialize\\SequenceParame_sequencingStation.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(SequencingStartParam));
                    serial.Serialize(fs, startParam_sequencingStation);
                }
                //保存当前测序当前ID号(测序仪工位)
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                if (File.Exists(@"Serialize\\SequenceRunID_sequencingStation.xml"))
                {
                    File.Delete(@"Serialize\\SequenceRunID_sequencingStation.xml");
                }
                using (FileStream fs = new FileStream(@"Serialize\\SequenceRunID_sequencingStation.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(RunID));
                    serial.Serialize(fs, id_sequencingStation);
                }
                //保存当前测序当前ID号（数据处理线程）
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                if (File.Exists(@"Serialize\\SequenceRunID_dataProcessingStation.xml"))
                {
                    File.Delete(@"Serialize\\SequenceRunID_dataProcessingStation.xml");
                }
                using (FileStream fs = new FileStream(@"Serialize\\SequenceRunID_dataProcessingStation.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(RunID));
                    serial.Serialize(fs, id_dataProcessingStation);
                }
                //保存芯片质检ID号
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                if (File.Exists(@"Serialize\\SequenceProductCode.xml"))
                {
                    File.Delete(@"Serialize\\SequenceProductCode.xml");
                }
                using (FileStream fs = new FileStream(@"Serialize\\SequenceProductCode.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(ProductCode));
                    serial.Serialize(fs, mProductCode);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        /// <summary>
        /// 反序列化IDNA引物参数
        /// </summary>
        /// <returns></returns>
        public static bool ReadIDNAParame()
        {

            try
            {
                // 设备全部记忆参数路径
                if (!File.Exists(@"Serialize\\MachineStatusIDNA.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(@"Serialize\\MachineStatusIDNA.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(CompleteReportingFromControl));
                        completeParam = (CompleteReportingFromControl)serial.Deserialize(fs);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 序列化IDNA引物文件
        /// </summary>
        /// 
        public static bool WriteIDNAParame()
        {
            try
            {
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                // 设备全部记忆参数路径
                if (File.Exists(@"Serialize\\MachineStatusIDNA.xml"))
                {
                    File.Delete(@"Serialize\\MachineStatusIDNA.xml");
                }
                using (FileStream fs = new FileStream(@"Serialize\\MachineStatusIDNA.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serial = new XmlSerializer(typeof(CompleteReportingFromControl));
                    serial.Serialize(fs, completeParam);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 测序当前正在绑定的所有任务字典反序列化
        /// </summary>
        /// <returns></returns>
        public static bool ReadCeXuDic()
        {
            try
            {
                #region 测序当前任务文件拷贝字典读取
                if (!File.Exists(@"Serialize\\MachineCeXuDic.xml"))
                {
                    return false;
                }
                else
                {
                    // 从XML文件中加载列表并转换回字典
                    List<CeXuKeyValue> loadedKeyValueList = new List<CeXuKeyValue>();
                    using (FileStream fileStream = new FileStream(@"Serialize\\MachineCeXuDic.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        // 创建XmlSerializer对象来序列化自定义类的列表
                        XmlSerializer serializer = new XmlSerializer(typeof(List<CeXuKeyValue>));
                        loadedKeyValueList = (List<CeXuKeyValue>)serializer.Deserialize(fileStream);
                    }
                    foreach (var item in loadedKeyValueList)
                    {
                        MyVariable.File_Copy[item.Key_SN] = item.Value_runid;
                    }
                }
                #endregion

                #region 总控传来标签号及碱基字典读取
                if (!File.Exists(@"Serialize\\JianJiDic.xml"))
                {
                    return false;
                }
                else
                {
                    // 从XML文件中加载列表并转换回字典
                    List<JianJiKeyValue> loadedKeyValueData = new List<JianJiKeyValue>();
                    using (FileStream fileStream = new FileStream(@"Serialize\\JianJiDic.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        // 创建XmlSerializer对象来序列化自定义类的列表
                        XmlSerializer serializer1 = new XmlSerializer(typeof(List<JianJiKeyValue>));
                        loadedKeyValueData = (List<JianJiKeyValue>)serializer1.Deserialize(fileStream);
                    }
                    foreach (var item in loadedKeyValueData)
                    {
                        MyVariable.JianJiDic[item.Key_number] = item.Value_JianJi;
                    }
                }
                #endregion

                #region  测序任务解析标签号及推测碱基读取
                if (!File.Exists(@"Serialize\\inferJianJiDic.xml"))
                {
                    return false;
                }
                else
                {
                    // 从XML文件中加载列表并转换回字典
                    List<InferJianJiKeyValue> loadedKeyValueData = new List<InferJianJiKeyValue>();
                    using (FileStream fileStream = new FileStream(@"Serialize\\inferJianJiDic.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        // 创建XmlSerializer对象来序列化自定义类的列表
                        XmlSerializer serializer1 = new XmlSerializer(typeof(List<InferJianJiKeyValue>));
                        loadedKeyValueData = (List<InferJianJiKeyValue>)serializer1.Deserialize(fileStream);
                    }
                    foreach (var item in loadedKeyValueData)
                    {
                        MyVariable.inferJianJiDic[item.Key_inferNumber] = item.Value_inferJianJi;
                    }
                }
                #endregion

                #region  测序任务解析标签号及推测碱基读取
                if (!File.Exists(@"Serialize\\differenceJianJiDic.xml"))
                {
                    return false;
                }
                else
                {
                    // 从XML文件中加载列表并转换回字典
                    List<DifferenceJianJiKeyValue> loadedKeyValueData = new List<DifferenceJianJiKeyValue>();
                    using (FileStream fileStream = new FileStream(@"Serialize\\differenceJianJiDic.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        // 创建XmlSerializer对象来序列化自定义类的列表
                        XmlSerializer serializer1 = new XmlSerializer(typeof(List<DifferenceJianJiKeyValue>));
                        loadedKeyValueData = (List<DifferenceJianJiKeyValue>)serializer1.Deserialize(fileStream);
                    }
                    foreach (var item in loadedKeyValueData)
                    {
                        MyVariable.differenceJianJiDic[item.Key_differenceNumber] = item.Value_differenceJianJi;
                    }
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 测序当前正在绑定的所有任务字典序列化
        /// </summary>
        /// <returns></returns>
        public static bool WriteCeXuDic()
        {
            try
            {
                #region  测序当前任务文件拷贝字典记录
                List<CeXuKeyValue> keyValueList = new List<CeXuKeyValue>();
                foreach (var item in MyVariable.File_Copy)
                {
                    keyValueList.Add(new CeXuKeyValue { Key_SN = item.Key, Value_runid = item.Value });
                }
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                // 设备全部记忆参数路径
                if (File.Exists(@"Serialize\\MachineCeXuDic.xml"))
                {
                    File.Delete(@"Serialize\\MachineCeXuDic.xml");
                }
                // 写入列表到XML文件
                using (FileStream fileStream = new FileStream(@"Serialize\\MachineCeXuDic.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // 创建XmlSerializer对象来序列化自定义类的列表
                    XmlSerializer serializer = new XmlSerializer(typeof(List<CeXuKeyValue>));
                    serializer.Serialize(fileStream, keyValueList);
                }
                #endregion

                #region  总控传来标签号及碱基字典记录
                List<JianJiKeyValue> keyValueData = new List<JianJiKeyValue>();
                foreach (var item in MyVariable.JianJiDic)
                {
                    keyValueData.Add(new JianJiKeyValue { Key_number = item.Key, Value_JianJi = item.Value });
                }
                // 设备全部记忆参数路径
                if (File.Exists(@"Serialize\\JianJiDic.xml"))
                {
                    File.Delete(@"Serialize\\JianJiDic.xml");
                }
                // 写入列表到XML文件
                using (FileStream fileStream1 = new FileStream(@"Serialize\\JianJiDic.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // 创建XmlSerializer对象来序列化自定义类的列表
                    XmlSerializer serializer1 = new XmlSerializer(typeof(List<JianJiKeyValue>));
                    serializer1.Serialize(fileStream1, keyValueData);
                }
                #endregion

                #region  测序任务解析标签号及推测碱基记录
                List<InferJianJiKeyValue> keyValueDatainfer = new List<InferJianJiKeyValue>();
                foreach (var item in MyVariable.inferJianJiDic)
                {
                    keyValueDatainfer.Add(new InferJianJiKeyValue { Key_inferNumber = item.Key, Value_inferJianJi = item.Value });
                }
                // 设备全部记忆参数路径
                if (File.Exists(@"Serialize\\inferJianJiDic.xml"))
                {
                    File.Delete(@"Serialize\\inferJianJiDic.xml");
                }
                // 写入列表到XML文件
                using (FileStream fileStream1 = new FileStream(@"Serialize\\inferJianJiDic.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // 创建XmlSerializer对象来序列化自定义类的列表
                    XmlSerializer serializer1 = new XmlSerializer(typeof(List<InferJianJiKeyValue>));
                    serializer1.Serialize(fileStream1, keyValueDatainfer);
                }
                #endregion

                #region  测序任务解析标签号及推测碱基记录
                List<DifferenceJianJiKeyValue> keyValueDataDifference = new List<DifferenceJianJiKeyValue>();
                foreach (var item in MyVariable.differenceJianJiDic)
                {
                    keyValueDataDifference.Add(new DifferenceJianJiKeyValue { Key_differenceNumber = item.Key, Value_differenceJianJi = item.Value });
                }
                // 设备全部记忆参数路径
                if (File.Exists(@"Serialize\\differenceJianJiDic.xml"))
                {
                    File.Delete(@"Serialize\\differenceJianJiDic.xml");
                }
                // 写入列表到XML文件
                using (FileStream fileStream1 = new FileStream(@"Serialize\\differenceJianJiDic.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // 创建XmlSerializer对象来序列化自定义类的列表
                    XmlSerializer serializer1 = new XmlSerializer(typeof(List<DifferenceJianJiKeyValue>));
                    serializer1.Serialize(fileStream1, keyValueDataDifference);
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        /// <summary>
        /// 反序列化数字孪生参数
        /// </summary>
        /// <returns></returns>
        public static bool ReadAnimationParame()
        {

            try
            {
                // 设备全部记忆参数路径
                if (!File.Exists(@"Serialize\\AnimationParam.xml"))
                {
                    return false;
                }
                else
                {
                    using (FileStream fs = new FileStream(@"Serialize\\AnimationParam.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        XmlSerializer serials = new XmlSerializer(typeof(AnimationClass));
                        animationParam = (AnimationClass)serials.Deserialize(fs);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 序列化数字孪生文件
        /// </summary>
        /// 
        public static bool WriteAnimationParame()
        {
            try
            {
                if (!Directory.Exists(Application.StartupPath + @"\Serialize\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Serialize\");
                }
                // 设备全部记忆参数路径
                if (File.Exists(@"Serialize\\AnimationParam.xml"))
                {
                    File.Delete(@"Serialize\\AnimationParam.xml");
                }
                using (FileStream fs = new FileStream(@"Serialize\\AnimationParam.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    XmlSerializer serials = new XmlSerializer(typeof(AnimationClass));
                    serials.Serialize(fs, animationParam);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }


    public class RobClawParam
    {
        #region 改机电动夹爪参数
        public  string robClaw_Com;
        public  int robClaw_Baudrate;
        public  int robClaw_Databits;
        public  string robClaw_Parity;
        public  string robClaw_Stopbits;
        #endregion
    }
    /// <summary>
    /// 测序任务键值类
    /// </summary>
    public class CeXuKeyValue
    {
        public string Key_SN { get; set; }
        public string Value_runid { get; set; }
    }
    /// <summary>
    /// 数据分析碱基键值类
    /// </summary>
    public class JianJiKeyValue
    {
        public int Key_number { get; set; }
        public string Value_JianJi { get; set; }
    }
    /// <summary>
    /// 标签号以及推测碱基存放集合
    /// </summary>
    public class InferJianJiKeyValue
    {
        public int Key_inferNumber { get; set; }
        public string Value_inferJianJi { get; set; }
    }
    /// <summary>
    /// 推测碱基和总控传来碱基不同存放集合
    /// </summary>
    public class DifferenceJianJiKeyValue
    {
        public int Key_differenceNumber { get; set; }
        public string Value_differenceJianJi { get; set; }
    }

}
