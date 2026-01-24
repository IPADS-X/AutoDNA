using CYAutoFramework;
using MsgBoxLib;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;

namespace CYStandardProcedure
{
    /// <summary>
    /// Hive上传信息记录界面
    /// </summary>
    public partial class HiveForm : Form
    {
        #region 控件窗体自适应
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
            Program.m_Hive.UpdataDashboard(dataGridView);
            Program.m_Hive.UpdateParameterToGrid(dataGridView1);
        }
        #endregion

        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        /// <summary>
        /// Hive上传信息记录界面
        /// </summary>
        public HiveForm()
        {
            InitializeComponent();
        }

        private void HiveForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            Program.m_Hive.UpdataErrorStatis += UpdataErrorChart;
            Program.m_Hive.UpdateErrorChart(dateTime_start .Value ,dateTime_end .Value , chart2);

            timer1.Enabled = true;

            /***子窗体自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += HiveForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);

            groupBox1.BackColor = System.Drawing.Color.Gainsboro;
            groupBox2.BackColor = System.Drawing.Color.Gainsboro;
            groupBox3.BackColor = Color .FromArgb (255,143,0);

            tabControl1.SelectedIndex = 0;
            tabControl1.SelectedIndex = 1;
            tabControl1.SelectedIndex = 0;
            lbl_Site.Text = Program.m_Hive.HiveSite;

            #region 计划停机类型添加
            comboBox1.Items.Clear();
            comboBox1.Items.Add("PD-01#日常维护");
            comboBox1.Items.Add("PD-02#胶水更换");
            comboBox1.Items.Add("PD-03#针头更换");
            comboBox1.Items.Add("PD-04#螺栓更换");
            comboBox1.Items.Add("PD-05#压力测试");
            comboBox1.Items.Add("PD-06#激光校准");
            comboBox1.Items.Add("PD-07#其他耗材更换或报警");
            comboBox1.Items.Add("PD-08#材料更换");
            comboBox1.Items.Add("PD-09#周维护");
            comboBox1.Items.Add("PD-10#胶水称重");
            comboBox1.Items.Add("PD-101#其他行为计划");
            comboBox1.SelectedIndex = 0;
            #endregion 

            dateTime_end.Value = DateTime.Now;
            dateTime_start.Value = DateTime.Now.AddDays(0 - Convert.ToInt32(TrackBar_Day.Value));


        }

        private void HiveForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);

            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Save, "参数保存");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Save, "Parameter saving");
            }
            else
            {
                toolTip1.SetToolTip(btn_Save, "Lưu tham số");
            }
        }




        private void UpdataErrorChart()
        {
            dateTime_end.Value = DateTime.Now;
            Program.m_Hive.UpdateErrorChart(dateTime_start.Value, dateTime_end.Value, chart2);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            Program.m_Hive.UpdateHiveTimeChart(chart1);
            label15.Text = Program.m_Hive.DischargSN;
            if (Program.m_Hive.DischargResult)
            {
                label15.BackColor = Color.FromArgb(0, 249, 0);
                lbl_PassFail.Text = "PASS";
            }
            else
            {
                label15.BackColor = Color.FromArgb(236, 93, 87);
                lbl_PassFail.Text = "FAILL";
            }
            lbl_InputOutput.Text = $"{Program.m_Hive.Input }/{Program.m_Hive.Output  }";
            lbl_Yield.Text = Program.m_Hive.Yield.ToString();
            lbl_UPH.Text = Program.m_Hive.UPH.ToString();
            lab_prestatus.Text = Program.m_Hive.PreHiveStatus.ToString();
            lab_status.Text = Program.m_Hive.HiveStatus.ToString();
            lab_CT.Text = Program.m_Hive.CT.ToString();




            #region 手动切换Hive状态
            switch (Program.m_Hive.HiveStatus)
            {
                case _HiveMachineStaus.正常做料状态:
                    btn_Downtime.Enabled = true;
                    btn_Eng.Enabled = false;
                    btn_Idle.Enabled = true;
                    Btn_Run.Enabled = false;
                    break;
                case _HiveMachineStaus.屏蔽上传做料状态:
                    btn_Downtime.Enabled = true;
                    btn_Eng.Enabled = false;
                    btn_Idle.Enabled = true;
                    Btn_Run.Enabled = false;
                    break;
                case _HiveMachineStaus.空闲状态:
                    btn_Downtime.Enabled = true;
                    btn_Eng.Enabled = false;
                    btn_Idle.Enabled = false;
                    Btn_Run.Enabled = false;
                    break;
                case _HiveMachineStaus.计划停机状态:
                    btn_Downtime.Enabled = false;
                    btn_Eng.Enabled = true;
                    btn_Idle.Enabled = false;
                    Btn_Run.Enabled = true;
                    break;
                case _HiveMachineStaus.宕机状态:
                    btn_Downtime.Enabled = false;
                    btn_Eng.Enabled = true;
                    btn_Idle.Enabled = false;
                    Btn_Run.Enabled = true;
                    break;

            }

            #endregion



        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            Program.m_Hive.UpdateGridToFile(dataGridView1);
            Program.m_Hive.UpdateParameterToGrid(dataGridView1);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox10.Visible = false;
            if (tabControl1.SelectedIndex == 0)
            {
                lbl_SWVersion.Text = Program.m_Hive.Sw_Version;
                lbl_MSHash.Text = Program.m_Hive.UUID;
                lbl_MainSWPath.Text = this.GetType().Assembly.Location;
                Program.m_Hive.UpdataDashboard(dataGridView);
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                Program.m_Hive.UpdateParameterToGrid(dataGridView1);
            }
        }



        /// <summary>
        /// 根据卡号获取人员信息
        /// </summary>
        /// <param name="number">卡ID</param>
        /// <param name="Name">姓名</param>
        /// <param name="CardID">工号</param>
        /// <param name="Lever">权限等级</param>
        private void GetPermissions(string number, out string Name, out string CardID, out int Lever)
        {
            try
            {
                INIFile ini = new INIFile(Program.m_Hive.AdminPath);
                Name = ini.Read<string>(number, "Name");
                CardID = ini.Read<string>(number, "CardID");
                Lever = ini.Read<int>(number, "UserLevel");
            }
            catch (Exception ex)
            {
                Name = "";
                CardID = "";
                Lever = 1;
            }
        }

        private void btn_Plaaned_Click(object sender, EventArgs e)
        {
            if (Program.m_Hive.HiveStatus == _HiveMachineStaus.宕机状态)
            {
                MessageBox.Show("当前状态为宕机状态，不能切换为计划停机状态");
                return;
            }
            else
            {
                string btn;
                string id;
                string sn;
                if (comboBox1.Text.Split('#')[0] == "PD-02" || comboBox1.Text.Split('#')[0] == "PD-08")
                {
                    MsgForm mb = new MsgForm();
                    mb.ShowDialog();
                    id = mb.CardID;
                    sn = mb.NewSN;
                    btn = mb.btn;
                }
                else
                {
                    MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.YesNo, false);
                    mb.MsgShowDialog("确认切换HIVE状态？", "输入员工工号");
                    btn = mb.ret.SelectedBtn;
                    id = mb.ret.RichText.Trim().Replace("\n", "");
                    sn = "";
                }
                if (btn == "btn_A")
                {
                    if (id == "")
                    {
                        MessageBox.Show("当前登录人没有更改权限");
                        return;
                    }
                    string name;
                    string cardid;
                    int level;
                    GetPermissions(id, out name, out cardid, out level);
                    if (level > 0)
                    {
                        Program.m_Hive.Oldsn = Program.m_Hive.Newsn;
                        Program.m_Hive.Newsn = sn;
                        Program.m_Hive.HivePlannedCode = comboBox1.Text.Split('#')[0];
                        Program.m_Hive.HiveStatus = _HiveMachineStaus.计划停机状态;


                        string msg1 = "姓名： " + name + "  卡号：  " + cardid + " 在" + DateTime.Now.ToString() + "   手动切换了Hive的状态";
                        string msg2 = " Hive上一次状态：" + Program.m_Hive.PreHiveStatus + "   当前状态：" + _HiveMachineStaus.计划停机状态.ToString();
                        HiveLog.WriteModification(msg1);
                        HiveLog.WriteModification(msg2);

                    }
                    else
                    {
                        MessageBox.Show("当前登录人没有更改权限");
                        return;
                    }
                }



            }
        }

        private void Btn_Run_Click(object sender, EventArgs e)
        {

            MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.YesNo, false);
            mb.MsgShowDialog("确认切换HIVE状态？", "输入员工工号");
            string btn = mb.ret.SelectedBtn;
            string id = mb.ret.RichText.Trim().Replace("\n", "");
            if (btn == "btn_A")
            {
                if (id == "")
                {
                    MessageBox.Show("当前登录人没有更改权限");
                    return;
                }
                string name;
                string cardid;
                int level;
                GetPermissions(id, out name, out cardid, out level);
                if (level > 0)
                {
                    Program.m_Hive.HivePlannedCode = comboBox1.Text.Split('#')[0];
                    Program.m_Hive.HiveStatus = _HiveMachineStaus.正常做料状态;

                    string msg1 = "姓名： " + name + "  卡号：  " + cardid + " 在" + DateTime.Now.ToString() + "   手动切换了Hive的状态";
                    string msg2 = " Hive上一次状态：" + Program.m_Hive.PreHiveStatus + "   当前状态：" + _HiveMachineStaus.正常做料状态.ToString();
                    HiveLog.WriteModification(msg1);
                    HiveLog.WriteModification(msg2);

                }
                else
                {
                    MessageBox.Show("当前登录人没有更改权限");
                    return;
                }
            }

        }

        private void btn_Eng_Click(object sender, EventArgs e)
        {
            MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.YesNo, false);
            mb.MsgShowDialog("确认切换HIVE状态？", "输入员工工号");
            string btn = mb.ret.SelectedBtn;
            string id = mb.ret.RichText.Trim().Replace("\n", "");
            if (btn == "btn_A")
            {
                if (id == "")
                {
                    MessageBox.Show("当前登录人没有更改权限");
                    return;
                }
                string name;
                string cardid;
                int level;
                GetPermissions(id, out name, out cardid, out level);
                if (level > 0)
                {
                    Program.m_Hive.HivePlannedCode = comboBox1.Text.Split('#')[0];
                    Program.m_Hive.HiveStatus = _HiveMachineStaus.屏蔽上传做料状态;

                    string msg1 = "姓名： " + name + "  卡号：  " + cardid + " 在" + DateTime.Now.ToString() + "   手动切换了Hive的状态";
                    string msg2 = " Hive上一次状态：" + Program.m_Hive.PreHiveStatus + "   当前状态：" + _HiveMachineStaus.屏蔽上传做料状态.ToString();
                    HiveLog.WriteModification(msg1);
                    HiveLog.WriteModification(msg2);

                }
                else
                {
                    MessageBox.Show("当前登录人没有更改权限");
                    return;
                }
            }
        }

        private void btn_Idle_Click(object sender, EventArgs e)
        {
            MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.YesNo, false);
            mb.MsgShowDialog("确认切换HIVE状态？", "输入员工工号");
            string btn = mb.ret.SelectedBtn;
            string id = mb.ret.RichText.Trim().Replace("\n", "");
            if (btn == "btn_A")
            {
                if (id == "")
                {
                    MessageBox.Show("当前登录人没有更改权限");
                    return;
                }
                string name;
                string cardid;
                int level;
                GetPermissions(id, out name, out cardid, out level);
                if (level > 0)
                {
                    Program.m_Hive.HivePlannedCode = comboBox1.Text.Split('#')[0];
                    Program.m_Hive.HiveStatus = _HiveMachineStaus.空闲状态;

                    string msg1 = "姓名： " + name + "  卡号：  " + cardid + " 在" + DateTime.Now.ToString() + "   手动切换了Hive的状态";
                    string msg2 = " Hive上一次状态：" + Program.m_Hive.PreHiveStatus + "   当前状态：" + _HiveMachineStaus.空闲状态.ToString();
                    HiveLog.WriteModification(msg1);
                    HiveLog.WriteModification(msg2);

                }
                else
                {
                    MessageBox.Show("当前登录人没有更改权限");
                    return;
                }
            }
        }

        private void btn_Downtime_Click(object sender, EventArgs e)
        {
            MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.YesNo, false);
            mb.MsgShowDialog("确认切换HIVE状态？", "输入员工工号");
            string btn = mb.ret.SelectedBtn;
            string id = mb.ret.RichText.Trim().Replace("\n", "");
            if (btn == "btn_A")
            {
                if (id == "")
                {
                    MessageBox.Show("当前登录人没有更改权限");
                    return;
                }
                string name;
                string cardid;
                int level;
                GetPermissions(id, out name, out cardid, out level);
                if (level > 0)
                {
                    Program.m_Hive.HivePlannedCode = comboBox1.Text.Split('#')[0];
                    Program.m_Hive.HiveStatus = _HiveMachineStaus.宕机状态;

                    string msg1 = "姓名： " + name + "  卡号：  " + cardid + " 在" + DateTime.Now.ToString() + "   手动切换了Hive的状态";
                    string msg2 = " Hive上一次状态：" + Program.m_Hive.PreHiveStatus + "   当前状态：" + _HiveMachineStaus.宕机状态.ToString();
                    HiveLog.WriteModification(msg1);
                    HiveLog.WriteModification(msg2);

                }
                else
                {
                    MessageBox.Show("当前登录人没有更改权限");
                    return;
                }
            }
        }

        private void btn_UploadMachineState_Click(object sender, EventArgs e)
        {
            txt_HiveSend.Text = "";
            txt_HiveReceive.Text = "";
            txt_HiveSend.Clear();
            txt_HiveReceive.Clear();
            string send = "";
            HiveMachineStatusInfo1 msg = new HiveMachineStatusInfo1();
            HiveMachineStatusData1 data = new HiveMachineStatusData1();

            HiveMachineStatusInfo2 msg1 = new HiveMachineStatusInfo2();
            HiveMachineStatusData2 data1 = new HiveMachineStatusData2();

            HiveMachineStatusInfo3 msg2 = new HiveMachineStatusInfo3();
            HiveMachineStatusData3 data2 = new HiveMachineStatusData3();
            switch (cmb_MachineState .Text .Trim ())
            {
                case "Running":
                   
                    msg.machine_state = "1";
                    msg.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    data.state   ="1" ;
                    data.message_id  ="" ;
                    msg.data = data;
                    send = JsonConvert.SerializeObject(msg);//根据结构体序列化信息
                    break;
                case "Idle":
                    msg.machine_state = "2";
                    msg.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    data.state = "2";
                    data.message_id = "";
                    msg.data = data;
                    send = JsonConvert.SerializeObject(msg);//根据结构体序列化信息
                    break;

                case "Engineering":

                    msg.machine_state = "3";
                    msg.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    data.state = "3";
                    data.message_id = "";
                    msg.data = data;
                    send = JsonConvert.SerializeObject(msg);//根据结构体序列化信息
                    break;

                case "PlannedDowntime":

                    msg1.machine_state = "4";
                    msg1.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    data1.state = "4";
                    data1.message_id = "";
                    data1.code ="PD01" ;
                    data1.error_message = "Daily Maintennance";
                    data1.MS_SHA1 = Program .m_Hive .UUID ;
                    data1.sw_version = Program .m_Hive.Sw_Version ;
                    data1.previous_state = "1";
                    data1.erroe_detail = data1.error_message;
                    data1.badge = "";
                    data1.CD_SHA1 = Program.m_Hive.UUID+"QWEQEQWDQWEWQEQE1312312312";
                    msg1.data = data1;
                    send = JsonConvert.SerializeObject(msg1);//根据结构体序列化信息
                    break;

                case "Downtime":
                    msg1.machine_state = "5";
                    msg1.state_change_time = string.Format("{0}T{1}+0{2}00", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), DateTime.Now.ToString("%z").Remove(0, 1));
                    data1.state = "5";
                    data1.message_id = "";
                    data1.code = "F01ESSE-01-03";
                    data1.error_message = "The emergency stop button is pressed";
                    data1.MS_SHA1 = Program.m_Hive.UUID;
                    data1.sw_version = Program.m_Hive.Sw_Version;
                    data1.previous_state = "1";
                    data1.erroe_detail = data1.error_message;
                    data1.badge = "";
                    data1.CD_SHA1 = Program.m_Hive.UUID + "QWEQEQWDQWEWQEQE1312312312";
                    msg1.data = data1;
                    send = JsonConvert.SerializeObject(msg1);//根据结构体序列化信息
                    break;
            }
            txt_HiveSend.Text = send ;
            Task.Factory.StartNew(new Action(() =>
            {
                string Rec1 = HTTPPostMsg(dataGridView1 [1,0].Value .ToString (), send );
                Invoke(new Action(() =>
                {
                    txt_HiveReceive.Text = Rec1;
                }));

            }));

        }

        private void btn_UploadErrorData_Click(object sender, EventArgs e)
        {
            txt_HiveSend.Text = "";
            txt_HiveReceive.Text = "";
            txt_HiveSend.Clear();
            txt_HiveReceive.Clear();

            HiveMachineErrorInfo edj = new HiveMachineErrorInfo();
            HiveMachineErrorData edjd = new HiveMachineErrorData();
            edj.message = txt_ErrorDataMessage.Text.Trim();
            edj.code = txt_ErrorDataCode.Text.Trim();
            edj.severity = cmb_severity.Text.Trim();
            edj.occurrence_time = string.Format("{0}T{1}+0800", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"));
            edj.resolved_time = string.Format("{0}T{1}+0800", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"));

            edjd.hive_state = txt_ErrorData_DataHiveState.Text.Trim();
            edjd.error_detail = txt_ErrorData_DataHiveDetail.Text.Trim();
            edj.data = edjd;

            txt_ErrorDataOccurrencetime.Text = edj.occurrence_time;
            txt_ErrorDataResolvedtime.Text = edj.resolved_time;
            string SendHive = string.Empty;
            SendHive = JsonConvert.SerializeObject(edj);
            txt_HiveSend.Text = SendHive;
            Task.Factory.StartNew(new Action(() =>
            {
                string Rec1 = HTTPPostMsg(dataGridView1[1, 1].Value.ToString(), SendHive);
                Invoke(new Action(() =>
                {
                    txt_HiveReceive.Text = Rec1;
                }));

            }));
        }

        /// <summary>
        /// Http POST 方法
        /// </summary>
        /// <param name="purl"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private string HTTPPostMsg(string purl, string str)
        {
            try
            {
                string StrDate = "";
                string strValue = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(purl);
                request.Method = "POST";
                request.Timeout = 100;
                request.ContentType = "application/json";
                var MemStream = new MemoryStream();
                var DataBytes = Encoding.UTF8.GetBytes(str);
                MemStream.Write(DataBytes, 0, DataBytes.Length);

                request.ContentLength = MemStream.Length;
                request.Proxy = null;
                request.ServicePoint.Expect100Continue = false;
                Stream WriterValue = request.GetRequestStream();

                MemStream.Position = 0;
                var BufferValue = new byte[MemStream.Length];
                MemStream.Read(BufferValue, 0, BufferValue.Length);
                MemStream.Close();

                WriterValue.Write(BufferValue, 0, BufferValue.Length);
                WriterValue.Close();
                HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                Stream s = response.GetResponseStream();
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate;
                }
                response.Close();
                return strValue;
            }
            catch (Exception EX)
            {
                return EX.Message;
            }
        }

        private void btn_MachineLog_Click(object sender, EventArgs e)
        {
            string v_OpenFolderPath = @"E:\SWLog\Standard\NormalLog\RunLog";
            if (!Directory.Exists(v_OpenFolderPath))
            {
                return;
            }
            System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
        }

        private void btn_HIVELog_Click(object sender, EventArgs e)
        {
            string v_OpenFolderPath = @"E:\SWLog\Standard\NormalLog\Hive";
            if (!Directory.Exists(v_OpenFolderPath))
            {
                return;
            }
            System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
        }

        private void lbl_MainSWPath_Click(object sender, EventArgs e)
        {
            string v_OpenFolderPath = Environment.CurrentDirectory;
           if (!Directory.Exists(v_OpenFolderPath))
            {
                return;
            }
            System.Diagnostics.Process.Start("explorer.exe",  v_OpenFolderPath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.m_Hive.InputTime = DateTime.Now;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.m_Hive.HiveErrorMsg("1工位机械手报警");
        }

        private void TrackBar_Day_ValueChangedDone(object sender, EventArgs e)
        {
            dateTime_end.Value = DateTime.Now;
            dateTime_start.Value = DateTime.Now.AddDays(0 - Convert.ToInt32(TrackBar_Day.Value));
        }

        private void dateTime_start_ValueChanged(object sender, EventArgs e)
        {
            if (dateTime_end.Value  <= dateTime_start.Value )
            {
                return;
            }
            else
            {
                Program.m_Hive.UpdateErrorChart(  dateTime_start.Value, dateTime_end.Value, chart2);
            }
        }

        private void dateTime_end_ValueChanged(object sender, EventArgs e)
        {
            if (dateTime_end.Value <= dateTime_start.Value)
            {
                return;
            }
            else
            {
                Program.m_Hive.UpdateErrorChart(dateTime_start.Value, dateTime_end.Value, chart2);
            }
        }



        /// <summary>
        /// 获取指定文件的哈希值
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        private string GetFileHash(string path)
        {
            var hash = SHA256.Create();
            var stream = new FileStream(path, FileMode.Open);
            byte[] hashbyte = hash.ComputeHash(stream);
            stream.Close();
            return BitConverter.ToString(hashbyte).Replace("-", "");
        }

        private void chart1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (groupBox10.Visible)
                {
                    groupBox10.Visible = false;
                }

                else
                {
                    dataGridView2.ReadOnly = false;
                    int Day = 0;
                    double count = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                    if (count <= 1)
                    {
                        Day = 1;
                    }
                    else
                    {
                        Day = Convert.ToInt32(count);
                    }

                    double[] StatusProportion = new double[5];

                    StatusProportion = Program.m_Hive.GetDayStatuseTime(Day);

                    dataGridView2.Rows.Clear();
                    dataGridView2.Columns.Clear();
                    dataGridView2.Columns.Add("Column1", "状态");
                    dataGridView2.Columns.Add("Column2", "比例");
                    dataGridView2.Columns.Add("Column3", "时间");
                    int width = dataGridView2.Width;
                    dataGridView2.Columns[0].Width = (int)(width * 0.4);
                    dataGridView2.Columns[1].Width = (int)(width * 0.3);
                    dataGridView2.Columns[2].Width = (int)(width * 0.2);
                    for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    {
                        dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dataGridView2.Columns[i].Resizable = DataGridViewTriState.NotSet;
                        //view.Columns[i].ReadOnly = false;
                    }
                    dataGridView2.EnableHeadersVisualStyles = false;//缺少该行代码，标题的样式无法改变
                    dataGridView2.RowHeadersVisible = false;//影藏行的标题头
                    dataGridView2.AllowUserToResizeRows = false;//行不可调整
                    dataGridView2.AllowUserToResizeColumns = false;//列不可调整
                    dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
                    dataGridView2.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                    dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                    dataGridView2.ColumnHeadersHeight = 30;
                    dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", (float)10, FontStyle.Bold);
                    dataGridView2.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                    dataGridView2.GridColor = Color.FromArgb(149, 148, 142);
                    dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                    dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.LightYellow;


                    string[] status = new string[] { "Running", "Idle", "Engineering", "Planned Downtime", "Downtime" };
                    string[] Proportion = new string[5];
                    string[] time = new string[5];
                    for (int i = 0; i < 5; i++)
                    {
                        Proportion[i] = $"{StatusProportion[i] * 100}%";
                        if (Day != 7)
                        {
                            time[i] = (24 * 60 * StatusProportion[i]).ToString("f3");
                        }
                        else
                        {
                            time[i] = ((DateTime.Now - DateTime.Now.Date).TotalMinutes * StatusProportion[i]).ToString("f3");
                        }
                    }


                    for (int i = 0; i < 5; i++)
                    {
                        dataGridView2.Rows.Add();
                        dataGridView2.Rows[i].DefaultCellStyle.Font = new Font("微软雅黑", (float)9.5, FontStyle.Bold);
                        dataGridView2[0, i].Value = status[i];
                        dataGridView2[1, i].Value = Proportion[i];
                        dataGridView2[2, i].Value = time[i];
                    }


                    dataGridView2.ReadOnly = true;
                    groupBox10.Visible = true;
                }
            }
            catch
            {

            }
           
           
        }

        private void chart1_MouseLeave(object sender, EventArgs e)
        {
            groupBox10.Visible = false;
        }

        private void chart1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                dataGridView2.ReadOnly = false;
                int Day = 0;
                double count = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                if (count <= 1)
                {
                    Day = 1;
                }
                else if (count >7)
                {
                    Day = 7;
                }
                else
                {
                    Day =Convert .ToInt16 ( count );
                }

                double[] StatusProportion = new double[5];

                StatusProportion = Program.m_Hive.GetDayStatuseTime(Day);

                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();
                dataGridView2.Columns.Add("Column1", "状态");
                dataGridView2.Columns.Add("Column2", "比例");
                dataGridView2.Columns.Add("Column3", "时间");
                int width = dataGridView2.Width;
                dataGridView2.Columns[0].Width = (int)(width * 0.4);
                dataGridView2.Columns[1].Width = (int)(width * 0.3);
                dataGridView2.Columns[2].Width = (int)(width * 0.2);
                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView2.Columns[i].Resizable = DataGridViewTriState.NotSet;
                    //view.Columns[i].ReadOnly = false;
                }
                dataGridView2.EnableHeadersVisualStyles = false;//缺少该行代码，标题的样式无法改变
                dataGridView2.RowHeadersVisible = false;//影藏行的标题头
                dataGridView2.AllowUserToResizeRows = false;//行不可调整
                dataGridView2.AllowUserToResizeColumns = false;//列不可调整
                dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
                dataGridView2.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dataGridView2.ColumnHeadersHeight = 30;
                dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", (float)10, FontStyle.Bold);
                dataGridView2.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                dataGridView2.GridColor = Color.FromArgb(149, 148, 142);
                dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.LightYellow;


                string[] status = new string[] { "Running", "Idle", "Engineering", "Planned Downtime", "Downtime" };
                string[] Proportion = new string[5];
                string[] time = new string[5];
                for (int i = 0; i < 5; i++)
                {
                    Proportion[i] = $"{(StatusProportion[i] * 100).ToString ("f2")}%";
                    if (Day != 7)
                    {
                        time[i] = (24 * 60 * StatusProportion[i]).ToString("f3");
                    }
                    else
                    {
                        time[i] = ((DateTime.Now - DateTime.Now.Date).TotalMinutes * StatusProportion[i]).ToString("f3");
                    }
                }


                for (int i = 0; i < 5; i++)
                {
                    dataGridView2.Rows.Add();
                    dataGridView2.Rows[i].DefaultCellStyle.Font = new Font("微软雅黑", (float)9.5, FontStyle.Bold);
                    dataGridView2[0, i].Value = status[i];
                    dataGridView2[1, i].Value = Proportion[i];
                    dataGridView2[2, i].Value = time[i];
                }


                dataGridView2.ReadOnly = true;
                groupBox10.Visible = true;
            }
            catch { }
        }
    }
}
