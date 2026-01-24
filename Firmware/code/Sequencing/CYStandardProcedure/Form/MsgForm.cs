using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class MsgForm : Form
    {
        /// <summary>
        /// 操作员的卡ID
        /// </summary>
        public string CardID;
        /// <summary>
        /// 物料的新SN
        /// </summary>
        public string NewSN;


        public string btn;
        public MsgForm()
        {
            CardID = "";
            NewSN = "";
            btn = "btn_B";
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void MsgForm_Load(object sender, EventArgs e)
        {
            txt_CardID.Focus();
        }
        
        private void btn_Yes_Click(object sender, EventArgs e)
        {
            CardID = txt_CardID.Text.Trim();
            NewSN = txt_NewSN.Text.Trim();
            btn = "btn_A";
            this.Close();
        }

        private void btn_No_Click(object sender, EventArgs e)
        {
            CardID = "";
            NewSN = "";
            btn = "btn_B";
            this.Close();
        }
    }
}
