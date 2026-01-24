using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Configuration;
using System.Management;
using System.Drawing;
using System.Threading;
using System.Data;
using System.Security.Cryptography;

namespace CYStandardProcedure
{
    /***********************
     * 现将Hive过程文件存放在SQLite文件数据库中 全部存放在D：/SystemData/Hive/HiveData.db文件中
     * 
     * 
     * Hive状态切换存放在HiveStateTable表中    列名    切换时间（主键）   状态
     * 
     * 
     * Hive报警信息存放在HiveErrorMsgTable表中 列名  时间（主键） 报警信息 报警分类  
     * 
     * 
     * 
     * 
     * ******************/

    /************************
     * 修改记录(1.6版本)
     * 修改报警信息上传才添加到数据库中（原来为只要有报警信息就添加到数据库中）
     * 修改机台状态时间显示（显示所有的数值）
     * 增加Hive时间统计当天详细信息显示
     * 
     * 
     * 
     * 修改记录(1.6.2版本)
     * 修改机台状态时间显示小数点位数为两位
     * 修改上传报警记录，替换掉所有的中文字符 
     *
     *
     *修改记录(1.6.3版本)
     *修改机台上传时候执行异步方法
     *
     * **************/

    public class Hive
    {
        /// <summary>
        /// 构造函数，初始化APP.config文件
        /// </summary>
        /// <param name="AppPath"> 文件路径，一般默认为@"..\..\App.config"</param>
        public Hive(string AppPath = @"App.config")
        {
            /************初始化Hive数据库***************/
            ExeConfigurationFileMap execonfig = new ExeConfigurationFileMap();
            execonfig.ExeConfigFilename = AppPath;
            AppCfg = ConfigurationManager.OpenMappedExeConfiguration(execonfig, ConfigurationUserLevel.None);

            /***************************
             * APP.config 文件信息对应
             * HiveStatus------ Hive机台状态
             * PreHiveStatus----Hive上次机台状态
             * ErrorMessage------报警信息
             * ErrorTime--------报警发生时间
             * ErrorCode--------故障代码
             * ErrorSeverity----故障等级
             * ErrorStatus------引发报警时机台状态
             * StatusChangeTime---上次机台切换时间
             * Version-------------软件版本号
             * Guid-------------当前软件版本号对应Guid号
             * UUID----------------当前版本对应的唯一编码
             * BosID---------------硬盘编号
             * *******************/
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("HiveStatus"))
            {
                AppCfg.AppSettings.Settings.Add("HiveStatus", _HiveMachineStaus.空闲状态.ToString());
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("PreHiveStatus"))
            {
                AppCfg.AppSettings.Settings.Add("PreHiveStatus", _HiveMachineStaus.空闲状态.ToString());
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("ErrorMessage"))
            {
                AppCfg.AppSettings.Settings.Add("ErrorMessage", "null");
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("ErrorTime"))
            {
                AppCfg.AppSettings.Settings.Add("ErrorTime", "null");
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("ErrorCode"))
            {
                AppCfg.AppSettings.Settings.Add("ErrorCode", "null");
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("ErrorSeverity"))
            {
                AppCfg.AppSettings.Settings.Add("ErrorSeverity", "null");
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("ErrorStatus"))
            {
                AppCfg.AppSettings.Settings.Add("ErrorStatus", "1");
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("StatusChangeTime"))
            {
                AppCfg.AppSettings.Settings.Add("StatusChangeTime", DateTime.Now.ToString());
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("Version"))
            {
                AppCfg.AppSettings.Settings.Add("Version", "CY_0.1.1.1_230627_PRO");
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("Guid"))
            {
                AppCfg.AppSettings.Settings.Add("Guid", Assembly.LoadFile(this.GetType().Assembly.Location).ManifestModule.ModuleVersionId.ToString());
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("UUID"))
            {
                AppCfg.AppSettings.Settings.Add("UUID", Guid.NewGuid().ToString("N"));
            }
            if (!AppCfg.AppSettings.Settings.AllKeys.Contains("BosID"))
            {
                AppCfg.AppSettings.Settings.Add("BosID", "");
            }
            AppCfg.Save();
            mHiveStatus = (_HiveMachineStaus)Enum.Parse(typeof(_HiveMachineStaus), AppCfg.AppSettings.Settings["HiveStatus"].Value);
            mPreHiveStatus = (_HiveMachineStaus)Enum.Parse(typeof(_HiveMachineStaus), AppCfg.AppSettings.Settings["PreHiveStatus"].Value);


            /*****************连接数据库，判断数据表是否存在********************/
            if (!Directory.Exists(mHiveSqlitePath))
            {
                Directory.CreateDirectory(mHiveSqlitePath);
            }
            HiveSqlite = new SQLiteHelper(mHiveSqlitePath + mHiveDataBaseName);
            string sqlitestr = $"select count(*) from sqlite_master where type='table' and name='{mHiveStatusTable}'";
            if (Convert.ToInt16(HiveSqlite.GetDataSet(sqlitestr).Tables[0].Rows[0].ItemArray[0]) < 1)
            {
                sqlitestr = $"create table  if not exists {mHiveStatusTable} (切换时间 Text primary key not null ,时间戳 INTEGER  not null, 状态 Text not null) ";
                HiveSqlite.ExecSQLResult(sqlitestr);
            }
            sqlitestr = $"select count(*) from sqlite_master where type='table' and name='{mHiveErrorMsgTable }'";
            if (Convert.ToInt16(HiveSqlite.GetDataSet(sqlitestr).Tables[0].Rows[0].ItemArray[0]) < 1)
            {
                sqlitestr = $"create table  if not exists {mHiveErrorMsgTable} ( 时间 Text primary key not null ,时间戳 INTEGER  not null, 报警信息 Text not null,报警类别 Text not null) ";
                HiveSqlite.ExecSQLResult(sqlitestr);
            }
        }


        #region 局部变量
        /// <summary>
        /// Hive数据库操作类
        /// </summary>
        private SQLiteHelper HiveSqlite;
        /// <summary>
        /// Hive数据库文件存放路径
        /// </summary>
        private const string mHiveSqlitePath = @"D:\SystemData\Hive\";
        /// <summary>
        /// Hive数据库名称
        /// </summary>
        private const string mHiveDataBaseName = "HiveData.db";
        /// <summary>
        /// Hive数据库存放Hive状态切换的数据表名称
        /// </summary>
        private const string mHiveStatusTable = "HiveStateTable";
        /// <summary>
        /// Hive数据库存放报警信息数据表名称
        /// </summary>
        private const string mHiveErrorMsgTable = "HiveErrorMsgTable";
        /// <summary>
        /// 一周自动运行时间统计
        /// </summary>
        private double[] mWeekRunTime = new double[7];
        /// <summary>
        /// 一周自动空闲时间统计
        /// </summary>
        private double[] mWeekIdleTime = new double[7];
        /// <summary>
        /// 一周工程模式时间统计
        /// </summary>
        private double[] mWeekEngineTime = new double[7];

        /// <summary>
        /// 一周计划停机时间统计
        /// </summary>
        private double[] mWeekPlannedTime = new double[7];

        /// <summary>
        /// 一周宕机时间统计
        /// </summary>
        private double[] mWeekDownTime = new double[7];

        /// <summary>
        /// 计划停机分类信息存放
        /// </summary>
        private Dictionary<string, HivePlannedInfo> mPlannedInfo = new Dictionary<string, HivePlannedInfo>();
        /// <summary>
        /// 关键参数展示存放字典
        /// </summary>
        private Dictionary<string, DashboardParame> mDashboardDic = new Dictionary<string, DashboardParame>();
        /// <summary>
        /// 报警详细信息字典_CH
        /// </summary>
        private Dictionary<string, HiveErrorConfigInfo> mHiveErrorDic_CH = new Dictionary<string, HiveErrorConfigInfo>();
        /// <summary>
        /// 报警详细信息字典_EN
        /// </summary>
        private Dictionary<string, HiveErrorConfigInfo> mHiveErrorDic_EN = new Dictionary<string, HiveErrorConfigInfo>();
        /// <summary>
        /// 报警详细信息字典_VN
        /// </summary>
        private Dictionary<string, HiveErrorConfigInfo> mHiveErrorDic_VN = new Dictionary<string, HiveErrorConfigInfo>();
        /// <summary>
        /// Hive配置信息字典
        /// </summary>
        private Dictionary<string, HiveConfigInfo> mHiveConfigInfo = new Dictionary<string, HiveConfigInfo>();


        /// <summary>
        /// Hive配置字典信息
        /// </summary>
        public Dictionary<string, HiveConfigInfo> HiveConfigInfo
        {
            get { return mHiveConfigInfo; }
            set { mHiveConfigInfo = value; }
        }
        /// <summary>
        /// Hive报警信息大分类字典
        /// </summary>
        private Dictionary<string, string> mErrorCateDic = new Dictionary<string, string>();
        /// <summary>
        /// 保存最近一个月时间内报警分类统计
        /// </summary>
        private List<Dictionary<string, int>> mErrorStatistics = new List<Dictionary<string, int>>();
        /// <summary>
        /// Hive状态Run和Idle状态切换间隔
        /// </summary>
        private double mChangeTime;
        /// <summary>
        /// App.config配置文件操作
        /// </summary>
        private static Configuration AppCfg;
        /// <summary>
        /// 未在报警信息字典中找到的报警信息
        /// </summary>
        private static List<string> MissErrMsg = new List<string>();
        /// <summary>
        /// Hivexml配置文件键名集合
        /// </summary>
        private static string[] mHiveConfigInfostr = new string[] { "上传类型", "url", "屏蔽上传" };


        private static string mConfigFilePath;


        private static string mMessage_id;

        #endregion


        #region Hive统计局部方法



        #region Sqlite 统计数据





        private void WriteHiveError(string errorcode, string msg)
        {

            string Category = "";
            if (!mErrorCateDic.ContainsKey(errorcode.Substring(0, 1)))
            {
                errorcode = "O990000-01-01";
                Category = mErrorCateDic["O"];
                msg = "Other Alarms";
            }
            else
            {
                Category = mErrorCateDic[errorcode.Substring(0, 1)];
            }
            double ss = ChangeTimeSlice(DateTime.Now);
            string sqlitestr = $"insert into {mHiveErrorMsgTable } values ('{DateTime.Now.ToString()}',{ss},'{msg }','{Category}')";
            if (HiveSqlite.ExecSQLResult(sqlitestr) < 1)
            {
                Thread.Sleep(1000);
                sqlitestr = $"insert into {mHiveErrorMsgTable } values ('{DateTime.Now.ToString()}','{msg }','{Category}')";
                HiveSqlite.ExecSQLResult(sqlitestr);
            }
        }


        #endregion 


        /// <summary>
        /// 向配置文件中添加不包含的报警信息
        /// </summary>
        private void WriteMissErr()
        {
            try
            {
                string path = Application.StartupPath + @"\ExeFile\Hive\MissError.xlsx";
                if (MissErrMsg.Count > 0)
                {
                    XSSFWorkbook WK = new XSSFWorkbook();
                    ISheet sheet = WK.CreateSheet("Alarm");
                    for (int i = 0; i < MissErrMsg.Count; i++)
                    {
                        sheet.CreateRow(i).CreateCell(0).SetCellValue(MissErrMsg[i]);
                    }
                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        WK.Write(fs);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Hive上传机台状态
        /// </summary>
        /// <param name="NewStatus"></param>
        private void UpDataStatus(_HiveMachineStaus NewStatus)
        {
            try
            {
                if (NewStatus == _HiveMachineStaus.正常做料状态 || NewStatus == _HiveMachineStaus.屏蔽上传做料状态)
                {
                    if (mHiveStatus == _HiveMachineStaus.宕机状态)
                    {
                        if (!mHiveConfigInfo[_HiveUploadType.ErrorData.ToString()].Shiled)
                        {
                            UpDataError();
                        }

                    }
                    HiveMachineStatusInfo1 msg = new HiveMachineStatusInfo1();
                    HiveMachineStatusData1 data = new HiveMachineStatusData1();
                    msg.machine_state = (Convert.ToInt16(NewStatus) + 1).ToString();
                    msg.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    data.state = (Convert.ToInt16(NewStatus) + 1).ToString();
                    data.message_id = mMessage_id;
                    msg.data = data;
                    string SendHive = JsonConvert.SerializeObject(msg);//根据结构体序列化信息
                    HiveLog.WriteHiveStatusLog("Send To Hive:  " + SendHive);
                    string Rec = HTTPPostMsg(mHiveConfigInfo[_HiveUploadType.MachineState.ToString()].url, SendHive);
                    HiveLog.WriteHiveStatusLog("Receive From Hive:  " + Rec);
                }
                else if (NewStatus == _HiveMachineStaus.空闲状态)
                {
                    HiveMachineStatusInfo1 msg = new HiveMachineStatusInfo1();
                    HiveMachineStatusData1 data = new HiveMachineStatusData1();
                    msg.machine_state = (Convert.ToInt16(NewStatus) + 1).ToString();
                    msg.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    data.state = (Convert.ToInt16(NewStatus) + 1).ToString();
                    data.message_id = mMessage_id;
                    msg.data = data;
                    string SendHive = JsonConvert.SerializeObject(msg);//根据结构体序列化信息
                    HiveLog.WriteHiveStatusLog("Send To Hive:  " + SendHive);
                    string Rec = HTTPPostMsg(mHiveConfigInfo[_HiveUploadType.MachineState.ToString()].url, SendHive);
                    HiveLog.WriteHiveStatusLog("Receive From Hive:  " + Rec);
                }
                else if (NewStatus == _HiveMachineStaus.计划停机状态)
                {
                    HiveMachineStatusInfo2 msg = new HiveMachineStatusInfo2();
                    HiveMachineStatusData2 data = new HiveMachineStatusData2();
                    msg.machine_state = (Convert.ToInt16(NewStatus) + 1).ToString();
                    msg.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    data.state = (Convert.ToInt16(NewStatus) + 1).ToString();
                    data.message_id = mMessage_id;
                    data.code = mHivePlannedCode;
                    data.error_message = mPlannedInfo[mHivePlannedCode].Msg.Replace(",", " ").Replace("，", " ").Replace("!", " ").Replace("！", " ");
                    data.MS_SHA1 = mUUID;
                    data.sw_version = mSw_Version;
                    data.previous_state = (Convert.ToInt16(mHiveStatus) + 1).ToString();
                    data.erroe_detail = data.error_message;
                    data.badge = mUserID;
                    data.CD_SHA1 = GetFileHash(mConfigFilePath);
                    msg.data = data;
                    string SendHive = JsonConvert.SerializeObject(msg);//根据结构体序列化信息
                    HiveLog.WriteHiveStatusLog("Send To Hive:  " + SendHive);
                    string Rec = HTTPPostMsg(mHiveConfigInfo[_HiveUploadType.MachineState.ToString()].url, SendHive);
                    HiveLog.WriteHiveStatusLog("Receive From Hive:  " + Rec);
                }
                else if (NewStatus == _HiveMachineStaus.宕机状态)
                {
                    HiveMachineStatusInfo2 msg = new HiveMachineStatusInfo2();
                    HiveMachineStatusData2 data = new HiveMachineStatusData2();
                    msg.machine_state = (Convert.ToInt16(NewStatus) + 1).ToString();
                    msg.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));

                    data.state = (Convert.ToInt16(NewStatus) + 1).ToString();
                    data.message_id = mMessage_id;
                    data.code = AppCfg.AppSettings.Settings["ErrorCode"].Value;
                    data.error_message = AppCfg.AppSettings.Settings["ErrorMessage"].Value.Replace(",", " ").Replace("，", " ").Replace("!", " ").Replace("！", " ");
                    data.MS_SHA1 = mUUID;
                    data.sw_version = mSw_Version;
                    data.previous_state = (Convert.ToInt16(mHiveStatus) + 1).ToString();
                    data.erroe_detail = data.error_message;
                    data.badge = mUserID;
                    data.CD_SHA1 = GetFileHash(mConfigFilePath);
                    msg.data = data;
                    string SendHive = JsonConvert.SerializeObject(msg);//根据结构体序列化信息
                    HiveLog.WriteHiveStatusLog("Send To Hive:  " + SendHive);
                    string Rec = HTTPPostMsg(mHiveConfigInfo[_HiveUploadType.MachineState.ToString()].url, SendHive);

                    HiveLog.WriteHiveStatusLog("Receive From Hive:  " + Rec);
                }
            }
            catch (Exception ex)
            {
                HiveLog.WriteHiveStatusLog("ReceiveError:  " + ex.Message);
            }
        }


        /// <summary>
        /// 上传HIve报警记录
        /// </summary>
        private void UpDataError()
        {
            try
            {
                WriteHiveError(AppCfg.AppSettings.Settings["ErrorCode"].Value, AppCfg.AppSettings.Settings["ErrorMessage"].Value);
                if (UpdataErrorStatis != null)
                {
                    UpdataErrorStatis.Invoke();
                }
                HiveMachineErrorInfo msg = new HiveMachineErrorInfo();
                HiveMachineErrorData data = new HiveMachineErrorData();
                msg.message = AppCfg.AppSettings.Settings["ErrorMessage"].Value.Replace(",", " ").Replace("，", " ").Replace("!", " ").Replace("！", " ");

                msg.code = AppCfg.AppSettings.Settings["ErrorCode"].Value;


                msg.occurrence_time = AppCfg.AppSettings.Settings["ErrorTime"].Value;
                msg.severity = AppCfg.AppSettings.Settings["ErrorSeverity"].Value;
                msg.resolved_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                data.hive_state = AppCfg.AppSettings.Settings["ErrorStatus"].Value;
                data.error_detail = AppCfg.AppSettings.Settings["ErrorCode"].Value;
                msg.data = data;
                AppCfg.AppSettings.Settings["ErrorMessage"].Value = "null";
                string SendHive = JsonConvert.SerializeObject(msg);//根据结构体序列化信息
                HiveLog.WriteHiveErrorLog("Send To Hive:  " + SendHive);
                string Rec = HTTPPostMsg(mHiveConfigInfo[_HiveUploadType.ErrorData.ToString()].url, SendHive);
                //string Rec = "OK";
                HiveLog.WriteHiveErrorLog("Receive From Hive:  " + Rec);
            }
            catch (Exception ex)
            {
                HiveLog.WriteHiveErrorLog("ReceiveError:  " + ex.Message);
            }
        }



        /// <summary>
        /// 机台运行状态和空闲状态切换
        /// </summary>
        private void RefeshIdleAndRun()
        {
            while (true)
            {
                Thread.Sleep(5);
                if (mHiveStatus == _HiveMachineStaus.正常做料状态 || mHiveStatus == _HiveMachineStaus.屏蔽上传做料状态)
                {
                    if ((DateTime.Now - mInputTime).TotalSeconds > mChangeTime)
                    {
                        HiveStatus = _HiveMachineStaus.空闲状态;
                    }
                }
                else if (mHiveStatus == _HiveMachineStaus.空闲状态)
                {
                    if ((DateTime.Now - mInputTime).TotalSeconds <= mChangeTime)
                    {
                        if (m_ShiledUpload)
                        {
                            HiveStatus = _HiveMachineStaus.屏蔽上传做料状态;
                        }
                        else
                        {
                            HiveStatus = _HiveMachineStaus.正常做料状态;
                        }
                    }
                }

            }
        }





        /// <summary>
        /// 获取指定文件的哈希值
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        private string GetFileHash(string path)
        {
            var hash = SHA256.Create();
            var stream = new FileStream(path, FileMode.Open);
            byte[] hashbyte = hash.ComputeHash(stream);
            stream.Close();
            return BitConverter.ToString(hashbyte).Replace("-", "");
        }

        #region 从配置文件中读取Hive系统的相关信息
        /// <summary>
        /// 读取Hive配置信息
        /// </summary>
        /// <returns></returns>
        private bool ReadHiveConfigInfo()
        {
            try
            {
                string filepath = Application.StartupPath + @"\ExeFile\Hive\HiveCfg.xml";
                if (!File.Exists(filepath))
                {
                    return false;
                }
                else
                {
                    this.mHiveConfigInfo.Clear();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filepath);
                    XmlNode node = doc.SelectSingleNode("/SystemHardWareCfg/Hive");
                    if (node.HasChildNodes)
                    {
                        XmlNodeList mlistNode = node.ChildNodes;
                        for (int i = 0; i < mlistNode.Count; i++)
                        {
                            XmlElement el = mlistNode[i] as XmlElement;
                            HiveConfigInfo info = new HiveConfigInfo();
                            info.type = el.GetAttribute(mHiveConfigInfostr[0]);
                            info.url = el.GetAttribute(mHiveConfigInfostr[1]);
                            info.Shiled = Convert.ToBoolean(int.Parse(el.GetAttribute(mHiveConfigInfostr[2])));
                            this.mHiveConfigInfo.Add(info.type, info);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }





        /// <summary>
        /// 读取Hive报警上传配置信息（从xlsx文件中读取）
        /// </summary>
        /// <returns></returns>
        private bool ReadHiveErrorConfigInfoXLS()
        {
            try
            {
                string filepath = Application.StartupPath + @"\ExeFile\Hive\HiveError.xlsx";
                if (!File.Exists(filepath))
                {
                    return false;
                }
                else
                {
                    mHiveErrorDic_CH.Clear();
                    mHiveErrorDic_EN.Clear();
                    mHiveErrorDic_VN.Clear();
                    mErrorCateDic.Clear();
                    mDashboardDic.Clear();
                    XSSFWorkbook WK;
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        WK = new XSSFWorkbook(fs);
                    }
                    ISheet sheet = WK.GetSheet("ErrorCode");
                    int RowCount = sheet.LastRowNum + 1;
                    for (int i = 0; i < RowCount; i++)
                    {
                        HiveErrorConfigInfo info = new HiveErrorConfigInfo();
                        info.AlarmCode = sheet.GetRow(i).GetCell(0).StringCellValue;
                        info.ErrorDescription_CH = sheet.GetRow(i).GetCell(1).StringCellValue;
                        info.ErrorDescription_EN = sheet.GetRow(i).GetCell(2).StringCellValue;
                        info.ErrorDescription_VN = sheet.GetRow(i).GetCell(3).StringCellValue;
                        info.ErrorMsg = sheet.GetRow(i).GetCell(4).StringCellValue;
                        info.Severity = sheet.GetRow(i).GetCell(5).StringCellValue;
                        if (!mHiveErrorDic_CH.ContainsKey(info.ErrorDescription_CH))
                        {
                            mHiveErrorDic_CH.Add(info.ErrorDescription_CH, info);
                        }
                        if (!mHiveErrorDic_EN.ContainsKey(info.ErrorDescription_EN))
                        {
                            mHiveErrorDic_EN.Add(info.ErrorDescription_EN, info);
                        }
                        if (!mHiveErrorDic_VN.ContainsKey(info.ErrorDescription_VN))
                        {
                            mHiveErrorDic_VN.Add(info.ErrorDescription_VN, info);
                        }
                    }
                    sheet = WK.GetSheet("ErrorCategory");
                    for (int i = 0; i < sheet.LastRowNum + 1; i++)
                    {
                        if (!mErrorCateDic.ContainsKey(sheet.GetRow(i).GetCell(0).ToString()))
                        {
                            mErrorCateDic.Add(sheet.GetRow(i).GetCell(1).ToString(), sheet.GetRow(i).GetCell(0).ToString());
                        }
                    }

                }

                filepath = Application.StartupPath + @"\ExeFile\Hive\HiveParame.xlsx";
                if (!File.Exists(filepath))
                {
                    return false;
                }
                else
                {
                    mPlannedInfo.Clear();
                    XSSFWorkbook WK;
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        WK = new XSSFWorkbook(fs);
                    }
                    ISheet sheet = WK.GetSheet("Planned");
                    int RowCount = sheet.LastRowNum + 1;
                    for (int i = 0; i < RowCount; i++)
                    {
                        HivePlannedInfo info = new HivePlannedInfo();
                        info.Code = sheet.GetRow(i).GetCell(0).ToString();
                        info.Msg = sheet.GetRow(i).GetCell(1).ToString();
                        info.Detail = sheet.GetRow(i).GetCell(2).ToString();
                        if (!mPlannedInfo.ContainsKey(info.Code))
                        {
                            mPlannedInfo.Add(info.Code, info);
                        }
                    }
                    sheet = WK.GetSheet("Parame");
                    mSw_Version = sheet.GetRow(0).GetCell(1).ToString();
                    mMS_SHA1 = sheet.GetRow(1).GetCell(1).ToString();
                    mVS_SHA1 = sheet.GetRow(2).GetCell(1).ToString();
                    mUserID = sheet.GetRow(3).GetCell(1).ToString();
                    mOldsn = sheet.GetRow(4).GetCell(1).ToString();
                    mNewsn = sheet.GetRow(5).GetCell(1).ToString();
                    mChangeTime = Convert.ToDouble(sheet.GetRow(6).GetCell(1).ToString());
                    mAdminPath = sheet.GetRow(7).GetCell(1).ToString();
                    mHiveSite = sheet.GetRow(8).GetCell(1).ToString();
                    mConfigFilePath = sheet.GetRow(9).GetCell(1).ToString();
                    mMessage_id = sheet.GetRow(10).GetCell(1).ToString();
                    sheet = WK.GetSheet("Dashboard");
                    int count = sheet.LastRowNum + 1;
                    for (int i = 1; i < count; i++)
                    {
                        DashboardParame info = new DashboardParame();
                        info.KeyName = sheet.GetRow(i).GetCell(0).ToString();
                        info.Value = sheet.GetRow(i).GetCell(1).ToString();
                        info.LSL = sheet.GetRow(i).GetCell(2).ToString();
                        info.USL = sheet.GetRow(i).GetCell(3).ToString();
                        mDashboardDic.Add(info.KeyName, info);
                    }




                }
                return true;
            }
            catch (Exception EX)
            {
                return false;
            }
        }


        /// <summary>
        /// 读取缺失的报警信息
        /// </summary>
        /// <returns></returns>
        private bool ReadMissErrorMsg()
        {
            string path = Application.StartupPath + @"\ExeFile\Hive\MissError.xlsx";
            MissErrMsg = new List<string>();
            try
            {
                if (!File.Exists(path))
                {
                    return true;
                }
                XSSFWorkbook WK;
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    WK = new XSSFWorkbook(fs);
                }
                ISheet sheet = WK.GetSheetAt(0);
                for (int i = 0; i < sheet.LastRowNum + 1; i++)
                {
                    MissErrMsg.Add(sheet.GetRow(i).GetCell(0).ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion










        #region Hive系统上传通讯方法
        /// <summary>
        /// 输入url + msg  返回查询结果
        /// </summary>
        /// <param name="purl"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private string HTTPPostGetMsg(string purl, string str)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(purl + str);
                request.Method = "GET";
                request.Timeout = 100;
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string receiveMsg = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return receiveMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Http POST 方法
        /// </summary>
        /// <param name="purl"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private string HTTPPostMsg(string purl, string str)
        {
            try
            {
                string StrDate = "";
                string strValue = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(purl);
                request.Method = "POST";
                request.Timeout = 100;
                request.ContentType = "application/json";
                var MemStream = new MemoryStream();
                var DataBytes = Encoding.UTF8.GetBytes(str);
                MemStream.Write(DataBytes, 0, DataBytes.Length);

                request.ContentLength = MemStream.Length;
                request.Proxy = null;
                request.ServicePoint.Expect100Continue = false;
                Stream WriterValue = request.GetRequestStream();

                MemStream.Position = 0;
                var BufferValue = new byte[MemStream.Length];
                MemStream.Read(BufferValue, 0, BufferValue.Length);
                MemStream.Close();

                WriterValue.Write(BufferValue, 0, BufferValue.Length);
                WriterValue.Close();
                HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                Stream s = response.GetResponseStream();
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate;
                }
                response.Close();
                return strValue;
            }
            catch (Exception EX)
            {
                return EX.Message;
            }
        }
        #endregion






        #region 机台软件版本号以及机台唯一编码对应方法





        /// <summary>
        /// 判断机台版本号
        /// </summary>
        private void CheckVersion()
        {
            if (mSw_Version == AppCfg.AppSettings.Settings["Version"].Value)
            {
                if (AppCfg.AppSettings.Settings["BosID"].Value == GetHardDiskSerialNumber())
                {
                    if (AppCfg.AppSettings.Settings["UUID"].Value == "")
                    {
                        AppCfg.AppSettings.Settings["UUID"].Value = Guid.NewGuid().ToString("N");
                        mUUID = AppCfg.AppSettings.Settings["UUID"].Value;
                        AppCfg.AppSettings.Settings["Guid"].Value = Assembly.LoadFile(this.GetType().Assembly.Location).ManifestModule.ModuleVersionId.ToString();
                    }
                    else
                    {
                        AppCfg.AppSettings.Settings["Guid"].Value = Assembly.LoadFile(this.GetType().Assembly.Location).ManifestModule.ModuleVersionId.ToString();
                        mUUID = AppCfg.AppSettings.Settings["UUID"].Value;
                    }
                }
                else
                {
                    AppCfg.AppSettings.Settings["BosID"].Value = GetHardDiskSerialNumber();
                    AppCfg.AppSettings.Settings["UUID"].Value = Guid.NewGuid().ToString("N");
                    mUUID = AppCfg.AppSettings.Settings["UUID"].Value;
                    AppCfg.AppSettings.Settings["Guid"].Value = Assembly.LoadFile(this.GetType().Assembly.Location).ManifestModule.ModuleVersionId.ToString();
                }
            }
            else
            {
                if (AppCfg.AppSettings.Settings["Guid"].Value == Assembly.LoadFile(this.GetType().Assembly.Location).ManifestModule.ModuleVersionId.ToString())
                {
                    mSw_Version = AppCfg.AppSettings.Settings["Version"].Value;
                    XSSFWorkbook wk;
                    using (FileStream fs = new FileStream(Application.StartupPath + @"\ExeFile\Hive\HiveParame.xlsx", FileMode.Open, FileAccess.Read))
                    {
                        wk = new XSSFWorkbook(fs);
                    }
                    ISheet sheet = wk.GetSheet("Parame");
                    sheet.GetRow(0).CreateCell(1).SetCellValue(mSw_Version);
                    using (FileStream fs = new FileStream(Application.StartupPath + @"\ExeFile\Hive\HiveParame.xlsx", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        wk.Write(fs);
                    }
                }
                else
                {
                    string aapser = AppCfg.AppSettings.Settings["Version"].Value;
                    if (int.Parse(mSw_Version.Split('_')[1].Split('.')[0]) <= int.Parse(aapser.Split('_')[1].Split('.')[0]))
                    {
                        mSw_Version = $"{mSw_Version.Split('_')[0]}_{int.Parse(aapser.Split('_')[1].Split('.')[0]) + 1}.{aapser.Split('_')[1].Split('.')[1]}.{aapser.Split('_')[1].Split('.')[2]}.{aapser.Split('_')[1].Split('.')[3]}_{mSw_Version.Split('_')[2]}_{mSw_Version.Split('_')[3]}";
                        XSSFWorkbook wk;
                        using (FileStream fs = new FileStream(Application.StartupPath + @"\ExeFile\Hive\HiveParame.xlsx", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            wk = new XSSFWorkbook(fs);
                        }
                        ISheet sheet = wk.GetSheet("Parame");
                        sheet.GetRow(0).CreateCell(1).SetCellValue(mSw_Version);
                        if (File.Exists(Application.StartupPath + @"\ExeFile\Hive\HiveParame.xlsx"))
                        {
                            File.Delete(Application.StartupPath + @"\ExeFile\Hive\HiveParame.xlsx");
                        }
                        using (FileStream fs = new FileStream(Application.StartupPath + @"\ExeFile\Hive\HiveParame.xlsx", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            wk.Write(fs);
                        }
                    }

                    AppCfg.AppSettings.Settings["BosID"].Value = GetHardDiskSerialNumber();
                    AppCfg.AppSettings.Settings["UUID"].Value = Guid.NewGuid().ToString("N");
                    mUUID = AppCfg.AppSettings.Settings["UUID"].Value;
                    AppCfg.AppSettings.Settings["Guid"].Value = Assembly.LoadFile(this.GetType().Assembly.Location).ManifestModule.ModuleVersionId.ToString();
                    AppCfg.AppSettings.Settings["Version"].Value = mSw_Version;
                }
            }
            AppCfg.Save();
        }



        /// <summary>
        /// 获取电脑硬盘序列号
        /// </summary>
        /// <returns></returns>
        private string GetHardDiskSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                string sHardDiskSerialNumber = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    sHardDiskSerialNumber = mo["SerialNumber"].ToString().Trim();
                    break;
                }
                return sHardDiskSerialNumber;
            }
            catch
            {
                return "";
            }
        }
        #endregion



        #endregion





        #region 前台界面信息交互

        /// <summary>
        /// 将时间戳转化为时间
        /// </summary>
        /// <param name="TimeSlice">时间戳</param>
        /// <returns></returns>
        private DateTime ChangeToTime(double TimeSlice)
        {
            DateTime starttime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return starttime.AddSeconds(TimeSlice);
        }

        /// <summary>
        /// 时间转化为时间戳
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns></returns>
        private double ChangeTimeSlice(DateTime dt)
        {
            DateTime starttime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (dt - starttime).TotalSeconds;
        }




        /// <summary>
        /// 从数据库中读取Hive状态时间
        /// </summary>
        private void GetHiveStatusTime()
        {
            for (int i = 0; i < 7; i++)
            {
                mWeekRunTime[i] = 0;
                mWeekIdleTime[i] = 0;
                mWeekEngineTime[i] = 0;
                mWeekPlannedTime[i] = 0;
                mWeekDownTime[i] = 0;
            }
            DateTime time = new DateTime();
            for (int i = 0; i < 7; i++)
            {
                time = DateTime.Now.AddDays(0 - i);
                string sqlitestr;
                if (i == 0)
                {
                    sqlitestr = $"select 切换时间,状态 from {mHiveStatusTable} where 时间戳 between {ChangeTimeSlice(time.Date)} and {ChangeTimeSlice(time) }   order by 时间戳 asc ";
                }
                else
                {
                    sqlitestr = $"select 切换时间,状态 from {mHiveStatusTable} where 时间戳 between {ChangeTimeSlice(time.Date)} and {ChangeTimeSlice(time.AddDays(1).Date) }   order by 时间戳 asc ";
                }


                DataTable dt = new DataTable();
                dt = HiveSqlite.GetDataSet(sqlitestr).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    mWeekRunTime[i] = 0;
                    mWeekIdleTime[i] = 0;
                    mWeekEngineTime[i] = 0;
                    mWeekPlannedTime[i] = 0;
                    mWeekDownTime[i] = 0;
                    sqlitestr = $"select 切换时间,状态  from {mHiveStatusTable} where 时间戳 < '{ChangeTimeSlice(time.Date)}'order by 时间戳 desc";
                    dt = new DataTable();
                    dt = HiveSqlite.GetDataSet(sqlitestr).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        if (i == 0)
                        {
                            mWeekIdleTime[i] = (DateTime.Now - DateTime.Now.Date).TotalMinutes;
                        }
                        else
                        {
                            mWeekIdleTime[i] = 24 * 60;
                        }

                    }
                    else
                    {
                        switch (dt.Rows[i].ItemArray[1].ToString())
                        {
                            case "正常做料状态":
                                if (i == 0)
                                {
                                    mWeekRunTime[i] += (DateTime.Now - DateTime.Now.Date).TotalMinutes;
                                }
                                else
                                {
                                    mWeekRunTime[i] += 24 * 60;
                                }

                                break;
                            case "空闲状态":
                                if (i == 0)
                                {
                                    mWeekIdleTime[i] += (DateTime.Now - DateTime.Now.Date).TotalMinutes;
                                }
                                else
                                {
                                    mWeekIdleTime[i] += 24 * 60;
                                }

                                break;
                            case "屏蔽上传做料状态":
                                if (i == 0)
                                {
                                    mWeekEngineTime[i] += (DateTime.Now - DateTime.Now.Date).TotalMinutes;
                                }
                                else
                                {
                                    mWeekEngineTime[i] += 24 * 60;
                                }




                                break;
                            case "计划停机状态":
                                if (i == 0)
                                {
                                    mWeekPlannedTime[i] += (DateTime.Now - DateTime.Now.Date).TotalMinutes;
                                }
                                else
                                {
                                    mWeekPlannedTime[i] += 24 * 60;
                                }



                                break;
                            case "宕机状态":
                                if (i == 0)
                                {
                                    mWeekDownTime[i] += (DateTime.Now - DateTime.Now.Date).TotalMinutes;
                                }
                                else
                                {
                                    mWeekDownTime[i] += 24 * 60;
                                }

                                break;
                        }
                    }
                }
                else
                {
                    string status;
                    for (int j = 0; j < dt.Rows.Count + 1; j++)
                    {
                        DateTime time1 = new DateTime();
                        DateTime time2 = new DateTime();
                        if (j == 0)
                        {
                            sqlitestr = $"select 切换时间,状态  from {mHiveStatusTable} where 时间戳< '{ChangeTimeSlice(time.Date)}'order by 时间戳 desc";
                            DataTable dt1 = new DataTable();
                            dt1 = HiveSqlite.GetDataSet(sqlitestr).Tables[0];
                            if (dt1.Rows.Count == 0)
                            {
                                status = _HiveMachineStaus.空闲状态.ToString();
                                time1 = time.Date;
                                time2 = Convert.ToDateTime(dt.Rows[j].ItemArray[0].ToString());
                            }
                            else
                            {
                                status = dt1.Rows[0].ItemArray[1].ToString();
                                time1 = time.Date;
                                time2 = Convert.ToDateTime(dt.Rows[j].ItemArray[0].ToString());
                            }


                        }
                        else if (j == dt.Rows.Count)
                        {
                            status = dt.Rows[j - 1].ItemArray[1].ToString();
                            time1 = Convert.ToDateTime(dt.Rows[j - 1].ItemArray[0].ToString());
                            if (i == 0)
                            {
                                time2 = DateTime.Now;
                            }
                            else
                            {
                                time2 = time.AddDays(1).Date;
                            }
                        }
                        else
                        {
                            time1 = Convert.ToDateTime(dt.Rows[j - 1].ItemArray[0].ToString());
                            time2 = Convert.ToDateTime(dt.Rows[j].ItemArray[0].ToString());
                            status = dt.Rows[j - 1].ItemArray[1].ToString();
                        }
                        switch (status)
                        {
                            case "正常做料状态":
                                mWeekRunTime[i] += (time2 - time1).TotalMinutes;
                                break;
                            case "空闲状态":
                                mWeekIdleTime[i] += (time2 - time1).TotalMinutes;
                                break;
                            case "屏蔽上传做料状态":
                                mWeekEngineTime[i] += (time2 - time1).TotalMinutes;
                                break;
                            case "计划停机状态":
                                mWeekPlannedTime[i] += (time2 - time1).TotalMinutes;
                                break;
                            case "宕机状态":
                                mWeekDownTime[i] += (time2 - time1).TotalMinutes;
                                break;

                        }


                    }
                }



            }
        }






        /// <summary>
        /// 获取一天Hive状态所占比例
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public double[] GetDayStatuseTime(int day)
        {
            double[] time = new double[5];
            try
            {
                day = 7 - day;
                double total = mWeekDownTime[day] + mWeekPlannedTime[day] + mWeekEngineTime[day] + mWeekIdleTime[day] + mWeekRunTime[day];
                time[0] = Math.Round(mWeekRunTime[day] / total, 4);
                time[1] = Math.Round(mWeekIdleTime[day] / total, 4);
                time[2] = Math.Round(mWeekEngineTime[day] / total, 4);
                time[3] = Math.Round(mWeekPlannedTime[day] / total, 4);
                time[4] = Math.Round(mWeekDownTime[day] / total, 4);
            }
            catch (Exception ex)
            {

            }
            return time;
        }

        /// <summary>
        /// 从数据库中读取Hive报警信息
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="listname"></param>
        /// <param name="listcount"></param>
        private void GetHiveErrorStatics(DateTime start, DateTime end, out List<string> listname, out List<int> listcount)
        {

            double s1 = ChangeTimeSlice(start);
            double s2 = ChangeTimeSlice(end);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            listname = new List<string>();
            listcount = new List<int>();
            string sqlstr = "";
            foreach (var item in mErrorCateDic)
            {
                sqlstr = $"select count (*) from {mHiveErrorMsgTable } where  报警类别='{item.Value }' and  时间戳 between '{s1  }' and '{s2 }'";
                DataTable dt = new DataTable();
                dt = HiveSqlite.GetDataSet(sqlstr).Tables[0];
                dict.Add(item.Value, Convert.ToInt32(dt.Rows[0].ItemArray[0]));
            }
            var sortedDict = from pair in dict orderby pair.Value descending select pair; //以字典Value值顺序排序[升序]

            int count = 0;
            foreach (var item in sortedDict)
            {
                if (count == 8)
                {
                    break;
                }
                else
                {
                    listname.Add(item.Key);
                    listcount.Add(item.Value);
                }
                count++;
            }


        }

        private string[] keyDescribe = new string[] { "上传类型", "url", "屏蔽上传" };


        /// <summary>
        /// 更新Hive配置参数到DataGridView控件中
        /// </summary>
        /// <param name="view"></param>
        public void UpdateParameterToGrid(DataGridView view)
        {
            try
            {
                view.Rows.Clear();
                view.Columns.Clear();
                /***设置DataGridView控件的样式***/
                view.Columns.Add("Column1", keyDescribe[0]);
                view.Columns.Add("Column2", keyDescribe[1]);
                view.Columns.Add("Column3", keyDescribe[2]);
                int width = view.Width;
                view.Columns[0].Width = (int)(width * 0.3);
                view.Columns[1].Width = (int)(width * 0.5);
                view.Columns[2].Width = (int)(width * 0.2);
                for (int i = 0; i < view.Columns.Count; i++)
                {
                    view.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    view.Columns[i].Resizable = DataGridViewTriState.NotSet;
                    //view.Columns[i].ReadOnly = false;
                }
                view.EnableHeadersVisualStyles = false;//缺少该行代码，标题的样式无法改变
                view.RowHeadersVisible = false;//影藏行的标题头
                view.AllowUserToResizeRows = false;//行不可调整
                view.AllowUserToResizeColumns = false;//列不可调整
                view.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                view.BorderStyle = System.Windows.Forms.BorderStyle.None;
                view.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                view.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                view.ColumnHeadersHeight = 30;
                view.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", (float)10, FontStyle.Bold);
                view.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                view.GridColor = Color.FromArgb(149, 148, 142);
                view.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                view.ColumnHeadersDefaultCellStyle.BackColor = Color.LightYellow;
                /***将字典里信息显示到DataGridView中***/
                int index = 0;
                foreach (var key in mHiveConfigInfo)
                {
                    view.Rows.Add();
                    view.Rows[index].DefaultCellStyle.Font = new Font("微软雅黑", (float)9.5, FontStyle.Bold);
                    view[0, index].Value = key.Value.type;
                    view[1, index].Value = key.Value.url;
                    view[2, index].Value = Convert.ToInt32(key.Value.Shiled);
                    index++;
                }
            }
            catch (Exception ex)
            {

            }
        }




        /// <summary>
        /// DataGridView控件全部系统参数更新到文件
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public bool UpdateGridToFile(DataGridView view)
        {
            try
            {

                mHiveConfigInfo.Clear();
                for (int i = 0; i < view.RowCount; i++)
                {
                    HiveConfigInfo info = new HiveConfigInfo();
                    info.type = view[0, i].Value.ToString();
                    info.url = view[1, i].Value.ToString();
                    info.Shiled = Convert.ToBoolean(Convert.ToInt32(view[2, i].Value.ToString()));
                    mHiveConfigInfo.Add(info.type, info);
                }

                XmlDocument doc = new XmlDocument();
                string filepath = Application.StartupPath + @"\ExeFile\Hive\HiveCfg.xml";
                doc.Load(filepath);
                XmlNode node = doc.SelectSingleNode("/SystemHardWareCfg/Hive");
                XmlNodeList nodeList = node.ChildNodes;
                foreach (XmlNode nd in nodeList)
                {
                    XmlElement element = nd as XmlElement;
                    string keyval = element.GetAttribute(keyDescribe[0]);//获取键值
                    element.SetAttribute(keyDescribe[1], mHiveConfigInfo[keyval].url);
                    element.SetAttribute(keyDescribe[2], Convert.ToInt32(mHiveConfigInfo[keyval].Shiled).ToString());
                }
                doc.Save(filepath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        /// <summary>
        /// 更新关键参数到DataGridview中
        /// </summary>
        /// <param name="dataGridView"></param>
        public void UpdataDashboard(DataGridView dataGridView)
        {
            try
            {


                dataGridView.Columns.Clear();
                dataGridView.Rows.Clear();
                //表头居中
                DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
                headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                dataGridView.ColumnHeadersDefaultCellStyle = headerStyle;
                //根据Header和所有单元格的内容自动调整行的高度
                dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                //设置内容对齐方式和字体 
                dataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView.Font = new Font("宋体", 10);

                //设置所有单元格都不可编辑
                dataGridView.ReadOnly = true;

                //设置标题头列宽
                dataGridView.RowHeadersWidth = 15;

                //不可以增加空行
                dataGridView.AllowUserToAddRows = false;

                //添加表头
                for (int i = 0; i < 4; i++)
                {
                    dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
                }
                //表格背景颜色
                dataGridView.DefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
                //选中区域字体颜色
                dataGridView.BackgroundColor = System.Drawing.Color.Gainsboro;
                dataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;
                //设置表格边框颜色
                dataGridView.GridColor = System.Drawing.Color.Gainsboro;
                //选中区域颜色
                dataGridView.DefaultCellStyle.SelectionBackColor = Color.Red;
                int width = dataGridView.Width;
                //指定标题列宽
                dataGridView.Columns[0].Width = (int)(width * 0.35);
                dataGridView.Columns[1].Width = (int)(width * 0.2);
                dataGridView.Columns[2].Width = (int)(width * 0.2);
                dataGridView.Columns[3].Width = (int)(width * 0.2);

                dataGridView.ColumnHeadersHeight = 30;
                dataGridView.RowHeadersWidth = 120;

                //添加标题字符
                dataGridView.Columns[0].HeaderText = "Key Name";
                dataGridView.Columns[1].HeaderText = "Value";
                dataGridView.Columns[2].HeaderText = "LSL";
                dataGridView.Columns[3].HeaderText = "USL";



                dataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;

                dataGridView.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                int count = 0;
                foreach (var va in mDashboardDic)
                {
                    dataGridView.Rows.Add();
                    dataGridView[0, count].Value = va.Value.KeyName;
                    dataGridView[1, count].Value = va.Value.Value;
                    dataGridView[2, count].Value = va.Value.LSL;
                    dataGridView[3, count].Value = va.Value.USL;
                    count++;
                }
            }
            catch (Exception ex)
            {

            }
        }



        /// <summary>
        /// 更新报警数量柱状图
        /// </summary>
        /// <param name="count">更新的天数</param>
        /// <param name="chart2">控件</param>
        public void UpdateErrorChart(DateTime start, DateTime end, Chart chart2)
        {
            try
            {
                List<string> listname = new List<string>();
                List<int> listcount = new List<int>();
                //string startstr;
                //string endstr;
                //if (start .Hour <10)
                //{
                //    startstr = $"{start.Year }-{start.Month }-{start.Day } 0{start.Hour }:{start.Minute }:{start.Second}.{start .Millisecond }";
                //}
                //else
                //{
                //    startstr = start.ToString("yyyy-MM-dd HH:mm:ss.fff");
                //}
                //if (end .Hour < 10)
                //{
                //    endstr = $"{end.Year }-{end.Month }-{end.Day } 0{end.Hour }:{end.Minute }:{end.Second}.{end .Millisecond }";
                //}
                //else
                //{
                //    endstr = end.ToString("yyyy-MM-dd HH:mm:ss.fff");
                //}
                GetHiveErrorStatics(start, end, out listname, out listcount);
                chart2.Series.Clear();
                chart2.Legends.Clear();
                chart2.ChartAreas.Clear();
                chart2.Titles.Clear();
                chart2.Titles.Add("Error Statistics");
                chart2.Titles[0].ForeColor = Color.Black;
                chart2.Titles[0].Font = new Font("微软雅黑", 12f, FontStyle.Regular);
                chart2.Titles[0].Alignment = ContentAlignment.TopLeft;
                chart2.BackColor = Color.Transparent;
                chart2.ChartAreas.Add("ChartAreas");
                chart2.Series.Add("Error");
                chart2.ChartAreas[0].BackColor = Color.Transparent;
                chart2.ChartAreas[0].BorderColor = Color.Transparent;
                chart2.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                chart2.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                chart2.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                chart2.ChartAreas[0].AxisX.TitleForeColor = Color.Blue;
                chart2.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#868686");
                chart2.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
                chart2.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);
                chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Transparent;
                chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent;
                chart2.ChartAreas[0].AxisX.IsMarginVisible = false;
                chart2.ChartAreas[0].Area3DStyle.Enable3D = false;
                chart2.ChartAreas[0].BackGradientStyle = GradientStyle.None;
                Legend legend3 = new Legend("#VALX");
                legend3.Title = "图例";
                legend3.TitleBackColor = Color.Transparent;
                legend3.BackColor = Color.Transparent;
                legend3.TitleForeColor = Color.Blue;
                legend3.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                legend3.Font = new Font("微软雅黑", 8f, FontStyle.Regular);
                legend3.ForeColor = Color.Blue;
                chart2.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
                chart2.Series[0].Label = "#VAL";                //设置显示X Y的值    
                chart2.Series[0].LabelForeColor = Color.Blue;
                chart2.Series[0].ChartType = SeriesChartType.Bar;
                chart2.Series[0].Color = Color.Red;
                chart2.Series[0].IsValueShownAsLabel = false;
                listname.Reverse();
                listcount.Reverse();
                chart2.Series[0].Points.DataBindXY(listname, listcount);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 更新Hive 时间统计
        /// </summary>
        /// <param name="chart1"></param>
        public void UpdateHiveTimeChart(Chart chart1)
        {
            try
            {

                string status = "";
                GetHiveStatusTime();
                List<double[]> m_list = new List<double[]>();
                m_list.Add(mWeekRunTime);
                m_list.Add(mWeekIdleTime);
                m_list.Add(mWeekEngineTime);
                m_list.Add(mWeekPlannedTime);
                m_list.Add(mWeekDownTime);
                //m_list.Reverse();
                chart1.Series.Clear();
                chart1.Legends.Clear();
                chart1.ChartAreas.Clear();
                chart1.Titles.Clear();
                ChartArea chartAreas = chart1.ChartAreas.Add("ChartAreas");
                chartAreas.AxisX.MajorGrid.Enabled = false;             // 坐标轴
                chartAreas.AxisY.MajorGrid.Enabled = false;             // Y轴主轴
                chartAreas.AxisY.Enabled = AxisEnabled.True;
                chartAreas.AxisY.LabelStyle.Format = "0%";
                chart1.Titles.Add("SState Change");
                chart1.Titles[0].Alignment = ContentAlignment.TopLeft;
                chartAreas.AxisY.Maximum = 1;


                chart1.Legends.Add("1");
                chart1.Legends[0].Docking = Docking.Top;


                for (int i = 0; i < 5; i++)
                {
                    double[] ss = new double[7];

                    for (int j = 0; j < 7; j++)
                    {
                        double tt = m_list[0][j] + m_list[1][j] + m_list[2][j] + m_list[3][j] + m_list[4][j];
                        ss[j] = Math.Round((m_list[i][j] / tt), 3);
                    }
                    string seriesName = i.ToString();
                    SeriesChartType chartType = SeriesChartType.StackedColumn;
                    bool isPrimary = true;
                    chart1.Series.Add(seriesName);

                    switch (i)
                    {
                        case 0:
                            chart1.Series[i.ToString()].Color = Color.FromArgb(0, 235, 0);
                            status = "Running";
                            chart1.Series[i.ToString()].LegendText = status;
                            break;
                        case 1:
                            chart1.Series[i.ToString()].Color = Color.FromArgb(0, 255, 254);
                            status = "Idle";
                            chart1.Series[i.ToString()].LegendText = status;
                            break;
                        case 2:
                            chart1.Series[i.ToString()].Color = Color.FromArgb(204, 171, 216);
                            status = "Engineering";
                            chart1.Series[i.ToString()].LegendText = status;
                            break;
                        case 3:
                            chart1.Series[i.ToString()].Color = Color.FromArgb(255, 215, 212);
                            status = "Planned Downtime";
                            chart1.Series[i.ToString()].LegendText = status;
                            break;
                        case 4:
                            chart1.Series[i.ToString()].Color = Color.FromArgb(235, 115, 115);
                            status = "DownTime";
                            chart1.Series[i.ToString()].LegendText = status;
                            break;
                        default:
                            chart1.Series[i.ToString()].Color = Color.FromArgb(211, 235, 115);
                            break;
                    }
                    chart1.Series[i.ToString()].ChartType = chartType;       // 图表类型
                    chart1.Series[i.ToString()].YAxisType = isPrimary ? AxisType.Primary : AxisType.Secondary;
                    chart1.Series[i.ToString()].BorderWidth = 2;
                    chart1.Series[i.ToString()].IsValueShownAsLabel = true;
                    chart1.Series[i.ToString()].LabelFormat = "P";

                    for (int k = ss.Length - 1; k >= 0; k--)
                    {
                        chart1.Series[i.ToString()].Points.AddXY(DateTime.Now.AddDays(0 - k).Date.ToString().Split(' ')[0], ss[k]);
                        chart1.Series[i.ToString()].ToolTip = "#VAL{P}"/*string.Format("{0}:#VAL{P}",status)*/;
                        string str = chart1.Series[i.ToString()].ToolTip;
                        chart1.Series[i.ToString()].ToolTip = $"{status } : {str }";
                    }

                }





            }
            catch (Exception ex)
            {

            }

        }



        #endregion





        #region Hive全局不需要外部修改参数




        /// <summary>
        /// 加载Hive参数
        /// </summary>
        /// <returns></returns>
        public bool InitialHiveParame()
        {
            try
            {
                if (ReadHiveConfigInfo())
                {
                    if (ReadHiveErrorConfigInfoXLS())
                    {
                        if (ReadMissErrorMsg())
                        {
                            CheckVersion();
                            Thread th = new Thread(RefeshIdleAndRun);
                            th.IsBackground = true;
                            th.Start();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }




        private _HiveMachineStaus mHiveStatus;
        /// <summary>
        /// 当前机台的Hive状态
        /// </summary>
        public _HiveMachineStaus HiveStatus
        {
            get { return mHiveStatus; }
            set
            {
                if (mHiveStatus != value)
                {
                    Stopwatch ST = new Stopwatch();
                    if (!mHiveConfigInfo[_HiveUploadType.MachineState.ToString()].Shiled)
                    {
                        ST.Restart();
                        UpDataStatus(value);
                        ST.Stop();
                        Double SS = ST.ElapsedMilliseconds;
                    }
                    ST.Restart();
                    ST.Stop();
                    Double CC = ST.ElapsedMilliseconds;
                    mPreHiveStatus = mHiveStatus;
                    mHiveStatus = value;
                    AppCfg.AppSettings.Settings["HiveStatus"].Value = value.ToString();
                    AppCfg.AppSettings.Settings["PreHiveStatus"].Value = mPreHiveStatus.ToString();
                    double ss = ChangeTimeSlice(DateTime.Now);
                    string sqlitestr = $"insert into {mHiveStatusTable } values ('{DateTime.Now.ToString()}',{ss},'{mHiveStatus.ToString()}')";
                    if (HiveSqlite.ExecSQLResult(sqlitestr) < 1)
                    {
                        Thread.Sleep(1000);
                        sqlitestr = $"insert into {mHiveStatusTable } values ('{DateTime.Now.ToString()}',{ss},'{mHiveStatus.ToString()}')";
                        HiveSqlite.ExecSQLResult(sqlitestr);
                    }
                    AppCfg.Save();
                }
            }
        }



        private string mHivePlannedCode;
        /// <summary>
        /// 当前Hive计划停机信息
        /// </summary>
        public string HivePlannedCode
        {
            get { return mHivePlannedCode; }
            set { mHivePlannedCode = value; }
        }


        /// <summary>
        /// 更新单次结果的委托
        /// </summary>
        /// <param name="ret"></param>
        public delegate void mHiveHandle();

        /// <summary>
        /// 更新报警分类统计柱状图
        /// </summary>
        public event mHiveHandle UpdataErrorStatis;


        private string mSw_Version;
        /// <summary>
        /// 当前机台总版本号
        /// </summary>
        public string Sw_Version
        {
            get { return mSw_Version; }
            set { mSw_Version = value; }
        }

        private string mUUID;
        /// <summary>
        /// 当前版本软件唯一识别码
        /// </summary>
        public string UUID
        {
            get { return mUUID; }
            set { mUUID = value; }
        }

        private string mMS_SHA1;
        /// <summary>
        /// 当前软件版本号
        /// </summary>
        public string MS_SHA1
        {
            get { return mMS_SHA1; }
            set { mMS_SHA1 = value; }
        }



        private string mVS_SHA1;
        /// <summary>
        /// 当前视觉软件版本号
        /// </summary>
        private string VS_SHA1
        {
            get { return mVS_SHA1; }
            set { mVS_SHA1 = value; }
        }


        private string mUserID;
        /// <summary>
        /// 当前登录工号
        /// </summary>
        public string UserID
        {
            get { return mUserID; }
            set { mUserID = value; }
        }

        private string mOldsn;
        /// <summary>
        /// 原始SN
        /// </summary>
        public string Oldsn
        {
            get { return mOldsn; }
            set { mOldsn = value; }
        }

        public string mNewsn;
        /// <summary>
        /// 新的SN
        /// </summary>
        public string Newsn
        {
            get { return mNewsn; }
            set { mNewsn = value; }
        }













        private _HiveMachineStaus mPreHiveStatus;
        /// <summary>
        /// Hive上一次机台状态
        /// </summary>
        public _HiveMachineStaus PreHiveStatus
        {
            get { return mPreHiveStatus; }
        }











        private string mAdminPath;


        /// <summary>
        /// 权限文件人员路径
        /// </summary>
        public string AdminPath
        {
            get { return mAdminPath; }
        }



        private string mHiveSite;

        /// <summary>
        /// Hive站别
        /// </summary>
        public string HiveSite
        {
            get { return mHiveSite; }
        }


        #endregion








        #region 人员全局需要外部修改参数


        private DateTime mInputTime = DateTime.Now.AddDays(-2);


        /// <summary>
        /// 进料时间
        /// </summary>
        public DateTime InputTime
        {
            get { return mInputTime; }
            set { mInputTime = value; }
        }



        private bool m_ShiledUpload;

        /// <summary>
        /// 是否屏蔽上传
        /// </summary>
        public bool ShiledUpload
        {
            get { return m_ShiledUpload; }
            set { m_ShiledUpload = value; }
        }


        private string mDischargSN = "null";


        /// <summary>
        /// 传输Hive出料产品SN
        /// </summary>
        public string DischargSN
        {
            get { return mDischargSN; }
            set { mDischargSN = value; }
        }



        private bool mDischargResult = true;
        /// <summary>
        /// 传输Hive出料产品结果
        /// </summary>
        public bool DischargResult
        {
            get { return mDischargResult; }
            set { mDischargResult = value; }
        }



        private int mInput = 0;
        /// <summary>
        /// 传输Hive进料数量
        /// </summary>
        public int Input
        {
            get { return mInput; }
            set { mInput = value; }
        }



        private int mOutput = 0;


        /// <summary>
        /// 传输Hive出料数量
        /// </summary>
        public int Output
        {
            get { return mOutput; }
            set { mOutput = value; }
        }



        private int mYield = 0;
        /// <summary>
        /// 传输Hive产量
        /// </summary>
        public int Yield
        {
            get { return mYield; }
            set { mYield = value; }
        }



        private int mUPH = 0;

        /// <summary>
        /// 传输Hive UPH
        /// </summary>
        public int UPH
        {
            get { return mUPH; }
            set { mUPH = value; }
        }

        private double mCT = 0.0;
        /// <summary>
        /// 传输Hive CT
        /// </summary>
        public double CT
        {
            get { return mCT; }
            set { mCT = value; }
        }


        /// <summary>
        /// 向Hive添加报警信息
        /// </summary>
        /// <param name="ErrorMsg"></param>
        public void HiveErrorMsg(string ErrorMsg)
        {
            bool flag = false;
            if (mHiveLanuage == "CH")
            {
                if (mHiveErrorDic_CH.ContainsKey(ErrorMsg))
                {
                    AppCfg.AppSettings.Settings["ErrorMessage"].Value = mHiveErrorDic_CH[ErrorMsg].ErrorMsg;
                    AppCfg.AppSettings.Settings["ErrorCode"].Value = mHiveErrorDic_CH[ErrorMsg].AlarmCode;
                    AppCfg.AppSettings.Settings["ErrorSeverity"].Value = mHiveErrorDic_CH[ErrorMsg].Severity;
                    AppCfg.AppSettings.Settings["ErrorTime"].Value = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else if (mHiveLanuage == "EN")
            {
                if (mHiveErrorDic_CH.ContainsKey(ErrorMsg))
                {
                    AppCfg.AppSettings.Settings["ErrorMessage"].Value = mHiveErrorDic_EN[ErrorMsg].ErrorMsg;
                    AppCfg.AppSettings.Settings["ErrorCode"].Value = mHiveErrorDic_EN[ErrorMsg].AlarmCode;
                    AppCfg.AppSettings.Settings["ErrorSeverity"].Value = mHiveErrorDic_EN[ErrorMsg].Severity;
                    AppCfg.AppSettings.Settings["ErrorTime"].Value = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {

                if (mHiveErrorDic_VN.ContainsKey(ErrorMsg))
                {
                    AppCfg.AppSettings.Settings["ErrorMessage"].Value = mHiveErrorDic_VN[ErrorMsg].ErrorMsg;
                    AppCfg.AppSettings.Settings["ErrorCode"].Value = mHiveErrorDic_VN[ErrorMsg].AlarmCode;
                    AppCfg.AppSettings.Settings["ErrorSeverity"].Value = mHiveErrorDic_VN[ErrorMsg].Severity;
                    AppCfg.AppSettings.Settings["ErrorTime"].Value = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    flag = true;
                }
                else
                {
                    flag = false;
                }

            }
            if (!flag)
            {
                AppCfg.AppSettings.Settings["ErrorMessage"].Value = "Other Alarms";
                AppCfg.AppSettings.Settings["ErrorCode"].Value = "O9OO00-01-03";
                AppCfg.AppSettings.Settings["ErrorSeverity"].Value = "warning";
                AppCfg.AppSettings.Settings["ErrorTime"].Value = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                if (MissErrMsg.IndexOf(ErrorMsg) == -1)
                {
                    MissErrMsg.Add(ErrorMsg);
                    WriteMissErr();
                }
            }

            AppCfg.AppSettings.Settings["ErrorStatus"].Value = Convert.ToInt16(mHiveStatus + 1).ToString();
            AppCfg.Save();





        }



        private string mHiveLanuage;

        /// <summary>
        /// 当前机台语言
        /// </summary>
        public string HiveLanuage
        {
            get { return mHiveLanuage; }
            set { mHiveLanuage = value; }
        }

        #endregion 




    }

}
