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
using ktCnt;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public class CarryStation : ObjectStation
    {
        INIFile ini = new INIFile(Application.StartupPath + "\\FileINI\\SequenceForm.ini");
        private string mName;
        private _ActionResult resetRet;//单步复位结果
        private _ActionResult runRet;//单步运行结果

        //private int FunctionStep = 0;//功能块步序
        private bool b_function;//功能块运行标志

        private short[] read_PLC1;

        private Stopwatch sw_YiYeQiang = new Stopwatch();
        private string cexuCode;//测序仪接口反馈响应码
        private string cexuCom22;//测序仪端口22
        private string cexuCom9502;//测序仪端口9502
        private string cexuMsg;
        private string cexuState;
        private string pipettegunCmd;

        public long time;//总控通讯超时判断

        private int code_general;//总控反馈响应码
        private int data_general;//总控反馈是否有后续任务；1是，0否
        private bool b_carrystation;//通用判断标志

        public CarryStation(string name) :
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
            StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Initial);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.ResetStep)
                    {
                        case 0:
                            LogConfig.Instance.ShowMessageToList("Run", "搬运工位线程开始复位", MsgType.Success, Color.Blue);
                            MyVariable.FunctionStep = 0;
                            this.ResetStep = 10;
                            break;
                        case 10://判断状态
                            if (MyVariable.EmptyRun_Run)
                            {
                                //空载具回收模式复位
                                LogConfig.Instance.ShowMessageToList("Run", "空载具回收模式复位", MsgType.Success, Color.Blue);
                                if (SerializeClass.mMemory.clamping_jaw_technology == MemoryClass.Clamping_jaw_technology.夹爪夹紧)
                                {
                                    this.ResetStep = 50;
                                }
                                else
                                {
                                    this.ResetStep = 20;
                                }
                            }
                            else
                            {
                                //自动运行模式复位
                                if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲)
                                {
                                    this.ResetStep = 20;
                                }
                                else
                                {
                                    this.ResetStep = 50;
                                }
                            }
                            break;
                        #region 空闲时复位流程
                        case 20://电动夹爪张开
                            CheckCurrentResetStatus();
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawAbsMove(Program.carryClawConfigList[0], 3000))
                            {
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.松开;
                                this.ResetStep = 40;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationHomeErrException("");
                            }
                            break;
                        case 40://串口发送,移液枪Z轴回初始位,移液枪初始化
                            CheckCurrentResetStatus();
                            LogConfig.Instance.ShowMessageToList("Run", "移液枪初始化", MsgType.Success, Color.Blue);
                            pipettegunCmd = $"41[Zz{MyVariable.z_Initial_speed}]1[It{MyVariable.gun_Initial_speed}];";
                            SerializeClass.animationParam.gunZMark = 0;
                            SerializeClass.animationParam.gunZSpeed = MyVariable.gun_Initial_speed;
                            resetRet = PipetteGunSend(pipettegunCmd);
                            ResetResultJudge(resetRet, 45);
                            break;
                        case 45://接收
                            CheckCurrentResetStatus();

                            resetRet = PipetteGunReceive();
                            ResetResultJudge(resetRet, 70);
                            break;
                        #endregion

                        #region 不是空闲时复位流程
                        case 50://串口发送,移液枪Z轴回初始位
                            CheckCurrentResetStatus();
                            LogConfig.Instance.ShowMessageToList("Run", "移液枪初始化", MsgType.Success, Color.Blue);
                            pipettegunCmd = $"41[Zz{MyVariable.z_Initial_speed}];";
                            SerializeClass.animationParam.gunZMark = 0;
                            SerializeClass.animationParam.gunZSpeed = MyVariable.gun_Initial_speed;
                            resetRet = PipetteGunSend(pipettegunCmd);
                            ResetResultJudge(resetRet, 60);
                            break;
                        case 60://串口接收,移液枪Z轴到位
                            CheckCurrentResetStatus();
                            resetRet = PipetteGunReceive();
                            ResetResultJudge(resetRet, 70);
                            break;
                        #endregion

                        case 70://判断是否夹载具
                            if (SerializeClass.mMemory.clamping_jaw_technology == MemoryClass.Clamping_jaw_technology.夹爪夹紧)
                            {
                                MotionConfig.Instance.ServoOn(_CarryStation2Axis.搬运ZAxis.ToString());
                                this.ResetStep = 80;
                            }
                            else
                            {
                                this.ResetStep = 90;
                            }
                            break;
                        case 80://判断夹的载具是枪头还是试剂
                            if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4
                                || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区3 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区4)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                                resetRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.试管搬运上升位置;
                                resetRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            ResetResultJudge(resetRet, 100);
                            break;
                        case 90://搬运模组Z轴回零
                            SerializeClass.animationParam.carryZMark = 0;
                            resetRet = WaitSingleAxisHome(_CarryStation2Axis.搬运ZAxis.ToString(), Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorHomeTimeOut.ToString()].CurrentValue));
                            ResetResultJudge(resetRet, 95);
                            break;
                        case 95:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            resetRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            ResetResultJudge(resetRet, 100);
                            break;
                        case 100://搬运模组X,Y轴回零
                            SerializeClass.animationParam.carryXMark = 0;
                            SerializeClass.animationParam.carryYMark = 0;
                            resetRet = WaitMultipleAxisHome(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                         Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorHomeTimeOut.ToString()].CurrentValue));
                            ResetResultJudge(resetRet, 110);
                            break;
                        case 110:
                            MyVariable.CarryStationResetOK = true;
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.待机位置;
                            resetRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            ResetResultJudge(resetRet, 200);
                            break;
                        case 200:
                            throw new StationHomeOK("搬运工位线程复位完成！");
                    }
                }
                /***子线程复位失败跳转到这里***/
                catch (StationHomeErrException ex)
                {
                    //LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.搬运工位.ToString() + ex.Message, MsgType.Error, Color.Red);
                    MyVariable.CarryStationResetOK = false;
                    this.ResetStep = 0;
                    this.ResetError = true;
                    StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
                /***子线程复位完成跳转到这里***/
                catch (StationHomeOK ex)
                {
                    this.ResetStep = 0;
                    this.ResetDone = true;
                    LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Success, Color.Green);
                    StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Stop);
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
            StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0:
                            switch (SerializeClass.mMemory.CarryStation_state)
                            {
                                case MemoryClass.CarryStation_State.空闲:
                                    this.RunStep = 10;
                                    break;
                                case MemoryClass.CarryStation_State.换料中:
                                    this.RunStep = 100;
                                    break;
                                case MemoryClass.CarryStation_State.上料中:
                                    this.RunStep = 470;
                                    break;
                                case MemoryClass.CarryStation_State.供料完成:
                                    this.RunStep = 840;
                                    break;
                                case MemoryClass.CarryStation_State.实验开始:
                                    this.RunStep = 1010;
                                    break;
                                case MemoryClass.CarryStation_State.开始步骤一:
                                    this.RunStep = 2020;
                                    break;
                                case MemoryClass.CarryStation_State.步骤一完成:
                                    if (MyVariable.newshow_IsOpenOver)//流转参观模式下流程跳变
                                    {
                                        this.RunStep = 8000;
                                    }
                                    else
                                    {
                                        this.RunStep = 2440;
                                    }
                                    break;
                                case MemoryClass.CarryStation_State.DNA文库配置完成:
                                    if (MyVariable.newshow_IsOpenOver)//流转参观模式下流程跳变
                                    {
                                        this.RunStep = 8020;
                                    }
                                    else
                                    {
                                        this.RunStep = 3300;
                                    }
                                    break;
                                case MemoryClass.CarryStation_State.开始步骤二:
                                    if (MyVariable.newshow_IsOpenOver)//流转参观模式下流程跳变
                                    {
                                        if (MyVariable.show_memory == 1)
                                        {
                                            this.RunStep = 8060;
                                        }
                                        else if (MyVariable.show_memory == 2)
                                        {
                                            this.RunStep = 8080;
                                        }
                                        else if (MyVariable.show_memory == 3)
                                        {
                                            this.RunStep = 8100;
                                        }
                                        else if (MyVariable.show_memory == 4)
                                        {
                                            this.RunStep = 8120;
                                        }
                                        else
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                            throw new StationErrorException("实验流程报警");
                                        }
                                    }
                                    else
                                    {
                                        this.RunStep = 3400;
                                    }
                                    break;
                                case MemoryClass.CarryStation_State.测序配置完成:
                                    this.RunStep = 3860;
                                    break;
                                case MemoryClass.CarryStation_State.清洗:
                                    this.RunStep = 4000;
                                    break;
                                case MemoryClass.CarryStation_State.废液已吸取:
                                    this.RunStep = 4880;
                                    break;
                                case MemoryClass.CarryStation_State.开始清洗步骤一:
                                    this.RunStep = 5040;
                                    break;
                                case MemoryClass.CarryStation_State.清洗步骤一完成:
                                    this.RunStep = 5440;
                                    break;
                                case MemoryClass.CarryStation_State.开始清洗步骤二:
                                    this.RunStep = 5580;
                                    break;
                                case MemoryClass.CarryStation_State.清洗步骤二完成:
                                    this.RunStep = 6060;
                                    break;
                                case MemoryClass.CarryStation_State.保存:
                                    this.RunStep = 6200;
                                    break;
                                case MemoryClass.CarryStation_State.开始清洗步骤三:
                                    this.RunStep = 6260;
                                    break;
                                case MemoryClass.CarryStation_State.保存液排气泡:
                                    this.RunStep = 6360;
                                    break;
                                case MemoryClass.CarryStation_State.清洗步骤三完成:
                                    this.RunStep = 6660;
                                    break;
                                case MemoryClass.CarryStation_State.开始清洗步骤四:
                                    this.RunStep = 6820;
                                    break;
                                case MemoryClass.CarryStation_State.实验完成:
                                    this.RunStep = 6960;
                                    break;
                                case MemoryClass.CarryStation_State.出料:
                                    this.RunStep = 7000;
                                    break;
                            }
                            break;

                        #region 当前状态  空闲
                        case 10:
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪默认松开;
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.未取枪头;
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.空闲;
                            CheckCurrentRunStatus(0, 10, 10);
                            this.RunStep = 20;
                            break;
                        case 20:
                            if (SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.换料)
                            {
                                this.RunStep = 100;
                            }
                            else if (SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.缺料)
                            {
                                this.RunStep = 470;
                            }
                            else if (SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.满料)
                            {
                                this.RunStep = 1000;
                            }
                            else
                            {
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region 换料,补料流程

                        #region 当前状态  换料中
                        case 100:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.换料中;
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.换料中;
                            switch (SerializeClass.mMemory.clamping_jaw_technology)
                            {
                                case MemoryClass.Clamping_jaw_technology.夹爪默认松开:
                                    this.RunStep = 110;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪夹紧:
                                    this.RunStep = 230;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪松开:
                                    this.RunStep = 380;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.过渡点:
                                    this.RunStep = 460;
                                    break;
                            }
                            break;
                        case 110:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 120, 110, 110);
                            break;
                        case 120:
                            CheckCurrentRunStatus(0, 120, 120);
                            LogConfig.Instance.ShowMessageToList("Run", "换料流程出料区域: " + SerializeClass.mMemory.area.ToString(), MsgType.Success, Color.Blue);
                            switch (SerializeClass.mMemory.area)
                            {
                                case MemoryClass.Area.枪头区1:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区1搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 140, 120, 120);
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 3);
                                        LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 3", MsgType.Success, Color.Blue);
                                    }
                                    break;
                                case MemoryClass.Area.枪头区2:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区2搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 140, 120, 120);
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 3);
                                        LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 3", MsgType.Success, Color.Blue);
                                    }
                                    break;
                                case MemoryClass.Area.枪头区3:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区3搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 140, 120, 120);
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 2);
                                        LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 2", MsgType.Success, Color.Blue);
                                    }
                                    break;
                                case MemoryClass.Area.枪头区4:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区4搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 140, 120, 120);
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 1);
                                        LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 1", MsgType.Success, Color.Blue);
                                    }
                                    break;
                                case MemoryClass.Area.低温区:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.低温区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 150, 120, 120);
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 9);
                                        LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 9", MsgType.Success, Color.Blue);
                                    }
                                    break;
                                case MemoryClass.Area.常温试剂区:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.常温试剂区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 160, 120, 120);
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 16);
                                        LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 16", MsgType.Success, Color.Blue);
                                    }
                                    break;
                                case MemoryClass.Area.离心管试管区:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.离心管试管区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 170, 120, 120);
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 21);
                                        LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 21", MsgType.Success, Color.Blue);
                                    }
                                    break;
                                case MemoryClass.Area.八联排试管区:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.八联排试管区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 180, 120, 120);
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 19);
                                        LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 19", MsgType.Success, Color.Blue);
                                    }
                                    break;
                            }
                            break;

                        case 140://到枪头区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 140, 140);
                            break;
                        case 150://到低温区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.低温区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.低温区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.低温区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 150, 150);
                            break;
                        case 160://到常温区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.常温试剂区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 160, 160);
                            break;
                        case 170://到1.5离心管区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.离心管试管区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 170, 170);
                            break;
                        case 180://到8连排试管区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.八联排试管区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 180, 180);
                            break;

                        case 200://发送夹爪夹紧指令
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawForceMove(Program.carryClawConfigList[1], 3000))
                            {
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 230;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 230:
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪夹紧;
                            this.RunStep = 240;
                            break;
                        case 240:
                            if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 280, 240, 240);
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.试管搬运上升位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 310, 240, 240);
                            }
                            break;
                        //去枪头出料区
                        case 280:
                            if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头出料区光电1] && !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头出料区光电2])
                            {
                                this.RunStep = 290;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "枪头出料区有载具搁置,无法换料!", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 290:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.枪头出料区搬运位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 300, 290, 290);
                            break;
                        case 300:
                            if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头1000进出料区抓取位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头200进出料区抓取位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            RunResultJudge(runRet, 350, 300, 300);
                            break;

                        //去出料区搬运位置
                        case 310:
                            if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电1] && !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电2])
                            {
                                this.RunStep = 320;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "出料区有载具搁置,无法换料!", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 320:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.出料区搬运位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 330, 320, 320);
                            break;
                        case 330:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.进出料区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 350, 330, 330);
                            break;


                        case 350://搬运夹爪松开指令
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawAbsMove(Program.carryClawConfigList[0], 3000))
                            {
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 380;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 380:
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪松开;
                            this.RunStep = 385;
                            break;
                        case 385:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 460, 385, 385);
                            break;

                        //case 390://抬Z轴
                        //    if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2)
                        //    {
                        //        runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                        //           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                        //           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        //        RunResultJudge(runRet, 410, 390, 390);
                        //    }
                        //    else if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4)
                        //    {
                        //        runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                        //           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                        //           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        //        RunResultJudge(runRet, 410, 390, 390);
                        //    }
                        //    else
                        //    {
                        //        runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                        //           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.出料扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                        //           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        //        RunResultJudge(runRet, 410, 390, 390);
                        //    }
                        //    break;
                        //case 410://走扫码位置
                        //    if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4)
                        //    {
                        //        runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                        //              new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头出料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头出料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                        //              Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        //        RunResultJudge(runRet, 430, 410, 410);
                        //    }
                        //    else
                        //    {
                        //        runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                        //              new double[] { ParameConfig.Instance.PointParameDic[_PointArray.出料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.出料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                        //              Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        //        RunResultJudge(runRet, 430, 410, 410);
                        //    }
                        //    break;
                        //case 430://扫码,传信息
                        //    if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledBarcode.ToString()].CurrentValue)))
                        //    {
                        //        this.RunStep = 460;
                        //    }
                        //    else
                        //    {
                        //        runRet = WaitNetData(_TcpClientModule.Scan.ToString(), Program.ScanCmd, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.ScanReceived);
                        //        if (Program.ScanReceived != "")
                        //        {
                        //            Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, short.Parse(Program.ScanReceived.Substring(2, 2)));
                        //            LogConfig.Instance.ShowMessageToList("Run", "扫码成功传递信息给PLC: " + Program.ScanReceived.Substring(2, 2), MsgType.Success, Color.Blue);
                        //            RunResultJudge(runRet, 460, 430, 430);
                        //        }
                        //        else
                        //        {
                        //            LogConfig.Instance.ShowMessageToList("Run", "未扫到码", MsgType.Success, Color.Red);
                        //            throw new StationErrorException("未扫到码");
                        //        }
                        //    }
                        //    break;
                        case 460:
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.过渡点;
                            this.RunStep = 470;
                            break;
                        #endregion

                        #region 当前状态  上料中
                        case 470:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.上料中;
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.上料中;
                            switch (SerializeClass.mMemory.clamping_jaw_technology)
                            {
                                case MemoryClass.Clamping_jaw_technology.过渡点:
                                    this.RunStep = 500;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪夹紧:
                                    this.RunStep = 575;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪松开:
                                    this.RunStep = 770;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪默认松开:
                                    this.RunStep = 500;
                                    break;
                            }
                            break;
                        case 500:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 502, 500, 500);
                            break;
                        case 502:
                            if (SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                            {
                                SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                SerializeClass.animationParam.material1 = (int)_PointArray.地轨避让位置;
                                runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 505, 502, 502);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人工作中，检查状态", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 505://走到避让位，允许进料
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "到避让位，允许进料；64601写1", MsgType.Success, Color.Brown);
                                Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 1);
                            }
                            this.RunStep = 508;
                            break;
                        case 508://判断地轨进料完成
                            CheckCurrentRunStatus(0, 508, 508);
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                if (SerializeClass.mMemory.area == MemoryClass.Area.进料区 && SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.八联排试管区)
                                {
                                    MyVariable.DNA_Arrive = true;
                                    MyVariable.need_Completed = false;
                                }
                                if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4
                                         || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区3 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区4)
                                {
                                    //光电复判
                                    if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头进料区光电1] && IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头进料区光电2])
                                    {
                                        this.RunStep = 510;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "载具未放入枪头进料区", MsgType.Success, Color.Red);
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                                else
                                {
                                    //光电复判
                                    if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.进料区光电1] && IOConfig.Instance.InputsStatus[(Int32)_InputCollect.进料区光电2])
                                    {
                                        this.RunStep = 510;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "载具未放入进料区", MsgType.Success, Color.Red);
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                            }
                            else
                            {
                                if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC1))
                                {
                                    if (read_PLC1[0] == 3)
                                    {
                                        if (SerializeClass.mMemory.area == MemoryClass.Area.进料区 && SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.八联排试管区)
                                        {
                                            MyVariable.DNA_Arrive = true;
                                            MyVariable.need_Completed = false;
                                        }
                                        if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4
                                                 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区3 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区4)
                                        {
                                            //光电复判
                                            if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头进料区光电1] && IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头进料区光电2])
                                            {
                                                this.RunStep = 510;
                                            }
                                            else
                                            {
                                                LogConfig.Instance.ShowMessageToList("Run", "载具未放入枪头进料区", MsgType.Success, Color.Red);
                                                throw new StationErrorException("实验流程报警");
                                            }
                                        }
                                        else
                                        {
                                            //光电复判
                                            if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.进料区光电1] && IOConfig.Instance.InputsStatus[(Int32)_InputCollect.进料区光电2])
                                            {
                                                this.RunStep = 510;
                                            }
                                            else
                                            {
                                                LogConfig.Instance.ShowMessageToList("Run", "载具未放入进料区", MsgType.Success, Color.Red);
                                                throw new StationErrorException("实验流程报警");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 510://到进料位扫码位置
                            if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4
                                || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区3 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区4)
                            {
                                SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头进料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头进料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                SerializeClass.animationParam.material1 = (int)_PointArray.枪头进料扫码位置;
                                runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                      new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头进料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头进料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                      Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 512, 510, 510);
                            }
                            else
                            {
                                SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.进料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.进料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                SerializeClass.animationParam.material1 = (int)_PointArray.进料扫码位置;
                                runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                      new double[] { ParameConfig.Instance.PointParameDic[_PointArray.进料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.进料扫码位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                      Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 512, 510, 510);
                            }
                            break;
                        case 512:
                            if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头1000扫码下降位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                   Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                   Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 515, 512, 512);
                            }
                            else if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区3 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区4)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头200扫码下降位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                   Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                   Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 515, 512, 512);
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进料扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.进料扫码下降位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                   Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进料扫码下降位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                   Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 515, 512, 512);
                            }
                            break;
                        case 515:
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledBarcode.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽扫码", MsgType.Success, Color.Blue);
                                this.RunStep = 520;
                            }
                            else
                            {
                                Program.ScanReceived = "";
                                runRet = WaitNetData(_TcpClientModule.Scan.ToString(), Program.ScanCmd, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.ScanReceived);
                                if (Program.ScanReceived == "NG")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "扫码NG", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                                else if (Program.ScanReceived != "")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "扫码成功：" + Program.ScanReceived, MsgType.Success, Color.Green);
                                    if (Program.ScanReceived.Length == 6)
                                    {
                                        RunResultJudge(runRet, 520, 515, 515);
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "扫码长度异常", MsgType.Success, Color.Red);
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "扫码失败", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 520://检查耗材是否匹配
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledBarcode.ToString()].CurrentValue)))
                            {
                                if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2
                                    || SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区4)
                                {
                                    this.RunStep = 530;
                                }
                                else
                                {
                                    this.RunStep = 540;
                                }
                            }
                            else
                            {
                                if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2)
                                {
                                    if (Program.ScanReceived.Substring(2, 2) == "03")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配成功", MsgType.Success, Color.Green);
                                        this.RunStep = 530;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配失败,请确认进出料是否统一!", MsgType.Success, Color.Red);
                                        this.RunStep = 515;
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                                else if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区3)
                                {
                                    if (Program.ScanReceived.Substring(2, 2) == "02")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配成功", MsgType.Success, Color.Green);
                                        this.RunStep = 530;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配失败,请确认进出料是否统一!", MsgType.Success, Color.Red);
                                        this.RunStep = 515;
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                                else if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区4 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区4)
                                {
                                    if (Program.ScanReceived.Substring(2, 2) == "01")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配成功", MsgType.Success, Color.Green);
                                        this.RunStep = 530;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配失败,请确认进出料是否统一!", MsgType.Success, Color.Red);
                                        this.RunStep = 515;
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                                else if (SerializeClass.mMemory.area == MemoryClass.Area.低温区 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.低温区)
                                {
                                    if (Program.ScanReceived.Substring(2, 2) == "09")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配成功", MsgType.Success, Color.Green);
                                        this.RunStep = 540;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配失败,请确认进出料是否统一!", MsgType.Success, Color.Red);
                                        this.RunStep = 515;
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                                else if (SerializeClass.mMemory.area == MemoryClass.Area.离心管试管区 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.离心管试管区)
                                {
                                    if (Program.ScanReceived.Substring(2, 2) == "21")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配成功", MsgType.Success, Color.Green);
                                        this.RunStep = 540;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "耗材匹配失败,请确认进出料是否统一!", MsgType.Success, Color.Red);
                                        this.RunStep = 515;
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                                else if (SerializeClass.mMemory.area == MemoryClass.Area.八联排试管区 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.八联排试管区)
                                {
                                    if (Program.ScanReceived.Substring(2, 2) == "23" || Program.ScanReceived.Substring(2, 2) == "19")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "样本匹配成功", MsgType.Success, Color.Green);
                                        this.RunStep = 540;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "样本匹配失败,请确认进出料是否统一!", MsgType.Success, Color.Red);
                                        this.RunStep = 515;
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                            }
                            break;

                        //枪头进料
                        case 530:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头进料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头进料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.枪头进料区搬运位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头进料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头进料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 535, 530, 530);
                            break;
                        case 535:
                            if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头1000进出料区抓取位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头200进出料区抓取位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            RunResultJudge(runRet, 560, 535, 535);
                            break;

                        //试剂进料
                        case 540:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.进料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.进料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.进料区搬运位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.进料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.进料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 545, 540, 540);
                            break;
                        case 545:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.进出料区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 560, 545, 545);
                            break;


                        case 560://搬运夹爪夹紧指令
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawForceMove(Program.carryClawConfigList[1], 3000))
                            {
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 575;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 575:
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪夹紧;
                            this.RunStep = 580;
                            break;
                        case 580:
                            if (SerializeClass.mMemory.area == MemoryClass.Area.枪头区1 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区2 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区1 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区2
                                || SerializeClass.mMemory.area == MemoryClass.Area.枪头区3 || SerializeClass.mMemory.area == MemoryClass.Area.枪头区4 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区3 || SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.枪头区4)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.试管搬运上升位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            RunResultJudge(runRet, 600, 580, 580);
                            break;
                        case 600://换料情况下判断区域
                            switch (SerializeClass.mMemory.area)
                            {
                                case MemoryClass.Area.枪头区1:
                                    MyVariable.area_QiangTou1.num_Remain = MyVariable.area_QiangTou1.num_XMax * MyVariable.area_QiangTou1.num_YMax;
                                    MyVariable.sign_TIP1 = false;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区1搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 620, 600, 600);
                                    break;
                                case MemoryClass.Area.枪头区2:
                                    MyVariable.consumables_Empty[0] = false;
                                    MyVariable.sign_TIP1 = false;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64616, 0);
                                    }
                                    MyVariable.area_QiangTou2.num_Remain = MyVariable.area_QiangTou2.num_XMax * MyVariable.area_QiangTou2.num_YMax;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区2搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 620, 600, 600);
                                    break;
                                case MemoryClass.Area.枪头区3:
                                    MyVariable.consumables_Empty[1] = false;
                                    MyVariable.sign_TIP3 = false;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64615, 0);
                                    }
                                    MyVariable.area_QiangTou3.num_Remain = MyVariable.area_QiangTou3.num_XMax * MyVariable.area_QiangTou3.num_YMax;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区3搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 620, 600, 600);
                                    break;
                                case MemoryClass.Area.枪头区4:
                                    MyVariable.consumables_Empty[2] = false;
                                    MyVariable.sign_TIP4 = false;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64614, 0);
                                    }
                                    MyVariable.area_QiangTou4.num_Remain = MyVariable.area_QiangTou4.num_XMax * MyVariable.area_QiangTou4.num_YMax;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区4搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 620, 600, 600);
                                    break;
                                case MemoryClass.Area.低温区:
                                    MyVariable.consumables_Empty[3] = false;
                                    MyVariable.sign_DiWen = false;
                                    //低温区补料完成,变量置为满料
                                    MyVariable.area_DiWen_FCF.num_Remain = MyVariable.FCF_MAX;
                                    MyVariable.area_DiWen_FCT.num_Remain = MyVariable.FCT_MAX;
                                    MyVariable.area_DiWen_SB.num_Remain = MyVariable.SB_MAX;
                                    MyVariable.area_DiWen_LIB.num_Remain = MyVariable.LIB_MAX;
                                    MyVariable.area_DiWen_DIL.num_Remain = MyVariable.DIL_MAX;
                                    MyVariable.area_DiWen_WMX.num_Remain = MyVariable.WMX_MAX;
                                    MyVariable.area_DiWen_S.num_Remain = MyVariable.S_MAX;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64622, 0);
                                    }
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.低温区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 640, 600, 600);
                                    break;
                                case MemoryClass.Area.常温试剂区:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.常温试剂区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 660, 600, 600);
                                    break;
                                case MemoryClass.Area.离心管试管区:
                                    MyVariable.sign_LiXinGuan = false;
                                    MyVariable.consumables_Empty[4] = false;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64634, 0);
                                    }
                                    MyVariable.area_LiXinGuan.num_Remain = MyVariable.area_LiXinGuan.num_XMax * MyVariable.area_LiXinGuan.num_YMax;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.离心管试管区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 680, 600, 600);
                                    break;
                                case MemoryClass.Area.八联排试管区:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.八联排试管区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 700, 600, 600);
                                    break;
                                case MemoryClass.Area.进料区:
                                    this.RunStep = 610;
                                    break;
                            }
                            break;
                        case 610://缺料情况下判断区域
                            switch (SerializeClass.mMemory.area_noout)
                            {
                                case MemoryClass.NoOutArea.枪头区1:
                                    MyVariable.area_QiangTou1.num_Remain = MyVariable.area_QiangTou1.num_XMax * MyVariable.area_QiangTou1.num_YMax;
                                    MyVariable.sign_TIP1 = false;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区1搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 620, 610, 610);
                                    break;
                                case MemoryClass.NoOutArea.枪头区2:
                                    MyVariable.consumables_Empty[0] = false;
                                    MyVariable.sign_TIP1 = false;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64616, 0);
                                    }
                                    MyVariable.area_QiangTou2.num_Remain = MyVariable.area_QiangTou2.num_XMax * MyVariable.area_QiangTou2.num_YMax;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区2搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 620, 610, 610);
                                    break;
                                case MemoryClass.NoOutArea.枪头区3:
                                    MyVariable.consumables_Empty[1] = false;
                                    MyVariable.sign_TIP3 = false;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64615, 0);
                                    }
                                    MyVariable.area_QiangTou3.num_Remain = MyVariable.area_QiangTou3.num_XMax * MyVariable.area_QiangTou3.num_YMax;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区3搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 620, 610, 610);
                                    break;
                                case MemoryClass.NoOutArea.枪头区4:
                                    MyVariable.consumables_Empty[2] = false;
                                    MyVariable.sign_TIP4 = false;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64614, 0);
                                    }
                                    MyVariable.area_QiangTou4.num_Remain = MyVariable.area_QiangTou4.num_XMax * MyVariable.area_QiangTou4.num_YMax;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区4搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 620, 610, 610);
                                    break;
                                case MemoryClass.NoOutArea.低温区:
                                    MyVariable.consumables_Empty[3] = false;
                                    MyVariable.sign_DiWen = false;
                                    //低温区补料完成,变量置为满料
                                    MyVariable.area_DiWen_FCF.num_Remain = MyVariable.FCF_MAX;
                                    MyVariable.area_DiWen_FCT.num_Remain = MyVariable.FCT_MAX;
                                    MyVariable.area_DiWen_SB.num_Remain = MyVariable.SB_MAX;
                                    MyVariable.area_DiWen_LIB.num_Remain = MyVariable.LIB_MAX;
                                    MyVariable.area_DiWen_DIL.num_Remain = MyVariable.DIL_MAX;
                                    MyVariable.area_DiWen_WMX.num_Remain = MyVariable.WMX_MAX;
                                    MyVariable.area_DiWen_S.num_Remain = MyVariable.S_MAX;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64622, 0);
                                    }
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.低温区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 640, 610, 610);
                                    break;
                                case MemoryClass.NoOutArea.常温试剂区:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.常温试剂区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 660, 610, 610);
                                    break;
                                case MemoryClass.NoOutArea.离心管试管区:
                                    MyVariable.consumables_Empty[4] = false;
                                    MyVariable.sign_LiXinGuan = false;
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64634, 0);
                                    }
                                    MyVariable.area_LiXinGuan.num_Remain = MyVariable.area_LiXinGuan.num_XMax * MyVariable.area_LiXinGuan.num_YMax;
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.离心管试管区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 680, 610, 610);
                                    break;
                                case MemoryClass.NoOutArea.八联排试管区:
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.八联排试管区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 700, 610, 610);
                                    break;
                            }
                            break;
                        case 620://到枪头区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 720, 620, 620);
                            break;
                        case 640://到低温区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.低温区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.低温区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.低温区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 720, 640, 640);
                            break;
                        case 660://到常温区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.常温试剂区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.常温试剂区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 720, 660, 660);
                            break;
                        case 680://到1.5离心管区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.离心管试管区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 720, 680, 680);
                            break;
                        case 700://到8连排试管区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.八联排试管区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 720, 700, 700);
                            break;
                        case 720://给PLC发送物料接收完成信号
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                if (!MyVariable.consumables_Empty[4] && !MyVariable.consumables_Empty[3] && !MyVariable.consumables_Empty[2] && !MyVariable.consumables_Empty[1] && !MyVariable.consumables_Empty[0])
                                {
                                    MyVariable.need_Completed = false;//物料补齐,信号标志给false
                                    LogConfig.Instance.ShowMessageToList("Run", "物料已补齐", MsgType.Success, Color.Green);
                                }
                                this.RunStep = 740;
                            }
                            else
                            {
                                Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 0);//清空载具号寄存器内容
                                Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 2);
                                if (SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.八联排试管区 || SerializeClass.mMemory.area == MemoryClass.Area.八联排试管区)
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                                }
                                LogConfig.Instance.ShowMessageToList("Run", "物料接收完成,64601地址写2", MsgType.Success, Color.Green);
                                this.RunStep = 730;
                            }
                            break;
                        case 730:
                            CheckCurrentRunStatus(0, 730, 730);
                            if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC1))
                            {
                                if (read_PLC1[0] == 0 || read_PLC1[0] == 1)
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                                    if (!MyVariable.consumables_Empty[4] && !MyVariable.consumables_Empty[3] && !MyVariable.consumables_Empty[2] && !MyVariable.consumables_Empty[1] && !MyVariable.consumables_Empty[0])
                                    {
                                        MyVariable.need_Completed = false;//物料补齐,信号标志给false
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 0);//耗材齐全,要料地址置0
                                        SerializeClass.animationParam.ground = (int)_groundEnum.无交互任务;
                                        LogConfig.Instance.ShowMessageToList("Run", "物料已补齐,64600地址写0", MsgType.Success, Color.Green);
                                    }
                                    this.RunStep = 740;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                throw new StationErrorException("通讯报警");
                            }
                            break;
                        case 740://发送夹爪松开指令
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawAbsMove(Program.carryClawConfigList[0], 3000))
                            {
                                MyVariable.feed_Completed = true;//给进料线程供料完成标志
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 770;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 770:
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪松开;
                            this.RunStep = 780;
                            break;
                        case 780:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 800, 780, 780);
                            break;
                        case 800:
                            if (SerializeClass.mMemory.area_noout == MemoryClass.NoOutArea.八联排试管区 || SerializeClass.mMemory.area == MemoryClass.Area.八联排试管区)
                            {
                                this.RunStep = 820;
                            }
                            else
                            {
                                this.RunStep = 840;
                            }
                            break;
                        case 820://与总控通讯,查询是否开始实验以及实验信息
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽总控...", MsgType.Success, Color.Blue);
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)))
                                {
                                    //单机跑测序流程,赋值
                                    SerializeClass.startParam_carryStation.protocol_group_id = ini.Read<string>("SequenceFormParam", "protocol_group_id");
                                    SerializeClass.startParam_carryStation.product_code = ini.Read<string>("SequenceFormParam", "product_code");
                                    SerializeClass.startParam_carryStation.sample_id = ini.Read<string>("SequenceFormParam", "sample_id");
                                    SerializeClass.startParam_carryStation.kit = ini.Read<string>("SequenceFormParam", "kit");
                                    SerializeClass.startParam_carryStation.speed = int.Parse(ini.Read<string>("SequenceFormParam", "speed"));
                                    SerializeClass.startParam_carryStation.min_read_length = int.Parse(ini.Read<string>("SequenceFormParam", "min_read_length"));
                                    SerializeClass.startParam_carryStation.guppy_filename = ini.Read<string>("SequenceFormParam", "guppy_filename");
                                    SerializeClass.startParam_carryStation.mux_scan_period = 1.5;
                                }
                                this.RunStep = 840;
                            }
                            else
                            {
                                TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).ClearNetData();
                                SerializeClass.mStartReportingToControl.sn = MyVariable.SN_CarryStation;
                                string jsonStr = JsonConvert.SerializeObject(SerializeClass.mStartReportingToControl);
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).WriteDataStr(jsonStr))
                                {
                                    LogToGeneral(jsonStr);//log
                                    this.time = this.GetCurveTime();
                                    WaitDelayTime(0.3);
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送实验请求", MsgType.Success, Color.Brown);
                                    SerializeClass.animationParam.general = (int)_generalEnum.请求开始实验;
                                    this.RunStep = 830;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送数据失败", MsgType.Error, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 830:
                            CheckCurrentRunStatus(0, 820, 820);
                            if (OverTimeS(time, Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据超时", MsgType.Error, Color.Red);
                                this.RunStep = 820;
                                throw new StationErrorException("通讯报警");
                            }
                            else
                            {
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).NetCanRead())
                                {
                                    TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).LoopReadData(1, out Program.ControlReceived, Encoding.UTF8);
                                    LogFromGeneral(Program.ControlReceived);//log
                                    b_carrystation = MyVariable.GeneralStartReceive(Program.ControlReceived,
                                          out code_general, out SerializeClass.startParam_carryStation.protocol_group_id, out SerializeClass.startParam_carryStation.product_code, out SerializeClass.startParam_carryStation.sample_id,
                                          out SerializeClass.startParam_carryStation.kit, out SerializeClass.startParam_carryStation.speed, out SerializeClass.startParam_carryStation.min_read_length,
                                          out SerializeClass.startParam_carryStation.guppy_filename, out SerializeClass.startParam_carryStation.mux_scan_period);
                                    SerializeClass.startParam_carryStation.guppy_filename = ini.Read<string>("SequenceFormParam", "guppy_filename");
                                    if (b_carrystation)
                                    {
                                        if (code_general == 200)
                                        {
                                            MyVariable.experiment_Arrive = true;
                                            LogConfig.Instance.ShowMessageToList("Run", "总控反馈：允许实验", MsgType.Success, Color.Green);
                                            this.RunStep = 840;
                                        }
                                        else if (MyVariable.newshow_IsOpen)
                                        {
                                            MyVariable.experiment_Arrive = true;
                                            LogConfig.Instance.ShowMessageToList("Run", "总控反馈：参观模式允许实验", MsgType.Success, Color.Green);
                                            this.RunStep = 840;
                                        }
                                        else
                                        {
                                            MyVariable.experiment_Arrive = false;
                                            LogConfig.Instance.ShowMessageToList("Run", "总控反馈：不进行实验", MsgType.Success, Color.Red);
                                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.过渡点;
                                            this.RunStep = 7000;
                                            throw new StationErrorException("实验流程报警");
                                        }
                                        SerializeClass.animationParam.general = (int)_generalEnum.无交互任务;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据异常", MsgType.Error, Color.Red);
                                        this.RunStep = 820;
                                        throw new StationErrorException("通讯报警");
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region 当前状态  供料完成
                        case 840:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.供料完成;
                            this.RunStep = 10;
                            break;
                        #endregion

                        #endregion


                        case 1000:
                            if (MyVariable.newshow_IsOpen)
                            {
                                this.RunStep = 8000;
                                break;
                            }
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序中
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育中)
                            {
                                this.RunStep = 10;
                            }
                            else if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序完成)
                            {
                                this.RunStep = 4000;
                            }
                            else if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育完成)
                            {
                                this.RunStep = 6200;
                            }
                            else
                            {
                                this.RunStep = 1010;
                            }
                            break;

                        #region 测序实验测序试剂配置流程

                        #region 当前状态  实验开始
                        case 1010:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.实验开始;
                            MyVariable.sign_zongkong = false;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 2020;
                                    break;
                                case MemoryClass.Pipette_gun_technology.未取枪头:
                                    LogConfig.Instance.ShowMessageToList("Run", "开始测序实验！", MsgType.Success, Color.DarkOrange);
                                    SerializeClass.animationParam.Result = "";
                                    this.RunStep = 1020;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号枪头:
                                    this.RunStep = 1080;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号试剂:
                                    this.RunStep = 1140;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号试剂:
                                    this.RunStep = 1200;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 1260;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 1320;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号试剂:
                                    this.RunStep = 1380;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号试剂:
                                    this.RunStep = 1520;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号枪头:
                                    this.RunStep = 1580;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取3号枪头:
                                    this.RunStep = 1640;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取3号试剂:
                                    this.RunStep = 1700;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排3号试剂:
                                    this.RunStep = 1840;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排3号枪头:
                                    this.RunStep = 1900;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取4号枪头:
                                    this.RunStep = 1960;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 1020://检查与测序仪通讯
                            LogConfig.Instance.ShowMessageToList("Run", "检查与测序仪通讯", MsgType.Success, Color.Brown);
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽测序仪", MsgType.Success, Color.Blue);
                                this.RunStep = 1060;
                            }
                            else
                            {
                                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceHandle.ToString()].CurrentValue)))
                                {
                                    this.RunStep = 1060;
                                    LogConfig.Instance.ShowMessageToList("Run", "手动使用测序仪", MsgType.Success, Color.Green);
                                    break;
                                }
                                b_carrystation = SequencingInterface.SequencingNoParam(SequencingInterface.sequencing_Connect, out cexuCode, out cexuMsg, out cexuState, out cexuCom22, out cexuCom9502);
                                if (b_carrystation)
                                {
                                    if (cexuCom22 == "0" && cexuCom9502 == "0")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "测序仪网络连接成功", MsgType.Success, Color.Green);
                                        this.RunStep = 1040;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "测序仪网络连接失败,检查网线", MsgType.Success, Color.Red);
                                        throw new StationErrorException("通讯报警");
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "接口查询异常,检查是否开启测序仪接口程序", MsgType.Success, Color.Red);
                                    throw new StationErrorException("测序仪报警");
                                }
                            }
                            break;
                        case 1040:
                            LogConfig.Instance.ShowMessageToList("Run", "检查测序仪是否插入芯片", MsgType.Success, Color.Brown);
                            b_carrystation = SequencingInterface.SequencingNoParam(SequencingInterface.sequencing_Chip, out cexuCode, out cexuMsg, out cexuState, out cexuCom22, out cexuCom9502);
                            if (b_carrystation)
                            {
                                if (cexuState == "0")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "测序仪已插入芯片", MsgType.Success, Color.Green);
                                    this.RunStep = 1060;
                                }
                                else if (cexuState == "1")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "测序仪未插入芯片", MsgType.Success, Color.Red);
                                    throw new StationErrorException("测序仪报警");
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "测序仪网络连接失败,检查网线", MsgType.Success, Color.Red);
                                    throw new StationErrorException("测序仪报警");
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "接口查询异常,检查是否开启测序仪接口程序", MsgType.Success, Color.Red);
                                throw new StationErrorException("测序仪报警");
                            }
                            break;
                        case 1060://取50ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取50ul枪头;
                            runRet = PickTip(1060, 1060, 1060, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 1080, 1060, 1060);
                            break;
                        case 1080:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号枪头;
                            this.RunStep = 1100;
                            break;
                        case 1100://升移液枪Z轴
                            CheckCurrentRunStatus(0, 1100, 1100);
                            runRet = UpGun(1100, 1100);
                            RunResultJudge(runRet, 1120, 1100, 1100);
                            break;
                        case 1120://取30ulFCT
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取33ulFCT试剂", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取30ulFCT试剂;
                            runRet = PickSolution(1120, 1120, 1120, _PointArray.低温区FCT取料位置.ToString(), 0, 0, MyVariable.FCT_volume + 300, MyVariable.gun_inliquid_speed, -1, MyVariable.z_DiWenFCT_pos);
                            if (runRet == _ActionResult.结果OK)
                            {
                                MyVariable.area_DiWen_FCT.num_Remain = MyVariable.area_DiWen_FCT.num_Remain - ((MyVariable.FCT_volume + 300) / 100);
                            }
                            RunResultJudge(runRet, 1140, 1120, 1120);
                            break;
                        case 1140:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号试剂;
                            this.RunStep = 1160;
                            break;
                        case 1160://上升移液枪Z轴
                            CheckCurrentRunStatus(0, 1160, 1160);
                            runRet = UpGun(1160, 1160);
                            RunResultJudge(runRet, 1180, 1160, 1160);
                            break;
                        case 1180://到当前实验第一个离心管排空FCT
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到1.5ml试管排30ul混合", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到离心管排液;
                            runRet = RemoveSolution(1180, 1180, 1180, _PointArray.离心管试管区取料位置.ToString(), MyVariable.area_LiXinGuan.num_X, MyVariable.area_LiXinGuan.num_Y, MyVariable.FCT_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan200_pos);
                            RunResultJudge(runRet, 1200, 1180, 1180);
                            break;
                        case 1200:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号试剂;
                            this.RunStep = 1220;
                            break;
                        case 1220://抬移液枪Z轴
                            CheckCurrentRunStatus(0, 1220, 1220);
                            runRet = UpGun(1220, 1220);
                            RunResultJudge(runRet, 1240, 1220, 1220);
                            break;
                        case 1240://到废料区下料
                            runRet = RemoveTip(1240, 1240, 1240, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 1260, 1240, 1240);
                            break;
                        case 1260:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 1280;
                            break;
                        case 1280://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 1280, 1280);
                            runRet = UpGun(1280, 1280);
                            RunResultJudge(runRet, 1300, 1280, 1280);
                            break;
                        case 1300://到1000ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(1300, 1300, 1300, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(1300, 1300, 1300, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 1320, 1300, 1300);
                            break;
                        case 1320:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 1340;
                            break;
                        case 1340://上升Z轴
                            CheckCurrentRunStatus(0, 1340, 1340);
                            runRet = UpGun(1340, 1340);
                            RunResultJudge(runRet, 1360, 1340, 1340);
                            break;
                        case 1360://(吸585ul FCF液体)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:第一次取585ulFCF试剂", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.第一次取585ulFCF试剂;
                            runRet = PickSolution(1360, 1360, 1360, _PointArray.低温区FCF取料位置.ToString(), 0, 0, MyVariable.FCF_volume, MyVariable.gun_inliquid_speed, -1, MyVariable.z_DiWenFCF_pos);
                            if (runRet == _ActionResult.结果OK)
                            {
                                MyVariable.area_DiWen_FCF.num_Remain = MyVariable.area_DiWen_FCF.num_Remain - (MyVariable.FCF_volume / 100);
                            }
                            RunResultJudge(runRet, 1380, 1360, 1360);
                            break;
                        case 1380:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号试剂;
                            this.RunStep = 1400;
                            break;
                        case 1400://上升Z轴
                            CheckCurrentRunStatus(0, 1400, 1400);
                            runRet = UpGun(1400, 1400);
                            RunResultJudge(runRet, 1420, 1400, 1400);
                            break;
                        case 1420://到当前实验第一个离心管
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到1.5ml试管混合", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到离心管排液;
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis] + (MyVariable.LiXinGuan_XShift * MyVariable.area_LiXinGuan.num_X);
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] + (MyVariable.LiXinGuan_YShift * MyVariable.area_LiXinGuan.num_Y);
                            SerializeClass.animationParam.material1 = (int)_PointArray.离心管试管区取料位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis]+(MyVariable.LiXinGuan_XShift * MyVariable.area_LiXinGuan.num_X),
                                                ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis]+(MyVariable.LiXinGuan_YShift * MyVariable.area_LiXinGuan.num_Y) },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 1440, 1420, 1420);
                            break;
                        case 1440://下Z轴指令
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp50000,30000];";
                                SerializeClass.animationParam.gunZMark = 50000;
                            }
                            else
                            {
                                pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}];41[Zp{MyVariable.z_LiXinGuan1000_pos},{MyVariable.z_check_speed}];";
                                SerializeClass.animationParam.gunZMark = MyVariable.z_LiXinGuan1000_pos;
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 1460, 1440, 1440);
                            break;
                        case 1460://等待反馈
                            runRet = PipetteZAxisReceive(1440, 1440, MyVariable.FunctionStep, MyVariable.FunctionStep);
                            RunResultJudge(runRet, 1480, 1440, 1440);
                            break;
                        case 1480://排液+吸打混匀(吸615ul液体,排液体)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:FCF混合液第一次吸打混匀", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.FCF混合液第一次吸打混匀;
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp60000,10000];";
                                SerializeClass.animationParam.gunZMark = 60000;
                            }
                            else
                            {
                                if (MyVariable.show_IsOpen)//不再展示吸打混匀,节省时间
                                {
                                    pipettegunCmd = $"1[Da{(MyVariable.FCF_volume - 500)},,{MyVariable.gun_outliquid_speed},];";
                                }
                                else
                                {
                                    pipettegunCmd = $"1[Da{(MyVariable.FCF_volume - 500)},,{MyVariable.gun_outliquid_speed},];1[Ia{(MyVariable.FCF_volume + MyVariable.FCT_volume - 2000)},{MyVariable.gun_inliquid_speed}];1[Da{(MyVariable.FCF_volume + MyVariable.FCT_volume - 2000)},,{MyVariable.gun_outliquid_speed},];";
                                }
                                //液面跟随
                                // pipettegunCmd = $"1[De{MyVariable.gun_outliquid_speed}];41[Zp{MyVariable.z_check_pos},{MyVariable.z_check_speed}]1[Ld0,10000];1[Iz{(MyVariable.FCF_volume + MyVariable.FCT_volume)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Dz{(MyVariable.FCF_volume + MyVariable.FCT_volume)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];";
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 1500, 1480, 1480);
                            break;
                        case 1500://等待反馈
                            runRet = PipetteGunReceive();
                            RunResultJudge(runRet, 1520, 1480, 1480);
                            break;
                        case 1520:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号试剂;
                            this.RunStep = 1540;
                            break;
                        case 1540://抬Z轴
                            CheckCurrentRunStatus(0, 1540, 1540);
                            runRet = UpGun(1540, 1540);
                            RunResultJudge(runRet, 1560, 1540, 1540);
                            break;
                        case 1560://到废料区下料
                            runRet = RemoveTip(1560, 1560, 1560, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 1580, 1560, 1560);
                            break;
                        case 1580:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号枪头;
                            this.RunStep = 1600;
                            break;
                        case 1600://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 1600, 1600);
                            runRet = UpGun(1600, 1600);
                            if (MyVariable.show_IsOpen)//参观模式,只取一次试剂
                            {
                                this.RunStep = 1940;
                            }
                            else
                            {
                                RunResultJudge(runRet, 1620, 1600, 1600);
                            }
                            break;
                        case 1620://到1000ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(1620, 1620, 1620, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(1620, 1620, 1620, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 1640, 1620, 1620);
                            break;
                        case 1640:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取3号枪头;
                            this.RunStep = 1660;
                            break;
                        case 1660://上升Z轴
                            CheckCurrentRunStatus(0, 1660, 1660);
                            runRet = UpGun(1660, 1660);
                            RunResultJudge(runRet, 1680, 1660, 1660);
                            break;
                        case 1680://取585ulFCF
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取585ulFCF试剂", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.第二次取585ulFCF试剂;
                            runRet = PickSolution(1680, 1680, 1680, _PointArray.低温区FCF取料位置.ToString(), 0, 0, MyVariable.FCF_volume, MyVariable.gun_inliquid_speed, -1, MyVariable.z_DiWenFCF_pos);
                            if (runRet == _ActionResult.结果OK)
                            {
                                MyVariable.area_DiWen_FCF.num_Remain = MyVariable.area_DiWen_FCF.num_Remain - (MyVariable.FCF_volume / 100);
                            }
                            RunResultJudge(runRet, 1700, 1680, 1680);
                            break;
                        case 1700:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取3号试剂;
                            this.RunStep = 1720;
                            break;
                        case 1720://上升Z轴
                            CheckCurrentRunStatus(0, 1720, 1720);
                            runRet = UpGun(1720, 1720);
                            RunResultJudge(runRet, 1740, 1720, 1720);
                            break;
                        case 1740://到当前实验第一个离心管位置
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到1.5ml试管混合", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到离心管排液;
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis] + (MyVariable.LiXinGuan_XShift * MyVariable.area_LiXinGuan.num_X);
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] + (MyVariable.LiXinGuan_YShift * MyVariable.area_LiXinGuan.num_Y);
                            SerializeClass.animationParam.material1 = (int)_PointArray.离心管试管区取料位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis]+(MyVariable.LiXinGuan_XShift * MyVariable.area_LiXinGuan.num_X),
                                                ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis]+(MyVariable.LiXinGuan_YShift * MyVariable.area_LiXinGuan.num_Y) },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 1760, 1740, 1740);
                            break;
                        case 1760://下Z轴指令
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp50000,30000];";
                                SerializeClass.animationParam.gunZMark = 50000;
                            }
                            else
                            {
                                pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}];41[Zp{(MyVariable.z_LiXinGuan1000_pos - 10000)},{MyVariable.z_check_speed}];";
                                SerializeClass.animationParam.gunZMark = (MyVariable.z_LiXinGuan1000_pos - 10000);
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 1780, 1760, 1760);
                            break;
                        case 1780://等待反馈
                            runRet = PipetteZAxisReceive(1760, 1760, MyVariable.FunctionStep, MyVariable.FunctionStep);
                            RunResultJudge(runRet, 1800, 1760, 1760);
                            this.RunStep = 1800;
                            break;
                        case 1800://排液+吸打混匀2次(吸900ul液体,排液体)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取900ulFCF混合液第二次吸打混匀", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取900ulFCF混合液第二次吸打混匀;
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp60000,10000];";
                                SerializeClass.animationParam.gunZMark = 60000;
                            }
                            else
                            {
                                if (MyVariable.show_IsOpen)//不再展示吸打混匀,节省时间
                                {
                                    pipettegunCmd = $"1[Da{MyVariable.FCF_volume - 500},,{MyVariable.gun_outliquid_speed},];";
                                }
                                else
                                {
                                    pipettegunCmd = $"1[Da{MyVariable.FCF_volume - 500},,{MyVariable.gun_outliquid_speed},];1[Ia90000,{MyVariable.gun_inliquid_speed}];1[Da90000,,{MyVariable.gun_outliquid_speed},];1[Ia90000,{MyVariable.gun_inliquid_speed}];1[Da90000,,{MyVariable.gun_outliquid_speed},];";
                                }
                                // pipettegunCmd = $"1[De{MyVariable.gun_outliquid_speed}];41[Zp{MyVariable.z_check_pos},{MyVariable.z_check_speed}]1[Ld0,10000];1[Iz90000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Dz90000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Iz90000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Dz90000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];";
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 1820, 1800, 1800);
                            this.RunStep = 1820;
                            break;
                        case 1820://等待反馈
                            runRet = PipetteGunReceive();
                            RunResultJudge(runRet, 1840, 1800, 1800);
                            break;
                        case 1840:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排3号试剂;
                            this.RunStep = 1860;
                            break;
                        case 1860://抬Z轴
                            CheckCurrentRunStatus(0, 1860, 1860);
                            runRet = UpGun(1860, 1860);
                            RunResultJudge(runRet, 1880, 1860, 1860);
                            break;
                        case 1880://到废料区下料
                            runRet = RemoveTip(1880, 1880, 1880, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 1900, 1880, 1880);
                            break;
                        case 1900:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排3号枪头;
                            this.RunStep = 1920;
                            break;
                        case 1920://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 1920, 1920);
                            runRet = UpGun(1920, 1920);
                            RunResultJudge(runRet, 1940, 1920, 1920);
                            break;
                        case 1940://到100ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(1940, 1940, 1940, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(1940, 1940, 1940, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            //   LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            //   runRet = PickTip(1940, 1940, 1940, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            //   MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 1960, 1940, 1940);
                            break;
                        case 1960:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取4号枪头;
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            this.RunStep = 1980;
                            break;
                        case 1980://上升Z轴
                            CheckCurrentRunStatus(0, 1980, 1980);
                            runRet = UpGun(1980, 1980);
                            RunResultJudge(runRet, 2000, 1980, 1980);
                            break;
                        case 2000:
                            CheckCurrentRunStatus(0, 2000, 2000);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.开盖完成)
                            {
                                SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                                this.RunStep = 2020;
                            }
                            break;
                        #endregion

                        #region 当前状态  开始步骤一
                        case 2020:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.开始步骤一;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 2040;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号试剂:
                                    this.RunStep = 2060;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 2120;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 2180;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号试剂:
                                    this.RunStep = 2320;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号试剂:
                                    this.RunStep = 2380;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 2040://到测序仪排气泡
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:芯片排气泡", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.芯片排气泡;
                            runRet = PickSolution(2040, 2040, 2040, _PointArray.预处理孔位置.ToString(), 0, 0, MyVariable.Bubble_Out, MyVariable.gun_outliquid_xinpian, -1, MyVariable.z_YuChuLiKong_pos);
                            RunResultJudge(runRet, 2060, 2040, 2040);
                            break;
                        case 2060:
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.芯片排气泡;
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号试剂;
                            this.RunStep = 2080;
                            break;
                        case 2080://抬Z轴
                            CheckCurrentRunStatus(0, 2080, 2080);
                            runRet = UpGun(2080, 2080);
                            RunResultJudge(runRet, 2090, 2080, 2080);
                            break;
                        case 2090:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.废料区1下料位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                       new double[] { ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                       Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 2095, 2090, 2090);
                            break;
                        case 2095://判断上一次排气泡是否完成
                            if (MyVariable.CCD_QiPao)
                            {
                                SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.实验开始;
                                this.RunStep = 1960;
                            }
                            else
                            {
                                this.RunStep = 2100;
                            }
                            break;
                        case 2100://下枪头
                            runRet = RemoveTip(2100, 2100, 2100, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 2120, 2100, 2100);
                            break;
                        case 2120:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 2140;
                            break;
                        case 2140://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 2140, 2140);
                            runRet = UpGun(2140, 2140);
                            RunResultJudge(runRet, 2160, 2140, 2140);
                            break;
                        case 2160://到1000ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(2160, 2160, 2160, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(2160, 2160, 2160, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 2180, 2160, 2160);
                            break;
                        case 2180:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 2200;
                            break;
                        case 2200://上升Z轴
                            CheckCurrentRunStatus(0, 2200, 2200);
                            runRet = UpGun(2200, 2200);
                            RunResultJudge(runRet, 2220, 2200, 2200);
                            break;
                        case 2220://到当前实验第一个离心管
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis] + (MyVariable.LiXinGuan_XShift * MyVariable.area_LiXinGuan.num_X);
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] + (MyVariable.LiXinGuan_YShift * MyVariable.area_LiXinGuan.num_Y);
                            SerializeClass.animationParam.material1 = (int)_PointArray.离心管试管区取料位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis]+(MyVariable.LiXinGuan_XShift * MyVariable.area_LiXinGuan.num_X),
                                                ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis]+(MyVariable.LiXinGuan_YShift * MyVariable.area_LiXinGuan.num_Y) },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 2240, 2220, 2220);
                            break;
                        case 2240://与移液枪通讯,移液枪下探到一定位置并吸取30ul空气,再转成液面探测
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp50000,30000];";
                                SerializeClass.animationParam.gunZMark = 50000;
                            }
                            else
                            {
                                if (MyVariable.show_IsOpen)//参观模式
                                {
                                    pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}]1[Ia2500,500];41[Zp{(MyVariable.z_LiXinGuan1000_pos)},{MyVariable.z_check_speed}];";
                                    SerializeClass.animationParam.gunZMark = (MyVariable.z_LiXinGuan1000_pos);
                                }
                                else
                                {
                                    pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}]1[Ia2500,500];41[Zp{(MyVariable.z_LiXinGuan1000_pos - 10000)},{MyVariable.z_check_speed}];";
                                    SerializeClass.animationParam.gunZMark = (MyVariable.z_LiXinGuan1000_pos - 10000);
                                }
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                                // pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}]1[Ia2500,500];41[Zp{MyVariable.z_check_pos},{MyVariable.z_check_speed}]1[Ld0,10000];";
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 2260, 2240, 2240);
                            break;
                        case 2260://等待反馈
                            runRet = PipetteZAxisReceive(2240, 2240, MyVariable.FunctionStep, MyVariable.FunctionStep);
                            RunResultJudge(runRet, 2280, 2240, 2240);
                            break;
                        case 2280://移液枪液面跟随吸液(吸900ul FCF混合液)+排出+吸液(吸900ul FCF混合液)+排出+吸液(吸800ul FCF混合液)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:FCF混合试剂吸取800ul", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.FCF混合试剂吸取800ul;
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp60000,10000];";
                                SerializeClass.animationParam.gunZMark = 60000;
                            }
                            else
                            {
                                pipettegunCmd = $"1[Ia{(MyVariable.FCFmix_volume1 + 300)},{MyVariable.gun_inliquid_speed}];";

                                // pipettegunCmd = $"1[Ia93000,{MyVariable.gun_inliquid_speed}];1[Da90000,,{MyVariable.gun_outliquid_speed},];1[Ia90000,{MyVariable.gun_inliquid_speed}];1[Da90000,,{MyVariable.gun_outliquid_speed},];1[Ia{MyVariable.FCFmix_volume1},{MyVariable.gun_inliquid_speed}];";

                                //pipettegunCmd = $"1[Iz90000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Dz90000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Iz90000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Dz90000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Iz{MyVariable.FCFmix_volume1},{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];";
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 2300, 2280, 2280);
                            this.RunStep = 2300;
                            break;
                        case 2300://等待反馈
                            runRet = PipetteGunReceive();
                            RunResultJudge(runRet, 2320, 2280, 2280);
                            break;
                        case 2320:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号试剂;
                            this.RunStep = 2340;
                            break;
                        case 2340://上升Z轴
                            CheckCurrentRunStatus(0, 2340, 2340);
                            runRet = UpGun(2340, 2340);
                            RunResultJudge(runRet, 2360, 2340, 2340);
                            break;
                        case 2360://到预处理孔排入720ulFCF混合液
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到预处理孔排入720ulFCF混合液", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到预处理孔排入720ulFCF混合液;
                            runRet = RemoveSolution(2360, 2360, 2360, _PointArray.预处理孔位置.ToString(), 0, 0, MyVariable.FCFmix_volumeOut1, MyVariable.gun_outliquid_xinpian, false, MyVariable.z_YuChuLiKong_pos);
                            RunResultJudge(runRet, 2380, 2360, 2360);
                            break;
                        case 2380:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号试剂;
                            this.RunStep = 2400;
                            break;
                        case 2400://抬Z轴
                            CheckCurrentRunStatus(0, 2400, 2400);
                            runRet = UpGun(2400, 2400);
                            RunResultJudge(runRet, 2420, 2400, 2400);
                            break;
                        case 2420:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                            this.RunStep = 2440;
                            break;
                        #endregion

                        #region 当前状态  步骤一完成
                        case 2440:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.步骤一完成;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 2460;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 2480;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 2540;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号试剂:
                                    this.RunStep = 2600;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号试剂:
                                    this.RunStep = 2660;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号枪头:
                                    this.RunStep = 2720;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取3号枪头:
                                    this.RunStep = 2780;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取3号试剂:
                                    this.RunStep = 2920;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排3号试剂:
                                    this.RunStep = 2980;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排3号枪头:
                                    this.RunStep = 3040;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取4号枪头:
                                    this.RunStep = 3100;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取4号试剂:
                                    this.RunStep = 3160;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排4号试剂:
                                    this.RunStep = 3220;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 2460://到废料区下料
                            runRet = RemoveTip(2460, 2460, 2460, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 2480, 2460, 2460);
                            break;
                        case 2480:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 2500;
                            break;
                        case 2500://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 2500, 2500);
                            runRet = UpGun(2500, 2500);
                            RunResultJudge(runRet, 2520, 2500, 2500);
                            break;
                        case 2520://取50ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取50ul枪头;
                            runRet = PickTip(2520, 2520, 2520, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 2540, 2520, 2520);
                            break;
                        case 2540:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 2560;
                            break;
                        case 2560://升移液枪Z轴
                            CheckCurrentRunStatus(0, 2560, 2560);
                            runRet = UpGun(2560, 2560);
                            RunResultJudge(runRet, 2580, 2560, 2560);
                            break;
                        case 2580://取37.5ulSB试剂
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取40ul SB试剂", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取SB试剂;
                            runRet = PickSolution(2580, 2580, 2580, _PointArray.低温区SB取料位置.ToString(), 0, 0, MyVariable.SB_volume + 250, MyVariable.gun_inliquid_speed, -1, MyVariable.z_DiWenSB_pos);
                            if (runRet == _ActionResult.结果OK)
                            {
                                MyVariable.area_DiWen_SB.num_Remain = MyVariable.area_DiWen_SB.num_Remain - ((MyVariable.SB_volume + 250) / 100);
                            }
                            RunResultJudge(runRet, 2600, 2580, 2580);
                            break;
                        case 2600://状态标记
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号试剂;
                            this.RunStep = 2620;
                            break;
                        case 2620://上升移液枪Z轴
                            CheckCurrentRunStatus(0, 2620, 2620);
                            runRet = UpGun(2620, 2620);
                            RunResultJudge(runRet, 2640, 2620, 2620);
                            break;
                        case 2640://到当前实验第二个离心管
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到1.5ml试管排37.5ul混合", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到离心管排液;
                            if (MyVariable.area_LiXinGuan.num_X + 1 == 3)
                            {
                                runRet = RemoveSolution(2640, 2640, 2640, _PointArray.离心管试管区取料位置.ToString(), 0, MyVariable.area_LiXinGuan.num_Y + 1, MyVariable.SB_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan200_pos);
                            }
                            else
                            {
                                runRet = RemoveSolution(2640, 2640, 2640, _PointArray.离心管试管区取料位置.ToString(), MyVariable.area_LiXinGuan.num_X + 1, MyVariable.area_LiXinGuan.num_Y, MyVariable.SB_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan200_pos);
                            }
                            RunResultJudge(runRet, 2660, 2640, 2640);
                            break;
                        case 2660:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号试剂;
                            this.RunStep = 2680;
                            break;
                        case 2680://抬移液枪Z轴
                            CheckCurrentRunStatus(0, 2680, 2680);
                            runRet = UpGun(2680, 2680);
                            RunResultJudge(runRet, 2700, 2680, 2680);
                            break;
                        case 2700://到废料区下料
                            runRet = RemoveTip(2700, 2700, 2700, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 2720, 2700, 2700);
                            break;
                        case 2720:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号枪头;
                            this.RunStep = 2740;
                            break;
                        case 2740://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 2740, 2740);
                            runRet = UpGun(2740, 2740);
                            if (MyVariable.show_IsOpen)
                            {
                                this.RunStep = 3080;//参观模式,跳过吸取LIB试剂
                            }
                            else
                            {
                                RunResultJudge(runRet, 2760, 2740, 2740);
                            }
                            break;
                        case 2760://取50ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取50ul枪头;
                            runRet = PickTip(2760, 2760, 2760, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 2780, 2760, 2760);
                            break;
                        case 2780:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取3号枪头;
                            this.RunStep = 2800;
                            break;
                        case 2800://上升Z轴
                            CheckCurrentRunStatus(0, 2800, 2800);
                            runRet = UpGun(2800, 2800);
                            RunResultJudge(runRet, 2820, 2800, 2800);
                            break;
                        case 2820:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区LIB取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区LIB取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.低温区LIB取料位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.低温区LIB取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.低温区LIB取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 2840, 2820, 2820);
                            break;
                        case 2840://与移液枪通讯,移液枪下探到一定位置并吸取30ul空气,再转成液面探测
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp50000,30000];";
                                SerializeClass.animationParam.gunZMark = 50000;
                            }
                            else
                            {
                                // pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}]1[Ia2500,500];41[Zp{MyVariable.z_check_pos},{MyVariable.z_check_speed}]1[Ld0,10000];";
                                pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}]1[Ia2500,500];41[Zp{MyVariable.z_DiWenLIB_pos},{MyVariable.z_check_speed}];";
                                SerializeClass.animationParam.gunZMark = MyVariable.z_DiWenLIB_pos;
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 2860, 2840, 2840);
                            break;
                        case 2860://等待反馈
                            runRet = PipetteZAxisReceive(2840, 2840, MyVariable.FunctionStep, MyVariable.FunctionStep);
                            RunResultJudge(runRet, 2880, 2840, 2840);
                            break;
                        case 2880://移液枪液面跟随吸液(吸30ul LIB液体)+排30ul+吸30ul+排30ul+吸25.5ul(吸打混匀后吸取25.5ul)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸打混匀LIB试剂并吸取28ul", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.吸打混匀LIB试剂并吸取;
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp60000,10000];";
                                SerializeClass.animationParam.gunZMark = 60000;
                            }
                            else
                            {
                                //if (MyVariable.area_DiWen_LIB.num_Remain >= 800)
                                //{
                                //    //    pipettegunCmd = $"1[Iz80000,{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];1[Dz79750,{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];1[Iz79750,{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];1[Dz79750,{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];1[Iz{MyVariable.LIB_volume},{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];";
                                //    pipettegunCmd = $"1[Ia80000,{MyVariable.gun_inliquid_speed}];1[Da79750,,{MyVariable.gun_outliquid_speed},];1[Ia79750,{MyVariable.gun_inliquid_speed}];1[Da79750,,{MyVariable.gun_outliquid_speed},];1[Ia{MyVariable.LIB_volume},{MyVariable.gun_inliquid_speed}];";//吸液,不需要液面跟随
                                //}
                                //else
                                //{
                                //    pipettegunCmd = $"1[Iz{(MyVariable.area_DiWen_LIB.num_Remain * 100)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];1[Dz{(MyVariable.area_DiWen_LIB.num_Remain * 100 - 250)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];1[Iz{(MyVariable.area_DiWen_LIB.num_Remain * 100 - 250)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];1[Dz{(MyVariable.area_DiWen_LIB.num_Remain * 100 - 250)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];1[Iz{MyVariable.LIB_volume},{MyVariable.gun_inliquid_speed},{MyVariable.surface_DiWen}];";
                                pipettegunCmd = $"1[Ia{(MyVariable.LIB_volume + 250)},{MyVariable.gun_inliquid_speed}];1[Da{MyVariable.LIB_volume},,{MyVariable.gun_outliquid_speed},];1[Ia{MyVariable.LIB_volume},{MyVariable.gun_inliquid_speed}];1[Da{MyVariable.LIB_volume},,{MyVariable.gun_outliquid_speed},];1[Ia{MyVariable.LIB_volume},{MyVariable.gun_inliquid_speed}];";//吸液,不需要液面跟随
                                //}
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            if (runRet == _ActionResult.结果OK)
                            {
                                MyVariable.area_DiWen_LIB.num_Remain = MyVariable.area_DiWen_LIB.num_Remain - ((MyVariable.LIB_volume + 250) / 100);
                            }
                            RunResultJudge(runRet, 2900, 2880, 2880);
                            this.RunStep = 2900;
                            break;
                        case 2900://等待反馈
                            runRet = PipetteGunReceive();
                            RunResultJudge(runRet, 2920, 2880, 2880);
                            break;
                        case 2920:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取3号试剂;
                            this.RunStep = 2940;
                            break;
                        case 2940://上升Z轴
                            CheckCurrentRunStatus(0, 2940, 2940);
                            runRet = UpGun(2940, 2940);
                            RunResultJudge(runRet, 2960, 2940, 2940);
                            break;
                        case 2960://到当前实验第二个离心管
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到1.5ml试管排25.5ul混合", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到离心管排液;
                            if (MyVariable.area_LiXinGuan.num_X + 1 == 3)
                            {
                                runRet = RemoveSolution(2960, 2960, 2960, _PointArray.离心管试管区取料位置.ToString(), 0, MyVariable.area_LiXinGuan.num_Y + 1, MyVariable.LIB_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan200_pos);
                            }
                            else
                            {
                                runRet = RemoveSolution(2960, 2960, 2960, _PointArray.离心管试管区取料位置.ToString(), MyVariable.area_LiXinGuan.num_X + 1, MyVariable.area_LiXinGuan.num_Y, MyVariable.LIB_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan200_pos);
                            }
                            RunResultJudge(runRet, 2980, 2960, 2960);
                            break;
                        case 2980:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排3号试剂;
                            this.RunStep = 3000;
                            break;
                        case 3000://抬Z轴
                            CheckCurrentRunStatus(0, 3000, 3000);
                            runRet = UpGun(3000, 3000);
                            RunResultJudge(runRet, 3020, 3000, 3000);
                            break;
                        case 3020://到废料区下料
                            runRet = RemoveTip(3020, 3020, 3020, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 3040, 3020, 3020);
                            break;
                        case 3040:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排3号枪头;
                            this.RunStep = 3060;
                            break;
                        case 3060://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 3060, 3060);
                            runRet = UpGun(3060, 3060);
                            RunResultJudge(runRet, 3080, 3060, 3060);
                            break;
                        case 3080://取50ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取50ul枪头;
                            runRet = PickTip(3080, 3080, 3080, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 3100, 3080, 3080);
                            break;
                        case 3100:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取4号枪头;
                            this.RunStep = 3120;
                            break;
                        case 3120://上升Z轴
                            CheckCurrentRunStatus(0, 3120, 3120);
                            runRet = UpGun(3120, 3120);
                            RunResultJudge(runRet, 3140, 3120, 3120);
                            break;
                        case 3140://到8联排试管区取DNA样本
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取12ulDNA文库样本", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取12ulDNA文库样本;
                            runRet = PickSolution(3140, 3140, 3140, _PointArray.八联排DNA样本取料位置.ToString(), 0, 0, MyVariable.DNA_volume, MyVariable.gun_inliquid_speed, -1, MyVariable.z_BaLianPai50_pos);
                            RunResultJudge(runRet, 3160, 3140, 3140);
                            break;
                        case 3160:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取4号试剂;
                            this.RunStep = 3180;
                            break;
                        case 3180://上升Z轴
                            CheckCurrentRunStatus(0, 3180, 3180);
                            runRet = UpGun(3180, 3180);
                            RunResultJudge(runRet, 3200, 3180, 3180);
                            break;
                        case 3200://到当前实验第二个离心管
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到1.5ml试管排12ul混合", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到离心管排液;
                            if (MyVariable.area_LiXinGuan.num_X + 1 == 3)
                            {
                                runRet = RemoveSolution(3200, 3200, 3200, _PointArray.离心管试管区取料位置.ToString(), 0, MyVariable.area_LiXinGuan.num_Y + 1, MyVariable.DNA_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan200_pos);
                            }
                            else
                            {
                                runRet = RemoveSolution(3200, 3200, 3200, _PointArray.离心管试管区取料位置.ToString(), MyVariable.area_LiXinGuan.num_X + 1, MyVariable.area_LiXinGuan.num_Y, MyVariable.DNA_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan200_pos);
                            }
                            RunResultJudge(runRet, 3220, 3200, 3200);
                            break;
                        case 3220:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排4号试剂;
                            this.RunStep = 3260;
                            break;
                        case 3260://抬Z轴
                            CheckCurrentRunStatus(0, 3260, 3260);
                            runRet = UpGun(3260, 3260);
                            RunResultJudge(runRet, 3280, 3260, 3260);
                            break;
                        case 3280:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                            this.RunStep = 3300;
                            break;
                        #endregion

                        #region 当前状态  DNA文库配置完成
                        case 3300:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.DNA文库配置完成;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 3320;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 3340;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 3320://到废料区下料
                            runRet = RemoveTip(3320, 3320, 3320, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 3340, 3320, 3320);
                            break;
                        case 3340:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 3360;
                            break;
                        case 3360://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 3360, 3360);
                            runRet = UpGun(3360, 3360);
                            RunResultJudge(runRet, 3370, 3360, 3360);
                            break;
                        case 3370:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.待机位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 3375, 3370, 3370);
                            break;
                        case 3375:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            this.RunStep = 3380;
                            break;
                        case 3380:
                            CheckCurrentRunStatus(0, 3380, 3380);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.开盖完成)
                            {
                                SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                                this.RunStep = 3400;
                            }
                            break;
                        #endregion

                        #region 当前状态  开始步骤二
                        case 3400:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.开始步骤二;
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 3420;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号枪头:
                                    this.RunStep = 3440;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号试剂:
                                    this.RunStep = 3500;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号试剂:
                                    this.RunStep = 3560;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 3620;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 3680;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号试剂:
                                    this.RunStep = 3740;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号试剂:
                                    this.RunStep = 3800;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 3420://取1000ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(3420, 3420, 3420, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(3420, 3420, 3420, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 3440, 3420, 3420);
                            break;
                        case 3440:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号枪头;
                            this.RunStep = 3460;
                            break;
                        case 3460://升移液枪Z轴
                            CheckCurrentRunStatus(0, 3460, 3460);
                            runRet = UpGun(3460, 3460);
                            RunResultJudge(runRet, 3480, 3460, 3460);
                            break;
                        case 3480://到当前实验第一个离心管区取FCF混合液(240ul)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取240ulFCF混合液", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取240ulFCF混合液;
                            runRet = PickSolution(3480, 3480, 3480, _PointArray.离心管试管区取料位置.ToString(), MyVariable.area_LiXinGuan.num_X, MyVariable.area_LiXinGuan.num_Y, MyVariable.FCFmix_volume2, MyVariable.gun_inliquid_speed, MyVariable.surface_LiXinGuan, MyVariable.z_LiXinGuan1000_pos);
                            RunResultJudge(runRet, 3500, 3480, 3480);
                            break;
                        case 3500://状态标记
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号试剂;
                            this.RunStep = 3520;
                            break;
                        case 3520://上升移液枪Z轴
                            CheckCurrentRunStatus(0, 3520, 3520);
                            runRet = UpGun(3520, 3520);
                            RunResultJudge(runRet, 3540, 3520, 3520);
                            break;
                        case 3540://快速打入预处理孔
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:快速打入芯片预处理孔", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.快速打入芯片预处理孔;
                            runRet = RemoveSolution(3540, 3540, 3540, _PointArray.预处理孔位置.ToString(), 0, 0, MyVariable.FCFmix_volumeOut2, MyVariable.gun_outliquid_fastspeed, false, MyVariable.z_YuChuLiKong_pos);
                            RunResultJudge(runRet, 3560, 3540, 3540);
                            break;
                        case 3560:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号试剂;
                            this.RunStep = 3580;
                            break;
                        case 3580://抬移液枪Z轴
                            CheckCurrentRunStatus(0, 3580, 3580);
                            runRet = UpGun(3580, 3580);
                            RunResultJudge(runRet, 3600, 3580, 3580);
                            break;
                        case 3600://到废料区下料
                            runRet = RemoveTip(3600, 3600, 3600, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 3620, 3600, 3600);
                            break;
                        case 3620:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 3640;
                            break;
                        case 3640://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 3640, 3640);
                            runRet = UpGun(3640, 3640);
                            RunResultJudge(runRet, 3660, 3640, 3640);
                            break;
                        case 3660://到200ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取200ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取200ul枪头;
                            runRet = PickTip(3660, 3660, 3660, _PointArray.枪头区3取料位置.ToString(), MyVariable.area_QiangTou3.num_X, MyVariable.area_QiangTou3.num_Y);
                            MyVariable.area_QiangTou3.num_Remain--;
                            RunResultJudge(runRet, 3680, 3660, 3660);
                            break;
                        case 3680:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 3700;
                            break;
                        case 3700://上升Z轴
                            CheckCurrentRunStatus(0, 3700, 3700);
                            runRet = UpGun(3700, 3700);
                            RunResultJudge(runRet, 3720, 3700, 3700);
                            break;
                        case 3720://到当前实验第二个离心管
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取75ulDNA文库", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取75ulDNA文库;
                            if (MyVariable.area_LiXinGuan.num_X + 1 == 3)
                            {
                                runRet = PickSolution(3720, 3720, 3720, _PointArray.离心管试管区取料位置.ToString(), 0, MyVariable.area_LiXinGuan.num_Y + 1, (MyVariable.DNA_volume + MyVariable.SB_volume + MyVariable.LIB_volume), MyVariable.gun_inliquid_speed, MyVariable.surface_LiXinGuan, MyVariable.z_LiXinGuan200_pos);
                            }
                            else
                            {
                                runRet = PickSolution(3720, 3720, 3720, _PointArray.离心管试管区取料位置.ToString(), MyVariable.area_LiXinGuan.num_X + 1, MyVariable.area_LiXinGuan.num_Y, (MyVariable.DNA_volume + MyVariable.SB_volume + MyVariable.LIB_volume), MyVariable.gun_inliquid_speed, MyVariable.surface_LiXinGuan, MyVariable.z_LiXinGuan200_pos);
                            }
                            MyVariable.area_LiXinGuan.num_Remain = MyVariable.area_LiXinGuan.num_Remain - 2;//测序实验离心管使用结束,数量-2
                            RunResultJudge(runRet, 3740, 3720, 3720);
                            break;
                        case 3740:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号试剂;
                            this.RunStep = 3760;
                            break;
                        case 3760://上升Z轴
                            CheckCurrentRunStatus(0, 3760, 3760);
                            runRet = UpGun(3760, 3760);
                            RunResultJudge(runRet, 3780, 3760, 3760);
                            break;
                        case 3780://到上样孔逐滴加入
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:逐滴加入芯片上样孔", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.逐滴加入芯片上样孔;
                            runRet = RemoveSolution(3780, 3780, 3780, _PointArray.上样孔位置.ToString(), 0, 0, -1, MyVariable.gun_outliquid_slowspeed, false, MyVariable.z_ShangYangKong_pos);
                            RunResultJudge(runRet, 3800, 3780, 3780);
                            break;
                        case 3800:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号试剂;
                            this.RunStep = 3820;
                            break;
                        case 3820://抬Z轴
                            CheckCurrentRunStatus(0, 3820, 3820);
                            runRet = UpGun(3820, 3820);
                            RunResultJudge(runRet, 3840, 3820, 3820);
                            break;
                        case 3840:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                            this.RunStep = 3860;
                            break;
                        #endregion

                        #region 当前状态  测序配置完成
                        case 3860:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.测序配置完成;
                            MyVariable.show_memory = 0;//线程记忆点,用于带记忆复位
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 3880;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 3900;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 3880://到废料区下料
                            runRet = RemoveTip(3880, 3880, 3880, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 3900, 3880, 3880);
                            break;
                        case 3900:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 3920;
                            break;
                        case 3920://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 3920, 3920);
                            runRet = UpGun(3920, 3920);
                            RunResultJudge(runRet, 3930, 3920, 3920);
                            break;
                        case 3930:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.待机位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 3935, 3930, 3930);
                            break;
                        case 3935:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            this.RunStep = 3940;
                            break;
                        case 3940:
                            CheckCurrentRunStatus(0, 3940, 3940);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序中)
                            {
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #endregion

                        #region 测序实验清洗试剂配置流程

                        #region 当前状态  清洗
                        case 4000:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.清洗;
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.未取枪头:
                                    LogConfig.Instance.ShowMessageToList("Run", "开始芯片清洗实验！", MsgType.Success, Color.DarkOrange);
                                    this.RunStep = 4020;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号枪头:
                                    this.RunStep = 4030;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号试剂:
                                    this.RunStep = 4060;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号试剂:
                                    this.RunStep = 4090;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 4120;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 4150;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号试剂:
                                    this.RunStep = 4180;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号试剂:
                                    this.RunStep = 4210;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号枪头:
                                    this.RunStep = 4240;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取3号枪头:
                                    this.RunStep = 4600;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取3号试剂:
                                    this.RunStep = 4660;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排3号枪头:
                                    this.RunStep = 4720;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取4号枪头:
                                    this.RunStep = 4780;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取4号试剂:
                                    this.RunStep = 4840;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 4020://到1000ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(4020, 4020, 4020, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(4020, 4020, 4020, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 4030, 4020, 4020);
                            break;
                        case 4030:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号枪头;
                            this.RunStep = 4040;
                            break;
                        case 4040:
                            CheckCurrentRunStatus(0, 4040, 4040);
                            runRet = UpGun(4040, 4040);
                            RunResultJudge(runRet, 4050, 4040, 4040);
                            break;
                        case 4050://(吸398ul DIL液体)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸取398ulDIL试剂", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取398ulDIL试剂;
                            runRet = PickSolution(4050, 4050, 4050, _PointArray.低温区DIL取料位置.ToString(), 0, 0, MyVariable.DIL_volume, MyVariable.gun_inliquid_speed, -1, MyVariable.z_DiWenDIL_pos);
                            if (runRet == _ActionResult.结果OK)
                            {
                                MyVariable.area_DiWen_DIL.num_Remain = MyVariable.area_DiWen_DIL.num_Remain - (MyVariable.DIL_volume / 100);
                            }
                            RunResultJudge(runRet, 4060, 4050, 4050);
                            break;
                        case 4060:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号试剂;
                            this.RunStep = 4070;
                            break;
                        case 4070://上升Z轴
                            CheckCurrentRunStatus(0, 4070, 4070);
                            runRet = UpGun(4070, 4070);
                            RunResultJudge(runRet, 4080, 4070, 4070);
                            break;
                        case 4080:
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到1.5ml试管排398ulDIL试剂", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到离心管排液;
                            runRet = RemoveSolution(4080, 4080, 4080, _PointArray.离心管试管区取料位置.ToString(), MyVariable.area_LiXinGuan.num_X, MyVariable.area_LiXinGuan.num_Y, MyVariable.DIL_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan1000_pos);
                            RunResultJudge(runRet, 4090, 4080, 4080);
                            break;
                        case 4090:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号试剂;
                            this.RunStep = 4100;
                            break;
                        case 4100://抬移液枪Z轴
                            CheckCurrentRunStatus(0, 4100, 4100);
                            runRet = UpGun(4100, 4100);
                            RunResultJudge(runRet, 4110, 4100, 4100);
                            break;
                        case 4110://到废料区下料
                            runRet = RemoveTip(4110, 4110, 4110, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 4120, 4110, 4110);
                            break;
                        case 4120:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 4130;
                            break;
                        case 4130://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 4130, 4130);
                            runRet = UpGun(4130, 4130);
                            RunResultJudge(runRet, 4140, 4130, 4130);
                            break;
                        case 4140://取50ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取50ul枪头;
                            runRet = PickTip(4140, 4140, 4140, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 4150, 4140, 4140);
                            break;
                        case 4150:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 4160;
                            break;
                        case 4160://升移液枪Z轴
                            CheckCurrentRunStatus(0, 4160, 4160);
                            runRet = UpGun(4160, 4160);
                            RunResultJudge(runRet, 4170, 4160, 4160);
                            break;
                        case 4170://取2ul WMX
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸取2ulWMX试剂", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取2ulWMX试剂;
                            runRet = PickSolution(4170, 4170, 4170, _PointArray.低温区WMX取料位置.ToString(), 0, 0, MyVariable.WMX_volume, MyVariable.gun_inliquid_speed, -1, MyVariable.z_DiWenWMX_pos);
                            if (runRet == _ActionResult.结果OK)
                            {
                                MyVariable.area_DiWen_WMX.num_Remain = MyVariable.area_DiWen_WMX.num_Remain - ((MyVariable.WMX_volume) / 100);
                            }
                            RunResultJudge(runRet, 4180, 4170, 4170);
                            break;
                        case 4180://状态标记
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号试剂;
                            this.RunStep = 4190;
                            break;
                        case 4190://上升移液枪Z轴
                            CheckCurrentRunStatus(0, 4190, 4190);
                            runRet = UpGun(4190, 4190);
                            RunResultJudge(runRet, 4200, 4190, 4190);
                            break;
                        case 4200://到清洗实验第一个离心管
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:到1.5ml试管排2ul混合", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.到离心管排液;
                            runRet = RemoveSolution(4200, 4200, 4200, _PointArray.离心管试管区取料位置.ToString(), MyVariable.area_LiXinGuan.num_X, MyVariable.area_LiXinGuan.num_Y, MyVariable.WMX_volume, MyVariable.gun_outliquid_speed, false, MyVariable.z_LiXinGuan200_pos);
                            RunResultJudge(runRet, 4204, 4200, 4200);
                            break;
                        case 4204://吸打，保证2ulWMX能排出
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp60000,10000];";
                                SerializeClass.animationParam.gunZMark = 60000;
                            }
                            else
                            {
                                pipettegunCmd = $"1[Ia4000,{MyVariable.gun_inliquid_speed}];1[Da4000,0,{MyVariable.gun_outliquid_slowspeed},10];";//吸打,保证2ulWMX能排出
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 4208, 4204, 4204);
                            break;
                        case 4208://等待反馈
                            runRet = PipetteGunReceive();
                            RunResultJudge(runRet, 4210, 4208, 4208);
                            break;
                        case 4210:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号试剂;
                            this.RunStep = 4220;
                            break;
                        case 4220://抬移液枪Z轴
                            CheckCurrentRunStatus(0, 4220, 4220);
                            runRet = UpGun(4220, 4220);
                            RunResultJudge(runRet, 4230, 4220, 4220);
                            break;
                        case 4230://到废料区下料
                            runRet = RemoveTip(4230, 4230, 4230, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 4240, 4230, 4230);
                            break;
                        case 4240:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号枪头;
                            this.RunStep = 4250;
                            break;
                        case 4250://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 4250, 4250);
                            runRet = UpGun(4250, 4250);
                            RunResultJudge(runRet, 4580, 4250, 4250);
                            break;
                        case 4580://取1000ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(4580, 4580, 4580, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(4580, 4580, 4580, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 4600, 4580, 4580);
                            break;
                        case 4600:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取3号枪头;
                            this.RunStep = 4620;
                            break;
                        case 4620://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 4620, 4620);
                            runRet = UpGun(4620, 4620);
                            RunResultJudge(runRet, 4640, 4620, 4620);
                            break;
                        case 4640://(吸900ul 废液)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸取实验废液", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.吸取实验废液;
                            runRet = PickSolution(4640, 4640, 4640, _PointArray.废液孔位置.ToString(), 0, 0, MyVariable.Waste_Experiment1, MyVariable.gun_inliquid_speed, -1, MyVariable.z_FeiYeKong_pos);
                            RunResultJudge(runRet, 4660, 4640, 4640);
                            break;
                        case 4660:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取3号试剂;
                            this.RunStep = 4680;
                            break;
                        case 4680://上升Z轴
                            CheckCurrentRunStatus(0, 4680, 4680);
                            runRet = UpGun(4680, 4680);
                            RunResultJudge(runRet, 4700, 4680, 4680);
                            break;
                        case 4700://到废料区下料
                            runRet = RemoveTip(4700, 4700, 4700, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 4720, 4700, 4700);
                            break;
                        case 4720:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排3号枪头;
                            this.RunStep = 4740;
                            break;
                        case 4740://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 4740, 4740);
                            runRet = UpGun(4740, 4740);
                            RunResultJudge(runRet, 4760, 4740, 4740);
                            break;
                        case 4760://取1000ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(4760, 4760, 4760, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(4760, 4760, 4760, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 4780, 4760, 4760);
                            break;
                        case 4780:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取4号枪头;
                            this.RunStep = 4800;
                            break;
                        case 4800://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 4800, 4800);
                            runRet = UpGun(4800, 4800);
                            RunResultJudge(runRet, 4820, 4800, 4800);
                            break;
                        case 4820://(吸废液)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸取实验废液", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.吸取实验废液;
                            runRet = PickSolution(4820, 4820, 4820, _PointArray.废液孔位置.ToString(), 0, 0, MyVariable.Waste_Experiment2, MyVariable.gun_inliquid_speed, -1, MyVariable.z_FeiYeKong_pos);
                            RunResultJudge(runRet, 4840, 4820, 4820);
                            break;
                        case 4840:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取4号试剂;
                            this.RunStep = 4860;
                            break;
                        case 4860://上升Z轴
                            CheckCurrentRunStatus(0, 4860, 4860);
                            runRet = UpGun(4860, 4860);
                            RunResultJudge(runRet, 4870, 4860, 4860);
                            break;
                        case 4870:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                            this.RunStep = 4880;
                            break;
                        #endregion

                        #region 当前状态  废液已吸取
                        case 4880:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.废液已吸取;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 4900;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 4920;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 4980;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 4900://到废料区下料
                            runRet = RemoveTip(4900, 4900, 4900, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 4920, 4900, 4900);
                            break;
                        case 4920:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 4940;
                            break;
                        case 4940://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 4940, 4940);
                            runRet = UpGun(4940, 4940);
                            RunResultJudge(runRet, 4960, 4940, 4940);
                            break;
                        case 4960://取100ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(4960, 4960, 4960, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(4960, 4960, 4960, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            //    LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            //    runRet = PickTip(4960, 4960, 4960, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            //    MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 4980, 4960, 4960);
                            break;
                        case 4980:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 5000;
                            break;
                        case 5000://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 5000, 5000);
                            runRet = UpGun(5000, 5000);
                            RunResultJudge(runRet, 5020, 5000, 5000);
                            break;
                        case 5020:
                            CheckCurrentRunStatus(0, 5020, 5020);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.开盖完成)
                            {
                                SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                                this.RunStep = 5040;
                            }
                            break;
                        #endregion

                        #region 当前状态  开始清洗步骤一
                        case 5040:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.开始清洗步骤一;
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 5060;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号试剂:
                                    this.RunStep = 5080;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 5140;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 5200;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号试剂:
                                    this.RunStep = 5340;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号试剂:
                                    this.RunStep = 5400;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;

                        case 5060://(排气泡)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:芯片排气泡", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.芯片排气泡;
                            runRet = PickSolution(5060, 5060, 5060, _PointArray.预处理孔位置.ToString(), 0, 0, MyVariable.Bubble_Out, MyVariable.gun_outliquid_xinpian, -1, MyVariable.z_YuChuLiKong_pos);
                            RunResultJudge(runRet, 5080, 5060, 5060);
                            break;
                        case 5080:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号试剂;
                            this.RunStep = 5100;
                            break;
                        case 5100://上升Z轴
                            CheckCurrentRunStatus(0, 5100, 5100);
                            runRet = UpGun(5100, 5100);
                            RunResultJudge(runRet, 5110, 5100, 5100);
                            break;
                        case 5110:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.废料区1下料位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                       new double[] { ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                       Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 5115, 5110, 5110);
                            break;
                        case 5115://判断上一次排气泡是否完成
                            if (MyVariable.CCD_QiPao)
                            {
                                SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.废液已吸取;
                                this.RunStep = 4980;
                            }
                            else
                            {
                                this.RunStep = 5120;
                            }
                            break;
                        case 5120://到废料区下料
                            runRet = RemoveTip(5120, 5120, 5120, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 5140, 5120, 5120);
                            break;
                        case 5140:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 5160;
                            break;
                        case 5160://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 5160, 5160);
                            runRet = UpGun(5160, 5160);
                            RunResultJudge(runRet, 5180, 5160, 5160);
                            break;
                        case 5180://到1000ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(5180, 5180, 5180, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(5180, 5180, 5180, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 5200, 5180, 5180);
                            break;
                        case 5200:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 5220;
                            break;
                        case 5220://上升Z轴
                            CheckCurrentRunStatus(0, 5220, 5220);
                            runRet = UpGun(5220, 5220);
                            RunResultJudge(runRet, 5240, 5220, 5220);
                            break;

                        case 5240://清洗实验第一个离心管取清洗液
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis] + (MyVariable.area_LiXinGuan.num_X * MyVariable.LiXinGuan_XShift);
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] + (MyVariable.area_LiXinGuan.num_Y * MyVariable.LiXinGuan_YShift);
                            SerializeClass.animationParam.material1 = (int)_PointArray.离心管试管区取料位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis]+(MyVariable.area_LiXinGuan.num_X*MyVariable.LiXinGuan_XShift),
                                                ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区取料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis]+(MyVariable.area_LiXinGuan.num_Y*MyVariable.LiXinGuan_YShift) },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 5260, 5240, 5240);
                            break;
                        case 5260://下Z轴指令
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp50000,30000];";
                                SerializeClass.animationParam.gunZMark = 50000;
                                SerializeClass.animationParam.gunZSpeed = 30000;
                            }
                            else
                            {
                                pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}]1[Ia2500,500];41[Zp{MyVariable.z_LiXinGuan1000_pos},{MyVariable.z_check_speed}];";
                                SerializeClass.animationParam.gunZMark = MyVariable.z_LiXinGuan1000_pos;
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 5280, 5260, 5260);
                            break;
                        case 5280://等待反馈
                            runRet = PipetteZAxisReceive(5260, 5260, MyVariable.FunctionStep, MyVariable.FunctionStep);
                            RunResultJudge(runRet, 5300, 5260, 5260);
                            break;
                        case 5300://吸打混匀+吸液(吸400ul清洗液)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸取400ul清洗溶液", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.吸取400ul清洗溶液;
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                            {
                                pipettegunCmd = $"41[Zp60000,10000];";
                                SerializeClass.animationParam.gunZMark = 60000;
                            }
                            else
                            {
                                //pipettegunCmd = $"1[Ia{(MyVariable.DIL_volume + MyVariable.WMX_volume - 1500)},{MyVariable.gun_inliquid_speed}];";//留15ul在离心管中，防止全部吸完有气泡
                                if (MyVariable.show_IsOpen)//参观模式简化吸打混匀
                                {
                                    pipettegunCmd = $"1[Ia{(MyVariable.DIL_volume + MyVariable.WMX_volume - 1500)},{MyVariable.gun_inliquid_speed}];1[Da{(MyVariable.DIL_volume + MyVariable.WMX_volume - 2000)},,{MyVariable.gun_outliquid_speed},];1[Ia{(MyVariable.DIL_volume + MyVariable.WMX_volume - 2000)},{MyVariable.gun_inliquid_speed}];";
                                }
                                else
                                {
                                    pipettegunCmd = $"1[Ia{(MyVariable.DIL_volume + MyVariable.WMX_volume - 1500)},{MyVariable.gun_inliquid_speed}];1[Da{(MyVariable.DIL_volume + MyVariable.WMX_volume - 2000)},,{MyVariable.gun_outliquid_speed},];1[Ia{(MyVariable.DIL_volume + MyVariable.WMX_volume - 2000)},{MyVariable.gun_inliquid_speed}];1[Da{(MyVariable.DIL_volume + MyVariable.WMX_volume - 2000)},,{MyVariable.gun_outliquid_speed},];1[Ia{(MyVariable.DIL_volume + MyVariable.WMX_volume - 2000)},{MyVariable.gun_inliquid_speed}];";//吸打混匀
                                }

                                //  pipettegunCmd = $"1[Iz{(MyVariable.DIL_volume + MyVariable.WMX_volume)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Dz{(MyVariable.DIL_volume + MyVariable.WMX_volume)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Iz{(MyVariable.DIL_volume + MyVariable.WMX_volume)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Dz{(MyVariable.DIL_volume + MyVariable.WMX_volume)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];1[Iz{(MyVariable.DIL_volume + MyVariable.WMX_volume)},{MyVariable.gun_inliquid_speed},{MyVariable.surface_LiXinGuan}];";
                            }
                            runRet = PipetteGunSend(pipettegunCmd);
                            RunResultJudge(runRet, 5320, 5300, 5300);
                            break;
                        case 5320://等待反馈
                            runRet = PipetteGunReceive();
                            RunResultJudge(runRet, 5340, 5300, 5300);
                            break;
                        case 5340:
                            MyVariable.area_LiXinGuan.num_Remain--;
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号试剂;
                            this.RunStep = 5360;
                            break;
                        case 5360://上升移液枪Z轴
                            CheckCurrentRunStatus(0, 5360, 5360);
                            runRet = UpGun(5360, 5360);
                            RunResultJudge(runRet, 5380, 5360, 5360);
                            break;
                        case 5380:
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:清洗试剂打入360ul到预处理孔", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.清洗试剂打入360ul到预处理孔;
                            runRet = RemoveSolution(5380, 5380, 5380, _PointArray.预处理孔位置.ToString(), 0, 0, MyVariable.DILmix_volumeOut, MyVariable.gun_outliquid_xinpian, false, MyVariable.z_YuChuLiKong_pos);
                            RunResultJudge(runRet, 5400, 5380, 5380);
                            break;
                        case 5400:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号试剂;
                            this.RunStep = 5420;
                            break;
                        case 5420://抬移液枪Z轴
                            CheckCurrentRunStatus(0, 5420, 5420);
                            runRet = UpGun(5420, 5420);
                            RunResultJudge(runRet, 5430, 5420, 5420);
                            break;
                        case 5430:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                            this.RunStep = 5440;
                            break;
                        #endregion

                        #region 当前状态  清洗步骤一完成
                        case 5440:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.清洗步骤一完成;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 5450;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 5460;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 5520;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 5450://到废料区下料
                            runRet = RemoveTip(5450, 5450, 5450, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 5460, 5450, 5450);
                            break;
                        case 5460:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 5480;
                            break;
                        case 5480://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 5480, 5480);
                            runRet = UpGun(5480, 5480);
                            RunResultJudge(runRet, 5500, 5480, 5480);
                            break;
                        case 5500://取1000ul枪头
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(5500, 5500, 5500, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(5500, 5500, 5500, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 5520, 5500, 5500);
                            break;
                        case 5520:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 5540;
                            break;
                        case 5540://上升Z轴
                            CheckCurrentRunStatus(0, 5540, 5540);
                            runRet = UpGun(5540, 5540);
                            RunResultJudge(runRet, 5560, 5540, 5540);
                            break;
                        case 5560:
                            CheckCurrentRunStatus(0, 5560, 5560);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.关盖完成)
                            {
                                SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                                this.RunStep = 5580;
                            }
                            break;
                        #endregion

                        #region 当前状态  开始清洗步骤二
                        case 5580:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.开始清洗步骤二;
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 6000;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号试剂:
                                    this.RunStep = 6020;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 6000://取废液
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸取清洗废液", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.吸取清洗废液;
                            runRet = PickSolution(6000, 6000, 6000, _PointArray.废液孔位置.ToString(), 0, 0, MyVariable.Waste_Clean, MyVariable.gun_inliquid_speed, -1, MyVariable.z_FeiYeKong_pos);
                            RunResultJudge(runRet, 6020, 6000, 6000);
                            break;
                        case 6020:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号试剂;
                            this.RunStep = 6040;
                            break;
                        case 6040://上升Z轴
                            CheckCurrentRunStatus(0, 6040, 6040);
                            runRet = UpGun(6040, 6040);
                            RunResultJudge(runRet, 6050, 6040, 6040);
                            break;
                        case 6050:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                            this.RunStep = 6060;
                            break;
                        #endregion

                        #region 当前状态  清洗步骤二完成
                        case 6060:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.清洗步骤二完成;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 6070;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 6080;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 6070://到废料区下料
                            runRet = RemoveTip(6070, 6070, 6070, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 6080, 6070, 6070);
                            break;
                        case 6080:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 6100;
                            break;
                        case 6100://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 6100, 6100);
                            runRet = UpGun(6100, 6100);
                            RunResultJudge(runRet, 6110, 6100, 6100);
                            break;
                        case 6110:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.待机位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 6115, 6110, 6110);
                            break;
                        case 6115:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            this.RunStep = 6120;
                            break;
                        case 6120:
                            CheckCurrentRunStatus(0, 6120, 6120);
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育中 || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育完成)
                                {
                                    this.RunStep = 10;
                                }
                            }
                            else
                            {
                                if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育中)
                                {
                                    this.RunStep = 10;
                                }
                            }
                            break;
                        #endregion

                        #endregion

                        #region 测序实验保存实验流程

                        #region 当前状态  保存
                        case 6200:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.保存;
                            this.RunStep = 6220;
                            break;
                        case 6220:
                            CheckCurrentRunStatus(0, 6220, 6220);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲)
                            {
                                this.RunStep = 6240;
                            }
                            break;
                        case 6240://查询总控是否有实验任务
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽总控...", MsgType.Success, Color.Blue);
                                LogConfig.Instance.ShowMessageToList("Run", "无实验任务,走芯片加保存液流程", MsgType.Success, Color.Brown);
                                this.RunStep = 6260;
                            }
                            else
                            {
                                TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).ClearNetData();
                                string jsonStr4 = JsonConvert.SerializeObject(SerializeClass.mSearchFolloUpTaskToControl);
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).WriteDataStr(jsonStr4))
                                {
                                    LogToGeneral(jsonStr4);
                                    this.time = this.GetCurveTime();
                                    SerializeClass.animationParam.general = (int)_generalEnum.查询后续实验任务;
                                    WaitDelayTime(0.3);
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控查询是否还有实验任务", MsgType.Success, Color.Brown);
                                    this.RunStep = 6250;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送数据失败", MsgType.Error, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 6250:
                            CheckCurrentRunStatus(0, 6240, 6240);
                            if (OverTimeS(time, Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据超时", MsgType.Error, Color.Red);
                                this.RunStep = 6240;
                                throw new StationErrorException("通讯报警");
                            }
                            else
                            {
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).NetCanRead())
                                {
                                    TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).LoopReadData(1, out Program.ControlReceived, Encoding.UTF8);
                                    LogFromGeneral(Program.ControlReceived);//log
                                    b_carrystation = MyVariable.GeneralSearchFolloUpTaskReceive(Program.ControlReceived, out code_general, out data_general);
                                    if (b_carrystation)
                                    {
                                        if (code_general == 200)
                                        {
                                            if (data_general == 0)
                                            {
                                                LogConfig.Instance.ShowMessageToList("Run", "无实验任务,走芯片加保存液流程", MsgType.Success, Color.Brown);
                                                this.RunStep = 6260;
                                            }
                                            else
                                            {
                                                LogConfig.Instance.ShowMessageToList("Run", "有实验任务,当前实验任务结束", MsgType.Success, Color.Brown);
                                                this.RunStep = 6960;
                                            }
                                        }
                                        else
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据异常", MsgType.Error, Color.Red);
                                            this.RunStep = 6240;
                                            throw new StationErrorException("通讯报警");
                                        }
                                        SerializeClass.animationParam.general = (int)_generalEnum.无交互任务;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据异常", MsgType.Error, Color.Red);
                                        this.RunStep = 6240;
                                        throw new StationErrorException("通讯报警");
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region 当前状态  开始清洗步骤三
                        case 6260:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.开始清洗步骤三;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.未取枪头:
                                    LogConfig.Instance.ShowMessageToList("Run", "开始芯片保存实验！", MsgType.Success, Color.DarkOrange);
                                    this.RunStep = 6280;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号枪头:
                                    this.RunStep = 6300;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 6280://到1000ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(6280, 6280, 6280, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(6280, 6280, 6280, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            //    LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            //    runRet = PickTip(6280, 6280, 6280, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            //    MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 6300, 6280, 6280);
                            break;
                        case 6300:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号枪头;
                            this.RunStep = 6320;
                            break;
                        case 6320://上升Z轴
                            CheckCurrentRunStatus(0, 6320, 6320);
                            runRet = UpGun(6320, 6320);
                            RunResultJudge(runRet, 6340, 6320, 6320);
                            break;
                        case 6340:
                            CheckCurrentRunStatus(0, 6340, 6340);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.开盖完成)
                            {
                                SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                                this.RunStep = 6360;
                            }
                            break;
                        #endregion

                        #region 当前状态  保存液排气泡
                        case 6360:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.保存液排气泡;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 6380;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号试剂:
                                    this.RunStep = 6400;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 6460;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 6520;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号试剂:
                                    this.RunStep = 6570;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排2号试剂:
                                    this.RunStep = 6620;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 6380://(排气泡)
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:芯片排气泡", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.芯片排气泡;
                            runRet = PickSolution(6380, 6380, 6380, _PointArray.预处理孔位置.ToString(), 0, 0, MyVariable.Bubble_Out, MyVariable.gun_outliquid_xinpian, -1, MyVariable.z_YuChuLiKong_pos);
                            RunResultJudge(runRet, 6400, 6380, 6380);
                            break;
                        case 6400:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号试剂;
                            this.RunStep = 6420;
                            break;
                        case 6420://上升Z轴
                            CheckCurrentRunStatus(0, 6420, 6420);
                            runRet = UpGun(6420, 6420);
                            RunResultJudge(runRet, 6430, 6420, 6420);
                            break;
                        case 6430:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.废料区1下料位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                       new double[] { ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.废料区1下料位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                       Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 6435, 6430, 6430);
                            break;
                        case 6435://判断上一次排气泡是否完成
                            if (MyVariable.CCD_QiPao)
                            {
                                SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.开始清洗步骤三;
                                this.RunStep = 6300;
                            }
                            else
                            {
                                this.RunStep = 6440;
                            }
                            break;
                        case 6440://到废料区下料
                            runRet = RemoveTip(6440, 6440, 6440, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 6460, 6440, 6440);
                            break;
                        case 6460:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 6480;
                            break;
                        case 6480://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 6480, 6480);
                            runRet = UpGun(6480, 6480);
                            RunResultJudge(runRet, 6500, 6480, 6480);
                            break;
                        case 6500://到1000ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(6500, 6500, 6500, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(6500, 6500, 6500, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 6520, 6500, 6500);
                            break;
                        case 6520:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 6540;
                            break;
                        case 6540://上升Z轴
                            CheckCurrentRunStatus(0, 6540, 6540);
                            runRet = UpGun(6540, 6540);
                            RunResultJudge(runRet, 6560, 6540, 6540);
                            break;
                        case 6560://取S保存液500ul
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸取500ul S保存试剂", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.吸取500ulS保存试剂;
                            runRet = PickSolution(6560, 6560, 6560, _PointArray.低温区S取料位置.ToString(), 0, 0, MyVariable.S_volume, MyVariable.gun_inliquid_speed, -1, MyVariable.z_DiWenS_pos);
                            if (runRet == _ActionResult.结果OK)
                            {
                                MyVariable.area_DiWen_S.num_Remain = MyVariable.area_DiWen_S.num_Remain - (MyVariable.S_volume / 100);
                            }
                            RunResultJudge(runRet, 6570, 6560, 6560);
                            break;
                        case 6570:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号试剂;
                            this.RunStep = 6580;
                            break;
                        case 6580://上升Z轴
                            CheckCurrentRunStatus(0, 6580, 6580);
                            runRet = UpGun(6580, 6580);
                            RunResultJudge(runRet, 6600, 6580, 6580);
                            break;
                        case 6600://打入预处理孔
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:保存试剂打入450ul到预处理孔", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.保存试剂打入450ul到预处理孔;
                            runRet = RemoveSolution(6600, 6600, 6600, _PointArray.预处理孔位置.ToString(), 0, 0, MyVariable.S_volumeOut, MyVariable.gun_outliquid_xinpian, false, MyVariable.z_YuChuLiKong_pos);
                            RunResultJudge(runRet, 6620, 6600, 6600);
                            break;
                        case 6620:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排2号试剂;
                            this.RunStep = 6640;
                            break;
                        case 6640://抬移液枪Z轴
                            CheckCurrentRunStatus(0, 6640, 6640);
                            runRet = UpGun(6640, 6640);
                            RunResultJudge(runRet, 6650, 6640, 6640);
                            break;
                        case 6650:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                            this.RunStep = 6660;
                            break;
                        #endregion

                        #region 当前状态  清洗步骤三完成
                        case 6660:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.清洗步骤三完成;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 6680;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 6700;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取2号枪头:
                                    this.RunStep = 6760;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 6680://到废料区下料
                            runRet = RemoveTip(6680, 6680, 6680, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 6700, 6680, 6680);
                            break;
                        case 6700:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 6720;
                            break;
                        case 6720://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 6720, 6720);
                            runRet = UpGun(6720, 6720);
                            RunResultJudge(runRet, 6740, 6720, 6720);
                            break;
                        case 6740://到1000ul枪头区
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取1000ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取1000ul枪头;
                            if (MyVariable.area_QiangTou1.num_Remain == 0)
                            {
                                runRet = PickTip(6740, 6740, 6740, _PointArray.枪头区2取料位置.ToString(), MyVariable.area_QiangTou2.num_X, MyVariable.area_QiangTou2.num_Y);
                                MyVariable.area_QiangTou2.num_Remain--;
                            }
                            else
                            {
                                runRet = PickTip(6740, 6740, 6740, _PointArray.枪头区1取料位置.ToString(), MyVariable.area_QiangTou1.num_X, MyVariable.area_QiangTou1.num_Y);
                                MyVariable.area_QiangTou1.num_Remain--;
                            }
                            RunResultJudge(runRet, 6760, 6740, 6740);
                            break;
                        case 6760:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取2号枪头;
                            this.RunStep = 6780;
                            break;
                        case 6780://上升Z轴
                            CheckCurrentRunStatus(0, 6780, 6780);
                            runRet = UpGun(6780, 6780);
                            RunResultJudge(runRet, 6800, 6780, 6780);
                            break;
                        case 6800:
                            CheckCurrentRunStatus(0, 6800, 6800);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.关盖完成)
                            {
                                SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                                this.RunStep = 6820;
                            }
                            break;
                        #endregion

                        #region 当前状态  开始清洗步骤四
                        case 6820:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.开始清洗步骤四;
                            switch (SerializeClass.mMemory.pipette_gun_technology)
                            {
                                case MemoryClass.Pipette_gun_technology.过渡点:
                                    this.RunStep = 6840;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已取1号试剂:
                                    this.RunStep = 6860;
                                    break;
                                case MemoryClass.Pipette_gun_technology.已排1号枪头:
                                    this.RunStep = 6920;
                                    break;
                                default:
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆点与工位记忆不符", MsgType.Success, Color.Red);
                                    LogConfig.Instance.ShowMessageToList("Run", "移液枪记忆:" + SerializeClass.mMemory.pipette_gun_technology.ToString() + ";搬运工位:" + SerializeClass.mMemory.CarryStation_state.ToString(), MsgType.Success, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 6840:
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:吸取保存试剂废液", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.吸取保存试剂废液;
                            runRet = PickSolution(6840, 6840, 6840, _PointArray.废液孔位置.ToString(), 0, 0, MyVariable.Waste_Save, MyVariable.gun_inliquid_speed, -1, MyVariable.z_FeiYeKong_pos);
                            RunResultJudge(runRet, 6860, 6840, 6840);
                            break;
                        case 6860:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已取1号试剂;
                            this.RunStep = 6880;
                            break;
                        case 6880://上升Z轴
                            CheckCurrentRunStatus(0, 6880, 6880);
                            runRet = UpGun(6880, 6880);
                            RunResultJudge(runRet, 6900, 6880, 6880);
                            break;
                        case 6900://到废料区下料
                            runRet = RemoveTip(6900, 6900, 6900, _PointArray.废料区1下料位置.ToString());
                            RunResultJudge(runRet, 6920, 6900, 6900);
                            break;
                        case 6920:
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.已排1号枪头;
                            this.RunStep = 6940;
                            break;
                        case 6940://移液枪Z轴上升
                            CheckCurrentRunStatus(0, 6940, 6940);
                            runRet = UpGun(6940, 6940);
                            RunResultJudge(runRet, 6950, 6940, 6940);
                            break;
                        case 6950:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.待机位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 6955, 6950, 6950);
                            break;
                        case 6955:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            this.RunStep = 6960;
                            break;
                        #endregion

                        #region 当前状态  实验完成
                        case 6960:
                            if (MyVariable.newshow_IsOpenOver)
                            {
                                MyVariable.show_IsOpen = false;
                                SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.实验完成;
                                LogConfig.Instance.ShowMessageToList("Run", "样本编号：" + MyVariable.SN_DataProcessingStation + "   实验完成！", MsgType.Success, Color.Green);
                                //退出参观模式,试剂用量恢复
                                MyVariable.ReadPipetteParam();
                                //关闭参观模式标志
                                MyVariable.newshow_IsOpen = false;
                                MyVariable.newshow_IsOpenOver = false;
                                MainForm.mainform.Invoke(new Action(() =>
                                {
                                    MainForm.mainform.roundButton1.Text = "流转参观模式";
                                    MainForm.mainform.roundButton1.BaseColor = Color.Tomato;
                                    MainForm.mainform.roundButton1.BaseColorEnd = Color.Tomato;
                                    MainForm.mainform.rbt_Show.Visible = true;
                                }));
                                SoftWareForm.m_softwarmform.Invoke(new Action(() =>
                                {
                                    SoftWareForm.m_softwarmform.lab_RunMode.Text = "自动运行模式";
                                }));
                                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                                {
                                    MainForm_Data.mMainForm_Data.label1.Visible = true;
                                    MainForm_Data.mMainForm_Data.txt_FolderPath.Visible = true;
                                    MainForm_Data.mMainForm_Data.lblCount.Visible = true;
                                    MainForm_Data.mMainForm_Data.lab_ZongKongJJ.Visible = true;
                                    MainForm_Data.mMainForm_Data.lblExecMsg.Visible = true;
                                    MainForm_Data.mMainForm_Data.label5.Visible = true;
                                    MainForm_Data.mMainForm_Data.txt_JianJiMsg.Visible = true;
                                }));
                                LogConfig.Instance.ShowMessageToList("Run", "流转参观模式已退出!", MsgType.Success, Color.Green);
                                throw new StationPauseException("参观流程结束");
                            }
                            if (MyVariable.show_IsOpen)
                            {
                                if (!CanGuanStart1())
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "共享文件start写入失败!", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                                this.time = this.GetCurveTime();
                                MyVariable.show_Repeat = true;
                            }
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.实验完成;
                            LogConfig.Instance.ShowMessageToList("Run", "样本编号：" + MyVariable.SN_DataProcessingStation + "   实验完成！", MsgType.Success, Color.Green);
                            this.RunStep = 6970;
                            break;
                        case 6970:
                            if (MyVariable.show_IsOpen)
                            {
                                MyVariable.area_DiWen_WMX.num_Remain = 3;
                            }
                            if (MyVariable.area_QiangTou2.num_Remain < 15)
                            {
                                MyVariable.consumables_Empty[0] = true;
                            }
                            if (MyVariable.area_QiangTou3.num_Remain < 1)
                            {
                                MyVariable.consumables_Empty[1] = true;
                            }
                            if (MyVariable.area_QiangTou4.num_Remain < 4)
                            {
                                MyVariable.consumables_Empty[2] = true;
                            }
                            if (MyVariable.area_DiWen_FCT.num_Remain < ((MyVariable.FCT_volume + 300) / 100) || MyVariable.area_DiWen_FCF.num_Remain < ((MyVariable.FCF_volume / 100) * 2)
                                || MyVariable.area_DiWen_SB.num_Remain < ((MyVariable.SB_volume + 250) / 100) || MyVariable.area_DiWen_LIB.num_Remain < ((MyVariable.LIB_volume + 250) / 100)
                                || MyVariable.area_DiWen_DIL.num_Remain < (MyVariable.DIL_volume / 100) || MyVariable.area_DiWen_WMX.num_Remain < ((MyVariable.WMX_volume) / 100)
                                || MyVariable.area_DiWen_S.num_Remain < (MyVariable.S_volume / 100))
                            {
                                MyVariable.consumables_Empty[3] = true;
                            }
                            if (MyVariable.area_LiXinGuan.num_Remain < 3)
                            {
                                MyVariable.consumables_Empty[4] = true;
                            }
                            this.RunStep = 6980;
                            break;
                        case 6980:
                            CheckCurrentRunStatus(0, 6980, 6980);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲)
                            {
                                //参观模式,流程结束
                                if (MyVariable.show_IsOpen)
                                {
                                    if (OverTimeS(time, 10))
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "共享文件start.txt写入失败", MsgType.Error, Color.Red);
                                        this.RunStep = 6960;
                                        throw new StationErrorException("通讯报警");
                                    }
                                    string str3 = @"\\" + ParameConfig.Instance.SystemParameDic[_ParamName.GeneralShareIP.ToString()].CurrentValue + @"\Cexu\Start";
                                    if (Directory.Exists(str3))
                                    {
                                        string filePath = Path.Combine(str3, "start.txt");
                                        if (File.Exists(filePath))//文件存在，启动要料流程
                                        {
                                            WaitDelayTime(5);
                                            this.RunStep = 10;
                                        }
                                    }
                                }
                                //正常流程,样本载具出料
                                else
                                {
                                    this.RunStep = 7000;
                                }
                            }
                            break;
                        #endregion

                        #endregion

                        #region 当前状态  出料
                        case 7000:
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.出料;
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.样本载具流出;
                            switch (SerializeClass.mMemory.clamping_jaw_technology)
                            {
                                case MemoryClass.Clamping_jaw_technology.夹爪默认松开:
                                    this.RunStep = 7020;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪夹紧:
                                    this.RunStep = 7140;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪松开:
                                    this.RunStep = 7260;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.过渡点:
                                    this.RunStep = 7020;
                                    break;
                            }
                            break;
                        case 7020:
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪默认松开;
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                            SerializeClass.mMemory.area = MemoryClass.Area.八联排试管区;
                            SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                            this.RunStep = 7040;
                            break;
                        case 7040:
                            if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.光电8联排试管区1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.光电8联排试管区2])
                            {
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 2);
                                    SerializeClass.animationParam.ground = (int)_groundEnum.空载具回收;
                                    LogConfig.Instance.ShowMessageToList("Run", "64600地址写入2", MsgType.Success, Color.Brown);
                                }
                                LogConfig.Instance.ShowMessageToList("Run", "样本空载具回收中...", MsgType.Success, Color.Brown);
                                this.RunStep = 7060;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "样本空载具丢失，请检查流程是否正确", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 7060:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7080, 7060, 7060);
                            break;
                        case 7080:
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 19);
                                LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 19", MsgType.Success, Color.Blue);
                            }
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.八联排试管区搬运位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7100, 7080, 7080);
                            break;
                        case 7100:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.八联排试管区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.八联排试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7120, 7100, 7100);
                            break;
                        case 7120://发送夹爪夹紧指令
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawForceMove(Program.carryClawConfigList[1], 3000))
                            {
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 7140;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 7140://夹爪夹紧
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪夹紧;
                            this.RunStep = 7160;
                            break;
                        case 7160:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.试管搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7180, 7160, 7160);
                            break;
                        case 7180://去出料区搬运位置
                            if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电1] && !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电2])
                            {
                                this.RunStep = 7200;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "出料区有载具搁置,无法换料!", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 7200:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.出料区搬运位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7220, 7200, 7200);
                            break;
                        case 7220:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.进出料区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7240, 7220, 7220);
                            break;
                        case 7240://搬运夹爪松开指令
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawAbsMove(Program.carryClawConfigList[0], 3000))
                            {
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 7260;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 7260://夹爪松开
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪松开;
                            this.RunStep = 7280;
                            break;
                        case 7280:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7300, 7280, 7280);
                            break;
                        case 7300:
                            if (SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                            {
                                SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                SerializeClass.animationParam.material1 = (int)_PointArray.地轨避让位置;
                                runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 7320, 7300, 7300);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人工作中，检查状态", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 7320://走到避让位，判断地轨到位,允许进料  
                            CheckCurrentRunStatus(0, 7320, 7320);
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC1);
                                if (read_PLC1[0] == 1)
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 1);
                                    this.RunStep = 7340;
                                }
                            }
                            else
                            {
                                this.RunStep = 7380;
                            }
                            break;
                        case 7340://判断地轨取料完成
                            CheckCurrentRunStatus(0, 7340, 7340);
                            if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC1))
                            {
                                if (read_PLC1[0] == 3)
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                                    this.RunStep = 7350;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                throw new StationErrorException("通讯报警");
                            }
                            break;
                        case 7350:
                            CheckCurrentRunStatus(0, 7350, 7350);
                            if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC1))
                            {
                                if (read_PLC1[0] == 1)
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 3);
                                    this.RunStep = 7360;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                throw new StationErrorException("通讯报警");
                            }
                            break;
                        case 7360:
                            CheckCurrentRunStatus(0, 7360, 7360);
                            if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64400, 2, out read_PLC1))
                            {
                                if (read_PLC1[0] == 2 && read_PLC1[1] == 0)
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 0);
                                    SerializeClass.animationParam.ground = (int)_groundEnum.无交互任务;
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                                    this.RunStep = 7380;
                                }
                            }
                            break;
                        case 7380:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7400, 7380, 7380);
                            break;
                        case 7400:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.待机位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 7420, 7400, 7400);
                            break;
                        case 7420:
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            LogConfig.Instance.ShowMessageToList("Run", "样本载具回收完成", MsgType.Success, Color.Green);
                            this.RunStep = 7430;
                            break;
                        case 7430://检查数据分析线程是否完成解析
                            CheckCurrentRunStatus(0, 7430, 7430);
                            if (!MyVariable.JianJiShiBie_Start && SerializeClass.mMemory.DataProcessingStation_state == MemoryClass.DataProcessingStation_State.空闲)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "样本已解析完成,可以开始下一次实验...", MsgType.Success, Color.Green);
                                MyVariable.b_StatusToControl = false;
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region 新参观模式流程
                        case 8000://取50ul枪头
                            SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.工作结束;
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.步骤一完成;
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取50ul枪头", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取50ul枪头;
                            runRet = PickTip(8000, 8000, 8000, _PointArray.枪头区4取料位置.ToString(), MyVariable.area_QiangTou4.num_X, MyVariable.area_QiangTou4.num_Y);
                            MyVariable.area_QiangTou4.num_Remain--;
                            RunResultJudge(runRet, 8020, 8000, 8000);
                            break;
                        case 8020://升移液枪Z轴
                            SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.DNA文库配置完成;
                            CheckCurrentRunStatus(0, 8020, 8020);
                            runRet = UpGun(8020, 8020);
                            RunResultJudge(runRet, 8040, 8020, 8020);
                            break;
                        case 8040://等待开盖完成,执行步序2
                            CheckCurrentRunStatus(0, 8040, 8040);
                            if (MyVariable.newshow_step1)
                            {
                                SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.开始步骤二;
                                SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                                MyVariable.newshow_step1 = false;
                                this.RunStep = 8060;
                            }
                            break;
                        case 8060://到样本载具取样本
                            MyVariable.show_memory = 1;//线程记忆点,用于带记忆复位
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:取12ulDNA文库样本", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.取12ulDNA文库样本;
                            runRet = PickSolution(8060, 8060, 8060, _PointArray.八联排DNA样本取料位置.ToString(), 0, 0, MyVariable.DNA_volume, MyVariable.gun_inliquid_speed, -1, MyVariable.z_BaLianPai50_pos);
                            RunResultJudge(runRet, 8080, 8060, 8060);
                            break;
                        case 8080://上升移液枪Z轴
                            MyVariable.show_memory = 2;//线程记忆点,用于带记忆复位
                            CheckCurrentRunStatus(0, 8080, 8080);
                            runRet = UpGun(8080, 8080);
                            RunResultJudge(runRet, 8100, 8080, 8080);
                            break;
                        case 8100://逐滴加入芯片上样孔
                            MyVariable.show_memory = 3;//线程记忆点,用于带记忆复位
                            LogConfig.Instance.ShowMessageToList("Run", "实验进程:逐滴加入芯片上样孔", MsgType.Success, Color.Brown);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.逐滴加入芯片上样孔;
                            runRet = RemoveSolution(8100, 8100, 8100, _PointArray.上样孔位置.ToString(), 0, 0, -1, MyVariable.gun_outliquid_slowspeed, false, MyVariable.z_ShangYangKong_pos);
                            RunResultJudge(runRet, 8120, 8100, 8100);
                            break;
                        case 8120://抬移液枪Z轴
                            MyVariable.show_memory = 4;//线程记忆点,用于带记忆复位
                            CheckCurrentRunStatus(0, 8120, 8120);
                            runRet = UpGun(8120, 8120);
                            SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.过渡点;
                            RunResultJudge(runRet, 3860, 8120, 8120);
                            break;
                            #endregion
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    sw_YiYeQiang.Stop();
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.搬运工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    sw_YiYeQiang.Stop();
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
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.搬运工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    MyVariable.FunctionStep = 0;
                    sw_YiYeQiang.Stop();
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.搬运工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunDone = true;
                    this.RunStep = 0;
                    StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }
        /// <summary>
        /// 空载具回收模式
        /// </summary>
        public override void StationEmptyRun()
        {
            this.RunDone = false;
            StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0://启动,判断机台是否空闲
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "设备已屏蔽PLC，无法回收空载具", MsgType.Success, Color.Red);
                                this.RunStep = 700;
                            }
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲 && SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.空闲
                                && SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲 && SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                            {
                                SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                                this.RunStep = 10;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "设备当前有任务执行，无法回收空载具", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 10:
                            switch (SerializeClass.mMemory.clamping_jaw_technology)
                            {
                                case MemoryClass.Clamping_jaw_technology.夹爪默认松开:
                                    this.RunStep = 15;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.过渡点:
                                    this.RunStep = 100;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪夹紧:
                                    this.RunStep = 230;
                                    break;
                                case MemoryClass.Clamping_jaw_technology.夹爪松开:
                                    this.RunStep = 380;
                                    break;
                            }
                            break;
                        case 15:
                            MyVariable.EmptyRun_Restart = false;
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪默认松开;
                            this.RunStep = 20;
                            break;
                        case 20://检查机台内是否有空载具
                            if (MyVariable.area_QiangTou2.num_Remain < 15)
                            {
                                MyVariable.consumables_Empty[0] = true;
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电2])
                                {
                                    MyVariable.EmptyRun_Qu.Enqueue(MemoryClass.Area.枪头区1);
                                    LogConfig.Instance.ShowMessageToList("Run", "1000枪头区1耗材不足，需要回收", MsgType.Success, Color.Brown);
                                }
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电2])
                                {
                                    MyVariable.EmptyRun_Qu.Enqueue(MemoryClass.Area.枪头区2);
                                    LogConfig.Instance.ShowMessageToList("Run", "1000枪头区2耗材不足，需要回收", MsgType.Success, Color.Brown);
                                }
                            }
                            if (MyVariable.area_QiangTou3.num_Remain < 1)
                            {
                                MyVariable.consumables_Empty[1] = true;
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电2])
                                {
                                    MyVariable.EmptyRun_Qu.Enqueue(MemoryClass.Area.枪头区3);
                                    LogConfig.Instance.ShowMessageToList("Run", "200枪头区耗材不足，需要回收", MsgType.Success, Color.Brown);
                                }
                            }
                            if (MyVariable.area_QiangTou4.num_Remain < 4)
                            {
                                MyVariable.consumables_Empty[2] = true;
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电2])
                                {
                                    MyVariable.EmptyRun_Qu.Enqueue(MemoryClass.Area.枪头区4);
                                    LogConfig.Instance.ShowMessageToList("Run", "50枪头区耗材不足，需要回收", MsgType.Success, Color.Brown);
                                }
                            }
                            if (MyVariable.area_DiWen_FCT.num_Remain < ((MyVariable.FCT_volume + 300) / 100) || MyVariable.area_DiWen_FCF.num_Remain < ((MyVariable.FCF_volume / 100) * 2)
                                || MyVariable.area_DiWen_SB.num_Remain < ((MyVariable.SB_volume + 250) / 100) || MyVariable.area_DiWen_LIB.num_Remain < ((MyVariable.LIB_volume + 250) / 100)
                                || MyVariable.area_DiWen_DIL.num_Remain < (MyVariable.DIL_volume / 100) || MyVariable.area_DiWen_WMX.num_Remain < ((MyVariable.WMX_volume) / 100)
                                || MyVariable.area_DiWen_S.num_Remain < (MyVariable.S_volume / 100))
                            {
                                MyVariable.consumables_Empty[3] = true;
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.低温区光电])
                                {
                                    MyVariable.EmptyRun_Qu.Enqueue(MemoryClass.Area.低温区);
                                    LogConfig.Instance.ShowMessageToList("Run", "低温区耗材不足，需要回收", MsgType.Success, Color.Brown);
                                }
                            }
                            if (MyVariable.area_LiXinGuan.num_Remain < 3)
                            {
                                MyVariable.consumables_Empty[4] = true;
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电2])
                                {
                                    MyVariable.EmptyRun_Qu.Enqueue(MemoryClass.Area.离心管试管区);
                                    LogConfig.Instance.ShowMessageToList("Run", "1.5试管区耗材不足，需要回收", MsgType.Success, Color.Brown);
                                }
                            }

                            if (MyVariable.EmptyRun_Qu.Count != 0)
                            {
                                Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 2);
                                SerializeClass.animationParam.ground = (int)_groundEnum.空载具回收;
                                this.RunStep = 100;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "当前设备无空载具回收", MsgType.Success, Color.Blue);
                                this.RunStep = 700;
                            }
                            break;
                        case 100:
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.过渡点;
                            this.RunStep = 110;
                            break;
                        case 110:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 120, 110, 110);
                            break;
                        case 120:
                            CheckCurrentRunStatus(0, 120, 120);
                            switch (MyVariable.EmptyRun_Qu.Peek())
                            {
                                case MemoryClass.Area.枪头区1:
                                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区1;
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 3);
                                    LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 3", MsgType.Success, Color.Blue);
                                    LogConfig.Instance.ShowMessageToList("Run", "空载具出料区域: " + MemoryClass.Area.枪头区1.ToString(), MsgType.Success, Color.Blue);
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区1搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区1搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 140, 120, 120);
                                    break;
                                case MemoryClass.Area.枪头区2:
                                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区2;
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 3);
                                    LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 3", MsgType.Success, Color.Blue);
                                    LogConfig.Instance.ShowMessageToList("Run", "空载具出料区域: " + MemoryClass.Area.枪头区2.ToString(), MsgType.Success, Color.Blue);
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区2搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区2搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 140, 120, 120);
                                    break;
                                case MemoryClass.Area.枪头区3:
                                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区3;
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 2);
                                    LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 2", MsgType.Success, Color.Blue);
                                    LogConfig.Instance.ShowMessageToList("Run", "空载具出料区域: " + MemoryClass.Area.枪头区3.ToString(), MsgType.Success, Color.Blue);
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区3搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区3搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 140, 120, 120);
                                    break;
                                case MemoryClass.Area.枪头区4:
                                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区4;
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 1);
                                    LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 1", MsgType.Success, Color.Blue);
                                    LogConfig.Instance.ShowMessageToList("Run", "空载具出料区域: " + MemoryClass.Area.枪头区4.ToString(), MsgType.Success, Color.Blue);
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.枪头区4搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头区4搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 140, 120, 120);
                                    break;
                                case MemoryClass.Area.低温区:
                                    SerializeClass.mMemory.area = MemoryClass.Area.低温区;
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 9);
                                    LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 9", MsgType.Success, Color.Blue);
                                    LogConfig.Instance.ShowMessageToList("Run", "空载具出料区域: " + MemoryClass.Area.低温区.ToString(), MsgType.Success, Color.Blue);
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.低温区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.低温区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 150, 120, 120);
                                    break;
                                case MemoryClass.Area.离心管试管区:
                                    SerializeClass.mMemory.area = MemoryClass.Area.离心管试管区;
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 21);
                                    LogConfig.Instance.ShowMessageToList("Run", "64602地址写: 21", MsgType.Success, Color.Blue);
                                    LogConfig.Instance.ShowMessageToList("Run", "空载具出料区域: " + MemoryClass.Area.离心管试管区.ToString(), MsgType.Success, Color.Blue);
                                    SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                    SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                    SerializeClass.animationParam.material1 = (int)_PointArray.离心管试管区搬运位置;
                                    runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                     new double[] { ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                    RunResultJudge(runRet, 170, 120, 120);
                                    break;
                            }
                            break;
                        case 140://到枪头区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 140, 140);
                            break;
                        case 150://到低温区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.低温区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.低温区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.低温区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 150, 150);
                            break;
                        case 170://到1.5离心管区
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.离心管试管区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.离心管试管区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 200, 170, 170);
                            break;

                        case 200://发送夹爪夹紧指令
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawForceMove(Program.carryClawConfigList[1], 3000))
                            {
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 230;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 230://夹爪夹紧
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪夹紧;
                            this.RunStep = 240;
                            break;
                        case 240:
                            if (MyVariable.EmptyRun_Qu.Peek() == MemoryClass.Area.低温区 || MyVariable.EmptyRun_Qu.Peek() == MemoryClass.Area.离心管试管区)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.试管搬运上升位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 310, 240, 240);
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                      Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                      Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 280, 240, 240);
                            }
                            break;
                        //去枪头出料区
                        case 280:
                            if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头出料区光电1] && !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头出料区光电2])
                            {
                                this.RunStep = 290;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "枪头出料区有载具搁置,无法换料!", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 290:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.枪头出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.枪头出料区搬运位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.枪头出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.枪头出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 300, 290, 290);
                            break;
                        case 300:
                            if (MyVariable.EmptyRun_Qu.Peek() == MemoryClass.Area.枪头区1 || MyVariable.EmptyRun_Qu.Peek() == MemoryClass.Area.枪头区2)
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头1000进出料区抓取位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头1000进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                SerializeClass.animationParam.material2 = (int)_PointArray.枪头200进出料区抓取位置;
                                runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头200进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            }
                            RunResultJudge(runRet, 350, 300, 300);
                            break;

                        //去出料区搬运位置
                        case 310:
                            if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电1] && !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电2])
                            {
                                this.RunStep = 320;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "出料区有载具搁置,无法换料!", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        case 320:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.出料区搬运位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.出料区搬运位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 330, 320, 320);
                            break;
                        case 330:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.进出料区抓取位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                                                           Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.进出料区抓取位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                                                           Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 350, 330, 330);
                            break;


                        case 350://搬运夹爪松开指令
                            if (SoftWareForm.carryclaw_initialize.WaitCarryClawAbsMove(Program.carryClawConfigList[0], 3000))
                            {
                                SerializeClass.animationParam.carryClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 380;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 380://夹爪松开
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪松开;
                            this.RunStep = 500;
                            break;
                        case 500:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 502, 500, 500);
                            break;
                        case 502:
                            if (SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                            {
                                SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                                SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                                SerializeClass.animationParam.material1 = (int)_PointArray.地轨避让位置;
                                runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                                 new double[] { ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.地轨避让位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                                 Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                                RunResultJudge(runRet, 505, 502, 502);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人工作中，检查状态", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;


                        case 505://走到避让位，判断地轨到位,允许进料
                            CheckCurrentRunStatus(0, 30, 30);
                            Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC1);
                            if (read_PLC1[0] == 1)
                            {
                                Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 1);
                                this.RunStep = 508;
                            }
                            break;
                        case 508://判断地轨取料完成
                            CheckCurrentRunStatus(0, 508, 508);
                            if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC1))
                            {
                                if (read_PLC1[0] == 3)
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                                    MyVariable.EmptyRun_Qu.Dequeue();
                                    this.RunStep = 510;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                throw new StationErrorException("通讯报警");
                            }
                            break;
                        case 510:
                            CheckCurrentRunStatus(0, 510, 510);
                            if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC1))
                            {
                                if (read_PLC1[0] == 1)
                                {
                                    this.RunStep = 520;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                throw new StationErrorException("通讯报警");
                            }
                            break;
                        case 520:
                            if (MyVariable.EmptyRun_Qu.Count != 0)
                            {
                                if (MyVariable.EmptyRun_Restart)
                                {
                                    //程序重启，重新给队列赋值
                                    this.RunStep = 15;
                                }
                                else
                                {
                                    //继续出空载具
                                    this.RunStep = 100;
                                }
                            }
                            else
                            {
                                //空载具出完
                                Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 3);
                                this.RunStep = 600;
                            }
                            break;
                        case 600:
                            CheckCurrentRunStatus(0, 600, 600);
                            if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64400, 2, out read_PLC1))
                            {
                                if (read_PLC1[0] == 2 && read_PLC1[1] == 0)
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 0);
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                                    SerializeClass.animationParam.ground = (int)_groundEnum.无交互任务;
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64602, 0);
                                    this.RunStep = 620;
                                }
                            }
                            break;
                        case 620:
                            SerializeClass.animationParam.carryZMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                            SerializeClass.animationParam.material2 = (int)_PointArray.枪头搬运上升位置;
                            runRet = WaitSingleAxisAbsMove(_CarryStation2Axis.搬运ZAxis.ToString(),
                               Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.枪头搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]),
                               Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 640, 620, 620);
                            break;
                        case 640:
                            SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis];
                            SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis];
                            SerializeClass.animationParam.material1 = (int)_PointArray.待机位置;
                            runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                                                             new double[] { ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                                                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 700, 640, 640);
                            break;
                        case 700:
                            MyVariable.EmptyRun_RunDone = true;
                            SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪默认松开;
                            LogConfig.Instance.ShowMessageToList("Run", "空载具回收流程结束", MsgType.Success, Color.Green);
                            throw new StationWorkDone("");
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.搬运工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.搬运工位.ToString() + "异常捕获：" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.搬运工位.ToString() + "报警捕获：" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunDone = true;
                    this.RunStep = 0;
                    StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        public override void StationCalibRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Stop);
        }

        public override void StationCPKRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].ChangeStatus(_StationStatus.Stop);
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
        /// 上升移液枪Z轴
        /// </summary>
        /// <param name="errorstep">异常步序</param>
        /// <param name="pausestep">暂停步序</param>
        /// <returns></returns>
        public _ActionResult UpGun(int errorstep, int pausestep)
        {
            b_function = true;
            while (b_function)
            {
                switch (MyVariable.FunctionStep)
                {
                    case 0://移液枪Z轴上升
                        CheckCurrentRunStatus(0, errorstep, pausestep);
                        pipettegunCmd = $"41[Zp{MyVariable.z_movepos_up},{MyVariable.z_movepos_speed}];";
                        SerializeClass.animationParam.gunZMark = MyVariable.z_movepos_up;
                        SerializeClass.animationParam.gunZSpeed = MyVariable.z_movepos_speed;
                        runRet = PipetteGunSend(pipettegunCmd);
                        if (runRet == _ActionResult.结果NG)
                        {
                            throw new StationErrorException("移液枪报警");
                        }
                        MyVariable.FunctionStep = 20;
                        break;
                    case 20://等待反馈
                        runRet = PipetteZAxisReceive(errorstep, pausestep, 0, 0);
                        RunResultJudge(runRet, errorstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 40;
                        break;
                    case 40:
                        MyVariable.FunctionStep = 0;
                        b_function = false;
                        break;
                }
            }
            return _ActionResult.结果OK;
        }

        private _PointArray points;
        /// <summary>
        /// 取枪头(到指定位置,移液枪Z轴取枪头)
        /// </summary>
        /// <param name="currentstep">当前步序</param>
        /// <param name="errorstep">异常步序</param>
        /// <param name="pausestep">暂停步序</param>
        /// <param name="pointname">点位</param>
        /// <param name="x_shift">X方向偏移</param>
        /// <param name="y_shift">Y方向偏移</param>
        public _ActionResult PickTip(int currentstep, int errorstep, int pausestep, string pointname, double x_shift, double y_shift)
        {
            b_function = true;
            while (b_function)
            {
                switch (MyVariable.FunctionStep)
                {
                    case 0://到指定枪头区
                        SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运XAxis] + (x_shift * MyVariable.Tip_XShift);
                        SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运YAxis] + (y_shift * MyVariable.Tip_YShift);
                        if (Enum.TryParse(pointname, out points))
                        {
                            SerializeClass.animationParam.material1 = Array.IndexOf(Enum.GetValues(typeof(_PointArray)), points);
                        }
                        runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                             new double[] { ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运XAxis] + (x_shift * MyVariable.Tip_XShift), ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运YAxis] + (y_shift * MyVariable.Tip_YShift) },
                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        RunResultJudge(runRet, currentstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 20;
                        break;
                    case 20://移液枪Z轴取枪头
                        SerializeClass.animationParam.TipPickStatus = (int)_TipEnum.触发;
                        if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                        {
                            pipettegunCmd = $"41[Zp50000,30000];";
                            SerializeClass.animationParam.gunZMark = 50000;
                        }
                        else
                        {
                            pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}];41[Zg{MyVariable.z_pickTip_speed},80];";//取枪头
                            SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_pickTip_speed) / 2;
                            SerializeClass.animationParam.gunZMark = 100000;
                        }
                        runRet = PipetteGunSend(pipettegunCmd);
                        if (runRet == _ActionResult.结果NG)
                        {
                            throw new StationErrorException("移液枪报警");
                        }
                        MyVariable.FunctionStep = 40;
                        break;
                    case 40://等待反馈
                        runRet = PipetteZAxisReceive(errorstep, pausestep, 20, 20);
                        RunResultJudge(runRet, currentstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 60;
                        break;
                    case 60:
                        SerializeClass.animationParam.TipPickStatus = (int)_TipEnum.不触发;
                        MyVariable.FunctionStep = 0;
                        b_function = false;
                        break;
                }
            }
            return _ActionResult.结果OK;
        }

        /// <summary>
        /// 下枪头(到指定位置,移液枪Z轴下降,下枪头)
        /// </summary>
        /// <param name="currentstep">当前步序</param>
        /// <param name="errorstep">异常步序</param>
        /// <param name="pausestep">暂停步序</param>
        /// <param name="pointname">点位名称</param>
        /// <returns></returns>
        public _ActionResult RemoveTip(int currentstep, int errorstep, int pausestep, string pointname)
        {
            LogConfig.Instance.ShowMessageToList("Run", "实验进程:排枪头", MsgType.Success, Color.Brown);
            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.排枪头;
            b_function = true;
            while (b_function)
            {
                switch (MyVariable.FunctionStep)
                {
                    case 0://到废料区下料
                        SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运XAxis];
                        SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运YAxis];
                        if (Enum.TryParse(pointname, out points))
                        {
                            SerializeClass.animationParam.material1 = Array.IndexOf(Enum.GetValues(typeof(_PointArray)), points);
                        }
                        runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                             new double[] { ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运XAxis], ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运YAxis] },
                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        RunResultJudge(runRet, currentstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 20;
                        break;
                    case 20://移液枪Z轴下降
                        SerializeClass.animationParam.TipBackStatus = (int)_TipEnum.触发;
                        if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                        {
                            pipettegunCmd = $"41[Zp50000,30000];";
                            SerializeClass.animationParam.gunZMark = 50000;
                        }
                        else
                        {
                            pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}];";
                            SerializeClass.animationParam.gunZMark = MyVariable.z_movepos_down;
                            SerializeClass.animationParam.gunZSpeed = MyVariable.z_movepos_speed;
                        }
                        runRet = PipetteGunSend(pipettegunCmd);
                        if (runRet == _ActionResult.结果NG)
                        {
                            throw new StationErrorException("移液枪报警");
                        }
                        MyVariable.FunctionStep = 40;
                        break;
                    case 40://等待反馈
                        runRet = PipetteZAxisReceive(errorstep, pausestep, 20, 20);
                        RunResultJudge(runRet, currentstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 60;
                        break;
                    case 60://下枪头
                        if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                        {
                            pipettegunCmd = $"41[Zp60000,10000];";
                            SerializeClass.animationParam.gunZMark = 60000;
                        }
                        else
                        {
                            pipettegunCmd = $"1[It500];";
                        }
                        runRet = PipetteGunSend(pipettegunCmd);
                        if (runRet == _ActionResult.结果NG)
                        {
                            throw new StationErrorException("移液枪报警");
                        }
                        MyVariable.FunctionStep = 80;
                        break;
                    case 80://等待反馈
                        runRet = PipetteGunReceive();
                        if (runRet == _ActionResult.结果OK)
                        {
                            MyVariable.FunctionStep = 100;
                        }
                        else
                        {
                            MyVariable.FunctionStep = 60;
                            throw new StationErrorException("移液枪报警");
                        }
                        break;
                    case 100:
                        SerializeClass.animationParam.TipBackStatus = (int)_TipEnum.不触发;
                        MyVariable.FunctionStep = 0;
                        b_function = false;
                        break;
                }
            }
            return _ActionResult.结果OK;
        }

        /// <summary>
        /// 吸液(到指定位置,移液枪Z轴下探+液面探测,液面跟随吸液)
        /// </summary>
        /// <param name="currentstep">当前步序</param>
        /// <param name="errorstep">异常步序</param>
        /// <param name="pausestep">暂停步序</param>
        /// <param name="pointname">点位名称</param>
        /// <param name="x_shift">X方向偏移</param>
        /// <param name="y_shift">Y方向偏移</param>
        /// <param name="volume">吸液体积</param>
        /// <param name="speed">吸液速度</param>
        /// <param name="surfaceArea">吸液表面积(-1表示不需要液面跟随)</param>
        /// <param name="pos">吸液时移液枪位置(0表示不需要提供位置,使用液面探测)</param>
        /// <returns></returns>
        public _ActionResult PickSolution(int currentstep, int errorstep, int pausestep, string pointname, double x_shift, double y_shift, double volume, double speed, double surfaceArea, double pos)
        {
            b_function = true;
            while (b_function)
            {
                switch (MyVariable.FunctionStep)
                {
                    case 0:
                        SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运XAxis] + (x_shift * MyVariable.LiXinGuan_XShift);
                        SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运YAxis] + (y_shift * MyVariable.LiXinGuan_YShift);
                        if (Enum.TryParse(pointname, out points))
                        {
                            SerializeClass.animationParam.material1 = Array.IndexOf(Enum.GetValues(typeof(_PointArray)), points);
                        }
                        runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                             new double[] { ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运XAxis] + (x_shift * MyVariable.LiXinGuan_XShift), ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运YAxis] + (y_shift * MyVariable.LiXinGuan_YShift) },
                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        RunResultJudge(runRet, currentstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 20;
                        break;
                    case 20://与移液枪通讯,移液枪下探到一定位置并吸取30ul空气,再转成液面探测
                        if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                        {
                            pipettegunCmd = $"41[Zp50000,30000];";
                            SerializeClass.animationParam.gunZMark = 50000;
                        }
                        else
                        {
                            if (pos != 0)
                            {
                                //不需要液面跟随吸液，走固定位置
                                pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}]1[Ia2500,500];41[Zp{pos},{MyVariable.z_check_speed}];";
                                SerializeClass.animationParam.gunZMark = pos;
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                            }
                            else
                            {
                                //液面探测
                                pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}]1[Ia2500,500];41[Zp{MyVariable.z_check_pos},{MyVariable.z_check_speed}]1[Ld0,10000];";
                                SerializeClass.animationParam.gunZMark = MyVariable.z_check_pos;
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                            }
                        }
                        runRet = PipetteGunSend(pipettegunCmd);
                        if (runRet == _ActionResult.结果NG)
                        {
                            throw new StationErrorException("移液枪报警");
                        }
                        MyVariable.FunctionStep = 40;
                        break;
                    case 40://等待反馈
                        runRet = PipetteZAxisReceive(errorstep, pausestep, 20, 20);
                        RunResultJudge(runRet, currentstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 60;
                        break;
                    case 60://移液枪液面跟随吸液
                        if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                        {
                            pipettegunCmd = $"41[Zp60000,10000];";
                            SerializeClass.animationParam.gunZMark = 60000;
                        }
                        else
                        {
                            if (surfaceArea == -1)
                            {
                                pipettegunCmd = $"1[Ia{volume},{speed}];";//吸液,不需要液面跟随
                            }
                            else
                            {
                                pipettegunCmd = $"1[Iz{volume},{speed},{surfaceArea}];";//液面跟随吸液
                            }
                        }
                        runRet = PipetteGunSend(pipettegunCmd);
                        if (runRet == _ActionResult.结果NG)
                        {
                            throw new StationErrorException("移液枪报警");
                        }
                        MyVariable.FunctionStep = 80;
                        break;
                    case 80://等待反馈
                        runRet = PipetteGunReceive();
                        if (runRet == _ActionResult.结果OK)
                        {
                            MyVariable.FunctionStep = 100;
                        }
                        else
                        {
                            MyVariable.FunctionStep = 60;
                            throw new StationErrorException("移液枪报警");
                        }
                        break;
                    case 100:
                        MyVariable.FunctionStep = 0;
                        b_function = false;
                        break;
                }
            }

            return _ActionResult.结果OK;
        }

        /// <summary>
        /// 排液(到指定位置,移液枪Z轴下降到指定高度,排液)
        /// </summary>
        /// <param name="currentstep">当前步序</param>
        /// <param name="errorstep">异常步序</param>
        /// <param name="pausestep">暂停步序</param>
        /// <param name="pointname">点位名称</param>
        /// <param name="x_shift">X方向偏移</param>
        /// <param name="y_shift">Y方向偏移</param>
        /// <param name="volume">排液体积(-1:表示排空)</param>
        /// <param name="speed">排液速度</param>
        /// <param name="check">是否需要液面探测(true:需要)(在芯片中排液)</param>
        /// <param name="pos">排液时Z轴位置(在离心管排液)</param>
        /// <returns></returns>
        public _ActionResult RemoveSolution(int currentstep, int errorstep, int pausestep, string pointname, double x_shift, double y_shift, double volume, double speed, bool check, double pos)
        {
            b_function = true;
            while (b_function)
            {
                switch (MyVariable.FunctionStep)
                {
                    case 0:
                        SerializeClass.animationParam.carryXMark = ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运XAxis] + (x_shift * MyVariable.LiXinGuan_XShift);
                        SerializeClass.animationParam.carryYMark = ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运YAxis] + (y_shift * MyVariable.LiXinGuan_YShift);
                        if (Enum.TryParse(pointname, out points))
                        {
                            SerializeClass.animationParam.material1 = Array.IndexOf(Enum.GetValues(typeof(_PointArray)), points);
                        }
                        runRet = WaitMultipleAxisAbsMove(new string[] { _CarryStation1Axis.搬运XAxis.ToString(), _CarryStation1Axis.搬运YAxis.ToString() },
                             new double[] { ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运XAxis] + (x_shift * MyVariable.LiXinGuan_XShift), ParameConfig.Instance.PointParameDic[pointname].PosList[(int)_CarryStation1Axis.搬运YAxis] + (y_shift * MyVariable.LiXinGuan_YShift) },
                             Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                        RunResultJudge(runRet, currentstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 20;
                        break;
                    case 20://下Z轴指令
                        if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                        {
                            pipettegunCmd = $"41[Zp50000,30000];";
                            SerializeClass.animationParam.gunZMark = 50000;
                        }
                        else
                        {
                            if (check)
                            {
                                //液面探测
                                pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}];41[Zp{MyVariable.z_check_pos},{MyVariable.z_check_speed}]1[Ld0,10000];";
                                SerializeClass.animationParam.gunZMark = MyVariable.z_check_pos;
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                            }
                            else
                            {
                                //走固定位置
                                pipettegunCmd = $"41[Zp{MyVariable.z_movepos_down},{MyVariable.z_movepos_speed}];41[Zp{pos},{MyVariable.z_check_speed}];";
                                SerializeClass.animationParam.gunZMark = pos;
                                SerializeClass.animationParam.gunZSpeed = (MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2;
                            }
                        }
                        runRet = PipetteGunSend(pipettegunCmd);
                        if (runRet == _ActionResult.结果NG)
                        {
                            throw new StationErrorException("移液枪报警");
                        }
                        MyVariable.FunctionStep = 40;
                        break;
                    case 40://等待反馈
                        runRet = PipetteZAxisReceive(errorstep, pausestep, 20, 20);
                        RunResultJudge(runRet, currentstep, errorstep, pausestep);
                        MyVariable.FunctionStep = 60;
                        break;
                    case 60://排液
                        if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPipette.ToString()].CurrentValue)))
                        {
                            pipettegunCmd = $"41[Zp60000,10000];";
                            SerializeClass.animationParam.gunZMark = 60000;
                        }
                        else
                        {
                            if (volume == -1)
                            {
                                pipettegunCmd = $"1[De{speed}];";//排空

                            }
                            else
                            {
                                pipettegunCmd = $"1[Da{volume},0,{speed},10];";

                            }
                        }
                        runRet = PipetteGunSend(pipettegunCmd);
                        if (runRet == _ActionResult.结果NG)
                        {
                            throw new StationErrorException("移液枪报警");
                        }
                        MyVariable.FunctionStep = 80;
                        break;
                    case 80://等待反馈
                        runRet = PipetteGunReceive();
                        if (runRet == _ActionResult.结果OK)
                        {
                            MyVariable.FunctionStep = 100;
                        }
                        else
                        {
                            MyVariable.FunctionStep = 60;
                            throw new StationErrorException("移液枪报警");
                        }
                        break;
                    case 100:
                        MyVariable.FunctionStep = 0;
                        b_function = false;
                        break;
                }
            }
            return _ActionResult.结果OK;
        }




        /// <summary>
        /// 移液枪发送指令
        /// </summary>
        /// <param name="sendmsg">发送内容</param>
        private _ActionResult PipetteGunSend(string sendmsg)
        {
            try
            {
                if (sendmsg.Contains("Z"))
                {
                    SerializeClass.animationParam.gunZStart = (int)_AxisStartSignEnum.启动;
                }
                LogToGun(sendmsg);
                Byte[] cmdListByt = System.Text.Encoding.UTF8.GetBytes(sendmsg);
                KpcState_e state = ktCntDll.KpcAddCmdList(123, cmdListByt);
                if (state == KpcState_e.KPC_OK)
                {
                    return _ActionResult.结果OK;
                }
                else
                {
                    LogConfig.Instance.ShowMessageToList("Run", "移液枪指令发送失败: " + state.ToString(), MsgType.Success, Color.Red);
                    return _ActionResult.结果NG;
                }
            }
            catch (Exception ex)
            {
                LogConfig.Instance.ShowMessageToList("Run", "PipetteGunSend()方法执行失败: " + ex.Message, MsgType.Success, Color.Red);
                return _ActionResult.结果NG;
            }
        }
        /// <summary>
        /// 移液枪状态判断(移液枪吸液或排液时暂停或异常等情况不停止,等待吸液/排液完成)
        /// </summary>
        /// <returns></returns>
        private _ActionResult PipetteGunReceive()
        {
            sw_YiYeQiang.Restart();
            while (true)
            {
                Thread.Sleep(10);
                KpcCntTaakState_e states = ktCntDll.KpcGetCntTaskState(123);
                if (sw_YiYeQiang.ElapsedMilliseconds / 1000 > Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.PipetteTimeOut.ToString()].CurrentValue))
                {
                    sw_YiYeQiang.Stop();
                    LogConfig.Instance.ShowMessageToList("Run", "移液枪运行失败: " + states.ToString(), MsgType.Success, Color.Red);
                    SerializeClass.animationParam.gunZStart = (int)_AxisStartSignEnum.停止;
                    return _ActionResult.结果NG;
                }
                else if (states == KpcCntTaakState_e.KPC_TASK_EXE_FINISH)
                {
                    sw_YiYeQiang.Stop();
                    SerializeClass.animationParam.gunZStart = (int)_AxisStartSignEnum.停止;
                    return _ActionResult.结果OK;
                }
            }
        }


        /// <summary>
        /// 移液枪Z轴状态判断(Z轴移动中暂停或异常等情况需要停止运动)
        /// </summary>
        /// <param name="runerror">运行流程异常步序</param>
        /// <param name="runpause">运行流程暂停步序</param>
        /// <param name="functionerror">功能块流程异常步序</param>
        /// <param name="functionpause">功能块流程暂停步序</param>
        /// <returns></returns>
        private _ActionResult PipetteZAxisReceive(int runerror, int runpause, int functionerror, int functionpause)
        {
            sw_YiYeQiang.Restart();
            while (true)
            {
                Thread.Sleep(10);
                if (mCurStatus == _StationStatus.Alarm)
                {
                    this.RunStep = 0;
                    MyVariable.FunctionStep = 0;
                    throw new StationAlarmException("");
                }
                if (mCurStatus == _StationStatus.Error)
                {
                    this.RunStep = runerror;
                    MyVariable.FunctionStep = functionerror;
                    throw new StationErrorException("移液枪报警");
                }
                if (mCurStatus == _StationStatus.Pause)
                {
                    this.RunStep = runpause;
                    MyVariable.FunctionStep = functionpause;
                    throw new StationPauseException("");
                }
                KpcCntTaakState_e states = ktCntDll.KpcGetCntTaskState(123);
                if (states == KpcCntTaakState_e.KPC_TASK_EXE_FINISH)
                {
                    sw_YiYeQiang.Stop();
                    return _ActionResult.结果OK;
                }
                else if (sw_YiYeQiang.ElapsedMilliseconds / 1000 > Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.PipetteTimeOut.ToString()].CurrentValue))
                {
                    sw_YiYeQiang.Stop();
                    LogConfig.Instance.ShowMessageToList("Run", "移液枪Z轴运行失败: " + states.ToString(), MsgType.Success, Color.Red);
                    return _ActionResult.结果NG;
                }
            }
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

        /// <summary>
        /// 给移液枪发送信息Log记录
        /// </summary>
        /// <param name="sendmsgs"></param>
        private void LogToGun(string sendmsgs)
        {
            string NowDate = string.Format("{0:yyyyMMdd}", DateTime.Now);//获取当前日期
            if (!Directory.Exists(@"E:\SWLog\PipetteGun\"))
            {
                Directory.CreateDirectory(@"E:\SWLog\PipetteGun\");
            }
            if (!File.Exists(@"E:\SWLog\PipetteGun\" + NowDate + ".txt"))
            {
                File.Create(@"E:\SWLog\PipetteGun\" + NowDate + ".txt").Close();
            }
            if (File.Exists(@"E:\SWLog\PipetteGun\" + NowDate + ".txt"))
            {
                using (FileStream fsWrite = new FileStream(@"E:\SWLog\PipetteGun\" + NowDate + ".txt", FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fsWrite, Encoding.Unicode))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  CeXu-->PipetteGun  " + sendmsgs);
                    }
                }
            }
        }

        public bool CanGuanStart1()
        {
            try
            {
                string str2 = @"\\" + ParameConfig.Instance.SystemParameDic[_ParamName.GeneralShareIP.ToString()].CurrentValue + @"\Cexu\Start";
                if (Directory.Exists(str2))
                {
                    MyVariable.DeleteFilesInDirectory(str2);
                    string filePath = Path.Combine(str2, "start.txt");
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

    }
}

