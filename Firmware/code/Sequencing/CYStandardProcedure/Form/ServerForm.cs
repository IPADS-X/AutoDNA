using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CYAutoFramework;

namespace CYStandardProcedure
{
    public partial class ServerForm : Form
    {
        private Dictionary<SocketServer, TreeNode> m_dictServerNode = new Dictionary<SocketServer, TreeNode>();

        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        /***提示语***/
        private ToolTip toolTip1 = new ToolTip();

        public ServerForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            SocketServerConfig.Instance.UpdateGridFromParam(dataGridView1);
            /***当窗体大小改变时候也需要重新设置标题语言***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            LoadTreeView();
            /***加载文件到DataGridView控件***/
            SocketServerConfig.Instance.UpdateGridFromParam(dataGridView1);
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += NetSetForm_LanguageChangeEvent;
        }

        private void NetSetForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);

            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(rbt_Save, "保存服务器");
                toolTip1.SetToolTip(rbt_ClearReceive, "清除接受数据");
                toolTip1.SetToolTip(rbt_ClearSend, "清除发送数据");
                toolTip1.SetToolTip(rbt_Send, "发送数据");
                toolTip1.SetToolTip(rbt_StartServer, "服务器启停");
                toolTip1.SetToolTip(rbt_DisConnectServer, "关闭全部客户端会话");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(rbt_Save, "Save server");
                toolTip1.SetToolTip(rbt_ClearReceive, "Clear accepted data");
                toolTip1.SetToolTip(rbt_ClearSend, "Clear to send data");
                toolTip1.SetToolTip(rbt_Send, "send data");
                toolTip1.SetToolTip(rbt_StartServer, "Server start or stop");
                toolTip1.SetToolTip(rbt_DisConnectServer, "Close all client sessions");
            }
            else
            {
                toolTip1.SetToolTip(rbt_Save, "Lưu máy chủ");
                toolTip1.SetToolTip(rbt_ClearReceive, "Xóa dữ liệu được chấp nhận");
                toolTip1.SetToolTip(rbt_ClearSend, "Xóa để gửi dữ liệu");
                toolTip1.SetToolTip(rbt_Send, "gửi dữ liệu");
                toolTip1.SetToolTip(rbt_StartServer, "Máy chủ bắt đầu hoặc dừng");
                toolTip1.SetToolTip(rbt_DisConnectServer, "Đóng tất cả các phiên khách");
            }
        }

        private void LoadTreeView()
        {
            treeView1.Nodes.Clear();
            m_dictServerNode.Clear();
            foreach (var item in SocketServerConfig.Instance.m_listServers)
            {
                TreeNode node = new TreeNode();
                node.Text = string.Format("{0} {1}:{2}", item.Name, item.Address.ToString(), item.Port);
                node.Tag = item;
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
                m_dictServerNode.Add(item, node);
                treeView1.Nodes.Add(node);
                /***订阅接收数据事件***/
                item.DataReceived += OnDataReceived;
            }
            OnClientChanged(null, null);
            SocketServerConfig.Instance.ClientConnected += OnClientChanged;
            SocketServerConfig.Instance.ClientDisconnected += OnClientChanged;
        }

        private void OnDataReceived(object sender, AsyncSocketEventArgs e)
        {
            if (IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    SocketServer server = (SocketServer)sender;
                    string strData = "";
                    /***可以通过e.m_state.ClientSocket.RemoteEndPoint种包含的IP地址来判断接收的是哪个客户端发送的数据***/
                    strData += server.Name + "  [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Receive from " + e.m_state.ClientSocket.RemoteEndPoint.ToString() + "]  ";
                    strData += server.Encoding.GetString(e.m_state.RecvDataBuffer, 0, e.m_state.Length);
                    richTextBox1.AppendText(strData + Environment.NewLine);
                });
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SocketServer server = null;
            switch (e.Node.Level)
            {
                case 0:
                    server = e.Node.Tag as SocketServer;
                    break;
                case 1:
                    server = e.Node.Parent.Tag as SocketServer;
                    break;
            }
            rbt_StartServer.BackgroundImage = server.IsRunning ? Properties.Resources.图片2 : Properties.Resources.图片1;
            //rbt_StartServer.Text = server.IsRunning ? "停止服务" : "启动服务";
        }

        private void OnClientChanged(object sender, AsyncSocketEventArgs e)
        {
            if (IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    foreach (TreeNode node in treeView1.Nodes)
                    {
                        node.Nodes.Clear();
                    }
                    foreach (var item in SocketServerConfig.Instance.m_dictClients)
                    {
                        TreeNode root;
                        if (m_dictServerNode.TryGetValue(item.Key, out root))
                        {
                            foreach (var va in item.Value)
                            {
                                TreeNode child = new TreeNode();
                                child.Text = va.ClientSocket.RemoteEndPoint.ToString();
                                child.Tag = va;
                                child.ImageIndex = 1;
                                child.SelectedImageIndex = 1;
                                root.Nodes.Add(child);
                            }
                        }
                    }
                    treeView1.ExpandAll();
                });
            }
        }

        private void rbt_Save_Click(object sender, EventArgs e)
        {
            if (!SocketServerConfig.Instance.SaveCfgXML(dataGridView1))
            {
                MessageBox.Show("保存失败！");
            }
            else
            {
                LoadTreeView();
                MessageBox.Show("保存成功！");
            }
        }

        private void rbt_ClearReceive_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void rbt_ClearSend_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
        }

        private void rbt_Send_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                /***方法1发送数据（指定客户端的IP地址）***/
                if (treeView1.SelectedNode.Level == 1)
                {
                    SocketServer server = treeView1.SelectedNode.Parent.Tag as SocketServer;
                    AsyncSocketState state = treeView1.SelectedNode.Tag as AsyncSocketState;
                    //server.Send(state, richTextBox2.Text);
                    SocketServerConfig.Instance.SendAsync(state.IPAddressStr, richTextBox2.Text);
                }
                /***方法3发送数据（指定服务器的名称）***/
                if (treeView1.SelectedNode.Level == 0)
                {
                    SocketServer server = treeView1.SelectedNode.Tag as SocketServer;
                    //server.Broadcast(richTextBox2.Text);
                    SocketServerConfig.Instance.SendAsync3(server.Name, richTextBox2.Text);
                }
            }
        }

        private void rbt_DisConnectServer_Click(object sender, EventArgs e)
        {
            SocketServer server = null;
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Level == 0)
                {
                    server = treeView1.SelectedNode.Tag as SocketServer;
                    server.CloseAllClient();
                }
                else
                {
                    server = treeView1.SelectedNode.Parent.Tag as SocketServer;
                    AsyncSocketState state = treeView1.SelectedNode.Tag as AsyncSocketState;
                    server.Close(state);
                }
            }
        }

        private void rbt_StartServer_Click(object sender, EventArgs e)
        {
            SocketServer server = null;
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Level == 0)
                {
                    server = treeView1.SelectedNode.Tag as SocketServer;
                }
                else
                {
                    server = treeView1.SelectedNode.Parent.Tag as SocketServer;
                }

                if (server.IsRunning)
                {
                    server.Stop();
                }
                else
                {
                    server.Start();
                }
                rbt_StartServer.BackgroundImage = server.IsRunning ? Properties.Resources.图片2 : Properties.Resources.图片1;
                //rbt_StartServer.Text = server.IsRunning ? "停止服务" : "启动服务";
            }
            else
            {
                MessageBox.Show("Unselected server!");
            }
        }
    }
}
