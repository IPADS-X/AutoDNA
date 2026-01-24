using System;
using System.Collections.Generic;
using CYStandardProcedure.WebReference;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
using CYAutoFramework;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace CYStandardProcedure
{
    public class SFClass_A38
    {
        /// <summary>
        /// 成都富士康的ShopFloor通讯类
        /// </summary>
        public SFClass_A38()
        { }

        /// <summary>
        /// 拼接SF查询信息   checkOrupdate = 1 为查询， 传入2 为上传 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="checkOrupdate"></param>
        /// <returns></returns>
        public string SF_MessageADD(SF_message msg, int checkOrupdate)
        {
            msg.url = msg.url;
            msg.cQuery = "c=" + msg.cQuery + "&";
            msg.line = "line=" + msg.line + "&";
            msg.station = "sfc_station=" + msg.station + "&";
            msg.mac = "mac_address=" + msg.mac + "&";
            msg.sn = "sn=" + msg.sn + "&";
            msg.part_sn = "part_sn=" + msg.part_sn + "&";
            msg.product = "product=" + msg.product + "&";
            msg.ts = "ts=" + msg.ts + "&";
            msg.tsid = "tsid=" + msg.tsid + "&";
            msg.pCheck = "p=" + msg.pCheck;
            msg.pUpdate = "p=" + msg.pUpdate;

            msg.returnMsg = null;
            //区分查询还是上传
            string model = null;
            switch (checkOrupdate)
            {
                case 1:
                    model = msg.pCheck;
                    break;
                case 2:
                    model = msg.pUpdate;
                    break;
            }

            string m_sn = null;
            string m_partsn = null;

            m_sn = msg.sn;
            m_partsn = msg.part_sn;

            msg.returnMsg = msg.cQuery + msg.line + msg.station + msg.mac + m_sn + m_partsn + msg.product + msg.ts + msg.tsid + model;
            return msg.returnMsg;
        }
        /// <summary>
        /// 输入url + msg  返回查询结果
        /// </summary>
        /// <param name="purl"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public string HTTPPostGetMsg(string purl, string str)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(purl + str);
                request.Method = "GET";
                request.Timeout = 100000;
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string receiveMsg = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return receiveMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 检测SF返回信息   区分结果
        /// </summary>
        /// <param name="resultMsg"></param>
        /// <param name="checkOrupdate"> 1为查询， 2为上传</param>
        /// <param name="station">站别信息</param>
        /// <returns></returns>
        public bool SF_ResultCheck(string resultMsg, int checkOrupdate, string station)
        {
            if (checkOrupdate == 1) //检测查询结果
            {

                string checkValue = null;   //根据结果检测是否存在某些内容
                switch (station)
                {
                    case "IT"://remove 站
                        checkValue = "check=OK";
                        break;
                    case "IF": //FG 站
                        checkValue = "label_FileContent";
                        break;
                    case "II"://ID 站
                        checkValue = "route_check=N/A";
                        break;
                    case "IP"://ocr站
                        checkValue = "route_check=N/A";
                        break;
                    default:
                        checkValue = "check=N/A";
                        break;
                }
                if (resultMsg.Contains(checkValue)) //根据站别区分检测变量信息 检测是否存在
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else//检测上传结果
            {
                string updateValue = null;
                switch (station)
                {
                    case "IT"://remove 站
                        updateValue = "update=OK";
                        break;
                    case "IF": //FG 站
                        updateValue = "update=OK";
                        break;
                    case "II"://ID 站
                        updateValue = "update=OK";
                        break;
                    case "IP"://ocr站
                        updateValue = "update=OK";
                        break;
                    default:
                        updateValue = "update=OK";
                        break;
                }
                if (resultMsg.Contains(updateValue)) //根据站别区分检测变量信息 检测是否存在
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }

    public struct SF_message
    {
        public string url;
        public string cQuery;
        public string line;
        public string station;
        public string mac;
        public string sn;
        public string part_sn;
        public string product;
        public string ts;
        public string tsid;
        public string pCheck;
        public string pUpdate;
        public string returnMsg;
    }

}
