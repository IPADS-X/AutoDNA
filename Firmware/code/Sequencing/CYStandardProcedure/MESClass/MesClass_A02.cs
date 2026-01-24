using CYAutoFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFSATPortal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CYStandardProcedure
{
    public class MesClass_A02
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        private string url;

        private string m_SaveCSVResult = LogConfig.Instance.CustomizedRootPath + @"\csvResult";

        private string m_SavePicResult = LogConfig.Instance.CustomizedRootPath + @"\picResult";

        /// <summary>
        /// 上海广达的Mes通讯类
        /// </summary>
        /// <param name="strurl">URL接口地址</param>
        public MesClass_A02(string strurl)
        {
            this.url = strurl;
        }

        /// <summary>
        /// 上传数据字典
        /// </summary>
        private Dictionary<string, string> sendstr = new Dictionary<string, string>();

        /// <summary>
        /// 上传数据字典属性
        /// </summary>
        public Dictionary<string, string> SendStrDic
        {
            get { return sendstr; }
            set { sendstr = value; }
        }

        /// <summary>
        /// 字典的键值对转换成Json格式的字符串
        /// </summary>
        /// <param name="dic">上传数据字典</param>
        /// <param name="jsonStr">Json格式的字符串</param>
        /// <returns></returns>
        public bool dicToJson(Dictionary<string, string> dic, out string jsonStr)
        {
            jsonStr = string.Empty;
            string splitstr = ";$;";//分割符
            try
            {
                foreach(var val in dic)
                {
                    jsonStr += val.Key + "=" + val.Value + splitstr;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 分析返回数据（供参考）
        /// </summary>
        /// <param name="receval">返回数据</param>
        /// <returns></returns>
        public bool AnalysisReceivedValue(string receval)
        {
            try
            {
                #region 方法一
                string str0 = "{\"cus_mac\": \"\", \"esp_mac\": \"2c3ae8080000\", \"print_times\": 1,\"status\": 200,\"test_result\": \"success\", \"testdata\": {" +
                     "\"id\": 18106055, \"created\": \"2018-01-09 10:26:15\", \"updated\": \"2021 - 03 - 18 14:49:27\", \"visibly\": true,\"module_id\": 0,\"device_type\": \"WROOM02\", \"fw_ver\": \"\", \"esp_mac\": \"2c3ae8080000\", \"cus_mac\": \"\"," +
      "\"iot_num\": \"\", \"flash_id\": \"\", \"test_result\": \"success\",\"test_msg\": \"\",\"factory_sid\": \"esp-fae-test-a95342f3\", \"batch_sid\": \"6a5fbb0d43\", \"efuse\": \"\",\"query_times\": 1,\"print_times\": 1, \"batch_index\": 2, \"latest\": true, \"is_commit\":false},\"JsonData\":[{\"id\":\"1866596\"},{\"ids\":\"1866596\"}],}";
                var test = JsonConvert.DeserializeObject(str0) as JObject;
                MessageBox.Show(test["esp_mac"].ToString() + "   " + test["print_times"].ToString() + "   " + test["status"].ToString());
                #endregion

                #region 方法二
                string srs = "{\"batchs\": [{\"id\": 8151, \"created\": \"2021 - 03 - 08 19:02:55\", \"updated\": \"2021 - 03 - 17 21:23:24\", \"visibly\": true, \"sid\": \"cd5207472a\", \"factory_sid\":\"Luxshare-c6e90251\", \"name\": \"ESP32-WROOM-32D_32Mb_Flash\", \"desc\":\"PW-2021-01-0387\", \"cnt\": 208000, \"remain\": 204702, \"esp_mac_from\": \"\"," +
                 "\"esp_mac_to\": \"\", \"cus_mac_from\": \"\", \"cus_mac_to\": \"\", \"esp_mac_num_from\": 0,\"esp_mac_num_to\": 0, \"cus_mac_num_from\": 0, \"cus_mac_num_to\": 0, \"is_cus\": false,\"success\": 3278, \"right_first_time\": 2658, \"failed\": 633, \"rejected\": 13, \"statsed\":\"2021-03-16 14:12:09\", \"print_num\": 3006}], \"status\": 200, \"total\": 1}";
                //使用Newtonsoft.Json解析"[]"格式问题
                var JsonStr = JsonConvert.DeserializeObject(srs) as JObject;
                var JsonList = JsonConvert.DeserializeObject(JsonStr["batchs"].ToString());
                JArray lstJson = JArray.Parse(JsonList.ToString());
                MessageBox.Show(((JObject)lstJson[0])["id"].ToString());
                #endregion

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="JsonStr">Json格式的字符串</param>
        /// <returns></returns>
        public string PostResponse(string JsonStr)
        {
            string strValue = "", StrDate = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 2000;
                var MemStream = new MemoryStream(); // 边界符  
                var boundary = DateTime.Now.Ticks.ToString("X");// 边界符  
                var BeginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n"); // 最后的结束符  
                var EndBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
                request.Method = "POST";
                request.ContentType = "multipart/form-data;boundary=" + boundary;//multipart/form-data模式

                /********上传压力传感值的数据文件*********/
                var fileStream = new FileStream(m_SaveCSVResult, FileMode.Open, FileAccess.Read);
                const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: {2}\r\n\r\n";
                //var header = string.Format(filePartHeader, "file1", "file","application /octet-stream");
                var header = string.Format(filePartHeader, "file1", m_SaveCSVResult, "application /ctet-stream");

                var headerbytes = Encoding.UTF8.GetBytes(header);
                MemStream.Write(BeginBoundary, 0, BeginBoundary.Length);
                MemStream.Write(headerbytes, 0, headerbytes.Length);

                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)//将文件内容写入流
                {
                    MemStream.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
                var enterbytes = Encoding.UTF8.GetBytes("\r\n");
                MemStream.Write(enterbytes, 0, enterbytes.Length);


                /********把压合好的图片值截图下来*********/
                const string filePartHeader2 = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: {2}\r\n\r\n";
                var header2 = string.Format(filePartHeader2, "file2", m_SavePicResult.Substring(m_SavePicResult.LastIndexOf('\\') + 1), "application /ctet-stream");
                var headerbytes2 = Encoding.UTF8.GetBytes(header2);

                //加了一下字段
                FileStream fs = new FileStream(m_SavePicResult, FileMode.Open, FileAccess.Read);
                byte[] bArr = new byte[fs.Length];
                fs.Read(bArr, 0, bArr.Length);
                fs.Close();
                //加了以上字段

                MemStream.Write(BeginBoundary, 0, BeginBoundary.Length);
                MemStream.Write(headerbytes2, 0, headerbytes2.Length);
                MemStream.Write(bArr, 0, bArr.Length);//加了此字段

                var enterbytes2 = Encoding.UTF8.GetBytes("\r\n");
                MemStream.Write(enterbytes2, 0, enterbytes2.Length);



                /********上传JSON字符串*********/
                MemStream.Write(BeginBoundary, 0, BeginBoundary.Length);
                var DataBytes = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}\r\n", "jsondata", JsonStr));
                //Comvaria.tabelname = newdata;
                MemStream.Write(DataBytes, 0, DataBytes.Length);
                MemStream.Write(EndBoundary, 0, EndBoundary.Length);

                request.ContentLength = MemStream.Length;
                request.Proxy = null;
                request.ServicePoint.Expect100Continue = false;
                Stream WriterValue = request.GetRequestStream();

                MemStream.Position = 0;
                var BufferValue = new byte[MemStream.Length];
                MemStream.Read(BufferValue, 0, BufferValue.Length);
                MemStream.Close();



                WriterValue.Write(BufferValue, 0, BufferValue.Length);
                WriterValue.Close();
                HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                Stream s = response.GetResponseStream();
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate;
                }

            }
            catch (Exception ex)
            {
                LogConfig.Instance.WriteAutoSFLog(ex.Message.ToString());
                strValue = "";
            }
            return strValue;
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


        #region 引用dll MES通讯
        /// <summary>
        /// 上海广达的Mes通讯类(dll)
        /// </summary>
        private Portal mSFPortal = new Portal();//实例化SF对象

        /// <summary>
        /// 与SF通讯查询信息
        /// </summary>
        /// <param name="station">站别</param>
        /// <param name="sendStr">上传数据</param>
        /// <returns></returns>
        public bool SendSFRequest(string station, string sendStr)
        {
            try
            {
                string step = "Request";

                //MES信息记录
                LogConfig.Instance.WriteAutoSFLog(sendStr);

                /***请求ShopFloor获得信息***/
                string receStr = mSFPortal.ATPortal(station, step, sendStr);
                //string receStr = mSFPortal.ATPortal(Program.m_SFStation, "Request", "SN=" + Program.m_SN + ";$;Station=" + Program.m_SFStation + ";$;Line=" + Program.m_SFLine);
                
                //MES信息记录
                LogConfig.Instance.WriteAutoSFLog(receStr);

                /***分离有效信息***/
                if (receStr.Contains("Pass"))
                {
                    string[] str0 = receStr.Split(new string[] { "<UnitSN>" }, StringSplitOptions.RemoveEmptyEntries);
                    string validinfo = str0[1].Split(new string[] { "</UnitSN>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    return true;
                }
                else if (receStr.Contains("Fail"))
                {
                    return false;
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
        /// 与SF通讯上传结果
        /// </summary>
        /// <param name="station">站别</param>
        /// <param name="sendStr">上传数据</param>
        /// <returns></returns>
        public bool SendSFResult(string station, string sendStr)
        {
            try
            {
                string step = "Result";

                //MES信息记录
                LogConfig.Instance.WriteAutoSFLog(sendStr);

                /***请求ShopFloor获得信息***/
                string receStr = mSFPortal.ATPortal(station, step, sendStr);
                //string receStr = mSFPortal.ATPortal(Program.m_SFStation, "Result", "UnitSN=" + Program.m_UnitSN + ";$;SN=" + Program.m_SN + ";$;Result=Pass;$;Station=" + Program.m_SFStation + ";$;Line=" + Program.m_SFLine);
                //string receStr = mSFPortal.ATPortal(Program.m_SFStation, "Result", "UnitSN=" + Program.m_UnitSN + ";$;SN=" + Program.m_SN + ";$;Result=Fail;$;Station=" + Program.m_SFStation + ";$;Line=" + Program.m_SFLine);

                //MES信息记录
                LogConfig.Instance.WriteAutoSFLog(receStr);

                /***分离有效信息***/
                if (receStr.Contains("Pass"))
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
        #endregion

    }
}
