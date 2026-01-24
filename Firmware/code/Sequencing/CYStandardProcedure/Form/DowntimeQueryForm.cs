using CYAutoFramework;
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

namespace CYStandardProcedure
{
    public partial class DowntimeQueryForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        /// <summary>
        /// 重绘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        /// <summary>
        /// 重绘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        /// <summary>
        /// 标题重绘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Graphics.FillRectangle(Brushes.LightPink, e.Bounds);
                e.DrawText();
            }
            else if (e.ColumnIndex == 1)
            {
                e.Graphics.FillRectangle(Brushes.LightPink, e.Bounds);
                e.DrawText();
            }
            else if (e.ColumnIndex == 2)
            {
                e.Graphics.FillRectangle(Brushes.LightPink, e.Bounds);
                e.DrawText();
            }
            else if (e.ColumnIndex == 3)
            {
                e.Graphics.FillRectangle(Brushes.LightPink, e.Bounds);
                e.DrawText();
            }
            else if (e.ColumnIndex == 4)
            {
                e.Graphics.FillRectangle(Brushes.LightPink, e.Bounds);
                e.DrawText();
            }
        }

        public DowntimeQueryForm()
        {
            InitializeComponent();
        }

        private void DowntimeClassifyForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            cyListView_Today.DrawItem += View_DrawItem;
            cyListView_Today.DrawSubItem += View_DrawSubItem;
            cyListView_Today.DrawColumnHeader += View_DrawColumnHeader;
            DownTime.Instance.QueryDayDownTimeEvent += Instance_QueryDayDownTimeEvent;
            DownTime.Instance.ShowDateDowntimeRecordEvent += Instance_ShowDateDowntimeRecordEvent;
            mAutosize.ControlInitializeSize(this);

            /***列指定名称***/
            for (int i = 0; i < cyListView_Today.Columns.Count; i++)
            {
                cyListView_Today.Columns[i].Name = "Column" + i.ToString();
            }

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += Instance_ChangeLanguageHandle;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void Instance_ShowDateDowntimeRecordEvent(object sender, List<DownTimeSlot> e)
        {
            this.Invoke(new Action(() =>
            {
                /***显示当天记录***/
                cyListView_Today.Items.Clear();
                cyListView_Today.BeginUpdate();
                foreach (var va in e)
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems.Add(va.StartTime);
                    item.SubItems.Add(va.ErrorMsg);
                    item.SubItems.Add(va.ErrorCode);
                    item.SubItems.Add(va.Solution);
                    item.ForeColor = Color.Black;
                    item.BackColor = Color.LightYellow;
                    cyListView_Today.Items.Add(item);
                }
                cyListView_Today.EndUpdate();
            }));
        }

        private void Instance_ChangeLanguageHandle(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);

            #region 按钮提示语
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Query, "宕机查询");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Query, "Downtime query");
            }
            else
            {
                toolTip1.SetToolTip(btn_Query, "Truy vấn thời gian chết");
            }
            #endregion
        }

        private void Instance_QueryDayDownTimeEvent(string[] tmstr, double[] minnute)
        {
            this.Invoke(new Action(() =>
            {
                chart_Downtime.Series[0].Points.DataBindXY(tmstr.ToList(), minnute.ToList());
            }));
        }

        private void DowntimeClassifyForm_SizeChanged(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }

        private void btn_Query_Click(object sender, EventArgs e)
        {
            DateTime dt = dtp_Date.Value;
            DownTime.Instance.GetDayDowntime(dt);
            DownTime.Instance.ShowDownTimeRecord(dt);
        }
    }
}
