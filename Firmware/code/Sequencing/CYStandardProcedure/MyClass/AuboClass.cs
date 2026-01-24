using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    /// <summary>
    /// 接口板用户DI地址
    /// </summary>
    public enum ROBOT_IO_U_DI
    {
        ROBOT_IO_F1 = 30,
        ROBOT_IO_F2 = 31,
        ROBOT_IO_F3 = 32,
        ROBOT_IO_F4 = 33,
        ROBOT_IO_F5 = 34,
        ROBOT_IO_F6 = 35,
        ROBOT_IO_U_DI_00 = 36,
        ROBOT_IO_U_DI_01 = 37,
        ROBOT_IO_U_DI_02 = 38,
        ROBOT_IO_U_DI_03 = 39,
        ROBOT_IO_U_DI_04 = 40,
        ROBOT_IO_U_DI_05 = 41,
        ROBOT_IO_U_DI_06 = 42,
        ROBOT_IO_U_DI_07 = 43,
        ROBOT_IO_U_DI_10 = 44,
        ROBOT_IO_U_DI_11 = 45,
        ROBOT_IO_U_DI_12 = 46,
        ROBOT_IO_U_DI_13 = 47,
        ROBOT_IO_U_DI_14 = 48,
        ROBOT_IO_U_DI_15 = 49,
        ROBOT_IO_U_DI_16 = 50,
        ROBOT_IO_U_DI_17 = 51
    }
    /// <summary>
    /// 接口板用户DO地址
    /// </summary>
    public enum ROBOT_IO_U_DO
    {
        ROBOT_IO_U_DO_00 = 32,
        ROBOT_IO_U_DO_01 = 33,
        ROBOT_IO_U_DO_02 = 34,
        ROBOT_IO_U_DO_03 = 35,
        ROBOT_IO_U_DO_04 = 36,
        ROBOT_IO_U_DO_05 = 37,
        ROBOT_IO_U_DO_06 = 38,
        ROBOT_IO_U_DO_07 = 39,
        ROBOT_IO_U_DO_10 = 40,
        ROBOT_IO_U_DO_11 = 41,
        ROBOT_IO_U_DO_12 = 42,
        ROBOT_IO_U_DO_13 = 43,
        ROBOT_IO_U_DO_14 = 44,
        ROBOT_IO_U_DO_15 = 45,
        ROBOT_IO_U_DO_16 = 46,
        ROBOT_IO_U_DO_17 = 47
    }

    /// <summary>
    /// 工具端IO名称
    /// </summary>
    public enum TOOL_IO
    {
        [Description("T_DI/O_00")]
        T_00,
        [Description("T_DI/O_01")]
        T_01,
        [Description("T_DI/O_02")]
        T_02,
        [Description("T_DI/O_03")]
        T_03
    }
    public class AuboClass
    {
        int result = 0xffff;
        const int RSERR_SUCC = 0;
        UInt16 rshd = 0xffff;
        double[] target0 = { 0, 0, 0, 0, 0, 0 }; //注意这个里面的值是弧度！
        Dictionary<string, double> posDic = new Dictionary<string, double>();
        private static AuboClass _instance;
        private static object syncObj = new object();
        private static object obj = new object();
        //
        // 摘要:
        //     单例模式
        public static AuboClass Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new AuboClass();
                        }
                    }
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }


        /// <summary>
        /// 机械臂初始化,连接
        /// </summary>
        /// <param name="robotIP">机械臂IP地址</param>
        /// <param name="serverPort">机械臂端口号</param>
        /// <returns></returns>
        public bool Initial(string robotIP, int serverPort)
        {
            result = AuboRobot.rs_initialize();
            if (RSERR_SUCC == result)
            {
                //创建机械臂控制上下文句柄
                if (AuboRobot.rs_create_context(ref rshd) == RSERR_SUCC)
                {
                    //连接机器人
                    if (AuboRobot.rs_login(rshd, robotIP, serverPort) == RSERR_SUCC)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 上电
        /// </summary>
        /// <param name="tool">动力学参数。如果末端夹持工具，此参数应该根据具体的来设定；如果末端没有夹持工具，将此参数的各项设置为0</param>
        /// <param name="state">机械臂启动状态</param>
        /// <returns></returns>
        public bool StartUp(AuboRobot.ToolDynamicsParam tool, int state)
        {
            //int state = 0;
            result = AuboRobot.rs_robot_startup(rshd, ref tool, 9, true, true, 1000, ref state);
            if (result == RSERR_SUCC)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool LogOut()
        {
            if (AuboRobot.rs_logout(rshd) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 初始化全局的运动属性
        /// </summary>
        /// <returns></returns>
        public bool InitGlobalMoveProfile()
        {
            if (AuboRobot.rs_init_global_move_profile(rshd) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 设置六个关节轴动的最大速度,加速度
        /// </summary>
        /// <param name="velc">最大速度</param>
        /// <param name="acc">最大加速度</param>
        /// <returns></returns>
        public bool SetJointVelcAcc(double[] velc, double[] acc)
        {
            if (AuboRobot.rs_set_global_joint_maxvelc(rshd, velc) == RSERR_SUCC
                && AuboRobot.rs_set_global_joint_maxacc(rshd, acc) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetLineVelcAcc(double velc, double acc)
        {
            if (AuboRobot.rs_set_global_end_max_line_velc(rshd, velc) == RSERR_SUCC
                && AuboRobot.rs_set_global_end_max_line_acc(rshd, acc) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 获取机械臂当前位置信息
        /// </summary>
        /// <param name="waypoint"></param>
        /// <param name="ori"></param>
        /// <param name="rpy"></param>
        /// <param name="M_PI"></param>
        /// <returns></returns>
        public Dictionary<string, double> GetCurrentPos(AuboRobot.wayPoint_S waypoint, AuboRobot.Ori ori, AuboRobot.Rpy rpy, double M_PI)
        {
            lock (obj)
            {
                posDic.Clear();
                AuboRobot.rs_get_current_waypoint(rshd, ref waypoint);
                ori = waypoint.orientation;
                AuboRobot.rs_quaternion_to_rpy(rshd, ref ori, ref rpy);
                posDic.Add("xPos", waypoint.cartPos.x);
                posDic.Add("yPos", waypoint.cartPos.y);
                posDic.Add("zPos", waypoint.cartPos.z);
                posDic.Add("rxPos", rpy.rx * 180 / M_PI);
                posDic.Add("ryPos", rpy.ry * 180 / M_PI);
                posDic.Add("rzPos", rpy.rz * 180 / M_PI);
                posDic.Add("joint1", waypoint.jointpos[0] * 180 / M_PI);
                posDic.Add("joint2", waypoint.jointpos[1] * 180 / M_PI);
                posDic.Add("joint3", waypoint.jointpos[2] * 180 / M_PI);
                posDic.Add("joint4", waypoint.jointpos[3] * 180 / M_PI);
                posDic.Add("joint5", waypoint.jointpos[4] * 180 / M_PI);
                posDic.Add("joint6", waypoint.jointpos[5] * 180 / M_PI);
                posDic.Add("oriW", waypoint.orientation.w);
                posDic.Add("oriX", waypoint.orientation.x);
                posDic.Add("oriY", waypoint.orientation.y);
                posDic.Add("oriZ", waypoint.orientation.z);
                return posDic;
            }
        }

        /// <summary>
        /// 移动到坐标系原点
        /// </summary>
        /// <returns></returns>
        public bool MoveOri()
        {
            if (AuboRobot.rs_move_joint(rshd, target0, true) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 运动到当前位置
        /// </summary>
        /// <param name="type">运动类型;0表示直线运动方式;1表示关节运动方式</param>
        /// <param name="pos">位置坐标(x, y, z)</param>
        /// <param name="tool_pos">工具描述</param>
        /// <param name="isblock">是否阻塞</param>
        /// <returns></returns>
        public int MoveToPos(int type, AuboRobot.Pos pos, AuboRobot.ToolInEndDesc tool_pos, bool isblock)
        {
            int result;
            if (type == 0)
            {
                result = AuboRobot.rs_move_line_to(rshd, ref pos, ref tool_pos, isblock);
                if (RSERR_SUCC == result)
                {
                    return result;
                }
                else
                {
                    return result;
                }

            }
            else
            {
                result = AuboRobot.rs_move_joint_to(rshd, ref pos, ref tool_pos, isblock);
                if (RSERR_SUCC == result)
                {
                    return result;
                }
                else
                {
                    return result;
                }
            }
        }

        public int MovePos(int type, double[] joints, bool isblock)
        {
            int result;
            if (type == 0)
            {
                result = AuboRobot.rs_move_line(rshd, joints, isblock);
                if (RSERR_SUCC == result)
                {
                    return result;
                }
                else
                {
                    return result;
                }

            }
            else
            {
                result = AuboRobot.rs_move_joint(rshd, joints, isblock);
                if (RSERR_SUCC == result)
                {
                    return result;
                }
                else
                {
                    return result;
                }

            }

        }

        /// <summary>
        /// 暂停运动
        /// </summary>
        /// <returns></returns>
        public bool MovePause()
        {
            if (AuboRobot.rs_move_pause(rshd) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 继续运动
        /// </summary>
        /// <returns></returns>
        public bool MoveContinue()
        {
            if (AuboRobot.rs_move_continue(rshd) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 停止运动
        /// </summary>
        /// <returns></returns>
        public bool MoveStop()
        {
            if (AuboRobot.rs_move_fast_stop(rshd) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置jog运动的坐标系
        /// </summary>
        /// <param name="user_coord">示教坐标系</param>
        /// <returns></returns>
        public bool TeachCoord(AuboRobot.CoordCalibrate user_coord)
        {
            if (AuboRobot.rs_set_teach_coord(rshd, ref user_coord) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Jog运动
        /// </summary>
        /// <param name="teachMode">要运动的关节或方向</param>
        /// <param name="dir">运动方向,true为正向,false为反向</param>
        /// <returns></returns>
        public bool MoveJog(AuboRobot.teach_mode teachMode, bool dir)
        {
            if (AuboRobot.rs_teach_move_start(rshd, teachMode, dir) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 停止Jog运动
        /// </summary>
        /// <returns></returns>
        public bool MoveJogStop()
        {
            if (AuboRobot.rs_teach_move_stop(rshd) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置用户IO（必须连接控制柜！！！） 
        /// </summary>
        /// <param name="address">用户输出名称</param>
        /// <param name="value">设置值</param>
        /// <returns></returns>
        public bool SetUserIO(int address, double value)
        {
            if (RSERR_SUCC == AuboRobot.rs_set_board_io_status_by_addr(rshd, 5, address, value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取用户IO（必须连接控制柜！！！)
        /// </summary>
        /// <param name="ioType">输入输出类型;4表示用户输入;5表示用户输出</param>
        /// <param name="address">用户输入输出地址</param>
        /// <returns></returns>
        public double GetUserIO(int ioType,int address)
        {
            double value = 0;
            if (RSERR_SUCC == AuboRobot.rs_get_board_io_status_by_addr(rshd, ioType, address, ref value))
            {
                return value;
            }
            else
            {
                return value;
            }
        }


        /// <summary>
        /// 设置工具端IO
        /// </summary>
        /// <param name="name">工具端IO名称</param>
        /// <param name="value">设置值</param>
        /// <returns></returns>
        public bool SetToolIO(string name, int value)
        {
            if (RSERR_SUCC == AuboRobot.rs_set_tool_do_status(rshd, name, value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取设置工具端IO
        /// </summary>
        /// <param name="name">工具端IO名称</param>
        /// <returns></returns>
        public double GetToolIO(string name)
        {
            double io_status = 0;
            if (RSERR_SUCC == AuboRobot.rs_get_tool_io_status(rshd, name, ref io_status))
            {
                return io_status;
            }
            else
            {
                return io_status;
            }
        }
        public bool RpyToQuaternion(AuboRobot.Rpy rpy_onuser, AuboRobot.Ori ori_onuser)
        {
            if (AuboRobot.rs_rpy_to_quaternion(rshd, ref rpy_onuser, ref ori_onuser)==RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool UserToBase(AuboRobot.Pos pos_onuser, AuboRobot.Ori ori_onuser, AuboRobot.CoordCalibrate user_coord, AuboRobot.ToolInEndDesc tool_desc, AuboRobot.Pos pos_onbase, AuboRobot.Ori ori_onbase)
        {
            if (AuboRobot.rs_user_to_base(rshd, ref pos_onuser, ref ori_onuser, ref user_coord, ref tool_desc, ref pos_onbase, ref ori_onbase)==RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool QuaternionToRpy(AuboRobot.Ori ori_onbase, AuboRobot.Rpy rpy_onbase)
        {
            if (AuboRobot.rs_quaternion_to_rpy(rshd, ref ori_onbase, ref rpy_onbase) == RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 基坐标转用户坐标
        /// </summary>
        /// <param name="pos_onbase"></param>
        /// <param name="ori_onbase"></param>
        /// <param name="user_coord"></param>
        /// <param name="tool_desc"></param>
        /// <param name="pos_onuser"></param>
        /// <param name="ori_onuser"></param>
        /// <returns></returns>
        public bool BaseToUser(AuboRobot.Pos pos_onbase, AuboRobot.Ori ori_onbase, AuboRobot.CoordCalibrate user_coord, AuboRobot.ToolInEndDesc tool_desc, AuboRobot.Pos pos_onuser, AuboRobot.Ori ori_onuser)
        {
            if (AuboRobot.rs_base_to_user(rshd, ref pos_onbase, ref ori_onbase, ref user_coord, ref tool_desc, ref pos_onuser, ref ori_onuser)==RSERR_SUCC)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取当前关节角度(弧度)
        /// </summary>
        /// <param name="jointAngles">输出当前关节角度(弧度)</param>
        /// <returns>获取结果，0表示成功</returns>
        public int GetCurrentJointAngles(out double[] jointAngles)
        {
            jointAngles = new double[6];
            try
            {
                lock (obj)
                {
                    var waypoint = new AuboRobot.wayPoint_S();
                    int result = AuboRobot.rs_get_current_waypoint(rshd, ref waypoint);
                    if (result == RSERR_SUCC)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            jointAngles[i] = waypoint.jointpos[i];
                        }
                    }
                    return result;
                }
            }
            catch (Exception)
            {
                return 0xffff;
            }
        }

        /// <summary>
        /// 逆运动学计算
        /// </summary>
        /// <param name="pos">目标位置</param>
        /// <param name="ori">目标姿态</param>
        /// <param name="jointAngles">输出关节角度(弧度)</param>
        /// <returns>计算结果，0表示成功</returns>
        public int InverseKinematics(AuboRobot.Pos pos, AuboRobot.Ori ori, out double[] jointAngles)
        {
            try
            {
                // 获取当前关节角作为参考，提高逆解精度
                double[] currentJoints;
                int getCurrentResult = GetCurrentJointAngles(out currentJoints);
                
                if (getCurrentResult != RSERR_SUCC)
                {
                    // 如果无法获取当前关节角，使用默认值
                    currentJoints = new double[6] { 0, 0, 0, 0, 0, 0 };
                }
                System.Diagnostics.Debug.WriteLine($"当前关节角: {string.Join(", ", currentJoints)}");
                var waypoint = new AuboRobot.wayPoint_S();
                
                // 使用当前关节角作为参考进行逆运动学计算
                int result = AuboRobot.rs_inverse_kin(rshd, currentJoints, ref pos, ref ori, ref waypoint);
                
                if (result == RSERR_SUCC)
                {
                    // 从 waypoint 中获取计算出的精确关节角
                    jointAngles = new double[6];
                    for (int i = 0; i < 6; i++)
                    {
                        jointAngles[i] = waypoint.jointpos[i];
                    }
                }
                else
                {
                    jointAngles = new double[6];
                }
                System.Diagnostics.Debug.WriteLine($"逆解关节角度: {string.Join(", ", jointAngles)}");
                
                return result;
            }
            catch (Exception)
            {
                jointAngles = new double[6];
                return 0xffff;
            }
        }
    }
}