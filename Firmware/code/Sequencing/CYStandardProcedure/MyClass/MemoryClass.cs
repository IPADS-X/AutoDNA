using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    /// <summary>
    /// 记忆类
    /// </summary>
    public class MemoryClass
    {
        /// <summary>
        /// 进料工位状态
        /// </summary>
        public enum FeedingStation_State
        {
            空闲=0,
            换料,
            缺料,
            满料
        }
        /// <summary>
        /// 搬运工位状态
        /// </summary>
        public enum CarryStation_State
        {
            空闲 = 0,
            换料中,
            上料中,
            供料完成,
            实验开始,
            开始步骤一,
            步骤一完成,
            DNA文库配置完成,
            开始步骤二,
            测序配置完成,
            清洗,
            废液已吸取,
            开始清洗步骤一,
            清洗步骤一完成,
            开始清洗步骤二,
            清洗步骤二完成,
            保存,
            开始清洗步骤三,
            保存液排气泡,
            清洗步骤三完成,
            开始清洗步骤四,
            实验完成,
            出料
        }


        /// <summary>
        /// 测序仪工位状态
        /// </summary>
        public enum SequencingStation_State
        {
            空闲 = 0,
            去开预处理孔,
            可开预处理孔,
            开盖完成,
            去开上样孔,
            可开上样孔,
            去关上样孔,
            可关上样孔,
            继续关预处理孔,
            等待关盖完成,
            测序中,
            测序完成,
            去关预处理孔,
            可关预处理孔,
            关盖完成,
            孵育中,
            孵育完成
        }
        /// <summary>
        /// 机器人工位状态
        /// </summary>
        public enum RobotStation_State
        {
            空闲 = 0,
            开预处理孔盖中,
            开盖完成,
            开上样孔盖中,
            关预处理孔盖中,
            关上样孔盖中,
            关盖完成
        }
        /// <summary>
        /// 数据处理线程状态
        /// </summary>
        public enum DataProcessingStation_State
        {
            空闲 = 0,
            文件拷贝中,
            文件解析中,
        }


        /// <summary>
        /// 机器人夹爪工艺点记录
        /// </summary>
        public enum RobotClaw_technology
        {
            夹爪默认松开,
            夹爪松开,
            夹爪夹紧
        }

        /// <summary>
        /// 搬运夹爪工艺点记录
        /// </summary>
        public enum Clamping_jaw_technology
        {
            夹爪默认松开,
            夹爪夹紧,
            夹爪松开,
            过渡点
        }

        /// <summary>
        /// 移液枪工艺点记录
        /// </summary>
        public enum Pipette_gun_technology
        {
            过渡点,
            未取枪头,
            已取1号枪头,
            已取1号试剂,
            已排1号试剂,
            已排1号枪头,
            已取2号枪头,
            已取2号试剂,
            已排2号试剂,
            已排2号枪头,
            已取3号枪头,
            已取3号试剂,
            已排3号试剂,
            已排3号枪头,
            已取4号枪头,
            已取4号试剂,
            已排4号试剂,
        }


        /// <summary>
        /// 测序机台区域
        /// </summary>
        public enum Area
        {
            枪头区1,
            枪头区2,
            枪头区3,
            枪头区4,
            低温区,
            常温试剂区,
            离心管试管区,
            八联排试管区,
            废料区1,
            废料区2,
            枪头进料区,
            枪头出料区,
            进料区,
            出料区
        }
        public enum NoOutArea
        {
            NULL,
            枪头区1,
            枪头区2,
            枪头区3,
            枪头区4,
            低温区,
            常温试剂区,
            离心管试管区,
            八联排试管区
        }

        public enum CarryStation_Working
        {
            开始工作,
            工作结束
        }


        /// <summary>
        /// 供料工位状态
        /// </summary>
        public FeedingStation_State FeedingStation_state;

        /// <summary>
        /// 搬运工位状态
        /// </summary>
        public CarryStation_State CarryStation_state;

        /// <summary>
        /// 测序仪工位状态
        /// </summary>
        public SequencingStation_State SequencingStation_state;

        /// <summary>
        /// 机器人工位状态
        /// </summary>
        public RobotStation_State RobotStation_state;
        /// <summary>
        /// 数据处理线程状态
        /// </summary>
        public DataProcessingStation_State DataProcessingStation_state;


        /// <summary>
        /// 搬运夹爪工艺过程点
        /// </summary>
        public Clamping_jaw_technology clamping_jaw_technology;
        /// <summary>
        /// 机器人夹爪工艺过程点
        /// </summary>
        public RobotClaw_technology robotclaw_technology;

        /// <summary>
        /// 移液枪工艺过程点
        /// </summary>
        public Pipette_gun_technology pipette_gun_technology;

        /// <summary>
        /// 机台有载具时补料区域提示
        /// </summary>
        public  Area area;
        /// <summary>
        /// 机台无载具时补料区域提示
        /// </summary>
        public  NoOutArea area_noout;
        /// <summary>
        /// 搬运工位工作标志（用于机器人避位）
        /// </summary>
        public CarryStation_Working carrystation_working;

    }
}
