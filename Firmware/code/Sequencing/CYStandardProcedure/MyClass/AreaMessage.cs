using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    /// <summary>
    /// 区域信息
    /// </summary>
    public class AreaMessage
    {
        /// <summary>
        /// 搬运模组换料区域名称
        /// </summary>
        public string name;
        /// <summary>
        /// 区域试管剩余数量
        /// </summary>
        public double num_Remain;
        /// <summary>
        /// 区域试管X方向
        /// </summary>
        public int num_X;
        /// <summary>
        /// 区域试管Y方向
        /// </summary>
        public int num_Y;
        /// <summary>
        /// 区域试管X方向最大值
        /// </summary>
        public int num_XMax;
        /// <summary>
        /// 区域试管Y方向最大值
        /// </summary>
        public int num_YMax;
    }
}
