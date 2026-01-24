using Modbus;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{

    /// <summary>
    /// 夹爪串口参数
    /// </summary>
    [Serializable]
    public class ModbusRtuConfig
    {

        /// <summary>
        /// 波特率
        /// </summary>
        public int iBaudRate { get; set; }
        /// <summary>
        /// 串口号
        /// </summary>
        public string iPortName { get; set; }
        /// <summary>
        /// 数据位
        /// </summary>
        public int iDataBits { get; set; }

        /// <summary>
        /// 校验位
        /// </summary>
        public Parity iParity { get; set; }

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits iStopBits { get; set; }

        /// <summary>
        /// 从站地址
        /// </summary>
        public int DevAdd { get; set; }


        /// <summary>
        /// 数据格式
        /// </summary>
        public DataFormat DataFormat { get; set; }

    }
}
