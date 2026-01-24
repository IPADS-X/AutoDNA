using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    [Serializable]
    public class MovePosition
    {
        /// <summary>
        /// 距离
        /// </summary>
        public float PushDistance { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public float PushVM { get; set; }
       /// <summary>
       /// 加速度
       /// </summary>
        
        public float PushAcc { get; set; }

        /// <summary>
        /// 出力
        /// </summary>

        public float PushForce { get; set; }
        
        /// <summary>
        /// 定位范围
        /// </summary>
        public float OrientationRange { get; set; }

        /// <summary>
        /// 时间范围
        /// </summary>
        public int TimeRange { get; set; }
    }
}
