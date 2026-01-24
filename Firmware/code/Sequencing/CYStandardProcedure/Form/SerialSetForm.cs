using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class SerialSetForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

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
            ParameConfig.Instance.ParameCfgDic["Com"].UpdateParameterToGrid(dataGridView1);
            /***当窗体大小改变时候也需要重新设置标题语言***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        public SerialSetForm()
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
            if (ParameConfig.Instance.SerialParameDic.Count > 0)
            {
                foreach (KeyValuePair<string, ComParame> va in ParameConfig.Instance.SerialParameDic)
                {
                    cmb_Serial.Items.Add(va.Key);
                }
                cmb_Serial.SelectedIndex = 0;
            }
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += SerialSetForm_LanguageChangeEvent; ;
        }

        private void SerialSetForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);

            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Save, "保存数据");
                toolTip1.SetToolTip(btn_Receive, "接受数据");
                toolTip1.SetToolTip(btn_Open, "打开串口");
                toolTip1.SetToolTip(btn_Send, "发送数据");
                toolTip1.SetToolTip(pic_SerialStatus, "连接状态");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Save, "Save Data");
                toolTip1.SetToolTip(btn_Receive, "Accept Data");
                toolTip1.SetToolTip(btn_Open, "Open Serial");
                toolTip1.SetToolTip(btn_Send, "Send Data");
                toolTip1.SetToolTip(pic_SerialStatus, "Connection Status");
            }
            else
            {
                toolTip1.SetToolTip(btn_Save, "Lưu dữ liệu");
                toolTip1.SetToolTip(btn_Receive, "Chấp nhận dữ liệu");
                toolTip1.SetToolTip(btn_Open, "Mở cổng nối tiếp");
                toolTip1.SetToolTip(btn_Send, "gửi dữ liệu");
                toolTip1.SetToolTip(pic_SerialStatus, "Kết nối");
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!ParameConfig.Instance.ParameCfgDic["Com"].UpdateGridToFile(dataGridView1))
            {
                MessageBox.Show("Save Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Saved Successfully！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            btn_Save.BaseColorEnd = Color.Transparent;
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                string str = dataGridView1[0, index].Value.ToString();
                if (SerialConfig.Instance.ReOpenSerial(str))
                {
                    MyVariable.num = 0;
                    MessageBox.Show(str + "Reopening Succeeded！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(str + "Reopening Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Abnormal Opening Of Serial Port Again！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btn_Open.BaseColorEnd = Color.Transparent;
        }
        string send_wenkongbiao;
        string[] sendtxt;
        private void btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                if (!cbx_wenkongbiao.Checked)
                {
                    if (SerialConfig.Instance.GetSerial(cmb_Serial.Text.Trim()).WriteStr(txt_Send.Text.Trim() + Environment.NewLine))
                    {
                        MessageBox.Show("Send Successfully！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Send Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    sendtxt = txt_Send.Text.Trim().Split('-');
                    if (sendtxt[0] == "read")
                    {
                        send_wenkongbiao = "01 03 00 00 00 02 C4 0B";
                    }
                    else if (sendtxt[0] == "set")
                    {
                        send_wenkongbiao = SetInstrumentTemperature(sendtxt[1]);
                    }
                    if (SerialConfig.Instance.GetSerial(cmb_Serial.Text.Trim()).WriteByte(SerialConfig.Instance.GetSerial(cmb_Serial.Text.Trim()).StrToByte(send_wenkongbiao)))
                    {
                        MessageBox.Show("Send Successfully！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Send Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

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
            Serial sr = SerialConfig.Instance.GetSerial(cmb_Serial.Text.Trim());
            Stopwatch watch = new Stopwatch();
            watch.Restart();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        Thread.Sleep(5);
                        if (watch.ElapsedMilliseconds > 3000)
                        {
                            watch.Stop();
                            MessageBox.Show("Receive Timeout！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (sr.SerialCanRead())
                        {
                            Thread.Sleep((int)sr.DelayTime * 1000);
                            if (!cbx_wenkongbiao.Checked)
                            {
                                string str;
                                sr.ReadByteToStr(out str);
                                this.Invoke(new Action(() =>
                                {
                                    txt_Receive.Clear();
                                    txt_Receive.Text = str;
                                }));
                            }
                            else
                            {
                                byte[] str;
                                sr.ReadByte(out str);
                                this.Invoke(new Action(() =>
                                {
                                    txt_Receive.Clear();
                                    if (str[3].ToString() == "255" && str[5].ToString() == "255")
                                    {
                                        txt_Receive.Text = ((Convert.ToDouble(str[6]) - 256) / 10).ToString();
                                    }
                                    else if (str[3].ToString() == "0" && str[5].ToString() == "0")
                                    {
                                        txt_Receive.Text = (Convert.ToDouble(str[6]) / 10).ToString();
                                    }
                                    else if (str[3].ToString() == "0" && str[5].ToString() != "0")
                                    {
                                        txt_Receive.Text = ((Convert.ToDouble(str[6]) + (Convert.ToDouble(str[5]) * 16 * 16)) / 10).ToString();
                                    }
                                }));
                            }
                            sr.ClearBuffer();
                            MessageBox.Show("Data Received Successfully！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Failed To Receive Data！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void cmb_Serial_SelectedIndexChanged(object sender, EventArgs e)
        {
            Serial sr = SerialConfig.Instance.GetSerial(cmb_Serial.Text.Trim());
            if (sr != null && sr.IsOpen())
            {
                pic_SerialStatus.BackgroundImage = Properties.Resources.ConOK;
            }
            else
            {
                pic_SerialStatus.BackgroundImage = Properties.Resources.ConNG;
            }
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
                string serialname = string.Empty;
                cmb_Serial.Invoke(new Action(() =>
                {
                    serialname = cmb_Serial.Text.Trim();
                }));
                Serial sr = SerialConfig.Instance.GetSerial(serialname);

                while (true)
                {
                    Thread.Sleep(5);
                    if (sr.SerialCanRead())
                    {
                        
                        Thread.Sleep((int)sr.DelayTime * 1000);
                        if (!cbx_wenkongbiao.Checked)
                        {
                            string str;
                            sr.ReadByteToStr(out str);
                            this.Invoke(new Action(() =>
                            {
                                txt_Receive.Text = str + "\r\n";
                            }));

                        }
                        else
                        {
                            byte[] str;
                            sr.ReadByte(out str);
                            this.Invoke(new Action(() =>
                            {
                                txt_Receive.Clear();
                                if (str[3].ToString() == "255" && str[5].ToString() == "255")
                                {
                                    txt_Receive.Text = ((Convert.ToDouble(str[6]) - 256) / 10).ToString();
                                }
                                else if (str[3].ToString() == "0" && str[5].ToString() == "0")
                                {
                                    txt_Receive.Text = (Convert.ToDouble(str[6]) / 10).ToString();
                                }
                                else if (str[3].ToString() == "0" && str[5].ToString() != "0")
                                {
                                    txt_Receive.Text = ((Convert.ToDouble(str[6]) + (Convert.ToDouble(str[5]) * 16 * 16)) / 10).ToString();
                                }
                            }));
                        }
                        sr.ClearBuffer();
                        break;
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed To Receive Data！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        /// <summary>
        /// 设置温控仪温度
        /// </summary>
        /// <param name="input">温度</param>
        /// <returns></returns>
        public string SetInstrumentTemperature(string input)
        {
            try
            {
                string hexValue = (int.Parse(input) * 10).ToString("X4");
                string formattedHex = "01 06 21 03 " + string.Join(" ", Enumerable.Range(0, hexValue.Length / 2).Select(i => hexValue.Substring(i * 2, 2)));

                string[] hexValuesSplit = formattedHex.Split(' ');
                byte[] byteArray = hexValuesSplit.Select(s => Convert.ToByte(s, 16)).ToArray();
                ushort crcValue = CalculateCRC16Modbus(byteArray);
                string formattedHex1 = crcValue.ToString("X4");

                string reversedHex = "";
                for (int i = formattedHex1.Length - 2; i >= 0; i -= 2)
                {
                    reversedHex += formattedHex1.Substring(i, 2) + " ";
                }
                reversedHex = reversedHex.Trim();

                return formattedHex + " " + reversedHex;

            }
            catch (Exception)
            {
                return "0";
            }
        }

        /// <summary>
        /// 计算校验码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ushort CalculateCRC16Modbus(byte[] data)
        {
            try
            {
                ushort polynomial = 0xA001; // CRC-16 MODBUS 多项式
                ushort crc = 0xFFFF; // 初始值为 0xFFFF
                foreach (byte b in data)
                {
                    crc ^= b;
                    for (int i = 0; i < 8; i++)
                    {
                        if ((crc & 1) == 1)
                        {
                            crc = (ushort)((crc >> 1) ^ polynomial);
                        }
                        else
                        {
                            crc >>= 1;
                        }
                    }
                }
                return crc;
            }
            catch (Exception)
            {
                return 0;
            }
        }



    }
}
