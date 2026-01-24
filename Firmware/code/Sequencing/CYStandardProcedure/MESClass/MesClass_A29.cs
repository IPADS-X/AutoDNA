using CYAutoFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CYStandardProcedure
{

    /// <summary>
    /// Mes参数
    /// </summary>
    public enum _MesParam
    {
        /// <summary>
        /// 立讯MES的IP
        /// </summary>
        MesIP,
        /// <summary>
        /// 立讯MES的端口号
        /// </summary>
        MesPort,

        /// <summary>
        /// 用户ID
        /// </summary>
        user,
        /// <summary>
        /// 设备ID
        /// </summary>
        eqpCode,
        /// <summary>
        /// 制程ID（站点ID）
        /// </summary>
        processId,
        /// <summary>
        /// 条码类型
        /// </summary>
        barCodeType,
        /// <summary>
        /// panel条码
        /// </summary>
        barCodes,
        /// <summary>
        /// cover条码类型
        /// </summary>
        collectDataType,
        /// <summary>
        /// cover条码
        /// </summary>
        coverSN,
    }


    /****************************************消息操作类****************************************/
    #region 消息操作类

    public class MesInfo
    {
        /// <summary>
        /// strURL是网络地址
        /// </summary>
        public string strURL;

        /// <summary>
        /// Request是发送http请求的类
        /// </summary>
        public HttpWebRequest Request;

        /// <summary>
        /// Response是应答http的类
        /// </summary>
        public HttpWebResponse Response;

        /// <summary>
        ///  在Http协议消息头中，使用Content-Type来表示具体请求中的媒体类型信息。
        /// 例如： Content-Type: text/html;charset:utf-8;
        /// "text/html",// HTML格式
        /// "text/plain",//纯文本格式
        /// "text/xml",// XML格式
        /// "image/gif",//gif图片格式
        /// "image/jpeg",//jpg图片格式
        /// "image/png",//png图片格式
        /// "application/xhtml+xml",//XHTML格式
        /// "application/xml",//XML数据格式
        /// "application/atom+xml",//Atom XML聚合格式
        /// "application/json",// JSON数据格式
        /// "application/pdf",//pdf格式
        /// "application/msword",// Word文档格式
        /// "application/octet-stream",// 二进制流数据（如常见的文件下载）
        /// "application/x-www-form-urlencoded",// <form encType ="">中默认的encType，form表单数据被编码为key/value格式发送到服务器（表单默认的提交数据的格式）
        /// "multipart/form-data"//需要在表单中进行文件上传时，就需要使用该格式
        /// </summary>
        public void UpLoadMesInfo(string UrlPath, string Method, string Contents, string TokenValue, out string FeedbackMessage)
        {
            FeedbackMessage = string.Empty;
            try
            {
                Request = (HttpWebRequest)WebRequest.Create(UrlPath);
                Request.Method = Method;
                Request.Accept = "*/*";
                Request.KeepAlive = true;
                Request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                Request.ContentType = "application/json";
                Stream requestStream;
                if (Method == "POST")
                {
                    if (TokenValue != "")
                    {
                        Request.Headers.Add("token", TokenValue);
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(Contents);
                    Request.ContentType = "application/json";
                    Request.ContentLength = bytes.Length;

                    requestStream = Request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();

                }
                Response = (HttpWebResponse)Request.GetResponse();
                using (var StreamReader = new StreamReader(Response.GetResponseStream(), Encoding.UTF8))
                {
                    FeedbackMessage = StreamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                FeedbackMessage = "上传MES失败";
                LogConfig.Instance.ShowMessageToList("Alarm", "MES通讯失败：" + ex.Message, MsgType.Error, Color.OrangeRed);
                LogConfig.Instance.WriteAutoSFLog("MES通讯失败：" + ex.Message);
            }
            finally
            {
                LogConfig.Instance.WriteAutoSFLog("MES通讯结果：" + FeedbackMessage);
            }
        }


        /// <summary>
        /// 发送文件，图片，Json数据
        /// </summary>
        /// <param name="UrlPath">上传数据的网址</param>
        /// <param name="JsonName">json的名字，如JsonData={}</param>
        /// <param name="JsonStr">Json格式的数据信息</param>
        /// <param name="SendFilePath">发送文件的路径位置</param>
        /// <param name="FeedbackMessage">反馈信息</param>
        public void SendHttpPostFileJsonData(string UrlPath, string JsonName, string JsonStr, string[] SendFilePath, out string FeedbackMessage)
        {
            string StrDate = ""; FeedbackMessage = "";
            var MemStream = new MemoryStream();
            //时间戳
            var boundary = DateTime.Now.Ticks.ToString("X");// boundary--边界符
            var BeginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n"); // 开始结束符  
            var EndBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");//结束
            try
            {
                Request = WebRequest.Create(strURL) as HttpWebRequest;
                Request.Timeout = 2000;
                Request.Method = "POST";
                Request.ContentType = "multipart/form-data;boundary =" + boundary;//multipart/form-data模式 
                string FileHeader = "";
                for (int i = 0; i < SendFilePath.Length; i++)
                {
                    var FileStream = new FileStream(SendFilePath[i], FileMode.Open, FileAccess.Read);
                    FileHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: {2}\r\n\r\n";
                    var Header = string.Format(FileHeader, "File" + i.ToString(), SendFilePath[i].Substring(SendFilePath[i].LastIndexOf('\\') + 1), "application /ctet-stream");

                    var HeaderBytes = Encoding.UTF8.GetBytes(Header);
                    MemStream.Write(BeginBoundary, 0, BeginBoundary.Length);
                    MemStream.Write(HeaderBytes, 0, HeaderBytes.Length);

                    byte[] buffer = new byte[4096];
                    int BytesRead = 0;
                    while ((BytesRead = FileStream.Read(buffer, 0, buffer.Length)) != 0)//将文件内容写入流
                    {
                        MemStream.Write(buffer, 0, BytesRead);
                    }
                    FileStream.Close();
                    var EnterBytes = Encoding.UTF8.GetBytes("\r\n");
                    MemStream.Write(EnterBytes, 0, EnterBytes.Length);

                }

                MemStream.Write(BeginBoundary, 0, BeginBoundary.Length);
                var DataBytes = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}\r\n", JsonName, JsonStr));
                MemStream.Write(DataBytes, 0, DataBytes.Length);
                MemStream.Write(EndBoundary, 0, EndBoundary.Length);

                Request.ContentLength = MemStream.Length;
                Request.Proxy = null;
                Request.ServicePoint.Expect100Continue = false;
                Stream WriterValue = Request.GetRequestStream();

                MemStream.Position = 0;
                var BufferValue = new byte[MemStream.Length];
                MemStream.Read(BufferValue, 0, BufferValue.Length);
                MemStream.Close();



                WriterValue.Write(BufferValue, 0, BufferValue.Length);
                WriterValue.Close();
                Response = Request.GetResponse() as HttpWebResponse;
                Stream s = Response.GetResponseStream();
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    FeedbackMessage += StrDate;
                }

            }
            catch (Exception ex)
            {
                FeedbackMessage = ex.Message;
            }
        }


        /**********************************备用发送方法*******************************************/
        public string SendCommand(string UrlPath, string SendStr, out string Message)
        {
            try
            {
                //Program.WriterSN("上传值:----------->" + SendStr);
                string MES_IP = UrlPath;//获取MES接口URL地址
                HttpWebRequest Request = WebRequest.Create(MES_IP) as HttpWebRequest;
                Request.Method = "Post";
                Request.Accept = "*/*";
                Request.KeepAlive = true;
                Request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                // Encoding encoding = Encoding.UTF8;
                //string str4 = "application/x-www-form-urlencoded";
                byte[] bytes = Encoding.UTF8.GetBytes(SendStr);
                Request.ContentType = "application/json";
                Request.ContentLength = bytes.Length;
                Stream requestStream = Request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response = Request.GetResponse() as HttpWebResponse;//获取MES反馈内容
                try
                {
                    Message = "";
                    //Debug.WriteLine(encoding2);
                    using (Stream Stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(Stream, Encoding.UTF8))
                        {
                            Message += reader.ReadToEnd();
                        }
                    }
                    //Program.WriterSN("响应值:----------->" + SendStr + "\r\n");
                    return Message;
                }
                finally
                {
                    response.Close();
                }
            }
            catch (Exception exception)
            {
                Message = exception.Message;
                return Message;
            }
        }

    }
    #endregion

    /****************************************1.Trackin****************************************/
    #region 1.Trackin
    /// <summary>
    /// Trackin发送Json字符串的根类
    /// </summary>
    public class Root_TrackInSend
    {
        /// <summary>
        /// 条码
        /// </summary>
        public List<string> barCodes { get; set; }
        /// <summary>
        /// 条码类型 Panel / Carrier
        /// </summary>
        public string barCodeType { get; set; }
        /// <summary>
        /// 制程ID
        /// </summary>
        public string processId { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string eqpCode { get; set; }
        /// <summary>
        /// Rule名称：TrackIn、TrackOut、CollectData
        /// </summary>
        public string ruleName { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string user { get; set; }
    }
    #endregion

    /****************************************2.CollectData****************************************/
    #region 2.CollectData
    /// <summary>
    /// 
    /// </summary>
    public class CollectDataValues_CollectData
    {
        /// <summary>
        /// 盖板码
        /// </summary>
        public string coverSN { get; set; }
    }
    /// <summary>
    /// CollectData发送Json字符串的根类
    /// </summary>
    public class Root_CollectDataSend
    {
        /// <summary>
        /// 条码
        /// </summary>
        public List<string> barCodes { get; set; }
        /// <summary>
        /// 条码类型 Panel / Carrier
        /// </summary>
        public string barCodeType { get; set; }
        /// <summary>
        /// 制程ID
        /// </summary>
        public string processId { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string eqpCode { get; set; }
        /// <summary>
        /// Rule名称：TrackIn、TrackOut、CollectData
        /// </summary>
        public string ruleName { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CollectDataValues_CollectData collectDataValues { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string collectDataType { get; set; }
    }
    #endregion

    /****************************************3.TrackOut****************************************/
    #region 3.TrackOut
    #region 可删除
    ///// <summary>
    ///// 
    ///// </summary>
    //public class details_defects
    //{
    //    /// <summary>
    //    /// 不良位置
    //    /// </summary>
    //    public string location { get; set; }
    //    /// <summary>
    //    /// 不良代码
    //    /// </summary>
    //    public string code { get; set; }
    //}
    ///// <summary>
    ///// 
    ///// </summary>
    //public class defects_TrackOut
    //{
    //    /// <summary>
    //    /// 不良条码
    //    /// </summary>
    //    public string objectId { get; set; }
    //    /// <summary>
    //    /// 位置
    //    /// </summary>
    //    public int unitIdLocation { get; set; }
    //    /// <summary>
    //    /// 不良类型 0:Defect; 1:Pass; 2:Skip; -1:Scrap;
    //    /// </summary>
    //    public int type { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public List<string> details { get; set; }
    //}
    ///// <summary>
    ///// 
    ///// </summary>
    //public class details_toolingFixtureBindings
    //{
    //    /// <summary>
    //    /// 穴位号
    //    /// </summary>
    //    public int seq { get; set; }
    //    /// <summary>
    //    /// 绑定的条码：UnitId、PanelNo、Carrier
    //    /// </summary>
    //    public string bindingCode { get; set; }
    //    /// <summary>
    //    /// BindingType：UnitId、PanelNo、Carrier
    //    /// </summary>
    //    public string bindingType { get; set; }
    //}
    ///// <summary>
    ///// 
    ///// </summary>
    //public class toolingFixtureBindings_TrackOut
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public int sortCode { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public int enabled { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string remark { get; set; }
    //    /// <summary>
    //    /// 治具编号
    //    /// </summary>
    //    public string tfEncode { get; set; }
    //    /// <summary>
    //    /// 绑定type：-1-解绑 0-重置批量绑定 1-追加绑，2-校验
    //    /// </summary>
    //    public int bindingType { get; set; }
    //    /// <summary>
    //    /// 治具类型Carrier、Magazine、Cover、Base
    //    /// </summary>
    //    public string tfType { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public List<string> details { get; set; }
    //}
    #endregion
    /// <summary>
    /// TrackOut发送Json字符串的根类
    /// </summary>
    public class Root_TrackOutSend
    {
        /// <summary>
        /// 条码
        /// </summary>
        public List<string> barCodes { get; set; }
        /// <summary>
        /// 条码类型 Panel / Carrier
        /// </summary>
        public string barCodeType { get; set; }
        /// <summary>
        /// 制程ID
        /// </summary>
        public string processId { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string eqpCode { get; set; }
        /// <summary>
        /// Rule名称：TrackIn、TrackOut、CollectData
        /// </summary>
        public string ruleName { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string user { get; set; }
        #region 可删除
        ///// <summary>
        ///// 
        ///// </summary>
        //public List<string> defects { get; set; }
        ///// <summary>
        ///// 不良位置
        ///// </summary>
        //public List<string> toolingFixtureEncodes { get; set; }
        ///// <summary>
        ///// 治具编号绑定对象
        ///// </summary>
        //public toolingFixtureBindings_TrackOut toolingFixtureBindings { get; set; }
        #endregion
    }
    #endregion


    /// <summary>
    /// MES通讯类（昆山立讯白泽系统）
    /// </summary>
    public class MesBaizeClass
    {
        public object obj;
        /// <summary>
        /// 昆山立讯的Mes通讯类（白泽系统）
        /// </summary>
        public MesBaizeClass()
        {
            obj = new object();
        }

        /// <summary>
        /// TrackIn交互
        /// </summary>
        /// <param name="sn">SN集合</param>
        /// <param name="errormessage">错误信息</param>
        /// <returns>true:trackin校验成功  false:trackin校验失败</returns>
        public bool TrackinCommunication(List<string> sn, out string errormessage)
        {
            lock (obj)
            {
                errormessage = "";
                try
                {
                    string url = ShopFloorForm.urlBaize;
                    string FeedbackMessage = "";
                    Root_TrackInSend root = new Root_TrackInSend();
                    root.barCodes = sn;
                    root.barCodeType = Program.MesInfoDic["TrackIn"][0];
                    root.processId = Program.MesInfoDic["TrackIn"][1];
                    root.eqpCode = Program.MesInfoDic["TrackIn"][2];
                    root.ruleName = "TrackIn";
                    root.user = Program.MesInfoDic["TrackIn"][4];

                    MesInfo mesinfo = new MesInfo();
                    LogConfig.Instance.WriteAutoSFLog("TrackIn校验上传：" + JsonConvert.SerializeObject(root, Formatting.Indented));
                    mesinfo.UpLoadMesInfo(url, "POST", JsonConvert.SerializeObject(root, Formatting.Indented), "", out FeedbackMessage);

                    if (FeedbackMessage != "" && FeedbackMessage != "上传MES失败")
                    {
                        /****************** Json格式解析方式1 **************************/
                        var test = JsonConvert.DeserializeObject(FeedbackMessage) as JObject;
                        var JsonTest = JsonConvert.DeserializeObject(test["result"].ToString()) as JObject;

                        string res_message = JsonTest["message"].ToString();
                        if (res_message == "OK")//成功响应
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "TrackIn校验成功：" + sn[0], MsgType.Success, Color.Green);
                            return true;
                        }
                        else//失败响应
                        {
                            errormessage = res_message;

                            LogConfig.Instance.ShowMessageToList("NG", "TrackIn校验失败：" + sn[0] + "-->" + errormessage, MsgType.NG, Color.Red);
                            return false;
                        }
                    }
                    else//NG
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MES通讯TrackIn异常：" + ex.Message.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// 绑定Panel SN和盖板码
        /// </summary>
        /// <param name="sn">Panel SN</param>
        /// <param name="coversn">盖板码</param>
        /// <param name="errormessage">错误信息</param>
        /// <returns></returns>
        public bool CollectDataCommunication(List<string> sn, string coversn, out string errormessage)
        {
            lock (obj)
            {
                errormessage = "";
                try
                {
                    string url = ShopFloorForm.urlBaize;
                    string FeedbackMessage = "";
                    Root_CollectDataSend root = new Root_CollectDataSend();
                    CollectDataValues_CollectData collectdata = new CollectDataValues_CollectData();
                    collectdata.coverSN = coversn;

                    root.barCodes = sn;
                    root.barCodeType = Program.MesInfoDic["CollectData"][0];
                    root.processId = Program.MesInfoDic["CollectData"][1];
                    root.eqpCode = Program.MesInfoDic["CollectData"][2];
                    root.ruleName = "CollectData";
                    root.user = Program.MesInfoDic["CollectData"][4];
                    root.collectDataValues = collectdata;
                    root.collectDataType = Program.MesInfoDic["CollectData"][5];

                    MesInfo mesinfo = new MesInfo();
                    LogConfig.Instance.WriteAutoSFLog("CollectData：Panel SN与盖板码绑定：" + JsonConvert.SerializeObject(root, Formatting.Indented));
                    mesinfo.UpLoadMesInfo(url, "POST", JsonConvert.SerializeObject(root, Formatting.Indented), "", out FeedbackMessage);

                    if (FeedbackMessage != "" && FeedbackMessage != "上传MES失败")
                    {
                        /****************** Json格式解析方式1 **************************/
                        var test = JsonConvert.DeserializeObject(FeedbackMessage) as JObject;
                        var JsonTest = JsonConvert.DeserializeObject(test["result"].ToString()) as JObject;
                        string res_message = JsonTest["message"].ToString();
                        if (res_message == "OK")//成功响应
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "Panel SN与盖板码绑定成功：" + sn[0], MsgType.Success, Color.Green);
                            return true;
                        }
                        else//失败响应
                        {
                            errormessage = res_message;

                            LogConfig.Instance.ShowMessageToList("NG", "Panel SN与盖板码绑定失败：" + sn[0] + "-->" + errormessage, MsgType.NG, Color.Red);
                            return false;
                        }
                    }
                    else//NG
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MES通讯CollectData异常：" + ex.Message.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// Trackout交互
        /// </summary>
        /// <param name="sn">SN</param>
        /// <param name="errormessage">错误信息</param>
        /// <returns></returns>
        public bool TrackOutCommunication(List<string> sn, out string errormessage)
        {
            lock (obj)
            {
                errormessage = "";
                try
                {
                    string url = ShopFloorForm.urlBaize;
                    string FeedbackMessage = "";
                    bool res = false;
                    Root_TrackOutSend root = new Root_TrackOutSend();
                    root.barCodes = sn;
                    root.barCodeType = Program.MesInfoDic["TrackOut"][0];
                    root.processId = Program.MesInfoDic["TrackOut"][1];
                    root.eqpCode = Program.MesInfoDic["TrackOut"][2];
                    root.ruleName = "TrackOut";
                    root.user = Program.MesInfoDic["TrackOut"][4];

                    MesInfo mesinfo = new MesInfo();
                    LogConfig.Instance.WriteAutoSFLog("TrackOut上传：" + JsonConvert.SerializeObject(root, Formatting.Indented));
                    mesinfo.UpLoadMesInfo(url, "POST", JsonConvert.SerializeObject(root, Formatting.Indented), "", out FeedbackMessage);

                    if (FeedbackMessage != "" && FeedbackMessage != "上传MES失败")
                    {
                        /****************** Json格式解析方式1 **************************/
                        var test = JsonConvert.DeserializeObject(FeedbackMessage) as JObject;
                        var JsonTest = JsonConvert.DeserializeObject(test["result"].ToString()) as JObject;
                        string res_message = JsonTest["message"].ToString();
                        if (res_message == "OK")//成功响应
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "TrackOut成功：" + sn[0], MsgType.Success, Color.Green);
                            res = true;
                        }
                        else//失败响应
                        {
                            errormessage = res_message;

                            LogConfig.Instance.ShowMessageToList("NG", "TrackOut失败：" + sn[0] + "-->" + errormessage, MsgType.NG, Color.Red);
                            res = false;
                        }
                    }
                    else//NG
                    {
                        res = false;
                    }
                    return res;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("MES通讯TrackOut异常：" + ex.Message.ToString());
                    return false;
                }
            }
        }

    }


    /// <summary>
    /// 旧版MES通讯类（昆山立讯）
    /// </summary>
    public class MesClass_A29
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        private string url;

        /// <summary>
        /// 昆山立讯的Mes通讯类（旧版）
        /// </summary>
        /// <param name="strurl">URL接口地址</param>
        public MesClass_A29(string strurl)
        {
            this.url = strurl;
        }

        /// <summary>
        /// 发送数据键值对字典
        /// </summary>
        private Dictionary<string, string> sendStrDic = new Dictionary<string, string>();
        /// <summary>
        /// 发送数据键值对字典属性
        /// </summary>
        public Dictionary<string, string> SendStrDic
        {
            get { return sendStrDic; }
            set { sendStrDic = value; }
        }

        /// <summary>
        /// 将键值对字典转换成json字符串
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string dicTojsonstr(Dictionary<string, string> dic)
        {
            string jsonStr = string.Empty;
            foreach (var val in dic)
            {
                if (val.Key != "terminal_id")
                {
                    jsonStr += "\"" + val.Key + "\":\"" + val.Value + "\",";
                }
                else
                {
                    jsonStr += "\"" + val.Key + "\":" + val.Value + ",";
                }
            }
            jsonStr = jsonStr.Substring(0, jsonStr.Length - 1);
            jsonStr = "{" + jsonStr + "}";
            return jsonStr;
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="SendStr">发送的数据</param>
        /// <returns>返回数据</returns>
        public string SendHttpPostJson(string SendStr)
        {
            string strData = "";
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();  //实例化字典
                HttpWebRequest Request = HttpWebRequest.CreateHttp(url); //根据接口地址实例化一个http请求
                Request.Method = "POST";  //请求方式
                Request.Accept = "*/*";  // 接收格式
                Request.ContentType = "application/json";   //内容类型
                                                            //Request.KeepAlive = true;    //保持链接   
                Stream RequestValue = Request.GetRequestStream();
                using (var streamWriter = new StreamWriter(RequestValue))
                {
                    streamWriter.Write(SendStr);
                }
                RequestValue.Close();
                HttpWebResponse Response = Request.GetResponse() as HttpWebResponse;  //实例化一个请求响应对象          
                using (StreamReader sread = new StreamReader(Response.GetResponseStream())) //接收数据流编码解析为UTF-8格式存储到数据流中
                {
                    strData = sread.ReadToEnd();  //读取完成后赋值                  
                }
            }
            catch (Exception ex)
            {
                LogConfig.Instance.WriteAutoSFLog(ex.Message.ToString());
                strData = "Error: " + ex.Message.ToString();
            }
            return strData;
        }

        /// <summary>
        /// 分析返回数据
        /// </summary>
        /// <param name="receval">返回数据</param>
        /// <param name="result">返回结果</param>
        /// <returns>true：OK|false：NG</returns>
        public bool AnalysisReceivedValue(string receval, out string result)
        {
            result = string.Empty;
            try
            {
                var JsonStr = JsonConvert.DeserializeObject(receval) as JObject;
                var JsonList = JsonConvert.DeserializeObject(JsonStr["data"].ToString()) as JObject;
                result = JsonList["message"].ToString();
                if (result == "OK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 在Http协议消息头中，使用Content-Type来表示具体请求中的媒体类型信息。
        /// 例如： Content-Type: text/html;charset:utf-8;
        /// "text/html",// HTML格式
        /// "text/plain",//纯文本格式
        /// "text/xml",// XML格式
        /// "image/gif",//gif图片格式
        /// "image/jpeg",//jpg图片格式
        /// "image/png",//png图片格式
        /// "application/xhtml+xml",//XHTML格式
        /// "application/xml",//XML数据格式
        /// "application/atom+xml",//Atom XML聚合格式
        /// "application/json",// JSON数据格式
        /// "application/pdf",//pdf格式
        /// "application/msword",// Word文档格式
        /// "application/octet-stream",// 二进制流数据（如常见的文件下载）
        /// "application/x-www-form-urlencoded",// <form encType =""></form>中默认的encType，form表单数据被编码为key/value格式发送到服务器（表单默认的提交数据的格式）
        /// "multipart/form-data"//需要在表单中进行文件上传时，就需要使用该格式
        /// </summary>

    }

}
