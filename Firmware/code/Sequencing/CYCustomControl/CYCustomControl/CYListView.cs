using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace CYCustomControl
{
    public class CYListView : ListView
    {
        public CYListView()
        {
            SetStyle(ControlStyles.DoubleBuffer |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }
    }
}
