using CYAutoFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class SequencingForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        INIFile ini = new INIFile(Application.StartupPath + "\\FileINI\\SequenceForm.ini");
        public NormalHandle mPickUpBox1 = new NormalHandle();
        public RunID_Form id_form = new RunID_Form();
        public ProductCode_Form mProductCode = new ProductCode_Form();
        private static object obj1 = new object();
        private static object obj2 = new object();
        private static object obj3 = new object();
        private static object obj4 = new object();



        public SequencingForm()
        {
            InitializeComponent();
        }
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }

        private void SequencingForm_Load(object sender, EventArgs e)
        {
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            Read();
        }

        public void Read()
        {
            try
            {
                txt_RunID.Text = ini.Read<string>("RunID", "ID");
                txt_name.Text = ini.Read<string>("SequenceFormParam", "protocol_group_id");
                txt_type.Text = ini.Read<string>("SequenceFormParam", "product_code");
                txt_num.Text = ini.Read<string>("SequenceFormParam", "sample_id");
                txt_box.Text = ini.Read<string>("SequenceFormParam", "kit");
                txt_speed.Text = ini.Read<string>("SequenceFormParam", "speed");
                txt_time.Text = ini.Read<string>("SequenceFormParam", "experiment_time");
                txt_short.Text = ini.Read<string>("SequenceFormParam", "min_read_length");
                txt_model.Text = ini.Read<string>("SequenceFormParam", "guppy_filename");
            }
            catch (Exception)
            {

            }
        }
        public void Write()
        {
            try
            {
                ini.Write("RunID", "ID", txt_RunID.Text.Trim());
                ini.Write("SequenceFormParam", "protocol_group_id", txt_name.Text.Trim());
                ini.Write("SequenceFormParam", "product_code", txt_type.Text.Trim());
                ini.Write("SequenceFormParam", "sample_id", txt_num.Text.Trim());
                ini.Write("SequenceFormParam", "kit", txt_box.Text.Trim());
                ini.Write("SequenceFormParam", "speed", txt_speed.Text.Trim());
                ini.Write("SequenceFormParam", "experiment_time", txt_time.Text.Trim());
                ini.Write("SequenceFormParam", "min_read_length", txt_short.Text.Trim());
                ini.Write("SequenceFormParam", "guppy_filename", txt_model.Text.Trim());
            }
            catch (Exception)
            {

            }

        }
        private void rbt_connect_Click(object sender, EventArgs e)
        {
            if (FormSequencingNoParam(SequencingInterface.sequencing_Connect))
            {
                MessageBox.Show("连接成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("连接失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbt_youwu_Click(object sender, EventArgs e)
        {
            if (FormSequencingNoParam(SequencingInterface.sequencing_Chip))
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void rbt_filecopy_Click(object sender, EventArgs e)
        {
            if (FormSequencingState(SequencingInterface.sequencing_FileCopy))
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbt_copystate_Click(object sender, EventArgs e)
        {
            if (FormSequencingState(SequencingInterface.sequencing_CopyState))
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbt_start_Click(object sender, EventArgs e)
        {
            if (FormSequencingStart())
            {
                Write();
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void rbt_check_Click(object sender, EventArgs e)
        {
            if (FormChipInspection())
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Write();
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbt_currentstate_Click(object sender, EventArgs e)
        {
            if (FormSequencingState(SequencingInterface.sequencing_State))
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbt_jianji_Click(object sender, EventArgs e)
        {
            if (FormSequencingState(SequencingInterface.sequencing_Basecalled))
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void rbt_pause_Click(object sender, EventArgs e)
        {
            if (FormSequencingNoParam(SequencingInterface.sequencing_Pause))
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbt_continue_Click(object sender, EventArgs e)
        {
            if (FormSequencingNoParam(SequencingInterface.sequencing_Continue))
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbt_stop_Click(object sender, EventArgs e)
        {
            if (FormSequencingNoParam(SequencingInterface.sequencing_Stop))
            {
                MessageBox.Show("发送成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("发送失败！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }






        /// <summary>
        /// 启动测序仪
        /// </summary>
        /// <returns></returns>
        public bool FormSequencingStart()
        {
            lock (obj1)
            {
                try
                {
                    rch_send.Text = "";
                    rch_receive.Text = "";
                    string strData = "";
                    mPickUpBox1.protocol_group_id = txt_name.Text.Trim();
                    mPickUpBox1.product_code = txt_type.Text.Trim();
                    mPickUpBox1.sample_id = txt_num.Text.Trim();
                    mPickUpBox1.kit = txt_box.Text.Trim();
                    mPickUpBox1.speed = int.Parse(txt_speed.Text.Trim());
                    mPickUpBox1.min_read_length = int.Parse(txt_short.Text.Trim());
                    mPickUpBox1.guppy_filename = txt_model.Text.Trim();
                    mPickUpBox1.mux_scan_period = 1.5;
                    string jsonStr = JsonConvert.SerializeObject(mPickUpBox1);
                    rch_send.Text = jsonStr;
                    HttpWebRequest Request = HttpWebRequest.CreateHttp("http://127.0.0.1:8080/sequencing/start"); //根据接口地址实例化一个http请求
                    Request.Method = "POST";  //请求方式
                    Request.Accept = "*/*";  // 接收格式
                    Request.ContentType = "application/json";   //内容类型
                                                                //Request.KeepAlive = true;    //保持链接   
                    Stream RequestValue = Request.GetRequestStream();
                    using (var streamWriter = new StreamWriter(RequestValue))
                    {
                        streamWriter.Write(jsonStr);
                    }
                    RequestValue.Close();
                    HttpWebResponse Response = Request.GetResponse() as HttpWebResponse;  //实例化一个请求响应对象          
                    using (StreamReader sread = new StreamReader(Response.GetResponseStream())) //接收数据流编码解析为UTF-8格式存储到数据流中
                    {
                        strData = sread.ReadToEnd();  //读取完成后赋值                  
                    }
                    rch_receive.Text = strData;
                    if (strData != "")   //解析Json格式数据添加到字典并返回
                    {
                        string[] SplitValue = strData.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                        for (int i = 0; i < SplitValue.Length; i++)
                        {
                            string[] KeyValue = SplitValue[i].Split(':');
                            if (SplitValue[i].Contains("run_id"))
                            {
                                txt_RunID.Text = KeyValue[2].Trim();
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }



        /// <summary>
        /// 测序暂停,继续,停止,芯片有无检查,网络检查
        /// </summary>
        /// <param name="s">接口</param>
        /// <returns></returns>
        public bool FormSequencingNoParam(string s)
        {
            lock (obj2)
            {
                try
                {
                    rch_send.Text = "";
                    rch_receive.Text = "";
                    string strData = "";
                    string jsonStr = "{}";
                    rch_send.Text = jsonStr;
                    HttpWebRequest Request = HttpWebRequest.CreateHttp(s); //根据接口地址实例化一个http请求
                    Request.Method = "POST";  //请求方式
                    Request.Accept = "*";  // 接收格式
                    Request.ContentType = "application/json";   //内容类型
                                                                //Request.KeepAlive = true;    //保持链接   
                    Stream RequestValue = Request.GetRequestStream();
                    using (var streamWriter = new StreamWriter(RequestValue))
                    {
                        streamWriter.Write(jsonStr);
                    }
                    RequestValue.Close();
                    HttpWebResponse Response = Request.GetResponse() as HttpWebResponse;  //实例化一个请求响应对象          
                    using (StreamReader sread = new StreamReader(Response.GetResponseStream())) //接收数据流编码解析为UTF-8格式存储到数据流中
                    {
                        strData = sread.ReadToEnd();  //读取完成后赋值                  
                    }
                    rch_receive.Text = strData;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }



        /// <summary>
        /// 查看测序状态,文件拷贝,查看文件拷贝状态,查询碱基识别进度
        /// </summary>
        /// <param name="s">接口</param>
        /// <returns></returns>
        public bool FormSequencingState(string s)
        {
            lock (obj3)
            {
                try
                {
                    rch_send.Text = "";
                    rch_receive.Text = "";
                    string strData = "";
                    id_form.run_id = txt_RunID.Text.Trim();
                    string jsonStr = JsonConvert.SerializeObject(id_form);
                    rch_send.Text = jsonStr;
                    HttpWebRequest Request = HttpWebRequest.CreateHttp(s); //根据接口地址实例化一个http请求
                    Request.Method = "POST";  //请求方式
                    Request.Accept = "*";  // 接收格式
                    Request.ContentType = "application/json";   //内容类型
                    Stream RequestValue = Request.GetRequestStream();
                    using (var streamWriter = new StreamWriter(RequestValue))
                    {
                        streamWriter.Write(jsonStr);
                    }
                    RequestValue.Close();
                    HttpWebResponse Response = Request.GetResponse() as HttpWebResponse;  //实例化一个请求响应对象          
                    using (StreamReader sread = new StreamReader(Response.GetResponseStream())) //接收数据流编码解析为UTF-8格式存储到数据流中
                    {
                        strData = sread.ReadToEnd();  //读取完成后赋值                  
                    }
                    rch_receive.Text = strData;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 芯片质检接口
        /// </summary>
        /// <returns></returns>
        public bool FormChipInspection()
        {
            lock (obj4)
            {
                try
                {
                    rch_send.Text = "";
                    rch_receive.Text = "";
                    string strData = "";
                    mProductCode.product_code = txt_type.Text.Trim();
                    string jsonStr = JsonConvert.SerializeObject(mProductCode);
                    rch_send.Text = jsonStr;
                    HttpWebRequest Request = HttpWebRequest.CreateHttp("http://127.0.0.1:8080/sequencing/chip/check"); //根据接口地址实例化一个http请求
                    Request.Method = "POST";  //请求方式
                    Request.Accept = "*/*";  // 接收格式
                    Request.ContentType = "application/json";   //内容类型
                    Stream RequestValue = Request.GetRequestStream();
                    using (var streamWriter = new StreamWriter(RequestValue))
                    {
                        streamWriter.Write(jsonStr);
                    }
                    RequestValue.Close();
                    HttpWebResponse Response = Request.GetResponse() as HttpWebResponse;  //实例化一个请求响应对象          
                    using (StreamReader sread = new StreamReader(Response.GetResponseStream())) //接收数据流编码解析为UTF-8格式存储到数据流中
                    {
                        strData = sread.ReadToEnd();  //读取完成后赋值                  
                    }
                    rch_receive.Text = strData;
                    if (strData != "")   //解析Json格式数据添加到字典并返回
                    {
                        string[] SplitValue = strData.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                        for (int i = 0; i < SplitValue.Length; i++)
                        {
                            string[] KeyValue = SplitValue[i].Split(':');
                            if (SplitValue[i].Contains("run_id"))
                            {
                                txt_RunID.Text = KeyValue[2].Trim();
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }








        public class NormalHandle
        {
            public string protocol_group_id;
            public string product_code;
            public string sample_id;
            public string kit;
            public int speed;
            public int min_read_length;
            public string guppy_filename;
            public double mux_scan_period;
        }
        public class RunID_Form
        {
            public string run_id;
        }
        public class ProductCode_Form
        {
            public string product_code;
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            JianJiForm jjf = new JianJiForm();
            jjf.Show();
        }

        private void rbt_save_Click(object sender, EventArgs e)
        {
            try
            {
                Write();
                MessageBox.Show("保存成功");
            }
            catch (Exception es)
            {

            }
        }
    }
}
