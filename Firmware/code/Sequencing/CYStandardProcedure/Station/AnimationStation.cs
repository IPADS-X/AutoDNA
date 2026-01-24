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
using System.Xml.Linq;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public class AnimationStation : ObjectStation
    {
        private string mName;
        private _ActionResult resetRet;//单步复位结果
        private _ActionResult runRet;//单步运行结果

        private string sendmsg;//给数字孪生发送的信息
        private int length;//字符长度
        private double second = 0;//秒数
        private double pipettePreviousPos = 0;//移液枪Z轴上一次位置

        int[] result2 = null;
        float curRobPos;
        Dictionary<string, double> CurPosDic = new Dictionary<string, double>();
        AuboRobot.wayPoint_S waypoint = new AuboRobot.wayPoint_S();
        AuboRobot.Ori ori = new AuboRobot.Ori();
        AuboRobot.Rpy rpy = new AuboRobot.Rpy();
        double M_PI = 3.14159265358979323846;
        Thread th;//数字孪生线程
        private int animationStep = 0;
        private bool animationMark = false;//数字孪生线程启动标志

        public AnimationStation(string name) :
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
            StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Initial);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.ResetStep)
                    {
                        case 0:
                            LogConfig.Instance.ShowMessageToList("Run", "数字孪生线程开始复位", MsgType.Success, Color.Blue);
                            this.ResetStep = 10;
                            break;
                        case 10:
                            if (!animationMark)
                            {
                                th = new Thread(AnimationThread);
                                th.IsBackground = true;
                                th.Start();
                                LogConfig.Instance.ShowMessageToList("Run", "数字孪生交互线程已开启", MsgType.Success, Color.Green);
                            }
                            this.ResetStep = 200;
                            break;
                        case 200:
                            throw new StationHomeOK("数字孪生线程复位完成！");
                    }
                }
                /***子线程复位失败跳转到这里***/
                catch (StationHomeErrException ex)
                {
                    //LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.数字孪生线程.ToString()+ ex.Message, MsgType.Error, Color.Red);
                    this.ResetError = true;
                    StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
                /***子线程复位完成跳转到这里***/
                catch (StationHomeOK ex)
                {
                    this.ResetStep = 0;
                    this.ResetDone = true;
                    LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Success, Color.Green);
                    StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Stop);
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
            StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0:
                            CheckCurrentRunStatus(0, 0, 0);
                            this.RunStep = 100;
                            break;
                        case 100:
                            CheckCurrentRunStatus(0, 100, 100);
                            if (animationMark)
                            {
                                this.RunStep = 0;
                            }
                            else
                            {
                                this.RunStep = 0;
                            }
                            break;
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.数字孪生线程.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Stop);
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
            StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0:
                            CheckCurrentRunStatus(0, 0, 0);
                            this.RunStep = 100;
                            break;
                        case 100:
                            CheckCurrentRunStatus(0, 100, 100);
                            if (animationMark)
                            {
                                this.RunStep = 0;
                            }
                            else
                            {
                                this.RunStep = 0;
                            }
                            break;
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.数字孪生线程.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        public override void StationCalibRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Stop);
        }

        public override void StationCPKRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].ChangeStatus(_StationStatus.Stop);
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

        private void WritemAnimationLog(string sendmsg)
        {
            string NowDate = string.Format("{0:yyyyMMdd}", DateTime.Now);//获取当前日期
            if (!Directory.Exists(@"E:\SWLog\Animation\"))
            {
                Directory.CreateDirectory(@"E:\SWLog\Animation\");
            }
            if (!File.Exists(@"E:\SWLog\Animation\" + NowDate + ".txt"))
            {
                File.Create(@"E:\SWLog\Animation\" + NowDate + ".txt").Close();
            }
            if (File.Exists(@"E:\SWLog\Animation\" + NowDate + ".txt"))
            {
                using (FileStream fsWrite = new FileStream(@"E:\SWLog\Animation\" + NowDate + ".txt", FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fsWrite, Encoding.Unicode))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  PC-->3D  " + sendmsg);
                    }
                }
            }
        }

        public void AnimationThread()
        {
            animationMark = true;
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (animationStep)
                    {
                        #region 获取实验步序
                        case 0:
                            if (
                                SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲
                                && SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.空闲
                                && SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲
                                && SerializeClass.mMemory.DataProcessingStation_state == MemoryClass.DataProcessingStation_State.空闲
                                && SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲
                                )
                            {
                                SerializeClass.animationParam.taskStep = (int)_taskStepEnum.无实验任务;
                                SerializeClass.animationParam.waitStep = (int)_waitStepEnum.无等待时间;
                                SerializeClass.animationParam.alarmMsg = (int)_alarmMsgEnum.无报警;
                                SerializeClass.animationParam.RemainTime = 0;
                            }
                            animationStep = 40;
                            break;
                        #endregion

                        #region 计算移液枪坐标
                        case 40:
                            WaitDelayTime(double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.AnimationTime.ToString()].CurrentValue) - 0.1);
                            second = second + double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.AnimationTime.ToString()].CurrentValue);
                            if (pipettePreviousPos != SerializeClass.animationParam.gunZMark)
                            {
                                second = double.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.AnimationTime.ToString()].CurrentValue);
                                pipettePreviousPos = SerializeClass.animationParam.gunZMark;
                            }
                            animationStep = 50;
                            break;
                        #endregion

                        #region 获取所有轴坐标并按规定格式整理
                        case 50://获取当前轴坐标

                            #region 获取轴当前位置
                            SerializeClass.animationParam.carryXCur = Math.Round(MotionConfig.Instance.CurPos[(int)_Axis.搬运XAxis], 3);
                            SerializeClass.animationParam.carryYCur = Math.Round(MotionConfig.Instance.CurPos[(int)_Axis.搬运YAxis], 3);
                            SerializeClass.animationParam.carryZCur = Math.Round(MotionConfig.Instance.CurPos[(int)_Axis.搬运ZAxis], 3);
                            SerializeClass.animationParam.sequXCur = Math.Round(MotionConfig.Instance.CurPos[(int)_Axis.测序仪XAxis], 3);
                            #endregion

                            #region 获取移液枪Z轴当前位置
                            if (((MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2) * second >= SerializeClass.animationParam.gunZMark)
                            {
                                SerializeClass.animationParam.gunZCur = Math.Round(SerializeClass.animationParam.gunZMark / 1000, 3);
                            }
                            else
                            {
                                SerializeClass.animationParam.gunZCur = Math.Round(((MyVariable.z_movepos_speed + MyVariable.z_check_speed) / 2000) * second, 3);
                            }
                            #endregion

                            #region 获取电动夹爪当前位置
                            result2 = null;
                            result2 = SoftWareForm.carryclaw_initialize.Rtu_carryClaw.ReadInputRegInt(Program.carryClawConfig.DevAdd, 0, 1);
                            if (result2 != null && result2.Length == 1)
                            {
                                SerializeClass.animationParam.carryClawCur = Math.Round(Convert.ToDouble(result2[0]) / 1000, 3);   //单位mm
                            }
                            if (SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 0, "04", out curRobPos))
                            {
                                SerializeClass.animationParam.robotClawCur = Math.Round(curRobPos, 3);   //单位mm
                            }
                            #endregion

                            #region 获取机器人关节实时角度
                            CurPosDic = AuboClass.Instance.GetCurrentPos(waypoint, ori, rpy, M_PI);
                            SerializeClass.animationParam.robot1Cur = Math.Round(CurPosDic["joint1"], 3);
                            SerializeClass.animationParam.robot2Cur = Math.Round(CurPosDic["joint2"], 3);
                            SerializeClass.animationParam.robot3Cur = Math.Round(CurPosDic["joint3"], 3);
                            SerializeClass.animationParam.robot4Cur = Math.Round(CurPosDic["joint4"], 3);
                            SerializeClass.animationParam.robot5Cur = Math.Round(CurPosDic["joint5"], 3);
                            SerializeClass.animationParam.robot6Cur = Math.Round(CurPosDic["joint6"], 3);
                            #endregion

                            #region 监控轴启动信号
                            if (MotionConfig.Instance.MotionStatusList[(int)_Axis.搬运XAxis].Moving)
                            {
                                SerializeClass.animationParam.carryXStart = (int)_AxisStartSignEnum.启动;
                            }
                            else
                            {
                                SerializeClass.animationParam.carryXStart = (int)_AxisStartSignEnum.停止;
                            }
                            if (MotionConfig.Instance.MotionStatusList[(int)_Axis.搬运YAxis].Moving)
                            {
                                SerializeClass.animationParam.carryYStart = (int)_AxisStartSignEnum.启动;
                            }
                            else
                            {
                                SerializeClass.animationParam.carryYStart = (int)_AxisStartSignEnum.停止;
                            }
                            if (MotionConfig.Instance.MotionStatusList[(int)_Axis.搬运ZAxis].Moving)
                            {
                                SerializeClass.animationParam.carryZStart = (int)_AxisStartSignEnum.启动;
                            }
                            else
                            {
                                SerializeClass.animationParam.carryZStart = (int)_AxisStartSignEnum.停止;
                            }
                            if (MotionConfig.Instance.MotionStatusList[(int)_Axis.测序仪XAxis].Moving)
                            {
                                SerializeClass.animationParam.sequXStart = (int)_AxisStartSignEnum.启动;
                            }
                            else
                            {
                                SerializeClass.animationParam.sequXStart = (int)_AxisStartSignEnum.停止;
                            }
                            #endregion

                            //转成json格式
                            sendmsg = JsonConvert.SerializeObject(SerializeClass.animationParam);
                            length = sendmsg.Length;
                            if (length < 10)
                            {
                                sendmsg = "000" + length + "_7" + sendmsg;
                            }
                            else if (length >= 10 && length < 100)
                            {
                                sendmsg = "00" + length + "_7" + sendmsg;
                            }
                            else if (length >= 100 && length < 1000)
                            {
                                sendmsg = "0" + length + "_7" + sendmsg;
                            }
                            else
                            {
                                sendmsg = length + "_7" + sendmsg;
                            }
                            animationStep = 100;
                            break;
                        #endregion

                        case 100://给数字孪生发送坐标
                            if (Convert.ToBoolean(Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledAnimation.ToString()].CurrentValue)))
                            {
                                // WritemAnimationLog(sendmsg);
                                animationStep = 0;
                            }
                            else
                            {
                                //WritemAnimationLog(sendmsg);
                                try
                                {
                                    TCPClientConfig.Instance.GetClient(_TcpClientModule.Animation.ToString()).ClearNetData();
                                    if (TCPClientConfig.Instance.GetClient(_TcpClientModule.Animation.ToString()).WriteDataStr(sendmsg))
                                    {
                                        animationStep = 0;
                                    }
                                    else
                                    {
                                        TCPClientConfig.Instance.ReConnectClient(_TcpClientModule.Animation.ToString());
                                        animationStep = 0;
                                    }
                                }
                                catch (Exception)
                                {
                                    TCPClientConfig.Instance.ReConnectClient(_TcpClientModule.Animation.ToString());
                                    animationStep = 0;
                                }
                            }
                            break;
                    }
                }
                /***暂停捕获***/
                catch (Exception ex)
                {
                    break;
                }
            }

        }
    }
}

