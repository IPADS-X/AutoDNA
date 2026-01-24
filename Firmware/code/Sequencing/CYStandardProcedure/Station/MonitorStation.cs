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

namespace CYStandardProcedure
{
    public class MonitorStation : ObjectStation
    {
        private string mName;
        private _ActionResult resetRet;//单步复位结果
        private _ActionResult runRet;//单步运行结果
        private int num;//重连计数
        Thread HaoCaiThread;
        private bool threadMark;
        Stopwatch swcontrol = new Stopwatch();
        public MonitorStation(string name) :
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
            StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Initial);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.ResetStep)
                    {
                        case 0:
                            LogConfig.Instance.ShowMessageToList("Run", "状态监控线程开始复位", MsgType.Success, Color.Blue);

                            this.ResetStep = 200;
                            break;
                        case 200:
                            throw new StationHomeOK("状态监控线程复位完成！");
                    }
                }
                /***子线程复位失败跳转到这里***/
                catch (StationHomeErrException ex)
                {
                    //LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.状态监控线程.ToString()+ ex.Message, MsgType.Error, Color.Red);
                    this.ResetError = true;
                    StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
                /***子线程复位完成跳转到这里***/
                catch (StationHomeOK ex)
                {
                    this.ResetStep = 0;
                    this.ResetDone = true;
                    LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Success, Color.Green);
                    StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        /// <summary>
        /// 单站运行动作
        /// </summary>
        public override void StationNormalRun()
        {
            MyVariable.num = 0;
            this.RunDone = false;
            StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0://启动
                            CheckCurrentRunStatus(0, 0, 0);
                            this.RunStep = 10;
                            break;

                        #region 实时计算刷新耗材数量（Tip头，离心管）
                        case 10://单独开线程，实时计算耗材数量
                            if (!threadMark)
                            {
                                HaoCaiThread = new Thread(HaoCaiAutoThread);
                                HaoCaiThread.IsBackground = true;
                                HaoCaiThread.Start();
                            }
                            this.RunStep = 20;
                            break;
                        #endregion

                        #region PLC实时交互，刷新载具进出料区有无载具存留
                        case 20://刷新载具进出料区有无载具存留
                            CheckCurrentRunStatus(0, 20, 20);
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头出料区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头出料区光电2])
                                {
                                    if (!Program.modbusTcp_PLC.WriteSingleRegister(1, 64603, 1))
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "PLC地址写入失败，检查连接", MsgType.Success, Color.Red);
                                        throw new StationErrorException("通讯报警");
                                    }
                                }
                                else
                                {
                                    if (!Program.modbusTcp_PLC.WriteSingleRegister(1, 64603, 0))
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "PLC地址写入失败，检查连接", MsgType.Success, Color.Red);
                                        throw new StationErrorException("通讯报警");
                                    }
                                }
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头进料区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头进料区光电2])
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64604, 1);
                                }
                                else
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64604, 0);
                                }
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电2])
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64605, 1);
                                }
                                else
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64605, 0);
                                }
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.进料区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.进料区光电2])
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64606, 1);
                                }
                                else
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64606, 0);
                                }
                            }
                            this.RunStep = 30;
                            break;
                        #endregion

                        #region 实时监控机器人状态
                        case 30://监控机器人状态
                            CheckCurrentRunStatus(0, 30, 30);
                            if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.Aubo机器人系统错误] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.Aubo机器人紧急停止])
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "Aubo机器人报警", MsgType.Success, Color.Red);
                                throw new StationAlarmException("机器人报警");
                            }
                            else
                            {
                                this.RunStep = 50;
                            }
                            break;
                        #endregion

                        //#region 实时监控数字孪生连接状态
                        //case 40://实时监控数字孪生连接状态
                        //    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledAnimation.ToString()].CurrentValue)))
                        //    {
                        //        if (!TCPClientConfig.Instance.GetClient(_TcpClientModule.Animation.ToString()).IsOpen())
                        //        {
                        //            if (num >= 1)
                        //            {
                        //                num = 0;
                        //                LogConfig.Instance.ShowMessageToList("Run", "数字孪生连接断开", MsgType.Success, Color.Red);
                        //                throw new StationErrorException("");
                        //            }
                        //            else
                        //            {
                        //                num++;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            this.RunStep = 50;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        this.RunStep = 50;
                        //    }
                        //    break;
                        //#endregion

                        #region 实时监控温控表连接状态
                        case 50:
                            CheckCurrentRunStatus(0, 50, 50);
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledTemperature.ToString()].CurrentValue)))
                            {
                                if (MyVariable.b_temperature)
                                {
                                    SerialConfig.Instance.ReOpenSerial(_SerialModule.TemperatureControl.ToString());
                                    MyVariable.b_temperature = false;
                                    LogConfig.Instance.ShowMessageToList("Run", "温控表读取失败，检查温控表通讯", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            this.RunStep = 60;
                            break;
                        #endregion

                        #region 实时监控总控连接状态
                        case 60://实时监控总控连接状态
                            CheckCurrentRunStatus(0, 60, 60);
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                            {
                                if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲
                                 && SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.空闲
                                 && SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲
                                 && SerializeClass.mMemory.DataProcessingStation_state == MemoryClass.DataProcessingStation_State.空闲
                                 && SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                                {
                                    swcontrol.Start();
                                    if (swcontrol.ElapsedMilliseconds / 1000 >= 20)
                                    {
                                        swcontrol.Reset();
                                        if (!TCPClientConfig.Instance.ReConnectClient(_TcpClientModule.GeneralControl.ToString()))
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "总控连接断开", MsgType.Success, Color.Red);
                                            throw new StationErrorException("通讯报警");
                                        }
                                    }
                                }
                            }
                            this.RunStep = 70;
                            break;
                        #endregion
                        case 70:
                            CheckCurrentRunStatus(0, 70, 70);
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) && !MyVariable.show_IsOpen)
                            {
                                if (!MyVariable.b_StatusToControl)
                                {
                                    if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲
                                     && SerializeClass.mMemory.FeedingStation_state == MemoryClass.FeedingStation_State.空闲
                                     && SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲
                                     && SerializeClass.mMemory.DataProcessingStation_state == MemoryClass.DataProcessingStation_State.空闲
                                     && SerializeClass.mMemory.RobotStation_state == MemoryClass.RobotStation_State.空闲)
                                    {
                                        if (MyVariable.ToGeneralStatus(1))
                                        {
                                            MyVariable.b_StatusToControl = true;
                                            LogConfig.Instance.ShowMessageToList("Run", "设备空闲，给总控文件夹和PLC写入空闲状态", MsgType.Success, Color.Green);
                                        }
                                        else
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "未找到机台状态共享文件夹，检查网络", MsgType.Success, Color.Red);
                                            throw new StationErrorException("通讯报警");
                                        }
                                    }
                                }
                            }
                            this.RunStep = 0;
                            break;
                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.状态监控线程.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Pause);
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
                    StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Error);
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
                    StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        /// <summary>
        /// 空载具回收模式
        /// </summary>
        public override void StationEmptyRun()
        {
            MyVariable.num = 0;
            this.RunDone = false;
            StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0://启动
                            CheckCurrentRunStatus(0, 0, 0);
                            this.RunStep = 20;
                            break;
                        case 20://刷新载具进出料区有无载具存留
                            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头出料区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头出料区光电2])
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64603, 1);
                                }
                                else
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64603, 0);
                                }
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头进料区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头进料区光电2])
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64604, 1);
                                }
                                else
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64604, 0);
                                }
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.出料区光电2])
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64605, 1);
                                }
                                else
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64605, 0);
                                }
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.进料区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.进料区光电2])
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64606, 1);
                                }
                                else
                                {
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64606, 0);
                                }
                            }
                            this.RunStep = 30;
                            break;
                        case 30://判断空载具回收是否完成
                            if (MyVariable.EmptyRun_RunDone)
                            {
                                MyVariable.EmptyRun_RunDone = false;
                                throw new StationWorkDone("空载具回收完成！");
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
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.状态监控线程.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Pause);
                    break;
                }
                /***异常捕获***/
                catch (StationErrorException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.状态监控线程.ToString() + "异常捕获：" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.状态监控线程.ToString() + "报警捕获：" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunDone = true;
                    this.RunStep = 0;
                    StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
            }
        }

        public override void StationCalibRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Stop);
        }

        public override void StationCPKRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].ChangeStatus(_StationStatus.Stop);
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

        public void HaoCaiAutoThread()
        {
            threadMark = true;
            while (true)
            {
                Thread.Sleep(50);
                try
                {
                    MyVariable.area_QiangTou1.num_Y = ((MyVariable.area_QiangTou1.num_XMax * MyVariable.area_QiangTou1.num_YMax) - Convert.ToInt32(MyVariable.area_QiangTou1.num_Remain)) / 10;
                    MyVariable.area_QiangTou1.num_X = ((MyVariable.area_QiangTou1.num_XMax * MyVariable.area_QiangTou1.num_YMax) - Convert.ToInt32(MyVariable.area_QiangTou1.num_Remain)) % 10;
                    MyVariable.area_QiangTou2.num_Y = ((MyVariable.area_QiangTou2.num_XMax * MyVariable.area_QiangTou2.num_YMax) - Convert.ToInt32(MyVariable.area_QiangTou2.num_Remain)) / 10;
                    MyVariable.area_QiangTou2.num_X = ((MyVariable.area_QiangTou2.num_XMax * MyVariable.area_QiangTou2.num_YMax) - Convert.ToInt32(MyVariable.area_QiangTou2.num_Remain)) % 10;
                    MyVariable.area_QiangTou3.num_Y = ((MyVariable.area_QiangTou3.num_XMax * MyVariable.area_QiangTou3.num_YMax) - Convert.ToInt32(MyVariable.area_QiangTou3.num_Remain)) / 10;
                    MyVariable.area_QiangTou3.num_X = ((MyVariable.area_QiangTou3.num_XMax * MyVariable.area_QiangTou3.num_YMax) - Convert.ToInt32(MyVariable.area_QiangTou3.num_Remain)) % 10;
                    MyVariable.area_QiangTou4.num_Y = ((MyVariable.area_QiangTou4.num_XMax * MyVariable.area_QiangTou4.num_YMax) - Convert.ToInt32(MyVariable.area_QiangTou4.num_Remain)) / 10;
                    MyVariable.area_QiangTou4.num_X = ((MyVariable.area_QiangTou4.num_XMax * MyVariable.area_QiangTou4.num_YMax) - Convert.ToInt32(MyVariable.area_QiangTou4.num_Remain)) % 10;
                    MyVariable.area_LiXinGuan.num_Y = ((MyVariable.area_LiXinGuan.num_XMax * MyVariable.area_LiXinGuan.num_YMax) - Convert.ToInt32(MyVariable.area_LiXinGuan.num_Remain)) / 3;
                    MyVariable.area_LiXinGuan.num_X = ((MyVariable.area_LiXinGuan.num_XMax * MyVariable.area_LiXinGuan.num_YMax) - Convert.ToInt32(MyVariable.area_LiXinGuan.num_Remain)) % 3;
                }
                catch (Exception ex)
                {

                }
            }
        }

    }
}

