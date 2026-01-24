using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    /// <summary>
    /// 数字孪生发送数据内容
    /// </summary>
    public class AnimationClass
    {
        /// <summary>
        /// 设备状态
        /// </summary>
        public int machineStatus;
        /// <summary>
        /// 实验步序
        /// </summary>
        public int taskStep;
        /// <summary>
        /// 实验等待时间步序
        /// </summary>
        public int waitStep;
        /// <summary>
        /// 报警信息
        /// </summary>
        public int alarmMsg;
        /// <summary>
        /// 和地轨交互
        /// </summary>
        public int ground;
        /// <summary>
        /// 和总控交互
        /// </summary>
        public int general;
        /// <summary>
        /// 低温模块温度
        /// </summary>
        public double temperature;
        /// <summary>
        /// 枪头抓取状态
        /// </summary>
        public int TipPickStatus;
        /// <summary>
        /// 枪头退出状态
        /// </summary>
        public int TipBackStatus;
        /// <summary>
        /// 预处理孔开关状态
        /// </summary>
        public int holeStatus;
        /// <summary>
        /// 搬运夹爪夹紧信号，松开信号
        /// </summary>
        public int carryClawStatus;
        /// <summary>
        /// 机器人夹爪夹紧信号，松开信号
        /// </summary>
        public int robotClawStatus;
        /// <summary>
        /// 搬运X轴坐标
        /// </summary>
        public double carryXCur;
        /// <summary>
        /// 搬运Y轴坐标
        /// </summary>
        public double carryYCur;
        /// <summary>
        /// 搬运Z轴坐标
        /// </summary>
        public double carryZCur;
        /// <summary>
        /// 测序仪X轴坐标
        /// </summary>
        public double sequXCur;
        /// <summary>
        /// 移液枪Z轴坐标
        /// </summary>
        public double gunZCur;
        /// <summary>
        /// 搬运夹爪坐标
        /// </summary>
        public double carryClawCur;
        /// <summary>
        /// 机器人夹爪坐标
        /// </summary>
        public double robotClawCur;
        /// <summary>
        /// 机器人关节1坐标
        /// </summary>
        public double robot1Cur;
        /// <summary>
        /// 机器人关节2坐标
        /// </summary>
        public double robot2Cur;
        /// <summary>
        /// 机器人关节3坐标
        /// </summary>
        public double robot3Cur;
        /// <summary>
        /// 机器人关节4坐标
        /// </summary>
        public double robot4Cur;
        /// <summary>
        /// 机器人关节5坐标
        /// </summary>
        public double robot5Cur;
        /// <summary>
        /// 机器人关节6坐标
        /// </summary>
        public double robot6Cur;
        /// <summary>
        /// 搬运X轴启动信号
        /// </summary>
        public int carryXStart;
        /// <summary>
        /// 搬运Y轴启动信号
        /// </summary>
        public int carryYStart;
        /// <summary>
        /// 搬运Z轴启动信号
        /// </summary>
        public int carryZStart;
        /// <summary>
        /// 测序仪X轴启动信号
        /// </summary>
        public int sequXStart;
        /// <summary>
        /// 移液枪Z轴启动信号
        /// </summary>
        public int gunZStart;
        /// <summary>
        /// 搬运X轴目标位置
        /// </summary>
        public double carryXMark;
        /// <summary>
        /// 搬运Y轴目标位置
        /// </summary>
        public double carryYMark;
        /// <summary>
        /// 搬运Z轴目标位置
        /// </summary>
        public double carryZMark;
        /// <summary>
        /// 测序仪X轴目标位置
        /// </summary>
        public double sequXMark;
        /// <summary>
        /// 移液枪Z轴目标位置
        /// </summary>
        public double gunZMark;
        /// <summary>
        /// 搬运X轴速度
        /// </summary>
        public double carryXSpeed;
        /// <summary>
        /// 搬运Y轴速度
        /// </summary>
        public double carryYSpeed;
        /// <summary>
        /// 搬运Z轴速度
        /// </summary>
        public double carryZSpeed;
        /// <summary>
        /// 测序仪X轴速度
        /// </summary>
        public double sequXSpeed;
        /// <summary>
        /// 移液枪Z轴速度
        /// </summary>
        public double gunZSpeed;
        /// <summary>
        /// 搬运XY目标点位名称(代号)
        /// </summary>
        public int material1;
        /// <summary>
        /// 搬运Z目标点位名称(代号)
        /// </summary>
        public int material2;
        /// <summary>
        /// 测序仪X目标点位名称(代号)
        /// </summary>
        public int material3;
        /// <summary>
        /// 测序数据"1-AC*48&TG*30&TC*23|2-AC*48&TG*30&TC*23&AG*48&others*30|3-AC*48&TG*30&TC*23|4-AC*48&TG*30&TC*23"
        /// </summary>
        public string BaseMsg;
        /// <summary>
        /// 实验反应剩余时间
        /// </summary>
        public double RemainTime;
        /// <summary>
        /// 测序结果
        /// </summary>
        public string Result;
    }
    /// <summary>
    /// 数字孪生设备状态枚举
    /// </summary>
    public enum _machineStatusEnum
    {
        停止中 = 0,
        复位中 = 1,
        运行中 = 2,
        暂停中 = 3,
        异常中 = 4,
        报警中 = 5,
    }
    /// <summary>
    /// 数字孪生实验步序枚举
    /// </summary>
    public enum _taskStepEnum
    {
        无实验任务 = 0,
        样本进料 = 1,
        换料中 = 2,
        上料中 = 3,
        取50ul枪头 = 4,
        取30ulFCT试剂 = 5,
        到离心管排液 = 6,
        排枪头 = 7,
        取1000ul枪头 = 8,
        第一次取585ulFCF试剂 = 9,
        FCF混合液第一次吸打混匀 = 10,
        第二次取585ulFCF试剂 = 11,
        取900ulFCF混合液第二次吸打混匀 = 12,
        芯片排气泡 = 13,
        FCF混合试剂吸取800ul = 14,
        到预处理孔排入720ulFCF混合液 = 15,
        取SB试剂 = 16,
        吸打混匀LIB试剂并吸取 = 17,
        取12ulDNA文库样本 = 18,
        取240ulFCF混合液 = 19,
        快速打入芯片预处理孔 = 20,
        取200ul枪头 = 21,
        取75ulDNA文库 = 22,
        逐滴加入芯片上样孔 = 23,
        取398ulDIL试剂 = 24,
        取2ulWMX试剂 = 25,
        吸取实验废液 = 26,
        吸取400ul清洗溶液 = 28,
        清洗试剂打入360ul到预处理孔 = 29,
        吸取清洗废液 = 30,
        吸取500ulS保存试剂 = 31,
        保存试剂打入450ul到预处理孔 = 32,
        吸取保存试剂废液 = 33,
        样本载具流出 = 34,
        芯片室温平衡5分钟 = 35,
        开始测序 = 36,
        测序完成 = 37,
        等待碱基识别 = 38,
        测序芯片开始孵育 = 39,
        测序芯片孵育完成 = 40,
        机器人开预处理孔盖中 = 41,
        机器人开上样孔盖中 = 42,
        机器人开盖完成 = 43,
        机器人关预处理孔盖中 = 44,
        机器人关上样孔盖中 = 45,
        机器人关盖完成 = 46,
        开始拷贝测序文件 = 47,
        文件拷贝完成 = 48,
        文件解析中 = 49,
        解析完成 = 50
    }
    /// <summary>
    /// 数字孪生实验等待时间步序枚举
    /// </summary>
    public enum _waitStepEnum
    {
        无等待时间 = 0,
        芯片室温平衡5分钟 = 1,
        芯片室温孵育60分钟 = 2,
    }
    /// <summary>
    /// 数字孪生报警信息枚举
    /// </summary>
    public enum _alarmMsgEnum
    {
        无报警 = 0,
        扫码枪报警 = 1,
        CCD报警 = 2,
        移液枪报警 = 3,
        电动夹爪报警 = 4,
        机器人报警 = 5,
        测序仪报警 = 6,
        实验流程报警 = 7,
        通讯报警 = 8,
    }
    /// <summary>
    /// 数字孪生和地轨交互枚举
    /// </summary>
    public enum _groundEnum
    {
        无交互任务 = 0,
        耗材要料 = 1,
        空载具回收 = 2,
        样本流转 = 3,
    }
    /// <summary>
    /// 数字孪生和总控交互枚举
    /// </summary>
    public enum _generalEnum
    {
        无交互任务 = 0,
        请求开始实验 = 1,
        测序结束上报 = 2,
        测序结果上报 = 3,
        查询后续实验任务 = 4,
    }
    /// <summary>
    /// 数字孪生TIP头抓取枚举
    /// </summary>
    public enum _TipEnum
    {
        不触发 = 0,
        触发 = 1,
    }
    /// <summary>
    /// 数字孪生预处理孔开关枚举
    /// </summary>
    public enum _holeEnum
    {
        开 = 0,
        关 = 1,
    }
    /// <summary>
    /// 数字孪生夹爪夹紧信号，松开信号枚举
    /// </summary>
    public enum _ClawStatusEnum
    {
        松开 = 0,
        夹紧 = 1,
    }
    /// <summary>
    /// 数字孪生轴启动信号枚举
    /// </summary>
    public enum _AxisStartSignEnum
    {
        停止 = 0,
        启动 = 1,
    }
}
