using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CYAutoFramework;

namespace CYStandardProcedure
{
    public partial class FrmUserWrite : Form
    {
        /***按钮提示语***/
        private ToolTip toolTip1  = new ToolTip();

        /***Ini文件配置对象***/
        private INIFile mIni;

        /***Ini文件路径***/
        private string mUserINIPath = System.Windows.Forms.Application.StartupPath + @"\ExeFile\User.ini";
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        public FrmUserWrite()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            mIni = new INIFile(mUserINIPath);
        }

        private void FrmUserWrite_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            this.panel1.MouseDown += new MouseEventHandler(panel1_MouseDown);
            LoadUser();
            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += FrmUserWrite_LanguageChangeEvent; ;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void FrmUserWrite_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Save, "写入权限");
            }
            else if(strLanguage=="EN")
            {
                toolTip1.SetToolTip(btn_Save, "Write Permission");
            }
            else
            {
                toolTip1.SetToolTip(btn_Save, "Ghi quyền");
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x02, 0);
            }
        }

        private void LoadUser()
        {
            try
            {
                dataGridView1.ColumnCount = 4;
                if (LanguageConfig.Instance.Language == "CH")
                {
                    dataGridView1.Columns[0].HeaderText = "卡号";
                    dataGridView1.Columns[1].HeaderText = "姓名";
                    dataGridView1.Columns[2].HeaderText = "工号";
                    dataGridView1.Columns[3].HeaderText = "权限等级";
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    dataGridView1.Columns[0].HeaderText = "CardID";
                    dataGridView1.Columns[1].HeaderText = "Name";
                    dataGridView1.Columns[2].HeaderText = "Job Number";
                    dataGridView1.Columns[3].HeaderText = "User Level";
                }
                else
                {
                    dataGridView1.Columns[0].HeaderText = "Số thẻ";
                    dataGridView1.Columns[1].HeaderText = "tên đầy đủ";
                    dataGridView1.Columns[2].HeaderText = "Số công việc";
                    dataGridView1.Columns[3].HeaderText = "Cấp người dùng";
                }
                int size = dataGridView1.Width / 4;//单个列的宽度
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns[i].Resizable = DataGridViewTriState.NotSet;
                    dataGridView1.Columns[i].Width = size;
                    dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (AdminConfig.Instance.UserLevel == 2)
                    {
                        dataGridView1.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dataGridView1.Columns[i].ReadOnly = true;
                    }
                }
                dataGridView1.EnableHeadersVisualStyles = false;//缺少该行代码，标题的样式无法改变
                dataGridView1.RowHeadersVisible = false;//影藏行的标题头
                dataGridView1.AllowUserToResizeRows = false;//行不可调整
                dataGridView1.AllowUserToResizeColumns = false;//列不可调整
                dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.BorderStyle = BorderStyle.None;
                dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dataGridView1.ColumnHeadersHeight = 30;
                dataGridView1.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.InsetDouble;
                dataGridView1.GridColor = Color.FromArgb(149, 148, 142);
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightYellow;
                int num = 0;
                for (int i = 0; i < AdminConfig.Instance.EmployeeInfoList.Count; i++)
                {
                    if (AdminConfig.Instance.EmployeeInfoList[i].UserLevel <= AdminConfig.Instance.UserLevel)
                    {
                        dataGridView1.Rows.Add();
                        dataGridView1[0, num].Value = AdminConfig.Instance.EmployeeInfoList[i].ID;
                        dataGridView1[1, num].Value = AdminConfig.Instance.EmployeeInfoList[i].Name;
                        dataGridView1[2, num].Value = AdminConfig.Instance.EmployeeInfoList[i].Number;
                        dataGridView1[3, num].Value = AdminConfig.Instance.EmployeeInfoList[i].UserLevel;
                        num++;
                    }
                }
            }
            catch (Exception ec)
            {
                ;
            }
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.dataGridView1.Rows[e.RowIndex].Selected = true;
                this.dataGridView1.CurrentCell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
                this.contextMenuStrip1.Show(this.dataGridView1, e.Location);
                contextMenuStrip1.Show(Cursor.Position);
            }
        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                DataGridViewTextBoxEditingControl tb =
                    (DataGridViewTextBoxEditingControl)e.Control;
                ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
                tb.ContextMenuStrip = contextMenuStrip1;
            }
            else
            {
                ((DataGridViewTextBoxEditingControl)e.Control).ContextMenuStrip = null;
            }
        }

        private void 添加一行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AdminConfig.Instance.UserLevel == 1)
            {
                MessageBox.Show("用户权限等级不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = dataGridView1.CurrentRow.Index;
            dataGridView1.Rows.Insert(index, 1);
        }

        private void 删除当前行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AdminConfig.Instance.UserLevel == 1)
            {
                MessageBox.Show("用户权限等级不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                if (!this.dataGridView1.Rows[dataGridView1.CurrentRow.Index].IsNewRow)
                {
                    DialogResult Dr = MessageBox.Show("Are you sure you want to delete this row？", "Tip", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    while (true)
                    {
                        Thread.Sleep(5);
                        if (Dr == DialogResult.Yes)
                        {
                            break;
                        }
                        else
                        {
                            return;
                        }
                    }
                    string name = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                    this.dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                    AdminConfig.Instance.DeleteEmployeeInfo(name);
                }
            }
            catch { }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Save_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (AdminConfig.Instance.UserLevel == 1)
                {
                    MessageBox.Show("用户权限等级不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (File.Exists(mUserINIPath))
                {
                    File.Delete(mUserINIPath);
                }
                string id;
                string name;
                string cardNum;
                int userlevel;
                int n = dataGridView1.Rows.Count;
                for (int i = 0; i < n - 1; i++)
                {
                    id = dataGridView1[0, i].Value.ToString();
                    name = dataGridView1[1, i].Value.ToString();
                    cardNum = dataGridView1[2, i].Value.ToString();
                    userlevel = int.Parse(dataGridView1[3, i].Value.ToString());
                    mIni.Write(id, "Name", name);
                    mIni.Write(id, "CardID", cardNum);
                    mIni.Write(id, "UserLevel", userlevel);
                }
                AdminConfig.Instance.IntiEnterCardInfo();
                MessageBox.Show("Save OK", "Tip！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }
    }
}
