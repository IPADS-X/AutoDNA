using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    /// <summary>
    /// 开始时请求总控是否实验 
    /// </summary>
    public class StartReportingToControl
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string requestType = "startReporting";
        /// <summary>
        /// 载具SN
        /// </summary>
        public string sn;
    }
    /// <summary>
    /// 测序结束时上报总控
    /// </summary>
    public class CompleteReportingToControl
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string requestType = "completereporting";
        /// <summary>
        /// 载具SN
        /// </summary>
        public string sn;
        /// <summary>
        /// 实验结果是否成功
        /// </summary>
        public string experimentResult;
    }
    /// <summary>
    /// 测序结束时总控反馈信息
    /// </summary>
    public class CompleteReportingFromControl
    {
        /// <summary>
        /// 当前测序IDNA引物
        /// </summary>
        public string data_idna;
        /// <summary>
        /// 任务ID
        /// </summary>
        public int data_taskId;
        /// <summary>
        /// 实验名称
        /// </summary>
        public string protocol_group_ids;
        /// <summary>
        /// 实验任务每个孔的碱基
        /// </summary>
        public string protocol_group_JianJi;

    }
    /// <summary>
    /// 文件解析完测序结果上报总控
    /// </summary>
    public class ChipDataReportingToControl
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string requestType = "chipDataReporting";
        /// <summary>
        /// 任务ID
        /// </summary>
        public int taskId;
        /// <summary>
        /// 芯片测序结果数据(总数）
        /// </summary>
        public int chipTotalCount;
        /// <summary>
        /// 芯片测序结果数据(匹配数）
        /// </summary>
        public int chipMatchCount;
        /// <summary>
        /// 芯片测序结果
        /// </summary>
        public string matchResult;
        /// <summary>
        /// 每个孔推测的碱基
        /// </summary>
        public string infer_sequence;
        /// <summary>
        /// 与实际碱基不匹配的孔及碱基
        /// </summary>
        public string NG_hole;
    }
    /// <summary>
    /// 文件解析完总控反馈信息
    /// </summary>
    public static class ChipDataReportingFromControl
    {
        public static int code;
        public static string message;
        public static string requestData_requestType;
        public static int requestData_taskId;
        public static int requestData_chipTotalCount;
        public static int requestData_chipMatchCount;
        public static string data;
    }

    /// <summary>
    /// 查询总控接下来是否有任务
    /// </summary>
    public class SearchFolloUpTaskToControl
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string requestType = "searchFolloUpTask";
    }
    /// <summary>
    /// 总控反馈是否有任务
    /// </summary>
    public static class SearchFolloUpTaskFromControl
    {
        public static int code;
        public static string message;
        public static string requestData_requestType;
        public static int data;
    }



}
