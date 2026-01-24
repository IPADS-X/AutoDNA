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
    public partial class YieldQueryForm : Form
    {

        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        public YieldQueryForm()
        {
            InitializeComponent();
        }

        private void YieldForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            mAutosize.ControlInitializeSize(this);
            Yield.Instance.SearchHourCapacityEvent += Instance_SearchHourCapacityEvent;//查询当天良率统计更新事件
            Yield.Instance.SearchClassesCapacityEvent += Instance_SearchClassesCapacityEvent;//查询最近一周良率统计
            cmb_Select.SelectedIndex = 0;

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += Instance_ChangeLanguageHandle;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
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
                toolTip1.SetToolTip(btn_Query, "产能查询");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Query, "Capacity Query");
            }
            else
            {
                toolTip1.SetToolTip(btn_Query, "Truy vấn năng lực");
            }
            #endregion

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

        private void Instance_SearchHourCapacityEvent(object sender, Dictionary<string, HourCapacity> e)
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
            }));
        }


        private void Instance_SearchClassesCapacityEvent(object sender, Dictionary<string, ClassesCapacity> e)
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

        private void YieldForm_SizeChanged(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }

        private void btn_Query_Click(object sender, EventArgs e)
        {
            DateTime data = dateTimePicker1.Value;
            string calsses = cmb_Select.Text.Trim();
            if (Yield.Instance.SearchClassesYield(data, calsses))
            {
                chart_Statist.Titles[0].Text = dateTimePicker1.Value.ToString("yyyy-MM-dd") + "-" + calsses;
                chart_Week.Titles[0].Text = dateTimePicker1.Value.ToString("yyyy-MM-dd") + "-" + calsses + "-" + "Week";
                chart_Yield.Titles[0].Text = dateTimePicker1.Value.ToString("yyyy-MM-dd") + "-" + calsses;
            }
        }
    }
}
