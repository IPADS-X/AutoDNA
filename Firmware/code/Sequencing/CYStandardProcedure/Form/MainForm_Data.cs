using CYAutoFramework;
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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CYStandardProcedure
{
    public partial class MainForm_Data : Form
    {
        public static MainForm_Data mMainForm_Data;
        //图例样式
        Legend legend2 = new Legend("#VALX");

        #region 控件窗体自适应
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            mMainForm_Data = this;
        }
        #endregion

        public MainForm_Data()
        {
            InitializeComponent();
            txt_IDNA.Enabled=false;
        }
        private void MainForm_Data_Load(object sender, EventArgs e)
        {
            /***子窗体自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
        }

        /// <summary>
        /// 画表(饼状图)
        /// </summary>
        /// <param name="a">碱基名称</param>
        /// <param name="b">数量</param>
        public void ChartDraw(List<string> a, List<double> b)
        {
            #region 饼状图
            ////标题
            //chart1.Titles.Add("数据分析");
            //chart1.Titles[0].ForeColor = Color.Blue;
            //chart1.Titles[0].Font = new Font("微软雅黑", 16f, FontStyle.Regular);
            //chart1.Titles[0].Alignment = ContentAlignment.TopCenter;
            //控件背景
            chart1.BackColor = Color.Transparent;
            //图表区背景
            chart1.ChartAreas[0].BackColor = Color.Transparent;
            chart1.ChartAreas[0].BorderColor = Color.Transparent;
            //X轴标签间距
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
            chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chart1.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 14f, FontStyle.Regular);
            chart1.ChartAreas[0].AxisX.TitleForeColor = Color.Blue;
            //X坐标轴颜色
            chart1.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a"); ;
            chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Blue;
            chart1.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);
            //X坐标轴标题
            chart1.ChartAreas[0].AxisX.Title = "数量(条)";
            chart1.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
            chart1.ChartAreas[0].AxisX.TitleForeColor = Color.Blue;
            chart1.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
            chart1.ChartAreas[0].AxisX.ToolTip = "数量(条)";
            //X轴网络线条
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");
            //Y坐标轴颜色
            chart1.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
            chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Blue;
            chart1.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);
            //Y坐标轴标题
            chart1.ChartAreas[0].AxisY.Title = "数量(条)";
            chart1.ChartAreas[0].AxisY.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
            chart1.ChartAreas[0].AxisY.TitleForeColor = Color.Blue;
            chart1.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Rotated270;
            chart1.ChartAreas[0].AxisY.ToolTip = "数量(条)";
            //Y轴网格线条
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");
            chart1.ChartAreas[0].AxisY2.LineColor = Color.Transparent;
            //背景渐变
            chart1.ChartAreas[0].BackGradientStyle = GradientStyle.None;

            chart1.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
            chart1.Series[0].Label = "#VAL";                //设置显示X Y的值    
            chart1.Series[0].LabelForeColor = Color.Blue;
            chart1.Series[0].ToolTip = "#VALX:#VAL(条)";     //鼠标移动到对应点显示数值
            chart1.Series[0].ChartType = SeriesChartType.Pie;    //图类型(折线)

            chart1.Series[0].Color = Color.Lime;
            chart1.Series[0].LegendText = legend2.Name;
            chart1.Series[0].IsValueShownAsLabel = true;
            chart1.Series[0].LabelForeColor = Color.Blue;
            chart1.Series[0].CustomProperties = "DrawingStyle = Cylinder";
            chart1.Series[0].CustomProperties = "PieLabelStyle = Outside";
            chart1.Legends[0].Position.Auto = true;
            chart1.Series[0].IsValueShownAsLabel = true;
            //是否显示图例
            chart1.Series[0].IsVisibleInLegend = true;
            chart1.Series[0].ShadowOffset = 0;

            //饼图折线
            chart1.Series[0]["PieLineColor"] = "Black";
            //绑定数据
            chart1.Series[0].Points.DataBindXY(a, b);
            chart1.Series[0].Points[0].Color = Color.Tomato;
            //绑定颜色
            chart1.Series[0].Palette = ChartColorPalette.BrightPastel;
            chart1.Series["Series1"].Label = "#PERCENT{P2}";
            //百分比字体大小
            chart1.Series["Series1"].Font = new Font("微软雅黑", 18f, FontStyle.Regular);
            chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;//开启三维模式;PointDepth:厚度BorderWidth:边框宽
            chart1.ChartAreas["ChartArea1"].Area3DStyle.Rotation = 15;//起始角度
            chart1.ChartAreas["ChartArea1"].Area3DStyle.Inclination = 45;//倾斜度(0～90)
            chart1.ChartAreas["ChartArea1"].Area3DStyle.LightStyle = LightStyle.Realistic;//表面光泽度
            #endregion
        }

        private void cbx_barcode_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_JianJiMsg.Text = "";
            MyVariable.AutoJianJiList.Clear();
            MyVariable.AutoNumList.Clear();
            foreach (var item in MyVariable.AutoAllJianJiDics[cbx_barcode.SelectedItem.ToString()])
            {
                txt_JianJiMsg.Text += item.Key + " : " + item.Value + Environment.NewLine;
            }
            foreach (var item in MyVariable.AutoJianJiDicsMost[cbx_barcode.SelectedItem.ToString()])
            {
                MyVariable.AutoJianJiList.Add(item.Key);
                MyVariable.AutoNumList.Add(item.Value);
            }
            if (MyVariable.AutoJianJiList.Count == 0)
            {
                MyVariable.AutoJianJiList.Add("Fail");
                MyVariable.AutoNumList.Add(1);
            }
            ChartDraw(MyVariable.AutoJianJiList, MyVariable.AutoNumList);
            if (MyVariable.JianJiDic.ContainsKey(Convert.ToInt32(cbx_barcode.SelectedItem.ToString().Replace("barcode", ""))))
            {
                lab_ZongKongJJ.Text = "总控传来碱基：    " + MyVariable.JianJiDic[Convert.ToInt32(cbx_barcode.SelectedItem.ToString().Replace("barcode", ""))];
            }
            else
            {
                lab_ZongKongJJ.Text = "此标签为残留碱基" ;
            }
            lab_jianjiMax.Text = "测序结果(饼状图中百分比最高的碱基)：    " + MyVariable.AutoJianJiList.First();
        }







    }
}
