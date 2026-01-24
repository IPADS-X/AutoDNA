using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class MainForm_Msg : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        public MainForm_Msg()
        {
            InitializeComponent();
        }
        #region 窗体控件自适应代码      
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion

        private void MainForm_Msg_Load(object sender, EventArgs e)
        {
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            timer1.Enabled = true;
            timer1.Interval = 200;
            timer1.Start();
            txt_snMsg.Text = MyVariable.SN_CarryStation;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lab_Feed_state.Text = SerializeClass.mMemory.FeedingStation_state.ToString();
            lab_Carry_state.Text = SerializeClass.mMemory.CarryStation_state.ToString();
            lab_Sequence_state.Text = SerializeClass.mMemory.SequencingStation_state.ToString();
            lab_Robot_state.Text = SerializeClass.mMemory.RobotStation_state.ToString();
            lab_data_state.Text = SerializeClass.mMemory.DataProcessingStation_state.ToString();
            lab_claw.Text = SerializeClass.mMemory.clamping_jaw_technology.ToString();
            lab_clawrobot.Text = SerializeClass.mMemory.robotclaw_technology.ToString();
            lab_gun.Text = SerializeClass.mMemory.pipette_gun_technology.ToString();
            lab_huanliao.Text = SerializeClass.mMemory.area.ToString();
            lab_buliao.Text = SerializeClass.mMemory.area_noout.ToString();
            lab_carryRun.Text = SerializeClass.mMemory.carrystation_working.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_DNA = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_zongkong = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_TIP1 = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_TIP3 = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_TIP4 = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_DiWen = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_LiXinGuan = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_SequenceFinish = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sign_FuYuFinish = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_inputSN_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.SN_CarryStation = txt_snMsg.Text.Trim();
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            {
                MyVariable.area_QiangTou1.num_Remain = 80;

                MyVariable.area_QiangTou2.num_Remain = 80;
                MyVariable.consumables_Empty[0] = false;

                MyVariable.area_QiangTou3.num_Remain = 80;
                MyVariable.consumables_Empty[1] = false;

                MyVariable.area_QiangTou4.num_Remain = 80;
                MyVariable.consumables_Empty[2] = false;

                MyVariable.area_DiWen_FCT.num_Remain = MyVariable.FCT_MAX;
                MyVariable.consumables_Empty[3] = false;

                MyVariable.area_DiWen_FCF.num_Remain = MyVariable.FCF_MAX;

                MyVariable.area_DiWen_SB.num_Remain = MyVariable.SB_MAX;

                MyVariable.area_DiWen_LIB.num_Remain = MyVariable.LIB_MAX;

                MyVariable.area_DiWen_DIL.num_Remain = MyVariable.DIL_MAX;

                MyVariable.area_DiWen_WMX.num_Remain = MyVariable.WMX_MAX;

                MyVariable.area_DiWen_S.num_Remain = MyVariable.S_MAX;

                MyVariable.area_LiXinGuan.num_Remain = 18;
                MyVariable.consumables_Empty[4] = false;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备不在停止状态中,写入失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            {
                MyVariable.area_QiangTou1.num_Remain = 0;

                MyVariable.area_QiangTou2.num_Remain = 0;
                MyVariable.consumables_Empty[0] = true;

                MyVariable.area_QiangTou3.num_Remain = 0;
                MyVariable.consumables_Empty[1] = true;

                MyVariable.area_QiangTou4.num_Remain = 0;
                MyVariable.consumables_Empty[2] = true;

                MyVariable.area_DiWen_FCT.num_Remain = 0;
                MyVariable.consumables_Empty[3] = true;

                MyVariable.area_DiWen_FCF.num_Remain = 0;

                MyVariable.area_DiWen_SB.num_Remain = 0;

                MyVariable.area_DiWen_LIB.num_Remain = 0;

                MyVariable.area_DiWen_DIL.num_Remain = 0;

                MyVariable.area_DiWen_WMX.num_Remain = 0;

                MyVariable.area_DiWen_S.num_Remain = 0;

                MyVariable.area_LiXinGuan.num_Remain = 0;
                MyVariable.consumables_Empty[4] = true;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备不在停止状态中,写入失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Pause)
            {
                MyVariable.sequencingNeedData = 0;
                MessageBox.Show("写入成功！");
            }
            else
            {
                MessageBox.Show("当前设备状态无法写入,请暂停后写入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
