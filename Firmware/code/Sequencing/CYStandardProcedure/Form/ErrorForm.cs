using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CYCustomControl;
using System.Threading;
using CYAutoFramework;
using MsgBoxLib;

namespace CYStandardProcedure
{
    public partial class ErrorForm : Form
    {
        private AutoSizeMDIForm mAutosize = new AutoSizeMDIForm();

        private bool bool_error;

        /***按钮和窗体字典***/
        private Dictionary<ToolStripButton, Form> mFormDic = new Dictionary<ToolStripButton, Form>();
        private Dictionary<string, List<Image>> mBtnDic = new Dictionary<string, List<Image>>();
        /***当前窗体***/
        private Form mCurForm;
        /***当前按钮***/
        private ToolStripButton mCurBtn;

        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        public ErrorForm()
        {
            InitializeComponent();
        }

        public void SwitchWnd(ToolStripButton btn)
        {
            if (mCurBtn != btn)
            {
                btn.Image = mBtnDic[btn.Name][1];
                foreach (ToolStripButton va in toolStrip1.Items)
                {
                    if (va.Name != btn.Name)
                    {
                        va.Image = mBtnDic[va.Name][0];
                    }
                }
                mCurBtn = btn;
                if (mCurForm != null)
                {
                    mCurForm.Hide();
                }
                if (mCurForm != mFormDic[btn])
                {
                    mCurForm = mFormDic[btn];
                    mCurForm.TopLevel = false;
                    mCurForm.Parent = panel1;
                    mCurForm.Dock = DockStyle.Fill;
                    mCurForm.Show();
                }
            }
        }

        private void ErrorForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            foreach (ToolStripButton va in toolStrip1.Items)
            {
                List<Image> ls = new List<Image>();
                ls.Clear();
                if (va is ToolStripButton)
                {
                    switch (va.Name)
                    {
                        case "btn_dtRecord":
                            ls.Add(Properties.Resources.宕机记录未选中);
                            ls.Add(Properties.Resources.宕机记录选中);
                            break;
                        case "btn_dtTimeStatis":
                            ls.Add(Properties.Resources.宕机时间统计未选中);
                            ls.Add(Properties.Resources.宕机时间统计选中);
                            break;
                        case "btn_dtTimeClassify":
                            ls.Add(Properties.Resources.异常统计未选中);
                            ls.Add(Properties.Resources.异常统计选中);
                            break;
                        case "btn_dtDiscard":
                            ls.Add(Properties.Resources.抛料未选中);
                            ls.Add(Properties.Resources.抛料选中);
                            break;
                    }
                    mBtnDic.Add(va.Name, ls);
                }
            }
            /***按钮和窗体绑定字典***/
            mFormDic.Add(btn_dtRecord, new DowntimeRecordForm());
            mFormDic.Add(btn_dtTimeStatis, new DowntimeStatisticsForm());
            mFormDic.Add(btn_dtTimeClassify, new DowntimeQueryForm());
            mFormDic.Add(btn_dtDiscard, new DowntimeDiscardForm());
            btn_dtRecord.PerformClick();
            mAutosize.controllInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += ErrorForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void ErrorForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            if (strLanguage == "CH")
            {
                //toolTip1.SetToolTip(rbt_AlarmLog, "报警日志记录");
                //toolTip1.SetToolTip(rbt_AlarmStatus, "报警状态显示");
                toolTip1.SetToolTip(btn_ClearDownTime, "清除异常");
            }
            else if (strLanguage == "EN")
            {
                //toolTip1.SetToolTip(rbt_AlarmLog, "AlarmLog");
                //toolTip1.SetToolTip(rbt_AlarmStatus, "AlarmStatus");
                toolTip1.SetToolTip(btn_ClearDownTime, "Clear Exception");
            }
            else
            {
                //toolTip1.SetToolTip(rbt_AlarmLog, "Xóa báo động");
                //toolTip1.SetToolTip(rbt_AlarmStatus, "Hiển thị trạng thái báo động");
                toolTip1.SetToolTip(btn_ClearDownTime, "Xóa ngoại lệ");
            }
        }

        private void ErrorForm_SizeChanged(object sender, EventArgs e)
        {
            mAutosize.controlAutoSize(this);
        }

        private void btn_dtRecord_Click(object sender, EventArgs e)
        {
            SwitchWnd(btn_dtRecord);
        }

        private void btn_dtTimeStatis_Click(object sender, EventArgs e)
        {
            SwitchWnd(btn_dtTimeStatis);
        }

        private void btn_dtTimeClassify_Click(object sender, EventArgs e)
        {
            SwitchWnd(btn_dtTimeClassify);
        }

        private void btn_dtDiscard_Click(object sender, EventArgs e)
        {
            SwitchWnd(btn_dtDiscard);
        }

        private void btn_ClearDownTime_Click(object sender, EventArgs e)
        {
            /***针对8338卡和汇川电机，软件设置驱动器参数，清除轴的报警状态***/
            //for (int i = 0; i < Enum.GetNames(typeof(_Axis)).Length; i++)
            //{
            //    //此方法经验证，只能清除一般故障，电机过载暂时无法清除
            //    MotionConfig.GetInstance().ClearAixsErr(((_Axis)i).ToString());
            //}

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
            bool_error = SoftWareForm.carryclaw_initialize.Rtu_carryClaw.ForceCoil(Program.carryClawConfig.DevAdd, 1402, false);
            if (bool_error)
            {
                bool_error = false;
                bool_error = SoftWareForm.carryclaw_initialize.Rtu_carryClaw.ForceCoil(Program.carryClawConfig.DevAdd, 1402, true);
            }
            /***清除机器人报警***/
            IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人清除报警.ToString(), 1);
            Task.Run(() =>
            {
                Thread.Sleep(500);
                IOConfig.Instance.SetSingleOut(_OutputCollect.Aubo机器人清除报警.ToString(), 0);
            });
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
            /***如果是异常状态切换为暂停***/
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Error)
            {
                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeStatus(ObjectStation._StationStatus.Pause);
            }
            for (int i = 1; i < Enum.GetNames(typeof(_ThreadModule)).Length; i++)
            {
                if (StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].mCurStatus == ObjectStation._StationStatus.Error)
                {
                    StationConfig.Instance.StationDic[Enum.GetNames(typeof(_ThreadModule))[i]].ChangeStatus(ObjectStation._StationStatus.Pause);
                }
            }
        }

        private void rbt_initial_Click(object sender, EventArgs e)
        {
            MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.YesNo, true);
            mb.TopMost = true;
            mb.MsgShowDialog("提示", "请确认是否初始化记忆");
            string btn = mb.ret.SelectedBtn;
            if (btn == "btn_A")
            {
                SerializeClass.mMemory.FeedingStation_state = MemoryClass.FeedingStation_State.空闲;
                SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.空闲;
                SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.空闲;
                SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.空闲;
                SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪默认松开;
                SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪默认松开;
                SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.未取枪头;
                SerializeClass.mMemory.area = MemoryClass.Area.枪头区1;
                SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                SerializeClass.mMemory.DataProcessingStation_state = MemoryClass.DataProcessingStation_State.空闲;
                Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 0);
                Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                Program.modbusTcp_PLC.WriteSingleRegister(1, 64614, 0);
                Program.modbusTcp_PLC.WriteSingleRegister(1, 64615, 0);
                Program.modbusTcp_PLC.WriteSingleRegister(1, 64616, 0);
                Program.modbusTcp_PLC.WriteSingleRegister(1, 64622, 0);
                Program.modbusTcp_PLC.WriteSingleRegister(1, 64634, 0);
                MyVariable.b_StatusToControl = false;
                MyVariable.need_Completed = false;
                MyVariable.CCD_QiPao = false;
                MyVariable.JianJiShiBie_Start = false;
                MyVariable.newshow_step1 = false;
                MyVariable.show_memory = 0;
                MyVariable.FunctionStep = 0;
                MyVariable.File_Copy.Clear();
                LogConfig.Instance.ShowMessageToList("Run", "初始化记忆成功！", MsgType.Success, Color.Green);
            }

        }
    }
}
