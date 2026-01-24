using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYCustomControl
{
    public partial class XListBox : UserControl
    {
        private DataGridView datagridView;
        private object obj;
        public XListBox()
        {
            InitializeComponent();
            obj = new object();
        }

        public int ListBoxRows
        {
            get { return datagridView.Rows.Count; }
        }

        #region 组件设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            datagridView = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.datagridView)).BeginInit();
            this.SuspendLayout();
            this.datagridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datagridView.Location = new System.Drawing.Point(0, 0);
            this.datagridView.BackgroundColor = Color.White;
            this.datagridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridViewTextBoxColumn column_textBox = new DataGridViewTextBoxColumn();
            this.datagridView.Columns.Add(column_textBox);
            this.datagridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.datagridView.AllowUserToAddRows = false;
            this.datagridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.datagridView.RowHeadersVisible = false;
            this.datagridView.ColumnHeadersVisible = false;
            this.datagridView.ReadOnly = true;
            this.datagridView.AllowUserToResizeRows = false;
            this.datagridView.MultiSelect = false;

            //设置自动换行
            this.datagridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            components = new System.ComponentModel.Container();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(datagridView);
            ((System.ComponentModel.ISupportInitialize)(this.datagridView)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion


        public void UpdateListMsg(string str, Color cr)
        {
            lock (obj)
            {
                base.BeginInvoke(new Action(() =>
                {
                    DataGridViewRow dr = new DataGridViewRow();
                    DataGridViewRow dr2 = new DataGridViewRow();
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    DateTime dt = DateTime.Now;
                    ///******添加消息*******/
                    cell2.Value = str;
                    cell2.Style.ForeColor = cr;
                    cell2.Style.Font = new Font("微软雅黑", (float)8, FontStyle.Regular);
                    dr2.Cells.Add(cell2);
                    datagridView.Rows.Insert(0, dr2);
                    /******添加时间******/
                    cell.Value = (dt.ToString("yyyy-MM-dd HH:mm:ss"));
                    cell.Style.ForeColor = cr;
                    cell.Style.Font = new Font("微软雅黑", (float)8, FontStyle.Regular);
                    dr.Cells.Add(cell);
                    datagridView.Rows.Insert(0, dr);
                })
                );
            }
        }

        public void ClearListMsg()
        {
            base.BeginInvoke(new Action(() =>
            {
                datagridView.Rows.Clear();
            })
            );
        }
    }
}
