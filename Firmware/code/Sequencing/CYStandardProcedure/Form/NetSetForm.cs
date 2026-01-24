using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CYStandardProcedure
{
    public partial class NetSetForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        public StartReportingToControl_Form mStartReportingToControl_Form = new StartReportingToControl_Form();
        public CompleteReportingToControl_Form mCompleteReportingToControl_Form = new CompleteReportingToControl_Form();
        public ChipDataReportingToControl_Form mChipDataReportingToControl_Form = new ChipDataReportingToControl_Form();
        public SearchFolloUpTaskToControl_Form mSearchFolloUpTaskToControl_Form = new SearchFolloUpTaskToControl_Form();

        /***提示语***/
        private ToolTip toolTip1 = new ToolTip();

        /***自动接收线程***/
        Thread autoReceiveThread;
        ThreadStart autoReceiveThreadStart;

        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            ParameConfig.Instance.ParameCfgDic["Net"].UpdateParameterToGrid(dataGridView1);
            /***当窗体大小改变时候也需要重新设置标题语言***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        public NetSetForm()
        {
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        private void SerialSetForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            if (ParameConfig.Instance.NetParameDic.Count > 0)
            {
                foreach (KeyValuePair<string, NetClientParame> va in ParameConfig.Instance.NetParameDic)
                {
                    cmb_Net.Items.Add(va.Key);
                }
                cmb_Net.SelectedIndex = 0;
            }
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += NetSetForm_LanguageChangeEvent;
            rbt_float.Checked = true;
            cbx_send.SelectedIndex = 0;
            cbx_receive.SelectedIndex = 0;
        }

        private void NetSetForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);

            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Connect, "重新连接");
                toolTip1.SetToolTip(btn_Receive, "接受数据");
                toolTip1.SetToolTip(btn_Save, "保存数据");
                toolTip1.SetToolTip(btn_Send, "发送数据");
                toolTip1.SetToolTip(pic_NetStatus, "连接状态");
                toolTip1.SetToolTip(rbt_queryPLC, "查询");
                toolTip1.SetToolTip(rbt_sendPLC, "给PLC发送");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Connect, "Reconnecting");
                toolTip1.SetToolTip(btn_Receive, "ReceiveData");
                toolTip1.SetToolTip(btn_Save, "SaveData");
                toolTip1.SetToolTip(btn_Send, "SendData");
                toolTip1.SetToolTip(pic_NetStatus, "Connection Status");
                toolTip1.SetToolTip(rbt_queryPLC, "查询");
                toolTip1.SetToolTip(rbt_sendPLC, "给PLC发送");
            }
            else
            {
                toolTip1.SetToolTip(btn_Connect, "Nối");
                toolTip1.SetToolTip(btn_Receive, "nhận dữ liệu");
                toolTip1.SetToolTip(btn_Save, "Lưu dữ liệu");
                toolTip1.SetToolTip(btn_Send, "gửi dữ liệu");
                toolTip1.SetToolTip(pic_NetStatus, "Kết nối");
                toolTip1.SetToolTip(rbt_queryPLC, "查询");
                toolTip1.SetToolTip(rbt_sendPLC, "给PLC发送");
            }
        }

        private void cmb_Net_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TCPClient c = TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim());
                if (c != null && c.IsOpen())
                {
                    pic_NetStatus.BackgroundImage = Properties.Resources.ConOK;
                }
                else
                {
                    pic_NetStatus.BackgroundImage = Properties.Resources.ConNG;
                }
            }
            catch { }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!ParameConfig.Instance.ParameCfgDic["Net"].UpdateGridToFile(dataGridView1))
            {
                MessageBox.Show("Save Fail！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Save Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            btn_Save.BaseColorEnd = Color.Transparent;
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                string str = dataGridView1[0, index].Value.ToString();
                if (str== "PLC")
                {
                    if (Program.modbusTcp_PLC.Connect())
                    {
                        MessageBox.Show(str + "Reconnection Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(str + "Reconnection Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (TCPClientConfig.Instance.ReConnectClient(str))
                    {
                        //MyVariable.ModbusTCPInstance();
                        MessageBox.Show(str + "Reconnection Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(str + "Reconnection Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Reconnection Network Exception！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btn_Connect.BaseColorEnd = Color.Transparent;
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                if (TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).WriteDataStr(txt_Send.Text.Trim(), Encoding.Default))
                {
                    MessageBox.Show("Send Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Send Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Failed To Send Data！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btn_Send.BaseColorEnd = Color.Transparent;
        }

        private void btn_Receive_Click(object sender, EventArgs e)
        {
            try
            {
                string str;
                int timeout = -1;//默认3S
                int length = 0;
                if (cmb_Net.Text.Trim() == _TcpClientModule.GeneralControl.ToString())
                {
                    length = TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).LoopReadData(timeout, out str, Encoding.UTF8);
                }
                else
                {
                    length = TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).LoopReadData(timeout, out str, Encoding.Default);
                }
                if (length > 0)
                {
                    txt_Receive.Clear();
                    txt_Receive.Text = str;
                }
                else
                {
                    txt_Receive.Clear();
                    txt_Receive.Text = "Receive Timeout!";
                }
            }
            catch
            {
                MessageBox.Show("Failed To Accept Data!", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btn_Receive.BaseColorEnd = Color.Transparent;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txt_Receive.Clear();

            if (checkBox1.Checked)
            {
                btn_Receive.Enabled = false;

                //开启线程
                autoReceiveThreadStart = new ThreadStart(autoReceiveInfo); //ThreadStart理解为一个函数指针，指向线程要执行的函数
                autoReceiveThread = new Thread(autoReceiveThreadStart);
                autoReceiveThread.IsBackground = true;
                autoReceiveThread.Start();
            }
            else
            {
                btn_Receive.Enabled = true;

                //关闭线程
                if (autoReceiveThread != null)
                {
                    if (autoReceiveThread.IsAlive)
                    {
                        autoReceiveThread.Abort();
                    }
                }
            }
        }

        private void autoReceiveInfo()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(5);
                    string netname = string.Empty;
                    cmb_Net.Invoke(new Action(() =>
                    {
                        netname = cmb_Net.Text.Trim();
                    }));

                    if (TCPClientConfig.Instance.GetClient(netname).NetCanRead())
                    {
                        string str;
                        int timeout = -1;//默认3S
                        int length = 0;
                        if (cmb_Net.Text.Trim() == _TcpClientModule.GeneralControl.ToString())
                        {
                            length = TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).LoopReadData(timeout, out str, Encoding.UTF8);
                        }
                        else
                        {
                            length = TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).LoopReadData(timeout, out str, Encoding.Default);
                        }
                        if (length > 0)
                        {
                            txt_Receive.Invoke(new Action(() =>
                            {
                                txt_Receive.AppendText(str + "\r\n");
                            }));
                        }
                        else
                        {
                            txt_Receive.Invoke(new Action(() =>
                            {
                                txt_Receive.Text = "Receive Timeout!\r\n";
                            }));
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed To Accept Data!", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbt_connectPLC_Click(object sender, EventArgs e)
        {
            if (Program.modbusTcp_PLC.Connect())
            {
                MessageBox.Show("PLC连接成功！");
            }
            else
            {
                MessageBox.Show("PLC连接失败！");
            }

        }

        private void rbt_sendPLC_Click(object sender, EventArgs e)
        {
            try
            {
                bool b = false;
                if (rbt_string.Checked)
                {
                    b = Program.modbusTcp_PLC.WriteStringToRegister(1, ushort.Parse(txt_sendaddr.Text.Trim()), txt_sendnum.Text, (ModbusLib.DataType)Enum.Parse(typeof(ModbusLib.DataType), cbx_send.Text.Trim()));
                }
                else
                {
                    b = Program.modbusTcp_PLC.WriteSingleRegister(1, ushort.Parse(txt_sendaddr.Text.Trim()), short.Parse(txt_sendnum.Text.Trim()));


                    //  b = Program.modbusTcp_PLC.WriteMultipleReal(1, ushort.Parse(txt_sendaddr.Text.Trim()), new float[] { float.Parse(txt_sendnum.Text.Trim()) },
                    //                                               (ModbusLib.DataType)Enum.Parse(typeof(ModbusLib.DataType), cbx_send.Text.Trim()));
                }
                if (b)
                {
                    MessageBox.Show("发送成功！");
                }
                else
                {
                    MessageBox.Show("发送失败！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入格式错误！" + ex.Message);
            }
        }
        private void rbt_queryPLC_Click(object sender, EventArgs e)
        {
            try
            {
                short[] rec;
                if (rbt_string.Checked)
                {
                    string readstr = string.Empty;
                    if (Program.modbusTcp_PLC.ReadStringFromRegister(1, ushort.Parse(txt_queryaddr.Text.Trim()), 20, "03", out readstr,
                          (ModbusLib.DataType)Enum.Parse(typeof(ModbusLib.DataType), cbx_receive.Text.Trim())))
                    {
                        txt_queryresult.Text = readstr.Trim();
                        MessageBox.Show("接收成功！");
                    }
                    else
                    {
                        txt_queryresult.Text = "";
                        MessageBox.Show("接收失败！");
                    }
                }
                else
                {
                    if (Program.modbusTcp_PLC.ReadHoldingRegisters(1, ushort.Parse(txt_queryaddr.Text.Trim()), 1, out rec))
                    {
                        txt_queryresult.Text = rec[0].ToString();
                        MessageBox.Show("接收成功！");
                    }
                    else
                    {
                        txt_queryresult.Text = "";
                        MessageBox.Show("接收失败！");
                    }




                    //float[] readvalue = null;
                    //if (Program.modbusTcp_PLC.ReadMultipleReal(1, ushort.Parse(txt_queryaddr.Text.Trim()), 1, "03", out readvalue,
                    //    (ModbusLib.DataType)Enum.Parse(typeof(ModbusLib.DataType), cbx_receive.Text.Trim())))
                    //{
                    //    txt_queryresult.Text = readvalue[0].ToString();
                    //    MessageBox.Show("接收成功！");
                    //}
                    //else
                    //{
                    //    txt_queryresult.Text = "";
                    //    MessageBox.Show("接收失败！");
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入格式错误！" + ex.Message);
            }
        }

        private void rbt_string_CheckedChanged(object sender, EventArgs e)
        {
            cbx_send.SelectedIndex = 1;
            cbx_receive.SelectedIndex = 1;
        }

        private void rbt_start_Click(object sender, EventArgs e)
        {
            try
            {
                txt_Send.Clear();
                mStartReportingToControl_Form.sn = txt_gen_sn.Text.Trim();
                string jsonStr = JsonConvert.SerializeObject(mStartReportingToControl_Form);
                txt_Send.Text = jsonStr;
                if (cmb_Net.Text.Trim() != _TcpClientModule.GeneralControl.ToString())
                {
                    MessageBox.Show("网络列表未选择 GeneralControl", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).WriteDataStr(jsonStr))
                {
                    MessageBox.Show("Send Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Send Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Failed To Send Data！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbt_finish_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmb_Net.Text.Trim() != _TcpClientModule.GeneralControl.ToString())
                {
                    MessageBox.Show("网络列表未选择 GeneralControl", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                txt_Send.Clear();
                mCompleteReportingToControl_Form.sn = txt_gen_sn.Text.Trim();
                mCompleteReportingToControl_Form.experimentResult = "OK";
                string jsonStr = JsonConvert.SerializeObject(mCompleteReportingToControl_Form);
                txt_Send.Text = jsonStr;
                if (TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).WriteDataStr(jsonStr))
                {
                    MessageBox.Show("Send Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Send Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Failed To Send Data！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbt_result_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmb_Net.Text.Trim() != _TcpClientModule.GeneralControl.ToString())
                {
                    MessageBox.Show("网络列表未选择 GeneralControl", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                txt_Send.Clear();
                mChipDataReportingToControl_Form.taskId = int.Parse(txt_gen_taskid.Text.Trim());
                mChipDataReportingToControl_Form.chipTotalCount = int.Parse(txt_gen_total.Text.Trim());
                mChipDataReportingToControl_Form.chipMatchCount = int.Parse(txt_gen_match.Text.Trim());
                string jsonStr = JsonConvert.SerializeObject(mChipDataReportingToControl_Form);
                txt_Send.Text = jsonStr;
                if (TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).WriteDataStr(jsonStr))
                {
                    MessageBox.Show("Send Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Send Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Failed To Send Data！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbt_workquery_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmb_Net.Text.Trim() != _TcpClientModule.GeneralControl.ToString())
                {
                    MessageBox.Show("网络列表未选择 GeneralControl", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                txt_Send.Clear();
                string jsonStr = JsonConvert.SerializeObject(mSearchFolloUpTaskToControl_Form);
                txt_Send.Text = jsonStr;
                if (TCPClientConfig.Instance.GetClient(cmb_Net.Text.Trim()).WriteDataStr(jsonStr))
                {
                    MessageBox.Show("Send Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Send Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Failed To Send Data！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
    /// <summary>
    /// 开始时请求总控是否实验 
    /// </summary>
    public class StartReportingToControl_Form
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string requestType = "startReporting";
        /// <summary>
        /// 载具SN
        /// </summary>
        public string sn;
    }
    /// <summary>
    /// 测序结束时上报总控
    /// </summary>
    public class CompleteReportingToControl_Form
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string requestType = "completereporting";
        /// <summary>
        /// 载具SN
        /// </summary>
        public string sn;
        /// <summary>
        /// 实验结果是否成功
        /// </summary>
        public string experimentResult;
    }
    /// <summary>
    /// 文件解析完测序结果上报总控
    /// </summary>
    public class ChipDataReportingToControl_Form
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string requestType = "chipDataReporting";
        /// <summary>
        /// 任务ID
        /// </summary>
        public int taskId;
        /// <summary>
        /// 芯片测序结果数据(总数）
        /// </summary>
        public int chipTotalCount;
        /// <summary>
        /// 芯片测序结果数据(匹配数）
        /// </summary>
        public int chipMatchCount;
    }
    /// <summary>
    /// 查询总控接下来是否有任务
    /// </summary>
    public class SearchFolloUpTaskToControl_Form
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string requestType = "searchFolloUpTask";
    }

}
