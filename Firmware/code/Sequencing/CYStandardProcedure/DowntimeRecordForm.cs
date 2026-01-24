using CYFramework;
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
    public partial class DowntimeRecordForm : Form
    {

        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        public DowntimeRecordForm()
        {
            InitializeComponent();
        }


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

        private void DownTimeRecordForm_Load(object sender, EventArgs e)
        {
            /***订阅Listview控件事件***/
            cyListView_Today.DrawItem += View_DrawItem;
            cyListView_Today.DrawSubItem += View_DrawSubItem;
            cyListView_Today.DrawColumnHeader += View_DrawColumnHeader;
            cyListView_30Day.DrawItem += View_DrawItem;
            cyListView_30Day.DrawSubItem += View_DrawSubItem;
            cyListView_30Day.DrawColumnHeader += View_DrawColumnHeader;
            mAutosize.ControlInitializeSize(this);
            DownTime.Instance.ReadDowntimeRecordEvent += Instance_ReadDowntimeRecordEvent;//读取宕机记录
            DownTime.Instance.AddDowntimeRecordEvent += Instance_AddDowntimeRecordEvent;//新增宕机记录
            DownTime.Instance.EndDowntimeRecordEvent += Instance_EndDowntimeRecordEvent;//解除ListView红色宕机部分
            DownTime.Instance.ClearListViewItemEvent += Instance_ClearListViewItemEvent;//清空宕机记录
            DownTime.Instance.ShowDownTimeRecord();//显示读取的宕机记录
        }




        private void Instance_ClearListViewItemEvent(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                cyListView_Today.Items.Clear();
                cyListView_30Day.Items.Clear();
            }));
        }

        private void Instance_EndDowntimeRecordEvent(object sender, EventArgs e)
        {
            int a1 = cyListView_Today.Items.Count - 1;
            int a2 = cyListView_30Day.Items.Count - 1;
            cyListView_Today.Items[0].BackColor = Color.LightYellow;
            cyListView_30Day.Items[0].BackColor = Color.LightYellow;
        }

        private void Instance_ReadDowntimeRecordEvent(List<DownTimeSlot> ls1, List<DownTimeSlot> ls2)
        {
            this.Invoke(new Action(() =>
            {
                /***显示当天记录***/
                cyListView_Today.Items.Clear();
                cyListView_Today.BeginUpdate();
                foreach (var va in ls1)
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems.Add(va.StartTime);
                    item.SubItems.Add(Language.Instance.TransMsg(va.ErrorMsg));
                    item.SubItems.Add(va.ErrorCode);
                    item.SubItems.Add(Language.Instance.TransMsg(va.Solution));
                    item.ForeColor = Color.Black;
                    item.BackColor = Color.LightYellow;
                    cyListView_Today.Items.Add(item);
                }
                cyListView_Today.EndUpdate();
                /***显示最近30天记录***/
                cyListView_30Day.Items.Clear();
                cyListView_30Day.BeginUpdate();
                foreach (var va in ls2)
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems.Add(va.StartTime);
                    item.SubItems.Add(Language.Instance.TransMsg(va.ErrorMsg));
                    item.SubItems.Add(va.ErrorCode);
                    item.SubItems.Add(Language.Instance.TransMsg(va.Solution));
                    item.ForeColor = Color.Black;
                    item.BackColor = Color.LightYellow;
                    cyListView_30Day.Items.Add(item);
                }
                cyListView_30Day.EndUpdate();
            }));
        }

        private void Instance_AddDowntimeRecordEvent(object obj, DownTimeSlot info)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    ListViewItem[] view = new ListViewItem[2];
                    for (int i = 0; i < view.Length; i++)
                    {
                        view[i] = new ListViewItem();
                    }
                    view[0].SubItems.Add(info.StartTime);
                    view[0].SubItems.Add(Language.Instance.TransMsg(info.ErrorMsg));
                    view[0].SubItems.Add(info.ErrorCode);
                    view[0].SubItems.Add(Language.Instance.TransMsg(info.Solution));
                    view[0].ForeColor = Color.Black;
                    view[0].BackColor = Color.IndianRed;
                    view[1].SubItems.Add(info.StartTime);
                    view[1].SubItems.Add(Language.Instance.TransMsg(info.ErrorMsg));
                    view[1].SubItems.Add(info.ErrorCode);
                    view[1].SubItems.Add(Language.Instance.TransMsg(info.Solution));
                    view[1].ForeColor = Color.Black;
                    view[1].BackColor = Color.IndianRed;
                    /***更新当天***/
                    cyListView_Today.BeginUpdate();
                    cyListView_Today.Items.Insert(0, view[0]);
                    cyListView_Today.EndUpdate();
                    /***更新最近30天***/
                    cyListView_30Day.BeginUpdate();
                    cyListView_30Day.Items.Insert(0, view[1]);
                    cyListView_30Day.EndUpdate();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "表格更新错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DownTimeRecordForm_SizeChanged(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DownTime.Instance.StartDowntime("轴报警!", DowntimeType.MotionError, "伺服驱动器断电");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DownTime.Instance.StartDowntime("真空吸异常!", DowntimeType.VacuumError, "请检查吸嘴");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DownTime.Instance.StartDowntime("平移气缸动点异常!", DowntimeType.MagnetError, "请检查磁环信号");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DownTime.Instance.StartDowntime("流线到位光电超时!", DowntimeType.SensorError, "请检查到位光电信号");
        }
    }
}
