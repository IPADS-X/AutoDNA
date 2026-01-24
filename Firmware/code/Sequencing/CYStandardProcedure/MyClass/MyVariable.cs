using CYAutoFramework;
using ktCnt;
using ModbusLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CYStandardProcedure
{
    public class MyVariable
    {
        #region 实例化区域信息
        /*******************************实例化区域信息********************************************/
        public static AreaMessage area_QiangTou1 = new AreaMessage();
        public static AreaMessage area_QiangTou2 = new AreaMessage();
        public static AreaMessage area_QiangTou3 = new AreaMessage();
        public static AreaMessage area_QiangTou4 = new AreaMessage();
        public static AreaMessage area_DiWen_FCT = new AreaMessage();
        public static AreaMessage area_DiWen_FCF = new AreaMessage();
        public static AreaMessage area_DiWen_SB = new AreaMessage();
        public static AreaMessage area_DiWen_LIB = new AreaMessage();
        public static AreaMessage area_DiWen_DIL = new AreaMessage();
        public static AreaMessage area_DiWen_WMX = new AreaMessage();
        public static AreaMessage area_DiWen_S = new AreaMessage();
        public static AreaMessage area_LiXinGuan = new AreaMessage();
        public static AreaMessage area_8LianPai = new AreaMessage();
        #endregion

        #region 机器人指令
        /**************************机器人指令*********************************/
        /// <summary>
        /// 机器人工程启动指令
        /// </summary>
        public static string robot_project_StartCmd = "DATA_HEAD00000036{\"program control\":\"start\"}DATA_TAIL";
        /// <summary>
        /// 机器人工程暂停指令
        /// </summary>
        public static string robot_project_PauseCmd = "DATA_HEAD00000036{\"program control\":\"pause\"}DATA_TAIL";
        /// <summary>
        /// 机器人工程继续指令
        /// </summary>
        public static string robot_project_ContinueCmd = "DATA_HEAD00000039{\"program control\":\"continue\"}DATA_TAIL";
        /// <summary>
        /// 机器人工程停止指令
        /// </summary>
        public static string robot_project_StopCmd = "DATA_HEAD00000035{\"program control\":\"stop\"}DATA_TAIL";
        /// <summary>
        /// 机器人运动指令
        /// </summary>
        public static string robot_RunCmd;
        /// <summary>
        /// 机器人步序
        /// </summary>
        public static string robot_RunStep;
        /// <summary>
        /// 机器人X方向运动偏移
        /// </summary>
        public static string robot_XShift;
        /// <summary>
        /// 机器人Y方向运动偏移
        /// </summary>
        public static string robot_YShift;
        /// <summary>
        /// 机器人Z方向运动偏移
        /// </summary>
        public static string robot_ZShift;
        #endregion

        #region 轴运动偏移量
        /****************************轴运动偏移量********************************/
        /// <summary>
        /// 枪头X方向运动偏移量
        /// </summary>
        public static double Tip_XShift;
        /// <summary>
        /// 枪头Y方向运动偏移量
        /// </summary>
        public static double Tip_YShift;
        /// <summary>
        /// 离心管X方向运动偏移量
        /// </summary>
        public static double LiXinGuan_XShift;
        /// <summary>
        /// 离心管Y方向运动偏移量
        /// </summary>
        public static double LiXinGuan_YShift;
        /// <summary>
        /// 机器人孔盖拍照X方向运动偏移量
        /// </summary>
        public static double KongGai_XShift;
        /// <summary>
        /// 机器人关上样孔盖X方向运动偏移量
        /// </summary>
        public static double CloseCover_XShift;
        /// <summary>
        /// 机器人关上样孔盖Y方向运动偏移量
        /// </summary>
        public static double CloseCover_YShift;
        #endregion

        #region 程序运行标志位
        /******************************程序运行标志位*************************************/
        /// <summary>
        /// 功能块步序
        /// </summary>
        public static int FunctionStep = 0;

        /// <summary>
        /// 搬运工位复位完成标志
        /// </summary>
        public static bool CarryStationResetOK = false;
        /// <summary>
        /// 机器人复位完成标志
        /// </summary>
        public static bool RobotStationResetOK = false;
        /// <summary>
        /// DNA文库地轨送料到位信号
        /// </summary>
        public static bool DNA_Arrive = false;
        /// <summary>
        /// 总控允许实验信号
        /// </summary>
        public static bool experiment_Arrive = false;
        /// <summary>
        /// 机器人工作完成标志（测序工位提前运动）
        /// </summary>
        public static bool RobotWorkDone = false;
        /// <summary>
        /// 搬运模组本次补料完成标志
        /// </summary>
        public static bool feed_Completed = false;
        /// <summary>
        /// 耗材不足标志 [0]:1000枪头;[1]:200枪头;[2]:50枪头;[3]:低温区;[4]:1.5离心管;
        /// </summary>
        public static bool[] consumables_Empty = new bool[5];
        /// <summary>
        /// 1000枪头补料次数
        /// </summary>
        public static int Tip1000 = 0;
        /// <summary>
        /// 给PLC发送补料信号标志
        /// </summary>
        public static bool need_Completed = false;
        /// <summary>
        /// 机台补料完成标志
        /// </summary>
        public static bool BuLiaoCompleted = false;
        /// <summary>
        /// 温控表实时显示失败次数
        /// </summary>
        public static int num = 0;
        /// <summary>
        /// 温控表通讯失败标志
        /// </summary>
        public static bool b_temperature;
        /// <summary>
        /// 给总控发送机台状态标志
        /// </summary>
        public static bool b_StatusToControl;

        /// <summary>
        /// 测序完成标志,数据处理线程工作信号
        /// </summary>
        public static bool CeXu_Completed;
        /// <summary>
        /// 相机拍照判断有无气泡
        /// </summary>
        public static bool CCD_QiPao = false;
        /// <summary>
        /// 搬运工位需要再次排气泡标志
        /// </summary>
        public static bool CarryStation_QiPao = false;
        /// <summary>
        /// 气泡大拍照次数
        /// </summary>
        public static int num_QiPao = 0;
        /// <summary>
        /// 芯片首次机器人替换原装上样孔盖标志
        /// </summary>
        public static bool RobotStation_Replace;
        /// <summary>
        /// 取上样孔孔盖时拍照次数
        /// </summary>
        public static int CCD_KongGaiCount;
        ///// <summary>
        ///// 移液枪Z轴当前位置(单位:um)
        ///// </summary>
        //public static double PipetteZ_CurrentPos = 0;
        /// <summary>
        /// 芯片开始孵育时间(记忆文件)
        /// </summary>
        public static string FuYuStartTimeMemory = "";
        /// <summary>
        /// 芯片开始室温平衡时间(记忆文件)
        /// </summary>
        public static string PingHengStartTimeMemory = "";

        #endregion

        #region 设备运行信息
        /***********************设备运行信息*********************************/
        /// <summary>
        /// 搬运工位当前DNA样本SN
        /// </summary>
        public static string SN_CarryStation;
        /// <summary>
        /// 测序工位当前DNA样本SN
        /// </summary>
        public static string SN_SequencingStation;
        /// <summary>
        /// 数据处理线程当前DNA样本SN
        /// </summary>
        public static string SN_DataProcessingStation;
        /// <summary>
        /// 文件拷贝目录
        /// </summary>
        public static Dictionary<string, string> File_Copy = new Dictionary<string, string>();
        ///// <summary>
        ///// 测序文件数据分析目录
        ///// </summary>
        //public static Dictionary<int, string> Data_Process = new Dictionary<int, string>();
        /// <summary>
        /// 获取每个孔的碱基
        /// </summary>
        public static Dictionary<int, string> JianJiDic = new Dictionary<int, string>();
        /// <summary>
        /// 推测每个孔的碱基
        /// </summary>
        public static Dictionary<int, string> inferJianJiDic = new Dictionary<int, string>();
        /// <summary>
        /// 推测和总控中不同的碱基
        /// </summary>
        public static Dictionary<int, string> differenceJianJiDic = new Dictionary<int, string>();

        /// <summary>
        /// auboSDKip
        /// </summary>
        public static string ipAddressaubo = "";
        /// <summary>
        /// auboSDKport
        /// </summary>
        public static string portaubo = "";

        #endregion

        #region 低温区试剂最大容量
        /********************************低温区试剂最大容量**************************************/
        /// <summary>
        /// FCF试剂最大容量
        /// </summary>
        public static double FCF_MAX;
        /// <summary>
        /// FCT试剂最大容量
        /// </summary>
        public static double FCT_MAX;
        /// <summary>
        /// SB试剂最大容量
        /// </summary>
        public static double SB_MAX;
        /// <summary>
        /// LIB试剂最大容量
        /// </summary>
        public static double LIB_MAX;
        /// <summary>
        /// DIL试剂最大容量
        /// </summary>
        public static double DIL_MAX;
        /// <summary>
        /// WMX试剂最大容量
        /// </summary>
        public static double WMX_MAX;
        /// <summary>
        /// S试剂最大容量
        /// </summary>
        public static double S_MAX;
        #endregion


        #region 测序相关参数
        /********************************测序相关参数**************************************/
        /// <summary>
        /// 当前测序状态码
        /// </summary>
        public static string sequencing_code;
        /// <summary>
        /// 查询测序数据
        /// </summary>
        public static string sequencing_data;
        /// <summary>
        /// 查询测序信息
        /// </summary>
        public static string sequencing_msg;
        /// <summary>
        /// 当前测序状态码
        /// </summary>
        public static string sequencing_total_pore_count;
        /// <summary>
        /// 测序所需数据量（Mb）
        /// </summary>
        public static double sequencingNeedData;
        /// <summary>
        /// 碱基识别开始信号(true:开始)
        /// </summary>
        public static bool JianJiShiBie_Start;

        #endregion

        #region 移液枪运行参数
        /************************************移液枪运行参数*********************************************/
        /// <summary>
        /// 移液枪初始化速度/*= 500*/
        /// </summary>
        public static double gun_Initial_speed;
        /// <summary>
        /// 移液枪Z轴初始化速度/* = 50000*/
        /// </summary>
        public static double z_Initial_speed;

        /// <summary>
        /// 移液枪Z轴下降固定位置/* = 60000*/
        /// </summary>
        public static double z_movepos_down;
        /// <summary>
        /// 移液枪Z轴上升固定位置
        /// </summary>
        public static double z_movepos_up = 0;
        /// <summary>
        /// 移液枪Z轴运动速度/* = 70000*/
        /// </summary>
        public static double z_movepos_speed;

        /// <summary>
        /// 移液枪Z轴液面探测最低位/* = 90000*/
        /// </summary>
        public static double z_check_pos;
        /// <summary>
        /// 移液枪Z轴液面探测速度/* = 20000*/
        /// </summary>
        public static double z_check_speed;
        /// <summary>
        /// 移液枪Z轴1000Tip下降到离心管排液位置/* = 60000*/
        /// </summary>
        public static double z_LiXinGuan1000_pos;
        /// <summary>
        /// 移液枪Z轴200/50Tip下降到离心管排液位置/* = 60000*/
        /// </summary>
        public static double z_LiXinGuan200_pos;
        /// <summary>
        /// 移液枪Z轴FCF低温区吸液位置/* = 80800*/
        /// </summary>
        public static double z_DiWenFCF_pos;
        /// <summary>
        /// 移液枪Z轴FCT低温区吸液位置/* = 110400*/
        /// </summary>
        public static double z_DiWenFCT_pos;
        /// <summary>
        /// 移液枪Z轴SB低温区吸液位置/* = 110400*/
        /// </summary>
        public static double z_DiWenSB_pos;
        /// <summary>
        /// 移液枪Z轴LIB低温区吸液位置/* = 73500*/
        /// </summary>
        public static double z_DiWenLIB_pos;
        /// <summary>
        /// 移液枪Z轴WMX低温区吸液位置/* = 107500*/
        /// </summary>
        public static double z_DiWenWMX_pos;
        /// <summary>
        /// 移液枪Z轴DIL低温区吸液位置/* = 81500*/
        /// </summary>
        public static double z_DiWenDIL_pos;
        /// <summary>
        /// 移液枪Z轴S低温区吸液位置/* = 81500*/
        /// </summary>
        public static double z_DiWenS_pos;
        /// <summary>
        /// 移液枪Z轴50Tip下降到八连排排液位置/*= 60000*/
        /// </summary>
        public static double z_BaLianPai50_pos;
        /// <summary>
        /// 移液枪Z轴下降到上样孔排液位置/* = 95000*/
        /// </summary>
        public static double z_ShangYangKong_pos;
        /// <summary>
        /// 移液枪Z轴下降到预处理孔排液位置/* = 50000*/
        /// </summary>
        public static double z_YuChuLiKong_pos;
        /// <summary>
        /// 移液枪Z轴下降到废液孔排液位置/* = 50000*/
        /// </summary>
        public static double z_FeiYeKong_pos;

        /// <summary>
        /// 移液枪Z轴取枪头速度/* = 20000*/
        /// </summary>
        public static double z_pickTip_speed;
        /// <summary>
        /// 移液枪通用吸液速度/* = 200*/
        /// </summary>
        public static double gun_inliquid_speed;
        /// <summary>
        /// 移液枪通用排液速度/* = 200*/
        /// </summary>
        public static double gun_outliquid_speed;
        /// <summary>
        /// 移液枪快速排液速度/* = 400*/
        /// </summary>
        public static double gun_outliquid_fastspeed;
        /// <summary>
        /// 移液枪逐滴排液速度/* = 100*/
        /// </summary>
        public static double gun_outliquid_slowspeed;
        /// <summary>
        /// 移液枪芯片排液速度/* = 50*/
        /// </summary>
        public static double gun_outliquid_xinpian;
        /// <summary>
        /// 离心管表面积/* = 78*/
        /// </summary>
        public static double surface_LiXinGuan;
        /// <summary>
        /// 低温试剂管表面积/* = 50*/
        /// </summary>
        public static double surface_DiWen;


        /// <summary>
        /// FCF单次吸取体积/* = 58500*/
        /// </summary>
        public static double FCF_volume;
        /// <summary>
        /// FCT吸取体积/* = 3000*/
        /// </summary>
        public static double FCT_volume;
        /// <summary>
        /// 第一次吸取FCF混合液体积/* = 80000*/
        /// </summary>
        public static double FCFmix_volume1;
        /// <summary>
        /// 第二次吸取FCF混合液体积/* = 24000*/
        /// </summary>
        public static double FCFmix_volume2;
        /// <summary>
        /// 吸取LIB试剂体积/* = 2550*/
        /// </summary>
        public static double LIB_volume;
        /// <summary>
        /// 吸取SB试剂体积/*= 3750*/
        /// </summary>
        public static double SB_volume;
        /// <summary>
        /// 吸取DNA试剂体积/* = 1200*/
        /// </summary>
        public static double DNA_volume;
        /// <summary>
        /// 吸取DIL试剂体积/* = 39800*/
        /// </summary>
        public static double DIL_volume;
        /// <summary>
        /// 吸取WMX试剂体积/* = 200*/
        /// </summary>
        public static double WMX_volume;
        /// <summary>
        /// 吸取S试剂体积/* = 50000*/
        /// </summary>
        public static double S_volume;

        /// <summary>
        /// 第一次打入FCF混合液体积/*= 72000*/
        /// </summary>
        public static double FCFmix_volumeOut1;
        /// <summary>
        /// 第二次打入FCF混合液体积/*= 18000*/
        /// </summary>
        public static double FCFmix_volumeOut2;
        /// <summary>
        /// 打入DIL清洗试剂体积/*= 36000*/
        /// </summary>
        public static double DILmix_volumeOut;
        /// <summary>
        /// 打入S保存液试剂体积/*= 45000*/
        /// </summary>
        public static double S_volumeOut;

        /// <summary>
        /// 排气泡时吸取的体积/*= 3000*/
        /// </summary>
        public static double Bubble_Out;
        /// <summary>
        /// 吸取测序实验废液1/*= 90000*/
        /// </summary>
        public static double Waste_Experiment1;
        /// <summary>
        /// 吸取测序实验废液2/*= 30000*/
        /// </summary>
        public static double Waste_Experiment2;
        /// <summary>
        /// 吸取清洗实验废液/* = 36000*/
        /// </summary>
        public static double Waste_Clean;
        /// <summary>
        /// 吸取保存液废液/* = 45000*/
        /// </summary>
        public static double Waste_Save;
        #endregion

        #region 调试界面解析数据使用
        /// <summary>
        /// 所有标签所有碱基集合
        /// </summary>
        public static Dictionary<string, Dictionary<string, int>> AllJianJiDics = new Dictionary<string, Dictionary<string, int>>();
        /// <summary>
        /// 所有标签所有碱基集合(超过5个用others表示)
        /// </summary>
        public static Dictionary<string, Dictionary<string, int>> JianJiDicsMost = new Dictionary<string, Dictionary<string, int>>();
        /// <summary>
        /// 单个标签所有碱基集合
        /// </summary>
        public static Dictionary<string, int> SingleJianJiDics = new Dictionary<string, int>();
        #endregion

        #region 自动流程解析数据界面使用
        /// <summary>
        /// 所有标签所有碱基集合
        /// </summary>
        public static Dictionary<string, Dictionary<string, int>> AutoAllJianJiDics = new Dictionary<string, Dictionary<string, int>>();
        /// <summary>
        /// 所有标签所有碱基集合(超过5个用others表示)
        /// </summary>
        public static Dictionary<string, Dictionary<string, int>> AutoJianJiDicsMost = new Dictionary<string, Dictionary<string, int>>();
        /// <summary>
        /// 单个标签所有碱基集合
        /// </summary>
        public static Dictionary<string, int> AutoSingleJianJiDics = new Dictionary<string, int>();
        /// <summary>
        /// 单个标签碱基集合
        /// </summary>
        public static List<string> AutoJianJiList = new List<string>();
        /// <summary>
        /// 单个标签碱基数量集合
        /// </summary>
        public static List<double> AutoNumList = new List<double>();

        #endregion




        #region 空载具回收模式全局
        /************************空载具回收模式全局****************************/
        /// <summary>
        /// 空载具回收模式标志
        /// </summary>
        public static bool EmptyRun_Run = false;
        /// <summary>
        /// 空载具回收运行结束标志
        /// </summary>
        public static bool EmptyRun_RunDone = false;
        /// <summary>
        /// 重启程序标志
        /// </summary>
        public static bool EmptyRun_Restart = true;
        /// <summary>
        /// 记录空载具队列
        /// </summary>
        public static Queue<MemoryClass.Area> EmptyRun_Qu = new Queue<MemoryClass.Area>(10);
        #endregion

        #region 上，下相机标定模式全局
        /************************上，下相机标定模式全局****************************/
        /// <summary>
        /// 相机标定测序仪工位到位标志
        /// </summary>
        public static bool CalibRun_Run = false;
        /// <summary>
        /// 相机标定结束标志
        /// </summary>
        public static bool CalibRun_RunDone = false;
        #endregion



        #region 单机验证
        /******************单机验证**********************/
        public static bool sign_DNA;

        public static bool sign_zongkong;

        public static bool sign_TIP1;
        public static bool sign_TIP2;
        public static bool sign_TIP3;
        public static bool sign_TIP4;
        public static bool sign_LiXinGuan;
        public static bool sign_DiWen;

        public static bool sign_SequenceFinish;
        public static bool sign_FuYuFinish;

        /// <summary>
        /// 单机做实验碱基序列
        /// </summary>
        public static string SingleExperiment = "";
        #endregion

        #region 参观模式
        /// <summary>
        /// 是否开始参观模式(是:true)
        /// </summary>
        public static bool show_IsOpen;
        /// <summary>
        /// 参观模式循环触发
        /// </summary>
        public static bool show_Repeat;


        /// <summary>
        /// 流转参观模式标志位(是:true)
        /// </summary>
        public static bool newshow_IsOpen;
        /// <summary>
        /// 是否开始流转参观模式(是:true)
        /// </summary>
        public static bool newshow_IsOpenOver = false;

        /// <summary>
        /// 步序1衔接标志
        /// </summary>
        public static bool newshow_step1 = false;


        /// <summary>
        /// 流转参观模式下搬运线程步序记忆
        /// </summary>
        public static int show_memory = 0;

        #endregion

        #region  改机夹爪点位
        public static float speed1_fuwei ;
        public static float acc1_fuwei;
        public static float force1_fuwei;
        public static float pos1_fuwei;
        public static float speed1_daowei;
        public static float acc1_daowei;
        public static float force1_daowei;
        public static float pos1_daowei;
        public static float speed2_fuwei;
        public static float acc2_fuwei;
        public static float force2_fuwei;
        public static float pos2_fuwei;
        public static float speed2_daowei;
        public static float acc2_daowei;
        public static float force2_daowei;
        public static float pos2_daowei;
        #endregion


        /// <summary>
        /// 同步参数信息
        /// </summary>
        /// <returns></returns>
        public static bool ReadPipetteParam()
        {
            try
            {
                gun_Initial_speed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Gun_Initial_Speed.ToString()].CurrentValue);
                z_Initial_speed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_Initial_Speed.ToString()].CurrentValue);

                z_movepos_down = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_Movepos_Down.ToString()].CurrentValue);
                z_movepos_up = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_Movepos_Up.ToString()].CurrentValue);
                z_movepos_speed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_Movepos_Speed.ToString()].CurrentValue);

                z_check_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_Check_Pos.ToString()].CurrentValue);
                z_check_speed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_Check_Speed.ToString()].CurrentValue);
                z_LiXinGuan1000_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_LiXinGuan1000_Pos.ToString()].CurrentValue);
                z_LiXinGuan200_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_LiXinGuan200_Pos.ToString()].CurrentValue);
                z_DiWenFCF_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_DiWenFCF_Pos.ToString()].CurrentValue);
                z_DiWenFCT_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_DiWenFCT_Pos.ToString()].CurrentValue);
                z_DiWenSB_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_DiWenSB_Pos.ToString()].CurrentValue);
                z_DiWenLIB_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_DiWenLIB_Pos.ToString()].CurrentValue);
                z_DiWenWMX_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_DiWenWMX_Pos.ToString()].CurrentValue);
                z_DiWenDIL_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_DiWenDIL_Pos.ToString()].CurrentValue);
                z_DiWenS_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_DiWenS_Pos.ToString()].CurrentValue);
                z_BaLianPai50_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_BaLianPai50_Pos.ToString()].CurrentValue);
                z_ShangYangKong_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_ShangYangKong_Pos.ToString()].CurrentValue);
                z_YuChuLiKong_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_YuChuLiKong_Pos.ToString()].CurrentValue);
                z_FeiYeKong_pos = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_FeiYeKong_Pos.ToString()].CurrentValue);
                z_pickTip_speed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Z_PickTip_Speed.ToString()].CurrentValue);
                gun_inliquid_speed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Gun_Inliquid_Speed.ToString()].CurrentValue);
                gun_outliquid_speed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Gun_Outliquid_Speed.ToString()].CurrentValue);
                gun_outliquid_fastspeed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Gun_Outliquid_Fastspeed.ToString()].CurrentValue);
                gun_outliquid_slowspeed = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Gun_Outliquid_Slowspeed.ToString()].CurrentValue);
                gun_outliquid_xinpian = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Gun_Outliquid_XinPian.ToString()].CurrentValue);
                surface_LiXinGuan = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Surface_LiXinGuan.ToString()].CurrentValue);
                surface_DiWen = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Surface_DiWen.ToString()].CurrentValue);

                FCF_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.FCF_Volume.ToString()].CurrentValue);
                FCT_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.FCT_Volume.ToString()].CurrentValue);
                FCFmix_volume1 = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.FCFmix_Volume1.ToString()].CurrentValue);
                FCFmix_volume2 = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.FCFmix_Volume2.ToString()].CurrentValue);
                LIB_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.LIB_Volume.ToString()].CurrentValue);
                SB_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.SB_Volume.ToString()].CurrentValue);
                DNA_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.DNA_Volume.ToString()].CurrentValue);
                DIL_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.DIL_Volume.ToString()].CurrentValue);
                WMX_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.WMX_Volume.ToString()].CurrentValue);
                S_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.S_Volume.ToString()].CurrentValue);

                FCFmix_volumeOut1 = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.FCFmix_VolumeOut1.ToString()].CurrentValue);
                FCFmix_volumeOut2 = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.FCFmix_VolumeOut2.ToString()].CurrentValue);
                DILmix_volumeOut = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.DILmix_VolumeOut.ToString()].CurrentValue);
                S_volumeOut = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.S_VolumeOut.ToString()].CurrentValue);
                Bubble_Out = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Bubble_Out.ToString()].CurrentValue);

                Waste_Experiment1 = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Waste_Experiment1.ToString()].CurrentValue);
                Waste_Experiment2 = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Waste_Experiment2.ToString()].CurrentValue);
                Waste_Clean = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Waste_Clean.ToString()].CurrentValue);
                Waste_Save = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Waste_Save.ToString()].CurrentValue);
                return true;
            }
            catch (Exception es)
            {
                return false;
            }
        }
        /// <summary>
        /// 同步参数信息(参观模式,用量缩减)
        /// </summary>
        /// <returns></returns>
        public static bool ReadShowVolume()
        {
            try
            {
                FCF_volume = 30000;
                FCT_volume = 1600;
                FCFmix_volume1 = 20000;
                FCFmix_volume2 = 8000;
                LIB_volume = 0;
                SB_volume = 1600;
                DNA_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.DNA_Volume.ToString()].CurrentValue);
                DIL_volume = 17000;
                WMX_volume = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.WMX_Volume.ToString()].CurrentValue);
                S_volume = 20000;

                FCFmix_volumeOut1 = 18000;
                FCFmix_volumeOut2 = 7000;
                DILmix_volumeOut = 15000;
                S_volumeOut = 18000;
                Bubble_Out = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Bubble_Out.ToString()].CurrentValue);

                Waste_Experiment1 = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Waste_Experiment1.ToString()].CurrentValue);
                Waste_Experiment2 = 10000;
                Waste_Clean = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Waste_Clean.ToString()].CurrentValue);
                Waste_Save = Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.Waste_Save.ToString()].CurrentValue);
                return true;
            }
            catch (Exception es)
            {
                return false;
            }
        }

        /// <summary>
        /// 与PLC连接实例化
        /// </summary>
        /// <returns></returns>
        public static bool ModbusTCPInstance()
        {
            string ipAddress = "";
            string port = "";
            string filePath = Application.StartupPath + @"\ExeFile\" + @"\NetParam" + ".xml";
            XDocument doc = XDocument.Load(filePath);
            XElement generalControlClient = doc.Descendants("NetworkClient").FirstOrDefault(e => e.Attribute("网口定义")?.Value == _TcpClientModule.PLC.ToString());
            if (generalControlClient != null)
            {
                ipAddress = generalControlClient.Attribute("IP地址")?.Value; //获取与PLC交互的IP地址
                port = generalControlClient.Attribute("端口号")?.Value;      //获取PLC交互的端口号
            }
            Program.modbusTcp_PLC = new ModbusTcp(ipAddress, int.Parse(port));
            return true;
        }
        /// <summary>
        /// 获取Aubo机器人ip和端口号
        /// </summary>
        /// <returns></returns>
        public static bool AuboSDKInstance()
        {
            string filePathaubo = Application.StartupPath + @"\ExeFile\" + @"\NetParam" + ".xml";
            XDocument docaubo = XDocument.Load(filePathaubo);
            XElement generalControlClientaubo = docaubo.Descendants("NetworkClient").FirstOrDefault(e => e.Attribute("网口定义")?.Value == _TcpClientModule.AuboRobotSDK.ToString());
            if (generalControlClientaubo != null)
            {
                ipAddressaubo = generalControlClientaubo.Attribute("IP地址")?.Value; //获取与PLC交互的IP地址
                portaubo = generalControlClientaubo.Attribute("端口号")?.Value;      //获取PLC交互的端口号
            }
            return true;
        }


        /// <summary>
        /// 连接移液枪
        /// </summary>
        /// <param name="num">com数字长度</param>
        /// <param name="com_1">com第一位</param>
        /// <param name="com_2">com第二位</param>
        /// <param name="rec">返回的结果</param>
        public static void PipetteGunConnect(int num, char com_1, char com_2, out string rec)
        {
            rec = "";
            IntPtr pt;
            UInt16 cont;
            IntPtr ptCfg = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(KpcConfig_t)) * 1);
            KpcConfig_t[] config = new KpcConfig_t[1];
            config[0] = new KpcConfig_t();

            //连接设备配置
            config[0].linkDeviceCont = 1;   //连接设备数量
            config[0].linkDeviceConfig = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(KpcLinkDeviceConfig_t)) * config[0].linkDeviceCont);
            KpcLinkDeviceConfig_t[] linkCfg = new KpcLinkDeviceConfig_t[1];
            linkCfg[0] = new KpcLinkDeviceConfig_t();
            cont = 0;

            linkCfg[0].index = 0;
            linkCfg[0].type = KpcLinkType_e.KPC_USB_SERIANL;
            linkCfg[0].info.serianl.baudRate = 38400;
            linkCfg[0].info.serianl.name = new byte[8];
            linkCfg[0].info.serianl.name[0] = (byte)'C';
            linkCfg[0].info.serianl.name[1] = (byte)'O';
            linkCfg[0].info.serianl.name[2] = (byte)'M';
            if (num == 2)
            {
                linkCfg[0].info.serianl.name[3] = (byte)com_1;
                linkCfg[0].info.serianl.name[4] = (byte)com_2;
                linkCfg[0].info.serianl.name[5] = 0;
            }
            else
            {
                linkCfg[0].info.serianl.name[3] = (byte)com_1;
                linkCfg[0].info.serianl.name[4] = 0;
            }
            pt = (IntPtr)(config[0].linkDeviceConfig + cont * Marshal.SizeOf(typeof(KpcLinkDeviceConfig_t)));
            Marshal.StructureToPtr(linkCfg[0], pt, false);
            cont++;

            //控制设备配置
            config[0].cntDeviceCont = 2;
            config[0].cntDeviceConfig = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(KpcCntDeviceConfig_t)) * config[0].cntDeviceCont);
            KpcCntDeviceConfig_t[] cntCfg = new KpcCntDeviceConfig_t[1];
            cntCfg[0] = new KpcCntDeviceConfig_t();
            cont = 0;

            cntCfg[0].index = (UInt16)(1);
            cntCfg[0].id = (byte)(1);
            cntCfg[0].linkDeviceIndex = 0;
            cntCfg[0].type = KpcDeviceType_e.KPC_ADP16;
            pt = (IntPtr)(config[0].cntDeviceConfig + cont * Marshal.SizeOf(typeof(KpcCntDeviceConfig_t)));
            Marshal.StructureToPtr(cntCfg[0], pt, false);
            cont++;

            cntCfg[0].index = (UInt16)(41);
            cntCfg[0].id = (byte)(41);
            cntCfg[0].linkDeviceIndex = 0;
            cntCfg[0].type = KpcDeviceType_e.KPC_ADPZ;
            pt = (IntPtr)(config[0].cntDeviceConfig + cont * Marshal.SizeOf(typeof(KpcCntDeviceConfig_t)));
            Marshal.StructureToPtr(cntCfg[0], pt, false);
            cont++;
            //控制任务配置
            config[0].cntTaskCont = 1;
            config[0].cntTaskIndex = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UInt16)) * config[0].cntTaskCont);
            UInt16[] taskIndex = new UInt16[1];
            taskIndex[0] = new UInt16();
            taskIndex[0] = 123;
            Marshal.StructureToPtr(taskIndex[0], config[0].cntTaskIndex, false);
            Marshal.StructureToPtr(config[0], ptCfg, false);
            KpcState_e state = ktCntDll.KpcInit(ptCfg);
            rec = state.ToString();
        }


        /// <summary>
        /// 开始实验时总控反馈
        /// </summary>
        /// <param name="msg">接收数据</param>
        /// <param name="code">响应码</param>
        /// <param name="protocol_group_id">实验名称</param>
        /// <param name="product_code">测序芯片类型</param>
        /// <param name="sample_id">样本编号</param>
        /// <param name="kit">试剂盒</param>
        /// <param name="speed">速度</param>
        ///// <param name="experiment_time">实验时间</param>
        /// <param name="min_read_length">最短读长</param>
        /// <param name="guppy_filename">碱基识别模型</param>
        /// <param name="mux_scan_period">孔扫描间隔时间</param>
        public static bool GeneralStartReceive(string msg, out int code, out string protocol_group_id, out string product_code, out string sample_id, out string kit, out int speed, out int min_read_length, out string guppy_filename, out double mux_scan_period)
        {
            try
            {
                code = 0;
                protocol_group_id = "";
                product_code = "";
                sample_id = "";
                kit = "";
                speed = 0;
                //experiment_time = 0;
                min_read_length = 0;
                guppy_filename = "";
                mux_scan_period = 0;
                if (msg != "")   //解析Json格式数据
                {
                    string[] SplitValue = msg.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                    for (int i = 0; i < SplitValue.Length; i++)
                    {
                        string[] KeyValue = SplitValue[i].Split(':');
                        if (KeyValue[0].Trim() == "code")
                        {
                            code = Convert.ToInt32(KeyValue[1].Trim());
                        }
                        if (SplitValue[i].Contains("protocol_group_id"))
                        {
                            protocol_group_id = KeyValue[1].Trim();
                        }
                        if (SplitValue[i].Contains("product_code"))
                        {
                            product_code = KeyValue[1].Trim();
                        }
                        if (SplitValue[i].Contains("sample_id"))
                        {
                            sample_id = KeyValue[1].Trim();
                        }
                        if (SplitValue[i].Contains("kit"))
                        {
                            kit = KeyValue[1].Trim();
                        }
                        if (SplitValue[i].Contains("speed"))
                        {
                            speed = Convert.ToInt32(KeyValue[1].Trim());
                        }
                        //if (SplitValue[i].Contains("experiment_time"))
                        //{
                        //    experiment_time = Convert.ToInt32(KeyValue[1].Trim());
                        //}
                        if (SplitValue[i].Contains("min_read_length"))
                        {
                            min_read_length = Convert.ToInt32(KeyValue[1].Trim());
                        }
                        if (SplitValue[i].Contains("guppy_filename"))
                        {
                            guppy_filename = KeyValue[1].Trim();
                        }
                        if (SplitValue[i].Contains("mux_scan_period"))
                        {
                            mux_scan_period = Convert.ToDouble(KeyValue[1].Trim());
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                code = 0;
                protocol_group_id = "";
                product_code = "";
                sample_id = "";
                kit = "";
                speed = 0;
                //experiment_time = 0;
                min_read_length = 0;
                guppy_filename = "";
                mux_scan_period = 0;
                return false;
            }
        }

        /// <summary>
        /// 测序结束时总控反馈
        /// </summary>
        /// <param name="msg">接收信息</param>
        /// <param name="code">响应码</param>
        /// <param name="idna">IDNA引物</param>
        /// <param name="taskid">任务ID，用于上报测序结果</param>
        /// <param name="experimentName">实验名称，用于查找测序文件</param>
        /// <param name="JianJiofALL">实验对应孔的碱基</param>
        /// <returns></returns>
        public static bool GeneralCompleteReceive(string msg, out int code, out string idna, out int taskid, out string experimentName, out string JianJiofALL)
        {
            try
            {
                code = 0;
                idna = "";
                taskid = 0;
                experimentName = "";
                JianJiofALL = "";
                if (msg != "")   //解析Json格式数据
                {
                    string[] SplitValue = msg.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                    for (int i = 0; i < SplitValue.Length; i++)
                    {
                        string[] KeyValue = SplitValue[i].Split(':');
                        if (SplitValue[i].Contains("code"))
                        {
                            code = Convert.ToInt32(KeyValue[1].Trim());
                        }
                        if (SplitValue[i].Contains("idna"))
                        {
                            idna = KeyValue[1].Trim();
                        }
                        if (SplitValue[i].Contains("taskId"))
                        {
                            taskid = Convert.ToInt32(KeyValue[1].Trim());
                        }
                        if (SplitValue[i].Contains("protocol_group_id"))
                        {
                            experimentName = KeyValue[1].Trim();
                        }
                        if (SplitValue[i].Contains("base_sequence"))
                        {
                            JianJiofALL = KeyValue[1].Trim();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                code = 0;
                idna = "";
                taskid = 0;
                experimentName = "";
                JianJiofALL = "";
                return false;
            }
        }

        /// <summary>
        /// 测序结果上传总控反馈
        /// </summary>
        /// <param name="msg">接收信息</param>
        /// <param name="code">响应码</param>
        /// <returns></returns>
        public static bool GeneralResultReceive(string msg, out int code)
        {
            try
            {
                code = 0;
                if (msg != "")   //解析Json格式数据
                {
                    string[] SplitValue = msg.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                    for (int i = 0; i < SplitValue.Length; i++)
                    {
                        string[] KeyValue = SplitValue[i].Split(':');
                        if (SplitValue[i].Contains("code"))
                        {
                            code = Convert.ToInt32(KeyValue[1].Trim());
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                code = 0;
                return false;
            }
        }
        /// <summary>
        /// 查询后续任务总控反馈
        /// </summary>
        /// <param name="msg">接收信息</param>
        /// <param name="code">响应码</param>
        /// <param name="data">是否有后续任务；1是，0否</param>
        /// <returns></returns>
        public static bool GeneralSearchFolloUpTaskReceive(string msg, out int code, out int data)
        {
            try
            {
                code = 0;
                data = 0;
                if (msg != "")   //解析Json格式数据
                {
                    string[] SplitValue = msg.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                    for (int i = 0; i < SplitValue.Length; i++)
                    {
                        string[] KeyValue = SplitValue[i].Split(':');
                        if (SplitValue[i].Contains("code"))
                        {
                            code = Convert.ToInt32(KeyValue[1].Trim());
                        }
                        if (SplitValue[i].Contains("data"))
                        {
                            data = Convert.ToInt32(KeyValue[1].Trim());
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                code = 0;
                data = 0;
                return false;
            }
        }
        public static bool ToGeneralStatus(int statusID)
        {
            try
            {
                string str2 = @"\\" + ParameConfig.Instance.SystemParameDic[_ParamName.GeneralShareIP.ToString()].CurrentValue + @"\Cexu\Status";
                if (Directory.Exists(str2))
                {
                    DeleteFilesInDirectory(str2);
                    string filePath = Path.Combine(str2, statusID + ".txt");
                    File.Create(filePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception d)
            {
                return false;
            }
        }
        /// <summary>
        /// 删除目录下所有文件
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        public static void DeleteFilesInDirectory(string directoryPath)
        {
            // 确保目录存在
            if (Directory.Exists(directoryPath))
            {
                string[] filePaths = Directory.GetFiles(directoryPath);
                // 删除所有文件
                foreach (string filePath in filePaths)
                {
                    File.Delete(filePath);
                }
            }
        }

    }
}
