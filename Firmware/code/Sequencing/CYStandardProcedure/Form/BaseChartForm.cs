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
    public partial class BaseChartForm : Form
    {
        string IDNA_string = "ATCAGTACGGTGCACCACCATGAA";
        string JianJi_string = "CA";
        string infer = "";
        int infercount = 0;
        readonly string SeqkitExePath = "C:/Windows/System32/seqkit.exe";
        ResultFolderVM resultFolderModel = null;
        public static BaseChartForm mBaseChartForm;
        /// <summary>
        /// 文件夹层级
        /// </summary>
        int MaxFolderLevel = 0;
        /// <summary>
        /// 总文件计数
        /// </summary>
        int TotalFileCount = 0;
        /// <summary>
        /// 已匹配文件计数
        /// </summary>
        int MatchedFileCount = 0;
        Thread SearchThread;
        bool IsProgress = false;
        /// <summary>
        /// 链条数
        /// </summary>
        int TotalDNACount = 0;
        /// <summary>
        /// 正确配对数
        /// </summary>
        int MatchDNACount = 0;
        /// <summary>
        /// 正确配对数
        /// </summary>
        int MatchJianJiCount = 0;
        //图例样式
        Legend legend2 = new Legend("#VALX");


        #region 控件窗体自适应
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion

        public BaseChartForm()
        {
            InitializeComponent();
            this.txtIDNA.Text = IDNA_string;
            this.txtFolderPath.Enabled = false;
            this.lab_jianjiMax.Text = "测序结果(饼状图中百分比最高的碱基)：";
            mBaseChartForm = this;
        }
        private void BaseChartForm_Load(object sender, EventArgs e)
        {
            /***子窗体自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

        }
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            //btnSelectFolder.Visible = false;
            //chart1.Series.Clear();  // 清空图表中的所有数据系列
            //chart1.Titles.Clear();  // 清空图表的标题
            //chart1.ChartAreas.Clear();  // 清空图表区域
            //chart1.Legends.Clear();  // 清空图例
            //chart1.Annotations.Clear();  // 清空注释
            //if (!File.Exists(SeqkitExePath)) { MessageBox.Show(@"请先将 seqkit.exe 放置在 C:\Windows\System32"); return; }
            if (IsProgress) { MessageBox.Show("正在处理中!"); return; };
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "请选择文件夹";

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string folderPath = this.txtFolderPath.Text = folderDialog.SelectedPath;
                    string[] subdirectories = Directory.GetDirectories(folderPath);
                    TotalFileCount = 0;
                    resultFolderModel = new ResultFolderVM()
                    {
                        FolderLevel = 0,
                        FolderPath = folderPath,
                        FolderName = new DirectoryInfo(folderPath).Name,
                        FileList = SeqkitHelper.GetFileList(folderPath),
                        SubFolderList = GetSubFolderList(folderPath, 1)
                    };
                    TotalFileCount += resultFolderModel.FileList.Count;
                    this.lblCount.Text = $"测序文件{TotalFileCount}个";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// 递归查找文件夹及文件
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="folerLevel"></param>
        /// <returns></returns>
        List<ResultFolderVM> GetSubFolderList(string basePath, int folerLevel)
        {
            var modelList = new List<ResultFolderVM>();
            foreach (var folderPath in Directory.GetDirectories(basePath))
            {
                var model = new ResultFolderVM()
                {
                    FolderLevel = folerLevel,
                    FolderPath = folderPath,
                    FolderName = new DirectoryInfo(folderPath).Name,
                    FileList = SeqkitHelper.GetFileList(folderPath),
                    SubFolderList = GetSubFolderList(folderPath, folerLevel + 1)
                };
                TotalFileCount += model.FileList.Count();
                modelList.Add(model);
            }
            if (folerLevel > MaxFolderLevel)
            {
                MaxFolderLevel = folerLevel;
            }
            return modelList;
        }

        /// <summary>
        /// 递归匹配处理
        /// </summary>
        /// <param name="model"></param>
        void MatchData(ResultFolderVM model)
        {
            foreach (var item in model.FileList)
            {
                if (!item.MatchedTxtPath.Contains("pass") || !item.MatchedTxtPath.Contains("fastq") || !item.MatchedTxtPath.Contains("barcode"))
                {
                    continue;
                }
                SeqkitHelper.MatcheAsTxt(IDNA_string, item.FilePath, item.MatchedTxtPath);
                item.OriginalCount = SeqkitHelper.GetOriginalCount(item.FilePath);
                item.MatchedCount = SeqkitHelper.GetMatcheCount(IDNA_string, item.FilePath);
                MyVariable.SingleJianJiDics = SeqkitHelper.SingleJianJiInfer(5, IDNA_string, item.MatchedTxtPath);
                if (!MyVariable.AllJianJiDics.ContainsKey(item.FolderName))
                {
                    MyVariable.AllJianJiDics.Add(item.FolderName, MyVariable.SingleJianJiDics);
                }
                else
                {
                    //两个字典合并成一个
                    var combinedDic = MyVariable.AllJianJiDics[item.FolderName]
                        .Concat(MyVariable.SingleJianJiDics)
                        .GroupBy(kvp => kvp.Key)
                        .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));

                    //排序
                    var sortedElementCounts = combinedDic
                        .OrderByDescending(kvp => kvp.Value)
                        .ToList();
                    combinedDic = sortedElementCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    MyVariable.AllJianJiDics[item.FolderName] = combinedDic;
                }
                TotalDNACount = TotalDNACount + item.OriginalCount;
                MatchDNACount = MatchDNACount + item.MatchedCount;
                MatchJianJiCount = MatchJianJiCount + item.DNAMatchedCount;
                MatchedFileCount++;
                Action action = () =>
                {
                    lblExecMsg.Text = $"正在处理 {MatchedFileCount}/{TotalFileCount}";
                };
                Invoke(action);
                Thread.Sleep(50);
            }
            foreach (var item in model.SubFolderList)
            {
                MatchData(item);
            }
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            // btnSelectFolder.Visible = false;
            //  btnExec.Visible = false;
            chart1.Series[0].Points.Clear(); // 清空所有数据点
            MyVariable.AllJianJiDics.Clear();
            MyVariable.JianJiDicsMost.Clear();
            cbx_barcode.Items.Clear();
            JianJiList.Clear();
            NumList.Clear();
            txt_JianJiMsg.Text = "";
            this.lab_jianjiMax.Text = "测序结果(饼状图中百分比最高的碱基)：";
            if (IsProgress) { MessageBox.Show("正在处理中!"); return; };
            try
            {
                //if (!File.Exists(SeqkitExePath)) { MessageBox.Show(@"请先将 seqkit.exe 放置在 C:\Windows\System32"); return; }
                if (resultFolderModel == null) { MessageBox.Show("请选择文件夹！"); return; }
                IDNA_string = this.txtIDNA.Text.Trim();
                if (string.IsNullOrEmpty(IDNA_string)) { MessageBox.Show("请输入iDNA！"); return; }
                //if (string.IsNullOrEmpty(JianJi_string)) { MessageBox.Show("请输入要查询的碱基！"); return; }

                TotalDNACount = 0;
                MatchDNACount = 0;
                MatchJianJiCount = 0;
                lab_totalDNA.Text = TotalDNACount.ToString();
                lab_matchDNA.Text = MatchDNACount.ToString();
                lab_matchJianJi.Text = MatchJianJiCount.ToString();
                lblExecMsg.BackColor = Color.BurlyWood;
                MatchedFileCount = 0;
                SearchThread = new Thread(new ThreadStart(SearchThreadJob));
                SearchThread.Start();
            }
            catch (Exception ex)
            {
                IsProgress = false;
                MessageBox.Show(ex.Message);
            }
        }
        void SearchThreadJob()
        {
            IsProgress = true;
            MatchData(resultFolderModel);

            //深拷贝,创建新对象
            foreach (var kvp in MyVariable.AllJianJiDics)
            {
                // 创建新的字典并复制内容
                MyVariable.JianJiDicsMost[kvp.Key] = new Dictionary<string, int>(kvp.Value);
            }
            //超过5种碱基用others代替总和
            foreach (var item in MyVariable.JianJiDicsMost)
            {
                if (MyVariable.JianJiDicsMost[item.Key].Count > 5)
                {
                    int sum = MyVariable.JianJiDicsMost[item.Key].Values.Skip(4).Sum(); // 计算第五项开始到最后的所有值的和
                    string key = "others"; // 新的键名
                                           // 删除第五项及之后的键值对
                    var keysToDelete = MyVariable.JianJiDicsMost[item.Key].Keys.Skip(4).ToList();
                    foreach (var k in keysToDelete)
                    {
                        MyVariable.JianJiDicsMost[item.Key].Remove(k);
                    }
                    MyVariable.JianJiDicsMost[item.Key][key] = sum; // 更新字典，键为"others"，值为sum
                }
            }

            Action action = () =>
            {
                foreach (var item in MyVariable.AllJianJiDics)
                {
                    cbx_barcode.Items.Add(item.Key);
                }
                if (cbx_barcode.Items.Count != 0)
                {
                    cbx_barcode.SelectedIndex = 0;
                }
                lab_totalDNA.Text = TotalDNACount.ToString();
                lab_matchDNA.Text = MatchDNACount.ToString();
                lblExecMsg.Text = $"执行成功！";
                lblExecMsg.BackColor = Color.White;
            };
            Invoke(action);
            IsProgress = false;
        }


        List<string> JianJiList = new List<string>();
        List<double> NumList = new List<double>();
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
            JianJiList.Clear();
            NumList.Clear();
            foreach (var item in MyVariable.AllJianJiDics[cbx_barcode.SelectedItem.ToString()])
            {
                txt_JianJiMsg.Text += item.Key + " : " + item.Value + Environment.NewLine;
            }
            foreach (var item in MyVariable.JianJiDicsMost[cbx_barcode.SelectedItem.ToString()])
            {
                JianJiList.Add(item.Key);
                NumList.Add(item.Value);
            }
            if (JianJiList.Count == 0)
            {
                JianJiList.Add("Fail");
                NumList.Add(1);
            }
            ChartDraw(JianJiList, NumList);
            lab_jianjiMax.Text = "测序结果(饼状图中百分比最高的碱基)：    " + JianJiList.First();
        }
    }
}
