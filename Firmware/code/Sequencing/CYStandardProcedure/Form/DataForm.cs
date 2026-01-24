using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CYAutoFramework;

namespace CYStandardProcedure
{
    public partial class DataForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        #region 窗体控件自适应代码
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion


        private int m_nMinute = DateTime.Now.Minute;

        private int m_nOldMinute = DateTime.Now.Minute;

        private static readonly object syslock = new object();

        private static readonly object syslock2 = new object();
        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        /// <summary>
        /// 控件TreeView树状显示文件夹的根目录路径
        /// </summary>
        private string codePath = string.Empty;
        /// <summary>
        /// 控件TreeView树状显示文件夹的根目录路径(不包含根节点的文本)
        /// </summary>
        private string codePathFront = string.Empty;

        public DataForm()
        {
            InitializeComponent();
        }

        private void DataForm_Load(object sender, EventArgs e)
        {
            //codePath = AppDomain.CurrentDomain.BaseDirectory + @"\NormalLog";
            codePath = LogConfig.Instance.treeViewRootPath;
            codePathFront = codePath.Replace(codePath.Split('\\')[codePath.Split('\\').Length - 1], "");

            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            /***订阅事件已监视文件夹内文件的变化***/
            LogConfig.Instance.LogFileMonitor("*.*", OnCreated, OnDeleted, OnRenamed, OnChanged);
            FileRefresh();
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += DataForm_LanguageChangeEvent; ;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void DataForm_LanguageChangeEvent(string strLanguage)
        {
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Prev, "上一个文件");
                toolTip1.SetToolTip(btn_Next, "下一个文件");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Prev, "Last File");
                toolTip1.SetToolTip(btn_Next, "Next file");
            }
            else
            {
                toolTip1.SetToolTip(btn_Prev, "Tập tin cuối");
                toolTip1.SetToolTip(btn_Next, "Tập tin kế");
            }
        }

        private void FileRefresh()
        {
            lock (syslock)
            {
                ListBoxClear();
                UpdateTreeView();
            }
        }

        private void ListBoxClear()
        {
            this.Invoke(new Action(() =>
            {
                list_Info.Items.Clear();
                list_File.Items.Clear();
            }));
        }

        /// <summary>
        /// 更新树状图
        /// </summary>
        private void UpdateTreeView()
        {
            this.Invoke(new Action(() =>
            {
                treeView1.Nodes.Clear();
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(codePath);
                    if (dir.Exists == true)
                    {
                        TreeNode newNode = new TreeNode(Path.GetFileNameWithoutExtension(codePath), 1, 1);
                        treeView1.Nodes.Add(newNode);
                        GetSubDirectoryNodes(newNode, codePath, false);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }));
        }

        /// <summary>
        /// 遍历子目录
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="fullName"></param>
        /// <param name="getFileNames"></param>
        private void GetSubDirectoryNodes(TreeNode parentNode, string fullName, bool getFileNames)
        {
            DirectoryInfo dir = new DirectoryInfo(fullName);
            DirectoryInfo[] subDirs = dir.GetDirectories();
            //为每一个子目录添加一个子节点
            foreach (DirectoryInfo subDir in subDirs)
            {
                //不显示隐藏文件夹
                if ((subDir.Attributes & FileAttributes.Hidden) != 0)
                {
                    continue;
                }
                TreeNode subNode = new TreeNode(subDir.Name, 1, 1);
                parentNode.Nodes.Add(subNode);
                //递归调用GetSubDirectoryNodes
                GetSubDirectoryNodes(subNode, subDir.FullName, getFileNames);
            }
            //获取目录中的文件
            if (getFileNames)
            {
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    TreeNode fileNode = new TreeNode(file.Name, 0, 0);
                    parentNode.Nodes.Add(fileNode);
                }
            }
        }

        /// <summary>
        /// 创建新文件发生
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            FileRefresh();
        }

        /// <summary>
        /// 删除文件发生
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            FileRefresh();
        }

        /// <summary>
        /// 重命名文件发生
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnRenamed(object source, FileSystemEventArgs e)
        {
            FileRefresh();
        }

        /// <summary>
        /// 文件内容改变发生(10分钟触发一次)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            m_nMinute = DateTime.Now.Minute;
            if (m_nMinute > m_nOldMinute + 10 || (m_nMinute + 50 > m_nOldMinute && m_nOldMinute > m_nMinute))
            {
                m_nOldMinute = m_nMinute;
                FileRefresh();
            }
        }

        /// <summary>
        /// 单击节点时候发生的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                lock (syslock)
                {
                    list_File.Items.Clear();
                    string[] strFile = Directory.GetFiles(codePathFront + e.Node.FullPath, "*.*");
                    foreach (string s in strFile)
                    {
                        list_File.Items.Add(Path.GetFileName(s));
                    }
                    if (list_File.Items.Count > 0)
                    {
                        list_File.SelectedIndex = 0;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 文件选项发生改变时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_File_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string strPath = codePathFront + treeView1.SelectedNode.FullPath + "\\" + list_File.SelectedItem.ToString();
                lock (syslock)
                {
                    list_Info.Items.Clear();
                    Task.Factory.StartNew(() =>
                    {
                        lock (syslock2)
                        {
                            FileInfo fi = new FileInfo(strPath);
                            if (!fi.Exists)
                            {
                                return;
                            }
                            try
                            {
                                list_Info.Invoke(new Action(() =>
                                {
                                    list_Info.Items.AddRange(File.ReadAllLines(strPath));
                                }));
                            }
                            catch { }
                        }
                    });
                }
            }
            catch { }
        }

        /// <summary>
        /// List选项向上移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Prev_Click(object sender, EventArgs e)
        {
            lock (syslock)
            {
                if (list_File.Items.Count > 0)
                {
                    int n = list_File.SelectedIndex;
                    if (n == 0)
                    {
                        list_File.SelectedIndex = list_File.Items.Count - 1;
                    }
                    else
                    {
                        list_File.SelectedIndex = n - 1;
                    }
                }
            }
        }

        /// <summary>
        /// List选项向下移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Next_Click(object sender, EventArgs e)
        {
            lock (syslock)
            {
                if (list_File.Items.Count > 0)
                {
                    int n = list_File.SelectedIndex + 1;
                    if (n >= list_File.Items.Count)
                    {
                        list_File.SelectedIndex = 0;
                    }
                    else
                    {
                        list_File.SelectedIndex = n;
                    }
                }
            }
        }

    }
}
