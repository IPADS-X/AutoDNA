using CYAutoFramework;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Windows.Forms;
using CYStandardProcedure.WebReference;
using MsgBoxLib;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace CYStandardProcedure
{
    public partial class ShopFloorForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        /***提示语***/
        private ToolTip toolTip1;

        /// <summary>
        /// MES通讯对象
        /// </summary>
        private string MesType = string.Empty;

        private void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);     
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
        public ShopFloorForm()
        {
            InitializeComponent();
        }

        private void ShopFloorForm_Load(object sender, EventArgs e)
        {
            #region 按钮提示语
            toolTip1 = new ToolTip();
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;



            toolTip1.SetToolTip(btn_Query_A45, "查询ShopFloor");
            toolTip1.SetToolTip(btn_Add_A45, "ShopFloor过站");

            toolTip1.SetToolTip(btn_Query_A29, "查询ShopFloor");
            toolTip1.SetToolTip(btn_Add_A29, "ShopFloor过站");

            toolTip1.SetToolTip(btn_Query_A38, "查询ShopFloor");
            toolTip1.SetToolTip(btn_Add_A38, "ShopFloor过站");

            toolTip1.SetToolTip(btn_Query_A02, "查询ShopFloor");
            toolTip1.SetToolTip(btn_Add_A02, "ShopFloor过站");



            #endregion

            infoLoad(out MesType);
            infoShow(MesType);


            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            cmb_Result_A45.SelectedIndex = 0;
        }

        #region 参数加载方法 MesParame.xml
        /// <summary>
        /// 参数加载
        /// 将对应的参数节点放至第一顺位
        /// </summary>
        /// <param name="mesType">MES通讯对象</param>
        /// <returns></returns>
        private bool infoLoad(out string mesType)
        {
            mesType = string.Empty;
            try
            {
                string filepath = Application.StartupPath + @"\ExeFile\MesParame.xml";
                if (!File.Exists(filepath))
                {
                    return false;
                }
                else
                {
                    /***先读取所有一级节点的名称***/
                    XDocument file = XDocument.Load(filepath);
                    XElement xle = file.Root;
                    IEnumerable<XElement> ixt = xle.Elements();
                    /***清除一级节点名称集合***/
                    List<string> MesParamTypeNameList = new List<string>();
                    MesParamTypeNameList.Clear();
                    foreach (var item in ixt)
                    {
                        MesParamTypeNameList.Add(item.Name.ToString());
                    }
                    mesType = MesParamTypeNameList[0].Split('_')[1];

                    /***key:上传方式 value:MES参数信息***/
                    Program.MesInfoDic = new Dictionary<string, List<string>>();
                    Program.MesInfoDic.Clear();

                    /***加载XML文件***/
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filepath);
                    /***找到特定的节点，第一个***/
                    XmlNode node = doc.SelectSingleNode("/SystemMesParam/" + MesParamTypeNameList[0]);
                    /***获取url接口地址***/
                    XmlElement xe = node as XmlElement;
                    urlA29 = xe.GetAttribute("URL");
                    urlA02 = xe.GetAttribute("URL");
                    urlBaize = xe.GetAttribute("URL");
                    /***获取属性名称***/
                    if (node.HasChildNodes)
                    {
                        XmlNodeList mlistNode = node.ChildNodes;
                        XmlElement el = mlistNode[0] as XmlElement;
                        XmlAttributeCollection ac = el.Attributes;
                        List<string> mesInfoName = new List<string>();
                        mesInfoName.Clear();
                        foreach (XmlAttribute att in ac)
                        {
                            mesInfoName.Add(att.Name);
                        }
                        int cont = mesInfoName.Count;

                        /***获取属性值***/
                        for (int j = 0; j < mlistNode.Count; j++)
                        {
                            /***MES参数信息***/
                            List<string> mesInfo = new List<string>();
                            mesInfo.Clear();
                            XmlElement el2 = mlistNode[j] as XmlElement;
                            for (int k = 1; k < cont; k++)
                            {
                                mesInfo.Add(el2.GetAttribute(mesInfoName[k]));
                            }
                            /***添加到Mes参数的字典***/
                            Program.MesInfoDic.Add(el2.GetAttribute(mesInfoName[0]), mesInfo);
                        }
                    }
                }
                return true;
            }
            catch (Exception ed)
            {
                return false;
            }
        }
        #endregion

        #region 参数显示方法
        /// <summary>
        /// 参数显示
        /// </summary>
        /// <param name="mesType">MES通讯对象</param>
        /// <returns></returns>
        private bool infoShow(string mesType)
        {
            try
            {
                switch (mesType)
                {
                    case "A45":
                        txt_SFLine_A45.Text = Program.MesInfoDic["Query"][0];
                        txt_SFStation_A45.Text = Program.MesInfoDic["Query"][1];
                        txt_SFApp_A45.Text = Program.MesInfoDic["Query"][2];
                        txt_SFFixid_A45.Text = Program.MesInfoDic["Query"][3];
                        rch_QueryP_A45.Text = Program.MesInfoDic["Query"][5];
                        rch_AddP_A45.Text = Program.MesInfoDic["Add"][5];

                        sfLinkA45 = new SFClass_A45();
                        break;
                    case "A38":
                        txt_SFLine_A38.Text = Program.MesInfoDic["Query"][2];
                        txt_SFStation_A38.Text = Program.MesInfoDic["Query"][3];
                        txt_SFMac_A38.Text = Program.MesInfoDic["Query"][4];
                        txt_SFFixid_A38.Text = Program.MesInfoDic["Query"][5].Split('-')[0];
                        rch_QueryP_A38.Text = Program.MesInfoDic["Query"][10];
                        rch_AddP_A38.Text = Program.MesInfoDic["Add"][10];

                        sfLinkA38 = new SFClass_A38();
                        break;
                    case "A02":


                        mesLinkA02 = new MesClass_A02(urlA02);
                        break;
                    case "A29":
                        txt_emp_id.Text = Program.MesInfoDic["Add"][0];
                        txt_panel_sn.Text = Program.MesInfoDic["Add"][1];
                        txt_tool_sn.Text = Program.MesInfoDic["Add"][2];
                        txt_terminal_id.Text = Program.MesInfoDic["Add"][3];
                        txt_wo.Text = Program.MesInfoDic["Add"][4];

                        mesLinkA29 = new MesClass_A29(urlA29);
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ed)
            {
                return false;
            }
        }
        #endregion

        #region A45 嘉善立讯

        /// <summary>
        /// 嘉善立讯SF通讯对象
        /// </summary>
        private SFClass_A45 sfLinkA45;

        private void btn_Query_A45_Click(object sender, EventArgs e)
        {
            string[] MesText = new string[Program.MesInfoDic["Query"].Count + 1];
            MesText[0] = txt_SNQuery_A45.Text.Trim();
            for (int i = 1; i < Program.MesInfoDic["Query"].Count + 1; i++)
            {
                MesText[i] = Program.MesInfoDic["Query"][i - 1];
            }

            string backStr = string.Empty;
            sfLinkA45.shopFloor_Query(MesText, out backStr);
            rch_QueryResult_A45.AppendText(backStr + "\r\n");

            sfLinkA45.shopFloor_Query(sfLinkA45.stringTojsonstr(MesText), out backStr);
            rch_QueryResult_A45.AppendText(backStr + "\r\n");

            if (sfLinkA45.NGstr != string.Empty)
            {
                MsgBox mb = new MsgBox(MsgBoxType.错误, BtnType.OK, true);
                mb.MsgShowDialog("ShopFloor上传出错！", sfLinkA45.NGstr);
            }
        }

        private void btn_Add_A45_Click(object sender, EventArgs e)
        {
            string[] MesText = new string[Program.MesInfoDic["Add"].Count + 1];
            MesText[0] = txt_SNAdd_A45.Text.Trim();
            for (int i = 1; i < Program.MesInfoDic["Add"].Count + 1; i++)
            {
                MesText[i] = Program.MesInfoDic["Add"][i - 1];
            }

            string backStr = string.Empty;
            sfLinkA45.shopFloor_Add(MesText, out backStr);
            rch_AddResult_A45.AppendText(backStr + "\r\n");

            sfLinkA45.shopFloor_Add(sfLinkA45.stringTojsonstr(MesText), out backStr);
            rch_AddResult_A45.AppendText(backStr + "\r\n");

            if (sfLinkA45.NGstr != string.Empty)
            {
                MsgBox mb = new MsgBox(MsgBoxType.错误, BtnType.OK, true);
                mb.MsgShowDialog("ShopFloor上传出错！", sfLinkA45.NGstr);
            }
        }

        #endregion

        #region A38 成都富士康

        /// <summary>
        /// 成都富士康SF通讯对象
        /// </summary>
        private SFClass_A38 sfLinkA38;

        private void btn_Query_A38_Click(object sender, EventArgs e)
        {
            try
            {
                rch_QueryResult_A38.Clear();

                SF_message sf_msg = new SF_message();
                sf_msg.url = Program.MesInfoDic["Query"][0];
                sf_msg.cQuery = Program.MesInfoDic["Query"][1];
                sf_msg.line = Program.MesInfoDic["Query"][2];
                sf_msg.station = Program.MesInfoDic["Query"][3];
                sf_msg.mac = Program.MesInfoDic["Query"][4];
                sf_msg.sn = Program.MesInfoDic["Query"][5].Split('-')[0] + "-" + txt_SNQuery_A38.Text.Trim();
                sf_msg.part_sn = Program.MesInfoDic["Query"][6];
                sf_msg.product = Program.MesInfoDic["Query"][7];
                sf_msg.ts = Program.MesInfoDic["Query"][8];
                sf_msg.tsid = Program.MesInfoDic["Query"][9];
                sf_msg.pCheck = Program.MesInfoDic["Query"][10];

                string checkMsg = sfLinkA38.SF_MessageADD(sf_msg, 1);
                rch_QueryResult_A38.AppendText(checkMsg + "\r\n");
                rch_QueryResult_A38.AppendText(Environment.NewLine);

                string resultMsg = sfLinkA38.HTTPPostGetMsg(sf_msg.url, checkMsg);
                rch_QueryResult_A38.AppendText(resultMsg + "\r\n");
                rch_QueryResult_A38.AppendText(Environment.NewLine);
            }
            catch (Exception es)
            {
                MsgBox mb = new MsgBox(MsgBoxType.错误, BtnType.OK, true);
                mb.MsgShowDialog("手动调试上传错误！", es.Message.ToString());
            }
        }

        private void btn_Add_A38_Click(object sender, EventArgs e)
        {
            try
            {
                rch_AddResult_A38.Clear();

                SF_message sf_msg = new SF_message();
                sf_msg.url = Program.MesInfoDic["Add"][0];
                sf_msg.cQuery = Program.MesInfoDic["Add"][1];
                sf_msg.line = Program.MesInfoDic["Add"][2];
                sf_msg.station = Program.MesInfoDic["Add"][3];
                sf_msg.mac = Program.MesInfoDic["Add"][4];
                sf_msg.sn = Program.MesInfoDic["Add"][5].Split('-')[0] + "-" + txt_SNAdd_A38.Text.Trim();
                sf_msg.part_sn = Program.MesInfoDic["Add"][6];
                sf_msg.product = Program.MesInfoDic["Add"][7];
                sf_msg.ts = Program.MesInfoDic["Add"][8];
                sf_msg.tsid = Program.MesInfoDic["Add"][9];
                sf_msg.pUpdate = Program.MesInfoDic["Add"][10];

                string updateMsg = sfLinkA38.SF_MessageADD(sf_msg, 2);
                rch_AddResult_A38.AppendText(updateMsg + "\r\n");
                rch_AddResult_A38.AppendText(Environment.NewLine);

                string resultMsg = sfLinkA38.HTTPPostGetMsg(sf_msg.url, updateMsg);
                rch_AddResult_A38.AppendText(resultMsg + "\r\n");
                rch_AddResult_A38.AppendText(Environment.NewLine);
            }
            catch (Exception es)
            {
                MsgBox mb = new MsgBox(MsgBoxType.错误, BtnType.OK, true);
                mb.MsgShowDialog("手动调试上传错误！", es.Message.ToString());
            }
        }

        #endregion

        #region A02 上海广达

        /// <summary>
        /// 
        /// </summary>
        private string urlA02 = string.Empty;

        /// <summary>
        /// 上海广达Mes通讯对象
        /// </summary>
        private MesClass_A02 mesLinkA02;

        private void btn_Query_A02_Click(object sender, EventArgs e)
        {

            
            
            
            /***手动调试记录查询上传内容及返回结果***/
            LogConfig.Instance.WriteHandSFLog(rch_QueryResult_A02.Text.Trim() + "\r\n");
        }

        private void btn_Add_A02_Click(object sender, EventArgs e)
        {


            /***手动调试记录查询上传内容及返回结果***/
            LogConfig.Instance.WriteHandSFLog(rch_AddResult_A02.Text.Trim() + "\r\n");
        }

        #endregion

        #region A29 昆山立讯

        /// <summary>
        /// 昆山立讯url接口地址
        /// </summary>
        private string urlA29 = string.Empty;
        /// <summary>
        /// 昆山立讯白泽系统url接口地址
        /// </summary>
        public static string urlBaize = string.Empty;

        /// <summary>
        /// 昆山立讯Mes通讯对象
        /// </summary>
        private MesClass_A29 mesLinkA29;

        private void btn_Add_A29_Click(object sender, EventArgs e)
        {
            /***形成json字符串***/
            mesLinkA29.SendStrDic.Clear();
            mesLinkA29.SendStrDic.Add("emp_id", txt_emp_id.Text.Trim());
            mesLinkA29.SendStrDic.Add("panel_sn", txt_panel_sn.Text.Trim());
            mesLinkA29.SendStrDic.Add("tool_sn", txt_tool_sn.Text.Trim());
            mesLinkA29.SendStrDic.Add("terminal_id", txt_terminal_id.Text.Trim());
            mesLinkA29.SendStrDic.Add("wo", txt_wo.Text.Trim());
            string sendstr = mesLinkA29.dicTojsonstr(mesLinkA29.SendStrDic);

            rch_AddResult_A29.AppendText(sendstr + "\r\n");
            rch_AddResult_A29.AppendText(Environment.NewLine);

            /***上传数据***/
            string receivestr = mesLinkA29.SendHttpPostJson(sendstr);
            rch_AddResult_A29.AppendText(receivestr + "\r\n");
            rch_AddResult_A29.AppendText(Environment.NewLine);
        }

        #endregion

    }
}