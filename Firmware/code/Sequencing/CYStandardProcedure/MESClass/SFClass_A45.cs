using CYAutoFramework;
using CYStandardProcedure.WebReference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    public class SFClass_A45
    {
        private LinkSF mLinkSF;

        private string ngstr = string.Empty;

        /// <summary>
        /// 错误原因
        /// </summary>
        public string NGstr
        {
            get { return ngstr; }
            set { ngstr = value; }
        }

        /// <summary>
        /// 上传数据细分类
        /// </summary>
        public class NormalHandle
        {
            public string sn;
            public string line;
            public string station;
            public string app;
            public string fixid;
            public string barcode;
            public string p;
            public string carrier;
        }

        /// <summary>
        /// 反序列化数据
        /// </summary>
        public class deQueryStr
        {
            public string Result;
            public Info[] info;
        }

        public class Info
        {
            public string msg;
        }

        /// <summary>
        /// 嘉善立讯的ShopFloor通讯类
        /// </summary>
        public SFClass_A45()
        { }

        /// <summary>
        /// 自动上传（Query）
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool shopFloor_Query(NormalHandle normal, out string msg)
        {
            msg = string.Empty;
            ngstr = string.Empty;
            try
            {
                /***序列化对象转换成json字符型***/
                string jsonStr = JsonConvert.SerializeObject(normal);
                jsonStr = "[" + jsonStr + "]";

                //MES信息记录
                LogConfig.Instance.WriteAutoSFLog(jsonStr);

                /***请求ShopFloor获得信息***/
                string str = mLinkSF.Query(jsonStr);

                //MES信息记录
                LogConfig.Instance.WriteAutoSFLog(str);

                //只保留"msg":后面的字符
                deQueryStr de = JsonConvert.DeserializeObject<deQueryStr>(str);
                string ret = de.Result;//SF反馈结果
                Info inf = de.info[0];
                string instr = inf.msg;

                /***分离有效信息***/
                string[] str1 = instr.Split(';');
                for (int i = 0; i < str1.Length; i++)
                {
                    if (str1[i].Contains("complist"))
                    {
                        string[] str2 = str1[i].Split('=');
                        string[] str3 = str2[1].Split('|');
                        msg = str3[str3.Length - 1];
                        break;
                    }
                }
                if (msg == string.Empty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("无法连接"))
                {
                    ngstr = "网络连接失败！";
                }
                else
                {
                    ngstr = ex.ToString();
                }
                return false;
            }
        }

        /// <summary>
        /// 手动上传（Query）
        /// <param name="text"></param>
        /// <param name="msg"></param>
        /// </summary>
        public bool shopFloor_Query(string[] text, out string msg)
        {
            msg = string.Empty;
            ngstr = string.Empty;
            try
            {
                NormalHandle normal = new NormalHandle();
                normal.sn = text[0];
                normal.line = text[1];
                normal.station = text[2];
                normal.app = text[3];
                normal.fixid = text[4];
                normal.barcode = text[5];
                normal.p = text[6];
                normal.carrier = text[7];

                /***序列化对象转换成json字符型***/
                string jsonStr = JsonConvert.SerializeObject(normal);
                jsonStr = "[" + jsonStr + "]";

                //MES信息记录
                LogConfig.Instance.WriteHandSFLog(jsonStr);

                /***请求ShopFloor获得信息***/
                string str = mLinkSF.Query(jsonStr);

                //MES信息记录
                LogConfig.Instance.WriteHandSFLog(str);

                //只保留"msg":后面的字符
                deQueryStr de = JsonConvert.DeserializeObject<deQueryStr>(str);
                string ret = de.Result;//SF反馈结果
                Info inf = de.info[0];
                string instr = inf.msg;

                /***分离有效信息***/
                string[] str1 = instr.Split(';');
                for (int i = 0; i < str1.Length; i++)
                {
                    if (str1[i].Contains("complist"))
                    {
                        string[] str2 = str1[i].Split('=');
                        string[] str3 = str2[1].Split('|');
                        msg = str3[str3.Length - 1];
                        break;
                    }
                }
                if (msg == string.Empty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("无法连接"))
                {
                    ngstr = "网络连接失败！";
                }
                else
                {
                    ngstr = ex.ToString();
                }
                return false;
            }
        }

        /// <summary>
        /// 自动上传shopFloor（Add）
        /// <param name="normal"></param>
        /// <param name="msg"></param>
        /// </summary>
        public bool shopFloor_Add(NormalHandle normal, out string msg)
        {
            msg = string.Empty;
            ngstr = string.Empty;
            try
            {
                /***序列化对象转换成json字符型***/
                string jsonStr = JsonConvert.SerializeObject(normal);
                jsonStr = "[" + jsonStr + "]";

                //MES信息记录
                LogConfig.Instance.WriteAutoSFLog(jsonStr);

                /***请求ShopFloor获得信息***/
                string str = mLinkSF.ADD(jsonStr);

                //MES信息记录
                LogConfig.Instance.WriteAutoSFLog(str);

                //只保留"msg":后面的字符
                deQueryStr de = JsonConvert.DeserializeObject<deQueryStr>(str);
                string ret = de.Result;//SF反馈结果
                Info inf = de.info[0];
                string instr = inf.msg;

                /***分离有效信息***/
                string[] strArr = instr.Split(',');
                bool ngRet = true;
                if (instr.Contains("OK"))
                {
                    ngRet = false;
                }
                else
                {
                    ngRet = true;
                    msg = instr;
                }

                //for (int i = 0; i < strArr.Length; i++)
                //{
                //    if (strArr[i].Contains("checkrouting"))
                //    {
                //        string[] str123 = strArr[i].Split('=');
                //        if (str123[1] == "OK")
                //        {
                //            ngRet = false;
                //        }
                //        else if (strArr[1].Contains("NG"))
                //        {
                //            ngRet = true;
                //            Program.NGstr = strArr[1];
                //        }
                //        break;
                //    }
                //}

                if (!ngRet)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("无法连接"))
                {
                    ngstr = "网络连接失败！";
                }
                else
                {
                    ngstr = ex.ToString();
                }
                return false;
            }
        }

        /// <summary>
        /// 手动上传shopFloor（Add）
        /// <param name="text"></param>
        /// <param name="msg"></param>
        /// </summary>
        public bool shopFloor_Add(string[] text, out string msg)
        {
            msg = string.Empty;
            ngstr = string.Empty;
            try
            {
                NormalHandle normal = new NormalHandle();
                normal.sn = text[0];
                normal.line = text[1];
                normal.station = text[2];
                normal.app = text[3];
                normal.fixid = text[4];
                normal.barcode = text[5];
                normal.p = text[6];
                normal.carrier = text[7];

                /***序列化对象转换成json字符型***/
                string jsonStr = JsonConvert.SerializeObject(normal);
                jsonStr = "[" + jsonStr + "]";

                //MES信息记录
                LogConfig.Instance.WriteHandSFLog(jsonStr);

                /***请求ShopFloor获得信息***/
                string str = mLinkSF.ADD(jsonStr);

                //MES信息记录
                LogConfig.Instance.WriteHandSFLog(str);

                //只保留"msg":后面的字符
                deQueryStr de = JsonConvert.DeserializeObject<deQueryStr>(str);
                string ret = de.Result;//SF反馈结果
                Info inf = de.info[0];
                string instr = inf.msg;

                /***分离有效信息***/
                string[] strArr = instr.Split(',');
                bool ngRet = true;
                if (instr.Contains("OK"))
                {
                    ngRet = false;
                }
                else
                {
                    ngRet = true;
                    msg = instr;
                }

                //for (int i = 0; i < strArr.Length; i++)
                //{
                //    if (strArr[i].Contains("checkrouting"))
                //    {
                //        string[] str123 = strArr[i].Split('=');
                //        if (str123[1] == "OK")
                //        {
                //            ngRet = false;
                //        }
                //        else if (strArr[1].Contains("NG"))
                //        {
                //            ngRet = true;
                //            Program.NGstr = strArr[1];
                //        }
                //        break;
                //    }
                //}

                if (!ngRet)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("无法连接"))
                {
                    ngstr = "网络连接失败！";
                }
                else
                {
                    ngstr = ex.ToString();
                }
                return false;
            }
        }

        /// <summary>
        /// 分析返回数据
        /// </summary>
        /// <param name="p">查询时p的值</param>
        /// <param name="msg">分离的返回数据</param>
        /// <returns></returns>
        private bool p_split(string p, string msg)
        {
            try
            {
                if (p != "")
                {
                    string[] str1 = p.Split(',');
                    string[] str2 = msg.Split(';');

                    string[] str0 = new string[str1.Length];

                    for (int i = 0; i < str1.Length; i++)
                    {
                        for (int j = 0; j < str2.Length; j++)
                        {
                            if (str2[j].Contains(str1[i]))
                            {
                                string[] str21 = str2[j].Split('=');
                                string[] str22 = str21[1].Split('|');
                                str0[i] = str22[str22.Length - 1];
                                break;
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将string数组转换成上传数据类
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public NormalHandle stringTojsonstr(string[] text)
        {
            NormalHandle normal = new NormalHandle();
            try
            {
                normal.sn = text[0];
                normal.line = text[1];
                normal.station = text[2];
                normal.app = text[3];
                normal.fixid = text[4];
                normal.barcode = text[5];
                normal.p = text[6];
                normal.carrier = text[7];
            }
            catch { }
            return normal;
        }
    }
}
