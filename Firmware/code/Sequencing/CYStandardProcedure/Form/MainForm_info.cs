using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CYCustomControl;
using CYAutoFramework;
using System.Windows.Forms;
using System.IO;

namespace CYStandardProcedure
{
    public partial class MainForm_info : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        private Label[] labels_QiangTou1 = new Label[80];
        private Label[] labels_QiangTou2 = new Label[80];
        private Label[] labels_QiangTou3 = new Label[80];
        private Label[] labels_QiangTou4 = new Label[80];
        private Label[] labels_LiXinGuan = new Label[18];
        private Label[] labels_ALL = new Label[338];



        #region 窗体控件自适应代码      
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion

        public MainForm_info()
        {
            InitializeComponent();
            InitializeLabels();
        }
        private void InitializeLabels()
        {
            for (int i = 0; i < labels_ALL.Length; i++)
            {
                labels_ALL[i] = (Label)this.Controls.Find("label" + (i + 1), true)[0];
            }
            for (int i = 0; i < labels_QiangTou1.Length; i++)
            {
                labels_QiangTou1[i] = (Label)this.Controls.Find("label" + (i + 1), true)[0];
            }
            for (int i = 0; i < labels_QiangTou2.Length; i++)
            {
                labels_QiangTou2[i] = (Label)this.Controls.Find("label" + (i + 81), true)[0];
            }
            for (int i = 0; i < labels_QiangTou3.Length; i++)
            {
                labels_QiangTou3[i] = (Label)this.Controls.Find("label" + (i + 161), true)[0];
            }
            for (int i = 0; i < labels_QiangTou4.Length; i++)
            {
                labels_QiangTou4[i] = (Label)this.Controls.Find("label" + (i + 241), true)[0];
            }
            for (int i = 0; i < labels_LiXinGuan.Length; i++)
            {
                labels_LiXinGuan[i] = (Label)this.Controls.Find("label" + (i + 321), true)[0];
            }

        }
        private void UpdateLabelColors()
        {
            try
            {
                for (int i = 0; i < 338; i++)
                {
                    labels_ALL[i].Image = imageList1.Images[0];
                }
                for (int i = 0; i < 80 - MyVariable.area_QiangTou1.num_Remain; i++)
                {
                    labels_QiangTou1[i].Image = imageList1.Images[1];
                }
                for (int i = 0; i < 80 - MyVariable.area_QiangTou2.num_Remain; i++)
                {
                    labels_QiangTou2[i].Image = imageList1.Images[1];
                }
                for (int i = 0; i < 80 - MyVariable.area_QiangTou3.num_Remain; i++)
                {
                    labels_QiangTou3[i].Image = imageList1.Images[1];
                }
                for (int i = 0; i < 80 - MyVariable.area_QiangTou4.num_Remain; i++)
                {
                    labels_QiangTou4[i].Image = imageList1.Images[1];
                }
                for (int i = 0; i < 18 - MyVariable.area_LiXinGuan.num_Remain; i++)
                {
                    labels_LiXinGuan[i].Image = imageList1.Images[1];
                }
            }
            catch (Exception ex)
            {
                LogConfig.Instance.ShowMessageToList("Run", ex.Message, MsgType.Success, Color.Red);
            }
        }
        private void MainForm_info_Load(object sender, EventArgs e)
        {
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateLabelColors();
            lab_1000Tip_1.Text = MyVariable.area_QiangTou1.num_Remain.ToString();
            lab_1000Tip_2.Text = MyVariable.area_QiangTou2.num_Remain.ToString();
            lab_200Tip.Text = MyVariable.area_QiangTou3.num_Remain.ToString();
            lab_50Tip.Text = MyVariable.area_QiangTou4.num_Remain.ToString();
            lab_lixinguan.Text = MyVariable.area_LiXinGuan.num_Remain.ToString();
            lab_FCF.Text = MyVariable.area_DiWen_FCF.num_Remain.ToString();
            lab_FCT.Text = MyVariable.area_DiWen_FCT.num_Remain.ToString();
            lab_SB.Text = MyVariable.area_DiWen_SB.num_Remain.ToString();
            lab_LIB.Text = MyVariable.area_DiWen_LIB.num_Remain.ToString();
            lab_DIL.Text = MyVariable.area_DiWen_DIL.num_Remain.ToString();
            lab_WMX.Text = MyVariable.area_DiWen_WMX.num_Remain.ToString();
            lab_S.Text = MyVariable.area_DiWen_S.num_Remain.ToString();
            lab_tip1x.Text = (MyVariable.area_QiangTou1.num_X + 1).ToString();
            lab_tip1y.Text = (MyVariable.area_QiangTou1.num_Y + 1).ToString();
            lab_tip2x.Text = (MyVariable.area_QiangTou2.num_X + 1).ToString();
            lab_tip2y.Text = (MyVariable.area_QiangTou2.num_Y + 1).ToString();
            lab_tip3x.Text = (MyVariable.area_QiangTou3.num_X + 1).ToString();
            lab_tip3y.Text = (MyVariable.area_QiangTou3.num_Y + 1).ToString();
            lab_tip4x.Text = (MyVariable.area_QiangTou4.num_X + 1).ToString();
            lab_tip4y.Text = (MyVariable.area_QiangTou4.num_Y + 1).ToString();
            lab_lixinguanx.Text = (MyVariable.area_LiXinGuan.num_X + 1).ToString();
            lab_lixinguany.Text = (MyVariable.area_LiXinGuan.num_Y + 1).ToString();
            if (MyVariable.consumables_Empty[0])
            {
                gbx_qiangtou1.BackColor = Color.MistyRose;
                gbx_qiangtou2.BackColor = Color.MistyRose;
            }
            else
            {
                gbx_qiangtou1.BackColor = Color.LightCyan;
                gbx_qiangtou2.BackColor = Color.LightCyan;
            }
            if (MyVariable.consumables_Empty[1])
            {
                gbx_qiangtou3.BackColor = Color.MistyRose;
            }
            else
            {
                gbx_qiangtou3.BackColor = Color.LightCyan;
            }
            if (MyVariable.consumables_Empty[2])
            {
                gbx_qiangtou4.BackColor = Color.MistyRose;
            }
            else
            {
                gbx_qiangtou4.BackColor = Color.LightCyan;
            }
            if (MyVariable.consumables_Empty[3])
            {
                gbx_diwen.BackColor = Color.MistyRose;
            }
            else
            {
                gbx_diwen.BackColor = Color.LightCyan;
            }
            if (MyVariable.consumables_Empty[4])
            {
                gbx_lixinguan.BackColor = Color.MistyRose;
            }
            else
            {
                gbx_lixinguan.BackColor = Color.LightCyan;
            }

        }
    }
}
