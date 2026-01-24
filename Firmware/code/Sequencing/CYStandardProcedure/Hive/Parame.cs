using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{

    /// <summary>
    /// Hive上传类型
    /// </summary>
    public enum _HiveUploadType
    {
        /// <summary>
        /// 报警信息
        /// </summary>
        ErrorData,//报警信息
        /// <summary>
        /// 设备信息
        /// </summary>
        MachineData,//设备信息
        /// <summary>
        /// 设备状态
        /// </summary>
        MachineState//设备状态
    }

    /*************** Hive配置参数结构体 ***************/
    #region
    /// <summary>
    /// Hive配置信息结构
    /// </summary>
    public struct HiveConfigInfo
    {

        /// <summary>
        /// 上传类型
        /// </summary>
        public string type;
        /// <summary>
        /// 上传地址
        /// </summary>
        public string url;
        /// <summary>
        /// 是否屏蔽
        /// </summary>
        public bool Shiled;

    }



    /// <summary>
    /// 计划停机信息类
    /// </summary>
    public class HivePlannedInfo
    {
        public string Code;
        public string Msg;
        public string Detail;
    }

    /// <summary>
    /// Hive报警配置信息结构
    /// </summary>
    public struct HiveErrorConfigInfo
    {
        /// <summary>
        /// 故障代码
        /// </summary>
        public string AlarmCode;
        /// <summary>
        /// 报警信息_CH
        /// </summary>
        public string ErrorDescription_CH;
        /// <summary>
        /// 报警信息_EN
        /// </summary>
        public string ErrorDescription_EN;
        /// <summary>
        /// 报警信息_VN
        /// </summary>
        public string ErrorDescription_VN;
        /// <summary>
        /// 上传Hive报警信息
        /// </summary>
        public string ErrorMsg;

        /// <summary>
        /// 报警等级
        /// </summary>
        public string Severity;
    }

    /// <summary>
    /// Hive定义机台状态枚举
    /// </summary>
    public enum _HiveMachineStaus
    {
        正常做料状态 = 0,
        空闲状态,
        屏蔽上传做料状态,
        计划停机状态,
        宕机状态,
    }

    #endregion


    #region 上传序列化类
    /// <summary>
    /// Hive上传机台状态（无报警）序列化类
    /// </summary>
    public class HiveMachineStatusInfo1
    {
        public string machine_state;
        public string state_change_time;
        public HiveMachineStatusData1 data;
    }
    public class HiveMachineStatusData1
    {
        public string state;
        public string message_id;
    }

    /// <summary>
    /// Hive上传机台状态（计划停机）序列化类
    /// </summary>
    public class HiveMachineStatusInfo2
    {
        public string machine_state;
        public string state_change_time;
        public HiveMachineStatusData2 data;

    }
    public class HiveMachineStatusData2
    {
        /// <summary>
        /// 机台状态
        /// </summary>
        public string state;
        /// <summary>
        /// 默认值
        /// </summary>
        public string message_id;
        /// <summary>
        /// 故障代码
        /// </summary>
        public string code;
        /// <summary>
        /// 故障信息
        /// </summary>
        public string error_message;
        /// <summary>
        /// 主软件Hash 值
        /// </summary>
        public string MS_SHA1;
        /// <summary>
        /// 主软件版本号
        /// </summary>
        public string sw_version;
        /// <summary>
        /// 机台之前状态
        /// </summary>
        public string previous_state;
        /// <summary>
        /// 故障信息
        /// </summary>
        public string erroe_detail;
        /// <summary>
        /// 人员卡号
        /// </summary>
        public string badge;
        /// <summary>
        /// Config Hash值
        /// </summary>
        public string CD_SHA1;



    }

    /// <summary>
    /// Hive上传机台状态（异常）序列化类
    /// </summary>
    public class HiveMachineStatusInfo3
    {
        public string machine_state;
        public string state_change_time;
        public HiveMachineStatusData3 data;

    }
    public class HiveMachineStatusData3
    {
        public string sw_version;
        public string previousstate;
        public string error_detail;
        public string error_message;
        public string code;
        public string MS_SHA1;
        public string VS_SHA1;
        public string badge;
        //public string old_sn;
        //public string new_sn;


    }

    /// <summary>
    /// Hive上传报警信息序列化类
    /// </summary>
    public class HiveMachineErrorInfo
    {
        public string message;
        public string code;
        public string severity;
        public string occurrence_time;
        public string resolved_time;
        public HiveMachineErrorData data;
    }
    public class HiveMachineErrorData
    {
        public string hive_state;
        public string error_detail;
    }
    #endregion




    /// <summary>
    /// 关键参数类
    /// </summary>
    public class DashboardParame
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string KeyName;

        /// <summary>
        /// 当前值
        /// </summary>
        public string Value;
        /// <summary>
        /// 最小值
        /// </summary>
        public string LSL;
        /// <summary>
        /// 最大值
        /// </summary>
        public string USL;
    }

    class Parame
    {
    }
}
