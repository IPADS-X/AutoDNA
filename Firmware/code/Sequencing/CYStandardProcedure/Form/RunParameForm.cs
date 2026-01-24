using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CYAutoFramework;

namespace CYStandardProcedure
{
    public partial class RunParameForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        #region 窗体控件自适应代码       
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            comboBox1_SelectedIndexChanged(new object(), new EventArgs());
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }
        #endregion
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        public RunParameForm()
        {
            InitializeComponent();
        }

        private void RunParameForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            /***添加参数种类信息***/
            int count = ParameConfig.Instance.SystemParamTypeNameList.Count;
            comboBox1.Items.Add("All Parame");
            for (int i = 1; i < count + 1; i++)
            {
                comboBox1.Items.Add(ParameConfig.Instance.SystemParamTypeNameList[i - 1]);
            }
            comboBox1.SelectedIndex = 0;
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += RunParameForm_LanguageChangeEvent; ;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void RunParameForm_LanguageChangeEvent(string strLanguage)
        {
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Save, "系统配置参数写入");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Save, "System Configuration Parameter Write");
            }
            else
            {
                toolTip1.SetToolTip(btn_Save, "Viết tham số cấu hình hệ thống");
            }
            /***重新加载并显示参数***/
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            int index = comboBox1.SelectedIndex;
            if (index == 0)
            {
                /***加载全部参数***/
                ParameConfig.Instance.ParameCfgDic["System"].UpdateParameterToGrid(dataGridView1);
            }
            else
            {
                /***加载特定参数***/
                ParameConfig.Instance.ParameCfgDic["System"].UpdateParameterToGrid(dataGridView1,
                    ParameConfig.Instance.RefineSystemParame[index - 1]);
            }
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件值***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            dataGridView1.Columns[0].ReadOnly = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            int index = comboBox1.SelectedIndex;
            if (index == 0)
            {
                /***加载全部参数***/
                ParameConfig.Instance.ParameCfgDic["System"].UpdateParameterToGrid(dataGridView1);
            }
            else
            {
                /***加载特定参数***/
                ParameConfig.Instance.ParameCfgDic["System"].UpdateParameterToGrid(dataGridView1,
                    ParameConfig.Instance.RefineSystemParame[index - 1]);
            }

            string path = Path.Combine(Application.StartupPath, "Language", LanguageConfig.Instance.Language, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            if (index == 0)
            {
                if (!ParameConfig.Instance.ParameCfgDic["System"].UpdateGridToFile(dataGridView1))
                {
                    MessageBox.Show("All parameters saved Failed！", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("All Parameters Saved Successfully！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MyVariable.ReadPipetteParam();
                    if (MyVariable.show_IsOpen)
                    {
                        MyVariable.ReadShowVolume();
                    }
                }
            }
            else
            {
                if (!ParameConfig.Instance.ParameCfgDic["System"].UpdateGridToFile(dataGridView1, index - 1))
                {
                    MessageBox.Show(comboBox1.Text.Trim() + "Save Failed！", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(comboBox1.Text.Trim() + "Save Successfully！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MyVariable.ReadPipetteParam();
                    if (MyVariable.show_IsOpen)
                    {
                        MyVariable.ReadShowVolume();
                    }
                }
            }
            btn_Save.BaseColorEnd = Color.Transparent;
            SoftWareForm.m_softwarmform.Invoke(new Action(() =>
            {
                #region 屏蔽参数显示
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)))
                {
                    SoftWareForm.m_softwarmform.tsl_general.Text = "屏蔽";
                    SoftWareForm.m_softwarmform.tsl_general.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
                }
                else
                {
                    SoftWareForm.m_softwarmform.tsl_general.Text = "正常";
                    SoftWareForm.m_softwarmform.tsl_general.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
                }
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                {
                    SoftWareForm.m_softwarmform.tsl_plc.Text = "屏蔽";
                    SoftWareForm.m_softwarmform.tsl_plc.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
                }
                else
                {
                    SoftWareForm.m_softwarmform.tsl_plc.Text = "正常";
                    SoftWareForm.m_softwarmform.tsl_plc.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
                }
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledCCD.ToString()].CurrentValue)))
                {
                    SoftWareForm.m_softwarmform.tsl_ccd.Text = "屏蔽";
                    SoftWareForm.m_softwarmform.tsl_ccd.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
                }
                else
                {
                    SoftWareForm.m_softwarmform.tsl_ccd.Text = "正常";
                    SoftWareForm.m_softwarmform.tsl_ccd.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
                }
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledBarcode.ToString()].CurrentValue)))
                {
                    SoftWareForm.m_softwarmform.tsl_barcord.Text = "屏蔽";
                    SoftWareForm.m_softwarmform.tsl_barcord.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
                }
                else
                {
                    SoftWareForm.m_softwarmform.tsl_barcord.Text = "正常";
                    SoftWareForm.m_softwarmform.tsl_barcord.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
                }
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledSequence.ToString()].CurrentValue)))
                {
                    SoftWareForm.m_softwarmform.tsl_sequence.Text = "屏蔽";
                    SoftWareForm.m_softwarmform.tsl_sequence.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
                }
                else
                {
                    SoftWareForm.m_softwarmform.tsl_sequence.Text = "正常";
                    SoftWareForm.m_softwarmform.tsl_sequence.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
                }
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledAnimation.ToString()].CurrentValue)))
                {
                    SoftWareForm.m_softwarmform.tsl_animation.Text = "屏蔽";
                    SoftWareForm.m_softwarmform.tsl_animation.BackColor = System.Drawing.Color.FromArgb(255, 174, 201);
                }
                else
                {
                    SoftWareForm.m_softwarmform.tsl_animation.Text = "正常";
                    SoftWareForm.m_softwarmform.tsl_animation.BackColor = System.Drawing.Color.FromArgb(0, 255, 0);
                }
                #endregion
            }));
            if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledLight.ToString()].CurrentValue)))
            {
                IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯红.ToString(), 0);
                IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯绿.ToString(), 0);
                IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯黄.ToString(), 0);
            }

        }
    }
}
