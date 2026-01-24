using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    /// <summary>
    /// 电动夹爪
    /// </summary>
    [Serializable]
    public class GripPawlConfig
    {
        /// <summary>
        /// 点位名
        /// </summary>
        public string PointName { get; set; }


        /// <summary>
        /// 位置
        /// 单位：um
        /// </summary>
        public int PushDistance { get; set; }


        /// <summary>
        /// 速度
        /// 单位：um/s
        /// </summary>
        public int PushVM { get; set; }


        /// <summary>
        /// 加速度
        /// 点位：um/s^2
        /// </summary>
        public int PushAcc { get; set; }



        /// <summary>
        /// 定位区间
        /// 单位: um
        /// </summary>
        public int OrientationRange { get; set; }



        /// <summary>
        /// 推压力
        /// </summary>
        public int PushForce { get; set; }


        /// <summary>
        /// 推压距离
        /// 点位：um
        /// </summary>
        public int ForceDistance { get; set; }



        /// <summary>
        /// 延时时间
        /// 点位：ms
        /// </summary>
        public int TimeRange { get; set; }


    }
}
