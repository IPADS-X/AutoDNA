using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CYAutoFramework;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace CYStandardProcedure
{
    public partial class VisionForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        #region 窗体控件自适应代码      
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);           
        }
        #endregion

        /***按钮提示语***/
        private ToolTip toolTip1= new ToolTip();

        public string UserINIPath = System.Windows.Forms.Application.StartupPath + @"\Calibration.ini";
        public VisionForm()
        {
            InitializeComponent();
        }

        private void VisionForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            init();
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += VisionForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void VisionForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件值***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(rbt_AutoCalib, "自动标定");
                toolTip1.SetToolTip(rbt_StartPos, "起点定位");
                toolTip1.SetToolTip(rbt_AutoCalib2, "自动标定");
                toolTip1.SetToolTip(rbt_StartPos2, "起点定位");
                cmb_CCD.Items.Clear();
                cmb_CCD.Text = "";
                cmb_CCD.SelectedText = "相机1";
                cmb_CCD.Items.Add("相机1");
                cmb_CCD.Items.Add("相机2");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(rbt_AutoCalib, "Automatic Calibration");
                toolTip1.SetToolTip(rbt_StartPos, "Starting Point Positioning");
                toolTip1.SetToolTip(rbt_AutoCalib2, "Automatic Calibration");
                toolTip1.SetToolTip(rbt_StartPos2, "Starting Point Positioning");
                cmb_CCD.Items.Clear();
                cmb_CCD.Text = "";
                cmb_CCD.SelectedText = "Camera1";
                cmb_CCD.Items.Add("Camera1");
                cmb_CCD.Items.Add("Camera2");
            }
            else
            {
                toolTip1.SetToolTip(rbt_AutoCalib, "Chính tả Keywords");
                toolTip1.SetToolTip(rbt_StartPos, "Vị trí điểm đầu");
                toolTip1.SetToolTip(rbt_AutoCalib2, "Chính tả Keywords");
                toolTip1.SetToolTip(rbt_StartPos2, "Vị trí điểm đầu");
                cmb_CCD.Items.Clear();
                cmb_CCD.Text = "";
                cmb_CCD.SelectedText = "Máy ảnh1";
                cmb_CCD.Items.Add("Máy ảnh1");
                cmb_CCD.Items.Add("Máy ảnh2");
            }
        }

        private void init()
        {
            //List<string> cardId = new List<string>();
            //List<string> name = new List<string>();
            //List<string> nameId = new List<string>();
            //List<string> Jurisdiction = new List<string>();
            //List<string> axisy = new List<string>();
            //int n = INIClass.INIGetAllSectionNames(UserINIPath).Length;
            //string[] card = new string[n];
            //card = INIClass.INIGetAllSectionNames(UserINIPath);
            //for (int i = 0; i < card.Length; i++)
            //{
            //    cardId.Add(card[i]);
            //    name.Add(INIClass.INIGetStringValue(UserINIPath, cardId[i], "CCDX"));
            //    nameId.Add(INIClass.INIGetStringValue(UserINIPath, cardId[i], "CCDY"));
            //    Jurisdiction.Add(INIClass.INIGetStringValue(UserINIPath, cardId[i], "AxisX"));
            //    axisy.Add(INIClass.INIGetStringValue(UserINIPath, cardId[i], "AxisY"));
            //}
            //this.dataGridView1.AllowUserToResizeColumns = false;
            //this.dataGridView1.AllowUserToResizeRows = false;
            //dataGridView1.EnableHeadersVisualStyles = false;
            //dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 12, FontStyle.Regular);
            //dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(179, 202, 255);
            //dataGridView1.Visible = true;
            //dataGridView1.DataSource = null;
            //dataGridView1.Rows.Clear();
            //dataGridView1.Columns.Clear();
            //dataGridView1.Columns.Add("1", "点位序号");
            //dataGridView1.Columns.Add("2", "像素坐标X");
            //dataGridView1.Columns.Add("3", "像素坐标Y");
            //dataGridView1.Columns.Add("4", "轴坐标X");
            //dataGridView1.Columns.Add("5", "轴坐标Y");           
            //dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            //dataGridView1.RowsDefaultCellStyle.Font = new Font("微软雅黑", 10, FontStyle.Regular);
            //dataGridView1.RowsDefaultCellStyle.ForeColor = Color.Black;
            //dataGridView1.RowsDefaultCellStyle.BackColor = Color.FromArgb(232, 239, 235); 
            ////禁止排序
            //for (int i = 0; i < 5; i++)
            //{
            //    dataGridView1.Columns[i].ReadOnly = true;
            //    dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            //}
            //for (int i = 0; i < cardId.Count; i++)
            //{
            //    dataGridView1.Rows.Add();
            //    dataGridView1[0, i].Value = cardId[i];
            //    dataGridView1[1, i].Value = name[i];
            //    dataGridView1[2, i].Value = nameId[i];
            //    dataGridView1[3, i].Value = Jurisdiction[i];
            //    dataGridView1[4, i].Value = axisy[i];
            //}
        }

        private void rbt_AutoCalib_Click(object sender, EventArgs e)
        {

        }

        private void rbt_StartPos_Click(object sender, EventArgs e)
        {

        }

        private void rbt_AutoCalib2_Click(object sender, EventArgs e)
        {

        }

        private void rbt_StartPos2_Click(object sender, EventArgs e)
        {

        }
    }
}
