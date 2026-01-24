using CYAutoFramework;
using ktCnt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class PipetteGunForm : Form
    {
        string path = Application.StartupPath + @"\ExeFile\" + @"\PipetteGunParam" + ".ini";
        INIFile ini = new INIFile(Application.StartupPath + @"\ExeFile\" + @"\PipetteGunParam" + ".ini");
        INIFile iniparam = new INIFile(Application.StartupPath + @"\FileINI\" + @"\PipetteGunForm" + ".ini");
        List<string> list = new List<string>();
        List<string> keyValues = new List<string>();
        string[] keys;
        string[] values;
        Stopwatch swatch = new Stopwatch();
        public static PipetteGunForm m_pipettegun;

        string com;
        char comindex1;
        char comindex2;

        private static object obj1 = new object();


        public PipetteGunForm()
        {
            InitializeComponent();
            m_pipettegun = this;
        }


        #region 控件窗体自适应
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion

        private void PipetteGunForm_Load(object sender, EventArgs e)
        {
            // 设置 DataGridView 的默认单元格样式
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            // 设置标题行的字体样式
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 14, FontStyle.Bold);
            list = iniHelper.GetAllSectionNames(path);
            keyValues = iniHelper.GetAllKeyValues(list[0], out keys, out values, path);
            foreach (var item in keys)
            {
                dataGridView1.Columns.Add(item, item);
            }
            for (int i = 0; i < list.Count; i++)
            {
                iniHelper.GetAllKeyValues(list[i], out keys, out values, path);
                int index = dataGridView1.Rows.Add();
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    dataGridView1.Rows[index].Cells[j].Value = values[j];
                }
            }
            ReadPipetteGunParam();
            /***子窗体自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        iniHelper.GetAllKeyValues(list[i], out keys, out values, path);
                        ini.Write((i + 1).ToString(), keys[j], dataGridView1.Rows[i].Cells[j].Value.ToString());
                    }
                }
                WritePipetteGunParam();
                MessageBox.Show("保存成功!");
            }

            catch (Exception ex)
            {
                MessageBox.Show("保存失败!" + ex.ToString());
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            lab_backstate.Text = "";
            try
            {
                string backstate = "";
                com = ini.Read<string>("1", "串口号");
                int i = com.Length;
                if (i == 5)
                {
                    comindex1 = com[3];
                    comindex2 = com[4];
                    MyVariable.PipetteGunConnect(2, comindex1, comindex2, out backstate);
                }
                else
                {
                    comindex1 = com[3];
                    comindex2 = '0';
                    MyVariable.PipetteGunConnect(1, comindex1, comindex2, out backstate);
                }
                lab_backstate.Text = backstate;
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败");
            }
        }

        private void rbt_gun_initial_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"1[It{txt_gunspeed1.Text.Trim()}];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_xiye_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"1[Ia{txt_volum1.Text.Trim()},{txt_gunspeed2.Text.Trim()},,];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_paiye_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"1[Da{txt_volum2.Text.Trim()},,{txt_gunspeed3.Text.Trim()},];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_yemiangensui_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"1[Iz{txt_volum3.Text.Trim()},{txt_gunspeed4.Text.Trim()},{txt_mianji.Text.Trim()}];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_paikong_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"1[De500];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_outTip_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"1[Dt500];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_z_initial_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"41[Zz{txt_zspeed1.Text.Trim()}];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_gopos_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"41[Zp{txt_pos.Text.Trim()},{txt_zspeed2.Text.Trim()}];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_getgun_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"41[Zg{txt_zspeed3.Text.Trim()},80];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_yemiantance_Click(object sender, EventArgs e)
        {
            string cmdListStr = $"41[Zp{txt_downpos.Text.Trim()},{txt_zspeed4.Text.Trim()}]1[Ld0,{txt_time.Text.Trim()}];";
            PipetteGunSend(cmdListStr);
        }

        private void rbt_z_stop_Click(object sender, EventArgs e)
        {
            //  string cmdListStr = $"41[Zt];";
            //   PipetteGunSend(cmdListStr);
            ktCntDll.KpcStopTaskExe(123);
        }





        /// <summary>
        /// 移液枪发送指令
        /// </summary>
        /// <param name="sendmsg">发送内容</param>
        private void PipetteGunSend(string sendmsg)
        {
            lock (obj1)
            {
                lab_backstate.Text = "";
                Byte[] cmdListByt = System.Text.Encoding.UTF8.GetBytes(sendmsg);
                KpcState_e state = ktCntDll.KpcAddCmdList(123, cmdListByt);

                lab_backstate.Text = state.ToString();
                timer1.Enabled = true;
                timer1.Interval = 200;
                timer1.Start();
                swatch.Restart();
            }
        }


        public void WritePipetteGunParam()
        {
            try
            {
                iniparam.Write("GunInitial", "speed", txt_gunspeed1.Text);
                iniparam.Write("GunInitial", "gooutgun", txt_gunout.Text);
                iniparam.Write("XiYe", "speed", txt_gunspeed2.Text);
                iniparam.Write("XiYe", "volume", txt_volum1.Text);
                iniparam.Write("PaiYe", "speed", txt_gunspeed3.Text);
                iniparam.Write("PaiYe", "volume", txt_volum2.Text);
                iniparam.Write("YeMianGenSui", "speed", txt_gunspeed4.Text);
                iniparam.Write("YeMianGenSui", "volume", txt_volum3.Text);
                iniparam.Write("YeMianGenSui", "surfacearea", txt_mianji.Text);

                iniparam.Write("ZInitial", "speed", txt_zspeed1.Text);
                iniparam.Write("GoPos", "speed", txt_zspeed2.Text);
                iniparam.Write("GoPos", "pos", txt_pos.Text);
                iniparam.Write("DownGetTip", "speed", txt_zspeed3.Text);
                iniparam.Write("YeMianTanCe", "speed", txt_zspeed4.Text);
                iniparam.Write("YeMianTanCe", "downpos", txt_downpos.Text);
                iniparam.Write("YeMianTanCe", "time", txt_time.Text);
            }
            catch (Exception)
            {

            }
        }
        private void ReadPipetteGunParam()
        {
            try
            {
                txt_gunspeed1.Text = iniparam.Read<string>("GunInitial", "speed");
                txt_gunout.Text = iniparam.Read<string>("GunInitial", "gooutgun");
                txt_gunspeed2.Text = iniparam.Read<string>("XiYe", "speed");
                txt_volum1.Text = iniparam.Read<string>("XiYe", "volume");
                txt_gunspeed3.Text = iniparam.Read<string>("PaiYe", "speed");
                txt_volum2.Text = iniparam.Read<string>("PaiYe", "volume");
                txt_gunspeed4.Text = iniparam.Read<string>("YeMianGenSui", "speed");
                txt_volum3.Text = iniparam.Read<string>("YeMianGenSui", "volume");
                txt_mianji.Text = iniparam.Read<string>("YeMianGenSui", "surfacearea");

                txt_zspeed1.Text = iniparam.Read<string>("ZInitial", "speed");
                txt_zspeed2.Text = iniparam.Read<string>("GoPos", "speed");
                txt_pos.Text = iniparam.Read<string>("GoPos", "pos");
                txt_zspeed3.Text = iniparam.Read<string>("DownGetTip", "speed");
                txt_zspeed4.Text = iniparam.Read<string>("YeMianTanCe", "speed");
                txt_downpos.Text = iniparam.Read<string>("YeMianTanCe", "downpos");
                txt_time.Text = iniparam.Read<string>("YeMianTanCe", "time");
            }
            catch (Exception)
            {

            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (swatch.ElapsedMilliseconds / 1000 < 20)
            {
                KpcCntDeviceState_e gunstate = ktCntDll.KpcGetCntDeviceState(1);
                KpcCntDeviceState_e Zstate = ktCntDll.KpcGetCntDeviceState(41);
                KpcCntTaakState_e taskstate = ktCntDll.KpcGetCntTaskState(123);
                lab_gunstate.Text = gunstate.ToString();
                lab_Zstate.Text = Zstate.ToString();
                lab_runstate.Text = taskstate.ToString();
            }
            else
            {
                swatch.Reset();
                timer1.Enabled = false;
                timer1.Stop();
            }
        }

        bool b1 = false;
        int step = 0;
        string cmdListStr;
        private void roundButton1_Click(object sender, EventArgs e)
        {
            if (b1 == false)
            {
                b1 = true;
                step = 0;
                Thread th = new Thread(Test);
                th.IsBackground = true;
                th.Start();
            }
            else
            {
                b1 = false;
            }
        }
        Int32[] ackData = new Int32[4];
        byte[] ackCont = new byte[1];
        int distance = 0;

        private void Test()
        {
            try
            {
                while (b1)
                {
                    Thread.Sleep(50);
                    switch (step)
                    {
                        case 0:
                            distance = Convert.ToInt32(txt_distance.Text.Trim());
                            cmdListStr = $"41[Zp{distance},30000];";
                            Byte[] cmdListByt = System.Text.Encoding.UTF8.GetBytes(cmdListStr);
                            KpcState_e state = ktCntDll.KpcAddCmdList(123, cmdListByt);
                            step = 10;
                            break;
                        case 10:
                            KpcCntDeviceState_e Zstate = ktCntDll.KpcGetCntDeviceState(41);
                            if (Zstate == KpcCntDeviceState_e.KPC_CNT_DEVICE_EXE_FINISH)
                            {
                                step = 20;
                            }
                            break;
                        case 20:
                            cmdListStr = "41[Rr101];";
                            Byte[] cmdListByt2 = System.Text.Encoding.UTF8.GetBytes(cmdListStr);
                            KpcState_e state2 = ktCntDll.KpcAddCmdList(123, cmdListByt2);
                            step = 25;
                            break;
                        case 25:
                            KpcCntTaakState_e states = ktCntDll.KpcGetCntTaskState(123);
                            if (states == KpcCntTaakState_e.KPC_TASK_EXE_FINISH)
                            {
                                step = 30;
                            }
                            break;
                        case 30:
                            //显示获取到的寄存器
                            string readRegStr = null;
                            ackCont[0] = 1;
                            if (ktCntDll.KpcGetCntDeviceAckData(41, ackData, ackCont) == KpcCntDeviceState_e.KPC_CNT_DEVICE_EXE_FINISH
                                && ackCont[0] == 1)
                            {
                                readRegStr += ackData[0].ToString();
                                PipetteGunForm.m_pipettegun.Invoke(new Action(() =>
                                {
                                    textBox1.Text = readRegStr;
                                }));
                                SavetestData("setPos", readRegStr);
                                step = 40;
                            }
                            break;
                        case 40:
                            cmdListStr = $"41[Zp0,30000];";
                            Byte[] cmdListByt4 = System.Text.Encoding.UTF8.GetBytes(cmdListStr);
                            KpcState_e state4 = ktCntDll.KpcAddCmdList(123, cmdListByt4);
                            step = 50;
                            break;
                        case 50:
                            KpcCntDeviceState_e Zstate1 = ktCntDll.KpcGetCntDeviceState(41);
                            if (Zstate1 == KpcCntDeviceState_e.KPC_CNT_DEVICE_EXE_FINISH)
                            {
                                step = 60;
                            }
                            break;
                        case 60:
                            cmdListStr = "41[Rr101];";
                            Byte[] cmdListByt6 = System.Text.Encoding.UTF8.GetBytes(cmdListStr);
                            KpcState_e state6 = ktCntDll.KpcAddCmdList(123, cmdListByt6);
                            step = 65;
                            break;
                        case 65:
                            KpcCntTaakState_e states2 = ktCntDll.KpcGetCntTaskState(123);
                            if (states2 == KpcCntTaakState_e.KPC_TASK_EXE_FINISH)
                            {
                                step = 70;
                            }
                            break;
                        case 70:
                            //显示获取到的寄存器
                            string readRegStr1 = null;
                            ackCont[0] = 1;
                            if (ktCntDll.KpcGetCntDeviceAckData(41, ackData, ackCont) == KpcCntDeviceState_e.KPC_CNT_DEVICE_EXE_FINISH
                                && ackCont[0] == 1)
                            {
                                readRegStr1 += ackData[0].ToString();
                                PipetteGunForm.m_pipettegun.Invoke(new Action(() =>
                                {
                                    textBox1.Text = readRegStr1;
                                }));
                                SavetestData("Origin", readRegStr1);
                                step = 0;
                            }
                            break;
                    }
                }
                MessageBox.Show("结束运行");
            }
            catch (Exception ex)
            {
                MessageBox.Show("运行出错:" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void SavetestData(string name, string pos)
        {
            try
            {
                if (!Directory.Exists(@"E:\SWLog\TESTData"))
                {
                    Directory.CreateDirectory(@"E:\SWLog\TESTData");
                }
                string strDataPath = @"E:\SWLog\TESTData\" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                if (!File.Exists(strDataPath))
                {
                    using (StreamWriter sww = new StreamWriter(strDataPath, true))
                    {
                        sww.WriteLine("时间,点位名称,当前位置");
                    }
                }
                using (StreamWriter sww = new StreamWriter(strDataPath, true))
                {
                    sww.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "," + name + "," + pos);
                }
            }
            catch (Exception d)
            {
                LogConfig.Instance.ShowMessageToList("Run", d.Message, MsgType.Error, Color.Red);
            }
        }
    }
}
