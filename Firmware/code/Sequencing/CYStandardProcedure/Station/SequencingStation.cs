using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using CYAutoFramework;
using System.Diagnostics;
using Newtonsoft.Json;
using CYStandardProcedure.WebReference;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using MsgBoxLib;

namespace CYStandardProcedure
{
    public class SequencingStation : ObjectStation
    {
        INIFile ini = new INIFile(Application.StartupPath + "\\FileINI\\SequenceForm.ini");
        private string mName;
        private _ActionResult resetRet;//单步复位结果
        private _ActionResult runRet;//单步运行结果
        private bool b_SequencingStation;//测序线程通用标志位
        Stopwatch CeXuDelayTime = new Stopwatch();
        Stopwatch CheckTime = new Stopwatch();
        private int cexuMin;
        private string cexuCode;//测序仪接口反馈响应码
        private string cexuCom22;//测序仪端口22
        private string cexuCom9502;//测序仪端口9502
        private string cexuMsg;
        private string cexuState;
        private long time;//延时时间记录
        private int code_general;//总控反馈响应码
        private int AllJianJiLength;//所有孔碱基的长度和
        DateTime lastTime = new DateTime();
        DateTime currentTime = new DateTime();
        TimeSpan timeDifference;
        bool waitStart = false;
        public SequencingStation(string name) :
            base(name)
        {
            this.mName = name;
        }

        /// <summary>
        /// 单站复位动作
        /// </summary>
        public override void StationReset()
        {
            /***子线程切换为复位状态***/
            StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Initial);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.ResetStep)
                    {
                        case 0://等待机器人工位复位结果
                            CheckCurrentResetStatus();
                            if (MyVariable.RobotStationResetOK)
                            {
                                MyVariable.RobotStationResetOK = false;
                                LogConfig.Instance.ShowMessageToList("Run", "测序仪工位开始复位", MsgType.Success, Color.Blue);
                                this.ResetStep = 50;
                            }
                            break;
                        case 50:
                            SerializeClass.animationParam.sequXMark = 0;
                            resetRet = WaitSingleAxisHome(_SequencingStationAxis.测序仪XAxis.ToString(), Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorHomeTimeOut.ToString()].CurrentValue));
                            ResetResultJudge(resetRet, 70);
                            break;
                        case 70:
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.开盖完成
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序中
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序完成
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.关盖完成
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育中
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育完成)
                            {
                                this.ResetStep = 90;
                            }
                            else
                            {
                                this.ResetStep = 150;
                            }
                            break;
                        case 90:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.滴试剂位置;
                            resetRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            ResetResultJudge(resetRet, 200);
                            break;
                        case 150:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.开关盖位置;
                            resetRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                       Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                       Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            ResetResultJudge(resetRet, 200);
                            break;
                        case 200:
                            throw new StationHomeOK("测序仪工位线程复位完成！");
                    }
                }
                /***子线程复位失败跳转到这里***/
                catch (StationHomeErrException ex)
                {
                    //LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString()+ ex.Message, MsgType.Error, Color.Red);
                    MyVariable.RobotStationResetOK = false;
                    this.ResetStep = 0;
                    this.ResetError = true;
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
                /***子线程复位完成跳转到这里***/
                catch (StationHomeOK ex)
                {
                    this.ResetStep = 0;
                    this.ResetDone = true;
                    LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Success, Color.Green);
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        /// <summary>
        /// 单站运行动作
        /// </summary>
        public override void StationNormalRun()
        {
            this.RunDone = false;
            StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0://判断当前状态
                            switch (SerializeClass.mMemory.SequencingStation_state)
                            {
                                case MemoryClass.SequencingStation_State.空闲:
                                    this.RunStep = 10;
                                    break;
                                case MemoryClass.SequencingStation_State.去开预处理孔:
                                    this.RunStep = 100;
                                    break;
                                case MemoryClass.SequencingStation_State.可开预处理孔:
                                    this.RunStep = 140;
                                    break;
                                case MemoryClass.SequencingStation_State.开盖完成:
                                    this.RunStep = 200;
                                    break;
                                case MemoryClass.SequencingStation_State.去开上样孔:
                                    waitStart = false;
                                    this.RunStep = 300;
                                    break;
                                case MemoryClass.SequencingStation_State.可开上样孔:
                                    this.RunStep = 400;
                                    break;
                                case MemoryClass.SequencingStation_State.去关上样孔:
                                    this.RunStep = 500;
                                    break;
                                case MemoryClass.SequencingStation_State.可关上样孔:
                                    this.RunStep = 540;
                                    break;
                                case MemoryClass.SequencingStation_State.继续关预处理孔:
                                    this.RunStep = 580;
                                    break;
                                case MemoryClass.SequencingStation_State.等待关盖完成:
                                    this.RunStep = 610;
                                    break;
                                case MemoryClass.SequencingStation_State.测序中:
                                    this.RunStep = 680;
                                    break;
                                case MemoryClass.SequencingStation_State.测序完成:
                                    this.RunStep = 740;
                                    break;
                                case MemoryClass.SequencingStation_State.去关预处理孔:
                                    this.RunStep = 800;
                                    break;
                                case MemoryClass.SequencingStation_State.可关预处理孔:
                                    this.RunStep = 840;
                                    break;
                                case MemoryClass.SequencingStation_State.关盖完成:
                                    this.RunStep = 900;
                                    break;
                                case MemoryClass.SequencingStation_State.孵育中:
                                    waitStart = false;
                                    this.RunStep = 1000;
                                    break;
                                case MemoryClass.SequencingStation_State.孵育完成:
                                    this.RunStep = 1120;
                                    break;
                            }
                            break;

                        #region 当前状态  空闲
                        case 10:
                            CheckCurrentRunStatus(0, 10, 10);
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.空闲;
                            this.RunStep = 20;
                            break;
                        case 20://判断搬运工位状态
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.实验开始
                                || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.废液已吸取
                                || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.开始清洗步骤三)
                            {
                                this.RunStep = 100;
                            }
                            else if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.步骤一完成)
                            {
                                waitStart = true;
                                this.RunStep = 300;
                            }
                            else if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.测序配置完成)
                            {
                                this.RunStep = 500;
                            }
                            else if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.清洗步骤一完成
                                || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.清洗步骤三完成)
                            {
                                this.RunStep = 800;
                            }
                            else if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.清洗步骤二完成)
                            {
                                waitStart = true;
                                this.RunStep = 1000;
                            }
                            else
                            {
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region 开预处理孔流程
                        /*************************************开预处理孔流程***********************************/
                        case 100:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.去开预处理孔;
                            MyVariable.RobotWorkDone = false;
                            this.RunStep = 120;
                            break;
                        case 120:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.开关盖位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 140, 120, 120);
                            break;
                        case 140:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.可开预处理孔;
                            this.RunStep = 160;
                            break;
                        case 160:
                            CheckCurrentRunStatus(0, 160, 160);
                            if (MyVariable.RobotWorkDone)
                            {
                                MyVariable.RobotWorkDone = false;
                                this.RunStep = 180;
                            }
                            break;
                        case 180:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.滴试剂位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 190, 180, 180);
                            break;
                        case 190:
                            CheckCurrentRunStatus(0, 190, 190);
                            if (SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.开盖完成)
                            {
                                this.RunStep = 200;
                            }
                            break;
                        case 200:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.开盖完成;
                            if (MyVariable.newshow_IsOpen)
                            {
                                MyVariable.newshow_step1 = true;
                            }
                            WaitDelayTime(0.5);
                            this.RunStep = 220;
                            break;
                        case 220:
                            CheckCurrentRunStatus(0, 220, 220);
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.开始步骤一
                                 || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.开始步骤二
                                 || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.开始清洗步骤一
                                 || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.保存液排气泡)
                            {
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region 开上样孔流程
                        /*************************************开上样孔流程***********************************/
                        #region 当前状态  去开上样孔
                        case 300:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.去开上样孔;
                            MyVariable.RobotWorkDone = false;
                            if (waitStart)
                            {
                                lastTime = DateTime.Now;
                                MyVariable.PingHengStartTimeMemory = lastTime.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                if (MyVariable.PingHengStartTimeMemory == "")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "室温平衡时间丢失，检查运行状态是否错误", MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                                }
                                lastTime = DateTime.ParseExact(MyVariable.PingHengStartTimeMemory, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            }
                            SerializeClass.animationParam.waitStep = (int)_waitStepEnum.芯片室温平衡5分钟;
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:芯片室温平衡5分钟", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.芯片室温平衡5分钟;
                            this.RunStep = 320;
                            break;
                        case 320:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.开关盖位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 360, 320, 320);
                            break;
                        case 360:
                            CheckCurrentRunStatus(0, 360, 360);
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.DNA文库配置完成)
                            {
                                this.RunStep = 380;
                            }
                            break;
                        case 380: //判断是否已经静置五分钟
                            CheckCurrentRunStatus(0, 380, 380);
                            currentTime = DateTime.Now;
                            timeDifference = currentTime - lastTime;
                            if (MyVariable.show_IsOpen || MyVariable.newshow_IsOpen)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:参观模式,跳过室温平衡时间,开上样孔", MsgType.Success, Color.Brown);
                                SerializeClass.animationParam.waitStep = (int)_waitStepEnum.无等待时间;
                                SerializeClass.animationParam.RemainTime = 0;
                                this.RunStep = 400;
                                break;
                            }
                            if (timeDifference.TotalSeconds >= (double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.PingHengTime.ToString()].CurrentValue) * 60))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:到达" + ParameConfig.Instance.SystemParameDic[_ParamName.PingHengTime.ToString()].CurrentValue + "分钟,开上样孔", MsgType.Success, Color.Brown);
                                SerializeClass.animationParam.waitStep = (int)_waitStepEnum.无等待时间;
                                SerializeClass.animationParam.RemainTime = 0;
                                this.RunStep = 400;
                            }
                            else
                            {
                                CheckTime.Restart();
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:芯片室温平衡5分钟,剩余" + (double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.PingHengTime.ToString()].CurrentValue) - timeDifference.TotalMinutes).ToString("f2") + "分钟", MsgType.Success, Color.Brown);
                                SerializeClass.animationParam.RemainTime = Math.Round(double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.PingHengTime.ToString()].CurrentValue) - timeDifference.TotalMinutes, 2);
                                this.RunStep = 390;
                            }
                            break;
                        case 390:
                            CheckCurrentRunStatus(0, 390, 390);
                            if (CheckTime.ElapsedMilliseconds / 1000 >= (double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.CheckXinPianTime.ToString()].CurrentValue) * 60))
                            {
                                CheckTime.Stop();
                                this.RunStep = 380;
                            }
                            break;
                        #endregion

                        case 400:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.可开上样孔;
                            this.RunStep = 160;
                            break;
                        #endregion

                        #region 关上样孔和预处理孔,并测序
                        case 500:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.去关上样孔;
                            MyVariable.SN_SequencingStation = MyVariable.SN_CarryStation;
                            //测序参数传递到测序仪工位
                            SerializeClass.startParam_sequencingStation.protocol_group_id = SerializeClass.startParam_carryStation.protocol_group_id;
                            SerializeClass.startParam_sequencingStation.product_code = SerializeClass.startParam_carryStation.product_code;
                            SerializeClass.startParam_sequencingStation.sample_id = SerializeClass.startParam_carryStation.sample_id;
                            SerializeClass.startParam_sequencingStation.kit = SerializeClass.startParam_carryStation.kit;
                            SerializeClass.startParam_sequencingStation.speed = SerializeClass.startParam_carryStation.speed;
                            // SerializeClass.startParam_sequencingStation.experiment_time = SerializeClass.startParam_carryStation.experiment_time;
                            SerializeClass.startParam_sequencingStation.min_read_length = SerializeClass.startParam_carryStation.min_read_length;
                            SerializeClass.startParam_sequencingStation.guppy_filename = SerializeClass.startParam_carryStation.guppy_filename;
                            SerializeClass.startParam_sequencingStation.mux_scan_period = SerializeClass.startParam_carryStation.mux_scan_period;
                            MyVariable.RobotWorkDone = false;
                            this.RunStep = 520;
                            break;
                        case 520:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.开关盖位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 540, 520, 520);
                            break;
                        case 540:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.可关上样孔;
                            this.RunStep = 560;
                            break;
                        case 560:
                            CheckCurrentRunStatus(0, 560, 560);
                            if (SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.关盖完成)
                            {
                                if (MyVariable.newshow_IsOpen)
                                {
                                    MyVariable.newshow_IsOpen = false;
                                    this.RunStep = 620;
                                }
                                else
                                {
                                    this.RunStep = 580;
                                }
                            }
                            break;
                        case 580:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.继续关预处理孔;
                            MyVariable.RobotWorkDone = false;
                            this.RunStep = 600;
                            break;
                        case 600:
                            CheckCurrentRunStatus(0, 600, 600);
                            if (SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.关预处理孔盖中)
                            {
                                this.RunStep = 610;
                            }
                            break;
                        case 610:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.等待关盖完成;
                            this.RunStep = 620;
                            break;
                        case 620:
                            CheckCurrentRunStatus(0, 620, 620);
                            if (MyVariable.RobotWorkDone)
                            {
                                MyVariable.RobotWorkDone = false;
                                this.RunStep = 625;
                            }
                            break;
                        case 625://向总控发送指令获取碱基和iDNA
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽总控...", MsgType.Success, Color.Brown);
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)))
                                {
                                    SerializeClass.completeParam.protocol_group_ids = ini.Read<string>("SequenceFormParam", "protocol_group_id");
                                    SerializeClass.completeParam.protocol_group_JianJi = MyVariable.SingleExperiment;
                                    MyVariable.JianJiDic.Clear();
                                    string[] KeyValuess = SerializeClass.completeParam.protocol_group_JianJi.Split('|');
                                    for (int i = 0; i < KeyValuess.Length; i++)
                                    {
                                        string[] KeyValues2s = KeyValuess[i].Split('-');
                                        MyVariable.JianJiDic.Add(int.Parse(KeyValues2s[0]), KeyValues2s[1]);
                                    }
                                    this.RunStep = 635;
                                }
                                else
                                {
                                    this.RunStep = 640;
                                }
                            }
                            else
                            {
                                TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).ClearNetData();
                                SerializeClass.mCompleteReportingToControl.sn = MyVariable.SN_SequencingStation;
                                SerializeClass.mCompleteReportingToControl.experimentResult = "OK";
                                string jsonStr2 = JsonConvert.SerializeObject(SerializeClass.mCompleteReportingToControl);
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).WriteDataStr(jsonStr2))
                                {
                                    LogToGeneral(jsonStr2);
                                    this.time = this.GetCurveTime();
                                    WaitDelayTime(0.3);
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送指令获取碱基和iDNA", MsgType.Success, Color.Brown);
                                    this.RunStep = 630;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送数据失败", MsgType.Error, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 630://接收总控信息
                            CheckCurrentRunStatus(0, 630, 630);
                            if (OverTimeS(time, Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据超时", MsgType.Error, Color.Red);
                                this.RunStep = 625;
                                throw new StationErrorException("通讯报警");
                            }
                            else
                            {
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).NetCanRead())
                                {
                                    TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).LoopReadData(1, out Program.ControlReceived, Encoding.UTF8);
                                    LogFromGeneral(Program.ControlReceived);
                                    b_SequencingStation = MyVariable.GeneralCompleteReceive(Program.ControlReceived, out code_general, out SerializeClass.completeParam.data_idna,
                                        out SerializeClass.completeParam.data_taskId, out SerializeClass.completeParam.protocol_group_ids, out SerializeClass.completeParam.protocol_group_JianJi);
                                    if (b_SequencingStation)
                                    {
                                        if (code_general == 200)
                                        {
                                            MyVariable.JianJiDic.Clear();
                                            string[] KeyValues = SerializeClass.completeParam.protocol_group_JianJi.Split('|');
                                            for (int i = 0; i < KeyValues.Length; i++)
                                            {
                                                string[] KeyValues2 = KeyValues[i].Split('-');
                                                MyVariable.JianJiDic.Add(int.Parse(KeyValues2[0]), KeyValues2[1]);
                                            }
                                            LogConfig.Instance.ShowMessageToList("Run", "获取iDNA引物,任务ID以及实验名称", MsgType.Success, Color.Brown);
                                            this.RunStep = 635;
                                        }
                                        else
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "总控反馈异常", MsgType.Success, Color.Red);
                                            this.RunStep = 625;
                                            throw new StationErrorException("通讯报警");
                                        }
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据异常", MsgType.Success, Color.Red);
                                        this.RunStep = 625;
                                        throw new StationErrorException("通讯报警");
                                    }
                                }
                            }
                            break;
                        case 635://计算测序所需数据量，设置测序停止时间
                            CheckCurrentRunStatus(0, 635, 635);
                            AllJianJiLength = 0;
                            foreach (var item in MyVariable.JianJiDic)
                            {
                                AllJianJiLength += item.Value.Length;
                            }
                            MyVariable.sequencingNeedData = ((22 * MyVariable.JianJiDic.Count) + (SerializeClass.completeParam.data_idna.Length * MyVariable.JianJiDic.Count) + (AllJianJiLength * 10)) * 0.02;
                            this.RunStep = 640;
                            break;
                        case 640://启动测序仪
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:屏蔽测序流程,开始测序...", MsgType.Success, Color.Brown);
                            }
                            else
                            {
                                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceHandle.ToString()].CurrentValue)))
                                {
                                    this.RunStep = 660;
                                    LogConfig.Instance.ShowMessageToList("Run", "手动使用测序仪", MsgType.Success, Color.Green);
                                    break;
                                }
                                if (SequencingInterface.SequencingStart(out MyVariable.sequencing_code, out SerializeClass.id_sequencingStation.run_id))
                                {
                                    if (MyVariable.sequencing_code == "0")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "测序启动成功", MsgType.Success, Color.Green);
                                        LogConfig.Instance.ShowMessageToList("Run", "实验进程:开始测序...", MsgType.Success, Color.Brown);
                                        SerializeClass.animationParam.taskStep = (int)_taskStepEnum.开始测序;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "测序开始异常,状态码:" + MyVariable.sequencing_code, MsgType.Success, Color.Red);
                                        throw new StationErrorException("测序仪报警");
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "测序仪连接异常,检查测序接口是否打开", MsgType.Success, Color.Red);
                                    throw new StationErrorException("测序仪报警");
                                }
                            }
                            this.RunStep = 660;
                            break;
                        case 660:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.滴试剂位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 670, 660, 660);
                            break;
                        case 670:
                            CheckCurrentRunStatus(0, 670, 670);
                            if (SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.关盖完成)
                            {
                                this.RunStep = 680;
                            }
                            break;
                        case 680:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.测序中;
                            CeXuDelayTime.Restart();
                            this.RunStep = 700;
                            break;
                        case 700://每过一分钟查询一次状态
                            CheckCurrentRunStatus(0, 700, 700);
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                WaitDelayTime(5);
                                if (MyVariable.sign_SequenceFinish)
                                {
                                    MyVariable.sign_SequenceFinish = false;
                                }
                                this.RunStep = 730;
                            }
                            else if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceHandle.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "手动使用测序仪", MsgType.Success, Color.Brown);
                                MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.OK, true);
                                mb.TopMost = true;
                                mb.MsgShowDialog("提示", "当前选择手动使用测序仪,请手动开始测序!");
                                string btn = mb.ret.SelectedBtn;
                                if (btn == "btn_A")
                                {
                                    this.RunStep = 730;
                                    throw new StationErrorException("实验流程报警");
                                }
                            }
                            else
                            {
                                if (CeXuDelayTime.ElapsedMilliseconds / 1000 > 60)
                                {
                                    CeXuDelayTime.Stop();
                                    cexuMin++;
                                    LogConfig.Instance.ShowMessageToList("Run", "实验进程:测序中,已进行" + cexuMin + "分钟", MsgType.Success, Color.Brown);
                                    this.RunStep = 720;
                                }
                            }
                            break;
                        case 720:
                            b_SequencingStation = SequencingInterface.SequencingState(SequencingInterface.sequencing_State, 1, out MyVariable.sequencing_code,
                                                                                       out MyVariable.sequencing_data, out MyVariable.sequencing_msg, out MyVariable.sequencing_total_pore_count);
                            if (b_SequencingStation)
                            {
                                if (MyVariable.sequencing_code == "0" && (MyVariable.sequencing_data == "1" || MyVariable.sequencing_data == "2" || MyVariable.sequencing_data == "5"))
                                {
                                    cexuMin = 0;
                                    LogConfig.Instance.ShowMessageToList("Run", "实验进程:测序完成", MsgType.Success, Color.Brown);
                                    SerializeClass.animationParam.taskStep = (int)_taskStepEnum.测序完成;
                                    this.RunStep = 730;
                                }
                                else
                                {
                                    this.RunStep = 725;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "测序仪连接异常,检查测序接口是否打开", MsgType.Success, Color.Red);
                                throw new StationErrorException("测序仪报警");
                            }
                            break;
                        case 725:
                            b_SequencingStation = SequencingInterface.SequencingState(SequencingInterface.sequencing_Basecalled, 1, out MyVariable.sequencing_code,
                                                           out MyVariable.sequencing_data, out MyVariable.sequencing_msg, out MyVariable.sequencing_total_pore_count);
                            if (b_SequencingStation)
                            {
                                if (MyVariable.sequencing_code == "0" && (Math.Round(Convert.ToDouble(MyVariable.sequencing_data) / 1024, 4) >= (MyVariable.sequencingNeedData * double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceFilecoef.ToString()].CurrentValue))))
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "碱基匹配数量已达" + Math.Round(Convert.ToDouble(MyVariable.sequencing_data) / 1024, 4) + "/" + (MyVariable.sequencingNeedData * double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceFilecoef.ToString()].CurrentValue)) + "Mb", MsgType.Success, Color.Green);
                                    this.RunStep = 727;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "碱基匹配数量已达" + (Convert.ToDouble(MyVariable.sequencing_data) / 1000) + "/" + (MyVariable.sequencingNeedData * double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceFilecoef.ToString()].CurrentValue)) + "Mb", MsgType.Success, Color.Brown);
                                    this.RunStep = 680;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "测序仪连接异常,检查测序接口是否打开", MsgType.Success, Color.Red);
                                throw new StationErrorException("测序仪报警");
                            }
                            break;
                        case 727:
                            b_SequencingStation = SequencingInterface.SequencingNoParam(SequencingInterface.sequencing_Stop, out cexuCode, out cexuMsg, out cexuState, out cexuCom22, out cexuCom9502);
                            if (b_SequencingStation)
                            {
                                if (cexuCode == "0" && cexuMsg == "ok")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "实验进程:停止测序,等待碱基识别", MsgType.Success, Color.Brown);
                                    SerializeClass.animationParam.taskStep = (int)_taskStepEnum.等待碱基识别;
                                    WaitDelayTime(60);
                                    this.RunStep = 680;
                                }
                                else
                                {
                                    this.RunStep = 680;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "测序仪连接异常,检查测序接口是否打开", MsgType.Success, Color.Red);
                                throw new StationErrorException("测序仪报警");
                            }
                            break;
                        case 730://给数据处理线程标志(调试模式不触发)
                            MyVariable.CeXu_Completed = true;
                            this.RunStep = 732;
                            break;
                        case 732://计时超时时间
                            CeXuDelayTime.Restart();
                            this.RunStep = 735;
                            break;
                        case 735:
                            CheckCurrentRunStatus(0, 732, 732);
                            if (!MyVariable.CeXu_Completed)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "文件拷贝线程已接收当前实验任务", MsgType.Success, Color.Green);
                                this.RunStep = 740;
                            }
                            else if (CeXuDelayTime.ElapsedMilliseconds / 1000 >= 60)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "文件拷贝线程未接收当前实验任务", MsgType.Success, Color.Red);
                                this.RunStep = 732;
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 740:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.测序完成;
                            this.RunStep = 760;
                            break;
                        case 760:
                            CheckCurrentRunStatus(0, 760, 760);
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.清洗)
                            {
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region  关预处理孔流程
                        case 800:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.去关预处理孔;
                            MyVariable.RobotWorkDone = false;
                            this.RunStep = 820;
                            break;
                        case 820:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.开关盖位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 840, 820, 820);
                            break;
                        case 840:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.可关预处理孔;
                            this.RunStep = 860;
                            break;
                        case 860:
                            CheckCurrentRunStatus(0, 860, 860);
                            if (MyVariable.RobotWorkDone)
                            {
                                MyVariable.RobotWorkDone = false;
                                this.RunStep = 880;
                            }
                            break;
                        case 880:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.滴试剂位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 890, 880, 880);
                            break;
                        case 890:
                            CheckCurrentRunStatus(0, 890, 890);
                            if (SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.关盖完成)
                            {
                                this.RunStep = 900;
                            }
                            break;
                        case 900:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.关盖完成;
                            WaitDelayTime(0.5);
                            this.RunStep = 920;
                            break;
                        case 920:
                            CheckCurrentRunStatus(0, 920, 920);
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.开始清洗步骤二
                                 || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.开始清洗步骤四)
                            {
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region  芯片孵育流程
                        case 1000:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.孵育中;
                            SerializeClass.animationParam.waitStep = (int)_waitStepEnum.芯片室温孵育60分钟;
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:测序芯片开始孵育...", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.测序芯片开始孵育;
                            if (waitStart)
                            {
                                lastTime = DateTime.Now;
                                MyVariable.FuYuStartTimeMemory = lastTime.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                if (MyVariable.FuYuStartTimeMemory == "")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "孵育时间丢失，检查运行状态是否错误", MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                                }
                                lastTime = DateTime.ParseExact(MyVariable.FuYuStartTimeMemory, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            }
                            this.RunStep = 1020;
                            break;
                        case 1020://孵育60分钟
                            CheckCurrentRunStatus(0, 1020, 1020);
                            currentTime = DateTime.Now;
                            timeDifference = currentTime - lastTime;
                            if (MyVariable.show_IsOpen)
                            {
                                WaitDelayTime(10);
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:参观模式,跳过芯片孵育时间,孵育完成", MsgType.Success, Color.Brown);
                                SerializeClass.animationParam.taskStep = (int)_taskStepEnum.测序芯片孵育完成;
                                SerializeClass.animationParam.waitStep = (int)_waitStepEnum.无等待时间;
                                SerializeClass.animationParam.RemainTime = 0;
                                this.RunStep = 1120;
                                break;
                            }
                            if (timeDifference.TotalSeconds > (double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.FuYuTime.ToString()].CurrentValue) * 60))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:测序芯片孵育完成", MsgType.Success, Color.Brown);
                                SerializeClass.animationParam.taskStep = (int)_taskStepEnum.测序芯片孵育完成;
                                SerializeClass.animationParam.waitStep = (int)_waitStepEnum.无等待时间;
                                SerializeClass.animationParam.RemainTime = 0;
                                this.RunStep = 1120;
                            }
                            else
                            {
                                CheckTime.Restart();
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:芯片室温孵育" + ParameConfig.Instance.SystemParameDic[_ParamName.FuYuTime.ToString()].CurrentValue + "分钟,剩余"
                                    + (double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.FuYuTime.ToString()].CurrentValue) - timeDifference.TotalMinutes).ToString("f2") + "分钟", MsgType.Success, Color.Brown);
                                SerializeClass.animationParam.RemainTime = Math.Round(double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.FuYuTime.ToString()].CurrentValue) - timeDifference.TotalMinutes, 2);
                                this.RunStep = 1025;
                            }
                            break;
                        case 1025:
                            CheckCurrentRunStatus(0, 1025, 1025);
                            if (CheckTime.ElapsedMilliseconds / 1000 >= (double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.CheckXinPianTime.ToString()].CurrentValue) * 60))
                            {
                                CheckTime.Stop();
                                this.RunStep = 1020;
                            }
                            break;
                        case 1120:
                            SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.孵育完成;
                            this.RunStep = 1140;
                            break;
                        case 1140:
                            CheckCurrentRunStatus(0, 1140, 1140);
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.保存)
                            {
                                this.RunStep = 10;
                            }
                            break;
                            #endregion
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    #region  数字孪生
                    if (ex.Message.Contains("扫码枪报警"))
                    {
                        SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.扫码枪报警;
                    }
                    else if (ex.Message.Contains("CCD报警"))
                    {
                        SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.CCD报警;
                    }
                    else if (ex.Message.Contains("移液枪报警"))
                    {
                        SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.移液枪报警;
                    }
                    else if (ex.Message.Contains("电动夹爪报警"))
                    {
                        SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.电动夹爪报警;
                    }
                    else if (ex.Message.Contains("机器人报警"))
                    {
                        SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.机器人报警;
                    }
                    else if (ex.Message.Contains("测序仪报警"))
                    {
                        SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.测序仪报警;
                    }
                    else if (ex.Message.Contains("实验流程报警"))
                    {
                        SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.实验流程报警;
                    }
                    else if (ex.Message.Contains("通讯报警"))
                    {
                        SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.通讯报警;
                    }
                    #endregion
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunStep = 0;
                    this.RunDone = true;
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        /// <summary>
        /// 单站空跑动作
        /// </summary>
        public override void StationEmptyRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Stop);
        }

        public override void StationCalibRun()
        {
            this.RunDone = false;
            StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0://判断当前状态
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲 && SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.空闲
                                && SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲 && SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "开始上相机标定", MsgType.Success, Color.Brown);
                                this.RunStep = 120;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "设备当前有任务执行，无法上相机标定", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 120:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.开关盖位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 140, 120, 120);
                            break;
                        case 140:
                            MyVariable.CalibRun_Run = true;
                            this.RunStep = 160;
                            break;
                        case 160:
                            CheckCurrentRunStatus(0, 160, 160);
                            if (MyVariable.CalibRun_RunDone)
                            {
                                MyVariable.CalibRun_RunDone = false;
                                this.RunStep = 180;
                            }
                            break;
                        case 180:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.滴试剂位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 180, 180);
                            break;
                        case 200:
                            throw new StationWorkDone("");
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunStep = 0;
                    this.RunDone = true;
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        public override void StationCPKRun()
        {
            this.RunDone = false;
            StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0://判断当前状态
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲 && SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.空闲
                                && SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲 && SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "开始下相机标定", MsgType.Success, Color.Brown);
                                this.RunStep = 120;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "设备当前有任务执行，无法下相机标定", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 120:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.开关盖位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 140, 120, 120);
                            break;
                        case 140:
                            MyVariable.CalibRun_Run = true;
                            this.RunStep = 160;
                            break;
                        case 160:
                            CheckCurrentRunStatus(0, 160, 160);
                            if (MyVariable.CalibRun_RunDone)
                            {
                                MyVariable.CalibRun_RunDone = false;
                                this.RunStep = 180;
                            }
                            break;
                        case 180:
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.滴试剂位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.滴试剂位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 180, 180);
                            break;
                        case 200:
                            throw new StationWorkDone("");
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.测序仪工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunStep = 0;
                    this.RunDone = true;
                    StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        public override void StationGRRRun()
        {
            base.StationGRRRun();
        }

        public override void StationCamStaticRun()
        {
            base.StationCamStaticRun();
        }

        public override void StationCamDynamicRun()
        {
            base.StationCamDynamicRun();
        }
        /// <summary>
        /// 给总控发送信息Log记录
        /// </summary>
        /// <param name="sendmsg"></param>
        private void LogToGeneral(string sendmsg)
        {
            string NowDate = string.Format("{0:yyyyMMdd}", DateTime.Now);//获取当前日期
            if (!Directory.Exists(@"E:\SWLog\General\"))
            {
                Directory.CreateDirectory(@"E:\SWLog\General\");
            }
            if (!File.Exists(@"E:\SWLog\General\" + NowDate + ".txt"))
            {
                File.Create(@"E:\SWLog\General\" + NowDate + ".txt").Close();
            }
            if (File.Exists(@"E:\SWLog\General\" + NowDate + ".txt"))
            {
                using (FileStream fsWrite = new FileStream(@"E:\SWLog\General\" + NowDate + ".txt", FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fsWrite, Encoding.Unicode))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  CeXu-->General  " + sendmsg);
                    }
                }
            }
        }

        /// <summary>
        /// 从总控获取信息log记录
        /// </summary>
        /// <param name="sendmsg"></param>
        private void LogFromGeneral(string sendmsg)
        {
            string NowDate = string.Format("{0:yyyyMMdd}", DateTime.Now);//获取当前日期
            if (!Directory.Exists(@"E:\SWLog\General\"))
            {
                Directory.CreateDirectory(@"E:\SWLog\General\");
            }
            if (!File.Exists(@"E:\SWLog\General\" + NowDate + ".txt"))
            {
                File.Create(@"E:\SWLog\General\" + NowDate + ".txt").Close();
            }
            if (File.Exists(@"E:\SWLog\General\" + NowDate + ".txt"))
            {
                using (FileStream fsWrite = new FileStream(@"E:\SWLog\General\" + NowDate + ".txt", FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fsWrite, Encoding.Unicode))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  General-->CeXu  " + sendmsg);
                    }
                }
            }
        }


    }
}

