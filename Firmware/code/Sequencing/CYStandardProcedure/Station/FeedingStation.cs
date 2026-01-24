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
    public class FeedingStation : ObjectStation
    {
        private string mName;
        private _ActionResult resetRet;//单步复位结果
        private _ActionResult runRet;//单步运行结果

        private short[] read_PLC;
        private string str2;
        private string strDataPath = "";



        public FeedingStation(string name) :
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
            StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Initial);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.ResetStep)
                    {
                        case 0:
                            LogConfig.Instance.ShowMessageToList("Run", "供料线程开始复位", MsgType.Success, Color.Blue);
                            this.ResetStep = 20;
                            break;
                        case 20:
                            if (MyVariable.show_IsOpen)
                            {
                                this.ResetStep = 200;
                                break;
                            }
                            if (GeneralStart(false))//检查到有提前要料文件,需要人为判断是否删除
                            {
                                MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.YesNo, true);
                                mb.TopMost = true;
                                mb.MsgShowDialog("提示", "当前检测到提前要料文件,是否需要删除,删除后启动则不会提前要料");
                                string btn = mb.ret.SelectedBtn;
                                if (btn == "btn_A")
                                {
                                    GeneralStart(true);
                                }
                                this.ResetStep = 200;
                            }
                            else
                            {
                                this.ResetStep = 200;
                            }
                            break;
                        case 200:
                            throw new StationHomeOK("供料线程复位完成！");
                    }
                }
                /***子线程复位失败跳转到这里***/
                catch (StationHomeErrException ex)
                {
                    //LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Error, Color.Red);
                    this.ResetError = true;
                    StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
                /***子线程复位完成跳转到这里***/
                catch (StationHomeOK ex)
                {
                    this.ResetStep = 0;
                    this.ResetDone = true;
                    LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Success, Color.Green);
                    StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Stop);
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
            StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0://判断工位状态
                            switch (SerializeClass.mMemory.FeedingStation_state)
                            {
                                case MemoryClass.FeedingStation_State.空闲:
                                    this.RunStep = 10;
                                    break;
                                case MemoryClass.FeedingStation_State.缺料:
                                    this.RunStep = 450;
                                    break;
                                case MemoryClass.FeedingStation_State.换料:
                                    this.RunStep = 400;
                                    break;
                                case MemoryClass.FeedingStation_State.满料:
                                    this.RunStep = 600;
                                    break;
                            }
                            break;

                        #region 当前状态  空闲
                        case 10:
                            SerializeClass.mMemory.FeedingStation_state = MemoryClass.FeedingStation_State.空闲;
                            CheckCurrentRunStatus(0, 10, 10);
                            this.RunStep = 20;
                            break;
                        case 20:
                            if (
                                SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.空闲
                                && SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.空闲
                                //|| SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序中
                                //|| SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育中       //设备在长时间停机等待的情况下允许下一次样本进料，禁用
                                )
                            {
                                this.RunStep = 30;
                            }
                            else if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序完成
                                || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育完成)
                            {
                                this.RunStep = 600;      //状态置有料,继续实验
                            }
                            else
                            {
                                //   LogConfig.Instance.ShowMessageToList("Run", "测序仪工位状态异常", MsgType.Success, Color.Red);
                                //  throw new StationErrorException("测序仪工位状态异常");
                                this.RunStep = 10;
                            }
                            break;
                        case 30:
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                //手动调试单机模式,样本进料
                                if (MyVariable.sign_DNA)
                                {
                                    MyVariable.sign_DNA = false;
                                    this.RunStep = 100;
                                }
                                //总控允许实验
                                else if (MyVariable.sign_zongkong || MyVariable.experiment_Arrive)//是否允许实验
                                {
                                    this.RunStep = 50;
                                }
                                //无任务
                                else
                                {
                                    this.RunStep = 10;
                                }
                            }
                            else
                            {
                                if (GeneralStart(false))//总控告知提前进耗材
                                {
                                    this.RunStep = 50;
                                }
                                //参观模式,上料完成,再次点击启动按钮开启工作流程 
                                else if (MyVariable.show_IsOpen)
                                {
                                    if (MyVariable.consumables_Empty[0] == true || MyVariable.consumables_Empty[1] == true || MyVariable.consumables_Empty[2] == true || MyVariable.consumables_Empty[3] == true)
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "流程异常,检查程序逻辑", MsgType.Success, Color.Red);
                                        throw new StationErrorException("通讯报警");
                                    }
                                    if (MyVariable.show_Repeat)
                                    {
                                        GeneralShowStart(true);//已经开始参观模式,若与总控交互信息未删除,则删除
                                        this.RunStep = 600;  //不缺料
                                    }
                                    else if (IOConfig.Instance.GetBitInput(_InputCollect.启动按钮.ToString())|| GeneralShowStart(true))
                                    {
                                        this.RunStep = 600;  //不缺料
                                    }
                                }
                                else if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 1, out read_PLC))
                                {
                                    if (read_PLC[0] == 4)//样本请求进料
                                    {
                                        string readstr = "";
                                        LogConfig.Instance.ShowMessageToList("Run", "地轨到站，请求进样本", MsgType.Success, Color.Brown);
                                        if (Program.modbusTcp_PLC.ReadStringFromRegister(1, 64408, 20, "03", out readstr, (ModbusLib.DataType)Enum.Parse(typeof(ModbusLib.DataType), "BADC")))
                                        {
                                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.样本进料;
                                            MyVariable.SN_CarryStation = readstr.Trim().Replace("\u0000", "");
                                            LogConfig.Instance.ShowMessageToList("Run", "获取样本载具SN:" + MyVariable.SN_CarryStation, MsgType.Success, Color.Green);
                                        }
                                        else
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                            throw new StationErrorException("通讯报警");
                                        }
                                        this.RunStep = 100;
                                    }
                                    else if (MyVariable.experiment_Arrive)  //是否允许实验
                                    {
                                        this.RunStep = 50;
                                    }
                                    else
                                    {
                                        this.RunStep = 10;
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 50://判断是否已经给PLC发送要料信号
                            if (!MyVariable.need_Completed)
                            {
                                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                                {
                                    this.RunStep = 60;
                                    break;
                                }
                                if (MyVariable.ToGeneralStatus(2))
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "设备开始运行，给总控文件夹写入忙碌状态", MsgType.Success, Color.Green);
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "未找到机台状态共享文件夹，检查网络", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                                this.RunStep = 60;
                            }
                            else
                            {
                                this.RunStep = 150;
                            }
                            break;
                        case 60://判断实验耗材是否充足
                            if (MyVariable.area_QiangTou2.num_Remain < 15)
                            {
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                {
                                    //给PLC发送1000枪头缺料地址*2
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64616, 2);
                                }
                                MyVariable.Tip1000 = 1;
                                MyVariable.consumables_Empty[0] = true;
                                LogConfig.Instance.ShowMessageToList("Run", "1000枪头区耗材不足", MsgType.Success, Color.Brown);
                            }
                            if (MyVariable.area_QiangTou3.num_Remain < 1)
                            {
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                {
                                    //给PLC发送200枪头缺料地址
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64615, 1);
                                }
                                MyVariable.consumables_Empty[1] = true;
                                LogConfig.Instance.ShowMessageToList("Run", "200枪头区耗材不足", MsgType.Success, Color.Brown);
                            }
                            if (MyVariable.area_QiangTou4.num_Remain < 4)
                            {
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                {
                                    //给PLC发送50枪头缺料地址
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64614, 1);
                                }
                                MyVariable.consumables_Empty[2] = true;
                                LogConfig.Instance.ShowMessageToList("Run", "50枪头区耗材不足", MsgType.Success, Color.Brown);
                            }
                            if (MyVariable.area_DiWen_FCT.num_Remain < ((MyVariable.FCT_volume + 300) / 100) || MyVariable.area_DiWen_FCF.num_Remain < ((MyVariable.FCF_volume / 100) * 2)
                                || MyVariable.area_DiWen_SB.num_Remain < ((MyVariable.SB_volume + 250) / 100) || MyVariable.area_DiWen_LIB.num_Remain < ((MyVariable.LIB_volume + 250) / 100)
                                || MyVariable.area_DiWen_DIL.num_Remain < (MyVariable.DIL_volume / 100) || MyVariable.area_DiWen_WMX.num_Remain < ((MyVariable.WMX_volume) / 100)
                                || MyVariable.area_DiWen_S.num_Remain < (MyVariable.S_volume / 100))
                            {
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                {
                                    //给PLC发送低温试剂缺料地址
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64622, 1);
                                }
                                MyVariable.consumables_Empty[3] = true;
                                LogConfig.Instance.ShowMessageToList("Run", "低温区耗材不足", MsgType.Success, Color.Brown);
                            }
                            if (MyVariable.area_LiXinGuan.num_Remain < 3)
                            {
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                {
                                    //给PLC发送1.5试管缺料地址
                                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64634, 1);
                                }
                                MyVariable.consumables_Empty[4] = true;
                                LogConfig.Instance.ShowMessageToList("Run", "1.5试管区耗材不足", MsgType.Success, Color.Brown);
                            }
                            foreach (var item in MyVariable.consumables_Empty)
                            {
                                if (item)
                                {
                                    if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                                    {
                                        //给PLC发送要料指令
                                        Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 1);
                                        SerializeClass.animationParam.ground = (int)_groundEnum.耗材要料;
                                    }
                                    MyVariable.need_Completed = true;
                                    break;
                                }
                            }
                            if (MyVariable.need_Completed)
                            {
                                this.RunStep = 150;
                            }
                            else
                            {
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                                {
                                    if (!GeneralStart(true))
                                    {
                                        str2 = @"\\" + ParameConfig.Instance.SystemParameDic[_ParamName.GeneralShareIP.ToString()].CurrentValue + @"\Cexu\Start";
                                        if (Directory.Exists(str2))
                                        {
                                            string filePath = Path.Combine(str2, "start.txt");
                                            if (File.Exists(filePath))//文件存在，启动要料流程
                                            {
                                                LogConfig.Instance.ShowMessageToList("Run", "提前上料共享文件删除失败", MsgType.Success, Color.Red);
                                                throw new StationErrorException("实验流程报警");
                                            }
                                        }
                                    }
                                    if (MyVariable.experiment_Arrive)//总控允许实验，表示样本已经进料完成，此时新进料方式耗材也已进料，可以开始实验
                                    {
                                        MyVariable.experiment_Arrive = false;//机台不缺料后进实验流程,清掉标志
                                        this.RunStep = 600;  //不缺料
                                    }
                                    else
                                    {
                                        MyVariable.b_StatusToControl = false;
                                        this.RunStep = 10;  //总控没有允许实验，说明样本还未到达设备，此时虽然设备满料，但还是不启动
                                    }
                                }
                                else
                                {
                                    MyVariable.experiment_Arrive = false;//机台不缺料后进实验流程,清掉标志
                                    this.RunStep = 600;  //不缺料
                                }
                            }
                            break;

                        case 100://判断机台内是否有DNA样本载具
                            if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.光电8联排试管区1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.光电8联排试管区2])
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "机台已存在样本载具，流程异常", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                                //  SerializeClass.mMemory.area = MemoryClass.Area.八联排试管区;
                                //  this.RunStep = 400;
                            }
                            else
                            {
                                SerializeClass.mMemory.area = MemoryClass.Area.进料区;
                                SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.八联排试管区;
                                this.RunStep = 450;
                            }
                            break;

                        //与PLC交互,判断到料情况
                        case 150:
                            CheckCurrentRunStatus(0, 150, 150);
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                            {
                                if (MyVariable.sign_TIP4)
                                {
                                    this.RunStep = 210;
                                }
                                else if (MyVariable.sign_TIP3)
                                {
                                    this.RunStep = 190;
                                }
                                else if (MyVariable.sign_TIP1)
                                {
                                    this.RunStep = 160;
                                }
                                else if (MyVariable.sign_DiWen)
                                {
                                    this.RunStep = 230;
                                }
                                else if (MyVariable.sign_LiXinGuan)
                                {
                                    this.RunStep = 250;
                                }
                            }
                            else
                            {
                                if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64400, 2, out read_PLC))
                                {
                                    if (read_PLC[0] == 1 && read_PLC[1] == 1)
                                    {
                                        this.RunStep = 155;//地轨到位，请求进料
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 155:
                            CheckCurrentRunStatus(0, 155, 155);
                            if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, 64401, 2, out read_PLC))
                            {
                                if (read_PLC[1] == 1)
                                {
                                    this.RunStep = 210;
                                }
                                else if (read_PLC[1] == 2)
                                {
                                    this.RunStep = 190;
                                }
                                else if (read_PLC[1] == 3)
                                {
                                    this.RunStep = 160;
                                }
                                else if (read_PLC[1] == 9)
                                {
                                    this.RunStep = 230;
                                }
                                else if (read_PLC[1] == 21)
                                {
                                    this.RunStep = 250;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "读取PLC失败,检查连接", MsgType.Success, Color.Red);
                                throw new StationErrorException("通讯报警");
                            }
                            break;

                        /**********************判断机台是否已经有相关区域载具**************************/
                        case 160://判断枪头区1是否有载具
                            if (MyVariable.consumables_Empty[0] && MyVariable.Tip1000 == 1)
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电2])
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区1;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                                    this.RunStep = 400;
                                }
                                else
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.进料区;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.枪头区1;
                                    this.RunStep = 450;
                                }
                                MyVariable.Tip1000++;
                            }
                            else
                            {
                                this.RunStep = 170;
                            }
                            break;

                        case 170://判断枪头区2是否有载具
                            if (MyVariable.consumables_Empty[0] && MyVariable.Tip1000 == 2)
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电2])
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区2;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                                    this.RunStep = 400;
                                }
                                else
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.进料区;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.枪头区2;
                                    this.RunStep = 450;
                                }
                                MyVariable.Tip1000 = 0;
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "线程异常,检查标志位赋值是否正确", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;

                        case 190://判断枪头区3是否有载具
                            if (MyVariable.consumables_Empty[1])
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电2])
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区3;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                                    this.RunStep = 400;
                                }
                                else
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.进料区;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.枪头区3;
                                    this.RunStep = 450;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "线程异常,检查标志位赋值是否正确", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;

                        case 210://判断枪头区4是否有载具
                            if (MyVariable.consumables_Empty[2])
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电2])
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区4;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                                    this.RunStep = 400;
                                }
                                else
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.进料区;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.枪头区4;
                                    this.RunStep = 450;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "线程异常,检查标志位赋值是否正确", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;

                        case 230://判断低温区是否有载具
                            if (MyVariable.consumables_Empty[3])
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.低温区光电])
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.低温区;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                                    this.RunStep = 400;
                                }
                                else
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.进料区;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.低温区;
                                    this.RunStep = 450;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "线程异常,检查标志位赋值是否正确", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;

                        case 250://判断1.5试管区是否有载具
                            if (MyVariable.consumables_Empty[4])
                            {
                                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电2])
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.离心管试管区;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                                    this.RunStep = 400;
                                }
                                else
                                {
                                    SerializeClass.mMemory.area = MemoryClass.Area.进料区;
                                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.离心管试管区;
                                    this.RunStep = 450;
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "线程异常,检查标志位赋值是否正确", MsgType.Success, Color.Red);
                                throw new StationErrorException("实验流程报警");
                            }
                            break;
                        #endregion

                        #region 当前状态  换料/缺料
                        /****************************机台耗材不足,走补料流程********************************/
                        case 400:
                            SerializeClass.mMemory.FeedingStation_state = MemoryClass.FeedingStation_State.换料;
                            this.RunStep = 500;
                            break;
                        case 450:
                            SerializeClass.mMemory.FeedingStation_state = MemoryClass.FeedingStation_State.缺料;
                            this.RunStep = 500;
                            break;
                        case 500:
                            CheckCurrentRunStatus(0, 500, 500);
                            if (MyVariable.feed_Completed)
                            {
                                MyVariable.feed_Completed = false;
                                this.RunStep = 10;
                            }
                            break;
                        #endregion

                        #region 当前状态  有料
                        /****************************机台耗材充足,走实验流程********************************/
                        case 600:
                            SerializeClass.mMemory.FeedingStation_state = MemoryClass.FeedingStation_State.满料;
                            this.RunStep = 620;
                            break;
                        case 620:
                            if (SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.测序中
                                 || SerializeClass.mMemory.SequencingStation_state == MemoryClass.SequencingStation_State.孵育中)
                            {
                                this.RunStep = 10;
                            }
                            else
                            {
                                this.RunStep = 640;
                            }
                            break;
                        case 640://等待实验结束
                            CheckCurrentRunStatus(0, 640, 640);
                            if (SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.测序配置完成
                                || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.清洗步骤二完成
                                || SerializeClass.mMemory.CarryStation_state == MemoryClass.CarryStation_State.实验完成)
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
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.供料线程.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Pause);
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
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.供料线程.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Error);
                    break;
                }
                /***报警捕获***/
                catch (StationAlarmException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.供料线程.ToString() + "case:" + RunStep.ToString(), MsgType.Success, Color.Red);
                    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(_StationStatus.Alarm);
                    break;
                }
                /***正常结束流程捕获***/
                catch (StationWorkDone ex)
                {
                    this.RunStep = 0;
                    this.RunDone = true;
                    StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Stop);
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
            StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Stop);
        }

        public override void StationCalibRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Stop);
        }

        public override void StationCPKRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.供料线程.ToString()].ChangeStatus(_StationStatus.Stop);
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
        /// 总控告知设备提前要料
        /// </summary>
        /// <param name="isDelete">是否删除文件</param>
        /// <returns></returns>
        public bool GeneralStart(bool isDelete)
        {
            try
            {
                str2 = @"\\" + ParameConfig.Instance.SystemParameDic[_ParamName.GeneralShareIP.ToString()].CurrentValue + @"\Cexu\Start";
                if (Directory.Exists(str2))
                {
                    string filePath = Path.Combine(str2, "start.txt");
                    if (File.Exists(filePath))//文件存在，启动要料流程
                    {
                        if (isDelete)//是否删除文件
                        {
                            File.Delete(filePath);
                            LogConfig.Instance.ShowMessageToList("Run", "提前上料结束，删除共享文件", MsgType.Success, Color.Green);
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
        /// 参观模式总控告知设备开始运行
        /// </summary>
        /// <param name="isDelete">是否删除文件</param>
        /// <returns></returns>
        public bool GeneralShowStart(bool isDelete)
        {
            try
            {
                str2 = @"\\" + ParameConfig.Instance.SystemParameDic[_ParamName.GeneralShareIP.ToString()].CurrentValue + @"\Cexu\Visit";
                if (Directory.Exists(str2))
                {
                    string filePath = Path.Combine(str2, "start.txt");
                    if (File.Exists(filePath))//文件存在，启动要料流程
                    {
                        if (isDelete)//是否删除文件
                        {
                            File.Delete(filePath);
                            LogConfig.Instance.ShowMessageToList("Run", "接收到参观启动信号,参观流程开始运行", MsgType.Success, Color.Green);
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

