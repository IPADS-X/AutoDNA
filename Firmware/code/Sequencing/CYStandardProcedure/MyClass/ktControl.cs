using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;



namespace ktCnt
{
    //状态
    public enum KpcState_e
    {
        KPC_OK = 0,     //无错误
        KPC_MALLOC_ERR, //动态分配内存错误
        KPC_CONFIG_ERR, //配置错误
        KPC_LINK_ERR,   //连接错误
        KPC_TYPE_ERR,   //类型错误
        KPC_CNT_ERR,    //控制错误
        KPC_CMD_ERR,    //指令错误
    }

    //连接设备类型
    public enum KpcLinkType_e
    {
        KPC_USB_SERIANL = 0,  //串口设备
        KPC_USER_SERIANL,     //用户串口设备
        KPC_CAN_ALYST_II,     //创芯CAN设备
        KPC_CAN_CANETE,       //周立功CAN设备
        KPC_USER_CAN,         //用户CAN设备
    };

    //keyto设备类型
    public enum KpcDeviceType_e
    {
        KPC_ADP16 = 0,    //ADP16
        KPC_ADP18,        //ADP18
        KPC_ADP20,        //ADP20
        KPC_ADP28,        //ADP28
        KPC_ADP5ML,       //ADP5ML
        KPC_ADPZ,         //ADPZ
        KPC_ADPXD,        //ADP变距
        KPC_LHXY,		  //移液臂
    };

    //控制任务状态
    public enum KpcCntTaakState_e
    {
        KPC_TASK_WAIT_FINISH,     //等待完成
        KPC_TASK_EXE_FINISH,      //执行完成
        KPC_TASK_EXE_ERR,         //执行错误
        KPC_TASK_CMD_ERR,         //指令错误
    };

    //控制设备执行状态
    public enum KpcCntDeviceState_e
    {
        KPC_CNT_DEVICE_WAIT_EXE = 0,        //等待执行
        KPC_CNT_DEVICE_WAIT_ACK,        //等待指令应答
        KPC_CNT_DEVICE_WAIT_FINISH,     //等待完成
                                        //当控制设备状态小于PCDS_EXE_FINISH，需要等待执行完成
        KPC_CNT_DEVICE_EXE_FINISH,          //执行完成
        KPC_CNT_DEVICE_ACK_ERR,                 //应答错误
        KPC_CNT_DEVICE_EXE_ERR,                 //执行错误
        KPC_CNT_DEVICE_CMD_ERR,                 //指令错误
        KPC_CNT_DEVICE_LINK_ERR,                //连接错误
    };

    //串口
    [StructLayout(LayoutKind.Sequential)]
    public struct SerianlLinkDeviceInfo_t
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] name;      //设备名称
        public UInt32 baudRate;  //波特率
    };
    //USB CAN设备
    public struct UsbCanLinkDeviceInfo_t
    {
        public byte deviceIndex;   //设备索引
        public byte passageIndex;  //通道索引
        public UInt16 baudRate;    //波特率
    };
    //以太网转CAN设备
    [StructLayout(LayoutKind.Sequential)]
    public struct EthCanLinkDeviceInfo_t
    {
        public byte deviceIndex;    //设备索引0-255
        public byte passageIndex;   //通道索引0-255
        public byte workMode;       //工作模式：0=客户端，1=服务端
        public UInt16 port;         //本地端口0-65535
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ip;    //ip地址
    };

    //连接配置
    public struct KpcLinkDeviceInfo_t
    {
        public SerianlLinkDeviceInfo_t serianl;
        public UsbCanLinkDeviceInfo_t usbCan;
        public EthCanLinkDeviceInfo_t ethCan;
    };
    public struct KpcLinkDeviceConfig_t
    {
        public UInt16 index;       //设备索引
        public KpcLinkType_e type; //连接设备类型
        public KpcLinkDeviceInfo_t info;
    };

    //keyto控制设备配置
    public struct KpcCntDeviceConfig_t
    {
        public UInt16 index;            //控制设备索引
        public KpcDeviceType_e type;    //控制设备类型
        public byte id;                 //控制设备ID
        public UInt16 linkDeviceIndex;  //控制设备的连接索引
    }

    //keyto配置
    public struct KpcConfig_t
    {
        public UInt16 linkDeviceCont;  //连接设备数量
        public IntPtr linkDeviceConfig;//连接设备配置
        public UInt16 cntDeviceCont;   //控制设备数量
        public IntPtr cntDeviceConfig; //控制设备配置
        public UInt16 cntTaskCont;     //控制任务数量
        public IntPtr cntTaskIndex;    //控制任务索引
    };

    public static class ktCntDll
    {
        const string dllName = "KtControlU.dll";
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern KpcState_e KpcInit(IntPtr pConfig);//初始化
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void KpcDeInit();//注销初始化
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern KpcState_e KpcAddCmdList(UInt16 cntTaskIndex, byte[] strCmdList);//添加指令集

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void KpcStopTaskExe(UInt16 cntTaskIndex);//停止任务执行

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern KpcCntTaakState_e KpcGetCntTaskState(UInt16 cntTaskIndex);//获取任务状态

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern KpcCntDeviceState_e KpcGetCntDeviceState(UInt16 cntDeviceIndex);//获取设备状态

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern KpcCntDeviceState_e KpcGetCntDeviceAckData(UInt16 cntDeviceIndex, Int32[] ackData, byte[] cont);//获取设备应答数据

        [DllImport(dllName, CharSet = CharSet.Unicode)]
        public static extern UInt32 KpcGetVersion();//获取设备应答数据

    }
}
