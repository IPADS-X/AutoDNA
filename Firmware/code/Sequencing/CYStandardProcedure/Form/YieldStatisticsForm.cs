using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using CYAutoFramework;

namespace CYStandardProcedure
{
    public partial class YieldStatisticsForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        public YieldStatisticsForm()
        {
            InitializeComponent();
        }

        private void YieldStatistics_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            mAutosize.ControlInitializeSize(this);
      
            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += Instance_ChangeLanguageHandle;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);

            Yield.Instance.HourCapacityChangedEvent += Instance_CapacityChangedEvent;//订阅当天良率统计更新事件
            Yield.Instance.ClassesCapacityChangedEvent += Instance_ClassesChangedEvent;//订阅一周良率统计更新实践
        }

        private void Instance_ClassesChangedEvent(object sender, Dictionary<string, ClassesCapacity> e)
        {
            this.Invoke(new Action(() =>
            {
                List<int> lsTotal = new List<int>();
                List<int> lsNG = new List<int>();
                List<int> lsOK = new List<int>();
                List<double> lsScale = new List<double>();
                foreach (KeyValuePair<string, ClassesCapacity> kvp in e)
                {
                    lsTotal.Add(kvp.Value.TotalCount);
                    lsNG.Add(kvp.Value.NgCount);
                    lsOK.Add(kvp.Value.OkCount);
                    if (kvp.Value.TotalCount == 0)
                    {
                        lsScale.Add(100);
                    }
                    else
                    {
                        double dd = Math.Round((double)kvp.Value.OkCount / kvp.Value.TotalCount, 3);
                        lsScale.Add(dd * 100);
                    }
                }
                chart_Week.Series[0].Points.DataBindXY(e.Keys.ToList(), lsTotal);
                chart_Week.Series[1].Points.DataBindXY(e.Keys.ToList(), lsOK);
                chart_Week.Series[2].Points.DataBindXY(e.Keys.ToList(), lsNG);
                chart_Week.Series[3].Points.DataBindXY(e.Keys.ToList(), lsScale);
            }));
        }

        private void Instance_CapacityChangedEvent(object sender, Dictionary<string, HourCapacity> e)
        {
            this.Invoke(new Action(() =>
            {
                List<int> lsTotal = new List<int>();
                List<int> lsNG = new List<int>();
                List<int> lsOK = new List<int>();
                List<double> lsScale = new List<double>();
                foreach (KeyValuePair<string, HourCapacity> kvp in e)
                {
                    lsTotal.Add(kvp.Value.TotalCount);
                    lsNG.Add(kvp.Value.NgCount);
                    lsOK.Add(kvp.Value.OkCount);
                    if (kvp.Value.TotalCount == 0)
                    {
                        lsScale.Add(100);
                    }
                    else
                    {
                        double dd = Math.Round((double)kvp.Value.OkCount / kvp.Value.TotalCount, 3);
                        lsScale.Add(dd * 100);
                    }
                }
                chart_Statist.Series[0].Points.DataBindXY(e.Keys.ToList(), lsTotal);
                chart_Statist.Series[1].Points.DataBindXY(e.Keys.ToList(), lsOK);
                chart_Statist.Series[2].Points.DataBindXY(e.Keys.ToList(), lsNG);
                chart_Statist.Series[3].Points.DataBindXY(e.Keys.ToList(), lsScale);
                Dictionary<string, int> dc = new Dictionary<string, int>();
                dc.Add("OK", lsOK.Sum());
                dc.Add("NG", lsNG.Sum());
                chart_Yield.Series[0].Points.DataBindXY(dc.Keys.ToList(), dc.Values.ToList());
                chart_Yield.Series[0].Points[0].Color = Color.LimeGreen;
                chart_Yield.Series[0].Points[0].LabelForeColor = Color.LimeGreen;
                chart_Yield.Series[0].Points[1].Color = Color.Red;
                chart_Yield.Series[0].Points[1].LabelForeColor = Color.Red;
                Instance_ChangeLanguageHandle(LanguageConfig.Instance.Language);
            }));
        }

        /// <summary>
        /// 语言改变事件订阅方法
        /// </summary>
        /// <param name="strLanguage"></param>
        private void Instance_ChangeLanguageHandle(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            #region 按钮提示语
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Write, "交接班时间写入");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Write, "Write shift handover time");
            }
            else
            {
                toolTip1.SetToolTip(btn_Write, "Thời gian chuyển giao Ghi");
            }
            #endregion

            /***检索当前时间是否在白班的时间段内***/
            string classes = Yield.Instance.GetCurClasses();
            if (classes == "Day")
            {
                /***白班***/
                switch (strLanguage)
                {
                    case "CH":
                        chart_Statist.Titles[0].Text = "白班统计";
                        chart_Week.Titles[0].Text = "白班一周良率统计";
                        chart_Yield.Titles[0].Text = "白班良率";
                        break;
                    case "EN":
                        chart_Statist.Titles[0].Text = "Day shift statistics";
                        chart_Week.Titles[0].Text = "Day shift one week yield statistics";
                        chart_Yield.Titles[0].Text = "Day shift yield";
                        break;
                    case "VN":
                        chart_Statist.Titles[0].Text = "Thống kê ban ngày";
                        chart_Week.Titles[0].Text = "Thống kê tỷ lệ tuần làm việc";
                        chart_Yield.Titles[0].Text = "Tỷ lệ ban ngày";
                        break;
                }
            }
            else
            {
                /***夜班***/
                switch (strLanguage)
                {
                    case "CH":
                        chart_Statist.Titles[0].Text = "夜班统计";
                        chart_Week.Titles[0].Text = "夜班一周良率统计";
                        chart_Yield.Titles[0].Text = "夜班良率";
                        break;
                    case "EN":
                        chart_Statist.Titles[0].Text = "Night shift statistics";
                        chart_Week.Titles[0].Text = "Night shift one week yield statistics";
                        chart_Yield.Titles[0].Text = "Night shift yield";
                        break;
                    case "VN":
                        chart_Statist.Titles[0].Text = "Thống kê Night Shift";
                        chart_Week.Titles[0].Text = "Ca đêm 1 tuần Thống kê";
                        chart_Yield.Titles[0].Text = "Tỷ lệ ca đêm";
                        break;
                }
            }

            switch (strLanguage)
            {
                case "CH":
                    chart_Statist.Series[0].LegendText = "总数";
                    chart_Statist.Series[1].LegendText = "OK数";
                    chart_Statist.Series[2].LegendText = "NG数";
                    chart_Statist.Series[3].LegendText = "良率";
                    chart_Week.Series[0].LegendText = "总数";
                    chart_Week.Series[1].LegendText = "OK数";
                    chart_Week.Series[2].LegendText = "NG数";
                    chart_Week.Series[3].LegendText = "良率";
                    break;
                case "EN":
                    chart_Statist.Series[0].LegendText = "Total";
                    chart_Statist.Series[1].LegendText = "OKQuantity";
                    chart_Statist.Series[2].LegendText = "NgQuantity";
                    chart_Statist.Series[3].LegendText = "Yield";
                    chart_Week.Series[0].LegendText = "Total";
                    chart_Week.Series[1].LegendText = "OKQuantity";
                    chart_Week.Series[2].LegendText = "NgQuantity";
                    chart_Week.Series[3].LegendText = "Yield";
                    break;
                case "VN":
                    chart_Statist.Series[0].LegendText = "Tổng số";
                    chart_Statist.Series[1].LegendText = "Số OK";
                    chart_Statist.Series[2].LegendText = "Số NG";
                    chart_Statist.Series[3].LegendText = "Tỷ lệ tốt";
                    chart_Week.Series[0].LegendText = "Tổng số";
                    chart_Week.Series[1].LegendText = "Số OK";
                    chart_Week.Series[2].LegendText = "Số NG";
                    chart_Week.Series[3].LegendText = "Tỷ lệ tốt";
                    break;
            }
        }

        private void YieldStatistics_SizeChanged(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }

        private void txt_HandHour_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 53 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                switch (LanguageConfig.Instance.Language)
                {
                    case "CH":
                        MessageBox.Show("请输入5-9范围内数字！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "EN":
                        MessageBox.Show("Please enter a number within the range of 5-9！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "VN":
                        MessageBox.Show("Vui lòng nhập số từ 5-9！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
                e.Handled = true;
            }
        }

        private void txt_HnadMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                switch (LanguageConfig.Instance.Language)
                {
                    case "CH":
                        MessageBox.Show("请输入合法数字！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "EN":
                        MessageBox.Show("Please enter a valid number！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "VN":
                        MessageBox.Show("Vui lòng nhập số hợp lệ！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
                e.Handled = true;
            }
        }

        private void txt_HandHour_TextChanged(object sender, EventArgs e)
        {
            if (txt_HandHour.Text != "")
            {
                int number = int.Parse(txt_HandHour.Text);
                txt_HandHour.Text = number.ToString();
                if (number > 9 || number < 5)
                {
                    switch (LanguageConfig.Instance.Language)
                    {
                        case "CH":
                            MessageBox.Show("请输入5-9范围内数字！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case "EN":
                            MessageBox.Show("Please enter a number within the range of 5-9！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case "VN":
                            MessageBox.Show("Vui lòng nhập số từ 5-9！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    txt_HandHour.Text = txt_HandHour.Text.Remove(0);
                    txt_HandHour.SelectionStart = 0;
                    return;
                }
            }
        }


        private void txt_HnadMin_TextChanged(object sender, EventArgs e)
        {
            if (txt_HnadMin.Text != "")
            {
                int number = int.Parse(txt_HnadMin.Text);
                txt_HnadMin.Text = number.ToString();
                if (number > 59)
                {
                    switch (LanguageConfig.Instance.Language)
                    {
                        case "CH":
                            MessageBox.Show("请输入0-59范围内数字！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case "EN":
                            MessageBox.Show("Please enter a number within the range of 0-59！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case "VN":
                            MessageBox.Show("Vui lòng nhập số từ 0-59！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    txt_HnadMin.Text = txt_HnadMin.Text.Remove(0);
                    txt_HnadMin.SelectionStart = 0;
                    return;
                }
            }
        }

        private void YieldStatistics_Shown(object sender, EventArgs e)
        {
            /***显示交接时间，分钟***/
            txt_HandHour.Text = Yield.Instance.HandOverHour;
            txt_HnadMin.Text = Yield.Instance.HandOverMinute;
            /***显示白班或者夜班统计信息***/
            Yield.Instance.ShowClassesYield();
            /***更新Chart控件标题***/
            Instance_ChangeLanguageHandle(LanguageConfig.Instance.Language);
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (txt_HandHour.Text.Trim() != "" &&
                txt_HnadMin.Text.Trim() != "")
            {
                if (txt_HandHour.Text.Trim().PadLeft(2, '0') == Yield.Instance.HandOverHour.ToString()
                    && txt_HnadMin.Text.Trim().PadLeft(2, '0') == Yield.Instance.HandOverMinute.ToString())
                {
                    return;
                }
                if (Yield.Instance.SetHandOverTime(txt_HandHour.Text.Trim().PadLeft(2, '0'), txt_HnadMin.Text.Trim().PadLeft(2, '0')))
                {
                    switch (LanguageConfig.Instance.Language)
                    {
                        case "CH":
                            MessageBox.Show("交接班时间设定成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case "EN":
                            MessageBox.Show("Successfully set the handover time！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case "VN":
                            MessageBox.Show("Thiết lập thời gian chuyển giao thành công！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                    }
                }
                else
                {
                    switch (LanguageConfig.Instance.Language)
                    {
                        case "CH":
                            MessageBox.Show("交接班时间设定失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case "EN":
                            MessageBox.Show("Shift handover time setting failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case "VN":
                            MessageBox.Show("Thiết lập thời gian chuyển giao thất bại！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
            }
            else
            {
                switch (LanguageConfig.Instance.Language)
                {
                    case "CH":
                        MessageBox.Show("设定值不可以为空！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "EN":
                        MessageBox.Show("The set value cannot be empty！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "VN":
                        MessageBox.Show("Giá trị không được để trống！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Yield.Instance.UpdateYield(true);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Yield.Instance.UpdateYield(false);
        }

    }
}
