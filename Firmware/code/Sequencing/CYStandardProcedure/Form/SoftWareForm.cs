using CYAutoFramework;
using CYCustomControl;
using cySplash;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ModbusLib;
using System.Diagnostics;
using System.Xml.Linq;
using ktCnt;

namespace CYStandardProcedure
{
    public partial class SoftWareForm : Form
    {
        public static CarryClawForm carryclaw_initialize = new CarryClawForm();
        public static RobotClawForm robotclaw_initialize = new RobotClawForm();
        public static RobotNewClawForm m_RobotNewClaw = new RobotNewClawForm();

        /***按钮和窗体字典***/
        private Dictionary<RoundButton, Form> mFormDic = new Dictionary<RoundButton, Form>();
        /***当前窗体***/
        private Form mCurForm;
        /***当前按钮***/
        private RoundButton mRoundBtn;
        /***Ini文件操作类(交接班时间)***/
        private INIOperation mIni = new INIOperation(Application.StartupPath + @"\ExeFile\Machine.ini");
        INIFile pipetteini = new INIFile(Application.StartupPath + @"\ExeFile\" + @"\PipetteGunParam" + ".ini");
        public static SoftWareForm m_softwarmform;
        List<string> keyValues = new List<string>();
        string[] keys;
        string[] values;
        bool bt;

        string com;
        char comindex1;
        char comindex2;


        public SoftWareForm()
        {
            InitializeComponent();
            m_softwarmform = this;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = mIni.GetStringValue("软件版本", "SoftVersion", "CYMachine");
            rbt_Machine.Text = mIni.GetStringValue("设备编号", "Number", "Machine");
            langaugeList2.CmbSelectClick += new LangaugeControl.LangaugeList.CmbSelect(langaugeList2_CmbSelectClick);
        }

        #region//禁止窗体移出屏幕
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x46:
                    Rectangle rect = Screen.GetWorkingArea(this);
                    WINDOWPOS winPos = (WINDOWPOS)m.GetLParam(typeof(WINDOWPOS));
                    if (winPos.x + winPos.cx > rect.Right)
                    {
                        winPos.x = rect.Right - winPos.cx;
                    }

                    if (winPos.y + winPos.cy > rect.Bottom)
                    {
                        winPos.y = rect.Bottom - winPos.cy;
                    }

                    if (winPos.x < rect.Top)
                    {
                        winPos.x = rect.Top;
                    }

                    if (winPos.y < rect.Left)
                    {
                        winPos.y = rect.Left;
                    }

                    System.Runtime.InteropServices.Marshal.StructureToPtr(winPos, m.LParam, false);
                    base.WndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            internal IntPtr hWnd;
            internal IntPtr hWndInsertAfter;
            internal int x;
            internal int y;
            internal int cx;
            internal int cy;
            internal int flags;
        }
        #endregion

        /// <summary>
        /// 语言改变的订阅事件
        /// </summary>
        /// <param name="strLanguage"></param>
        private void SoftWareForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            if (Program.m_Hive != null)
            {
                Program.m_Hive.HiveLanuage = strLanguage;
            }
            /***改变状态栏文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, statusStrip1, ini);
            /***权限刷新***/
            if (AdminConfig.Instance.UserLevel == 0)
            {
                lab_Admin.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "OP", "操作员");
                lab_Admin.ForeColor = Color.Orange;
            }
            else if (AdminConfig.Instance.UserLevel == 1)
            {
                lab_Admin.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "EE", "工程师");
                lab_Admin.ForeColor = Color.Blue;
            }
            else
            {
                lab_Admin.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "AD", "管理员");
                lab_Admin.ForeColor = Color.LimeGreen;
            }
            /***模式刷新***/
            SoftWareForm_ModeChangeEvent(StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurMode);
            /***状态刷新***/
            SoftWareForm_StatusChangeEvent(StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus);
            /***语言显示***/
            lab_Langauge.Text = LanguageConfig.Instance.Language;
        }

        /// <summary>
        /// 连接电动夹爪
        /// </summary>
        /// <returns></returns>
        public bool ClawConnect()
        {
            try
            {
                if (SerializeClass.m_ModbusRtuRob.Connect()&&
                    carryclaw_initialize.Rtu_carryClaw.OpenMyCom(Program.carryClawConfig.iBaudRate, Program.carryClawConfig.iPortName, Program.carryClawConfig.iDataBits, Program.carryClawConfig.iParity, Program.carryClawConfig.iStopBits))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool pipetteConnect()
        {
            try
            {
                string backstate = "";
                com = pipetteini.Read<string>("1", "串口号");
                int i = com.Length;
                if (i == 5)
                {
                    comindex1 = com[3];
                    comindex2 = com[4];
                    MyVariable.PipetteGunConnect(2, comindex1, comindex2, out backstate);
                }
                else
                {
                    comindex1 = com[3];
                    comindex2 = '0';
                    MyVariable.PipetteGunConnect(1, comindex1, comindex2, out backstate);
                }
                if (backstate == "KPC_OK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void SoftWareForm_Load(object sender, EventArgs e)
        {

            bool loadErr = false;//加载出错标志
            string ip = string.Empty;
            int port = 0;
            MyVariable.ModbusTCPInstance();
            Splash.mEvent.WaitOne();
            /***判断语言***/
            if (LanguageConfig.Instance.Language == "CH")
            {
                Splash.ShowSplashTisp("程序加载中,请耐心等候......", Color.LightYellow);
            }
            else if (LanguageConfig.Instance.Language == "EN")
            {
                Splash.ShowSplashTisp("The program is loading, please wait patiently......", Color.LightYellow);
            }
            else
            {
                Splash.ShowSplashTisp("Chương trình đang tải, xin hãy kiên nhẫn chờ......", Color.LightYellow);
            }
            if (!Directory.Exists(@"E:\ShowImage"))
            {
                Directory.CreateDirectory(@"E:\ShowImage");
            }
            #region 读取记忆文件
            SerializeClass.ReadMemoryParame();
            SerializeClass.ReadRobClawParame();
            SerializeClass.ReadAnimationParame();
            SerializeClass.ReadSequenceParame();
            SerializeClass.ReadIDNAParame();
            SerializeClass.ReadCeXuDic();
            FileSave.ReadVolumeMax();
            FileSave.ReadAreaMsg();
            FileSave.ReadRobotClawMsg();
            FileSave.ReadBoolMsg();
            Splash.ShowSplashTisp("读取设备记忆文件！", Color.Lime);
            Thread.Sleep(500);
            #endregion

            #region 加载设备配置参数
            Splash.ShowSplashTisp("设备配置参数加载中...", Color.Lime);
            if (!ParameConfig.Instance.ReadParameConfigFromXml())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("设备配置参数加载失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Device configuration parameter loading failed！", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Lỗi nạp Tham số cấu hình thiết bị！", Color.Red);
                }
                loadErr = true;
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("设备配置参数加载成功！", Color.Lime);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Device configuration parameters loaded successfully！", Color.Lime);
                }
                else
                {
                    Splash.ShowSplashTisp("Thiết lập đã tải thành công！", Color.Lime);
                }
            }
            Thread.Sleep(500);
            #endregion

            #region 加载电动夹爪通信参数
            Program.carryClawConfig = Xml_SerializerHelper.XmlDeserialize<ModbusRtuConfig>(carryclaw_initialize.path_CarryClaw);
            SerializeClass.m_ModbusRtuRob = new ModbusRtu(SerializeClass.m_RobClawParam.robClaw_Com, SerializeClass.m_RobClawParam.robClaw_Baudrate,
    SerializeClass.m_RobClawParam.robClaw_Databits, SerializeClass.m_RobClawParam.robClaw_Parity, SerializeClass.m_RobClawParam.robClaw_Stopbits);

            //       Program.robotClawConfig = Xml_SerializerHelper.XmlDeserialize<ModbusRtuConfig>(robotclaw_initialize.path_RobotClaw);
            #endregion

            #region 加载电动夹爪运动参数
            Program.carryClawConfigList = Xml_SerializerHelper.XmlDeserialize<List<GripPawlConfig>>(carryclaw_initialize.path2_CarryClaw);
      //      Program.robotClawConfigList = Xml_SerializerHelper.XmlDeserialize<List<GripPawlConfig>>(robotclaw_initialize.path2_RobotClaw);

            #endregion

            #region 加载自定义Hive
            //Program.m_Hive = new Hive();
            //if (Program.m_Hive.InitialHiveParame())
            //{
            //    if (LanguageConfig.Instance.Language == "CH")
            //    {
            //        Splash.ShowSplashTisp("自定义Hive参数加载成功！", Color.Lime);
            //        Program.m_Hive.HiveLanuage = "CH";
            //    }
            //    else if (LanguageConfig.Instance.Language == "EN")
            //    {
            //        Splash.ShowSplashTisp("Successfully loaded custom Hive parameters!", Color.Lime);
            //        Program.m_Hive.HiveLanuage = "EN";
            //    }
            //    else
            //    {
            //        Splash.ShowSplashTisp("Tham số Hive tùy chỉnh tải thành công!", Color.Lime);
            //        Program.m_Hive.HiveLanuage = "VN";
            //    }
            //    Program.m_Hive.ShiledUpload = false;
            //}
            //else
            //{
            //    if (LanguageConfig.Instance.Language == "CH")
            //    {
            //        Splash.ShowSplashTisp("自定义Hive参数加载失败！", Color.Red);
            //    }
            //    else if (LanguageConfig.Instance.Language == "EN")
            //    {
            //        Splash.ShowSplashTisp("Failed to load custom Hive parameters!", Color.Red);
            //    }
            //    else
            //    {
            //        Splash.ShowSplashTisp("Tải tham số Hive tùy chỉnh không thành công!", Color.Red);
            //    }
            //    loadErr = true;
            //}
            //Thread.Sleep(500);
            #endregion

            #region 加载运动控制卡
            /***如果204C或8338卡加载MotionConfig1.xml，M60加载MotionConfig1.ini***/
            if (!MotionConfig.Instance.ReadMotionCardConfigFromXml(0, Application.StartupPath + @"\ExeFile\MotionConfig1.ini"))
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("运动控制卡初始化失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Motion card loading failed！", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Khởi tạo thẻ kiểm soát vận động hỏng！", Color.Red);
                }
                loadErr = true;
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("运动控制卡初始化成功！", Color.Lime);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Motion card loading successfully！", Color.Lime);
                }
                else
                {
                    Splash.ShowSplashTisp("Khởi tạo thẻ điều khiển vận động thành công！", Color.Lime);
                }
            }
            Thread.Sleep(500);
            #endregion

            #region 初始化IO卡
            if (!IOConfig.Instance.ReadIOCardConfigFromXml())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("IO卡初始化失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("IO card loading failed！", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Mã bộ khởi tạo không thành！", Color.Red);
                }
                loadErr = true;
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("IO卡初始化成功！", Color.Lime);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("IO card loading successfully！", Color.Lime);
                }
                else
                {
                    Splash.ShowSplashTisp("Đã khởi tạo thẻ Nhanh！", Color.Lime);
                }
            }
            IOConfig.Instance.SetSingleOut(_OutputCollect.照明.ToString(), 1);
            IOConfig.Instance.SetSingleOut(_OutputCollect.配电盘照明.ToString(), 1);
            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人远程开机.ToString(), 1);
            Task.Run(() =>
            {
                Thread.Sleep(500);
                IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人远程开机.ToString(), 0);
            });
            Thread.Sleep(500);
            #endregion

            #region 连接网络通信,如果无网口通讯注释此段代码
            if (LanguageConfig.Instance.Language == "CH")
            {
                Splash.ShowSplashTisp("网络通讯初始化中...", Color.Lime);
            }
            else if (LanguageConfig.Instance.Language == "EN")
            {
                Splash.ShowSplashTisp("Network initialization...", Color.Lime);
            }
            else
            {
                Splash.ShowSplashTisp("Khởi tạo giao thức mạng...", Color.Lime);
            }
            if (TCPClientConfig.Instance.OpenAllClient())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("网络通讯初始化成功！", Color.Lime);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Network communication initialization succeeded！", Color.Lime);
                }
                else
                {
                    Splash.ShowSplashTisp("Kết nối mạng khởi động！", Color.Lime);
                }
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("网络通讯初始化失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Network communication initialization failed！", Color.Lime);
                }
                else
                {
                    Splash.ShowSplashTisp("Khởi tạo giao thức mạng thất bại！", Color.Lime);
                }
                loadErr = true;
            }
            Thread.Sleep(500);
            #endregion

            #region aubo机器人sdk
            Splash.ShowSplashTisp("遨博机器人加载中...", Color.Lime);
            if (MyVariable.AuboSDKInstance())
            {
                if (AuboClass.Instance.Initial(MyVariable.ipAddressaubo, int.Parse(MyVariable.portaubo)))
                {
                    Splash.ShowSplashTisp("遨博机器人加载成功", Color.Lime);
                }
                else
                {
                    //loadErr = true;
                    Splash.ShowSplashTisp("遨博机器人加载失败", Color.Red);
                }
            }
            else
            {
                //loadErr = true;
                Splash.ShowSplashTisp("遨博机器人加载失败", Color.Red);
            }
            Thread.Sleep(500);
            #endregion


            #region 初始化串口通讯,如果无串口通讯注释此段代码
            if (LanguageConfig.Instance.Language == "CH")
            {
                Splash.ShowSplashTisp("串口通讯初始化中...", Color.Lime);
            }
            else if (LanguageConfig.Instance.Language == "EN")
            {
                Splash.ShowSplashTisp("Serial initialization...", Color.Lime);
            }
            else
            {
                Splash.ShowSplashTisp("Khởi đầu liên lạc nối tiếp...", Color.Lime);
            }

            if (SerialConfig.Instance.OpenAllSerial())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("串口初始化成功！", Color.Lime);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("The serialport initialized successfully！", Color.Lime);
                }
                else
                {
                    Splash.ShowSplashTisp("Thành lập chuỗi cổng nối！", Color.Lime);
                }
                timer1.Enabled = true;
                timer1.Interval = 500;
                timer1.Start();
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("串口初始化失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("The serialport initialized failed！", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Khởi tạo cổng seri bị lỗi！", Color.Lime);
                }
                loadErr = true;
            }
            Thread.Sleep(500);
            #endregion

            #region PLC  连接
            Splash.ShowSplashTisp("PLC连接中...", Color.Lime);
            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
            {
                if (!Program.modbusTcp_PLC.Connect())
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        Splash.ShowSplashTisp("PLC连接失败！", Color.Red);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        Splash.ShowSplashTisp("Device configuration parameter loading failed！", Color.Red);
                    }
                    else
                    {
                        Splash.ShowSplashTisp("Lỗi nạp Tham số cấu hình thiết bị！", Color.Red);
                    }
                    loadErr = true;
                }
                else
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        Splash.ShowSplashTisp("PLC连接成功！", Color.Lime);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        Splash.ShowSplashTisp("Device configuration parameters loaded successfully！", Color.Lime);
                    }
                    else
                    {
                        Splash.ShowSplashTisp("Thiết lập đã tải thành công！", Color.Lime);
                    }
                }
                Thread.Sleep(500);
            }
            #endregion

            #region 电动夹爪  连接
            Splash.ShowSplashTisp("电动夹爪连接中...", Color.Lime);
            if (!ClawConnect())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("电动夹爪连接失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Device configuration parameter loading failed！", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Lỗi nạp Tham số cấu hình thiết bị！", Color.Red);
                }
                loadErr = true;
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("电动夹爪连接成功！", Color.Lime);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Device configuration parameters loaded successfully！", Color.Lime);
                }
                else
                {
                    Splash.ShowSplashTisp("Thiết lập đã tải thành công！", Color.Lime);
                }
            }
            Thread.Sleep(500);
            #endregion

            #region 移液枪  连接
            Splash.ShowSplashTisp("移液枪连接中...", Color.Lime);
            MyVariable.ReadPipetteParam();
            if (!pipetteConnect())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("移液枪连接失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Device configuration parameter loading failed！", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Lỗi nạp Tham số cấu hình thiết bị！", Color.Red);
                }
                loadErr = true;
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("移液枪连接成功！", Color.Lime);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Device configuration parameters loaded successfully！", Color.Lime);
                }
                else
                {
                    Splash.ShowSplashTisp("Thiết lập đã tải thành công！", Color.Lime);
                }
            }
            Thread.Sleep(500);
            #endregion

            #region 打开本地服务器，如果无网口通讯注释此段代码
            //if (LanguageConfig.Instance.Language == "CH")
            //{
            //    Splash.ShowSplashTisp("服务器打开中...", Color.Lime);
            //}
            //else if (LanguageConfig.Instance.Language == "EN")
            //{
            //    Splash.ShowSplashTisp("The server is opening...", Color.Lime);
            //}
            //else
            //{
            //    Splash.ShowSplashTisp("Máy chủ đang mở...", Color.Lime);
            //}

            //if (SocketServerConfig.Instance.ReadCfgFromXml())
            //{
            //    if (LanguageConfig.Instance.Language == "CH")
            //    {
            //        Splash.ShowSplashTisp("服务器打开成功！", Color.Lime);
            //    }
            //    else if (LanguageConfig.Instance.Language == "EN")
            //    {
            //        Splash.ShowSplashTisp("Server opened successfully！", Color.Lime);
            //    }
            //    else
            //    {
            //        Splash.ShowSplashTisp("Máy chủ đã mở thành công！", Color.Lime);
            //    }
            //    foreach (var item in SocketServerConfig.Instance.m_listServers)
            //    {
            //        /***订阅接收数据事件***/
            //        item.DataReceived += OnDataReceived;
            //    }
            //}
            //else
            //{
            //    if (LanguageConfig.Instance.Language == "CH")
            //    {
            //        Splash.ShowSplashTisp("服务器打开失败！", Color.Red);
            //    }
            //    else if (LanguageConfig.Instance.Language == "EN")
            //    {
            //        Splash.ShowSplashTisp("Server failed to open！", Color.Red);
            //    }
            //    else
            //    {
            //        Splash.ShowSplashTisp("Máy chủ không mở được！", Color.Red);
            //    }
            //    loadErr = true;
            //}
            #endregion

            #region 加载汇川机器人
            //if (!InovanceRobotConfig.Instance.ReadRobotCfgFromXml())
            //{
            //    if (LanguageConfig.Instance.Language == "CH")
            //    {
            //        Splash.ShowSplashTisp("汇川机器人初始化失败！", Color.Red);
            //    }
            //    else if (LanguageConfig.Instance.Language == "EN")
            //    {
            //        Splash.ShowSplashTisp("Inovance Robot initialization NG！", Color.Red);
            //    }
            //    else
            //    {
            //        Splash.ShowSplashTisp("Inovance Robot không khởi chạy được！", Color.Red);
            //    }
            //    loadErr = true;
            //}
            #endregion

            #region 加载东芝机械手
            //if (!TsRemoteRobotConfig.Instance.ReadRobotCfgFromXml())
            //{
            //    if (LanguageConfig.Instance.Language == "CH")
            //    {
            //        Splash.ShowSplashTisp("东芝机械手初始化失败！", Color.Red);
            //    }
            //    else if (LanguageConfig.Instance.Language == "EN")
            //    {
            //        Splash.ShowSplashTisp("TsRemote Robot initialization NG！", Color.Red);
            //    }
            //    else
            //    {
            //        Splash.ShowSplashTisp("TsRemote Robot không khởi chạy được！", Color.Red);
            //    }
            //    loadErr = true;
            //}
            #endregion

            #region 加载LCTE-MINI-4O4-2O2模块
            //if (!LCTEmini2O2Config.Instance.miniHandle.InitialMiniBus())
            //{
            //    if (LanguageConfig.Instance.Language == "CH")
            //    {
            //        Splash.ShowSplashTisp("LCTE-MINI-2O2 模块初始化失败！", Color.Red);
            //    }
            //    else if (LanguageConfig.Instance.Language == "EN")
            //    {
            //        Splash.ShowSplashTisp("LCTE-MINI-2O2 initialization NG！", Color.Red);
            //    }
            //    else
            //    {
            //        Splash.ShowSplashTisp("LCTE-MINI-2O2 không khởi chạy được！", Color.Red);
            //    }
            //    loadErr = true;
            //}
            #endregion

            #region 添加按钮和窗体的字典
            mFormDic.Add(rbt_Main, new MainForm());
            mFormDic.Add(rbt_Debug, new DebugForm());
            mFormDic.Add(rbt_Vision, new VisionForm());
            mFormDic.Add(rbt_Alarm, new ErrorForm());
            mFormDic.Add(rbt_Yield, new CapacityForm());
            mFormDic.Add(rbt_Machine, new MachineForm());
            mFormDic.Add(rbt_Data, new DataForm());
            mFormDic.Add(rbt_Image, new ImageForm());
            mFormDic.Add(rbt_Admin, new LoginForm());
            rbt_Main.PerformClick();
            #endregion


            #region 读取宕机记录+读取抛料记录
            if (!DownTime.Instance.ReadDowntime())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("宕机配置读取失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Failed to read the downtime configuration!", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Lỗi đọc cấu hình thời gian chết!", Color.Red);
                }
                loadErr = true;
            }
            if (!Discard.Instance.ReadDiscard())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("抛料配置读取失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Failed to read the throwing configuration!", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Lỗi đọc cấu hình ném!", Color.Red);
                }
                loadErr = true;
            }
            #endregion

            #region 读取交接班时间+读取产量统计信息
            if (!Yield.Instance.ReadYield())
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    Splash.ShowSplashTisp("产能配置读取失败！", Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    Splash.ShowSplashTisp("Failed to read capacity configuration!", Color.Red);
                }
                else
                {
                    Splash.ShowSplashTisp("Lỗi đọc cấu hình năng lực!", Color.Red);
                }
                loadErr = true;
            }
            Thread.Sleep(200);
            #endregion

            Splash.KillSplash();

            #region 用户权限设定
            /***订阅事件***/
            AdminConfig.Instance.UserLogInChanged += mUser_UserLogInChanged;
            /***默认操作员权限***/
            AdminConfig.Instance.ExitLogin();
            #endregion

            /***最大化窗口***/
            this.WindowState = FormWindowState.Maximized;


            #region 添加站位
            StationConfig.Instance.AddStation(_ThreadModule.总线程.ToString(), new MainStation(_ThreadModule.总线程.ToString()));
            StationConfig.Instance.AddStation(_ThreadModule.供料线程.ToString(), new FeedingStation(_ThreadModule.供料线程.ToString()));
            StationConfig.Instance.AddStation(_ThreadModule.搬运工位.ToString(), new CarryStation(_ThreadModule.搬运工位.ToString()));
            StationConfig.Instance.AddStation(_ThreadModule.测序仪工位.ToString(), new SequencingStation(_ThreadModule.测序仪工位.ToString()));
            StationConfig.Instance.AddStation(_ThreadModule.机器人工位.ToString(), new RobotStation(_ThreadModule.机器人工位.ToString()));
            StationConfig.Instance.AddStation(_ThreadModule.状态监控线程.ToString(), new MonitorStation(_ThreadModule.状态监控线程.ToString()));
            StationConfig.Instance.AddStation(_ThreadModule.数据处理线程.ToString(), new DataProcessingStation(_ThreadModule.数据处理线程.ToString()));
            StationConfig.Instance.AddStation(_ThreadModule.数字孪生线程.ToString(), new AnimationStation(_ThreadModule.数字孪生线程.ToString()));
            #endregion

            #region 订阅主线程状态改变事件，同时切换Stop
            StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].StatusChangeEvent += SoftWareForm_StatusChangeEvent;
            StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus = ObjectStation._StationStatus.Stop;
            SoftWareForm_StatusChangeEvent(ObjectStation._StationStatus.Stop);
            #endregion

            #region 订阅主线程模式改变状态，同时切换自动运行模式
            StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ModeChangeEvent += SoftWareForm_ModeChangeEvent;
            StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurMode = ObjectStation._RunMode.NormalRun;
            SoftWareForm_ModeChangeEvent(ObjectStation._RunMode.NormalRun);
            #endregion

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += SoftWareForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);

            if (LanguageConfig.Instance.Language == "CH")
            {
                langaugeList2.CMB1.SelectedIndex = 0;
            }
            else if (LanguageConfig.Instance.Language == "EN")
            {
                langaugeList2.CMB1.SelectedIndex = 1;
            }
            else
            {
                langaugeList2.CMB1.SelectedIndex = 2;
            }

            if (!loadErr)
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "程序加载成功！", MsgType.Success, Color.Green);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "Program loaded successfully！", MsgType.Success, Color.Green);
                }
                else
                {
                    LogConfig.Instance.ShowMessageToList("Run", "Chương trình nạp thành công！", MsgType.Success, Color.Green);
                }
                /***开启所有站位***/
                StationConfig.Instance.StartStation();
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "程序加载失败！", MsgType.Error, Color.Red);
                    LogConfig.Instance.ShowMessageToList("Run", "请排查异常后重新打开程序！", MsgType.Error, Color.Red);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "Program loading failed！", MsgType.Error, Color.Red);
                    LogConfig.Instance.ShowMessageToList("Run", "Please check the exception and open the program again！", MsgType.Error, Color.Red);
                }
                else
                {
                    LogConfig.Instance.ShowMessageToList("Run", "Lỗi tải chương trình！", MsgType.Error, Color.Red);
                    LogConfig.Instance.ShowMessageToList("Run", "Xin hãy kiểm tra ngoại lệ và mở lại chương trình！", MsgType.Error, Color.Red);
                }
            }

            #region 屏蔽参数显示
            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)))
            {
                tsl_general.Text = "屏蔽";
                tsl_general.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
            }
            else
            {
                tsl_general.Text = "正常";
                tsl_general.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
            }
            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
            {
                tsl_plc.Text = "屏蔽";
                tsl_plc.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
            }
            else
            {
                tsl_plc.Text = "正常";
                tsl_plc.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
            }
            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
            {
                tsl_ccd.Text = "屏蔽";
                tsl_ccd.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
            }
            else
            {
                tsl_ccd.Text = "正常";
                tsl_ccd.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
            }
            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledBarcode.ToString()].CurrentValue)))
            {
                tsl_barcord.Text = "屏蔽";
                tsl_barcord.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
            }
            else
            {
                tsl_barcord.Text = "正常";
                tsl_barcord.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
            }
            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)))
            {
                tsl_sequence.Text = "屏蔽";
                tsl_sequence.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
            }
            else
            {
                tsl_sequence.Text = "正常";
                tsl_sequence.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
            }
            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledAnimation.ToString()].CurrentValue)))
            {
                tsl_animation.Text = "屏蔽";
                tsl_animation.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
            }
            else
            {
                tsl_animation.Text = "正常";
                tsl_animation.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
            }
            #endregion

            MyVariable.EmptyRun_Restart = true;
        }

        private void SoftWareForm_StatusChangeEvent(ObjectStation._StationStatus newStatus)
        {
            this.Invoke(new Action(() =>
            {
                switch (newStatus)
                {
                    case ObjectStation._StationStatus.Alarm:
                        lab_Status.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "Alarm", "报警中");
                        lab_Status.BackColor = Color.Red;
                        rbt_Start.ImageIndex = 10;
                        rbt_Pause.ImageIndex = 12;
                        rbt_Stop.ImageIndex = 15;
                        ktCntDll.KpcStopTaskExe(123);
                        SerializeClass.animationParam.machineStatus = (int)_machineStatusEnum.报警中;
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 1);
                        Task.Run(() =>
                        {
                            Thread.Sleep(500);
                            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 0);
                        });
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                        {
                            IOConfig.Instance.LightAction(LightState.红灯闪);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.复位按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.启动按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.停止按钮灯.ToString(), 0);
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                        {
                            if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64609, 1) && Program.modbusTcp_PLC.WriteSingleRegister(1, 64612, 0))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64609地址写: 1;64612地址写: 0", MsgType.Success, Color.Green);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64609地址写: 1;64612地址写: 0 失败", MsgType.Success, Color.Red);
                            }
                        }
                        break;

                    case ObjectStation._StationStatus.Error:
                        lab_Status.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "Error", "异常中");
                        lab_Status.BackColor = Color.Pink;
                        rbt_Start.ImageIndex = 10;
                        rbt_Pause.ImageIndex = 13;
                        rbt_Stop.ImageIndex = 14;
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序暂停.ToString(), 1);
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                        {
                            IOConfig.Instance.LightAction(LightState.红灯闪);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.复位按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.启动按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.停止按钮灯.ToString(), 0);
                        SerializeClass.animationParam.machineStatus = (int)_machineStatusEnum.异常中;
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                        {
                            if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64610, 1) && Program.modbusTcp_PLC.WriteSingleRegister(1, 64612, 0))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64610地址写: 1;64612地址写: 0", MsgType.Success, Color.Green);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64610地址写: 1;64612地址写: 0 失败", MsgType.Success, Color.Red);
                            }
                        }
                        break;

                    case ObjectStation._StationStatus.Initial:
                        lab_Status.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "Initial", "复位中");
                        lab_Status.BackColor = Color.LightSkyBlue;
                        rbt_Start.ImageIndex = 10;
                        rbt_Pause.ImageIndex = 12;
                        rbt_Stop.ImageIndex = 14;
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                        {
                            IOConfig.Instance.LightAction(LightState.黄灯闪);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.启动按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.停止按钮灯.ToString(), 0);
                        SerializeClass.animationParam.machineStatus = (int)_machineStatusEnum.复位中;
                        break;

                    case ObjectStation._StationStatus.Pause:
                        lab_Status.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "Pause", "暂停中");
                        lab_Status.BackColor = Color.Gray;
                        rbt_Start.ImageIndex = 10;
                        rbt_Pause.ImageIndex = 13;
                        rbt_Stop.ImageIndex = 14;
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序暂停.ToString(), 1);
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                        {
                            IOConfig.Instance.LightAction(LightState.黄灯开);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.复位按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.启动按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.停止按钮灯.ToString(), 1);
                        SerializeClass.animationParam.machineStatus = (int)_machineStatusEnum.暂停中;
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                        {
                            if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64610, 1) && Program.modbusTcp_PLC.WriteSingleRegister(1, 64612, 0))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64610地址写: 1;64612地址写: 0", MsgType.Success, Color.Green);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64610地址写: 1 ;64612地址写: 0 失败", MsgType.Success, Color.Red);
                            }
                        }
                        break;

                    case ObjectStation._StationStatus.Run:
                        lab_Status.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "Run", "运行中");
                        lab_Status.BackColor = Color.Lime;
                        rbt_Start.ImageIndex = 11;
                        rbt_Pause.ImageIndex = 12;
                        rbt_Stop.ImageIndex = 14;
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序暂停.ToString(), 0);
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                        {
                            IOConfig.Instance.LightAction(LightState.绿灯开);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.复位按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.启动按钮灯.ToString(), 1);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.停止按钮灯.ToString(), 0);
                        SerializeClass.animationParam.machineStatus = (int)_machineStatusEnum.运行中;
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                        {
                            if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64609, 0) && Program.modbusTcp_PLC.WriteSingleRegister(1, 64610, 0))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64609,64610地址写: 0", MsgType.Success, Color.Green);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64609,64610地址写: 0 失败", MsgType.Success, Color.Red);
                            }
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲
                                && SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.空闲
                                && SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲
                                && SerializeClass.mMemory.DataProcessingStation_state == MemoryClass.DataProcessingStation_State.空闲
                                && SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                            {
                                if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64612, 1))
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "64612地址写: 1", MsgType.Success, Color.Green);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "64612地址写: 1 失败", MsgType.Success, Color.Red);
                                }
                            }
                        }
                        break;

                    case ObjectStation._StationStatus.Stop:
                        lab_Status.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "Stop", "停止中");
                        lab_Status.BackColor = Color.Orange;
                        rbt_Start.ImageIndex = 10;
                        rbt_Pause.ImageIndex = 12;
                        rbt_Stop.ImageIndex = 14;
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                        {
                            IOConfig.Instance.LightAction(LightState.黄灯开);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.复位按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.启动按钮灯.ToString(), 0);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.停止按钮灯.ToString(), 0);
                        SerializeClass.animationParam.machineStatus = (int)_machineStatusEnum.停止中;
                        if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                        {
                            if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64610, 1) && Program.modbusTcp_PLC.WriteSingleRegister(1, 64612, 0))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64610地址写: 1;64612地址写: 0", MsgType.Success, Color.Green);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64610地址写: 1;64612地址写: 0 失败", MsgType.Success, Color.Red);
                            }
                        }
                        break;
                }
            }));
        }
        private void ChangeHiveStatus(ObjectStation._StationStatus newStatus)
        {
            if (Program.m_Hive == null)
            {
                return;
            }
            switch (newStatus)
            {
                case ObjectStation._StationStatus.Alarm:

                    Program.m_Hive.HiveErrorMsg(ErrorMessage());

                    if (Program.m_Hive.HiveStatus != _HiveMachineStaus.计划停机状态)
                    {
                        Program.m_Hive.HiveStatus = _HiveMachineStaus.宕机状态;
                    }
                    break;
                case ObjectStation._StationStatus.Error:
                    Program.m_Hive.HiveErrorMsg(ErrorMessage());
                    if (Program.m_Hive.HiveStatus != _HiveMachineStaus.计划停机状态)
                    {
                        Program.m_Hive.HiveStatus = _HiveMachineStaus.宕机状态;
                    }
                    break;
                case ObjectStation._StationStatus.Pause:
                    if (Program.m_Hive.HiveStatus == _HiveMachineStaus.正常做料状态 || Program.m_Hive.HiveStatus == _HiveMachineStaus.屏蔽上传做料状态)
                    {
                        Program.m_Hive.HiveStatus = _HiveMachineStaus.空闲状态;
                    }
                    break;
                case ObjectStation._StationStatus.Stop:
                    if (Program.m_Hive.HiveStatus == _HiveMachineStaus.正常做料状态 || Program.m_Hive.HiveStatus == _HiveMachineStaus.屏蔽上传做料状态)
                    {
                        Program.m_Hive.HiveStatus = _HiveMachineStaus.空闲状态;
                    }
                    break;
                case ObjectStation._StationStatus.Initial:
                    if (Program.m_Hive.HiveStatus == _HiveMachineStaus.正常做料状态 || Program.m_Hive.HiveStatus == _HiveMachineStaus.屏蔽上传做料状态)
                    {
                        Program.m_Hive.HiveStatus = _HiveMachineStaus.空闲状态;
                    }
                    break;
                case ObjectStation._StationStatus.Run:
                    if (Program.m_Hive.HiveStatus == _HiveMachineStaus.计划停机状态)
                    {
                        if (Program.m_Hive.ShiledUpload)
                        {
                            Program.m_Hive.HiveStatus = _HiveMachineStaus.屏蔽上传做料状态;
                        }
                        else
                        {
                            Program.m_Hive.HiveStatus = _HiveMachineStaus.正常做料状态;
                        }
                    }
                    else if (Program.m_Hive.HiveStatus == _HiveMachineStaus.宕机状态)
                    {
                        if (Program.m_Hive.ShiledUpload)
                        {
                            Program.m_Hive.HiveStatus = _HiveMachineStaus.屏蔽上传做料状态;
                        }
                        else
                        {
                            Program.m_Hive.HiveStatus = _HiveMachineStaus.正常做料状态;
                        }
                    }
                    break;
            }
        }

        private string ErrorMessage()
        {
            string errmsg = string.Empty;
            if (DownTime.Instance.CurDayDownTimeList.Count > 0)
            {
                errmsg = DownTime.Instance.CurDayDownTimeList[DownTime.Instance.CurDayDownTimeList.Count - 1].ErrorMsg;
            }
            else
            {
                LogConfig.Instance.ShowMessageToList("Alarm", "代码编写失误！！缺少代码 开始宕机方法\"StartDowntime\"", MsgType.NG, Color.Tomato);
            }
            return errmsg;
        }

        private void SoftWareForm_ModeChangeEvent(ObjectStation._RunMode newMode)
        {
            this.Invoke(new Action(() =>
            {
                switch (newMode)
                {
                    case ObjectStation._RunMode.NormalRun:
                        lab_RunMode.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "NormalRun", "自动生产模式");
                        break;

                    case ObjectStation._RunMode.EmptyRun:
                        lab_RunMode.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "EmptyRun", "空跑模式");
                        break;

                    case ObjectStation._RunMode.AutoCalib:
                        lab_RunMode.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "AutoCalib", "自动标定模式");
                        break;

                    case ObjectStation._RunMode.CPKMode:
                        lab_RunMode.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "CPKMode", "自动CPK模式");
                        break;

                    case ObjectStation._RunMode.GRRMode:
                        lab_RunMode.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "GRRMode", "自动GRR模式");
                        break;

                    case ObjectStation._RunMode.CamStatisMode:
                        lab_RunMode.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "CamStatisMode", "相机静态测试模式");
                        break;

                    case ObjectStation._RunMode.CamDynamicMode:
                        lab_RunMode.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "CamDynamicMode", "相机动态测试模式");
                        break;
                }
            }));
        }

        private void mUser_UserLogInChanged(object sender, AdminConfig.UserLogChangedEventArgs e)
        {
            if (e.UserLevel == 0)
            {
                lab_Admin.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "OP", "操作员");
                lab_Admin.ForeColor = Color.Orange;
            }
            else if (e.UserLevel == 1)
            {
                lab_Admin.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "EE", "工程师");
                lab_Admin.ForeColor = Color.Blue;
            }
            else
            {
                lab_Admin.Text = LanguageConfig.Instance.GetString(this.GetType().Namespace, this.GetType().Name, "AD", "管理员");
                lab_Admin.ForeColor = Color.LimeGreen;
            }
        }

        public void SwitchWnd(RoundButton btn)
        {
            if (mRoundBtn != btn)
            {
                if (mRoundBtn != null)
                    mRoundBtn.ImageIndex--;
                mRoundBtn = btn;
                mRoundBtn.ImageIndex++;
                if (mCurForm != null)
                    mCurForm.Hide();
                if (mCurForm != mFormDic[btn])
                {
                    mCurForm = mFormDic[btn];
                    mCurForm.TopLevel = false;
                    mCurForm.Parent = panel_Main;
                    mCurForm.Dock = DockStyle.Fill;
                    mCurForm.Show();
                }
            }
        }

        private void rbt_Main_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Main);
        }

        private void rbt_Debug_Click(object sender, EventArgs e)
        {
            //if (AdminConfig.Instance.UserLevel == 0)
            //{
            //    MessageBox.Show("用户权限等级不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (AdminConfig.Instance.CurEnterEmployee.Name == string.Empty)
            //{
            //    MessageBox.Show("登录者未刷ID卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            SwitchWnd(rbt_Debug);
        }

        private void rbt_Vision_Click(object sender, EventArgs e)
        {
            //if (AdminConfig.Instance.UserLevel == 0)
            //{
            //    MessageBox.Show("用户权限等级不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (AdminConfig.Instance.CurEnterEmployee.Name == string.Empty)
            //{
            //    MessageBox.Show("登录者未刷ID卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            SwitchWnd(rbt_Vision);
        }

        private void rbt_Alarm_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Alarm);
        }

        private void rbt_Yield_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Yield);
        }

        private void rbt_Machine_Click(object sender, EventArgs e)
        {
            //SwitchWnd(rbt_Machine);
        }

        private void rbt_Data_Click(object sender, EventArgs e)
        {
            string v_OpenFolderPath = @"E:\SWLog";
            if (!Directory.Exists(v_OpenFolderPath))
            {
                return;
            }
            System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
            SwitchWnd(rbt_Data);
        }

        private void rbt_Image_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Image);
        }

        private void rbt_Admin_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Admin);
        }

        private void SoftWareForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            /***停止所有站位***/
            StationConfig.Instance.StopStation();
            /***注销IO卡***/
            IOConfig.Instance.ReleaseIOCard();
            /***关闭运动控制卡***/
            MotionConfig.Instance.CloseCard();
            /***断开东芝机械手的连接(如果有)***/
            TsRemoteRobotConfig.Instance.DisConnectTsRemoteRobot();
            /***断开汇川机械手的连接(如果有)***/
            InovanceRobotConfig.Instance.DisConnectInovanceRobot();
            /***关闭所有网络连接(如果有)***/
            TCPClientConfig.Instance.CloseAllClient();
            /***关闭所有串口连接(如果有)***/
            SerialConfig.Instance.CloseAllSerial();
            this.Dispose();
            Application.Exit();
            /***结束所有程序进程(可不添加)***/
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        private string pathAnimation = @"E:\SWLog\Animation";
        private void SoftWareForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("是否确认关闭软件", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.ServiceNotification);
            if (res == DialogResult.Yes)
            {
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
                IOConfig.Instance.SetSingleOut(_OutputCollect.照明.ToString(), 0);
                IOConfig.Instance.SetSingleOut(_OutputCollect.配电盘照明.ToString(), 0);
                IOConfig.Instance.SetSingleOut(_OutputCollect.复位按钮灯.ToString(), 0);
                IOConfig.Instance.SetSingleOut(_OutputCollect.启动按钮灯.ToString(), 0);
                IOConfig.Instance.SetSingleOut(_OutputCollect.停止按钮灯.ToString(), 0);
                FileSave.DeleteFilesOlderThanDays(pathAnimation, 7);
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void rbt_Start_Click(object sender, EventArgs e)
        {
            bool mStartFail = false;
            string mTigStr = string.Empty;
            int index = ParameConfig.Instance.SystemParamTypeNameList.IndexOf("SafetyShiledParame");
            int index2 = 0;
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus != ObjectStation._StationStatus.Alarm)
            {
                if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ResetDone)
                {
                    if (IOConfig.Instance.SafetyNames.Count > 0)
                    {
                        /***启动前检查安全装置信息***/
                        for (int i = 0; i < IOConfig.Instance.SafetyNames.Count; i++)
                        {
                            index2 = IOConfig.Instance.InputNames[0].FindIndex(item => item.Equals(IOConfig.Instance.SafetyNames[i]));

                            /***如果高电平有效***/
                            if (IOConfig.Instance.SafetyDictionary[IOConfig.Instance.SafetyNames[i]].PointLevel == 1)
                            {
                                if (IOConfig.Instance.SafetyStatus[i])
                                {
                                    if (ParameConfig.Instance.RefineSystemParame[index].ElementAt(i).Value.CurrentValue == "0")
                                    {
                                        if (LanguageConfig.Instance.Language == "CH")
                                        {
                                            mTigStr = IOConfig.Instance.SafetyNames[i];
                                        }
                                        else if (LanguageConfig.Instance.Language == "EN")
                                        {
                                            mTigStr = IOConfig.Instance.InputNames[1][index2];
                                        }
                                        else
                                        {
                                            mTigStr = IOConfig.Instance.InputNames[2][index2];
                                        }
                                        mStartFail = true;
                                        break;
                                    }
                                }
                            }
                            /***如果低电平有效***/
                            else if (IOConfig.Instance.SafetyDictionary[IOConfig.Instance.SafetyNames[i]].PointLevel == 0)
                            {
                                if (!IOConfig.Instance.SafetyStatus[i])
                                {
                                    if (ParameConfig.Instance.RefineSystemParame[index].ElementAt(i).Value.CurrentValue == "0")
                                    {
                                        if (LanguageConfig.Instance.Language == "CH")
                                        {
                                            mTigStr = IOConfig.Instance.SafetyNames[i];
                                        }
                                        else if (LanguageConfig.Instance.Language == "EN")
                                        {
                                            mTigStr = IOConfig.Instance.InputNames[1][index2];
                                        }
                                        else
                                        {
                                            mTigStr = IOConfig.Instance.InputNames[2][index2];
                                        }
                                        mStartFail = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    /***是否启动失败***/
                    if (mStartFail)
                    {
                        if (LanguageConfig.Instance.Language == "CH")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", mTigStr + "触发,设备启动失败！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                        else if (LanguageConfig.Instance.Language == "EN")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", mTigStr + "is triggered , the device fails to start！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                        else
                        {
                            LogConfig.Instance.ShowMessageToList("Run", mTigStr + "Kích hoạt" + " Không thể khởi động thiết bị！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                    }
                    else if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Run)
                    {
                        if (LanguageConfig.Instance.Language == "CH")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "设备正在运行，启动按钮无效！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                        else if (LanguageConfig.Instance.Language == "EN")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "The device is running and the start button is invalid！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                        else
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "Thiết bị đang chạy và nút khởi động không hợp lệ！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                    }
                    else if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Initial)
                    {
                        if (LanguageConfig.Instance.Language == "CH")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "设备复位中，启动按钮无效！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                        else if (LanguageConfig.Instance.Language == "EN")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "During device reset, the start button is invalid！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                        else
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "Trong khi đặt lại thiết bị, nút bắt đầu không hợp lệ！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                    }
                    else if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Error)
                    {
                        if (LanguageConfig.Instance.Language == "CH")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "设备异常未清除，启动按钮无效！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                        else if (LanguageConfig.Instance.Language == "EN")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "The equipment exception is not cleared, and the start button is invalid！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                        else
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "Sự bất thường của thiết bị không được xóa và nút khởi động không hợp lệ！", MsgType.Warning, Color.DarkGoldenrod);
                        }
                    }
                    else if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop ||
                        StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
                    {
                        if (LanguageConfig.Instance.Language == "CH")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "设备启动成功！", MsgType.Success, Color.Green);
                        }
                        else if (LanguageConfig.Instance.Language == "EN")
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "The device starts successfully！", MsgType.Success, Color.Green);
                        }
                        else
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "Thiết bị khởi động thành công！", MsgType.Success, Color.Green);
                        }
                        StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].RunDone = false;
                        StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(ObjectStation._StationStatus.Run);
                        switch (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurMode)
                        {
                            case ObjectStation._RunMode.NormalRun:
                                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].NormalRunEvent.Set();
                                break;
                            case ObjectStation._RunMode.EmptyRun:
                                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].EmptyRunEvent.Set();
                                break;
                            case ObjectStation._RunMode.AutoCalib:
                                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].CalibEvent.Set();
                                break;
                            case ObjectStation._RunMode.GRRMode:
                                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].GRREvent.Set();
                                break;
                            case ObjectStation._RunMode.CPKMode:
                                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].CPKEvent.Set();
                                break;
                            case ObjectStation._RunMode.CamStatisMode:
                                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].CamStaticEvent.Set();
                                break;
                            case ObjectStation._RunMode.CamDynamicMode:
                                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].CamDynamicEvent.Set();
                                break;
                        }
                    }
                }
                else
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        LogConfig.Instance.ShowMessageToList("Run", "设备未复位，启动失败！", MsgType.Warning, Color.DarkGoldenrod);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        LogConfig.Instance.ShowMessageToList("Run", "The device is not reset and failed to start！", MsgType.Warning, Color.DarkGoldenrod);
                    }
                    else
                    {
                        LogConfig.Instance.ShowMessageToList("Run", "Thiết bị không được đặt lại và không khởi động được！", MsgType.Warning, Color.DarkGoldenrod);
                    }
                }
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "设备报警中，复位失败！", MsgType.Warning, Color.DarkGoldenrod);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "In the equipment alarm, the machine reset failed！", MsgType.Warning, Color.DarkGoldenrod);
                }
                else
                {
                    LogConfig.Instance.ShowMessageToList("Run", "Trong báo động thiết bị, việc đặt lại không thành công！", MsgType.Warning, Color.DarkGoldenrod);
                }
            }
        }

        private void rbt_Pause_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Run)
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "暂停按钮按下，设备暂停！", MsgType.Warning, Color.DarkGoldenrod);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "When the pause button is pressed, the device pauses！", MsgType.Warning, Color.DarkGoldenrod);
                }
                else
                {
                    LogConfig.Instance.ShowMessageToList("Run", "Khi nhấn nút tạm dừng, thiết bị sẽ tạm dừng！", MsgType.Warning, Color.DarkGoldenrod);
                }
                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(ObjectStation._StationStatus.Pause);
            }
        }

        private void rbt_Stop_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Run)
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "停止按钮按下，设备停止，需重新复位！", MsgType.Warning, Color.DarkGoldenrod);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    LogConfig.Instance.ShowMessageToList("Run", "When the stop button is pressed, the device stops and needs to be reset！", MsgType.Warning, Color.DarkGoldenrod);
                }
                else
                {
                    LogConfig.Instance.ShowMessageToList("Run", "Khi nhấn nút dừng, thiết bị sẽ dừng và cần được đặt lại！", MsgType.Warning, Color.DarkGoldenrod);
                }
                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(ObjectStation._StationStatus.Alarm);
                StationConfig.Instance.MainStation.ResetDone = false;
            }
        }

        private void langaugeList2_CmbSelectClick(object sender, EventArgs e)
        {
            if (langaugeList2.Langauge_Select == 0)
            {
                LanguageConfig.Instance.ChangeLanguage("CH");
            }
            else if (langaugeList2.Langauge_Select == 1)
            {
                LanguageConfig.Instance.ChangeLanguage("EN");
            }
            else if (langaugeList2.Langauge_Select == 2)
            {
                LanguageConfig.Instance.ChangeLanguage("VN");
            }
        }

        private void OnDataReceived(object sender, AsyncSocketEventArgs e)
        {
            if (IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    SocketServer server = (SocketServer)sender;
                    string strData = "";
                    /***可以通过e.m_state.ClientSocket.RemoteEndPoint种包含的IP地址来判断接收的是哪个客户端发送的数据***/
                    string clientStr = e.m_state.ClientSocket.RemoteEndPoint.ToString();
                    strData += server.Name + ": Receive from " + clientStr + "> ";
                    strData += server.Encoding.GetString(e.m_state.RecvDataBuffer, 0, e.m_state.Length);
                    LogConfig.Instance.ShowMessageToList("Run", strData, MsgType.Success, Color.Green);
                });
            }
        }



        private bool isProcessing = false;
        Stopwatch watch = new Stopwatch();
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (isProcessing)
                {
                    return;
                }
                if (MyVariable.num > 2)
                {
                    if (!bt)
                    {
                        MyVariable.b_temperature = true;
                        bt = true;
                    }
                    return;
                }
                bt = false;
                isProcessing = true;
                SerialConfig.Instance.GetSerial(_SerialModule.TemperatureControl.ToString()).WriteByte(SerialConfig.Instance.GetSerial(_SerialModule.TemperatureControl.ToString()).StrToByte("01 03 00 00 00 02 C4 0B"));
                Serial sr = SerialConfig.Instance.GetSerial(_SerialModule.TemperatureControl.ToString());
                watch.Restart();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        while (true)
                        {
                            Thread.Sleep(50);
                            if (watch.ElapsedMilliseconds > 3000)
                            {
                                watch.Stop();
                                LogConfig.Instance.ShowMessageToList("Run", "温控表通讯超时", MsgType.Success, Color.Red);
                                SerialConfig.Instance.ReOpenSerial(_SerialModule.TemperatureControl.ToString());
                                Thread.Sleep(500);
                                MyVariable.num++;
                                isProcessing = false;
                                return;
                            }
                            if (sr.SerialCanRead())
                            {
                                Thread.Sleep((int)sr.DelayTime * 1000);
                                byte[] str;
                                sr.ReadByte(out str);
                                this.Invoke(new Action(() =>
                                {
                                    if (str.Length >= 6)
                                    {
                                        if (str[3].ToString() == "255" && str[5].ToString() == "255")
                                        {
                                            SerializeClass.animationParam.temperature = (Convert.ToDouble(str[6]) - 256) / 10;
                                            tsl_temperature.Text = ((Convert.ToDouble(str[6]) - 256) / 10).ToString();
                                        }
                                        if (str[3].ToString() == "0" && str[5].ToString() == "0")
                                        {
                                            SerializeClass.animationParam.temperature = Convert.ToDouble(str[6]) / 10;
                                            tsl_temperature.Text = (Convert.ToDouble(str[6]) / 10).ToString();
                                        }
                                        if (str[3].ToString() == "0" && str[5].ToString() != "0")
                                        {
                                            SerializeClass.animationParam.temperature = (Convert.ToDouble(str[6]) + (Convert.ToDouble(str[5]) * 16 * 16)) / 10;
                                            tsl_temperature.Text = ((Convert.ToDouble(str[6]) + (Convert.ToDouble(str[5]) * 16 * 16)) / 10).ToString();
                                        }
                                    }
                                }));
                                sr.ClearBuffer();
                                MyVariable.num = 0;
                                isProcessing = false;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isProcessing = false;
                        MyVariable.num++;
                        LogConfig.Instance.ShowMessageToList("Run", "温控表读取异常 : " + ex.Message, MsgType.Success, Color.Red);
                    }
                });
            }
            catch (Exception es)
            {
                LogConfig.Instance.ShowMessageToList("Run", "温控表连接异常 : " + es.Message, MsgType.Success, Color.Red);
                SerialConfig.Instance.ReOpenSerial(_SerialModule.TemperatureControl.ToString());
                Thread.Sleep(500);
                MyVariable.num++;
                isProcessing = false;
            }
        }
    }
}