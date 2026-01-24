using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CYAutoFramework;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using cySplash;
using ModbusLib;
using System.IO;
using System.Text;
using CYStandardProcedure.Station;

namespace CYStandardProcedure
{

    /// <summary>
    /// 设备输入信号
    /// </summary>
    public enum _InputCollect
    {
        启动按钮 = 0,
        复位按钮,
        暂停按钮,
        急停按钮,
        安全门1,
        安全门2,
        Aubo机器人系统错误,
        Aubo机器人运动,
        Aubo机器人未停止,
        Aubo机器人程序运行,
        Aubo机器人程序停止,
        Aubo机器人程序暂停,
        Aubo机器人紧急停止,
        备用5,
        备用6,
        备用7,
        枪头进料区光电1,
        枪头进料区光电2,
        枪头出料区光电1,
        枪头出料区光电2,
        进料区光电1,
        进料区光电2,
        出料区光电1,
        出料区光电2,
        枪头区1光电1,
        枪头区1光电2,
        枪头区2光电1,
        枪头区2光电2,
        枪头区3光电1,
        枪头区3光电2,
        枪头区4光电1,
        枪头区4光电2,
        光电8联排试管区1,
        光电8联排试管区2,
        离心管试管区光电1,
        离心管试管区光电2,
        低温区光电,
        常温试剂区光电1,
        常温试剂区光电2,
        废料区1光电,
        废料区2光电,
        搬运夹爪检测光电,
    }

    /// <summary>
    /// 设备输出信号
    /// </summary>
    public enum _OutputCollect
    {
        三色灯红,
        三色灯黄,
        三色灯绿,
        照明,
        停止按钮灯,
        启动按钮灯,
        复位按钮灯,
        配电盘照明,
        Aubo机器人远程开机,
        Aubo机器人远程关机,
        Aubo机器人程序启动,
        Aubo机器人程序停止,
        Aubo机器人程序暂停,
        Aubo防护停止启动,
        Aubo机器人清除报警,
        Aubo机器人防护停止,
        备用5,
        备用6,
        备用7,
        备用8,
        备用9,
        备用10,
        备用11,
        备用12,
        备用13,
        备用14,
        备用15,
        备用16,
        备用17,
        备用18,
        备用19,
        备用20
    }
    
    /// <summary>
    /// 设备参数
    /// </summary>
    public enum _ParamName
    {
        ShiledSafetyDoor1,
        ShiledSafetyDoor2,

        ShiledSequence,
        ShiledPLC,
        ShiledMainControl,
        ShiledBarcode,
        ShiledCCD,
        ShiledPipette,
        ShiledTemperature,
        ShiledAnimation,
        ShiledAutoOpen,
        ShiledLight,

        MotorRunTimeOut,
        MotorHomeTimeOut,
        RobotTimeOut,
        CCDTimeOut,
        PipetteTimeOut,

        AnimationTime,
        PingHengTime,
        FuYuTime,
        CheckXinPianTime,
        GeneralShareIP,
        SequenceFilecoef,
        SequenceHandle,

        Gun_Initial_Speed,
        Z_Initial_Speed,
        Z_Movepos_Down,
        Z_Movepos_Up,
        Z_Movepos_Speed,
        Z_Check_Pos,
        Z_Check_Speed,
        Z_LiXinGuan1000_Pos,
        Z_LiXinGuan200_Pos,
        Z_DiWenFCF_Pos,
        Z_DiWenFCT_Pos,
        Z_DiWenSB_Pos,
        Z_DiWenLIB_Pos,
        Z_DiWenWMX_Pos,
        Z_DiWenDIL_Pos,
        Z_DiWenS_Pos,
        Z_BaLianPai50_Pos,
        Z_ShangYangKong_Pos,
        Z_YuChuLiKong_Pos,
        Z_FeiYeKong_Pos,
        Z_PickTip_Speed,
        Gun_Inliquid_Speed,
        Gun_Outliquid_Speed,
        Gun_Outliquid_Fastspeed,
        Gun_Outliquid_Slowspeed,
        Gun_Outliquid_XinPian,
        Surface_LiXinGuan,
        Surface_DiWen,

        FCF_Volume,
        FCT_Volume,
        FCFmix_Volume1,
        FCFmix_Volume2,
        LIB_Volume,
        SB_Volume,
        DNA_Volume,
        DIL_Volume,
        WMX_Volume,
        S_Volume,
        FCFmix_VolumeOut1,
        FCFmix_VolumeOut2,
        DILmix_VolumeOut,
        S_VolumeOut,
        Bubble_Out,
        Waste_Experiment1,
        Waste_Experiment2,
        Waste_Clean,
        Waste_Save,
    }


    /// <summary>
    /// 设备轴名称
    /// </summary>
    public enum _Axis
    {
        测序仪XAxis,
        搬运ZAxis,
        搬运XAxis ,
        搬运YAxis,
    }
    public enum _CarryStation1Axis
    {
        搬运XAxis = 0,
        搬运YAxis,
    }
    public enum _CarryStation2Axis
    {
        搬运ZAxis = 0,
    }
    public enum _SequencingStationAxis
    {
        测序仪XAxis
    }

    /// <summary>
    /// 设备运动点位名称
    /// </summary>
    public enum _PointArray
    {
        //搬运模组XY点位
        待机位置,
        地轨避让位置,
        枪头进料扫码位置,
        进料扫码位置,
        枪头区1搬运位置,
        枪头区2搬运位置,
        枪头区3搬运位置,
        枪头区4搬运位置,
        低温区搬运位置,
        常温试剂区搬运位置,
        离心管试管区搬运位置,
        八联排试管区搬运位置,
        枪头进料区搬运位置,
        枪头出料区搬运位置,
        进料区搬运位置,
        出料区搬运位置,

        预处理孔位置,
        上样孔位置,
        废液孔位置,

        枪头区1取料位置,
        枪头区2取料位置,
        枪头区3取料位置,
        枪头区4取料位置,
        低温区FCF取料位置,
        低温区FCT取料位置,
        低温区SB取料位置,
        低温区LIB取料位置,
        低温区DIL取料位置,
        低温区WMX取料位置,
        低温区S取料位置,
        八联排DNA样本取料位置,
        离心管试管区取料位置,
        废料区1下料位置,
        废料区2下料位置,

        //搬运模组Z点位
        试管搬运上升位置,
        枪头搬运上升位置,
        枪头200扫码下降位置,
        枪头1000扫码下降位置,
        进料扫码下降位置,
        枪头区抓取位置,
        低温区抓取位置,
        常温试剂区抓取位置,
        离心管试管区抓取位置,
        八联排试管区抓取位置,
        枪头1000进出料区抓取位置,
        枪头200进出料区抓取位置,
        进出料区抓取位置,

        //测序仪模组点位
        滴试剂位置,
        开关盖位置,
        上3D线扫位置,
    }

    public enum _TcpClientModule
    {
        GeneralControl,
        PLC,
        Scan,
        AuboRobot,
        AuboRobotSDK,
        RobotProject,
        CCD,
        Animation
    }

    public enum _ThreadModule
    {
        总线程 = 0,
        供料线程,
        搬运工位,
        测序仪工位,
        机器人工位,
        状态监控线程,
        数据处理线程,
        数字孪生线程,
    }
    public enum _SerialModule
    {
        TemperatureControl
    }






    static class Program
    {

        /// <summary>
        /// 保存搬运电动夹爪运动参数
        /// </summary>
        public static List<GripPawlConfig> carryClawConfigList = new List<GripPawlConfig>();
        /// <summary>
        /// 保存搬运电动夹爪通讯参数
        /// </summary>
        public static ModbusRtuConfig carryClawConfig = new ModbusRtuConfig();

        /// <summary>
        /// 保存机器人电动夹爪运动参数
        /// </summary>
        public static List<GripPawlConfig> robotClawConfigList = new List<GripPawlConfig>();

        /// <summary>
        /// 保存机器人电动夹爪通讯参数
        /// </summary>
        public static ModbusRtuConfig robotClawConfig = new ModbusRtuConfig();

        /// <summary>
        /// 搬运夹爪Form实例
        /// </summary>
        public static CarryClawForm carryClawForm => SoftWareForm.carryclaw_initialize;

        /// <summary>
        /// 机器人精密夹爪Form实例
        /// </summary>
        public static RobotNewClawForm robotNewClawForm => SoftWareForm.m_RobotNewClaw;

        /// <summary>
        /// Hive操作类对象
        /// </summary>
        public static Hive m_Hive;

        /// <summary>
        /// 上相机排气泡指令（反馈 气泡小:true;气泡大:false）
        /// </summary>
        public static string CCDQiPaoCmd = "CY_TCam_01_01_99_WZ";
        /// <summary>
        /// 上相机拍上样孔指令（反馈 偏移坐标X,Y）
        /// </summary>
        public static string CCDCmd_ShangYangKong = "CY_TCam_01_02_99_WZ";
        /// <summary>
        /// 上相机拍芯片上样孔盖指令（反馈 无盖子:null;自制盖:pass;原装盖:fail）
        /// </summary>
        public static string CCDCmd_XinPianGai = "CY_TCam_01_03_99_WZ";
        /// <summary>
        /// 上相机拍耗材区是否有盖子指令(反馈 true，false)
        /// </summary>
        public static string CCDCmd_IsHaveCover = "CY_TCam_01_04_99_WZ";
        /// <summary>
        /// 下相机拍上样孔盖指令
        /// </summary>
        public static string CCDCmd_KongGai = "CY_TCam_02_01_99_WZ";
        /// <summary>
        /// 上3D相机拍上样孔盖指令
        /// </summary>
        public static string CCDCmd_Up3D = "CY_TCam_03_01_99_WZ";
        /// <summary>
        /// 下3D相机拍pin针指令
        /// </summary>
        public static string CCDCmd_Down3D = "CY_TCam_04_01_99_WZ";
        /// <summary>
        /// 相机反馈
        /// </summary>
        public static string CCDReceived = string.Empty;
        /// <summary>
        /// 总控反馈
        /// </summary>
        public static string ControlReceived = string.Empty;
        /// <summary>
        /// 扫码指令
        /// </summary>
        public static string ScanCmd = "+";
        /// <summary>
        /// 扫码反馈
        /// </summary>
        public static string ScanReceived = string.Empty;

        /// <summary>
        /// MES参数信息字典属性
        /// key:上传方式 value:MES参数信息
        /// </summary>
        public static Dictionary<string, List<string>> MesInfoDic;

        public static ModbusTcp modbusTcp_PLC;  

        /// <summary>
        /// Web API服务站点
        /// </summary>
        public static WebApiStation webApiStation;

        #region 程序自检
        public static Process RunningInstance()
        {
            Process current = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in processes) //查找相同名称的进程
            {
                if (process.Id != current.Id) //忽略当前进程
                {
                    if (process.ProcessName == current.ProcessName)//判断进程名称是否和当前运行进程名称一样
                    {
                        if (Assembly.GetExecutingAssembly().Location.Replace(@"/", @"\") == current.MainModule.FileName)
                        {
                            return process;
                        }
                    }
                }
            }
            return null;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str;
            var error = e.ExceptionObject as Exception;
            if (error != null)
            {
                str = string.Format("出现应用程序未处理的异常----->" + "Application UnhandledException:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }
            //SystemError.WriteAlarmLog(str);
            /***停止Web API服务***/
            try
            {
                webApiStation?.Stop();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"停止Web API服务时发生错误: {ex.Message}");
            }
            
            /***注销IO卡***/
            IOConfig.Instance.ReleaseIOCard();
            /***关闭运动控制卡***/
            MotionConfig.Instance.CloseCard();
            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 1);
            Thread.Sleep(200);
            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 0);
            SerializeClass.WriteMemoryParame();
            SerializeClass.WriteAnimationParame();
            SerializeClass.WriteSequenceParame();
            SerializeClass.WriteIDNAParame();
            SerializeClass.WriteCeXuDic();
            FileSave.WriteAreaMsg();
            FileSave.WriteBoolMsg();
            FileSave.WriteVolumeMax();
            WritemApplicationWrongLog(str);
            MessageBox.Show("发生错误，请查看程序日志！" + Environment.NewLine + str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str;
            var error = e.Exception;
            if (error != null)
            {
                str = string.Format("出现应用程序未处理的异常----->" + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                     error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }
            /***停止Web API服务***/
            try
            {
                webApiStation?.Stop();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"停止Web API服务时发生错误: {ex.Message}");
            }
            
            /***注销IO卡***/
            IOConfig.Instance.ReleaseIOCard();
            /***关闭运动控制卡***/
            MotionConfig.Instance.CloseCard();
            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 1);
            Thread.Sleep(200);
            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 0);
            SerializeClass.WriteMemoryParame();
            SerializeClass.WriteAnimationParame();
            SerializeClass.WriteSequenceParame();
            SerializeClass.WriteIDNAParame();
            SerializeClass.WriteCeXuDic();
            FileSave.WriteAreaMsg();
            FileSave.WriteBoolMsg();
            FileSave.WriteVolumeMax();
            WritemApplicationWrongLog(str);
            MessageBox.Show("发生错误，请查看程序日志！" + Environment.NewLine + str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
        #endregion


        private static void WritemApplicationWrongLog(string sendmsg)
        {
            try
            {
                string NowDate = string.Format("{0:yyyyMMdd}", DateTime.Now);//获取当前日期
                if (!Directory.Exists(@"E:\SWLog\ApplicationWrong\"))
                {
                    Directory.CreateDirectory(@"E:\SWLog\ApplicationWrong\");
                }
                if (!File.Exists(@"E:\SWLog\ApplicationWrong\" + NowDate + ".txt"))
                {
                    File.Create(@"E:\SWLog\ApplicationWrong\" + NowDate + ".txt").Close();
                }
                if (File.Exists(@"E:\SWLog\ApplicationWrong\" + NowDate + ".txt"))
                {
                    using (FileStream fsWrite = new FileStream(@"E:\SWLog\ApplicationWrong\" + NowDate + ".txt", FileMode.Append))
                    {
                        using (StreamWriter sw = new StreamWriter(fsWrite, Encoding.Unicode))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "    " + sendmsg);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }



        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (RunningInstance() == null)
            {
                try
                {
                    //处理未捕获的异常
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    //处理UI线程异常
                    Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                    //处理非UI线程异常
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Thread t = new Thread(Splash.ShowSplash);
                    t.IsBackground = true;
                    t.Start(Application.StartupPath + @"\ExeFile\CY.PNG");
                    
                    // 启动Web API服务
                    try
                    {
                        webApiStation = new WebApiStation();
                        webApiStation.Start();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"启动Web API服务时发生错误: {ex.Message}");
                    }
                    
                    Application.Run(new SoftWareForm());
                }
                catch (Exception ex)
                {
                    var strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now + "\r\n";
                    var str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                                               ex.GetType().Name, ex.Message, ex.StackTrace);
                    //SystemError.WriteAlarmLog(str);
                    MessageBox.Show("发生错误，请查看程序日志！" + Environment.NewLine + str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            else
            {
                MessageBox.Show("当前应用程序已经在运行！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
    }
}
