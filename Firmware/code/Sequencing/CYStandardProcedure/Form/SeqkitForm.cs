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

namespace CYStandardProcedure
{
    public partial class SeqkitForm : Form
    {
        string IDNA_string = "ATCAGTACGGTGCACCACCATGAA";
        string JianJi_string = "CA";
        string infer = "";
        int infercount = 0;
        readonly string SeqkitExePath = "C:/Windows/System32/seqkit.exe";
        ResultFolderVM resultFolderModel = null;
        public static SeqkitForm mseqkitForm;
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


        #region 控件窗体自适应
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion

        public SeqkitForm()
        {
            InitializeComponent();
            this.txtFolderPath.Enabled = false;
            this.txtIDNA.Text = IDNA_string;
            this.txt_JianJi.Text = JianJi_string;
            mseqkitForm = this;
        }
        private void SeqkitForm_Load(object sender, EventArgs e)
        {
            /***子窗体自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
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
                SeqkitHelper.MatcheAsTxt(IDNA_string, item.FilePath, item.MatchedTxtPath);
                item.OriginalCount = SeqkitHelper.GetOriginalCount(item.FilePath);
                item.MatchedCount = SeqkitHelper.GetMatcheCount(IDNA_string, item.FilePath);
                item.DNAMatchedCount = SeqkitHelper.GetDNAMatcheCount(item.MatchedTxtPath, IDNA_string, JianJi_string);
                SeqkitHelper.JianJiInfer(5, IDNA_string, item.MatchedTxtPath, out infer, out infercount);
                item.JianJiInfer = infer;
                item.JianJiInferCount = infercount;
                if (JianJi_string== item.JianJiInfer)
                {
                    item.JianJiInferResult = "是";
                }
                else
                {
                    item.JianJiInferResult = "否";
                }
                if (item.MatchedTxtPath.Contains("pass") && item.MatchedTxtPath.Contains("fastq"))
                {
                    TotalDNACount = TotalDNACount + item.OriginalCount;
                    MatchDNACount = MatchDNACount + item.MatchedCount;
                    MatchJianJiCount = MatchJianJiCount + item.DNAMatchedCount;
                }
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

        DataTable DT = new DataTable();
        void ToDataRow(ResultFolderVM model, DataRow baseRow)
        {

            baseRow[GetFolderColumnName(model.FolderLevel)] = model.FolderName;

            if (model.FileList.Count == 0 && model.SubFolderList.Count == 0)
            {
                DataRow row = DT.NewRow();
                for (int i = 0; i < MaxFolderLevel; i++)
                {
                    row[GetFolderColumnName(i)] = baseRow[GetFolderColumnName(i)];
                }
                DT.Rows.Add(row);
            }
            else
            {
                foreach (var item in model.FileList)
                {
                    DataRow row = DT.NewRow();
                    for (int i = 0; i < MaxFolderLevel; i++)
                    {
                        row[GetFolderColumnName(i)] = baseRow[GetFolderColumnName(i)];
                    }
                    //有效文件数量	文件序号		
                    row["文件名称"] = item.FileName;
                    row["链条数"] = item.OriginalCount;
                    row["其中iDNA正确配对条数"] = item.MatchedCount;
                    row["要查询的碱基"] = JianJi_string;
                    row["iDNA和查询碱基正确配对条数"] = item.DNAMatchedCount;
                    row["推测碱基"] = item.JianJiInfer;
                    row["推测碱基存在数量"] = item.JianJiInferCount;
                    row["推测是否正确"] = item.JianJiInferResult;
                    DT.Rows.Add(row);
                }
                foreach (var item in model.SubFolderList)
                {
                    ToDataRow(item, baseRow);
                }
            }
        }

        string GetFolderColumnName(int level)
        {
            return "文件夹" + (level == 0 ? "" : level + 1 + "级");
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            if (IsProgress) { MessageBox.Show("正在处理中!"); return; };
            try
            {
                //if (!File.Exists(SeqkitExePath)) { MessageBox.Show(@"请先将 seqkit.exe 放置在 C:\Windows\System32"); return; }
                if (resultFolderModel == null) { MessageBox.Show("请选择文件夹！"); return; }
                IDNA_string = this.txtIDNA.Text.Trim();
                JianJi_string = this.txt_JianJi.Text.Trim();
                if (string.IsNullOrEmpty(IDNA_string)) { MessageBox.Show("请输入iDNA！"); return; }
                //if (string.IsNullOrEmpty(JianJi_string)) { MessageBox.Show("请输入要查询的碱基！"); return; }

                dgvResult.Columns.Clear();
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
            DT = new DataTable();
            for (int i = 0; i < MaxFolderLevel; i++)
            {
                DT.Columns.Add(new DataColumn() { ColumnName = GetFolderColumnName(i), DataType = typeof(string) });
            }
            DT.Columns.Add(new DataColumn() { ColumnName = "文件名称", DataType = typeof(string) });
            DT.Columns.Add(new DataColumn() { ColumnName = "链条数", DataType = typeof(int) });
            DT.Columns.Add(new DataColumn() { ColumnName = "其中iDNA正确配对条数", DataType = typeof(int) });
            DT.Columns.Add(new DataColumn() { ColumnName = "要查询的碱基", DataType = typeof(string) });
            DT.Columns.Add(new DataColumn() { ColumnName = "iDNA和查询碱基正确配对条数", DataType = typeof(int) });
            DT.Columns.Add(new DataColumn() { ColumnName = "推测碱基", DataType = typeof(string) });
            DT.Columns.Add(new DataColumn() { ColumnName = "推测碱基存在数量", DataType = typeof(int) });
            DT.Columns.Add(new DataColumn() { ColumnName = "推测是否正确", DataType = typeof(string) });

            ToDataRow(resultFolderModel, DT.NewRow());

            Action action = () =>
            {
                lab_totalDNA.Text = TotalDNACount.ToString();
                lab_matchDNA.Text = MatchDNACount.ToString();
                lab_matchJianJi.Text = MatchJianJiCount.ToString();
                dgvResult.DataSource = new BindingSource() { DataSource = DT };
                lblExecMsg.Text = $"执行成功！";
                lblExecMsg.BackColor = Color.White;
            };
            Invoke(action);
            IsProgress = false;
        }


    }
}
