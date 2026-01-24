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
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CYStandardProcedure
{
    public class DataProcessingStation : ObjectStation
    {
        INIFile ini = new INIFile(Application.StartupPath + "\\FileINI\\SequenceForm.ini");
        private string mName;
        private _ActionResult resetRet;//单步复位结果
        private _ActionResult runRet;//单步运行结果
        Stopwatch DataCheckTime = new Stopwatch();//查询完成周期
        private long time;//延时时间记录
        private bool b_DataProcessingStation;//数据处理线程通用标志位
        private int code_general;//总控反馈响应码
        private DateTime dtEndTime = new DateTime();
        private double copyMin;//拷贝时长
        private string cexuFilePath = @"E:\test\";
        private string IDNA_string;
        Stopwatch JianJiShiBieTime = new Stopwatch();
        private int JianJiShiBieMin = 0;
        readonly string SeqkitExePath = "C:/Windows/System32/seqkit.exe";
        ResultFolderVM resultFolderModel = null;
        /// <summary>
        /// 文件夹层级
        /// </summary>
        int MaxFolderLevel = 0;
        /// <summary>
        /// 总文件计数
        /// </summary>
        int TotalFileCount = 0;
        /// <summary>
        /// 已匹配文件计数
        /// </summary>
        int MatchedFileCount = 0;
        /// <summary>
        /// 链条数
        /// </summary>
        int TotalDNACounts = 0;
        /// <summary>
        /// 正确配对数
        /// </summary>
        int MatchDNACounts = 0;
        int MatchJianJiCounts = 0;
        bool IsProgress = false;









        public DataProcessingStation(string name) :
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
            StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Initial);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.ResetStep)
                    {
                        case 0:
                            LogConfig.Instance.ShowMessageToList("Run", "数据处理线程开始复位", MsgType.Success, Color.Blue);
                            this.ResetStep = 200;
                            break;
                        case 200:
                            throw new StationHomeOK("数据处理线程复位完成！");
                    }
                }
                /***子线程复位失败跳转到这里***/
                catch (StationHomeErrException ex)
                {
                    this.ResetError = true;
                    StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Stop);
                    break;
                }
                /***子线程复位完成跳转到这里***/
                catch (StationHomeOK ex)
                {
                    this.ResetStep = 0;
                    this.ResetDone = true;
                    LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Success, Color.Green);
                    StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Stop);
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
            StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Run);
            while (true)
            {
                try
                {
                    Thread.Sleep(5);
                    switch (this.RunStep)
                    {
                        case 0://启动
                            switch (SerializeClass.mMemory.DataProcessingStation_state)
                            {
                                case MemoryClass.DataProcessingStation_State.空闲:
                                    if (MyVariable.JianJiShiBie_Start)
                                    {
                                        JianJiShiBieTime.Restart();
                                        this.RunStep = 30;
                                    }
                                    else
                                    {
                                        this.RunStep = 10;
                                    }
                                    break;
                                case MemoryClass.DataProcessingStation_State.文件拷贝中:
                                    this.RunStep = 60;
                                    break;
                                case MemoryClass.DataProcessingStation_State.文件解析中:
                                    this.RunStep = 160;
                                    break;
                            }
                            this.RunStep = 10;
                            break;

                        #region 当前状态  空闲
                        case 10://状态置空闲
                            CheckCurrentRunStatus(0, 10, 10);
                            SerializeClass.mMemory.DataProcessingStation_state = MemoryClass.DataProcessingStation_State.空闲;
                            this.RunStep = 20;
                            break;
                        case 20://判断测序是否结束
                            CheckCurrentRunStatus(0, 20, 20);
                            if (MyVariable.CeXu_Completed)
                            {
                                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)) || MyVariable.show_IsOpen
                                    || Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceHandle.ToString()].CurrentValue)))
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "实验进程:屏蔽测序流程,不绑定样本SN和测序ID", MsgType.Success, Color.Brown);
                                    MyVariable.SN_DataProcessingStation = MyVariable.SN_SequencingStation;
                                    MyVariable.CeXu_Completed = false;
                                    this.RunStep = 100;
                                }
                                else
                                {
                                    MyVariable.File_Copy.Add(MyVariable.SN_SequencingStation, SerializeClass.id_sequencingStation.run_id);//将样本SN和测序ID号绑定
                                    MyVariable.SN_DataProcessingStation = MyVariable.SN_SequencingStation;
                                    MyVariable.CeXu_Completed = false;
                                    JianJiShiBieTime.Restart();
                                    MyVariable.JianJiShiBie_Start = true;
                                    this.RunStep = 30;//接收到测序仪线程测序结束标志,线程启动,开始等待碱基识别完成并拷贝解析文件
                                }
                            }
                            else if (MyVariable.File_Copy.Count != 0)//当前有未拷贝完成的实验，继续查询拷贝状态
                            {
                                this.RunStep = 60;
                            }
                            break;
                        case 30://等待碱基识别结束
                            CheckCurrentRunStatus(0, 30, 30);
                            if (JianJiShiBieTime.ElapsedMilliseconds / 1000 > 60)
                            {
                                b_DataProcessingStation = SequencingInterface.SequencingState(SequencingInterface.sequencing_State, 1, out MyVariable.sequencing_code,
                                                           out MyVariable.sequencing_data, out MyVariable.sequencing_msg, out MyVariable.sequencing_total_pore_count);
                                if (b_DataProcessingStation)
                                {
                                    if (MyVariable.sequencing_code == "0" && (MyVariable.sequencing_data == "1" || MyVariable.sequencing_data == "2"))
                                    {
                                        JianJiShiBieMin = 0;
                                        LogConfig.Instance.ShowMessageToList("Run", "实验进程:碱基识别完成", MsgType.Success, Color.Green);
                                        WaitDelayTime(20);
                                        JianJiShiBieTime.Stop();
                                        this.RunStep = 40;
                                    }
                                    else if (MyVariable.sequencing_code == "0" && MyVariable.sequencing_data == "5")
                                    {
                                        JianJiShiBieMin++;
                                        LogConfig.Instance.ShowMessageToList("Run", "实验进程:碱基识别中,已识别" + JianJiShiBieMin + "分钟", MsgType.Success, Color.Brown);
                                        JianJiShiBieTime.Restart();
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "测序仪状态反馈异常", MsgType.Success, Color.Red);
                                        throw new StationErrorException("测序仪报警");
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "测序仪连接异常,检查测序接口是否打开", MsgType.Success, Color.Red);
                                    throw new StationErrorException("测序仪报警");
                                }

                            }
                            break;
                        case 40://和测序仪通讯，开始拷贝文件
                            SerializeClass.id_dataProcessingStation.run_id = MyVariable.File_Copy[MyVariable.SN_DataProcessingStation];
                            b_DataProcessingStation = SequencingInterface.SequencingState(SequencingInterface.sequencing_FileCopy, 2, out MyVariable.sequencing_code, out MyVariable.sequencing_data, out MyVariable.sequencing_msg, out MyVariable.sequencing_total_pore_count);
                            if (b_DataProcessingStation)
                            {
                                if ((MyVariable.sequencing_code == "0" || MyVariable.sequencing_code == "9007") && MyVariable.sequencing_msg == "ok")
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "实验进程:开始拷贝测序文件", MsgType.Success, Color.Brown);
                                    SerializeClass.animationParam.taskStep = (int)_taskStepEnum.开始拷贝测序文件;
                                    this.RunStep = 60;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "拷贝文件失败,检查网络", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            else
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "测序仪连接异常,检查测序接口是否打开", MsgType.Success, Color.Red);
                                throw new StationErrorException("通讯报警");
                            }
                            break;
                        #endregion

                        #region 当前状态  文件拷贝中
                        case 60://状态置文件拷贝中
                            MyVariable.JianJiShiBie_Start = false;
                            SerializeClass.mMemory.DataProcessingStation_state = MemoryClass.DataProcessingStation_State.文件拷贝中;
                            DataCheckTime.Restart();
                            this.RunStep = 90;
                            break;
                        case 80://每个周期查询一次是否拷贝完成
                            DataCheckTime.Restart();
                            if (MyVariable.File_Copy.Count == 0)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "文件拷贝任务丢失，检查程序", MsgType.Success, Color.Red);
                                throw new StationErrorException("通讯报警");
                            }
                            foreach (var item in MyVariable.File_Copy)
                            {
                                SerializeClass.id_dataProcessingStation.run_id = item.Value;
                                b_DataProcessingStation = SequencingInterface.SequencingState(SequencingInterface.sequencing_CopyState, 3, out MyVariable.sequencing_code, out MyVariable.sequencing_data, out MyVariable.sequencing_msg, out MyVariable.sequencing_total_pore_count);
                                if (b_DataProcessingStation)
                                {
                                    if (MyVariable.sequencing_code == "0" && MyVariable.sequencing_data == "1")//拷贝完成
                                    {
                                        DataCheckTime.Stop();
                                        MyVariable.SN_DataProcessingStation = item.Key;
                                        MyVariable.File_Copy.Remove(item.Key);
                                        copyMin = 0;
                                        LogConfig.Instance.ShowMessageToList("Run", "文件拷贝完成，删除任务", MsgType.Success, Color.Green);
                                        SerializeClass.animationParam.taskStep = (int)_taskStepEnum.文件拷贝完成;
                                        this.RunStep = 100;
                                        break;
                                    }
                                    else if (MyVariable.sequencing_code == "0" && MyVariable.sequencing_data == "2")
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "文件拷贝失败", MsgType.Success, Color.Red);
                                        this.RunStep = 40;
                                        throw new StationErrorException("通讯报警");
                                    }
                                    else
                                    {
                                        this.RunStep = 90;
                                    }
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "测序仪连接异常,检查测序接口是否打开", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 90:
                            CheckCurrentRunStatus(0, 90, 90);
                            if (DataCheckTime.ElapsedMilliseconds / 1000 > 30)
                            {
                                copyMin = copyMin + 0.5;
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:测序文件拷贝中,已进行" + copyMin + "分钟", MsgType.Success, Color.Brown);
                                this.RunStep = 80;
                            }

                            /*****************************多个实验任务同时拷贝文件，当前接口不支持多个文件拷贝，注释以下代码*******************************/
                            //if (MyVariable.CeXu_Completed)
                            //{
                            //    this.RunStep = 10;//上一个测序文件拷贝未完成，此时当前实验完成，需要拷贝
                            //}

                            break;
                        case 100://拷贝完成，给总控发送测序结束指令
                            if (MyVariable.newshow_IsOpenOver && !Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)))
                            {
                                TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).ClearNetData();
                                SerializeClass.mCompleteReportingToControl.sn = MyVariable.SN_DataProcessingStation;
                                SerializeClass.mCompleteReportingToControl.experimentResult = "OK";
                                string jsonStr2 = JsonConvert.SerializeObject(SerializeClass.mCompleteReportingToControl);
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).WriteDataStr(jsonStr2))
                                {
                                    LogToGeneral(jsonStr2);
                                    this.time = this.GetCurveTime();
                                    WaitDelayTime(0.3);
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送测序结束指令", MsgType.Success, Color.Brown);
                                    SerializeClass.animationParam.general = (int)_generalEnum.测序结束上报;
                                    this.RunStep = 120;
                                    break;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送数据失败", MsgType.Error, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽总控...", MsgType.Success, Color.Brown);
                                if (!Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue))
                                    && !Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceHandle.ToString()].CurrentValue)))
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
                                }
                                this.RunStep = 160;
                            }
                            else
                            {
                                TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).ClearNetData();
                                SerializeClass.mCompleteReportingToControl.sn = MyVariable.SN_DataProcessingStation;
                                SerializeClass.mCompleteReportingToControl.experimentResult = "OK";
                                string jsonStr2 = JsonConvert.SerializeObject(SerializeClass.mCompleteReportingToControl);
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).WriteDataStr(jsonStr2))
                                {
                                    LogToGeneral(jsonStr2);
                                    this.time = this.GetCurveTime();
                                    WaitDelayTime(0.3);
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送测序结束指令", MsgType.Success, Color.Brown);
                                    SerializeClass.animationParam.general = (int)_generalEnum.测序结束上报;
                                    this.RunStep = 120;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送数据失败", MsgType.Error, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 120://接收总控信息
                            CheckCurrentRunStatus(0, 100, 100);
                            if (OverTimeS(time, Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据超时", MsgType.Error, Color.Red);
                                this.RunStep = 100;
                                throw new StationErrorException("通讯报警");
                            }
                            else
                            {
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).NetCanRead())
                                {
                                    TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).LoopReadData(1, out Program.ControlReceived, Encoding.UTF8);
                                    LogFromGeneral(Program.ControlReceived);
                                    b_DataProcessingStation = MyVariable.GeneralCompleteReceive(Program.ControlReceived, out code_general, out SerializeClass.completeParam.data_idna,
                                        out SerializeClass.completeParam.data_taskId, out SerializeClass.completeParam.protocol_group_ids, out SerializeClass.completeParam.protocol_group_JianJi);
                                    if (b_DataProcessingStation)
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
                                            //MyVariable.Data_Process[SerializeClass.completeParam.data_taskId] = SerializeClass.completeParam.data_idna;
                                            LogConfig.Instance.ShowMessageToList("Run", "获取iDNA引物,任务ID以及实验名称", MsgType.Success, Color.Brown);
                                            this.RunStep = 160;
                                        }
                                        else
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "总控反馈异常", MsgType.Success, Color.Red);
                                            this.RunStep = 100;
                                            throw new StationErrorException("通讯报警");
                                        }
                                        SerializeClass.animationParam.general = (int)_generalEnum.无交互任务;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据异常", MsgType.Success, Color.Red);
                                        this.RunStep = 100;
                                        throw new StationErrorException("通讯报警");
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region 当前状态  文件解析中
                        case 160:
                            SerializeClass.mMemory.DataProcessingStation_state = MemoryClass.DataProcessingStation_State.文件解析中;
                            SerializeClass.animationParam.taskStep = (int)_taskStepEnum.文件解析中;
                            this.RunStep = 180;
                            break;
                        case 180://解析文件
                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)) || MyVariable.show_IsOpen
                                || Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.SequenceHandle.ToString()].CurrentValue)))
                            {
                                WaitDelayTime(3);
                                LogConfig.Instance.ShowMessageToList("Run", "实验进程:屏蔽测序流程,解析默认赋值9999", MsgType.Success, Color.Brown);
                                SerializeClass.mChipDataReportingToControl.taskId = SerializeClass.completeParam.data_taskId;
                                SerializeClass.mChipDataReportingToControl.chipTotalCount = 9999;
                                SerializeClass.mChipDataReportingToControl.chipMatchCount = 9999;
                                this.RunStep = 200;
                            }
                            else
                            {
                                SerializeClass.mChipDataReportingToControl.taskId = SerializeClass.completeParam.data_taskId;
                                SerializeClass.mChipDataReportingToControl.chipTotalCount = 0;
                                SerializeClass.mChipDataReportingToControl.chipMatchCount = 0;
                                IDNA_string = SerializeClass.completeParam.data_idna;             //"ATCAGTACGGTGCACCACCATGAA";
                                cexuFilePath = @"E:\test\" + SerializeClass.completeParam.protocol_group_ids;                                      //@"E:\test\20240112";
                                MyVariable.inferJianJiDic.Clear();
                                if (DataAnalysisMethod(cexuFilePath))
                                {
                                    MyVariable.differenceJianJiDic.Clear();

                                    /************************总控传来的碱基集合和测序推测的碱基集合作比对(只比对总控传来的标签号)**************************/
                                    //96孔中总控碱基有的标签测序碱基没有的
                                    var missingInDict2 = MyVariable.JianJiDic.Where(pair => !MyVariable.inferJianJiDic.ContainsKey(pair.Key)).ToList();
                                    if (missingInDict2.Any())
                                    {
                                        foreach (var item in missingInDict2)
                                        {
                                            MyVariable.differenceJianJiDic.Add(item.Key, "NG");
                                        }
                                    }
                                    #region 96孔中测序碱基有的标签总控碱基没有的(不作为参考依据)
                                    //96孔中测序碱基有的标签总控碱基没有的
                                    //var missingInDict1 = MyVariable.inferJianJiDic.Where(pair => !MyVariable.JianJiDic.ContainsKey(pair.Key)).ToList();
                                    //if (missingInDict1.Any())
                                    //{
                                    //    foreach (var item in missingInDict1)
                                    //    {
                                    //        MyVariable.differenceJianJiDic.Add(item.Key, item.Value);
                                    //    }
                                    //}
                                    #endregion
                                    //总控和测序都有的孔但是碱基不相同的
                                    var differentValues = MyVariable.inferJianJiDic.Where(pair => MyVariable.JianJiDic.ContainsKey(pair.Key) && MyVariable.JianJiDic[pair.Key] != pair.Value).ToList();
                                    if (differentValues.Any())
                                    {
                                        foreach (var item in differentValues)
                                        {
                                            MyVariable.differenceJianJiDic.Add(item.Key, item.Value);
                                        }
                                    }
                                    SaveDNAData(MyVariable.SN_DataProcessingStation, SerializeClass.completeParam.protocol_group_ids, SerializeClass.completeParam.data_taskId,
                                    SerializeClass.completeParam.data_idna, SerializeClass.mChipDataReportingToControl.chipTotalCount, SerializeClass.mChipDataReportingToControl.chipMatchCount);
                                    LogConfig.Instance.ShowMessageToList("Run", "实验进程:解析完成,结果总数:" + SerializeClass.mChipDataReportingToControl.chipTotalCount + "匹配数:" + SerializeClass.mChipDataReportingToControl.chipMatchCount, MsgType.Success, Color.Brown);
                                    SerializeClass.animationParam.taskStep = (int)_taskStepEnum.解析完成;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "文件解析失败", MsgType.Success, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                                this.RunStep = 200;
                            }
                            break;
                        case 200://给总控发送实验结果
                            if (MyVariable.newshow_IsOpenOver && !Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)))
                            {
                                TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).ClearNetData();
                                SerializeClass.mChipDataReportingToControl.infer_sequence = "";
                                SerializeClass.mChipDataReportingToControl.NG_hole = "";
                                foreach (var item in MyVariable.inferJianJiDic)
                                {
                                    SerializeClass.mChipDataReportingToControl.infer_sequence += item.Key + "-" + item.Value + "|";
                                }
                                if (SerializeClass.mChipDataReportingToControl.infer_sequence.Length > 0) // 确保字符串不为空
                                {
                                    SerializeClass.mChipDataReportingToControl.infer_sequence = SerializeClass.mChipDataReportingToControl.infer_sequence.Substring(0, SerializeClass.mChipDataReportingToControl.infer_sequence.Length - 1); // 截取从索引0开始到倒数第二个字符的子字符串
                                }
                                foreach (var item in MyVariable.differenceJianJiDic)
                                {
                                    SerializeClass.mChipDataReportingToControl.NG_hole += item.Key + "-" + item.Value + "|";
                                }
                                if (SerializeClass.mChipDataReportingToControl.NG_hole.Length > 0) // 确保字符串不为空
                                {
                                    SerializeClass.mChipDataReportingToControl.NG_hole = SerializeClass.mChipDataReportingToControl.NG_hole.Substring(0, SerializeClass.mChipDataReportingToControl.NG_hole.Length - 1); // 截取从索引0开始到倒数第二个字符的子字符串
                                    SerializeClass.mChipDataReportingToControl.matchResult = "NG";
                                    SerializeClass.animationParam.Result = "NG";
                                }
                                else
                                {
                                    SerializeClass.mChipDataReportingToControl.matchResult = "OK";
                                    SerializeClass.animationParam.Result = "OK";
                                }
                                string jsonStr3 = JsonConvert.SerializeObject(SerializeClass.mChipDataReportingToControl);
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).WriteDataStr(jsonStr3))
                                {
                                    LogToGeneral(jsonStr3);
                                    this.time = this.GetCurveTime();
                                    WaitDelayTime(0.3);
                                    SerializeClass.animationParam.general = (int)_generalEnum.测序结果上报;
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送实验结果", MsgType.Success, Color.Brown);
                                    this.RunStep = 220;
                                    break;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送数据失败", MsgType.Error, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }


                            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)) || MyVariable.show_IsOpen)
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "屏蔽总控...", MsgType.Success, Color.Brown);
                                this.RunStep = 10;
                            }
                            else
                            {
                                TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).ClearNetData();
                                SerializeClass.mChipDataReportingToControl.infer_sequence = "";
                                SerializeClass.mChipDataReportingToControl.NG_hole = "";
                                foreach (var item in MyVariable.inferJianJiDic)
                                {
                                    SerializeClass.mChipDataReportingToControl.infer_sequence += item.Key + "-" + item.Value + "|";
                                }
                                if (SerializeClass.mChipDataReportingToControl.infer_sequence.Length > 0) // 确保字符串不为空
                                {
                                    SerializeClass.mChipDataReportingToControl.infer_sequence = SerializeClass.mChipDataReportingToControl.infer_sequence.Substring(0, SerializeClass.mChipDataReportingToControl.infer_sequence.Length - 1); // 截取从索引0开始到倒数第二个字符的子字符串
                                }
                                foreach (var item in MyVariable.differenceJianJiDic)
                                {
                                    SerializeClass.mChipDataReportingToControl.NG_hole += item.Key + "-" + item.Value + "|";
                                }
                                if (SerializeClass.mChipDataReportingToControl.NG_hole.Length > 0) // 确保字符串不为空
                                {
                                    SerializeClass.mChipDataReportingToControl.NG_hole = SerializeClass.mChipDataReportingToControl.NG_hole.Substring(0, SerializeClass.mChipDataReportingToControl.NG_hole.Length - 1); // 截取从索引0开始到倒数第二个字符的子字符串
                                    SerializeClass.mChipDataReportingToControl.matchResult = "NG";
                                    SerializeClass.animationParam.Result = "NG";
                                }
                                else
                                {
                                    SerializeClass.mChipDataReportingToControl.matchResult = "OK";
                                    SerializeClass.animationParam.Result = "OK";
                                }
                                string jsonStr3 = JsonConvert.SerializeObject(SerializeClass.mChipDataReportingToControl);
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).WriteDataStr(jsonStr3))
                                {
                                    LogToGeneral(jsonStr3);
                                    this.time = this.GetCurveTime();
                                    WaitDelayTime(0.3);
                                    SerializeClass.animationParam.general = (int)_generalEnum.测序结果上报;
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送实验结果", MsgType.Success, Color.Brown);
                                    this.RunStep = 220;
                                }
                                else
                                {
                                    LogConfig.Instance.ShowMessageToList("Run", "向总控发送数据失败", MsgType.Error, Color.Red);
                                    throw new StationErrorException("通讯报警");
                                }
                            }
                            break;
                        case 220://接收总控信息
                            CheckCurrentRunStatus(0, 200, 200);
                            if (OverTimeS(time, Convert.ToInt32(ParameConfig.Instance.SystemParameDic[_ParamName.CCDTimeOut.ToString()].CurrentValue)))
                            {
                                LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据超时", MsgType.Error, Color.Red);
                                this.RunStep = 200;
                                throw new StationErrorException("通讯报警");
                            }
                            else
                            {
                                if (TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).NetCanRead())
                                {
                                    TCPClientConfig.Instance.GetClient(_TcpClientModule.GeneralControl.ToString()).LoopReadData(1, out Program.ControlReceived, Encoding.UTF8);
                                    LogFromGeneral(Program.ControlReceived);
                                    b_DataProcessingStation = MyVariable.GeneralResultReceive(Program.ControlReceived, out code_general);
                                    if (b_DataProcessingStation)
                                    {
                                        if (code_general == 200)
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "测序数据分析结果上报成功", MsgType.Success, Color.Green);
                                            this.RunStep = 10;
                                        }
                                        else
                                        {
                                            LogConfig.Instance.ShowMessageToList("Run", "测序数据分析结果上报失败", MsgType.Success, Color.Red);
                                            this.RunStep = 200;
                                            throw new StationErrorException("通讯报警");
                                        }
                                        SerializeClass.animationParam.general = (int)_generalEnum.无交互任务;
                                    }
                                    else
                                    {
                                        LogConfig.Instance.ShowMessageToList("Run", "总控反馈数据异常", MsgType.Success, Color.Red);
                                        this.RunStep = 200;
                                        throw new StationErrorException("通讯报警");
                                    }
                                }
                            }
                            break;
                            #endregion

                    }
                }
                /***暂停捕获***/
                catch (StationPauseException ex)
                {
                    LogConfig.Instance.ShowMessageToList("Run", _ThreadModule.数据处理线程.ToString() + "暂停捕获：" + RunStep.ToString(), MsgType.Success, Color.Blue);
                    StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Pause);
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
                    StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Error);
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
                    StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Stop);
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
            StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Stop);
        }

        public override void StationCalibRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Stop);
        }

        public override void StationCPKRun()
        {
            this.RunDone = true;
            StationConfig.Instance.StationDic[_ThreadModule.数据处理线程.ToString()].ChangeStatus(_StationStatus.Stop);
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



        /// <summary>
        /// 解析序列
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public bool DataAnalysisMethod(string filePath)
        {
            MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
            {
                MainForm_Data.mMainForm_Data.chart1.Series[0].Points.Clear(); // 清空所有数据点
                MyVariable.AutoAllJianJiDics.Clear();
                MyVariable.AutoJianJiDicsMost.Clear();
                MainForm_Data.mMainForm_Data.cbx_barcode.Items.Clear();
                MyVariable.AutoJianJiList.Clear();
                MyVariable.AutoNumList.Clear();
                MainForm_Data.mMainForm_Data.txt_JianJiMsg.Text = "";
                MainForm_Data.mMainForm_Data.lab_jianjiMax.Text = "测序结果(饼状图中百分比最高的碱基)：";
            }));

            if (IsProgress)
            {
                LogConfig.Instance.ShowMessageToList("Run", "解析中，无法重复运行", MsgType.Success, Color.Red);
                return false;
            };
            try
            {
                string folderPath = filePath;
                string[] subdirectories = Directory.GetDirectories(folderPath);
                TotalFileCount = 0;
                resultFolderModel = new ResultFolderVM()
                {
                    FolderLevel = 0,
                    FolderPath = folderPath,
                    FolderName = new DirectoryInfo(folderPath).Name,
                    FileList = SeqkitHelper.GetFileList(folderPath),
                    SubFolderList = GetSubFolderList(folderPath, 1)
                };
                TotalFileCount += resultFolderModel.FileList.Count;
                TotalDNACounts = 0;
                MatchDNACounts = 0;
                MatchJianJiCounts = 0;
                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                {
                    MainForm_Data.mMainForm_Data.lblCount.Text = $"测序文件{TotalFileCount}个";
                    MainForm_Data.mMainForm_Data.lab_totalDNA.Text = TotalDNACounts.ToString();
                    MainForm_Data.mMainForm_Data.lab_matchDNA.Text = MatchDNACounts.ToString();
                    MainForm_Data.mMainForm_Data.txt_FolderPath.Text = cexuFilePath;
                    MainForm_Data.mMainForm_Data.txt_IDNA.Text = IDNA_string;
                }));
                MatchedFileCount = 0;

                IsProgress = true;
                MatchData(resultFolderModel);

                //深拷贝,创建新对象
                foreach (var kvp in MyVariable.AutoAllJianJiDics)
                {
                    // 创建新的字典并复制内容
                    MyVariable.AutoJianJiDicsMost[kvp.Key] = new Dictionary<string, int>(kvp.Value);
                }
                //超过5种碱基用others代替总和
                foreach (var item in MyVariable.AutoJianJiDicsMost)
                {
                    if (MyVariable.AutoJianJiDicsMost[item.Key].Count > 5)
                    {
                        int sum = MyVariable.AutoJianJiDicsMost[item.Key].Values.Skip(4).Sum(); // 计算第五项开始到最后的所有值的和
                        string key = "others"; // 新的键名
                                               // 删除第五项及之后的键值对
                        var keysToDelete = MyVariable.AutoJianJiDicsMost[item.Key].Keys.Skip(4).ToList();
                        foreach (var k in keysToDelete)
                        {
                            MyVariable.AutoJianJiDicsMost[item.Key].Remove(k);
                        }
                        MyVariable.AutoJianJiDicsMost[item.Key][key] = sum; // 更新字典，键为"others"，值为sum
                    }
                }

                //编译字符给数字孪生
                string s = "";
                foreach (var item in MyVariable.AutoJianJiDicsMost)
                {
                    if (MyVariable.AutoJianJiDicsMost[item.Key].Count == 0)
                    {
                        continue;
                    }
                    s += Convert.ToInt32(item.Key.Replace("barcode", "")) + "-";
                    foreach (var members in MyVariable.AutoJianJiDicsMost[item.Key])
                    {
                        s += members.Key + "*" + members.Value + "&";
                    }
                    s = s.Substring(0, s.Length - 1) + "|";
                }
                if (s.Length > 0)
                {
                    s = s.Substring(0, s.Length - 1);
                }
                SerializeClass.animationParam.BaseMsg = s;

                //将每一个barcode中第一个(数量最多)碱基名称复制到一个新的字典中,用于总控比对
                foreach (var item in MyVariable.AutoAllJianJiDics)
                {
                    if (MyVariable.AutoAllJianJiDics[item.Key].Count != 0)
                    {
                        MyVariable.inferJianJiDic.Add(Convert.ToInt32(item.Key.Replace("barcode", "")), MyVariable.AutoAllJianJiDics[item.Key].First().Key);
                    }
                }

                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                {
                    foreach (var item in MyVariable.AutoAllJianJiDics)
                    {
                        MainForm_Data.mMainForm_Data.cbx_barcode.Items.Add(item.Key);
                    }
                    if (MainForm_Data.mMainForm_Data.cbx_barcode.Items.Count != 0)
                    {
                        MainForm_Data.mMainForm_Data.cbx_barcode.SelectedIndex = 0;
                    }
                    MainForm_Data.mMainForm_Data.lab_totalDNA.Text = TotalDNACounts.ToString();
                    MainForm_Data.mMainForm_Data.lab_matchDNA.Text = MatchDNACounts.ToString();
                    MainForm_Data.mMainForm_Data.lblExecMsg.Text = $"执行成功！";
                    MainForm_Data.mMainForm_Data.lblExecMsg.BackColor = Color.White;
                }));
                IsProgress = false;
                SerializeClass.mChipDataReportingToControl.chipTotalCount = TotalDNACounts;
                SerializeClass.mChipDataReportingToControl.chipMatchCount = MatchJianJiCounts;
                return true;
            }
            catch (Exception ex)
            {
                IsProgress = false;
                return false;
            }
        }
        /// <summary>
        /// 递归查找文件夹及文件
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="folerLevel"></param>
        /// <returns></returns>
        List<ResultFolderVM> GetSubFolderList(string basePath, int folerLevel)
        {
            var modelList = new List<ResultFolderVM>();
            foreach (var folderPath in Directory.GetDirectories(basePath))
            {
                var model = new ResultFolderVM()
                {
                    FolderLevel = folerLevel,
                    FolderPath = folderPath,
                    FolderName = new DirectoryInfo(folderPath).Name,
                    FileList = SeqkitHelper.GetFileList(folderPath),
                    SubFolderList = GetSubFolderList(folderPath, folerLevel + 1)
                };
                TotalFileCount += model.FileList.Count();
                modelList.Add(model);
            }
            if (folerLevel > MaxFolderLevel)
            {
                MaxFolderLevel = folerLevel;
            }
            return modelList;
        }


        /// <summary>
        /// 递归匹配处理
        /// </summary>
        /// <param name="model"></param>
        void MatchData(ResultFolderVM model)
        {
            foreach (var item in model.FileList)
            {
                if (!item.MatchedTxtPath.Contains("pass") || !item.MatchedTxtPath.Contains("fastq") || !item.MatchedTxtPath.Contains("barcode"))
                {
                    continue;
                }
                SeqkitHelper.MatcheAsTxt(IDNA_string, item.FilePath, item.MatchedTxtPath);
                item.OriginalCount = SeqkitHelper.GetOriginalCount(item.FilePath);
                item.MatchedCount = SeqkitHelper.GetMatcheCount(IDNA_string, item.FilePath);
                MyVariable.AutoSingleJianJiDics = SeqkitHelper.SingleJianJiInfer(5, IDNA_string, item.MatchedTxtPath);
                if (!MyVariable.AutoAllJianJiDics.ContainsKey(item.FolderName))
                {
                    MyVariable.AutoAllJianJiDics.Add(item.FolderName, MyVariable.AutoSingleJianJiDics);
                }
                else
                {
                    //两个字典合并成一个
                    var combinedDic = MyVariable.AutoAllJianJiDics[item.FolderName]
                        .Concat(MyVariable.AutoSingleJianJiDics)
                        .GroupBy(kvp => kvp.Key)
                        .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));

                    //排序
                    var sortedElementCounts = combinedDic
                        .OrderByDescending(kvp => kvp.Value)
                        .ToList();
                    combinedDic = sortedElementCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    MyVariable.AutoAllJianJiDics[item.FolderName] = combinedDic;
                }
                TotalDNACounts = TotalDNACounts + item.OriginalCount;
                MatchDNACounts = MatchDNACounts + item.MatchedCount;
                MatchJianJiCounts = MatchJianJiCounts + item.DNAMatchedCount;
                MatchedFileCount++;
                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                {
                    MainForm_Data.mMainForm_Data.lblExecMsg.Text = $"正在处理 {MatchedFileCount}/{TotalFileCount}";
                }));
                Thread.Sleep(50);
            }
            foreach (var item in model.SubFolderList)
            {
                MatchData(item);
            }
        }








        /// <summary>
        /// 测序实验数据分析结果
        /// </summary>
        /// <param name="resultSN">实验SN</param>
        /// <param name="resultName">实验名称</param>
        /// <param name="resultTaskID">实验任务ID</param>
        /// <param name="resultIDNA">iDNA</param>
        /// <param name="resultTotal">链条总数</param>
        /// <param name="resultMatch">正确配对条数</param>
        public void SaveDNAData(string resultSN, string resultName, int resultTaskID, string resultIDNA, int resultTotal, int resultMatch)
        {
            try
            {
                if (!Directory.Exists(@"E:\SWLog\CeXuData"))
                {
                    Directory.CreateDirectory(@"E:\SWLog\CeXuData");
                }
                string strDataPath = @"E:\SWLog\CeXuData\" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                if (!File.Exists(strDataPath))
                {
                    using (StreamWriter sww = new StreamWriter(strDataPath, true))
                    {
                        sww.WriteLine("时间,SN,实验名称,实验任务ID,iDNA,总数,碱基匹配数");
                    }
                }
                dtEndTime = DateTime.Now;     //记录结束时间
                using (StreamWriter sww = new StreamWriter(strDataPath, true))
                {
                    sww.WriteLine(dtEndTime.ToString("dd/MM/yyyy HH:mm:ss") + "," + resultSN + "," + resultName + "," + resultTaskID + "," + resultIDNA + "," + resultTotal + "," + resultMatch);
                }
            }
            catch (Exception d)
            {
                LogConfig.Instance.ShowMessageToList("Run", d.Message, MsgType.Error, Color.Red);
            }
        }


    }
}

