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
using MsgBoxLib;

namespace CYStandardProcedure
{
    public class MainstationPauseException : Exception
    {
        public MainstationPauseException(string msg)
        {
        }
    }

    public class RobotStation : ObjectStation
    {
        public long time;
        private string mName;
        private _ActionResult resetRet;//单步复位结果
        private _ActionResult runRet;//单步运行结果
        string recRobotMsg = "";//接收机器人信息
        Stopwatch robotStartTime = new Stopwatch();//开启机器人工程后连接延时判断
        public static event Action LoadPicAction;//显示上相机液体检测图片
        public static event Action LoadPicAction2;//显示下相机取料定位图片
        public static event Action LoadPicAction3;//显示下相机盖板类型图片
        public static event Action LoadPicAction4;//显示下相机盖板有无图片
        public static event Action LoadPicAction5;//显示下相机放料定位图片
        public static event Action LoadPicAction6;//显示上3D相机图片
        public static event Action LoadPicAction7;//显示下3D相机图片
        private string[] strArray1;//相机接收数据数组
        private string[] strArray2;//相机接收运动偏移数据数组

        private string currentpos_Robot;//机器人当前坐标
        private string Calib_CCDsend;//标定模式CCD发送指令

        private int xiCount = 0;

        public RobotStation(string name) :
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
            StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Initial);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.ResetStep)
                    {
                        case 0://网口通讯,关闭工程
                            if (TCPClientConfig.Instance.ReConnectClient(_TcpClientModule.AuboRobot.ToString()))
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.Aubo机器人程序停止])
                                {
                                    this.ResetStep = 10;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "关闭机器人工程", MsgType.Success, Color.Blue);
                                    resetRet = WaitNetData(_TcpClientModule.AuboRobot.ToString(), MyVariable.robot_project_StopCmd, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.RobotTimeOut.ToString()].CurrentValue), out recRobotMsg);
                                    if (recRobotMsg.Contains("project") || recRobotMsg.Contains("Program stop"))
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "机器人工程关闭成功", MsgType.Success, Color.Green);
                                        ResetResultJudge(resetRet, 10);
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "机器人通讯错误！ 机器人返回信息: " + recRobotMsg, MsgType.Error, Color.Red);
                                        throw new StationHomeErrException("机器人通讯错误！ 机器人返回信息: " + recRobotMsg);
                                    }
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人通讯失败，检查网络！", MsgType.Error, Color.Red);
                                throw new StationHomeErrException("机器人通讯失败，检查网络！");
                            }
                            break;
                        case 10://判断当前记忆点
                            for (byte i = 1; i < 3; i++)//机器人电动夹爪上使能
                            {
                                SerializeClass.m_ModbusRtuRob.WriteSingleCoil(i, 1, true);
                            }
                            if (SerializeClass.mMemory.robotclaw_technology == MemoryClass.RobotClaw_technology.夹爪夹紧)
                            {
                                this.ResetStep = 30;
                            }
                            else
                            {
                                this.ResetStep = 20;
                            }
                            break;
                        case 20://复位夹爪
                            if (SoftWareForm.m_RobotNewClaw.WaitRobClawHome(1))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.松开;
                                this.ResetStep = 25;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪复位失败", MsgType.Success, Color.Red);
                                throw new StationHomeErrException("机器人夹紧夹爪复位失败");
                            }
                            break;
                        case 25://复位旋转夹爪
                            if (SoftWareForm.m_RobotNewClaw.WaitRobClawHome(2))
                            {
                                this.ResetStep = 27;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人旋转夹爪复位失败", MsgType.Success, Color.Red);
                                throw new StationHomeErrException("机器人旋转夹爪复位失败");
                            }
                            break;
                        case 27://旋转夹爪走待机位置
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(2, MyVariable.force2_fuwei, MyVariable.speed2_fuwei, MyVariable.acc2_fuwei, MyVariable.pos2_fuwei, 999))
                            {
                                this.ResetStep = 30;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人旋转夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationHomeErrException("机器人旋转夹爪运动失败");
                            }
                            break;
                        case 30:
                            CheckCurrentResetStatus();
                            if (MyVariable.CarryStationResetOK)
                            {
                                MyVariable.CarryStationResetOK = false;
                                this.ResetStep = 40;
                            }
                            break;
                        case 40://延时启动
                            WaitDelayTime(0.2);
                            if (TCPClientConfig.Instance.ReConnectClient(_TcpClientModule.AuboRobot.ToString()) && TCPClientConfig.Instance.ReConnectClient(_TcpClientModule.AuboRobotSDK.ToString()))
                            {
                                if (MyVariable.AuboSDKInstance())
                                {
                                    if (AuboClass.Instance.Initial(MyVariable.ipAddressaubo, int.Parse(MyVariable.portaubo)))
                                    {
                                        this.ResetStep = 60;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "机器人SDK加载失败", MsgType.Error, Color.Red);
                                        throw new StationHomeErrException("机器人通讯错误");
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "机器人SDK加载失败", MsgType.Error, Color.Red);
                                    throw new StationHomeErrException("机器人通讯错误");
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人连接失败", MsgType.Error, Color.Red);
                                throw new StationHomeErrException("机器人通讯错误");
                            }
                            break;
                        case 60://网口通讯,开启工程
                            resetRet = WaitNetData(_TcpClientModule.AuboRobot.ToString(), MyVariable.robot_project_StartCmd, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.RobotTimeOut.ToString()].CurrentValue), out recRobotMsg);
                            if (recRobotMsg.Contains("Program start"))
                            {
                                robotStartTime.Restart();
                                LogConfig.Instance.ShowMessageToList("Run", "机器人工程开启成功", MsgType.Success, Color.Green);
                                ResetResultJudge(resetRet, 80);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人通讯错误！ 机器人返回信息: " + recRobotMsg, MsgType.Error, Color.Red);
                                throw new StationHomeErrException("机器人通讯错误！ 机器人返回信息: " + recRobotMsg);
                            }
                            break;
                        case 80://打开机器人客户端
                            CheckCurrentResetStatus();
                            WaitDelayTime(1);
                            if (TCPClientConfig.Instance.ReConnectClient(_TcpClientModule.RobotProject.ToString()))
                            {
                                robotStartTime.Restart();
                                LogConfig.Instance.ShowMessageToList("Run", "机器人连接成功", MsgType.Success, Color.Green);
                                this.ResetStep = 90;
                            }
                            else if (robotStartTime.ElapsedMilliseconds / 1000 >= 6)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人工程开启时间过长,检查通讯", MsgType.Error, Color.Red);
                                throw new StationHomeErrException("机器人工程开启时间过长,检查通讯");
                            }
                            break;
                        case 90:
                            CheckCurrentResetStatus();
                            if (TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).NetCanRead())
                            {
                                TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).LoopReadData(1, out recRobotMsg);
                                if (recRobotMsg.Contains("CanReset"))
                                {
                                    MyVariable.RobotStationResetOK = true;
                                    robotStartTime.Restart();
                                    this.ResetStep = 100;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "机器人反馈数据异常！" + recRobotMsg, MsgType.Error, Color.Red);
                                    throw new StationHomeErrException("机器人反馈数据异常！" + recRobotMsg);
                                }
                            }
                            else if (robotStartTime.ElapsedMilliseconds / 1000 >= 30)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人复位超时", MsgType.Error, Color.Red);
                                throw new StationHomeErrException("机器人复位超时");
                            }
                            break;
                        case 100://等待机器人复位完成
                            CheckCurrentResetStatus();
                            int length = TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).LoopReadData(20, out recRobotMsg, Encoding.Default);
                            if (length > 0)
                            {
                                if (recRobotMsg.Contains("HomeOK"))
                                {
                                    robotStartTime.Stop();
                                }
                                else
                                {
                                    robotStartTime.Stop();
                                    LogConfig.Instance.ShowMessageToList("Run", "复位完成字符串不全:" + recRobotMsg, MsgType.Success, Color.Red);
                                }
                                this.ResetStep = 150;
                            }
                            else if (robotStartTime.ElapsedMilliseconds / 1000 >= 30)
                            {
                                robotStartTime.Stop();
                                LogConfig.Instance.ShowMessageToList("Run", "机器人复位失败", MsgType.Error, Color.Red);
                                throw new StationHomeErrException("机器人复位失败");
                            }
                            break;
                        case 150:
                            if (SerializeClass.mMemory.robotclaw_technology == MemoryClass.RobotClaw_technology.夹爪夹紧)
                            {
                                this.ResetStep = 160;
                            }
                            else
                            {
                                this.ResetStep = 200;
                            }
                            break;
                        case 160:
                            TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).ClearNetData();
                            if (TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).WriteDataStr("10,0,0,0"))
                            {
                                this.time = this.GetCurveTime();
                                this.ResetStep = 170;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "向机器人发送数据失败", MsgType.Error, Color.Red);
                                throw new StationHomeErrException("向机器人发送数据失败");
                            }
                            break;
                        case 170:
                            if (OverTimeS(time, Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.RobotTimeOut.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人反馈数据超时", MsgType.Error, Color.Red);
                                throw new StationHomeErrException("机器人反馈数据超时");
                            }
                            else
                            {
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).NetCanRead())
                                {
                                    TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).LoopReadData(1, out recRobotMsg);
                                    if (recRobotMsg.Contains("OK"))
                                    {
                                        this.ResetStep = 200;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "机器人反馈数据异常" + recRobotMsg, MsgType.Error, Color.Red);
                                        throw new StationHomeErrException("机器人反馈数据异常" + recRobotMsg);
                                    }
                                }
                            }
                            break;
                        case 200:
                            throw new StationHomeOK("机器人工位线程复位完成！");
                    }
                }
                /***子线程复位失败跳转到这里***/
                catch (StationHomeErrException ex)
                {
                    //  LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString(), MsgType.Error, Color.Red);
                    MyVariable.RobotStationResetOK = false;
                    MyVariable.CarryStationResetOK = false;
                    this.ResetStep = 0;
                    this.ResetError = true;
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
                /***子线程复位完成跳转到这里***/
                catch (StationHomeOK ex)
                {
                    this.ResetStep = 0;
                    this.ResetDone = true;
                    LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Success, Color.Green);
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Stop);
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
            this.time = this.GetCurveTime();//设备暂停，异常状态后机器人执行暂停工程，重新开启后机器人继续运行，计时刷新
            StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0:
                            switch (SerializeClass.mMemory.RobotStation_state)
                            {
                                case MemoryClass.RobotStation_State.空闲:
                                    this.RunStep = 10;
                                    break;
                                case MemoryClass.RobotStation_State.开预处理孔盖中:
                                    this.RunStep = 100;
                                    break;
                                case MemoryClass.RobotStation_State.开盖完成:
                                    this.RunStep = 1020;
                                    break;
                                case MemoryClass.RobotStation_State.开上样孔盖中:
                                    this.RunStep = 300;
                                    break;
                                case MemoryClass.RobotStation_State.关预处理孔盖中:
                                    this.RunStep = 600;
                                    break;
                                case MemoryClass.RobotStation_State.关上样孔盖中:
                                    this.RunStep = 800;
                                    break;
                                case MemoryClass.RobotStation_State.关盖完成:
                                    this.RunStep = 1220;
                                    break;
                            }
                            break;

                        #region 当前状态  空闲
                        case 10:
                            CheckCurrentRunStatus(0, 10, 10);
                            SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.空闲;
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪默认松开;
                            this.RunStep = 20;
                            break;
                        case 20://判断测序仪工位状态
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.可开预处理孔)
                            {
                                this.RunStep = 100;
                            }
                            else if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.可开上样孔)
                            {
                                this.RunStep = 300;
                            }
                            else if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.可关预处理孔
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.继续关预处理孔)
                            {
                                this.RunStep = 600;
                            }
                            else if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.可关上样孔)
                            {
                                this.RunStep = 800;
                            }
                            else
                            {
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region 机器人开预处理孔盖流程
                        case 100:
                            SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.开预处理孔盖中;
                            switch (SerializeClass.mMemory.robotclaw_technology)
                            {
                                case MemoryClass.RobotClaw_technology.夹爪默认松开:
                                    this.RunStep = 110;
                                    break;
                                case MemoryClass.RobotClaw_technology.夹爪夹紧:
                                    this.RunStep = 180;
                                    break;
                                case MemoryClass.RobotClaw_technology.夹爪松开:
                                    this.RunStep = 270;
                                    break;
                            }
                            break;
                        case 110:
                            CheckCurrentRunStatus(0, 110, 110);
                            if (SerializeClass.mMemory.carrystation_working == MemoryClass.CarryStation_Working.工作结束)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运模组运行完毕，开始开盖流程", MsgType.Success, Color.Blue);
                                SerializeClass.animationParam.taskStep = (int)_taskStepEnum.机器人开预处理孔盖中;
                                this.RunStep = 120;
                            }
                            break;
                        case 120://给机器人发公共位置
                            MyVariable.robot_RunStep = "10";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 125);
                            break;
                        case 125://等待机器人反馈
                            CheckCurrentRunStatus(0, 125, 125);
                            ReceiveFromRobot(130, 120);
                            break;
                        case 130://给机器人发上样孔盖拍照位置
                            MyVariable.robot_RunStep = "20";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 135);
                            break;
                        case 135://等待机器人反馈
                            CheckCurrentRunStatus(0, 135, 135);
                            ReceiveFromRobot(140, 130);
                            break;
                        case 140://上相机拍照(判断上样孔盖是否使用自制盖子)
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledAutoOpen.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "当前设定手动开关盖，不判断上样孔盖类型", MsgType.Success, Color.Brown);
                                this.RunStep = 160;
                            }
                            else if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽相机。。。", MsgType.Success, Color.Blue);
                                WaitDelayTime(1);
                                this.RunStep = 160;
                            }
                            else
                            {
                                runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Program.CCDCmd_XinPianGai, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                                LogCCD(Program.CCDCmd_XinPianGai, Program.CCDReceived);
                                if (MyVariable.show_IsOpen)
                                {
                                    LoadPicAction3();
                                    RunResultJudge(runRet, 160, 140, 140);
                                }
                                else if (Program.CCDReceived.Contains("fail"))//原装盖子
                                {
                                    LoadPicAction3();
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                                    {
                                        IOConfig.Instance.LightAction(LightState.红灯闪);
                                    }
                                    MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.OK, true);
                                    mb.TopMost = true;
                                    mb.MsgShowDialog("提示", "测序芯片当前使用原装上样孔盖，需拆除至孔盖备用区");
                                    string btn = mb.ret.SelectedBtn;
                                    if (btn == "btn_A")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "测序芯片使用原装上样孔盖，需拆除", MsgType.Error, Color.Red);
                                        throw new StationErrorException("实验流程报警");
                                    }
                                }
                                else if (Program.CCDReceived.Contains("pass"))//自制盖子
                                {
                                    LoadPicAction3();
                                    RunResultJudge(runRet, 160, 140, 140);
                                }
                                else if (Program.CCDReceived.Contains("null"))//无盖子
                                {
                                    LoadPicAction3();
                                    MyVariable.RobotStation_Replace = true;
                                    LogConfig.Instance.ShowMessageToList("Run", "测序芯片未关上样孔，走关上样孔流程", MsgType.Success, Color.Brown);
                                    SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.关上样孔盖中;
                                    SerializeClass.animationParam.taskStep = (int)_taskStepEnum.机器人关上样孔盖中;
                                    RunResultJudge(runRet, 830, 140, 140);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                    throw new StationErrorException("相机通讯错误");
                                }
                            }
                            break;
                        case 160://给电动夹爪发夹紧信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_daowei, MyVariable.speed1_daowei, MyVariable.acc1_daowei, MyVariable.pos1_daowei, 999))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 180;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 180:
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪夹紧;
                            if (MyVariable.CCD_QiPao)
                            {
                                this.RunStep = 240;
                            }
                            else
                            {
                                this.RunStep = 200;
                            }
                            break;
                        case 200://给机器人发开预处理孔步骤1
                            MyVariable.robot_RunStep = "110";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 210);
                            break;
                        case 210:
                            CheckCurrentRunStatus(0, 210, 210);
                            ReceiveFromRobot(220, 200);
                            break;
                        case 220://给机器人发开预处理孔步骤2
                            SerializeClass.animationParam.holeStatus = (int)_holeEnum.开;
                            MyVariable.robot_RunStep = "120";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 225);
                            break;
                        case 225:
                            CheckCurrentRunStatus(0, 225, 225);
                            ReceiveFromRobot(240, 220);
                            break;
                        case 240://给机器人发开预处理孔步骤3
                            MyVariable.robot_RunStep = "130";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 250);
                            break;
                        case 250:
                            CheckCurrentRunStatus(0, 250, 250);
                            ReceiveFromRobot(255, 240);
                            break;
                        case 255://相机拍照确认气泡大小
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽相机。。。", MsgType.Success, Color.Blue);
                                MyVariable.CCD_QiPao = false;
                                MyVariable.num_QiPao = 0;
                                WaitDelayTime(1);
                                this.RunStep = 260;
                            }
                            else
                            {
                                runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Program.CCDQiPaoCmd, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                                LogCCD(Program.CCDQiPaoCmd, Program.CCDReceived);
                                if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                    throw new StationErrorException("相机通讯错误");
                                }
                                strArray1 = Program.CCDReceived.Split('_');
                                if (MyVariable.show_IsOpen)
                                {
                                    LoadPicAction();
                                    if (xiCount >= 2)
                                    {
                                        MyVariable.CCD_QiPao = false;
                                        xiCount = 0;
                                        LogConfig.Instance.ShowMessageToList("Run", "气泡体积小", MsgType.Success, Color.Blue);
                                    }
                                    else
                                    {
                                        MyVariable.CCD_QiPao = true;
                                        xiCount++;
                                        LogConfig.Instance.ShowMessageToList("Run", "气泡体积大", MsgType.Success, Color.Blue);
                                    }
                                    RunResultJudge(runRet, 260, 255, 255);
                                }
                                else if (strArray1[6] == "true")
                                {
                                    LoadPicAction();
                                    MyVariable.CCD_QiPao = false;
                                    MyVariable.num_QiPao = 0;
                                    LogConfig.Instance.ShowMessageToList("Run", "气泡体积小", MsgType.Success, Color.Blue);
                                    RunResultJudge(runRet, 260, 255, 255);
                                }
                                else if (strArray1[6] == "false")
                                {
                                    LoadPicAction();
                                    MyVariable.CCD_QiPao = true;
                                    MyVariable.num_QiPao++;
                                    if (MyVariable.num_QiPao >= 3)
                                    {
                                        this.RunStep = 255;
                                        LogConfig.Instance.ShowMessageToList("Run", "连续2次无法排出气泡，确认枪头位置！", MsgType.Success, Color.Red);
                                        throw new StationErrorException("实验流程报警");
                                    }
                                    LogConfig.Instance.ShowMessageToList("Run", "气泡体积大", MsgType.Success, Color.Blue);
                                    RunResultJudge(runRet, 260, 255, 255);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "CCD反馈异常", MsgType.Error, Color.Red);
                                    throw new StationErrorException("CCD反馈异常");
                                }
                            }
                            break;
                        case 260://给电动夹爪发松开信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_fuwei, MyVariable.speed1_fuwei, MyVariable.acc1_fuwei, MyVariable.pos1_fuwei, 999))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 270;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 270:
                            MyVariable.RobotWorkDone = true;
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪松开;
                            this.RunStep = 1000;
                            break;

                        #endregion

                        #region 机器人开上样孔盖流程
                        case 300:
                            SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.开上样孔盖中;
                            switch (SerializeClass.mMemory.robotclaw_technology)
                            {
                                case MemoryClass.RobotClaw_technology.夹爪默认松开:
                                    this.RunStep = 310;
                                    break;
                                case MemoryClass.RobotClaw_technology.夹爪夹紧:
                                    this.RunStep = 400;
                                    break;
                                case MemoryClass.RobotClaw_technology.夹爪松开:
                                    this.RunStep = 490;
                                    break;
                            }
                            break;
                        case 310:
                            CheckCurrentRunStatus(0, 310, 310);
                            if (SerializeClass.mMemory.carrystation_working == MemoryClass.CarryStation_Working.工作结束)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运模组运行完毕，开始开盖流程", MsgType.Success, Color.Blue);
                                SerializeClass.animationParam.taskStep = (int)_taskStepEnum.机器人开上样孔盖中;
                                this.RunStep = 315;
                            }
                            break;
                        case 315:
                            CheckCurrentRunStatus(0, 315, 315);
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledAutoOpen.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "当前设定自动开关盖，走机器人开上样孔盖流程", MsgType.Success, Color.Brown);
                                this.RunStep = 320;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "当前设定手动开关盖，请人工开上样孔盖", MsgType.Success, Color.Brown);
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                                {
                                    IOConfig.Instance.LightAction(LightState.黄灯闪);
                                }
                                MsgBox mb1 = new MsgBox(MsgBoxType.提示, BtnType.OK, true);
                                mb1.TopMost = true;
                                mb1.MsgShowDialog("提示", "当前设置手动开关盖模式，请手动开上样孔盖");
                                string btn1 = mb1.ret.SelectedBtn;
                                if (btn1 == "btn_A")
                                {
                                    this.RunStep = 490;
                                    throw new MainstationPauseException("开上样孔盖");
                                }
                            }
                            break;
                        case 320://给机器人发公共位置
                            MyVariable.robot_RunStep = "10";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 325);
                            break;
                        case 325:
                            CheckCurrentRunStatus(0, 325, 325);
                            ReceiveFromRobot(360, 320);
                            break;
                        case 360://给机器人发开上样孔步骤1
                            MyVariable.robot_RunStep = "210";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 365);
                            break;
                        case 365:
                            CheckCurrentRunStatus(0, 365, 365);
                            ReceiveFromRobot(380, 360);
                            break;
                        case 380://给电动夹爪发夹紧信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_daowei, MyVariable.speed1_daowei, MyVariable.acc1_daowei, MyVariable.pos1_daowei, MyVariable.force1_daowei))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 400;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 400://获取电动夹爪状态
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪夹紧;
                            this.RunStep = 410;
                            break;
                        case 410://给机器人发开上样孔步骤1（抬机器人，去相机拍照处）
                            MyVariable.robot_RunStep = "213";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 413);
                            break;
                        case 413:
                            CheckCurrentRunStatus(0, 413, 413);
                            ReceiveFromRobot(415, 410);
                            break;
                        case 415://给机器人发开上样孔步骤ccd（相机拍照位）
                            MyVariable.robot_RunStep = "215";
                            MyVariable.robot_XShift = (MyVariable.CCD_KongGaiCount * MyVariable.KongGai_XShift * (-1)).ToString();
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 417);
                            break;
                        case 417:
                            CheckCurrentRunStatus(0, 417, 417);
                            ReceiveFromRobot(420, 415);
                            break;
                        case 420://上相机拍照(判断当前废料区孔位是否有盖子)
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽相机。。。", MsgType.Success, Color.Blue);
                                MyVariable.CCD_KongGaiCount = 0;
                                WaitDelayTime(1);
                                this.RunStep = 430;
                            }
                            else
                            {
                                runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Program.CCDCmd_IsHaveCover, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                                LogCCD(Program.CCDCmd_IsHaveCover, Program.CCDReceived);
                                LoadPicAction4();
                                if (Program.CCDReceived.Contains("true"))//有盖子，无法放料
                                {
                                    MyVariable.CCD_KongGaiCount++;
                                    RunResultJudge(runRet, 415, 420, 420);
                                }
                                else if (Program.CCDReceived.Contains("false"))//无盖子，可以抛料
                                {
                                    RunResultJudge(runRet, 430, 420, 420);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                    throw new StationErrorException("相机通讯错误");
                                }
                                if (MyVariable.CCD_KongGaiCount > 3)
                                {
                                    MyVariable.CCD_KongGaiCount = 0;
                                    LogConfig.Instance.ShowMessageToList("Run", "上样孔孔盖存放区域盖子已用完，请补料", MsgType.Error, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                                }
                            }
                            break;
                        case 430://给机器人发开上样孔步骤2（判断拿取的是哪个盖子，放到对应的废料区）
                            if (MyVariable.CCD_KongGaiCount == 0)
                            {
                                MyVariable.robot_RunStep = "220";
                            }
                            else if (MyVariable.CCD_KongGaiCount == 1)
                            {
                                MyVariable.robot_RunStep = "230";
                            }
                            else if (MyVariable.CCD_KongGaiCount == 2)
                            {
                                MyVariable.robot_RunStep = "240";
                            }
                            else if (MyVariable.CCD_KongGaiCount == 3)
                            {
                                MyVariable.robot_RunStep = "250";
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "无法判断对应废料区,检查程序逻辑", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            MyVariable.CCD_KongGaiCount = 0;
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 435);
                            break;
                        case 435:
                            CheckCurrentRunStatus(0, 435, 435);
                            ReceiveFromRobot(440, 430);
                            break;
                        case 440://给电动夹爪发松开信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_fuwei, MyVariable.speed1_fuwei, MyVariable.acc1_fuwei, MyVariable.pos1_fuwei, 999))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 460;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 460://获取电动夹爪状态
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪松开;
                            this.RunStep = 480;
                            break;
                        case 480://给机器人发开上样孔步骤3
                            MyVariable.robot_RunStep = "260";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 485);
                            break;
                        case 485:
                            CheckCurrentRunStatus(0, 485, 485);
                            ReceiveFromRobot(490, 480);
                            break;
                        case 490:
                            MyVariable.RobotWorkDone = true;
                            this.RunStep = 1000;
                            break;
                        #endregion

                        #region 机器人关预处理孔盖流程
                        case 600:
                            SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.关预处理孔盖中;
                            switch (SerializeClass.mMemory.robotclaw_technology)
                            {
                                case MemoryClass.RobotClaw_technology.夹爪默认松开:
                                    this.RunStep = 610;
                                    break;
                                case MemoryClass.RobotClaw_technology.夹爪夹紧:
                                    this.RunStep = 670;
                                    break;
                                case MemoryClass.RobotClaw_technology.夹爪松开:
                                    this.RunStep = 770;
                                    break;
                            }
                            break;
                        case 610:
                            CheckCurrentRunStatus(0, 610, 610);
                            if (SerializeClass.mMemory.carrystation_working == MemoryClass.CarryStation_Working.工作结束)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运模组运行完毕，开始关盖流程", MsgType.Success, Color.Blue);
                                SerializeClass.animationParam.taskStep = (int)_taskStepEnum.机器人关预处理孔盖中;
                                this.RunStep = 620;
                            }
                            break;
                        case 620://给机器人发公共位置
                            MyVariable.robot_RunStep = "10";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 625);
                            break;
                        case 625:
                            CheckCurrentRunStatus(0, 625, 625);
                            ReceiveFromRobot(660, 620);
                            break;
                        case 660://给电动夹爪发夹紧信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_daowei, MyVariable.speed1_daowei, MyVariable.acc1_daowei, MyVariable.pos1_daowei, 999))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 670;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 670:
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪夹紧;
                            this.RunStep = 680;
                            break;
                        case 680://给机器人发关预处理孔步骤1
                            MyVariable.robot_RunStep = "310";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 690);
                            break;
                        case 690:
                            CheckCurrentRunStatus(0, 690, 690);
                            ReceiveFromRobot(720, 680);
                            break;
                        case 720://给机器人发关预处理孔步骤2
                            SerializeClass.animationParam.holeStatus = (int)_holeEnum.关;
                            MyVariable.robot_RunStep = "320";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 725);
                            break;
                        case 725:
                            CheckCurrentRunStatus(0, 725, 725);
                            ReceiveFromRobot(740, 720);
                            break;
                        case 740://给机器人发关预处理孔步骤3
                            MyVariable.robot_RunStep = "330";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 750);
                            break;
                        case 750:
                            CheckCurrentRunStatus(0, 750, 750);
                            ReceiveFromRobot(760, 740);
                            break;
                        case 760://给电动夹爪发松开信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_fuwei, MyVariable.speed1_fuwei, MyVariable.acc1_fuwei, MyVariable.pos1_fuwei, 999))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 770;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 770:
                            MyVariable.RobotWorkDone = true;
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪松开;
                            this.RunStep = 1200;
                            break;

                        #endregion

                        #region 机器人关上样孔盖流程
                        case 800:
                            SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.关上样孔盖中;
                            switch (SerializeClass.mMemory.robotclaw_technology)
                            {
                                case MemoryClass.RobotClaw_technology.夹爪默认松开:
                                    this.RunStep = 810;
                                    break;
                                case MemoryClass.RobotClaw_technology.夹爪夹紧:
                                    this.RunStep = 900;
                                    break;
                                case MemoryClass.RobotClaw_technology.夹爪松开:
                                    this.RunStep = 990;
                                    break;
                            }
                            break;
                        case 810:
                            CheckCurrentRunStatus(0, 810, 810);
                            if (SerializeClass.mMemory.carrystation_working == MemoryClass.CarryStation_Working.工作结束)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "搬运模组运行完毕，开始关盖流程", MsgType.Success, Color.Blue);
                                SerializeClass.animationParam.taskStep = (int)_taskStepEnum.机器人关上样孔盖中;
                                this.RunStep = 815;
                            }
                            break;
                        case 815:
                            CheckCurrentRunStatus(0, 815, 815);
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledAutoOpen.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "当前设定自动开关盖，走机器人关上样孔盖流程", MsgType.Success, Color.Brown);
                                this.RunStep = 820;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "当前设定手动开关盖，请人工关上样孔盖", MsgType.Success, Color.Brown);
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                                {
                                    IOConfig.Instance.LightAction(LightState.黄灯闪);
                                }
                                MsgBox mb2 = new MsgBox(MsgBoxType.提示, BtnType.OK, true);
                                mb2.TopMost = true;
                                mb2.MsgShowDialog("提示", "当前设置手动开关盖模式，请手动关上样孔盖");
                                string btn2 = mb2.ret.SelectedBtn;
                                if (btn2 == "btn_A")
                                {
                                    this.RunStep = 995;
                                    throw new MainstationPauseException("关上样孔盖");
                                }
                            }
                            break;
                        case 820://给机器人发公共位置
                            MyVariable.robot_RunStep = "10";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 825);
                            break;
                        case 825:
                            CheckCurrentRunStatus(0, 825, 825);
                            ReceiveFromRobot(830, 820);
                            break;
                        case 830://给机器人发关上样孔步骤0（到孔盖相机拍照位置）
                            MyVariable.robot_RunStep = "410";
                            MyVariable.robot_XShift = (MyVariable.CCD_KongGaiCount * MyVariable.KongGai_XShift * (-1)).ToString();
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 835);
                            break;
                        case 835:
                            CheckCurrentRunStatus(0, 835, 835);
                            ReceiveFromRobot(840, 863);
                            break;
                        case 840://上相机拍照(判断当前孔位是否有盖子)
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽相机。。。", MsgType.Success, Color.Blue);
                                MyVariable.CCD_KongGaiCount = 0;
                                WaitDelayTime(1);
                                this.RunStep = 860;
                            }
                            else
                            {
                                runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Program.CCDCmd_IsHaveCover, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                                LogCCD(Program.CCDCmd_IsHaveCover, Program.CCDReceived);
                                LoadPicAction4();
                                if (Program.CCDReceived.Contains("true"))//有盖子，取料
                                {
                                    RunResultJudge(runRet, 860, 840, 840);
                                }
                                else if (Program.CCDReceived.Contains("false"))//无盖子
                                {
                                    MyVariable.CCD_KongGaiCount++;
                                    RunResultJudge(runRet, 830, 840, 840);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                    throw new StationErrorException("相机通讯错误");
                                }
                                if (MyVariable.CCD_KongGaiCount > 3)
                                {
                                    MyVariable.CCD_KongGaiCount = 0;
                                    LogConfig.Instance.ShowMessageToList("Run", "上样孔孔盖存放区域盖子已用完，请补料", MsgType.Error, Color.Red);
                                    throw new StationErrorException("实验流程报警");
                                }
                            }
                            break;
                        case 860://给机器人发关上样孔步骤1(选择对应孔盖夹取)
                            if (MyVariable.CCD_KongGaiCount == 0)
                            {
                                MyVariable.robot_RunStep = "420";
                            }
                            else if (MyVariable.CCD_KongGaiCount == 1)
                            {
                                MyVariable.robot_RunStep = "430";
                            }
                            else if (MyVariable.CCD_KongGaiCount == 2)
                            {
                                MyVariable.robot_RunStep = "440";
                            }
                            else if (MyVariable.CCD_KongGaiCount == 3)
                            {
                                MyVariable.robot_RunStep = "450";
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "计数异常，检查程序逻辑", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            MyVariable.CCD_KongGaiCount = 0;
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 865);
                            break;
                        case 865:
                            CheckCurrentRunStatus(0, 865, 865);
                            ReceiveFromRobot(880, 860);
                            break;
                        case 880://给电动夹爪发夹紧信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_daowei, MyVariable.speed1_daowei, MyVariable.acc1_daowei, MyVariable.pos1_daowei, MyVariable.force1_daowei))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 900;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 900://获取电动夹爪状态
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪夹紧;
                            this.RunStep = 905;
                            break;
                        case 905://到下3D相机处扫描PIN针
                            MyVariable.robot_RunStep = "455";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 910);
                            break;
                        case 910:
                            CheckCurrentRunStatus(0, 910, 910);
                            ReceiveFromRobot(915, 905);
                            break;
                        case 915://触发3D相机
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽相机。。。", MsgType.Success, Color.Blue);
                                WaitDelayTime(1);
                                this.RunStep = 917;
                            }
                            else
                            {
                                runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Program.CCDCmd_Down3D, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                                LogCCD(Program.CCDCmd_Down3D, Program.CCDReceived);
                                if (MyVariable.show_IsOpen || MyVariable.newshow_IsOpen)
                                {
                                    RunResultJudge(runRet, 917, 915, 915);
                                }
                                else if (Program.CCDReceived.Contains("NG"))
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "3D相机拍照定位NG", MsgType.Error, Color.Red);
                                    throw new StationErrorException("CCD报警");
                                }
                                else if (Program.CCDReceived.Contains("CY"))
                                {
                                    RunResultJudge(runRet, 917, 915, 915);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "3D相机通讯错误", MsgType.Error, Color.Red);
                                    throw new StationErrorException("CCD报警");
                                }
                            }
                            break;
                        case 917://夹爪旋转
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(2, MyVariable.force2_daowei, MyVariable.speed2_daowei, MyVariable.acc2_daowei, MyVariable.pos2_daowei, 999))
                            {
                                this.RunStep = 919;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人旋转夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationHomeErrException("机器人旋转夹爪运动失败");
                            }
                            break;
                        case 919://旋转夹爪回到待机位
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(2, MyVariable.force2_fuwei, MyVariable.speed2_fuwei, MyVariable.acc2_fuwei, MyVariable.pos2_fuwei, 999))
                            {
                                LoadPicAction7();
                                this.RunStep = 920;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人旋转夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationHomeErrException("机器人旋转夹爪运动失败");
                            }
                            break;
                        case 920://给机器人发关上样孔步骤2(到下相机位置拍照)
                            MyVariable.robot_RunStep = "460";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 925);
                            break;
                        case 925:
                            CheckCurrentRunStatus(0, 925, 925);
                            ReceiveFromRobot(930, 920);
                            break;
                        case 930://下相机拍照
                            WaitDelayTime(0.2);
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽相机。。。", MsgType.Success, Color.Blue);
                                WaitDelayTime(1);
                                this.RunStep = 940;
                            }
                            else
                            {
                                runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Program.CCDCmd_KongGai, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                                LogCCD(Program.CCDCmd_KongGai, Program.CCDReceived);
                                if (MyVariable.show_IsOpen || MyVariable.newshow_IsOpen)
                                {
                                    LoadPicAction2();
                                    RunResultJudge(runRet, 940, 930, 930);
                                }
                                else if (Program.CCDReceived.Contains("NG"))
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "相机拍照定位NG", MsgType.Error, Color.Red);
                                    throw new StationErrorException("CCD报警");
                                }
                                else if (Program.CCDReceived.Contains("CY"))
                                {
                                    LoadPicAction2();
                                    RunResultJudge(runRet, 940, 930, 930);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                    throw new StationErrorException("CCD报警");
                                }
                            }
                            break;
                        case 940://给机器人发关上样孔步骤3(到上相机位置拍照)
                            MyVariable.robot_RunStep = "470";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 945);
                            break;
                        case 945:
                            CheckCurrentRunStatus(0, 945, 945);
                            ReceiveFromRobot(950, 940);
                            break;
                        case 950://上相机拍照
                            WaitDelayTime(0.2);
                            MyVariable.CloseCover_XShift = 0;
                            MyVariable.CloseCover_YShift = 0;
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽相机。。。", MsgType.Success, Color.Blue);
                                WaitDelayTime(1);
                                this.RunStep = 960;
                            }
                            else
                            {
                                runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Program.CCDCmd_ShangYangKong, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                                LogCCD(Program.CCDCmd_ShangYangKong, Program.CCDReceived);
                                if (MyVariable.show_IsOpen || MyVariable.newshow_IsOpen)
                                {
                                    MyVariable.CloseCover_XShift = 0;
                                    MyVariable.CloseCover_YShift = 0;
                                    LogConfig.Instance.ShowMessageToList("Run", "获取位置偏移;X:" + MyVariable.CloseCover_XShift + "Y:" + MyVariable.CloseCover_YShift, MsgType.Success, Color.Brown);
                                    LoadPicAction5();
                                    RunResultJudge(runRet, 960, 950, 950);
                                }
                                else if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                    throw new StationErrorException("CCD报警");
                                }
                                else if (Program.CCDReceived.Contains("NG"))
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "相机拍照定位NG", MsgType.Error, Color.Red);
                                    if (MyVariable.show_IsOpen || MyVariable.newshow_IsOpen)
                                    {
                                        MyVariable.CloseCover_XShift = 0;
                                        MyVariable.CloseCover_YShift = 0;
                                        LogConfig.Instance.ShowMessageToList("Run", "获取位置偏移;X:" + MyVariable.CloseCover_XShift + "Y:" + MyVariable.CloseCover_YShift, MsgType.Success, Color.Brown);
                                        LoadPicAction5();
                                        RunResultJudge(runRet, 960, 950, 950);
                                    }
                                    else
                                    {
                                        throw new StationErrorException("CCD报警");
                                    }
                                }
                                else
                                {
                                    strArray1 = Program.CCDReceived.Split('_');
                                    strArray2 = strArray1[6].Split(',');
                                    if (Math.Abs(Convert.ToDouble(strArray2[0])) >= 2 || Math.Abs(Convert.ToDouble(strArray2[1])) >= 2)
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "相机反馈偏移过大，请检查实际位置！", MsgType.Error, Color.Red);
                                        if (!MyVariable.show_IsOpen)
                                        {
                                            MyVariable.CloseCover_XShift = 0;
                                            MyVariable.CloseCover_YShift = 0;
                                        }
                                        else
                                        {
                                            throw new StationErrorException("CCD报警");
                                        }
                                    }
                                    else
                                    {
                                        MyVariable.CloseCover_XShift = Convert.ToDouble(strArray2[0]);
                                        MyVariable.CloseCover_YShift = Convert.ToDouble(strArray2[1]);
                                    }
                                    LogConfig.Instance.ShowMessageToList("Run", "获取位置偏移;X:" + MyVariable.CloseCover_XShift + "Y:" + MyVariable.CloseCover_YShift, MsgType.Success, Color.Brown);
                                    LoadPicAction5();
                                    RunResultJudge(runRet, 960, 950, 950);
                                }
                            }
                            break;
                        case 960://给机器人发关上样孔步骤4(关闭上样孔)
                            MyVariable.robot_RunStep = "480";
                            if (MyVariable.CloseCover_XShift > 0)//给机器人传递偏移，如果偏移量为正值，则减去正值，如果偏移量为负值，则加上该值的绝对值
                            {
                                MyVariable.robot_XShift = "-" + MyVariable.CloseCover_XShift.ToString("f3");
                            }
                            else
                            {
                                MyVariable.robot_XShift = Math.Abs(MyVariable.CloseCover_XShift).ToString("f3");
                            }
                            if (MyVariable.CloseCover_YShift > 0)
                            {
                                MyVariable.robot_YShift = "-" + MyVariable.CloseCover_YShift.ToString("f3");
                            }
                            else
                            {
                                MyVariable.robot_YShift = Math.Abs(MyVariable.CloseCover_YShift).ToString("f3");
                            }
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 965);
                            break;
                        case 965:
                            CheckCurrentRunStatus(0, 965, 965);
                            ReceiveFromRobot(970, 960);
                            break;
                        case 970://给电动夹爪发松开信号
                            WaitDelayTime(0.2);
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_fuwei, MyVariable.speed1_fuwei, MyVariable.acc1_fuwei, MyVariable.pos1_fuwei, 999))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 975;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 975://获取电动夹爪状态
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪松开;
                            this.RunStep = 976;
                            break;
                        case 976://给机器人发关上样孔步骤5
                            MyVariable.robot_RunStep = "485";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 977);
                            break;
                        case 977:
                            CheckCurrentRunStatus(0, 977, 977);
                            ReceiveFromRobot(979, 976);
                            break;
                        case 979://轴动到3D拍照位
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.上3D线扫位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.上3D线扫位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.上3D线扫位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 980, 979, 979);
                            break;
                        case 980://触发上3D相机拍照
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽相机。。。", MsgType.Success, Color.Blue);
                                WaitDelayTime(1);
                                this.RunStep = 982;
                            }
                            else
                            {
                                runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Program.CCDCmd_Up3D, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                                LogCCD(Program.CCDCmd_Up3D, Program.CCDReceived);
                                if (MyVariable.show_IsOpen || MyVariable.newshow_IsOpen)
                                {
                                    RunResultJudge(runRet, 982, 980, 980);
                                }
                                else if (Program.CCDReceived.Contains("NG"))
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "3D相机拍照定位NG", MsgType.Error, Color.Red);
                                    throw new StationErrorException("CCD报警");
                                }
                                else if (Program.CCDReceived.Contains("CY"))
                                {
                                    RunResultJudge(runRet, 982, 980, 980);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "3D相机通讯错误", MsgType.Error, Color.Red);
                                    throw new StationErrorException("CCD报警");
                                }
                            }
                            break;
                        case 982://给机器人发关上样孔步骤5
                            MyVariable.robot_RunStep = "490";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 985);
                            break;
                        case 985:
                            CheckCurrentRunStatus(0, 985, 985);
                            ReceiveFromRobot(989, 982);
                            break;
                        case 989://轴动到开盖位置
                            LoadPicAction6();
                            SerializeClass.animationParam.sequXMark = Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]);
                            SerializeClass.animationParam.material3 = (int)_PointArray.开关盖位置;
                            runRet = WaitSingleAxisAbsMove(_SequencingStationAxis.测序仪XAxis.ToString(),
                                     Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.开关盖位置.ToString()].PosList[(int)_SequencingStationAxis.测序仪XAxis]),
                                     Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
                            RunResultJudge(runRet, 990, 989, 989);
                            break;
                        case 990:
                            if (MyVariable.RobotStation_Replace)
                            {
                                MyVariable.RobotStation_Replace = false;
                                SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.开预处理孔盖中;
                                SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪默认松开;
                                this.RunStep = 160;
                            }
                            else
                            {
                                this.RunStep = 995;
                            }
                            break;
                        case 995:
                            MyVariable.RobotWorkDone = true;
                            this.RunStep = 1200;
                            break;
                        #endregion

                        #region 开盖完回待机位置
                        case 1000:
                            this.RunStep = 1010;
                            break;
                        case 1010://给机器人发回待机位置
                            MyVariable.robot_RunStep = "0";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 1015);
                            break;
                        case 1015:
                            CheckCurrentRunStatus(0, 1015, 1015);
                            ReceiveFromRobot(1020, 1010);
                            break;
                        case 1020:
                            SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.开盖完成;
                            LogConfig.Instance.ShowMessageToList("Run", "开盖完成", MsgType.Success, Color.Green);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.机器人开盖完成;
                            this.RunStep = 1040;
                            break;
                        case 1040:
                            CheckCurrentRunStatus(0, 1040, 1040);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.开盖完成
                               || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲)

                            {
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region 关盖完回待机位置
                        case 1200:
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.测序配置完成 && SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.可关上样孔)
                            {
                                if (MyVariable.newshow_IsOpen)
                                {
                                    this.RunStep = 1210;
                                    MyVariable.show_IsOpen = true;
                                }
                                else
                                {
                                    this.RunStep = 1220;
                                }
                            }
                            else
                            {
                                this.RunStep = 1210;
                            }
                            break;
                        case 1210://给机器人发回待机位置
                            MyVariable.robot_RunStep = "0";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 1215);
                            break;
                        case 1215:
                            CheckCurrentRunStatus(0, 1215, 1215);
                            ReceiveFromRobot(1220, 1210);
                            break;
                        case 1220:
                            SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.关盖完成;
                            LogConfig.Instance.ShowMessageToList("Run", "关盖完成", MsgType.Success, Color.Green);
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.机器人关盖完成;
                            this.RunStep = 1240;
                            break;
                        case 1240:
                            CheckCurrentRunStatus(0, 1240, 1240);
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.关盖完成
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.继续关预处理孔
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序中
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲)
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
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Pause);
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
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunStep = 0;
                    this.RunDone = true;
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
                catch (MainstationPauseException ex)
                {
                    //this.RunStep = 0;
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    //this.RunDone = true;
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Pause);
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
            StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Stop);
        }


        /// <summary>
        /// 上相机标定模式
        /// </summary>
        public override void StationCalibRun()
        {
            this.RunDone = false;
            this.time = this.GetCurveTime();//设备暂停，异常状态后机器人执行暂停工程，重新开启后机器人继续运行，计时刷新
            StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0:
                            CheckCurrentRunStatus(0, 0, 0);
                            if (MyVariable.CalibRun_Run)
                            {
                                MyVariable.CalibRun_Run = false;
                                this.RunStep = 120;
                            }
                            break;
                        case 120://给机器人发公共位置
                            MyVariable.robot_RunStep = "10";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 125);
                            break;
                        case 125://等待机器人反馈
                            CheckCurrentRunStatus(0, 125, 125);
                            ReceiveFromRobot(200, 120);
                            break;
                        case 200://给机器人发标定步骤1
                            MyVariable.robot_RunStep = "10010";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 210);
                            break;
                        case 210:
                            CheckCurrentRunStatus(0, 210, 210);
                            ReceiveFromRobotCalib(215, 200, out currentpos_Robot);
                            break;
                        case 215://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 220, 215, 215);
                            break;
                        case 220://给机器人发标定步骤2
                            MyVariable.robot_RunStep = "10020";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 225);
                            break;
                        case 225:
                            CheckCurrentRunStatus(0, 225, 225);
                            ReceiveFromRobotCalib(230, 220, out currentpos_Robot);
                            break;
                        case 230://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 240, 230, 230);
                            break;
                        case 240://给机器人发标定步骤3
                            MyVariable.robot_RunStep = "10030";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 250);
                            break;
                        case 250:
                            CheckCurrentRunStatus(0, 250, 250);
                            ReceiveFromRobotCalib(260, 240, out currentpos_Robot);
                            break;
                        case 260://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 270, 260, 260);
                            break;
                        case 270://给机器人发标定步骤4
                            MyVariable.robot_RunStep = "10040";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 280);
                            break;
                        case 280:
                            CheckCurrentRunStatus(0, 280, 280);
                            ReceiveFromRobotCalib(290, 270, out currentpos_Robot);
                            break;
                        case 290://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 300, 290, 290);
                            break;

                        case 300://给机器人发标定步骤5
                            MyVariable.robot_RunStep = "10050";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 310);
                            break;
                        case 310:
                            CheckCurrentRunStatus(0, 310, 310);
                            ReceiveFromRobotCalib(320, 300, out currentpos_Robot);
                            break;
                        case 320://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 330, 320, 320);
                            break;
                        case 330://给机器人发标定步骤6
                            MyVariable.robot_RunStep = "10060";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 340);
                            break;
                        case 340:
                            CheckCurrentRunStatus(0, 340, 340);
                            ReceiveFromRobotCalib(350, 330, out currentpos_Robot);
                            break;
                        case 350://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 360, 350, 350);
                            break;
                        case 360://给机器人发标定步骤7
                            MyVariable.robot_RunStep = "10070";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 370);
                            break;
                        case 370:
                            CheckCurrentRunStatus(0, 370, 370);
                            ReceiveFromRobotCalib(380, 360, out currentpos_Robot);
                            break;
                        case 380://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 390, 380, 380);
                            break;
                        case 390://给机器人发标定步骤8
                            MyVariable.robot_RunStep = "10080";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 400);
                            break;
                        case 400:
                            CheckCurrentRunStatus(0, 400, 400);
                            ReceiveFromRobotCalib(410, 390, out currentpos_Robot);
                            break;
                        case 410://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 420, 410, 410);
                            break;
                        case 420://给机器人发标定步骤9
                            MyVariable.robot_RunStep = "10090";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 430);
                            break;
                        case 430:
                            CheckCurrentRunStatus(0, 430, 430);
                            ReceiveFromRobotCalib(440, 420, out currentpos_Robot);
                            break;
                        case 440://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_01_02_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 450, 440, 440);
                            break;
                        case 450:
                            MyVariable.CalibRun_RunDone = true;
                            this.RunStep = 460;
                            break;
                        case 460://给机器人发回待机位置
                            MyVariable.robot_RunStep = "0";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 470);
                            break;
                        case 470:
                            CheckCurrentRunStatus(0, 470, 470);
                            ReceiveFromRobot(480, 460);
                            break;
                        case 480:
                            LogConfig.Instance.ShowMessageToList("Run", "上相机标定完成", MsgType.Success, Color.Green);
                            throw new StationWorkDone("");
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunStep = 0;
                    this.RunDone = true;
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }
        /// <summary>
        /// 下相机标定模式
        /// </summary>
        public override void StationCPKRun()
        {
            this.RunDone = false;
            this.time = this.GetCurveTime();//设备暂停，异常状态后机器人执行暂停工程，重新开启后机器人继续运行，计时刷新
            StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0:
                            CheckCurrentRunStatus(0, 0, 0);
                            if (MyVariable.CalibRun_Run)
                            {
                                MyVariable.CalibRun_Run = false;
                                this.RunStep = 120;
                            }
                            break;
                        case 120://给机器人发公共位置
                            MyVariable.robot_RunStep = "10";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 125);
                            break;
                        case 125://等待机器人反馈
                            CheckCurrentRunStatus(0, 125, 125);
                            ReceiveFromRobot(130, 120);
                            break;
                        case 130://取上样孔盖位置
                            MyVariable.robot_RunStep = "20000";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 135);
                            break;
                        case 135://等待机器人反馈
                            CheckCurrentRunStatus(0, 135, 135);
                            ReceiveFromRobotCalib(140, 130, out currentpos_Robot);
                            break;
                        case 140://给电动夹爪发夹紧信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_daowei, MyVariable.speed1_daowei, MyVariable.acc1_daowei, MyVariable.pos1_daowei, MyVariable.force1_daowei))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.夹紧;
                                this.RunStep = 150;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 150:
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪夹紧;
                            this.RunStep = 160;
                            break;
                        case 160://机器人上升
                            MyVariable.robot_RunStep = "20005";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 165);
                            break;
                        case 165://等待机器人反馈
                            CheckCurrentRunStatus(0, 165, 165);
                            ReceiveFromRobotCalib(200, 160, out currentpos_Robot);
                            break;
                        case 200://给机器人发标定步骤1
                            MyVariable.robot_RunStep = "20010";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 210);
                            break;
                        case 210:
                            CheckCurrentRunStatus(0, 210, 210);
                            ReceiveFromRobotCalib(215, 200, out currentpos_Robot);
                            break;
                        case 215://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 220, 215, 215);
                            break;
                        case 220://给机器人发标定步骤2
                            MyVariable.robot_RunStep = "20020";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 225);
                            break;
                        case 225:
                            CheckCurrentRunStatus(0, 225, 225);
                            ReceiveFromRobotCalib(230, 220, out currentpos_Robot);
                            break;
                        case 230://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 240, 230, 230);
                            break;
                        case 240://给机器人发标定步骤3
                            MyVariable.robot_RunStep = "20030";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 250);
                            break;
                        case 250:
                            CheckCurrentRunStatus(0, 250, 250);
                            ReceiveFromRobotCalib(260, 240, out currentpos_Robot);
                            break;
                        case 260://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 270, 260, 260);
                            break;
                        case 270://给机器人发标定步骤4
                            MyVariable.robot_RunStep = "20040";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 280);
                            break;
                        case 280:
                            CheckCurrentRunStatus(0, 280, 280);
                            ReceiveFromRobotCalib(290, 270, out currentpos_Robot);
                            break;
                        case 290://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 300, 290, 290);
                            break;
                        case 300://给机器人发标定步骤5
                            MyVariable.robot_RunStep = "20050";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 310);
                            break;
                        case 310:
                            CheckCurrentRunStatus(0, 310, 310);
                            ReceiveFromRobotCalib(320, 300, out currentpos_Robot);
                            break;
                        case 320://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 330, 320, 320);
                            break;
                        case 330://给机器人发标定步骤6
                            MyVariable.robot_RunStep = "20060";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 340);
                            break;
                        case 340:
                            CheckCurrentRunStatus(0, 340, 340);
                            ReceiveFromRobotCalib(350, 330, out currentpos_Robot);
                            break;
                        case 350://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 360, 350, 350);
                            break;
                        case 360://给机器人发标定步骤7
                            MyVariable.robot_RunStep = "20070";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 370);
                            break;
                        case 370:
                            CheckCurrentRunStatus(0, 370, 370);
                            ReceiveFromRobotCalib(380, 360, out currentpos_Robot);
                            break;
                        case 380://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 390, 380, 380);
                            break;
                        case 390://给机器人发标定步骤8
                            MyVariable.robot_RunStep = "20080";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 400);
                            break;
                        case 400:
                            CheckCurrentRunStatus(0, 400, 400);
                            ReceiveFromRobotCalib(410, 390, out currentpos_Robot);
                            break;
                        case 410://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 420, 410, 410);
                            break;
                        case 420://给机器人发标定步骤9
                            MyVariable.robot_RunStep = "20090";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 430);
                            break;
                        case 430:
                            CheckCurrentRunStatus(0, 430, 430);
                            ReceiveFromRobotCalib(440, 420, out currentpos_Robot);
                            break;
                        case 440://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 600, 440, 440);
                            break;

                        #region  14点标定
                        case 450://给机器人发标定步骤10
                            MyVariable.robot_RunStep = "20100";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 460);
                            break;
                        case 460:
                            CheckCurrentRunStatus(0, 460, 460);
                            ReceiveFromRobotCalib(470, 450, out currentpos_Robot);
                            break;
                        case 470://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 480, 470, 470);
                            break;
                        case 480://给机器人发标定步骤11
                            MyVariable.robot_RunStep = "20110";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 490);
                            break;
                        case 490:
                            CheckCurrentRunStatus(0, 490, 490);
                            ReceiveFromRobotCalib(500, 480, out currentpos_Robot);
                            break;
                        case 500://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 510, 500, 500);
                            break;
                        case 510://给机器人发标定步骤12
                            MyVariable.robot_RunStep = "20120";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 520);
                            break;
                        case 520:
                            CheckCurrentRunStatus(0, 520, 520);
                            ReceiveFromRobotCalib(530, 510, out currentpos_Robot);
                            break;
                        case 530://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 540, 530, 530);
                            break;
                        case 540://给机器人发标定步骤13
                            MyVariable.robot_RunStep = "20130";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 550);
                            break;
                        case 550:
                            CheckCurrentRunStatus(0, 550, 550);
                            ReceiveFromRobotCalib(560, 540, out currentpos_Robot);
                            break;
                        case 560://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 570, 560, 560);
                            break;
                        case 570://给机器人发标定步骤14
                            MyVariable.robot_RunStep = "20140";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 580);
                            break;
                        case 580:
                            CheckCurrentRunStatus(0, 580, 580);
                            ReceiveFromRobotCalib(590, 570, out currentpos_Robot);
                            break;
                        case 590://相机拍照
                            WaitDelayTime(0.6);
                            LogConfig.Instance.ShowMessageToList("Run", "机器人当前位置:" + currentpos_Robot, MsgType.Success, Color.Blue);
                            Calib_CCDsend = $"CY_TCal_02_01_{currentpos_Robot}_WZ";
                            runRet = WaitNetData(_TcpClientModule.CCD.ToString(), Calib_CCDsend, Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue), out Program.CCDReceived);
                            LogCCD(Calib_CCDsend, Program.CCDReceived);
                            if (runRet != _ActionResult.结果OK || Program.CCDReceived == "")
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "相机通讯错误", MsgType.Error, Color.Red);
                                throw new StationErrorException("CCD报警");
                            }
                            //LoadPicAction();
                            RunResultJudge(runRet, 600, 590, 590);
                            break;
                        #endregion


                        case 600://返回取盖处
                            MyVariable.robot_RunStep = "20150";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 610);
                            break;
                        case 610:
                            CheckCurrentRunStatus(0, 610, 610);
                            ReceiveFromRobotCalib(620, 600, out currentpos_Robot);
                            break;
                        case 620://给电动夹爪发松开信号
                            if (SoftWareForm.m_RobotNewClaw.WaitRobotClawRun(1, MyVariable.force1_fuwei, MyVariable.speed1_fuwei, MyVariable.acc1_fuwei, MyVariable.pos1_fuwei, 999))
                            {
                                SerializeClass.animationParam.robotClawStatus = (int)_ClawStatusEnum.松开;
                                this.RunStep = 630;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机器人夹紧夹爪运动失败", MsgType.Success, Color.Red);
                                throw new StationErrorException("电动夹爪报警");
                            }
                            break;
                        case 630://获取电动夹爪状态
                            SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪松开;
                            this.RunStep = 640;
                            break;
                        case 640://机器人上升
                            MyVariable.robot_RunStep = "20160";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 650);
                            break;
                        case 650:
                            CheckCurrentRunStatus(0, 650, 650);
                            ReceiveFromRobotCalib(660, 640, out currentpos_Robot);
                            break;
                        case 660:
                            MyVariable.CalibRun_RunDone = true;
                            this.RunStep = 670;
                            break;
                        case 670://给机器人发回待机位置
                            MyVariable.robot_RunStep = "0";
                            MyVariable.robot_XShift = "0";
                            MyVariable.robot_YShift = "0";
                            MyVariable.robot_ZShift = "0";
                            MyVariable.robot_RunCmd = MyVariable.robot_RunStep + "," + MyVariable.robot_XShift + "," + MyVariable.robot_YShift + "," + MyVariable.robot_ZShift;
                            SendToRobot(MyVariable.robot_RunCmd, 680);
                            break;
                        case 680:
                            CheckCurrentRunStatus(0, 680, 680);
                            ReceiveFromRobot(690, 670);
                            break;
                        case 690:
                            LogConfig.Instance.ShowMessageToList("Run", "下相机标定完成", MsgType.Success, Color.Green);
                            throw new StationWorkDone("");
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.机器人工位.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunStep = 0;
                    this.RunDone = true;
                    StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].ChangeStatus(_StationStatus.Stop);
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
        /// 给机器人发送指令
        /// </summary>
        /// <param name="sendMsg">指令内容</param>
        /// <param name="nextstep">下一个步序</param>
        public void SendToRobot(string sendMsg, int nextstep)
        {
            TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).ClearNetData();
            if (TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).WriteDataStr(sendMsg))
            {
                this.time = this.GetCurveTime();
                this.RunStep = nextstep;
            }
            else
            {
                LogConfig.Instance.ShowMessageToList("Run", "向机器人发送数据失败", MsgType.Error, Color.Red);
                throw new StationErrorException("机器人报警");
            }
        }
        /// <summary>
        /// 机器人反馈
        /// </summary>
        /// <param name="nextstep">下一个步序</param>
        /// <param name="errorstep">异常步序</param>
        public void ReceiveFromRobot(int nextstep, int errorstep)
        {
            if (OverTimeS(time, Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.RobotTimeOut.ToString()].CurrentValue)))
            {
                LogConfig.Instance.ShowMessageToList("Run", "机器人反馈数据超时", MsgType.Error, Color.Red);
                this.RunStep = errorstep;
                throw new StationErrorException("机器人报警");
            }
            else
            {
                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).NetCanRead())
                {
                    TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).LoopReadData(1, out recRobotMsg);
                    if (recRobotMsg.Contains("OK"))
                    {
                        this.RunStep = nextstep;
                    }
                    else
                    {
                        LogConfig.Instance.ShowMessageToList("Run", "机器人反馈数据异常" + recRobotMsg, MsgType.Error, Color.Red);
                        this.RunStep = errorstep;
                        throw new StationErrorException("机器人报警");
                    }
                }
            }
        }


        /// <summary>
        /// 机器人反馈
        /// </summary>
        /// <param name="nextstep">下一个步序</param>
        /// <param name="errorstep">异常步序</param>
        /// <param name="pos">机器人坐标</param>
        public void ReceiveFromRobotCalib(int nextstep, int errorstep, out string pos)
        {
            pos = "";
            if (OverTimeS(time, Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.RobotTimeOut.ToString()].CurrentValue)))
            {
                LogConfig.Instance.ShowMessageToList("Run", "机器人反馈数据超时", MsgType.Error, Color.Red);
                this.RunStep = errorstep;
                throw new StationErrorException("机器人报警");
            }
            else
            {
                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).NetCanRead())
                {
                    TCPClientConfig.Instance.GetClient(_TcpClientModule.RobotProject.ToString()).LoopReadData(1, out recRobotMsg);
                    if (recRobotMsg != "")
                    {
                        pos = recRobotMsg;
                        this.RunStep = nextstep;
                    }
                    else
                    {
                        LogConfig.Instance.ShowMessageToList("Run", "机器人反馈数据异常" + recRobotMsg, MsgType.Error, Color.Red);
                        this.RunStep = errorstep;
                        throw new StationErrorException("机器人报警");
                    }
                }
            }
        }



        /// <summary>
        /// CCD日志
        /// </summary>
        /// <param name="sendmsg">发送内容</param>
        /// <param name="receivemsg">接收内容</param>
        private void LogCCD(string sendmsg, string receivemsg)
        {
            string NowDate = string.Format("{0:yyyyMMdd}", DateTime.Now);//获取当前日期
            if (!Directory.Exists(@"E:\SWLog\CCD\"))
            {
                Directory.CreateDirectory(@"E:\SWLog\CCD\");
            }
            if (!File.Exists(@"E:\SWLog\CCD\" + NowDate + ".txt"))
            {
                File.Create(@"E:\SWLog\CCD\" + NowDate + ".txt").Close();
            }
            if (File.Exists(@"E:\SWLog\CCD\" + NowDate + ".txt"))
            {
                using (FileStream fsWrite = new FileStream(@"E:\SWLog\CCD\" + NowDate + ".txt", FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fsWrite, Encoding.Unicode))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  PC-->CCD  " + sendmsg);
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  CCD-->PC  " + receivemsg);
                    }
                }
            }
        }

    }
}

