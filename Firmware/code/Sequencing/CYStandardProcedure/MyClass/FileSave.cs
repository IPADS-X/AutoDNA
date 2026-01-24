using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    class FileSave
    {
        /// <summary>
        /// 读取机台区域信息
        /// </summary>
        public static bool ReadRobotClawMsg()
        {
            try
            {
                INIFile ini = new INIFile(Application.StartupPath + "\\ExeFile\\RobotClaws\\RobotClawPos.ini");

                MyVariable.speed1_fuwei = ini.Read<float>("Claw1_1", "speed");
                MyVariable.acc1_fuwei = ini.Read<float>("Claw1_1", "acc");
                MyVariable.force1_fuwei = ini.Read<float>("Claw1_1", "force");
                MyVariable.pos1_fuwei = ini.Read<float>("Claw1_1", "pos");

                MyVariable.speed1_daowei = ini.Read<float>("Claw1_2", "speed");
                MyVariable.acc1_daowei = ini.Read<float>("Claw1_2", "acc");
                MyVariable.force1_daowei = ini.Read<float>("Claw1_2", "force");
                MyVariable.pos1_daowei = ini.Read<float>("Claw1_2", "pos");

                MyVariable.speed2_fuwei = ini.Read<float>("Claw2_1", "speed");
                MyVariable.acc2_fuwei = ini.Read<float>("Claw2_1", "acc");
                MyVariable.force2_fuwei = ini.Read<float>("Claw2_1", "force");
                MyVariable.pos2_fuwei = ini.Read<float>("Claw2_1", "pos");

                MyVariable.speed2_daowei = ini.Read<float>("Claw2_2", "speed");
                MyVariable.acc2_daowei = ini.Read<float>("Claw2_2", "acc");
                MyVariable.force2_daowei = ini.Read<float>("Claw2_2", "force");
                MyVariable.pos2_daowei = ini.Read<float>("Claw2_2", "pos");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 存储机台区域信息
        /// </summary>
        public static bool WriteAreaMsg()
        {
            try
            {
                INIFile ini = new INIFile(Application.StartupPath + "\\FileINI\\AreaRecord.ini");
                ini.Write("QiangTou1", "Remain", MyVariable.area_QiangTou1.num_Remain);
                ini.Write("QiangTou1", "X", MyVariable.area_QiangTou1.num_X);
                ini.Write("QiangTou1", "XMax", MyVariable.area_QiangTou1.num_XMax);
                ini.Write("QiangTou1", "Y", MyVariable.area_QiangTou1.num_Y);
                ini.Write("QiangTou1", "YMax", MyVariable.area_QiangTou1.num_YMax);
                ini.Write("QiangTou1", "RemainBool", MyVariable.consumables_Empty[0]);

                ini.Write("QiangTou2", "Remain", MyVariable.area_QiangTou2.num_Remain);
                ini.Write("QiangTou2", "X", MyVariable.area_QiangTou2.num_X);
                ini.Write("QiangTou2", "XMax", MyVariable.area_QiangTou2.num_XMax);
                ini.Write("QiangTou2", "Y", MyVariable.area_QiangTou2.num_Y);
                ini.Write("QiangTou2", "YMax", MyVariable.area_QiangTou2.num_YMax);
                ini.Write("QiangTou2", "RemainBool", MyVariable.consumables_Empty[0]);

                ini.Write("QiangTou3", "Remain", MyVariable.area_QiangTou3.num_Remain);
                ini.Write("QiangTou3", "X", MyVariable.area_QiangTou3.num_X);
                ini.Write("QiangTou3", "XMax", MyVariable.area_QiangTou3.num_XMax);
                ini.Write("QiangTou3", "Y", MyVariable.area_QiangTou3.num_Y);
                ini.Write("QiangTou3", "YMax", MyVariable.area_QiangTou3.num_YMax);
                ini.Write("QiangTou3", "RemainBool", MyVariable.consumables_Empty[1]);

                ini.Write("QiangTou4", "Remain", MyVariable.area_QiangTou4.num_Remain);
                ini.Write("QiangTou4", "X", MyVariable.area_QiangTou4.num_X);
                ini.Write("QiangTou4", "XMax", MyVariable.area_QiangTou4.num_XMax);
                ini.Write("QiangTou4", "Y", MyVariable.area_QiangTou4.num_Y);
                ini.Write("QiangTou4", "YMax", MyVariable.area_QiangTou4.num_YMax);
                ini.Write("QiangTou4", "RemainBool", MyVariable.consumables_Empty[2]);

                ini.Write("DiWenFCT", "Remain", MyVariable.area_DiWen_FCT.num_Remain);
                ini.Write("DiWenFCT", "X", MyVariable.area_DiWen_FCT.num_X);
                ini.Write("DiWenFCT", "XMax", MyVariable.area_DiWen_FCT.num_XMax);
                ini.Write("DiWenFCT", "Y", MyVariable.area_DiWen_FCT.num_Y);
                ini.Write("DiWenFCT", "YMax", MyVariable.area_DiWen_FCT.num_YMax);
                ini.Write("DiWenFCT", "RemainBool", MyVariable.consumables_Empty[3]);

                ini.Write("DiWenFCF", "Remain", MyVariable.area_DiWen_FCF.num_Remain);
                ini.Write("DiWenFCF", "X", MyVariable.area_DiWen_FCF.num_X);
                ini.Write("DiWenFCF", "XMax", MyVariable.area_DiWen_FCF.num_XMax);
                ini.Write("DiWenFCF", "Y", MyVariable.area_DiWen_FCF.num_Y);
                ini.Write("DiWenFCF", "YMax", MyVariable.area_DiWen_FCF.num_YMax);
                ini.Write("DiWenFCF", "RemainBool", MyVariable.consumables_Empty[3]);

                ini.Write("DiWenSB", "Remain", MyVariable.area_DiWen_SB.num_Remain);
                ini.Write("DiWenSB", "X", MyVariable.area_DiWen_SB.num_X);
                ini.Write("DiWenSB", "XMax", MyVariable.area_DiWen_SB.num_XMax);
                ini.Write("DiWenSB", "Y", MyVariable.area_DiWen_SB.num_Y);
                ini.Write("DiWenSB", "YMax", MyVariable.area_DiWen_SB.num_YMax);
                ini.Write("DiWenSB", "RemainBool", MyVariable.consumables_Empty[3]);

                ini.Write("DiWenLIB", "Remain", MyVariable.area_DiWen_LIB.num_Remain);
                ini.Write("DiWenLIB", "X", MyVariable.area_DiWen_LIB.num_X);
                ini.Write("DiWenLIB", "XMax", MyVariable.area_DiWen_LIB.num_XMax);
                ini.Write("DiWenLIB", "Y", MyVariable.area_DiWen_LIB.num_Y);
                ini.Write("DiWenLIB", "YMax", MyVariable.area_DiWen_LIB.num_YMax);
                ini.Write("DiWenLIB", "RemainBool", MyVariable.consumables_Empty[3]);

                ini.Write("DiWenDIL", "Remain", MyVariable.area_DiWen_DIL.num_Remain);
                ini.Write("DiWenDIL", "X", MyVariable.area_DiWen_DIL.num_X);
                ini.Write("DiWenDIL", "XMax", MyVariable.area_DiWen_DIL.num_XMax);
                ini.Write("DiWenDIL", "Y", MyVariable.area_DiWen_DIL.num_Y);
                ini.Write("DiWenDIL", "YMax", MyVariable.area_DiWen_DIL.num_YMax);
                ini.Write("DiWenDIL", "RemainBool", MyVariable.consumables_Empty[3]);

                ini.Write("DiWenWMX", "Remain", MyVariable.area_DiWen_WMX.num_Remain);
                ini.Write("DiWenWMX", "X", MyVariable.area_DiWen_WMX.num_X);
                ini.Write("DiWenWMX", "XMax", MyVariable.area_DiWen_WMX.num_XMax);
                ini.Write("DiWenWMX", "Y", MyVariable.area_DiWen_WMX.num_Y);
                ini.Write("DiWenWMX", "YMax", MyVariable.area_DiWen_WMX.num_YMax);
                ini.Write("DiWenWMX", "RemainBool", MyVariable.consumables_Empty[3]);

                ini.Write("DiWenS", "Remain", MyVariable.area_DiWen_S.num_Remain);
                ini.Write("DiWenS", "X", MyVariable.area_DiWen_S.num_X);
                ini.Write("DiWenS", "XMax", MyVariable.area_DiWen_S.num_XMax);
                ini.Write("DiWenS", "Y", MyVariable.area_DiWen_S.num_Y);
                ini.Write("DiWenS", "YMax", MyVariable.area_DiWen_S.num_YMax);
                ini.Write("DiWenS", "RemainBool", MyVariable.consumables_Empty[3]);

                ini.Write("LiXinGuan", "Remain", MyVariable.area_LiXinGuan.num_Remain);
                ini.Write("LiXinGuan", "X", MyVariable.area_LiXinGuan.num_X);
                ini.Write("LiXinGuan", "XMax", MyVariable.area_LiXinGuan.num_XMax);
                ini.Write("LiXinGuan", "Y", MyVariable.area_LiXinGuan.num_Y);
                ini.Write("LiXinGuan", "YMax", MyVariable.area_LiXinGuan.num_YMax);
                ini.Write("LiXinGuan", "RemainBool", MyVariable.consumables_Empty[4]);

                ini.Write("8LianPai", "Remain", MyVariable.area_8LianPai.num_Remain);
                ini.Write("8LianPai", "X", MyVariable.area_8LianPai.num_X);
                ini.Write("8LianPai", "XMax", MyVariable.area_8LianPai.num_XMax);
                ini.Write("8LianPai", "Y", MyVariable.area_8LianPai.num_Y);
                ini.Write("8LianPai", "YMax", MyVariable.area_8LianPai.num_YMax);


                //运动偏移量
                ini.Write("TipXShift", "tipXshift", MyVariable.Tip_XShift);
                ini.Write("TipYShift", "tipYshift", MyVariable.Tip_YShift);
                ini.Write("LiXinGuanXShift", "lixinguanXshift", MyVariable.LiXinGuan_XShift);
                ini.Write("LiXinGuanYShift", "lixinguanYshift", MyVariable.LiXinGuan_YShift);
                ini.Write("KongGaiXShift", "konggaiXshift", MyVariable.KongGai_XShift);



                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 读取机台区域信息
        /// </summary>
        public static bool ReadAreaMsg()
        {
            try
            {
                INIFile ini = new INIFile(Application.StartupPath + "\\FileINI\\AreaRecord.ini");

                MyVariable.area_QiangTou1.name = MemoryClass.Area.枪头区1.ToString();
                MyVariable.area_QiangTou1.num_Remain = ini.Read<double>("QiangTou1", "Remain");
                MyVariable.area_QiangTou1.num_X = ini.Read<int>("QiangTou1", "X");
                MyVariable.area_QiangTou1.num_XMax = ini.Read<int>("QiangTou1", "XMax");
                MyVariable.area_QiangTou1.num_Y = ini.Read<int>("QiangTou1", "Y");
                MyVariable.area_QiangTou1.num_YMax = ini.Read<int>("QiangTou1", "YMax");

                MyVariable.area_QiangTou2.name = MemoryClass.Area.枪头区2.ToString();
                MyVariable.area_QiangTou2.num_Remain = ini.Read<double>("QiangTou2", "Remain");
                MyVariable.area_QiangTou2.num_X = ini.Read<int>("QiangTou2", "X");
                MyVariable.area_QiangTou2.num_XMax = ini.Read<int>("QiangTou2", "XMax");
                MyVariable.area_QiangTou2.num_Y = ini.Read<int>("QiangTou2", "Y");
                MyVariable.area_QiangTou2.num_YMax = ini.Read<int>("QiangTou2", "YMax");
                MyVariable.consumables_Empty[0] = ini.Read<bool>("QiangTou2", "RemainBool");

                MyVariable.area_QiangTou3.name = MemoryClass.Area.枪头区3.ToString();
                MyVariable.area_QiangTou3.num_Remain = ini.Read<double>("QiangTou3", "Remain");
                MyVariable.area_QiangTou3.num_X = ini.Read<int>("QiangTou3", "X");
                MyVariable.area_QiangTou3.num_XMax = ini.Read<int>("QiangTou3", "XMax");
                MyVariable.area_QiangTou3.num_Y = ini.Read<int>("QiangTou3", "Y");
                MyVariable.area_QiangTou3.num_YMax = ini.Read<int>("QiangTou3", "YMax");
                MyVariable.consumables_Empty[1] = ini.Read<bool>("QiangTou3", "RemainBool");

                MyVariable.area_QiangTou4.name = MemoryClass.Area.枪头区4.ToString();
                MyVariable.area_QiangTou4.num_Remain = ini.Read<double>("QiangTou4", "Remain");
                MyVariable.area_QiangTou4.num_X = ini.Read<int>("QiangTou4", "X");
                MyVariable.area_QiangTou4.num_XMax = ini.Read<int>("QiangTou4", "XMax");
                MyVariable.area_QiangTou4.num_Y = ini.Read<int>("QiangTou4", "Y");
                MyVariable.area_QiangTou4.num_YMax = ini.Read<int>("QiangTou4", "YMax");
                MyVariable.consumables_Empty[2] = ini.Read<bool>("QiangTou4", "RemainBool");

                MyVariable.area_DiWen_FCT.num_Remain = ini.Read<double>("DiWenFCT", "Remain");
                MyVariable.area_DiWen_FCT.num_X = ini.Read<int>("DiWenFCT", "X");
                MyVariable.area_DiWen_FCT.num_XMax = ini.Read<int>("DiWenFCT", "XMax");
                MyVariable.area_DiWen_FCT.num_Y = ini.Read<int>("DiWenFCT", "Y");
                MyVariable.area_DiWen_FCT.num_YMax = ini.Read<int>("DiWenFCT", "YMax");
                MyVariable.consumables_Empty[3] = ini.Read<bool>("DiWenFCT", "RemainBool");

                MyVariable.area_DiWen_FCF.num_Remain = ini.Read<double>("DiWenFCF", "Remain");
                MyVariable.area_DiWen_FCF.num_X = ini.Read<int>("DiWenFCF", "X");
                MyVariable.area_DiWen_FCF.num_XMax = ini.Read<int>("DiWenFCF", "XMax");
                MyVariable.area_DiWen_FCF.num_Y = ini.Read<int>("DiWenFCF", "Y");
                MyVariable.area_DiWen_FCF.num_YMax = ini.Read<int>("DiWenFCF", "YMax");

                MyVariable.area_DiWen_SB.num_Remain = ini.Read<double>("DiWenSB", "Remain");
                MyVariable.area_DiWen_SB.num_X = ini.Read<int>("DiWenSB", "X");
                MyVariable.area_DiWen_SB.num_XMax = ini.Read<int>("DiWenSB", "XMax");
                MyVariable.area_DiWen_SB.num_Y = ini.Read<int>("DiWenSB", "Y");
                MyVariable.area_DiWen_SB.num_YMax = ini.Read<int>("DiWenSB", "YMax");

                MyVariable.area_DiWen_LIB.num_Remain = ini.Read<double>("DiWenLIB", "Remain");
                MyVariable.area_DiWen_LIB.num_X = ini.Read<int>("DiWenLIB", "X");
                MyVariable.area_DiWen_LIB.num_XMax = ini.Read<int>("DiWenLIB", "XMax");
                MyVariable.area_DiWen_LIB.num_Y = ini.Read<int>("DiWenLIB", "Y");
                MyVariable.area_DiWen_LIB.num_YMax = ini.Read<int>("DiWenLIB", "YMax");

                MyVariable.area_DiWen_DIL.num_Remain = ini.Read<double>("DiWenDIL", "Remain");
                MyVariable.area_DiWen_DIL.num_X = ini.Read<int>("DiWenDIL", "X");
                MyVariable.area_DiWen_DIL.num_XMax = ini.Read<int>("DiWenDIL", "XMax");
                MyVariable.area_DiWen_DIL.num_Y = ini.Read<int>("DiWenDIL", "Y");
                MyVariable.area_DiWen_DIL.num_YMax = ini.Read<int>("DiWenDIL", "YMax");

                MyVariable.area_DiWen_WMX.num_Remain = ini.Read<double>("DiWenWMX", "Remain");
                MyVariable.area_DiWen_WMX.num_X = ini.Read<int>("DiWenWMX", "X");
                MyVariable.area_DiWen_WMX.num_XMax = ini.Read<int>("DiWenWMX", "XMax");
                MyVariable.area_DiWen_WMX.num_Y = ini.Read<int>("DiWenWMX", "Y");
                MyVariable.area_DiWen_WMX.num_YMax = ini.Read<int>("DiWenWMX", "YMax");

                MyVariable.area_DiWen_S.num_Remain = ini.Read<double>("DiWenS", "Remain");
                MyVariable.area_DiWen_S.num_X = ini.Read<int>("DiWenS", "X");
                MyVariable.area_DiWen_S.num_XMax = ini.Read<int>("DiWenS", "XMax");
                MyVariable.area_DiWen_S.num_Y = ini.Read<int>("DiWenS", "Y");
                MyVariable.area_DiWen_S.num_YMax = ini.Read<int>("DiWenS", "YMax");

                MyVariable.area_LiXinGuan.name = MemoryClass.Area.离心管试管区.ToString();
                MyVariable.area_LiXinGuan.num_Remain = ini.Read<double>("LiXinGuan", "Remain");
                MyVariable.area_LiXinGuan.num_X = ini.Read<int>("LiXinGuan", "X");
                MyVariable.area_LiXinGuan.num_XMax = ini.Read<int>("LiXinGuan", "XMax");
                MyVariable.area_LiXinGuan.num_Y = ini.Read<int>("LiXinGuan", "Y");
                MyVariable.area_LiXinGuan.num_YMax = ini.Read<int>("LiXinGuan", "YMax");
                MyVariable.consumables_Empty[4] = ini.Read<bool>("LiXinGuan", "RemainBool");

                MyVariable.area_8LianPai.name = MemoryClass.Area.八联排试管区.ToString();
                MyVariable.area_8LianPai.num_Remain = ini.Read<double>("8LianPai", "Remain");
                MyVariable.area_8LianPai.num_X = ini.Read<int>("8LianPai", "X");
                MyVariable.area_8LianPai.num_XMax = ini.Read<int>("8LianPai", "XMax");
                MyVariable.area_8LianPai.num_Y = ini.Read<int>("8LianPai", "Y");
                MyVariable.area_8LianPai.num_YMax = ini.Read<int>("8LianPai", "YMax");

                //读取点位运动偏移量
                MyVariable.Tip_XShift = ini.Read<double>("TipXShift", "tipXshift");
                MyVariable.Tip_YShift = ini.Read<double>("TipYShift", "tipYshift");
                MyVariable.LiXinGuan_XShift = ini.Read<double>("LiXinGuanXShift", "lixinguanXshift");
                MyVariable.LiXinGuan_YShift = ini.Read<double>("LiXinGuanYShift", "lixinguanYshift");
                MyVariable.KongGai_XShift = ini.Read<double>("KongGaiXShift", "konggaiXshift");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 存储机台bool标志信息
        /// </summary>
        /// <returns></returns>
        public static bool WriteBoolMsg()
        {
            try
            {
                INIFile Boolini = new INIFile(Application.StartupPath + "\\FileINI\\BoolRecord.ini");
                Boolini.Write("Experiment", "Allow", MyVariable.experiment_Arrive);
                Boolini.Write("NeedCompleted", "needCompleted", MyVariable.need_Completed);
                Boolini.Write("Tip1000", "tip1000", MyVariable.Tip1000);
                Boolini.Write("CeXuCompleted", "cexucompleted", MyVariable.CeXu_Completed);
                Boolini.Write("CCD_QiPao", "ccd_qipao", MyVariable.CCD_QiPao);
                Boolini.Write("CarryStation_QiPao", "carrystation_qipao", MyVariable.CarryStation_QiPao);
                Boolini.Write("Num_QiPao", "num_qipao", MyVariable.num_QiPao);
                Boolini.Write("Num_KongGai", "num_konggai", MyVariable.CCD_KongGaiCount);
                Boolini.Write("RobotStation_Replace", "robotStationReplace", MyVariable.RobotStation_Replace);
                Boolini.Write("SN", "SN_CarryStation", MyVariable.SN_CarryStation);
                Boolini.Write("SN", "SN_SequencingStation", MyVariable.SN_SequencingStation);
                Boolini.Write("SN", "SN_DataProcessingStation", MyVariable.SN_DataProcessingStation);
                Boolini.Write("CloseCover", "XShift", MyVariable.CloseCover_XShift);
                Boolini.Write("CloseCover", "YShift", MyVariable.CloseCover_YShift);
                Boolini.Write("Sequencing", "sequencingNeedData", MyVariable.sequencingNeedData);
                Boolini.Write("PingHengTimeMemory", "PingHengStartTimeMemory", MyVariable.PingHengStartTimeMemory);
                Boolini.Write("FuYuTimeMemory", "FuYuStartTimeMemory", MyVariable.FuYuStartTimeMemory);
                Boolini.Write("StatusToControl", "StatusSign", MyVariable.b_StatusToControl);
                Boolini.Write("JianJiShiBie", "Start", MyVariable.JianJiShiBie_Start);
                Boolini.Write("BaseMsgs", "msg", SerializeClass.animationParam.BaseMsg);
                Boolini.Write("RemainTimes", "time", SerializeClass.animationParam.RemainTime);
                Boolini.Write("Result", "result", SerializeClass.animationParam.Result);
                if (MyVariable.EmptyRun_Qu.Count != 0)
                {
                    Boolini.Write("EmptyRunArea", "emptyrunArea", MyVariable.EmptyRun_Qu.Peek());
                }
                else
                {
                    Boolini.Write("EmptyRunArea", "emptyrunArea", MemoryClass.Area.进料区);
                }
                return true;
            }
            catch (Exception es)
            {
                return false;
            }
        }
        /// <summary>
        /// 读取机台bool标志信息
        /// </summary>
        /// <returns></returns>
        public static bool ReadBoolMsg()
        {
            try
            {
                INIFile Boolini = new INIFile(Application.StartupPath + "\\FileINI\\BoolRecord.ini");
                MyVariable.experiment_Arrive = Boolini.Read<bool>("Experiment", "Allow");
                MyVariable.need_Completed = Boolini.Read<bool>("NeedCompleted", "needCompleted");
                MyVariable.Tip1000 = Boolini.Read<int>("Tip1000", "tip1000");
                MyVariable.CeXu_Completed = Boolini.Read<bool>("CeXuCompleted", "cexucompleted");
                MyVariable.CCD_QiPao = Boolini.Read<bool>("CCD_QiPao", "ccd_qipao");
                MyVariable.CarryStation_QiPao = Boolini.Read<bool>("CarryStation_QiPao", "carrystation_qipao");
                MyVariable.num_QiPao = Boolini.Read<int>("Num_QiPao", "num_qipao");
                MyVariable.CCD_KongGaiCount = Boolini.Read<int>("Num_KongGai", "num_konggai");
                MyVariable.RobotStation_Replace = Boolini.Read<bool>("RobotStation_Replace", "robotStationReplace");
                MyVariable.SN_CarryStation = Boolini.Read<string>("SN", "SN_CarryStation");
                MyVariable.SN_SequencingStation = Boolini.Read<string>("SN", "SN_SequencingStation");
                MyVariable.SN_DataProcessingStation = Boolini.Read<string>("SN", "SN_DataProcessingStation");
                MyVariable.CloseCover_XShift = Boolini.Read<double>("CloseCover", "XShift");
                MyVariable.CloseCover_YShift = Boolini.Read<double>("CloseCover", "YShift");
                MyVariable.sequencingNeedData = Boolini.Read<double>("Sequencing", "sequencingNeedData");
                MyVariable.PingHengStartTimeMemory = Boolini.Read<string>("PingHengTimeMemory", "PingHengStartTimeMemory");
                MyVariable.FuYuStartTimeMemory = Boolini.Read<string>("FuYuTimeMemory", "FuYuStartTimeMemory");
                MyVariable.b_StatusToControl = Boolini.Read<bool>("StatusToControl", "StatusSign");
                MyVariable.JianJiShiBie_Start = Boolini.Read<bool>("JianJiShiBie", "Start");
                SerializeClass.animationParam.BaseMsg= Boolini.Read<string>("BaseMsgs", "msg");
                SerializeClass.animationParam.RemainTime = Boolini.Read<double>("RemainTimes", "time");
                SerializeClass.animationParam.Result = Boolini.Read<string>("Result", "result");
                if (Boolini.Read<MemoryClass.Area>("EmptyRunArea", "emptyrunArea") != MemoryClass.Area.进料区)
                {
                    MyVariable.EmptyRun_Qu.Enqueue(Boolini.Read<MemoryClass.Area>("EmptyRunArea", "emptyrunArea"));
                }
                INIFile JianJiini = new INIFile(Application.StartupPath + "\\FileINI\\SequenceForm.ini");
                MyVariable.SingleExperiment = JianJiini.Read<string>("SingleExperiment", "Barcode");
                return true;
            }
            catch (Exception es)
            {
                return false;
            }
        }
        /// <summary>
        /// 存储低温试剂最大存储量信息
        /// </summary>
        /// <returns></returns>
        public static bool WriteVolumeMax()
        {
            try
            {
                INIFile Volumeini = new INIFile(Application.StartupPath + "\\ExeFile\\ColdVolumeMax.ini");
                Volumeini.Write("VolumeMax", "FCFMax", MyVariable.FCF_MAX);
                Volumeini.Write("VolumeMax", "FCTMax", MyVariable.FCT_MAX);
                Volumeini.Write("VolumeMax", "SBMax", MyVariable.SB_MAX);
                Volumeini.Write("VolumeMax", "LIBMax", MyVariable.LIB_MAX);
                Volumeini.Write("VolumeMax", "DILMax", MyVariable.DIL_MAX);
                Volumeini.Write("VolumeMax", "WMXMax", MyVariable.WMX_MAX);
                Volumeini.Write("VolumeMax", "SMax", MyVariable.S_MAX);
                return true;
            }
            catch (Exception es)
            {
                return false;
            }
        }
        /// <summary>
        /// 读取低温试剂最大存储量信息
        /// </summary>
        /// <returns></returns>
        public static bool ReadVolumeMax()
        {
            try
            {
                INIFile Volumeini = new INIFile(Application.StartupPath + "\\ExeFile\\ColdVolumeMax.ini");
                MyVariable.FCF_MAX = Volumeini.Read<double>("VolumeMax", "FCFMax");
                MyVariable.FCT_MAX = Volumeini.Read<double>("VolumeMax", "FCTMax");
                MyVariable.SB_MAX = Volumeini.Read<double>("VolumeMax", "SBMax");
                MyVariable.LIB_MAX = Volumeini.Read<double>("VolumeMax", "LIBMax");
                MyVariable.DIL_MAX = Volumeini.Read<double>("VolumeMax", "DILMax");
                MyVariable.WMX_MAX = Volumeini.Read<double>("VolumeMax", "WMXMax");
                MyVariable.S_MAX = Volumeini.Read<double>("VolumeMax", "SMax");
                return true;
            }
            catch (Exception es)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除指定文件夹下天数之前的文件
        /// </summary>
        /// <param name="folderPath">文件路径</param>
        /// <param name="days">天数</param>
        public static void DeleteFilesOlderThanDays(string folderPath, int days)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    if (file.LastWriteTime < DateTime.Now.AddDays(-days))
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
