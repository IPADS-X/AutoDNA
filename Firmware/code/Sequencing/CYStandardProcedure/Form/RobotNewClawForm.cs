using CYAutoFramework;
using ModbusLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class RobotNewClawForm : Form
    {
        public static RobotNewClawForm m_RobotNewClawForm;
        public RobotNewClawForm()
        {
            InitializeComponent();
            m_RobotNewClawForm = this;
        }
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        private void RobotNewClawForm_Load(object sender, EventArgs e)
        {
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            #region 界面信息显示
            txt_com.Text = SerializeClass.m_RobClawParam.robClaw_Com;
            txt_baudrate.Text = SerializeClass.m_RobClawParam.robClaw_Baudrate.ToString();
            txt_databits.Text = SerializeClass.m_RobClawParam.robClaw_Databits.ToString();
            txt_parity.Text = SerializeClass.m_RobClawParam.robClaw_Parity;
            txt_stopbits.Text = SerializeClass.m_RobClawParam.robClaw_Stopbits;
            txt_speed1_1.Text = MyVariable.speed1_fuwei.ToString();
            txt_acc1_1.Text = MyVariable.acc1_fuwei.ToString();
            txt_force1_1.Text = MyVariable.force1_fuwei.ToString();
            txt_pos1_1.Text = MyVariable.pos1_fuwei.ToString();
            txt_speed1_2.Text = MyVariable.speed1_daowei.ToString();
            txt_acc1_2.Text = MyVariable.acc1_daowei.ToString();
            txt_force1_2.Text = MyVariable.force1_daowei.ToString();
            txt_pos1_2.Text = MyVariable.pos1_daowei.ToString();
            txt_speed2_1.Text = MyVariable.speed2_fuwei.ToString();
            txt_acc2_1.Text = MyVariable.acc2_fuwei.ToString();
            txt_force2_1.Text = MyVariable.force2_fuwei.ToString();
            txt_pos2_1.Text = MyVariable.pos2_fuwei.ToString();
            txt_speed2_2.Text = MyVariable.speed2_daowei.ToString();
            txt_acc2_2.Text = MyVariable.acc2_daowei.ToString();
            txt_force2_2.Text = MyVariable.force2_daowei.ToString();
            txt_pos2_2.Text = MyVariable.pos2_daowei.ToString();
            #endregion
            Thread th = new Thread(Method);
            th.IsBackground = true;
            th.Start();
        }

        private void rb_Open_Click(object sender, EventArgs e)
        {
            if (!SerializeClass.m_ModbusRtuRob.Connect())
            {
                MessageBox.Show("连接失败！");
                return;
            }
            else
            {
                MessageBox.Show("连接成功！");
            }
        }

        private void rb_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SerializeClass.m_RobClawParam.robClaw_Com = txt_com.Text.Trim();
                SerializeClass.m_RobClawParam.robClaw_Baudrate = Convert.ToInt32(txt_baudrate.Text.Trim());
                SerializeClass.m_RobClawParam.robClaw_Databits = Convert.ToInt32(txt_databits.Text.Trim());
                SerializeClass.m_RobClawParam.robClaw_Parity = txt_parity.Text.Trim();
                SerializeClass.m_RobClawParam.robClaw_Stopbits = txt_stopbits.Text.Trim();
                SerializeClass.WriteRobClawParame();
                SerializeClass.m_ModbusRtuRob = new ModbusRtu(SerializeClass.m_RobClawParam.robClaw_Com, SerializeClass.m_RobClawParam.robClaw_Baudrate,
                    SerializeClass.m_RobClawParam.robClaw_Databits, SerializeClass.m_RobClawParam.robClaw_Parity, SerializeClass.m_RobClawParam.robClaw_Stopbits);
                MessageBox.Show("保存成功！");
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！");
            }
        }

        private void rb_Close_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.DisConnect();
            MessageBox.Show("已断开连接！");
        }

        private void btn_Svo1_Click(object sender, EventArgs e)
        {
            if (btn_Svo1.Tag.ToString() == "失使能")
            {
                if (SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 1, true))
                {
                    btn_Svo1.Tag = "上使能";
                    btn_Svo1.BackgroundImage = Properties.Resources.Svo;
                }
            }
            else if (btn_Svo1.Tag.ToString() == "上使能")
            {
                if (SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 1, false))
                {
                    btn_Svo1.Tag = "失使能";
                    btn_Svo1.BackgroundImage = Properties.Resources.NoSvo;
                }
            }
        }
        private void btn_Svo2_Click(object sender, EventArgs e)
        {
            if (btn_Svo2.Tag.ToString() == "失使能")
            {
                if (SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 1, true))
                {
                    btn_Svo2.Tag = "上使能";
                    btn_Svo2.BackgroundImage = Properties.Resources.Svo;
                }
            }
            else if (btn_Svo2.Tag.ToString() == "上使能")
            {
                if (SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 1, false))
                {
                    btn_Svo2.Tag = "失使能";
                    btn_Svo2.BackgroundImage = Properties.Resources.NoSvo;
                }
            }
        }
        bool b1 = false;
        private void btn_Home1_Click(object sender, EventArgs e)
        {
            //SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 17, false);
            //SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 17, true);
            //bool[] coils = new bool[1] { false };
            //Task.Run(() =>
            //{
            //    Thread.Sleep(200);
            //    do
            //    {
            //        Thread.Sleep(10);
            //        SerializeClass.m_ModbusRtuRob.ReadInputs(1, 1015, 1, out coils);
            //    }
            //    while (!coils[0]);
            //    MessageBox.Show("回零完成！");
            //});
            if (b1)
            {
                return;
            }
            Task.Run(() => 
            {
                b1 = true;
                if (WaitRobClawHome(1))
                {
                    b1 = false;
                    MessageBox.Show("回零完成！");
                }
                else
                {
                    b1 = false;
                    MessageBox.Show("回零失败！");
                }
            });
        }
        bool b2 = false;
        private void btn_Home2_Click(object sender, EventArgs e)
        {
            //SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 17, false);
            //SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 17, true);
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2284, 0);//位置地址清零

            //bool[] coils = new bool[1] { false };
            //Task.Run(() =>
            //{
            //    Thread.Sleep(200);
            //    do
            //    {
            //        Thread.Sleep(10);
            //        SerializeClass.m_ModbusRtuRob.ReadInputs(2, 1015, 1, out coils);
            //    }
            //    while (!coils[0]);
            //    MessageBox.Show("回零完成！");
            //});
            if (b2)
            {
                return;
            }
            Task.Run(() => 
            {
                b2 = true ;
                if (WaitRobClawHome(2))
                {
                    b2 = false;
                    MessageBox.Show("回零完成！");
                }
                else
                {
                    b2 = false;
                    MessageBox.Show("回零失败！");
                }
            });
        }
        private void btn_ResetError1_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 0, false);
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 0, true);
        }

        private void btn_ResetError2_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 0, false);
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 0, true);
        }

        private void btn_StopMove1_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 3, false);
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 3, true);
        }

        private void btn_StopMove2_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 3, false);
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 3, true);
        }

        private void btn_MoveN1_MouseDown(object sender, MouseEventArgs e)
        {
            float Position = 0;
            float real;
            SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 0, "04", out real);
            Position = real - 1000;
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2290, 1);//设置力矩
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2288, (float)nud_speed1.Value);//加速度
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2286, (float)nud_Acc1.Value);//速度
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2284, Position);//位置
        }
        private void btn_MoveN1_MouseUp(object sender, MouseEventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 3, false);
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(1, 3, true);
        }

        private void btn_MoveP1_MouseDown(object sender, MouseEventArgs e)
        {
            float Position = 0;
            float real;
            SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 0, "04", out real);
            Position = real + 1000;
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2290, 1);//设置力矩
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2288, (float)nud_speed1.Value);//加速度
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2286, (float)nud_Acc1.Value);//速度
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2284, Position);//位置
        }

        private void btn_MoveN2_MouseDown(object sender, MouseEventArgs e)
        {
            float Position = 0;
            float real;
            SerializeClass.m_ModbusRtuRob.ReadSingleReal(2, 0, "04", out real);
            Position = real - 1000;
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2290, 1);//设置力矩
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2288, (float)nud_speed2.Value);//加速度
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2286, (float)nud_Acc2.Value);//速度
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2284, Position);//位置
        }

        private void btn_MoveN2_MouseUp(object sender, MouseEventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 3, false);
            SerializeClass.m_ModbusRtuRob.WriteSingleCoil(2, 3, true);
        }

        private void btn_MoveP2_MouseDown(object sender, MouseEventArgs e)
        {
            float Position = 0;
            float real;
            SerializeClass.m_ModbusRtuRob.ReadSingleReal(2, 0, "04", out real);
            Position = real + 1000;
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2290, 1);//设置力矩
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2288, (float)nud_speed2.Value);//加速度
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2286, (float)nud_Acc2.Value);//速度
            SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2284, Position);//位置
        }

        float real;
        bool[] Err = new bool[4];

        /// <summary>
        /// 轴信息实时刷新
        /// </summary>
        public void Method()
        {
            while (true)
            {
                Thread.Sleep(200);
                SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 0, "04", out real);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
               {
                   tx_FeedbackPosition.Text = real.ToString("F3");
               }));
                SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 2, "04", out real);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
               {
                   tx_FeedbackVelocity.Text = real.ToString("F3");
               }));
                SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 16, "04", out real);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
               {
                   tx_ForceSensor.Text = $"{Math.Round(real, 3)} %";
               }));
                SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 2154, "03", out real);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
               {
                   tx_Torque.Text = $"{Math.Round(real, 3)} N";
               }));

                SerializeClass.m_ModbusRtuRob.ReadSingleReal(2, 0, "04", out real);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
                {
                    tx_FeedbackPosition2.Text = real.ToString("F3");
                }));
                SerializeClass.m_ModbusRtuRob.ReadSingleReal(2, 2, "04", out real);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
                {
                    tx_FeedbackVelocity2.Text = real.ToString("F3");
                }));
                SerializeClass.m_ModbusRtuRob.ReadSingleReal(2, 16, "04", out real);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
                {
                    tx_ForceSensor2.Text = $"{Math.Round(real, 3)} %";
                }));
                SerializeClass.m_ModbusRtuRob.ReadSingleReal(2, 2154, "03", out real);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
                {
                    tx_Torque2.Text = $"{Math.Round(real, 3)} N";
                }));
                SerializeClass.m_ModbusRtuRob.ReadInputs(1, 0, 4, out Err);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
                {
                    lb_Err.BackColor = Err[0] || Err[1] || Err[2] || Err[3] ? Color.Red : Color.Green;
                }));
                SerializeClass.m_ModbusRtuRob.ReadInputs(2, 0, 4, out Err);

                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
                {
                    lb_Err2.BackColor = Err[0] || Err[1] || Err[2] || Err[3] ? Color.Red : Color.Green;
                }));
                SerializeClass.m_ModbusRtuRob.ReadCoils(1, 1, 1, out Err);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
                {
                    if (Err[0])
                    {
                        btn_Svo1.Tag = "上使能";
                        btn_Svo1.BackgroundImage = Properties.Resources.Svo;
                    }
                    else
                    {
                        btn_Svo1.Tag = "失使能";
                        btn_Svo1.BackgroundImage = Properties.Resources.NoSvo;
                    }
                }));
                SerializeClass.m_ModbusRtuRob.ReadCoils(2, 1, 1, out Err);
                RobotNewClawForm.m_RobotNewClawForm.Invoke(new Action(() =>
                {
                    if (Err[0])
                    {
                        btn_Svo2.Tag = "上使能";
                        btn_Svo2.BackgroundImage = Properties.Resources.Svo;
                    }
                    else
                    {
                        btn_Svo2.Tag = "失使能";
                        btn_Svo2.BackgroundImage = Properties.Resources.NoSvo;
                    }
                }));
            }
        }
        float curPos;
        private void rbt_getPos1_1_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 0, "04", out curPos);
            txt_pos1_1.Text = curPos.ToString();
        }

        private void rbt_getPos1_2_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.ReadSingleReal(1, 0, "04", out curPos);
            txt_pos1_2.Text = curPos.ToString();
        }

        private void rbt_getPos2_1_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.ReadSingleReal(2, 0, "04", out curPos);
            txt_pos2_1.Text = curPos.ToString();
        }

        private void rbt_getPos2_2_Click(object sender, EventArgs e)
        {
            SerializeClass.m_ModbusRtuRob.ReadSingleReal(2, 0, "04", out curPos);
            txt_pos2_2.Text = curPos.ToString();
        }

        INIFile ini = new INIFile(Application.StartupPath + "\\ExeFile\\RobotClaws\\RobotClawPos.ini");
        private void rbt_savePos1_1_Click(object sender, EventArgs e)
        {
            try
            {
                MyVariable.speed1_fuwei = float.Parse(txt_speed1_1.Text.Trim());
                MyVariable.acc1_fuwei = float.Parse(txt_acc1_1.Text.Trim());
                MyVariable.force1_fuwei = float.Parse(txt_force1_1.Text.Trim());
                MyVariable.pos1_fuwei = float.Parse(txt_pos1_1.Text.Trim());
                ini.Write("Claw1_1", "speed", MyVariable.speed1_fuwei);
                ini.Write("Claw1_1", "acc", MyVariable.acc1_fuwei);
                ini.Write("Claw1_1", "force", MyVariable.force1_fuwei);
                ini.Write("Claw1_1", "pos", MyVariable.pos1_fuwei);
                MessageBox.Show("保存成功！");
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！");
            }
        }

        private void rbt_savePos1_2_Click(object sender, EventArgs e)
        {
            try
            {
                MyVariable.speed1_daowei = float.Parse(txt_speed1_2.Text.Trim());
                MyVariable.acc1_daowei = float.Parse(txt_acc1_2.Text.Trim());
                MyVariable.force1_daowei = float.Parse(txt_force1_2.Text.Trim());
                MyVariable.pos1_daowei = float.Parse(txt_pos1_2.Text.Trim());
                ini.Write("Claw1_2", "speed", MyVariable.speed1_daowei);
                ini.Write("Claw1_2", "acc", MyVariable.acc1_daowei);
                ini.Write("Claw1_2", "force", MyVariable.force1_daowei);
                ini.Write("Claw1_2", "pos", MyVariable.pos1_daowei);
                MessageBox.Show("保存成功！");
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！");
            }
        }

        private void rbt_savePos2_1_Click(object sender, EventArgs e)
        {
            try
            {
                MyVariable.speed2_fuwei = float.Parse(txt_speed2_1.Text.Trim());
                MyVariable.acc2_fuwei = float.Parse(txt_acc2_1.Text.Trim());
                MyVariable.force2_fuwei = float.Parse(txt_force2_1.Text.Trim());
                MyVariable.pos2_fuwei = float.Parse(txt_pos2_1.Text.Trim());
                ini.Write("Claw2_1", "speed", MyVariable.speed2_fuwei);
                ini.Write("Claw2_1", "acc", MyVariable.acc2_fuwei);
                ini.Write("Claw2_1", "force", MyVariable.force2_fuwei);
                ini.Write("Claw2_1", "pos", MyVariable.pos2_fuwei);
                MessageBox.Show("保存成功！");
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！");
            }
        }

        private void rbt_savePos2_2_Click(object sender, EventArgs e)
        {
            try
            {
                MyVariable.speed2_daowei = float.Parse(txt_speed2_2.Text.Trim());
                MyVariable.acc2_daowei = float.Parse(txt_acc2_2.Text.Trim());
                MyVariable.force2_daowei = float.Parse(txt_force2_2.Text.Trim());
                MyVariable.pos2_daowei = float.Parse(txt_pos2_2.Text.Trim());
                ini.Write("Claw2_2", "speed", MyVariable.speed2_daowei);
                ini.Write("Claw2_2", "acc", MyVariable.acc2_daowei);
                ini.Write("Claw2_2", "force", MyVariable.force2_daowei);
                ini.Write("Claw2_2", "pos", MyVariable.pos2_daowei);
                MessageBox.Show("保存成功！");
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！");
            }

        }
        private void rbt_runPos1_1_Click(object sender, EventArgs e)
        {
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2290, MyVariable.force1_fuwei);//设置力矩
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2288, MyVariable.acc1_fuwei);//加速度
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2286, MyVariable.speed1_fuwei);//速度
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2284, MyVariable.pos1_fuwei);//位置
            if (b1)
            {
                return;
            }
            Task.Run(() =>
            {
                b1 = true;
                if (WaitRobotClawRun(1, MyVariable.force1_fuwei, MyVariable.speed1_fuwei, MyVariable.acc1_fuwei, MyVariable.pos1_fuwei, 999))
                {
                    b1 = false;
                    MessageBox.Show("运动完成");
                }
                else
                {
                    b1 = false;
                    MessageBox.Show("运动失败");
                }
            });
        }

        private void rbt_runPos1_2_Click(object sender, EventArgs e)
        {
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2290, MyVariable.force1_daowei);//设置力矩
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2288, MyVariable.acc1_daowei);//加速度
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2286, MyVariable.speed1_daowei);//速度
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(1, 2284, MyVariable.pos1_daowei);//位置
            if (b1)
            {
                return;
            }
            Task.Run(() => 
            {
                b1 = true;
                if (WaitRobotClawRun(1, MyVariable.force1_daowei, MyVariable.speed1_daowei, MyVariable.acc1_daowei, MyVariable.pos1_daowei, MyVariable.force1_daowei))
                {
                    b1 = false;
                    MessageBox.Show("运动完成");
                }
                else
                {
                    b1 = false;
                    MessageBox.Show("运动失败");
                }
            });
        }

        private void rbt_runPos2_1_Click(object sender, EventArgs e)
        {
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2290, MyVariable.force2_fuwei);//设置力矩
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2288, MyVariable.acc2_fuwei);//加速度
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2286, MyVariable.speed2_fuwei);//速度
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2284, MyVariable.pos2_fuwei);//位置
            if (b2)
            {
                return;
            }
            Task.Run(() =>
            {
                b2 = true;
                if (WaitRobotClawRun(2, MyVariable.force2_fuwei, MyVariable.speed2_fuwei, MyVariable.acc2_fuwei, MyVariable.pos2_fuwei, 999))
                {
                    b2 = false;
                    MessageBox.Show("运动完成");
                }
                else
                {
                    b2 = false;
                    MessageBox.Show("运动失败");
                }
            });
        }

        private void rbt_runPos2_2_Click(object sender, EventArgs e)
        {
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2290, MyVariable.force2_daowei);//设置力矩
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2288, MyVariable.acc2_daowei);//加速度
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2286, MyVariable.speed2_daowei);//速度
            //SerializeClass.m_ModbusRtuRob.WriteSingleReal(2, 2284, MyVariable.pos2_daowei);//位置
            if (b2)
            {
                return;
            }
            Task.Run(() => 
            {
                b2 = true;
                if (WaitRobotClawRun(2, MyVariable.force2_daowei, MyVariable.speed2_daowei, MyVariable.acc2_daowei, MyVariable.pos2_daowei, 999))
                {
                    b2 = false;
                    MessageBox.Show("运动完成");
                }
                else
                {
                    b2 = false;
                    MessageBox.Show("运动失败");
                }
            });
        }

        Stopwatch sw_home = new Stopwatch();
        /// <summary>
        /// 机器人电动夹爪回零
        /// </summary>
        /// <param name="id">轴ID</param>
        /// <returns></returns>
        public bool WaitRobClawHome(byte id)
        {
            try
            {
                SerializeClass.m_ModbusRtuRob.WriteSingleCoil(id, 17, false);
                SerializeClass.m_ModbusRtuRob.WriteSingleCoil(id, 17, true);
                SerializeClass.m_ModbusRtuRob.WriteSingleReal(id, 2284, 0);//位置地址清零
                sw_home.Restart();
                bool[] coils = new bool[1] { false };
                while (true)
                {
                    Thread.Sleep(10);
                    SerializeClass.m_ModbusRtuRob.ReadInputs(id, 1015, 1, out coils);
                    if (coils[0])
                    {
                        sw_home.Stop();
                        return true;
                    }
                    else if (sw_home.ElapsedMilliseconds / 1000 >= 15)
                    {
                        sw_home.Stop();
                        return false;
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        Stopwatch sw = new Stopwatch();
        float curpos_run;
        float curN_run;
        /// <summary>
        /// 等待机器人夹爪运动
        /// </summary>
        /// <param name="id">轴ID</param>
        /// <param name="force">力矩(推压运动设置0-1,绝对运动设置1)</param>
        /// <param name="speed">速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="pos">目标位置</param>
        /// <param name="setN">判断力矩(需要加持物体设置具体力,默认999)</param>
        /// <returns></returns>
        public bool WaitRobotClawRun(byte id, float force, float speed, float acc, float pos, float setN)
        {
            try
            {
                if (!SerializeClass.m_ModbusRtuRob.WriteSingleReal(id, 2290, force))
                {
                    return false;
                }
                if (!SerializeClass.m_ModbusRtuRob.WriteSingleReal(id, 2288, acc))
                {
                    return false;
                }
                if (!SerializeClass.m_ModbusRtuRob.WriteSingleReal(id, 2286, speed))
                {
                    return false;
                }
                if (!SerializeClass.m_ModbusRtuRob.WriteSingleReal(id, 2284, pos))
                {
                    return false;
                }
                sw.Restart();
                while (true)
                {
                    Thread.Sleep(10);
                    SerializeClass.m_ModbusRtuRob.ReadSingleReal(id, 0, "04", out curpos_run);//返回实时位置
                    SerializeClass.m_ModbusRtuRob.ReadSingleReal(id, 2154, "03", out curN_run);//返回力矩
                    if ((Math.Abs(curpos_run - pos) <= 1) || (Math.Abs(curN_run - setN) <= 0.15))
                    {
                        sw.Stop();
                        Thread.Sleep(500);//到位延迟
                        return true;
                    }
                    else if (sw.ElapsedMilliseconds / 1000 >= 15)
                    {
                        sw.Stop();
                        return false;
                    }
                }
                //Task.Run(() =>
                //{
                //});
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
