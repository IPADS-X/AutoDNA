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
using System.Xml.Linq;

namespace CYStandardProcedure
{
    public partial class AxisParameForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        #region 窗体控件自适应代码                  
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            /***显示参数***/
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            ParameConfig.Instance.ParameCfgDic["Axis"].UpdateParameterToGrid(dataGridView1);
            /***当窗体大小改变时候也需要重新设置标题语言***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }
        #endregion
        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        public AxisParameForm()
        {
            InitializeComponent();
        }

        private void AxisParameForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += AxisParameForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void AxisParameForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件值***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);

            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Save, "轴配置参数写入");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Save, "Axis parameter Save");
            }
            else
            {
                toolTip1.SetToolTip(btn_Save, "Lưu tham số trục");
            }
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

        private void btn_Save_Click(object sender, EventArgs es)
        {
            if (!ParameConfig.Instance.ParameCfgDic["Axis"].UpdateGridToFile(dataGridView1))
            {
                MessageBox.Show("ServoParame Save Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                /***修改参数记录日志***/
                if (isSaveChangedValue)
                {
                    LogConfig.Instance.WriteDataChangeLog("修改参数：" + str0[0] + "  " + str0[1] + "   值：" + str0[2]);
                    isSaveChangedValue = false;
                }
                MessageBox.Show("ServoParame Save Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                #region 数字孪生轴速度
                string filePath = Application.StartupPath + @"\ExeFile\" + @"\AxisParame" + ".xml";
                XDocument doc = XDocument.Load(filePath);
                XElement generalControlClient = doc.Descendants("Axis").FirstOrDefault(e => e.Attribute("轴名称")?.Value == _Axis.搬运XAxis.ToString());
                if (generalControlClient != null)
                {
                    SerializeClass.animationParam.carryXSpeed = Convert.ToDouble(generalControlClient.Attribute("速度")?.Value); 
                }
                XElement generalControlClient2 = doc.Descendants("Axis").FirstOrDefault(e => e.Attribute("轴名称")?.Value == _Axis.搬运YAxis.ToString());
                if (generalControlClient2 != null)
                {
                    SerializeClass.animationParam.carryYSpeed = Convert.ToDouble(generalControlClient2.Attribute("速度")?.Value);
                }

                XElement generalControlClient3 = doc.Descendants("Axis").FirstOrDefault(e => e.Attribute("轴名称")?.Value == _Axis.搬运ZAxis.ToString());
                if (generalControlClient3 != null)
                {
                    SerializeClass.animationParam.carryZSpeed = Convert.ToDouble(generalControlClient3.Attribute("速度")?.Value);
                }

                XElement generalControlClient4 = doc.Descendants("Axis").FirstOrDefault(e => e.Attribute("轴名称")?.Value == _Axis.测序仪XAxis.ToString());
                if (generalControlClient4 != null)
                {
                    SerializeClass.animationParam.sequXSpeed = Convert.ToDouble(generalControlClient4.Attribute("速度")?.Value);
                }
                #endregion

            }
            btn_Save.BaseColorEnd = Color.Transparent;
        }
        /// <summary>
        /// 修改参数是否需要保存
        /// </summary>
        private bool isSaveChangedValue = false;
        private string[] str0;
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            /***选中单元格名称***/
            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            if (colIndex == -1 || rowIndex == -1)
            { return; }
            str0 = new string[3] { "", "", "" };
            str0[0] = dataGridView1[0, rowIndex].Value.ToString();
            str0[1] = dataGridView1.Columns[colIndex].HeaderText;
            str0[2] = dataGridView1[colIndex, rowIndex].Value.ToString();
            isSaveChangedValue = true;
        }
    }
}
