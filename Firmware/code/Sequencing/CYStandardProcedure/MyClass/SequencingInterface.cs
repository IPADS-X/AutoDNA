using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    /// <summary>
    /// 测序实验启动参数
    /// </summary>
    public class SequencingStartParam
    {
        /// <summary>
        /// 实验名称
        /// </summary>
        public string protocol_group_id;
        /// <summary>
        /// 测序芯片类型
        /// </summary>
        public string product_code;
        /// <summary>
        /// 样本编号
        /// </summary>
        public string sample_id;
        /// <summary>
        /// 试剂盒
        /// </summary>
        public string kit;
        /// <summary>
        /// 速度
        /// </summary>
        public int speed;
        ///// <summary>
        ///// 实验时间
        ///// </summary>
        //public int experiment_time;
        /// <summary>
        /// 最短读长
        /// </summary>
        public int min_read_length;
        /// <summary>
        /// 碱基识别模型
        /// </summary>
        public string guppy_filename;
        /// <summary>
        /// 孔扫描间隔时间
        /// </summary>
        public double mux_scan_period;
    }
    /// <summary>
    /// 测序实验ID
    /// </summary>
    public class RunID
    {
        /// <summary>
        /// 测序实验ID
        /// </summary>
        public string run_id;
    }
    /// <summary>
    /// 芯片质检ID
    /// </summary>
    public class ProductCode
    {
        /// <summary>
        /// 芯片质检ID
        /// </summary>
        public string product_code;
    }

    /// <summary>
    /// 测序仪接口
    /// </summary>
    public class SequencingInterface
    {
        private static object obj1 = new object();
        private static object obj2 = new object();
        private static object obj3 = new object();
        private static object obj4 = new object();


        /// <summary>
        /// 测序暂停
        /// </summary>
        public static string sequencing_Pause = "http://127.0.0.1:8080/sequencing/pause";
        /// <summary>
        /// 测序继续
        /// </summary>
        public static string sequencing_Continue = "http://127.0.0.1:8080/sequencing/resume";
        /// <summary>
        /// 测序停止
        /// </summary>
        public static string sequencing_Stop = "http://127.0.0.1:8080/sequencing/stop";
        /// <summary>
        /// 检查是否有芯片
        /// </summary>
        public static string sequencing_Chip = "http://127.0.0.1:8080/sequencing/chip/state";
        /// <summary>
        /// 检查网络是否连接
        /// </summary>
        public static string sequencing_Connect = "http://127.0.0.1:8080/sequencing/network";
        /// <summary>
        /// 测序当前状态
        /// </summary>
        public static string sequencing_State = "http://127.0.0.1:8080/sequencing/state";

        /// <summary>
        /// 拷贝文件
        /// </summary>
        public static string sequencing_FileCopy = "http://127.0.0.1:8080/sequencing/file/copy";

        /// <summary>
        /// 文件拷贝状态
        /// </summary>
        public static string sequencing_CopyState = "http://127.0.0.1:8080/sequencing/file/state";
        /// <summary>
        /// 查询碱基识别进度
        /// </summary>
        public static string sequencing_Basecalled = "http://127.0.0.1:8080/sequencing/basecalled/fraction";


        /// <summary>
        /// 启动测序仪
        /// </summary>
        /// <param name="code">响应码</param>
        /// <param name="runid">当前测序ID号</param>
        /// <returns></returns>
        public static bool SequencingStart(out string code, out string runid)
        {
            lock (obj1)
            {
                try
                {
                    code = "";
                    runid = "";
                    string strData = "";
                    string jsonStr = JsonConvert.SerializeObject(SerializeClass.startParam_sequencingStation);
                    HttpWebRequest Request = HttpWebRequest.CreateHttp("http://127.0.0.1:8080/sequencing/start"); //根据接口地址实例化一个http请求
                    LogToSequencing("http://127.0.0.1:8080/sequencing/start", jsonStr);//log
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
                    LogFromSequencing("http://127.0.0.1:8080/sequencing/start", strData);//log
                    if (strData != "")   //解析Json格式数据添加到字典并返回
                    {
                        string[] SplitValue = strData.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                        for (int i = 0; i < SplitValue.Length; i++)
                        {
                            string[] KeyValue = SplitValue[i].Split(':');
                            if (SplitValue[i].Contains("code"))
                            {
                                code = KeyValue[1].Trim();
                            }
                            if (SplitValue[i].Contains("run_id"))
                            {
                                runid = KeyValue[2].Trim();
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    code = "";
                    runid = "";
                    return false;
                }
            }
        }

        /// <summary>
        /// 测序暂停,继续,停止,芯片有无检查,网络检查
        /// </summary>
        /// <param name="s">接口</param>
        /// <param name="code">响应码</param>
        /// <param name="msg">信息</param>
        /// <param name="state">状态,0表示有芯片,1表示没有</param>
        /// <param name="com22">端口22连接状态,0表示连接,10056表示未连接</param>
        /// <param name="com9502">端口9502连接状态,0表示连接,10056表示未连接</param>
        /// <returns></returns>
        public static bool SequencingNoParam(string s, out string code, out string msg, out string state, out string com22, out string com9502)
        {
            lock (obj2)
            {
                try
                {
                    code = "";
                    msg = "";
                    state = "";
                    com22 = "";
                    com9502 = "";
                    string strData = "";
                    string jsonStr = "{}";
                    HttpWebRequest Request = HttpWebRequest.CreateHttp(s); //根据接口地址实例化一个http请求
                    LogToSequencing(s, jsonStr);//log
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
                    LogFromSequencing(s, strData);
                    if (strData != "")   //解析Json格式数据添加到字典并返回
                    {
                        string[] SplitValue = strData.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');
                        if (SplitValue.Length == 3)
                        {
                            for (int i = 0; i < SplitValue.Length; i++)
                            {
                                string[] KeyValue = SplitValue[i].Split(':');
                                if (SplitValue[i].Contains("code"))
                                {
                                    code = KeyValue[1].Trim();
                                }
                                if (SplitValue[i].Contains("22"))
                                {
                                    com22 = KeyValue[1].Trim();
                                }
                                if (SplitValue[i].Contains("9502"))
                                {
                                    com9502 = KeyValue[2].Trim();
                                }
                            }
                        }
                        else if (SplitValue[1].Contains("state"))
                        {
                            for (int i = 0; i < SplitValue.Length; i++)
                            {
                                string[] KeyValue = SplitValue[i].Split(':');
                                if (SplitValue[i].Contains("code"))
                                {
                                    code = KeyValue[1].Trim();
                                }
                                if (SplitValue[i].Contains("state"))
                                {
                                    state = KeyValue[2].Trim();
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < SplitValue.Length; i++)
                            {
                                string[] KeyValue = SplitValue[i].Split(':');
                                if (SplitValue[i].Contains("code"))
                                {
                                    code = KeyValue[1].Trim();
                                }
                                if (SplitValue[i].Contains("msg"))
                                {
                                    msg = KeyValue[1].Trim();
                                }
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    code = "";
                    msg = "";
                    state = "";
                    com22 = "";
                    com9502 = "";
                    return false;
                }
            }
        }


        /// <summary>
        /// 查看测序状态,文件拷贝,查看文件拷贝状态,查询碱基识别进度
        /// </summary>
        /// <param name="s">接口</param>
        /// <param name="sc">不同工位的RunID(1:查询测序状态/查询碱基识别；2:拷贝文件；3:查询拷贝文件状态)</param>
        /// <param name="code">响应码</param>
        /// <param name="data">数据</param>
        /// <param name="msg">信息</param>
        /// <param name="total_pore_count">孔活性数量</param>
        /// <returns></returns>
        public static bool SequencingState(string s, int sc, out string code, out string data, out string msg, out string total_pore_count)
        {
            lock (obj3)
            {
                try
                {
                    code = "";
                    data = "";
                    msg = "";
                    total_pore_count = "";
                    string strData = "";
                    string jsonStr = "";
                    if (sc == 1)//查询测序状态
                    {
                        jsonStr = JsonConvert.SerializeObject(SerializeClass.id_sequencingStation);
                    }
                    else if (sc == 2)//拷贝文件
                    {
                        jsonStr = JsonConvert.SerializeObject(SerializeClass.id_dataProcessingStation);
                    }
                    else if (sc == 3)//查询拷贝文件状态
                    {
                        jsonStr = JsonConvert.SerializeObject(SerializeClass.id_dataProcessingStation);
                    }
                    HttpWebRequest Request = HttpWebRequest.CreateHttp(s); //根据接口地址实例化一个http请求
                    LogToSequencing(s, jsonStr);//log
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
                    LogFromSequencing(s, strData);
                    if (strData != "")   //解析Json格式数据添加到字典并返回
                    {
                        string[] SplitValue = strData.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                        if (SplitValue.Length == 3 && SplitValue[1].Contains("percent"))
                        {
                            for (int i = 0; i < SplitValue.Length; i++)
                            {
                                string[] KeyValue = SplitValue[i].Split(':');
                                if (SplitValue[i].Contains("code"))
                                {
                                    code = KeyValue[1].Trim();
                                }
                                if (SplitValue[i].Contains("finish"))
                                {
                                    data = KeyValue[1].Trim();//获取已识别碱基数，到达数量后停止测序
                                }
                            }
                        }
                        else if (SplitValue.Length == 3)
                        {
                            for (int i = 0; i < SplitValue.Length; i++)
                            {
                                string[] KeyValue = SplitValue[i].Split(':');
                                if (SplitValue[i].Contains("code"))
                                {
                                    code = KeyValue[1].Trim();
                                }
                                if (SplitValue[i].Contains("data"))
                                {
                                    data = KeyValue[2].Trim();
                                }
                                if (SplitValue[i].Contains("total_pore_count"))
                                {
                                    total_pore_count = KeyValue[1].Trim();
                                }
                            }
                        }
                        else if (SplitValue[1].Contains("state"))
                        {
                            for (int i = 0; i < SplitValue.Length; i++)
                            {
                                string[] KeyValue = SplitValue[i].Split(':');
                                if (SplitValue[i].Contains("code"))
                                {
                                    code = KeyValue[1].Trim();
                                }
                                if (SplitValue[i].Contains("state"))
                                {
                                    data = KeyValue[2].Trim();
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < SplitValue.Length; i++)
                            {
                                string[] KeyValue = SplitValue[i].Split(':');
                                if (SplitValue[i].Contains("code"))
                                {
                                    code = KeyValue[1].Trim();
                                }
                                if (SplitValue[i].Contains("msg"))
                                {
                                    msg = KeyValue[1].Trim();
                                }
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    code = "";
                    data = "";
                    msg = "";
                    total_pore_count = "";
                    return false;
                }
            }
        }

        /// <summary>
        /// 芯片质检接口
        /// </summary>
        /// <param name="code">响应码</param>
        /// <param name="runid">当前质检ID</param>
        /// <returns></returns>
        public static bool ChipInspection(out string code, out string runid)
        {
            lock (obj4)
            {
                try
                {
                    code = "";
                    runid = "";
                    string strData = "";
                    //mProductCode.product_code = "FLO-MIN114";
                    string jsonStr = JsonConvert.SerializeObject(SerializeClass.mProductCode);
                    HttpWebRequest Request = HttpWebRequest.CreateHttp("http://127.0.0.1:8080/sequencing/chip/check"); //根据接口地址实例化一个http请求
                    LogToSequencing("http://127.0.0.1:8080/sequencing/chip/check", jsonStr);//log
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
                    LogFromSequencing("http://127.0.0.1:8080/sequencing/chip/check", strData);//log
                    if (strData != "")   //解析Json格式数据添加到字典并返回
                    {
                        string[] SplitValue = strData.Replace('"', ' ').Replace('{', ' ').Replace('}', ' ').Split(',');

                        for (int i = 0; i < SplitValue.Length; i++)
                        {
                            string[] KeyValue = SplitValue[i].Split(':');
                            if (SplitValue[i].Contains("code"))
                            {
                                code = KeyValue[1].Trim();
                            }
                            if (SplitValue[i].Contains("run_id"))
                            {
                                runid = KeyValue[2].Trim();
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    code = "";
                    runid = "";
                    return false;
                }
            }
        }
        /// <summary>
        /// 给测序仪发送指令日志
        /// </summary>
        /// <param name="http"></param>
        /// <param name="sendmsg"></param>
        private static void LogToSequencing(string http, string sendmsg)
        {
            string NowDate = string.Format("{0:yyyyMMdd}", DateTime.Now);//获取当前日期
            //测序拷贝文件地址
            if (!Directory.Exists(@"E:\test\"))
            {
                Directory.CreateDirectory(@"E:\test\");
            }

            if (!Directory.Exists(@"E:\SWLog\Sequencing\"))
            {
                Directory.CreateDirectory(@"E:\SWLog\Sequencing\");
            }
            if (!File.Exists(@"E:\SWLog\Sequencing\" + NowDate + ".txt"))
            {
                File.Create(@"E:\SWLog\Sequencing\" + NowDate + ".txt").Close();
            }
            if (File.Exists(@"E:\SWLog\Sequencing\" + NowDate + ".txt"))
            {
                using (FileStream fsWrite = new FileStream(@"E:\SWLog\Sequencing\" + NowDate + ".txt", FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fsWrite, Encoding.Unicode))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  Interface Name:  " + http);
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  PC-->CeXuYi  " + sendmsg + Environment.NewLine);
                    }
                }
            }
        }
        /// <summary>
        /// 测序仪接口反馈指令日志
        /// </summary>
        /// <param name="http"></param>
        /// <param name="sendmsg"></param>
        private static void LogFromSequencing(string http, string sendmsg)
        {
            string NowDate = string.Format("{0:yyyyMMdd}", DateTime.Now);//获取当前日期
            if (!Directory.Exists(@"E:\SWLog\Sequencing\"))
            {
                Directory.CreateDirectory(@"E:\SWLog\Sequencing\");
            }
            if (!File.Exists(@"E:\SWLog\Sequencing\" + NowDate + ".txt"))
            {
                File.Create(@"E:\SWLog\Sequencing\" + NowDate + ".txt").Close();
            }
            if (File.Exists(@"E:\SWLog\Sequencing\" + NowDate + ".txt"))
            {
                using (FileStream fsWrite = new FileStream(@"E:\SWLog\Sequencing\" + NowDate + ".txt", FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fsWrite, Encoding.Unicode))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  Interface Name:  " + http);
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  CeXuYi-->PC  " + sendmsg + Environment.NewLine);
                    }
                }
            }
        }



    }
}

