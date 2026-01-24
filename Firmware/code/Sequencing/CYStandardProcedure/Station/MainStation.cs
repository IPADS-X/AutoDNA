using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using CYAutoFramework;
using System.Diagnostics;

namespace CYStandardProcedure
{
    public class MainStation : ObjectStation
    {
        private string mName;
        private bool bool_errors;
        Stopwatch resetSign = new Stopwatch();//复位延时
        public MainStation(string name) :
            base(name)
        {
            this.mName = name;
        }

        /// <summary>
        /// 基类中虚函数重写，主线程的复位流程
        /// </summary>
        public override void StationReset()
        {
            LogConfig.Instance.ShowMessageToList("Run", "长按3秒复位,短按清除异常", MsgType.Success, Color.Blue);
            StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Stop);
            resetSign.Restart();
            while (true)
            {
                Thread.Sleep(5);
                if (resetSign.ElapsedMilliseconds / 1000 >= 3)
                {
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Initial);
                    IOConfig.Instance.SetSingleOut(_OutputCollect.复位按钮灯.ToString(), 1);
                    IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 1);
                    Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 0);
                        Thread.Sleep(2000);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序暂停.ToString(), 0);
                    });
                    break;
                }
                else if (!IOConfig.Instance.InputsStatus[(int)_InputCollect.复位按钮])
                {
                    /***针对M60卡，清除急停触发状态（手动）***/
                    MotionConfig.Instance.ClearEmgStatus();

                    /***人为异常清除后，单次宕机结束***/
                    DownTime.Instance.EndDownTime();
                    /***清除电动夹爪报警***/
                    for (byte i = 1; i < 3; i++)
                    {
                        SerializeClass.m_ModbusRtuRob.WriteSingleCoil(i, 0, false);
                        SerializeClass.m_ModbusRtuRob.WriteSingleCoil(i, 0, true);
                    }
                    bool_errors = SoftWareForm.carryclaw_initialize.Rtu_carryClaw.ForceCoil(Program.carryClawConfig.DevAdd, 1402, false);
                    if (bool_errors)
                    {
                        bool_errors = false;
                        bool_errors = SoftWareForm.carryclaw_initialize.Rtu_carryClaw.ForceCoil(Program.carryClawConfig.DevAdd, 1402, true);
                    }
                    /***清除机器人报警***/
                    IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人清除报警.ToString(), 1);
                    Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人清除报警.ToString(), 0);
                    });
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(ObjectStation._StationStatus.Pause);
                    for (int i = 1; i < Enum.GetNames(typeof(_ThreadModule)).Length; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == ObjectStation._StationStatus.Error)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Pause);
                        }
                    }
                    /***给PLC报警清除信号***/
                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                    {
                        if (!Program.modbusTcp_PLC.Connect())
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "PLC连接失败", MsgType.Success, Color.Red);
                        }
                        if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64611, 1))
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "64611地址写: 1", MsgType.Success, Color.Green);
                        }
                        else
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "64611地址写: 1 失败", MsgType.Success, Color.Red);
                        }
                        Task.Run(() =>
                        {
                            Thread.Sleep(500);
                            if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64611, 0))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64611地址写: 0", MsgType.Success, Color.Green);
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "64611地址写: 0 失败", MsgType.Success, Color.Red);
                            }
                        });
                    }
                    //if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledAnimation.ToString()].CurrentValue)))
                    //{
                    //    if (!TCPClientConfig.Instance.ReConnectClient(_TcpClientModule.Animation.ToString()))
                    //    {
                    //        LogConfig.Instance.ShowMessageToList("Run", "数字孪生连接失败", MsgType.Success, Color.Red);
                    //    }
                    //}
                    LogConfig.Instance.ShowMessageToList("Run", "清除异常", MsgType.Success, Color.Green);
                    return;
                }
            }
            this.ResetStep = 0;
            /***复位结果数组***/
            bool[] resetRet = new bool[StationConfig.Instance.StationDic.Count - 1];
            /***子工位复位状态置位***/
            for (int i = 0; i < Enum.GetNames(typeof(_ThreadModule)).Length; i++)
            {
                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ResetStep = 0;
                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].RunStep = 0;
                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ResetDone = false;
                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ResetError = false;
            }
            /***清除运行,NG,报警信息***/
            LogConfig.Instance.ClearListMessage("Run");
            LogConfig.Instance.ClearListMessage("NG");
            LogConfig.Instance.ClearListMessage("Alarm");
            /***设备切换为复位状态***/
            StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Initial);
            LogConfig.Instance.ShowMessageToList("Run", "设备开始复位！", MsgType.Success, Color.Green);
            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo防护停止启动.ToString(), 1);
            Task.Run(() =>
            {
                Thread.Sleep(500);
                IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo防护停止启动.ToString(), 0);
            });
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    /***设备报警，复位退出***/
                    if (this.mCurStatus == _StationStatus.Alarm)
                    {
                        /***子线程切换为报警状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Alarm);
                        }
                        throw new StationHomeErrException("设备报警，复位失败！");
                    }
                    /***子线程有任何一个复位失败，复位失败***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        /***一个子线程复位失败***/
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ResetError)
                        {
                            StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Stop);

                            for (int j = 1; j < StationConfig.Instance.StationDic.Count; j++)
                            {
                                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[j]].ChangeStatus(ObjectStation._StationStatus.Pause);
                            }

                            /***主线程复位失败***/
                            this.ResetError = true;
                            throw new StationHomeErrException("设备复位失败！");
                        }
                    }
                    /***主线程复位完成，设备复位完成***/
                    if (this.ResetDone)
                    {
                        LogConfig.Instance.ShowMessageToList("Run", "设备复位完成！", MsgType.Success, Color.Green);
                        StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Stop);
                        break;
                    }
                    switch (this.ResetStep)
                    {
                        case 0:
                            /***子工位开始复位***/
                            for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                            {
                                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ResetEvent.Set();
                            }
                            this.ResetStep = 10;
                            break;
                        case 10:
                            /***等待子工位复位完成***/
                            for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                            {
                                if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ResetDone)
                                {
                                    resetRet[i - 1] = true;
                                }
                                else
                                {
                                    resetRet[i - 1] = false;
                                }
                            }
                            if (Array.IndexOf(resetRet, false) == -1)
                            {
                                this.ResetStep = 20;
                            }
                            break;
                        case 20:
                            /***主线程复位完成***/
                            this.ResetStep = 0;
                            this.ResetDone = true;
                            break;
                    }
                }
                catch (StationHomeErrException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Error, Color.Red);
                    /***所有轴停止***/
                    for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                    {
                        MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 基类中虚函数重写，主线程运行流程
        /// </summary>
        public override void StationNormalRun()
        {
            this.RunDone = false;
            this.RunStep = 0;
            /***给PLC开始运行信号***/
            if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
            {
                if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64611, 2))
                {
                    LogConfig.Instance.ShowMessageToList("Run", "64611地址写: 2", MsgType.Success, Color.Green);
                }
                else
                {
                    LogConfig.Instance.ShowMessageToList("Run", "64611地址写: 2 失败", MsgType.Success, Color.Red);
                }
                Task.Run(() =>
                {
                    Thread.Sleep(500);
                    if (Program.modbusTcp_PLC.WriteSingleRegister(1, 64611, 0))
                    {
                        LogConfig.Instance.ShowMessageToList("Run", "64611地址写: 0", MsgType.Success, Color.Green);
                    }
                    else
                    {
                        LogConfig.Instance.ShowMessageToList("Run", "64611地址写: 0 失败", MsgType.Success, Color.Red);
                    }
                });
            }
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    /***主线程报警***/
                    if (this.mCurStatus == _StationStatus.Alarm)
                    {
                        this.ResetDone = false;
                        /***子线程切换为报警状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Alarm);
                        }
                        /***所有轴停止***/
                        for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                        {
                            MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                            MotionConfig.Instance.HomeCancel(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 1);
                        Task.Run(() =>
                        {
                            Thread.Sleep(500);
                            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 0);
                        });
                        break;
                    }
                    /***主线程暂停***/
                    if (this.mCurStatus == _StationStatus.Pause)
                    {
                        //TsRemoteRobotConfig.Instance.TsRemoteRobotDic[RobotName.搬运机器人.ToString ()].BreakMove();
                        /***子工位切换为暂停状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Pause);
                        }
                        /***所有轴停止***/
                        for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                        {
                            MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                        }
                        break;
                    }
                    /***子线程有任何一个运行异常设备报异常***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == _StationStatus.Error)
                        {
                            throw new StationErrorException("");
                        }
                    }
                    /***主线程启动完成子线程循环监控***/
                    if (this.RunDone)
                    {
                        StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Stop);
                        break;
                    }
                    switch (this.RunStep)
                    {
                        case 0:
                            /***子线程启动***/
                            for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                            {
                                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].NormalRunEvent.Set();
                            }
                            this.RunStep = 20;
                            break;
                        case 20:
                            if (StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].RunDone)
                            {
                                /***运行一次完成，OK产能+1***/
                                Yield.Instance.UpdateYield(true);
                                this.RunStep = 0;
                                /***程序循环标志***/
                                this.RunDone = true;
                            }
                            break;
                    }
                }
                catch (StationErrorException ex)
                {
                    /***主线程切换为异常状态***/
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Error);
                    /***如果子线程还在运行状态，切换为暂停状态***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == _StationStatus.Run)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(_StationStatus.Pause);
                        }
                    }
                    /***所有轴停止***/
                    for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                    {
                        MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                    }
                    /***运行异常，NG产能+1***/
                    Yield.Instance.UpdateYield(false);
                    break;
                }
            }
        }

        /// <summary>
        /// 基类中虚函数重写，主线程空跑流程
        /// </summary>
        public override void StationEmptyRun()
        {
            this.RunDone = false;
            this.RunStep = 0;
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    /***主线程报警***/
                    if (this.mCurStatus == _StationStatus.Alarm)
                    {
                        this.ResetDone = false;
                        /***子线程切换为报警状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Alarm);
                        }
                        /***所有轴停止***/
                        for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                        {
                            MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                            MotionConfig.Instance.HomeCancel(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 1);
                        Task.Run(() =>
                        {
                            Thread.Sleep(500);
                            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 0);
                        });
                        break;
                    }
                    /***主线程暂停***/
                    if (this.mCurStatus == _StationStatus.Pause)
                    {
                        //TsRemoteRobotConfig.Instance.TsRemoteRobotDic[RobotName.搬运机器人.ToString ()].BreakMove();
                        /***子工位切换为暂停状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Pause);
                        }
                        /***所有轴停止***/
                        for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                        {
                            MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                        }
                        break;
                    }
                    /***子线程有任何一个运行异常设备报异常***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == _StationStatus.Error)
                        {
                            throw new StationErrorException("");
                        }
                    }
                    /***主线程启动完成子线程循环监控***/
                    if (this.RunDone)
                    {
                        StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Stop);
                        break;
                    }
                    switch (this.RunStep)
                    {
                        case 0:
                            /***子线程启动***/
                            for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                            {
                                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].EmptyRunEvent.Set();
                            }
                            this.RunStep = 20;
                            break;
                        case 20:
                            if (StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].RunDone)
                            {
                                /***运行一次完成，OK产能+1***/
                                Yield.Instance.UpdateYield(true);
                                this.RunStep = 0;
                                /***程序循环标志***/
                                this.RunDone = true;
                            }
                            break;
                    }
                }
                catch (StationErrorException ex)
                {
                    /***主线程切换为异常状态***/
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Error);
                    /***如果子线程还在运行状态，切换为暂停状态***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == _StationStatus.Run)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(_StationStatus.Pause);
                        }
                    }
                    /***所有轴停止***/
                    for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                    {
                        MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                    }
                    /***运行异常，NG产能+1***/
                    Yield.Instance.UpdateYield(false);
                    break;
                }
            }
        }

        /// <summary>
        /// 基类中虚函数重写,主线程标定流程
        /// </summary>
        public override void StationCalibRun()
        {
            this.RunDone = false;
            this.RunStep = 0;
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    /***主线程报警***/
                    if (this.mCurStatus == _StationStatus.Alarm)
                    {
                        this.ResetDone = false;
                        /***子线程切换为报警状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Alarm);
                        }
                        /***所有轴停止***/
                        for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                        {
                            MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                            MotionConfig.Instance.HomeCancel(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 1);
                        Task.Run(() =>
                        {
                            Thread.Sleep(500);
                            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 0);
                        });
                        break;
                    }
                    /***主线程暂停***/
                    if (this.mCurStatus == _StationStatus.Pause)
                    {
                        /***子工位切换为暂停状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Pause);
                        }
                        /***所有轴停止***/
                        for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                        {
                            MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                        }
                        break;
                    }
                    /***子线程有任何一个运行异常设备报异常***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == _StationStatus.Error)
                        {
                            throw new StationErrorException("");
                        }
                    }
                    /***主线程启动完成子线程循环监控***/
                    if (this.RunDone)
                    {
                        StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Stop);
                        break;
                    }
                    switch (this.RunStep)
                    {
                        case 0:
                            /***子线程启动***/
                            for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                            {
                                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].CalibEvent.Set();
                            }
                            this.RunStep = 20;
                            break;
                        case 20:
                            if (StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].RunDone)
                            {
                                /***运行一次完成，OK产能+1***/
                                Yield.Instance.UpdateYield(true);
                                this.RunStep = 0;
                                /***程序循环标志***/
                                this.RunDone = true;
                            }
                            break;
                    }
                }
                catch (StationErrorException ex)
                {
                    /***主线程切换为异常状态***/
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Error);
                    /***如果子线程还在运行状态，切换为暂停状态***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == _StationStatus.Run)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(_StationStatus.Pause);
                        }
                    }
                    /***所有轴停止***/
                    for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                    {
                        MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                    }
                    /***运行异常，NG产能+1***/
                    Yield.Instance.UpdateYield(false);
                    break;
                }
            }
        }


        /// <summary>
        /// 基类中虚函数重写，主线程CPK流程
        /// </summary>
        public override void StationCPKRun()
        {
            this.RunDone = false;
            this.RunStep = 0;
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    /***主线程报警***/
                    if (this.mCurStatus == _StationStatus.Alarm)
                    {
                        this.ResetDone = false;
                        /***子线程切换为报警状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Alarm);
                        }
                        /***所有轴停止***/
                        for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                        {
                            MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                            MotionConfig.Instance.HomeCancel(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                        }
                        IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 1);
                        Task.Run(() =>
                        {
                            Thread.Sleep(500);
                            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人程序停止.ToString(), 0);
                        });
                        break;
                    }
                    /***主线程暂停***/
                    if (this.mCurStatus == _StationStatus.Pause)
                    {
                        /***子工位切换为暂停状态(0,表示主线程,1往后表示子线程)***/
                        for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Pause);
                        }
                        /***所有轴停止***/
                        for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                        {
                            MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                        }
                        break;
                    }
                    /***子线程有任何一个运行异常设备报异常***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == _StationStatus.Error)
                        {
                            throw new StationErrorException("");
                        }
                    }
                    /***主线程启动完成子线程循环监控***/
                    if (this.RunDone)
                    {
                        StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Stop);
                        break;
                    }
                    switch (this.RunStep)
                    {
                        case 0:
                            /***子线程启动***/
                            for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                            {
                                StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].CPKEvent.Set();
                            }
                            this.RunStep = 20;
                            break;
                        case 20:
                            if (StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.搬运工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.机器人工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.测序仪工位.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.状态监控线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.数字孪生线程.ToString()].RunDone
                                && StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].RunDone)
                            {
                                /***运行一次完成，OK产能+1***/
                                Yield.Instance.UpdateYield(true);
                                this.RunStep = 0;
                                /***程序循环标志***/
                                this.RunDone = true;
                            }
                            break;
                    }
                }
                catch (StationErrorException ex)
                {
                    /***主线程切换为异常状态***/
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Error);
                    /***如果子线程还在运行状态，切换为暂停状态***/
                    for (int i = 1; i < StationConfig.Instance.StationDic.Count; i++)
                    {
                        if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == _StationStatus.Run)
                        {
                            StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(_StationStatus.Pause);
                        }
                    }
                    /***所有轴停止***/
                    for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
                    {
                        MotionConfig.Instance.EmgAxisMove(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
                    }
                    /***运行异常，NG产能+1***/
                    Yield.Instance.UpdateYield(false);
                    break;
                }
            }
        }


        /// <summary>
        /// 基类中虚函数重写,主线程GRR流程
        /// </summary>
        public override void StationGRRRun()
        {
            base.StationGRRRun();
        }

        /// <summary>
        /// 基类中虚函数重写，主线程相机静态Grr流程
        /// </summary>
        public override void StationCamStaticRun()
        {
            base.StationCamStaticRun();
        }

        /// <summary>
        /// 基类中虚函数重写，主线程相机动态Grr流程
        /// </summary>
        public override void StationCamDynamicRun()
        {
            base.StationCamDynamicRun();
        }
    }
}
