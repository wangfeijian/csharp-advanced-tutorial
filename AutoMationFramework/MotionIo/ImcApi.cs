#define HANDLETYPE_UINT64
//#define MACHINE_X86

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Inovance.InoMotionCotrollerShop.InoServiceContract.EtherCATConfigApi
{
    /// <summary>
    /// 兼容早期版本使用UInt32 HANDLETYPE，后期版本系统使用UInt64 HANDLETYPE。
    /// </summary>
#if HANDLETYPE_UINT64
    using HANDLETYPE = UInt64;
#else
    using HANDLETYPE = UInt32;
#endif

    public class ImcApi
    {
        #region STATIC
#if MACHINE_X86
        public const string EtherCATConfigApiDllName = "IMC_API.dll";
#else
        public const string EtherCATConfigApiDllName = "IMC_API.dll";
#endif
        #endregion

        #region FUNCTION RETURN DEFINE
        public const uint EXE_SUCCESS = 0x00000000;
        public const uint ERR_HANDLE = 0xFFFFFFFF;
        #endregion

        #region DATA STRUCT DEFINE
        [StructLayout(LayoutKind.Sequential)]
        public struct TRsouresNum
        {
            public Int16 terminalexist; // 端子板是否连接
            public Int16 axNum; // ECAT 总线的所有轴数
            public Int16 diNum; // ECAT 总线上DI 资源
            public Int16 doNum; // ECAT 总线上DO 资源
            public Int16 adNum; // ECAT 总线上AD 资源
            public Int16 daNum; // ECAT 总线上DA 资源
            public Int16 encNum;// ECAT 总线上ENC资源
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TMtPara
        {
            public double bgVel;    // 起始速度值 pulse/ms
            public double maxVel;   // 最大速度 pulse/ms
            public double maxAcc;   // 最大加速度 pulse/ms^2
            public double maxDec;   // 最大减速度 pulse/ms^2
            public double maxJerk;  // 最大加加速度 pulse/ms^3
            public double stopDec;  // 平滑停止减速度 pulse/ms^2
            public double eStopDec; // 急停减速度 pulse/ms^2
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TAxAttriPara
        {
            public Int16 arrivalBand; 		// 到位误差 pulse
            public Int16 arrivalTime; 		// 到位保持时间 ms
            public Int32 errorLmt; 		// 最大跟随误差 pulse
            public Int32 softPosLimitPos; 	// 软正限位 pulse
            public Int32 softNegLimitPos; 	// 软负限位 pulse
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TAxCheckEn
        {
            public Int16 alarmEn; 		// 报警是否有效标志
            public Int16 softLmtEn; 	// 软限位是否有效标志
            public Int16 hwLmtEn; 		// 硬限位是否有效标志
            public Int16 errorLmtEn; 	// 跟随误差是否检查标志
        };

        // 采集数据类型
        // 单轴数据类型
        public const uint SAMPLE_ADDRESS_TYPE_AX_PRF_POS = (0x01);
        public const uint SAMPLE_ADDRESS_TYPE_AX_ENC_POS = (0x02);
        public const uint SAMPLE_ADDRESS_TYPE_AX_PRF_VEL = (0x03);
        public const uint SAMPLE_ADDRESS_TYPE_AX_ENC_VEL = (0x04);
        public const uint SAMPLE_ADDRESS_TYPE_AX_PRF_ACC = (0x05);
        public const uint SAMPLE_ADDRESS_TYPE_AX_ENC_ACC = (0x06);

        public const uint SAMPLE_ADDRESS_TYPE_PRF_POS = (0x07);
        public const uint SAMPLE_ADDRESS_TYPE_PRF_POS1 = (0x08);
        public const uint SAMPLE_ADDRESS_TYPE_PRF_POS2 = (0x09);

        public const uint SAMPLE_ADDRESS_TYPE_AX_TORQ = (0x0a);

        // 0x100 - 0x1FF (插补选项)
        public const uint SAMPLE_ADDRESS_TYPE_CRD1_POSX = (0x100);
        public const uint SAMPLE_ADDRESS_TYPE_CRD1_POSY = (0x101);
        public const uint SAMPLE_ADDRESS_TYPE_CRD1_POSZ = (0x102);
        public const uint SAMPLE_ADDRESS_TYPE_CRD1_VEL = (0x103);

        public const uint SAMPLE_ADDRESS_TYPE_CRD2_POSX = (0x150);
        public const uint SAMPLE_ADDRESS_TYPE_CRD2_POSY = (0x151);
        public const uint SAMPLE_ADDRESS_TYPE_CRD2_POSZ = (0x152);
        public const uint SAMPLE_ADDRESS_TYPE_CRD2_VEL = (0x153);

        public const uint SAMPLE_ADDRESS_TYPE_CRD3_POSX = (0x200);
        public const uint SAMPLE_ADDRESS_TYPE_CRD3_POSY = (0x201);
        public const uint SAMPLE_ADDRESS_TYPE_CRD3_POSZ = (0x202);
        public const uint SAMPLE_ADDRESS_TYPE_CRD3_VEL = (0x203);

        public const uint SAMPLE_ADDRESS_TYPE_CRD4_POSX = (0x250);
        public const uint SAMPLE_ADDRESS_TYPE_CRD4_POSY = (0x251);
        public const uint SAMPLE_ADDRESS_TYPE_CRD4_POSZ = (0x252);
        public const uint SAMPLE_ADDRESS_TYPE_CRD4_VEL = (0x253);

        public const uint SAMPLE_TRIG_IMMEDIATE = (0); // 立即采集
        public const uint SAMPLE_TRIG_DELAY = (1); // 延时采集
        public const uint SAMPLE_TRIG_LOCAL_DI = (2); // 本地DI触发
        public const uint SAMPLE_TRIG_ECAT_DI = (3); // ECAT 的DI 触发

        [StructLayout(LayoutKind.Sequential)]
        public struct TSamplePara
        {
            public Int16 interval;       // 采样时间间隔
            public Int16 trigType;       // 触发采样类型：0 立即 1 延时 2 本地di 3 ECAT di
            public Int16 delay;          // 延时时间 ms
            public Int16 diNo;           // di输入号
            public Int16 diLevel;        // di的触发输入值 0或1
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct THomingPara
        {
            public Int16 homeMethod; 			// 回原点方法
            public Int32 offset; 				// 回原点后的零点偏执 pulse
            public UInt32 highVel; 			// 高速搜索减速点速度 pulse/ms
            public UInt32 lowVel; 				// 搜索原点低 pulse/ms
            public UInt32 acc; 				// 加速度 pulse/ms^2
            public UInt32 overtime; 			// 超时时间 ms
            public Int16 posSrc; 				// 仅对端子板轴回零有效，ECAT 轴无效
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct THomingParaInUint
        {
            public Int16 homeMethod;     // 回原点方法
            public double offset;         // 回原点后的零点偏执 unit
            public double highVel;       // 高速搜索减速点速度 unit/s
            public double lowVel;        // 搜索原点低 unit/s
            public double acc;           // 加速度 unit/s^2
            public UInt32 overtime;      // 超时时间 ms
            public Int16 posSrc;        // 仅对端子板轴回零有效，ECAT 轴无效
        };

        //public const uint HOME_STATUS_INITIAL         = (0x0000);
        //public const uint HOME_STATUS_RUNNING         = (0x0001);
        //public const uint HOME_STATUS_STOPPED         = (0x0002);
        //public const uint HOME_STATUS_TOZERO          = (0x0004);
        //public const uint HOME_STATUS_SUCCEED         = (0x0008);
        //public const uint HOME_STATUS_ERROR           = (0x00F0);
        //public const uint HOME_STATUS_ERROR_TYPE      = (0x00F1);
        //public const uint HOME_STATUS_ERROR_EXECUTE   = (0x00F2);
        //public const uint HOME_STATUS_ERROR_SENSOR    = (0x00F3);
        //public const uint HOME_STATUS_ERROR_OVERTIME  = (0x00F4);

        //20180914修改
        //public const uint HOME_IN_PROGRESS = (0);     // 正在回零中
        //public const uint HOME_INTERRUPTED_OR_NOT_START = (1);     // 回零中断或者没有开始启动
        //public const uint HOME_ATTAINED_BUT_NOT_REACH = (2);     // 回零结束，但没有到设定的目标位置
        //public const uint HOME_SUCESS = (3);     // 回零成功
        //public const uint HOME_ERR_VEL_NOT_ZERO = (4);     // 回零中发生错误，同时速度不为0 
        //public const uint HOME_ERR_VEL_ZERO = (5);     // 回零中发生错误，同时速度为0
        public const Int16 HOME_IN_PROGRESS = (0);     // 正在回零中
        public const Int16 HOME_INTERRUPTED_OR_NOT_START = (1);     // 回零中断或者没有开始启动
        public const Int16 HOME_ATTAINED_BUT_NOT_REACH = (2);     // 回零结束，但没有到设定的目标位置
        public const Int16 HOME_SUCESS = (3);     // 回零成功
        public const Int16 HOME_ERR_VEL_NOT_ZERO = (4);     // 回零中发生错误，同时速度不为0 
        public const Int16 HOME_ERR_VEL_ZERO = (5);     // 回零中发生错误，同时速度为0

        //crd高级参数结构体
        [StructLayout(LayoutKind.Sequential)]
        public struct TCrdAdvParam
        {
            public Int16 userVelMode;          // 用户速度规划模式：0 系统前瞻速度规划，1 用户设定速度规划(默认：0)
            public Int16 transMode;            // 过渡模式 1：无过渡 2：圆弧过渡 （默认:2）
            public Int16 noDataProtect;        // 数据断流保护：0不保护，1保护 （默认：1）
            public Int16 circAccChangeEn;      // 圆弧变加速使能：0不变加速，1变加速 （默认：0） 
            public Int16 noCoplaneCircOptm;    // 异面圆弧优化：0不开启，1开启
            public double turnCoef;            // 拐弯系数: [0.01~50]（默认：1.0）
            public double tol;                 // 插补精度: 大于0的值（默认：0 ，单位取决于设置的当量）
        };

        //跟随模式切换启动条件结构体
        //public struct TFolStartCond
        //{
        //  public Int16 type;                 // 启动类型: 0:立即启动 1:等待时间 2:等待DI 3:等待主轴运动
        //  public Int16 delay;                // 等待时间 单位:ms
        //  public Int16 diNO;                 // 等待DI输入号
        //  public Int16 diType;               // 等待DI类型
        //  public Int16 diLevel;              // DI的触发输入值 0或1
        //  public double waitPos;             // 主轴运动的位置，绝对位置
        //  public Int16 waitPosType;          // 等待主轴运动位置的类型.0:小于等于waitPos触发 1:大于等于waitPos触发
        //};

        //public struct TFolParam
        //{
        //  public double masterScale;   // 主轴齿数
        //  public double slaveScale;    // 从轴齿数
        //  public Int16 masterNo;       // 主轴轴号
        //  public Int16 masterType;     // 主轴类型
        //  public Int16 slaveType;      // 从轴类型
        //  public Int16 dirMode;        // 方向模式
        //};
        [StructLayout(LayoutKind.Sequential)]
        public struct TGearParam
        {
            public double masterScale;		// 主轴齿数
            public double slaveScale;		// 从轴齿数
            public Int16 masterNo;			// 主轴轴号
            public Int16 masterType;		// 主轴类型
            public Int16 dirMode;			// 方向模式
            public Int32 masterSlopeDis;	// 主轴离合区
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TEventIO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Int16[] inType;		// 输入的类型：0表示物理输入 1表示虚拟输入
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Int16[] inPortNo;		// 输入的端口号
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Int16[] inInvert;		// 输入是否取反：0表示不取反 1表示取反
            public Int16 inOperator;		// 输入操作符：0与操作 1或操作  2异或操作 3 单输入
            public Int16 outType;			// 输出类型：0表示物理输出 1表示虚拟输出（虚拟输出一般作为其他单元的输入源）
            public Int16 outPortNo;		// 输出端口
            public Int16 outInvert;		// 输出取反：0表示不取反 1表示取反
            public Int16 activeType;		// 生效的类型：0单次生效 1持续生效
            public Int16 triggerType;      // 触发类型：0:电平触发 1：上升沿触发 2：下降沿触发
            public Int16 delayTime;		// 延时输出的时间:ms
        };

        public struct TEventDiMotion
        {
            public Int16 inType;  				// 输入的类型：0表示物理输入 1表示虚拟输入
            public Int16 inPortNo;				// 输入的端口号
            public Int16 motionAxNo;			// 触发的运动轴号
            public Int16 delay;				    // 单位：ms
            public Int16 motionType;            // 启停类型 0:PTP启动  1：Jog启动  2：平滑停止  3：急停  4：更新参数
            public Int16 triggerType;           // 触发类型 0:电平触发（高电平） 1:上升沿触发 2：下降沿触发
            public Int16 invertBit;			    // 取反位，输入取反
            public double tgtPos;				// 目标位置
            public double tgtVel;				// 目标速度
            public double acc;					// 加速度
            public double dec;					// 减速度
        };

        public struct TEventCompareOut
        {
            public Int16 inType;  				// 输入类型：0编码器 1规划位置
            public Int16 portNo;				// 端口号
            public Int16 outPortNo;			// DO输出端口号
            public Int16 outVal;				// 输出电平值
            public Int16 outType;				// 输出类型：0电平 1脉宽 2虚拟值（虚拟输出一般作为其他单元的输入源）
            public Int16 cmpType;				// 比较的方式：0大于等于  1小于
            public Int16 pulseWidth;			// 脉冲宽度
            public double comparePos;			// 比较的位置
        };

        /*
        ///单个TMultiCmpData结构体使用示例代码，TEventIO结构体使用只需将下面的代码中typeof(ImcApi.TMultiCmpData)替换为typeof(ImcApi.TEventIO)。
        int size = 0;
        IntPtr pBuff = IntPtr.Zero;
        size = Marshal.SizeOf(typeof(ImcApi.TMultiCmpData));
        pBuff = Marshal.AllocHGlobal(size);
        ImcApi.TMultiCmpData multiCmpData = (ImcApi.TMultiCmpData)Marshal.PtrToStructure(pBuff, typeof(ImcApi.TMultiCmpData));

        multiCmpData.compareData[0] = 1;
        multiCmpData.compareData[1] = 2;
        multiCmpData.compareData[2] = 3;
        Console.Out.WriteLine(multiCmpData.compareData[0] + "," + multiCmpData.compareData[1] + "," + multiCmpData.compareData[2]);
        Marshal.FreeHGlobal(pBuff);
         
        ///TMultiCmpData结构体数组使用示例代码，TEventIO结构体数组使用只需将下面的代码中typeof(ImcApi.TMultiCmpData)替换为typeof(ImcApi.TEventIO)。
        int size = 0;
        int num = 100;
        IntPtr pBuff = IntPtr.Zero;
        size = Marshal.SizeOf(typeof(ImcApi.TMultiCmpData));
        pBuff = Marshal.AllocHGlobal(size * num);
        ImcApi.TMultiCmpData[] multiCmpData = new ImcApi.TMultiCmpData[num];
        for (int i = 0; i < num; i++)
        {
            multiCmpData[i] = (ImcApi.TMultiCmpData)Marshal.PtrToStructure(new IntPtr(pBuff.ToInt32() + size * i), typeof(ImcApi.TMultiCmpData));
        }
        for (int i = 0; i < num; i++)
        {
            multiCmpData[i].compareData[0] = i + 1;
            multiCmpData[i].compareData[1] = i + 2;
            multiCmpData[i].compareData[2] = i + 3;
            Console.Out.WriteLine(multiCmpData[i].compareData[0] + "," + multiCmpData[i].compareData[1] + "," + multiCmpData[i].compareData[2]);
        }
        Marshal.FreeHGlobal(pBuff);
        */
        public struct TMultiCmpData
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Int32[] compareData;
        };

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                              ECAT板卡状态 定义      
         说明：以下枚举针对IMC_GetECATMasterSts函数
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        public const uint EC_MASTER_IDLE = (0); // EtherCat尚未初始化
        public const uint EC_MASTER_INIT = (1); // EtherCat初始化
        public const uint EC_MASTER_SCAN_SLAVE = (2); // EtherCat正在扫描从站设备
        public const uint EC_MASTER_SCAN_SLAVE_END = (3); // EtherCat扫描从站设备结束
        public const uint EC_MASTER_SCAN_MODULES = (4); // EtherCat正在扫描从站设备MODULES
        public const uint EC_MASTER_SCAN_MODULES_END = (5); // EtherCat扫描从站设备MODULES结束
        public const uint EC_MASTER_OP = (6); // EtherCat进入OP状态
        public const uint EC_MASTER_ERR = (7); // EtherCat链路状态有错误

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                      ECAT板卡从站状态 定义      
         说明：以下枚举针对IMC_GetSlaveCurSts， IMC_GetSlaveReqSts函数
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        public const uint EC_SLAVE_STATE_UNKNOWN = (0x00); // EtherCat从站在未知状态
        public const uint EC_SLAVE_STATE_INIT = (0x01); // EtherCat从站在初始状态
        public const uint EC_SLAVE_STATE_PREOP = (0x02); // EtherCat从站在PREOP状态
        public const uint EC_SLAVE_STATE_BOOT = (0x03); // EtherCat从站在BOOT状态
        public const uint EC_SLAVE_STATE_SAFEOP = (0x04); // EtherCat从站在SAVEOP状态
        public const uint EC_SLAVE_STATE_OP = (0x08); // EtherCat从站在OP状态
        public const uint EC_SLAVE_STATE_ACK_ERR = (0x10); // EtherCat从站有错误

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                      ecat Error code 定义      
         说明：以下宏定义针对IMC_GetEcatErrCode函数,其他为内部错误
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //public const uint ERROR_CODE_NO_MASTER = (0x0001);      //不存在ECAT MASTER设备
        //public const uint ERROR_CODE_INVALID_DOMAIN = (0x0002);      //非法Domain域
        public const uint ERROR_CODE_NO_SUCH_SLAVE = (0x0003);      //没有这个从站
        public const uint ERROR_CODE_INVALID_PDO = (0x0004);      //非法PDO
        public const uint ERROR_CODE_INVALID_SDO = (0x0005);      //非法SDO
        public const uint ERROR_CODE_INVALID_ENTRY = (0x0006);      //非法ENTRY   
        //public const uint ERROR_CODE_NOMEM = (0x0007);      //ECAT协议栈分配内存失败   
        //public const uint ERROR_CODE_MASTER_FAIL = (0x0008);      //启动MASTER主站失败  
        //public const uint ERROR_CODE_COMMON_FAIL = (0x0009);      //通信失败 
        //public const uint ERROR_CODE_CYCCOM_FAIL = (0x000A);      //周期定时器失败        
        //public const uint ERROR_CODE_BUFFCFG_FAIL = (0x000B);      //PDO buffer配置失败      
        //public const uint ERROR_CODE_INITMODULFAIL = (0x000C);      //初始化Module失败      
        public const uint ERROR_CODE_PASECFGFAIL = (0x000D);      //解析XML设备配置失败 
        //public const uint ERROR_CODE_CFGECATDSPFAIL = (0x000E);      //同步配置到DSP失败
        //public const uint ERROR_CODE_DOMAINREGFAIL = (0x000F);      //Domain域配置信息有误
        //public const uint ERROR_CODE_CREATIMERFAIL = (0x0010);      //创建定时器失败
        //public const uint ERROR_CODE_STARTIMERFAIL = (0x0011);      //启动定时器失败
        //public const uint ERROR_CODE_CFGVERSELFAIL = (0x0012);      //获取版本信息失败
        //public const uint ERROR_CODE_CFGMEMORYFAIL = (0x0013);      //分配过程数据buffer失败
        //public const uint ERROR_CODE_MEMWARNINFAIL = (0x0014);      //创建共享数据失败
        public const uint ERROR_CODE_CFGREGISTFAIL = (0x0015);      //配置reg失败
        public const uint ERROR_CODE_CFGDIFFONLINE = (0x0016);      //在线从站与配置不一致
        public const uint ERROR_CODE_SLAVE_OFFLINE = (0x001a);      //从站掉线错误，高8位为轴号，即bit[15:8]-轴号  
        public const uint ERROR_CODE_SDOBF_NONECAT = (0x001b);      //SDO缓冲区收到非ECAT帧错误
        public const uint ERROR_CODE_PORT0_NOTLINK = (0x001c);      //端口未接ECAT设备错误
        //public const uint ERROR_CODE_SETIR_STARTIM_ERR = (0x001d);      //开始DC及中断工作发送错误
        public const uint ERROR_CODE_SET_CYCLETIME_PARA_ERR = (0x001e);      //设置周期时间参数错误
        public const uint ERROR_CODE_COE_SDO_INIT_ERR = (0x001f);      //在初始化阶段coe配置错误
        public const uint ERROR_CODE_SLAVE_STATE_ERR = (0x0020);      //从站状态错误，高8位为轴号，即bit[15:8]-轴号    
        //public const uint ERROR_CODE_WR_STACMD_ERR = (0x0021);      //写写命令字失败
        //public const uint ERROR_CODE_RDSTS_REG_ERR = (0x0022);      //读状态字失败
        //public const uint ERROR_CODE_RD_DLLERR_ERR = (0x0023);      //读取DLL失败
        //public const uint ERROR_CODE_RD_SDOBF_ERR = (0x0024);      //读SDO缓冲区错误
        //public const uint ERROR_CODE_RD_ERROR_CODE_SDOBF_LEN_ERR = (0x0025);      //读SDO缓冲区长度错误
        //public const uint ERROR_CODE_SDOBF_LEN_ERR = (0x0026);      //SDO长度错误
        //public const uint ERROR_CODE_SDOBF_RCV_ERR = (0x0027);      //接收SDO错误
        //public const uint ERROR_CODE_SDOBF_BUSY_ERR = (0x0028);      //SDO操作忙状态
        //public const uint ERROR_CODE_SDOBF_DATAGRAM = (0x0029);      //SDO数据帧错误
        //public const uint ERROR_CODE_RD_PDOBF_ERR = (0x002a);      //读取PDO缓冲区错误
        //public const uint ERROR_CODE_RD_ERROR_CODE_PDOBF_LEN_ERR = (0x002b);      //读取PDO缓冲区长度错误
        //public const uint ERROR_CODE_PDOBF_LEN_ERR = (0x002c);      //PDO长度错误
        //public const uint ERROR_CODE_PDOBF_RCV_ERR = (0x002d);      //PDO接收错误
        //public const uint ERROR_CODE_NETCARDOPEN_ERR = (0x002e);      //打开网卡失败
        //public const uint ERROR_CODE_GPMC_IOCTRL_ERR = (0x002f);      //打开GPMC失败
        //public const uint ERROR_CODE_GPMC_ECATRD_ERR = (0x0030);      //GPMC读ECAT失败
        //public const uint ERROR_CODE_GPMC_ECATWR_ERR = (0x0031);      //GPMC写ECAT失败
        //public const uint ERROR_CODE_RD_TXTMSTMP_ERR = (0x0032);      //GPMC读取ECAT发送时间失败
        //public const uint ERROR_CODE_RD_RXTMSTMP_ERR = (0x0033);      //GPMC读取ECAT接收时间失败
        //public const uint ERROR_CODE_RD_PDOBFLFT_ERR = (0x0034);      //GPMC读取ECAT PDO缓冲区剩余失败
        //public const uint ERROR_CODE_RD_APPTIME_ERR = (0x0035);      //GPMC读取ECAT APP时间失败
        //public const uint ERROR_CODE_WR_BUFF_ERR = (0x0036);      //写缓冲区错误 
        public const uint ERROR_CODE_SLAVE_SII_ERR = (0x0037);      //E2ROM 信息有误,处理方法：一般为从站保存的e2rom信息有误，可以使用twincat确认并联系厂家。

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
             abort code 定义      
         说明：以下宏定义针对IIMC_GetEcatSdo\IMC_SetEcatSdo函数      
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        public const uint ABORT_CODE1 = (0x05030000); //从站Toggle bit 没有变化
        public const uint ABORT_CODE2 = (0x05040000); //SDO 访问超时
        public const uint ABORT_CODE3 = (0x05040001); //客户端/服务器 命令非法或未知
        public const uint ABORT_CODE4 = (0x05040005); //内存溢出
        public const uint ABORT_CODE5 = (0x06010000); //不支持访问该对象
        public const uint ABORT_CODE6 = (0x06010001); //尝试去读一个只写对象
        public const uint ABORT_CODE7 = (0x06010002); //尝试去写一个只读对象
        public const uint ABORT_CODE8 = (0x06020000); //对象字典中不存在该对象
        public const uint ABORT_CODE9 = (0x06040041); //该对象不能映射成PDO
        public const uint ABORT_CODE10 = (0x06040042); //对象映射成PDO超出 PDO长度
        public const uint ABORT_CODE11 = (0x06040043); //通用参数非法
        public const uint ABORT_CODE12 = (0x06040047); //设备内不兼容
        public const uint ABORT_CODE13 = (0x06060000); //由于硬件原因访问失败
        public const uint ABORT_CODE14 = (0x06070010); //数据类型不匹配，长度参数
        public const uint ABORT_CODE15 = (0x06070012); //数据类型不匹配，长度太大
        public const uint ABORT_CODE16 = (0x06070013); //数据类型不匹配，长度太小
        public const uint ABORT_CODE17 = (0x06090011); //该对象子索引不存在
        public const uint ABORT_CODE18 = (0x06090030); //参数超出范围
        public const uint ABORT_CODE19 = (0x06090031); //参数超出范围太大
        public const uint ABORT_CODE20 = (0x06090032); //参数超出范围太小
        public const uint ABORT_CODE21 = (0x06090036); //最大值小于最小值
        public const uint ABORT_CODE22 = (0x08000000); //一般错误
        public const uint ABORT_CODE23 = (0x08000020); //数据不能被传输或被保存
        public const uint ABORT_CODE24 = (0x08000021); //数据不能被传输或被保存由于本地控制
        public const uint ABORT_CODE25 = (0x08000022); //数据不能被传输或被保存由于当前状态
        public const uint ABORT_CODE26 = (0x08000023); //缺乏对象字典或者对象字典创建失败

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
             伺服操作模式      
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		public const uint TQ_OP_MODE = (4);
		public const uint HM_OP_MODE = (6);
		public const uint CSP_OP_MODE = (8);
		public const uint CSV_OP_MODE = (9);
		public const uint CST_OP_MODE = (10);

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
             回零模式      
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		public const uint HOMING_MODE_CIA = (0);
		public const uint HOMING_MODE_TORQ = (1);
		public const uint HOMING_MODE_ECAT_CSP = (2);
		
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
             端子板专用IO的宏定义      
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        public const uint SPECIAL_IO_RDY = (0);  // 准备完成信号
        public const uint SPECIAL_IO_ARRIV = (1);  // 到位信号
        public const uint SPECIAL_IO_ALARM = (2);  // 报警信号
        public const uint SPECIAL_IO_POSLMT = (3);  // 正限位信号
        public const uint SPECIAL_IO_NEGLMT = (4);  // 负限位信号
        public const uint SPECIAL_IO_CLR = (5);  // 清除报警信号
        public const uint SPECIAL_IO_SV = (6);  // 伺服使能信号
        public const uint SPECIAL_IO_HOME = (7);  // 回零输入信号
        public const uint SPECIAL_IO_INDEX = (8);  // 电机Z相信号
        #endregion

        #region FUNCTION DEFINE

        ///*==========================================================================*/
        ///*----             FUNCTION DEFINE                                       ---*/
        ///*==========================================================================*/

        ///*==========================================================================*/
        ///*----1.1 板卡的操作以及资源的获取接口                                   ---*/
        ///*==========================================================================*/
        //_DLL_API UINT32 IMC_GetCardsNum(INT32 *pCardsNum);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCardsNum", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCardsNum(ref Int32 cardsNum);

        //_DLL_API UINT32 IMC_OpenCard(INT32 cardNo, UINT32 *pCardHandle, INT32 blockFlag);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_OpenCard", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_OpenCard(Int32 cardNo, ref HANDLETYPE pCardHandle, Int32 blockFlag);

        //_DLL_API UINT32 IMC_CloseCard(HANDLETYPE cardHandle);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CloseCard", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CloseCard(HANDLETYPE cardHandle);


        //_DLL_API UINT32 IMC_OpenCardHandle(INT32 cardNo, UINT32 *pCardHandle);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_OpenCardHandle", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_OpenCardHandle(Int32 cardNo, ref HANDLETYPE pCardHandle);

        //_DLL_API UINT32 IMC_ScanCardECAT(HANDLETYPE cardHandle, INT32 blockFlag);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ScanCardECAT", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ScanCardECAT(HANDLETYPE cardHandle, Int32 blockFlag);

        //_DLL_API UINT32 IMC_GetECATMasterSts(HANDLETYPE cardHandle,UINT32 *pStatus);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetECATMasterSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetECATMasterSts(HANDLETYPE cardHandle, ref UInt32 pStatus);

        //_DLL_API UINT32 IMC_ScanCard(UINT64 cardHandle, INT32 blockFlag);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ScanCard", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ScanCard(HANDLETYPE cardHandle, Int32 blockFlag);
        //_DLL_API UINT32 IMC_GetCardSts(UINT64 cardHandle,UINT32 *pStatus);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCardSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCardSts(HANDLETYPE cardHandle, ref UInt32 pStatus);

        //_DLL_API UINT32 IMC_CloseCardHandle(HANDLETYPE cardHandle);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CloseCardHandle", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CloseCardHandle(HANDLETYPE cardHandle);

        //_DLL_API UINT32 IMC_GetCardResource(HANDLETYPE cardHandle, TRsouresNum *pRsouresNum);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCardResource", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCardResource(HANDLETYPE cardHandle, ref TRsouresNum pRsouresNum);

        //_DLL_API UINT32 IMC_UpLoadDeviceConfig(HANDLETYPE cardHandle,const char *pathName);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_UpLoadDeviceConfig", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_UpLoadDeviceConfig(HANDLETYPE cardHandle, string pathName);

        //_DLL_API UINT32 IMC_DownLoadDeviceConfig(HANDLETYPE cardHandle,const char *pathName);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DownLoadDeviceConfig", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DownLoadDeviceConfig(HANDLETYPE cardHandle, string pathName);

        //_DLL_API UINT32 IMC_DownLoadSystemConfig(HANDLETYPE cardHandle, const char *pathName);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DownLoadSystemConfig", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DownLoadSystemConfig(HANDLETYPE cardHandle, string pathName);


        //_DLL_API UINT32 IMC_GetBlockFunExeAndRet(UINT64 cardHandle, UINT32 *pExeSts, UINT32 *pRet);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetBlockFunExeAndRet", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetBlockFunExeAndRet(HANDLETYPE cardHandle, ref UInt32 pExeSts, ref UInt32 pRet);

        //_DLL_API UINT32 IMC_API_DelMaster(UINT64 cardHandle);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_API_DelMaster", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_API_DelMaster(HANDLETYPE cardHandle);

        //_DLL_API UINT32 IMC_StartNetTest(UINT64 cardHandle, char *netAddr);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartNetTest", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartNetTest(HANDLETYPE cardHandle, string netAddr);

        //_DLL_API UINT32 IMC_EndNetTest(UINT64 cardHandle, UINT32 *loss_ratio, UINT32 *pSendCnt, UINT32 *pRcvCnt);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EndNetTest", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EndNetTest(HANDLETYPE cardHandle, ref UInt32 loss_ratio, ref UInt32 pSendCnt, ref UInt32 pRcvCnt);

        //_DLL_API UINT32 IMC_DownLoadUpdateFile(UINT64 cardHandle, const char *pathName);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DownLoadUpdateFile", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DownLoadUpdateFile(HANDLETYPE cardHandle, string pathName);

        //_DLL_API UINT32 IMC_UpdateSysFw(UINT64 cardHandle);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_UpdateSysFw", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_UpdateSysFw(HANDLETYPE cardHandle);

        //_DLL_API UINT32 IMC_InstallSysFw(UINT64 cardHandle);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_InstallSysFw", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_InstallSysFw(HANDLETYPE cardHandle);

        //_DLL_API UINT32 IMC_GetChangeSysFwSts(UINT64 cardHandle, INT16 *pSts);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetChangeSysFwSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetChangeSysFwSts(HANDLETYPE cardHandle, ref Int16 pSts);

        //_DLL_API UINT32 IMC_GetWaitRstTime(UINT64 cardHandle, INT32 *pTime);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetWaitRstTime", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetWaitRstTime(HANDLETYPE cardHandle, ref Int32 pTime);

        //_DLL_API UINT32 IMC_GetHwVer(UINT64 cardHandle, INT16 *pVer);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetHwVer", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetHwVer(HANDLETYPE cardHandle, ref Int16 pVer);


        //测试
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CheckConfRight", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CheckConfRight(HANDLETYPE cardHandle, ref Int16 pPassword);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetSysMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetSysMode(HANDLETYPE cardHandle, ref Int16 pSysMode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetSysMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetSysMode(HANDLETYPE cardHandle, Int16 sysMode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxisNum", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxisNum(HANDLETYPE cardHandle, ref Int16 pAxisNum);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxisNum", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxisNum(HANDLETYPE cardHandle, Int16 axisNum);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_QuitConfig", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_QuitConfig(HANDLETYPE cardHandle);






        ///*==========================================================================*/
        ///*----1.2.1 ECAT资源设置接口                                             ---*/
        ///*==========================================================================*/
        //_DLL_API UINT32 IMC_SetEcatGrpDiInverse(HANDLETYPE cardHandle, INT16 groupNo, INT16 inverse);
        //_DLL_API UINT32 IMC_GetEcatGrpDiInverse(HANDLETYPE cardHandle, INT16 groupNo, INT16 *pInverse);
        //_DLL_API UINT32 IMC_SetEcatGrpDoInverse(HANDLETYPE cardHandle, INT16 groupNo, INT16 inverse);
        //_DLL_API UINT32 IMC_GetEcatGrpDoInverse(HANDLETYPE cardHandle, INT16 groupNo, INT16 *pInverse);

        //_DLL_API UINT32 IMC_SetEcatAxDirInv(HANDLETYPE cardHandle, INT16 chn, INT16 inverse);
        //_DLL_API UINT32 IMC_GetEcatAxDirInv(HANDLETYPE cardHandle, INT16 chn, INT16 *pInverse);

        //_DLL_API UINT32 IMC_SetEcatAdcPara(HANDLETYPE cardHandle, INT16 AdNo,INT16 inputType,INT16 filter,INT16 superSetPara); 
        //_DLL_API UINT32 IMC_GetEcatAdcPara(HANDLETYPE cardHandle, INT16 AdNo,INT16 *pInputType,INT16 *pFilter,INT16 *pSuperSetPara); 

        //_DLL_API UINT32 IMC_SetEcatDacPara(HANDLETYPE cardHandle, INT16 DaNo,INT16 outputType,INT16 stopOutputVal); 
        //_DLL_API UINT32 IMC_GetEcatDacPara(HANDLETYPE cardHandle, INT16 DaNo,INT16 *pOutputType,INT16 *pStopOutputVal); 

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatGrpDiInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatGrpDiInverse(HANDLETYPE cardHandle, Int16 groupNo, Int16 inverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatGrpDiInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatGrpDiInverse(HANDLETYPE cardHandle, Int16 groupNo, ref Int16 pInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatGrpDoInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatGrpDoInverse(HANDLETYPE cardHandle, Int16 groupNo, Int16 inverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatGrpDoInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatGrpDoInverse(HANDLETYPE cardHandle, Int16 groupNo, ref Int16 pInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatAxDirInv", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatAxDirInv(HANDLETYPE cardHandle, Int16 chn, Int16 inverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAxDirInv", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAxDirInv(HANDLETYPE cardHandle, Int16 chn, ref Int16 pInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatAdcPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatAdcPara(HANDLETYPE cardHandle, Int16 AdNo, Int16 inputType, Int16 filter, Int16 superSetPara);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAdcPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAdcPara(HANDLETYPE cardHandle, Int16 AdNo, ref Int16 pInputType, ref Int16 pFilter, ref Int16 pSuperSetPara);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatDacPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_SetEcatDacPara(HANDLETYPE cardHandle, Int16 DaNo, Int16 outputType, Int16 stopOutputVal);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatDacPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatDacPara(HANDLETYPE cardHandle, Int16 DaNo, ref Int16 pOutputType, ref Int16 pStopOutputVal);
        ///*==========================================================================*/
        ///*----1.2.2 端子板资源设置接口                                           ---*/
        ///*==========================================================================*/
        //_DLL_API UINT32 IMC_GetBoardWorkSts(HANDLETYPE cardHandle, INT16 *pValue);
        //_DLL_API UINT32 IMC_GetBoardDiagSts(UINT64 cardHandle, INT16 *pValue);
        //_DLL_API UINT32 IMC_SetLocalDiFilterTime(HANDLETYPE cardHandle, INT16 diIndex, UINT16 *pFilterTime, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetLocalDiFilterTime(HANDLETYPE cardHandle, INT16 diIndex, UINT16 *pFilterTime, INT16 count = 1);
        //_DLL_API UINT32 IMC_SetLocalHmLmtPrbFilterTime(HANDLETYPE cardHandle, INT16 homeFltTime, INT16 limitFltTime, INT16 probeFltTime);
        //_DLL_API UINT32 IMC_GetLocalHmLmtPrbFilterTime(HANDLETYPE cardHandle, INT16 *pHomeFltTime, INT16 *pLimitFltTime, INT16 *pProbeFltTime);
        //_DLL_API UINT32 IMC_SetLocalSpecialIOInverse(HANDLETYPE cardHandle, INT16 type, INT16 inverse);
        //_DLL_API UINT32 IMC_GetLocalSpecialIOInverse(HANDLETYPE cardHandle, INT16 type, INT16 *pInverse);
        //_DLL_API UINT32 IMC_SetLocalDiInverse(HANDLETYPE cardHandle, INT16 inverse);
        //_DLL_API UINT32 IMC_GetLocalDiInverse(HANDLETYPE cardHandle, INT16 *pInverse);
        //_DLL_API UINT32 IMC_SetLocalDoInverse(HANDLETYPE cardHandle, INT16 inverse);
        //_DLL_API UINT32 IMC_GetLocalDoInverse(HANDLETYPE cardHandle, INT16 *pInverse);
        //_DLL_API UINT32 IMC_SetLocalEncDir(HANDLETYPE cardHandle, INT16 index, INT16 *pDirArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetLocalEncDir(HANDLETYPE cardHandle, INT16 index, INT16 *pDirArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_SetLocalPulseMode(HANDLETYPE cardHandle, INT16 plsIndex, INT16 mode, INT16 dirInverse);
        //_DLL_API UINT32 IMC_GetLocalPulseMode(HANDLETYPE cardHandle, INT16 plsIndex, INT16 *pMode, INT16 *pDirInverse);
        //_DLL_API UINT32 IMC_SetLocalGpoUseType(UINT64 cardHandle, INT16 index, INT16 type);
        //_DLL_API UINT32 IMC_GetLocalGpoUseType(UINT64 cardHandle, INT16 index, INT16 *pType);
        //_DLL_API UINT32 IMC_SetEncFilterPara(UINT64 cardHandle, INT16 index, INT16 filterDepth, INT16 filterCoef);
        //_DLL_API UINT32 IMC_GetEncFilterPara(UINT64 cardHandle, INT16 index, INT16 *pFilterDepth, INT16 *pFilterCoef);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetBoardWorkSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetBoardWorkSts(HANDLETYPE cardHandle, ref Int16 pValue);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetBoardDiagSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetBoardDiagSts(HANDLETYPE cardHandle, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalDiFilterTime", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalDiFilterTime(HANDLETYPE cardHandle, Int16 diIndex, UInt16[] pFilterTime, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalDiFilterTime", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalDiFilterTime(HANDLETYPE cardHandle, Int16 diIndex, UInt16[] pFilterTime, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalHmLmtPrbFilterTime", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalHmLmtPrbFilterTime(HANDLETYPE cardHandle, Int16 homeFltTime, Int16 limitFltTime, Int16 probeFltTime);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalHmLmtPrbFilterTime", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalHmLmtPrbFilterTime(HANDLETYPE cardHandle, ref Int16 pHomeFltTime, ref Int16 pLimitFltTime, ref Int16 pProbeFltTime);

        //public const uint SPECIAL_IO_RDY = (0);   // 准备完成信号
        //public const uint SPECIAL_IO_ARRIV = (1);   // 到位信号
        //public const uint SPECIAL_IO_ALARM = (2);   // 报警信号
        //public const uint SPECIAL_IO_POSLMT = (3);   // 正限位信号
        //public const uint SPECIAL_IO_NEGLMT = (4);   // 负限位信号
        //public const uint SPECIAL_IO_CLR = (5);   // 清除报警信号
        //public const uint SPECIAL_IO_SV = (6);   // 伺服使能信号

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalSpecialIOInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalSpecialIOInverse(HANDLETYPE cardHandle, Int16 type, Int16 inverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalSpecialIOInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalSpecialIOInverse(HANDLETYPE cardHandle, Int16 type, ref Int16 pInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalDiInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalDiInverse(HANDLETYPE cardHandle, Int16 inverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalDiInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalDiInverse(HANDLETYPE cardHandle, ref Int16 pInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalDoInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalDoInverse(HANDLETYPE cardHandle, Int16 inverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalDoInverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalDoInverse(HANDLETYPE cardHandle, ref Int16 pInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalEncDir", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalEncDir(HANDLETYPE cardHandle, Int16 index, Int16[] pDirArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalEncDir", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalEncDir(HANDLETYPE cardHandle, Int16 index, Int16[] pDirArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalPulseMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalPulseMode(HANDLETYPE cardHandle, Int16 plsIndex, Int16 mode, Int16 dirInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalPulseMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalPulseMode(HANDLETYPE cardHandle, Int16 plsIndex, ref Int16 pMode, ref Int16 pDirInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalGpoUseType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalGpoUseType(HANDLETYPE cardHandle, Int16 index, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalGpoUseType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalGpoUseType(HANDLETYPE cardHandle, Int16 index, ref Int16 pType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEncFilterPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEncFilterPara(HANDLETYPE cardHandle, Int16 index, Int16 filterDepth, Int16 filterCoef);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEncFilterPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEncFilterPara(HANDLETYPE cardHandle, Int16 index, ref Int16 pFilterDepth, ref Int16 pFilterCoef);

        ///*==========================================================================*/
        ///*----1.2.3 系统参数设置接口                                             ---*/
        ///*==========================================================================*/
        //_DLL_API UINT32 IMC_GetHwSysPara(HANDLETYPE cardHandle, INT16 *pCycleTime, INT16 *pHwType);
        //_DLL_API UINT32 IMC_ResetSysPara(HANDLETYPE cardHandle);
        //_DLL_API UINT32 IMC_GetCalcLoadRatio(HANDLETYPE cardHandle,double *pLoadRatio);
        //_DLL_API UINT32 IMC_GetVersion (HANDLETYPE cardHandle, INT16 *pVersion);
        //_DLL_API UINT32 IMC_WriteUserCode(HANDLETYPE cardHandle, char *pCodeArray, INT16 len);
        //_DLL_API UINT32 IMC_CheckUserCode(HANDLETYPE cardHandle, char *pCodeArray, INT16 len);

        //_DLL_API UINT32 IMC_SetEmgStopMode(HANDLETYPE cardHandle,INT16 stopMode);
        //_DLL_API UINT32 IMC_GetEmgStopMode(HANDLETYPE cardHandle,INT16 *pStopMode);
        //_DLL_API UINT32 IMC_SetEmgFilter(HANDLETYPE cardHandle,INT16 filter);
        //_DLL_API UINT32 IMC_GetEmgFilter(HANDLETYPE cardHandle,INT16 *pFilter);
        //_DLL_API UINT32 IMC_SetEmgTrigLevelInv(HANDLETYPE cardHandle, INT16 inverse);
        //_DLL_API UINT32 IMC_GetEmgTrigLevelInv(HANDLETYPE cardHandle, INT16 *pInverse);
        //_DLL_API UINT32 IMC_GetEmgDiLevel (HANDLETYPE cardHandle,INT16 * pLevel);
        //_DLL_API UINT32 IMC_SetEmgDoResetFlag(UINT64 cardHandle,INT16 enable);

        //_DLL_API UINT32 IMC_OpenWatchDog(UINT64 cardHandle,INT32 feedTime);
        //_DLL_API UINT32 IMC_FeedWatchDog(UINT64 cardHandle);
        //_DLL_API UINT32 IMC_CloseWatchDog(UINT64 cardHandle);
        //_DLL_API UINT32 IMC_EnableCheckEcatErrCode(UINT64 cardHandle,INT16 flag);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetHwSysPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetHwSysPara(HANDLETYPE cardHandle, ref Int16 pCycleTime, ref Int16 pHwType);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ResetSysPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ResetSysPara(HANDLETYPE cardHandle);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCalcLoadRatio", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCalcLoadRatio(HANDLETYPE cardHandle, ref double pLoadRatio);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetVersion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetVersion(HANDLETYPE cardHandle, Int16[] pVersion);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_WriteUserCode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_WriteUserCode(HANDLETYPE cardHandle, string pCodeArray, Int16 len);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CheckUserCode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CheckUserCode(HANDLETYPE cardHandle, string pCodeArray, Int16 len);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEmgStopMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEmgStopMode(HANDLETYPE cardHandle, Int16 stopMode);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEmgStopMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEmgStopMode(HANDLETYPE cardHandle, ref Int16 pStopMode);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEmgFilter", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEmgFilter(HANDLETYPE cardHandle, Int16 filter);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEmgFilter", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEmgFilter(HANDLETYPE cardHandle, ref Int16 pFilter);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEmgTrigLevelInv", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEmgTrigLevelInv(HANDLETYPE cardHandle, Int16 inverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEmgTrigLevelInv", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEmgTrigLevelInv(HANDLETYPE cardHandle, ref Int16 pInverse);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEmgDiLevel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEmgDiLevel(HANDLETYPE cardHandle, ref Int16 pLevel);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEmgDoResetFlag", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEmgDoResetFlag(HANDLETYPE cardHandle, Int16 enable);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEmgDoResetFlag", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEmgDoResetFlag(HANDLETYPE cardHandle, ref Int16 pEnFlag);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEmgDoResetFlag", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEmgSvOffVelThreshold(HANDLETYPE cardHandle, Int16 axNo, Int32 thrVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEmgDoResetFlag", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEmgSvOffVelThreshold(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pThrVel);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_OpenWatchDog", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_OpenWatchDog(HANDLETYPE cardHandle, Int32 feedTime);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FeedWatchDog", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_FeedWatchDog(HANDLETYPE cardHandle);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CloseWatchDog", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CloseWatchDog(HANDLETYPE cardHandle);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EnableCheckEcatErrCode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EnableCheckEcatErrCode(HANDLETYPE cardHandle, Int16 flag);

        ///*==========================================================================*/
        ///*----1.2.4 轴参数设置接口                                               ---*/
        ///*==========================================================================*/
        //_DLL_API UINT32 IMC_SetAxActive(UINT64 cardHandle, INT16 axNo, INT16 active);
        //_DLL_API UINT32 IMC_GetAxActive(UINT64 cardHandle, INT16 axNo, INT16 *pActive);
        //_DLL_API UINT32 IMC_SetAxMaxMtPara(HANDLETYPE cardHandle, INT16 axNo, TMtPara *pMtPara);
        //_DLL_API UINT32 IMC_GetAxMaxMtPara(HANDLETYPE cardHandle, INT16 axNo, TMtPara *pMtPara);
        ////_DLL_API UINT32 IMC_SetAxEquiv(HANDLETYPE cardHandle, INT16 axNo, double *pAxEquArray,INT16 count=1);
        ////_DLL_API UINT32 IMC_GetAxEquiv(HANDLETYPE cardHandle, INT16 axNo, double *pAxEquArray,INT16 count=1);
        //_DLL_API UINT32 IMC_SetAxBondCfg(HANDLETYPE cardHandle, INT16 axNo, INT16 axType, INT16 outputChn, INT16 loaclEncSrc);
        //_DLL_API UINT32 IMC_GetAxBondCfg(HANDLETYPE cardHandle, INT16 axNo, INT16 *pAxType, INT16 *pOutputChn, INT16 *pLoaclEncSrc);
        //_DLL_API UINT32 IMC_ResetAxBondCfg(UINT64 cardHandle);
        //_DLL_API UINT32 IMC_SetAxAttriPara(HANDLETYPE cardHandle, INT16 axNo, TAxAttriPara *pAxAttriPara);
        //_DLL_API UINT32 IMC_GetAxAttriPara(HANDLETYPE cardHandle, INT16 axNo, TAxAttriPara *pAxAttriPara);
        //_DLL_API UINT32 IMC_SetAxSoftLimit(HANDLETYPE cardHandle, INT16 axNo, INT32 softPosLimitPos, INT32 softNegLimitPos);
        //_DLL_API UINT32 IMC_GetAxSoftLimit(HANDLETYPE cardHandle, INT16 axNo, INT32 *pSoftPosLimitPos, INT32 *pSoftNegLimitPos);
        //_DLL_API UINT32 IMC_SetAxArrivalBand(HANDLETYPE cardHandle, INT16 axNo, INT16 arrivalBand, INT16 arrivalTime);
        //_DLL_API UINT32 IMC_GetAxArrivalBand(HANDLETYPE cardHandle, INT16 axNo, INT16 *pArrivalBand, INT16 *pArrivalTime);
        //_DLL_API UINT32 IMC_SetAxErrorPos(HANDLETYPE cardHandle, INT16 axNo, INT32 errorPos);
        //_DLL_API UINT32 IMC_GetAxErrorPos(HANDLETYPE cardHandle, INT16 axNo, INT32 *pErrorPos);
        //_DLL_API UINT32 IMC_SetAxBacklash (HANDLETYPE cardHandle,INT16 axNo, INT32 wholeCmpVal, INT32 cmpVel, INT16 cmpDir);
        //_DLL_API UINT32 IMC_GetAxBacklash (HANDLETYPE cardHandle,INT16 axNo, INT32 *pWholeCmpVal, INT32 *pCmpVel, INT16 *pCmpDir);
        //_DLL_API UINT32 IMC_AxSafeCheckEnable(HANDLETYPE cardHandle, INT16 axNo, TAxCheckEn *pAxCheckEn);
        //_DLL_API UINT32 IMC_AxSafeCheckSts(HANDLETYPE cardHandle, INT16 axNo, TAxCheckEn *pAxCheckEn);
        //_DLL_API UINT32 IMC_AxAlarmEnable(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxAlarmDisable(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxSoftLmtsEnable(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxSoftLmtsDisable(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxHwLmtsEnable(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxHwLmtsDisable(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxErrPosEnable(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxErrPosDisable(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_SetupAxGantry(UINT64 cardHandle,INT16 groupNo,INT16 masterAxNo,INT16 slaveAxNo,INT32 errorLmt);
        //_DLL_API UINT32 IMC_AbortAxGantry(UINT64 cardHandle,INT16 groupNo);
        //_DLL_API UINT32 IMC_GetAxGantrySts(UINT64 cardHandle,INT16 groupNo,INT16 *pMasterAxNo,INT16 *pSlaveAxNo,INT32 *pErrorLmt,INT16 *pSts);
        //_DLL_API UINT32 IMC_ClrAxGantrySts(UINT64 cardHandle,INT16 groupNo);
        //_DLL_API UINT32 IMC_AxSetScrewCompTable(UINT64 cardHandle, INT16 axNo , INT16 dir ,INT16 paraNum,INT32 *pCompValArray);
        //_DLL_API UINT32 IMC_AxGetScrewCompTable(UINT64 cardHandle, INT16 axNo , INT16 dir , INT16 *pParaNum,INT32 *pCompValArray);
        //_DLL_API UINT32 IMC_AxEnableScrewComp(UINT64 cardHandle,INT16 axNo, INT32 posStartPos, INT32 posEndPos ,INT16 pointNum);
        //_DLL_API UINT32 IMC_AxDisableScrewComp(UINT64 cardHandle,INT16 axNo);
        //_DLL_API UINT32 IMC_AxGetScrewCompStatus(UINT64 cardHandle,INT16 axNo,INT16 *pSts);
        //_DLL_API UINT32 IMC_AxGetScrewCompValue(UINT64 cardHandle,INT16 axNo,INT32 *pCompVal);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxActive", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxActive(HANDLETYPE cardHandle, Int16 axNo, Int16 active);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxActive", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxActive(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pActive);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxMaxMtPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxMaxMtPara(HANDLETYPE cardHandle, Int16 axNo, ref TMtPara pMtPara);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxMaxMtPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxMaxMtPara(HANDLETYPE cardHandle, Int16 axNo, ref TMtPara pMtPara);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxEquiv", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxEquiv(HANDLETYPE cardHandle, Int16 axNo, double[] pAxEquArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxEquiv", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxEquiv(HANDLETYPE cardHandle, Int16 axNo, double[] pAxEquArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxBondCfg", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxBondCfg(HANDLETYPE cardHandle, Int16 axNo, Int16 axType, Int16 outputChn, Int16 loaclEncSrc);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxBondCfg", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxBondCfg(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pAxType, ref Int16 pOutputChn, ref Int16 pLoaclEncSrc);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ResetAxBondCfg", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ResetAxBondCfg(HANDLETYPE cardHandle);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxAttriPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxAttriPara(HANDLETYPE cardHandle, Int16 axNo, ref TAxAttriPara pAxAttriPara);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxAttriPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxAttriPara(HANDLETYPE cardHandle, Int16 axNo, ref TAxAttriPara pAxAttriPara);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxSoftLimit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxSoftLimit(HANDLETYPE cardHandle, Int16 axNo, Int32 softPosLimitPos, Int32 softNegLimitPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxSoftLimit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxSoftLimit(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pSoftPosLimitPos, ref Int32 pSoftNegLimitPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxArrivalBand", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxArrivalBand(HANDLETYPE cardHandle, Int16 axNo, Int16 arrivalBand, Int16 arrivalTime);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxArrivalBand", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxArrivalBand(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pArrivalBand, ref Int16 pArrivalTime);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxErrorPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxErrorPos(HANDLETYPE cardHandle, Int16 axNo, Int32 errorPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxErrorPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxErrorPos(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pErrorPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxBacklash", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxBacklash(HANDLETYPE cardHandle, Int16 axNo, Int32 wholeCmpVal, Int32 cmpVel, Int16 cmpDir);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxBacklash", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxBacklash(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pWholeCmpVal, ref Int32 pCmpVel, ref Int16 pCmpDir);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxSafeCheckEnable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxSafeCheckEnable(HANDLETYPE cardHandle, Int16 axNo, ref TAxCheckEn pAxCheckEn);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxSafeCheckSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxSafeCheckSts(HANDLETYPE cardHandle, Int16 axNo, ref TAxCheckEn pAxCheckEn);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxAlarmEnable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxAlarmEnable(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxAlarmDisable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxAlarmDisable(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxSoftLmtsEnable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxSoftLmtsEnable(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxSoftLmtsDisable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxSoftLmtsDisable(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxHwLmtsEnable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxHwLmtsEnable(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxHwLmtsDisable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxHwLmtsDisable(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxErrPosEnable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxErrPosEnable(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxErrPosDisable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxErrPosDisable(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatAxProbeMaskBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatAxProbeMaskBit(HANDLETYPE cardHandle, Int16 axNo, Int16 prbDiBitNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAxProbeMaskBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAxProbeMaskBit(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pPrbDiBitNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetupAxGantry", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetupAxGantry(HANDLETYPE cardHandle, Int16 groupNo, Int16 masterAxNo, Int16 slaveAxNo, Int32 errorLmt);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AbortAxGantry", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AbortAxGantry(HANDLETYPE cardHandle, Int16 groupNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxGantrySts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxGantrySts(HANDLETYPE cardHandle, Int16 groupNo, ref Int16 pMasterAxNo, ref Int16 pSlaveAxNo, ref Int32 pErrorLmt, ref Int16 pSts);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ClrAxGantrySts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ClrAxGantrySts(HANDLETYPE cardHandle, Int16 groupNo);


		[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxSetScrewCompTable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxSetScrewCompTable(HANDLETYPE cardHandle, Int16 axNo , Int16 dir ,Int16 paraNum,Int32[] pCompValArray);
		
		[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxGetScrewCompTable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxGetScrewCompTable(HANDLETYPE cardHandle, Int16 axNo , Int16 dir ,ref Int16 paraNum,Int32[] pCompValArray);
		
		[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxEnableScrewComp", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxEnableScrewComp(HANDLETYPE cardHandle,Int16 axNo, Int32 posStartPos, Int32 posEndPos ,Int16 pointNum);
		
		[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxDisableScrewComp", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxDisableScrewComp(HANDLETYPE cardHandle,Int16 axNo);
		
		[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxGetScrewCompStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxGetScrewCompStatus(HANDLETYPE cardHandle,Int16 axNo,ref Int16 pSts);
		
		[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxGetScrewCompValue", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxGetScrewCompValue(HANDLETYPE cardHandle,Int16 axNo,ref Int32 pCompVal);

        ///*==========================================================================*/
        ///*----2.1 ECAT硬件操作接口                                               ---*/
        ///*==========================================================================*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----2.1.1ECAT总线 SDO操作以及状态查询                                  ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_GetSlaveCurSts(HANDLETYPE cardHandle, UINT16 Station_id, UINT16 cnt, INT16 *pStatusArray);
        //_DLL_API UINT32 IMC_GetSlaveReqSts(HANDLETYPE cardHandle, UINT16 Station_id, UINT16 cnt, INT16 *pStatusArray);
        //_DLL_API UINT32 IMC_GetEcatErrCode(HANDLETYPE cardHandle, UINT32 *pErrCode);
        //_DLL_API UINT32 IMC_GetAxEcatStation(UINT64 cardHandle, UINT16 virAxNo, INT16 *pPhyStation_id, INT16 *pPhySlot_id);
        //_DLL_API UINT32 IMC_GetEcatStationPidVid(UINT64 cardHandle, UINT16 Station_id, UINT32 *pPid, UINT32 *pVid);
        //_DLL_API UINT32 IMC_GetEcatSdo(HANDLETYPE cardHandle, INT16 station_id, UINT16 index,  UINT16 subindex, UINT8 *target, UINT32 target_size, UINT32 *result_size, UINT32 *abort_code);
        //_DLL_API UINT32 IMC_SetEcatSdo(HANDLETYPE cardHandle, INT16 station_id, UINT16 index,  UINT16 subindex, UINT8 *data,UINT32 data_size, UINT32 *abort_code);
        //_DLL_API UINT32 IMC_GetAxSdo(UINT64 cardHandle, INT16 virAxNo, UINT16 index,  UINT16 subindex, UINT8 *target, UINT32 target_size, UINT32 *result_size, UINT32 *abort_code);
        //_DLL_API UINT32 IMC_SetAxSdo(UINT64 cardHandle, INT16 virAxNo, UINT16 index,  UINT16 subindex, UINT8 *data,UINT32 data_size, UINT32 *abort_code);
        
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetSlaveCurSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetSlaveCurSts(HANDLETYPE cardHandle, UInt16 Station_id, UInt16 cnt, Int16[] pStatus);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetSlaveReqSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetSlaveReqSts(HANDLETYPE cardHandle, UInt16 Station_id, UInt16 cnt, Int16[] pStatus);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatErrCode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatErrCode(HANDLETYPE cardHandle, ref UInt32 pErrCode);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxEcatStation", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxEcatStation(HANDLETYPE cardHandle, UInt16 virAxNo, ref Int16 pPhyStation_id, ref Int16 pPhySlot_id);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxEcatPidVid", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxEcatPidVid(HANDLETYPE cardHandle, UInt16 virAxNo, ref UInt32 pPid, ref UInt32 pVid);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatStationPidVid", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatStationPidVid(HANDLETYPE cardHandle, UInt16 Station_id, ref UInt32 pPid, ref UInt32 pVid);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatSdo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatSdo(HANDLETYPE cardHandle, Int16 station_id, UInt16 index, UInt16 subindex, Byte[] target, UInt32 target_size, ref UInt32 result_size, ref UInt32 abort_code);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatSdo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatSdo(HANDLETYPE cardHandle, Int16 station_id, UInt16 index, UInt16 subindex, Byte[] data, UInt32 data_size, ref UInt32 abort_code);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxSdo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxSdo(HANDLETYPE cardHandle, Int16 virAxNo, UInt16 index, UInt16 subindex, Byte[] target, UInt32 target_size, ref UInt32 result_size, ref UInt32 abort_code);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxSdo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxSdo(HANDLETYPE cardHandle, Int16 virAxNo, UInt16 index, UInt16 subindex, Byte[] data, UInt32 data_size, ref UInt32 abort_code);
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----2.1.2 ECAT总线 DI/DO AI/AO操作						             ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_SetEcatDoBit(HANDLETYPE cardHandle, INT16 doNo, INT16 Value);
        //_DLL_API UINT32 IMC_GetEcatDoBit(HANDLETYPE cardHandle, INT16 doNo, INT16 *pValue);
        //_DLL_API UINT32 IMC_SetEcatGrpDo(HANDLETYPE cardHandle, INT16 groupNo, INT16 Value);
        //_DLL_API UINT32 IMC_GetEcatGrpDo(HANDLETYPE cardHandle, INT16 groupNo, INT16 *pValue);
        //_DLL_API UINT32 IMC_GetEcatDiBit(HANDLETYPE cardHandle, INT16 diNo, INT16 *pValue);
        //_DLL_API UINT32 IMC_GetEcatGrpDi(HANDLETYPE cardHandle, INT16 groupNo, INT16 *pValue);

        //_DLL_API UINT32 IMC_GetEcatAdVal(HANDLETYPE cardHandle, INT16 adNo, INT16 *pValue);
        //_DLL_API UINT32 IMC_SetEcatDaVal(HANDLETYPE cardHandle, INT16 daNo, INT16 Value);
        //_DLL_API UINT32 IMC_GetEcatDaVal(HANDLETYPE cardHandle, INT16 daNo, INT16 *pValue);

        //_DLL_API UINT32 IMC_GetEcatAdSts(HANDLETYPE cardHandle, INT16 adNo, INT16 *pSts);
        //_DLL_API UINT32 IMC_GetEcatDaSts(HANDLETYPE cardHandle, INT16 daNo, INT16 *pSts);

        //_DLL_API UINT32 IMC_GetEcatPtVal(UINT64 cardHandle, INT16 ptNo, double *pValue);

        //_DLL_API UINT32 IMC_EcatAdEnable(HANDLETYPE cardHandle, INT16 adNo, INT16 enable);
        //_DLL_API UINT32 IMC_EcatDaEnable(HANDLETYPE cardHandle, INT16 daNo, INT16 enable,INT16 stopSts);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatDoBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatDoBit(HANDLETYPE cardHandle, Int16 doNo, Int16 Value);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatDoBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatDoBit(HANDLETYPE cardHandle, Int16 doNo, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatGrpDo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatGrpDo(HANDLETYPE cardHandle, Int16 groupNo, Int16 Value);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatGrpDo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatGrpDo(HANDLETYPE cardHandle, Int16 groupNo, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatDiBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatDiBit(HANDLETYPE cardHandle, Int16 diNo, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatGrpDi", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatGrpDi(HANDLETYPE cardHandle, Int16 groupNo, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAdVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAdVal(HANDLETYPE cardHandle, Int16 adNo, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatDaVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatDaVal(HANDLETYPE cardHandle, Int16 daNo, Int16 Value);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatDaVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatDaVal(HANDLETYPE cardHandle, Int16 daNo, ref Int16 pValue);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAdSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetEcatAdSts(HANDLETYPE cardHandle, Int16 adNo, ref Int16 pSts);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatDaSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetEcatDaSts(HANDLETYPE cardHandle, Int16 daNo, ref Int16 pSts);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatPtVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatPtVal(HANDLETYPE cardHandle, Int16 ptNo, ref double pValue);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EcatAdEnable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EcatAdEnable(HANDLETYPE cardHandle, Int16 adNo, Int16 enable);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EcatDaEnable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_EcatDaEnable(HANDLETYPE cardHandle, Int16 daNo, Int16 enable, Int16 stopSts);

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----2.1.3 ECAT总线轴捕获操作                                           ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ////_DLL_API UINT32 IMC_SetEcatCaptMode (HANDLETYPE cardHandle, INT16 index,INT16 trigType, INT16 sns);
        ////_DLL_API UINT32 IMC_GetEcatCaptMode (HANDLETYPE cardHandle, INT16 index,INT16 *pTrigType,INT16 *pSns);
        ////_DLL_API UINT32 IMC_GetEcatCaptSts (HANDLETYPE cardHandle, INT16 index,INT16 *pSts,INT32 *pCaptPos);
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatCaptMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_SetEcatCaptMode(HANDLETYPE cardHandle, Int16 index, Int16 trigType, Int16 sns);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatCaptMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetEcatCaptMode(HANDLETYPE cardHandle, Int16 index, ref Int16 pTrigType, ref Int16 pSns);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatCaptSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetEcatCaptSts(HANDLETYPE cardHandle, Int16 index, ref Int16 pSts, ref Int32 pCaptPos);

        ///*==========================================================================*/
        ///*----2.2 端子板硬件操作接口                                             ---*/
        ///*==========================================================================*/
        /// 获取端子板版本信息
        /// _DLL_API UINT32 IMC_GetBoardVersion(UINT64 cardHandle, INT16 *pVersion, INT16 *pMonth,INT16 *pYear);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetBoardVersion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetBoardVersion(HANDLETYPE cardHandle, ref Int16 pVersion, ref Int16 pMonth, ref Int16 pYear);

        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----2.2.1 DIO操作接口                                                  ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_SetLocalDoBit(HANDLETYPE cardHandle, INT16 doNo, INT16 value);
        //_DLL_API UINT32 IMC_GetLocalDoBit(HANDLETYPE cardHandle, INT16 doNo, INT16 *pValue);
        //_DLL_API UINT32 IMC_SetLocalDo(HANDLETYPE cardHandle, INT16 value);
        //_DLL_API UINT32 IMC_GetLocalDo(HANDLETYPE cardHandle, INT16 *pValue);
        //_DLL_API UINT32 IMC_GetLocalDiBit(HANDLETYPE cardHandle, INT16 diNo, INT16 *pValue);
        //_DLL_API UINT32 IMC_GetLocalDi(HANDLETYPE cardHandle, INT16 *pValue);
        //_DLL_API UINT32 IMC_GetLocalADVal(HANDLETYPE cardHandle, INT16 *pValue);
        //_DLL_API UINT32 IMC_GetLocalSpecialDi(HANDLETYPE cardHandle, INT16 type, INT16 *pValue);
        //_DLL_API UINT32 IMC_GetLocalSpecialDiBit(HANDLETYPE cardHandle, INT16 index, INT16 type, INT16 *pValue);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalDoBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalDoBit(HANDLETYPE cardHandle, Int16 doNo, Int16 value);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalDoBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalDoBit(HANDLETYPE cardHandle, Int16 doNo, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalDo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalDo(HANDLETYPE cardHandle, Int16 value);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalDo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalDo(HANDLETYPE cardHandle, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalDiBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalDiBit(HANDLETYPE cardHandle, Int16 diNo, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalDi", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalDi(HANDLETYPE cardHandle, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalADVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalADVal(HANDLETYPE cardHandle, ref Int16 pValue);

        //public const uint SPECIAL_IO_RDY = (0);   // 准备完成信号
        //public const uint SPECIAL_IO_ARRIV = (1);   // 到位信号
        //public const uint SPECIAL_IO_ALARM = (2);   // 报警信号
        //public const uint SPECIAL_IO_POSLMT = (3);   // 正限位信号
        //public const uint SPECIAL_IO_NEGLMT = (4);   // 负限位信号
        //public const uint SPECIAL_IO_HOME = (7);   // 回零输入信号
        //public const uint SPECIAL_IO_INDEX = (8);   // 电机Z相信号
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalSpecialDi", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalSpecialDi(HANDLETYPE cardHandle, Int16 type, ref Int16 pValue);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalSpecialDiBit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalSpecialDiBit(HANDLETYPE cardHandle, Int16 index, Int16 type, ref Int16 pValue);



        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----2.2.2 编码器操作接口                                               ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_GetLocalCntPos(HANDLETYPE cardHandle, INT16 index, INT32 *pCntPosArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetLocalCntVel (HANDLETYPE cardHandle, INT16 index,INT32 *pCntVelArray,INT16 count = 1);
        //_DLL_API UINT32 IMC_GetLocalEncPos(HANDLETYPE cardHandle,INT16 index,INT32 *pEncPosArray,INT16 count = 1);
        //_DLL_API UINT32 IMC_GetLocalEncVel (HANDLETYPE cardHandle, INT16 index,INT32 *pEncVelArray,INT16 count = 1);
        ////_DLL_API UINT32 IMC_SetLocalCntPos (HANDLETYPE cardHandle, INT16 index,INT32 cntPos);
        //_DLL_API UINT32 IMC_SetLocalEncPos (HANDLETYPE cardHandle, INT16 index,INT32 encPos);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalCntPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalCntPos(HANDLETYPE cardHandle, Int16 index, Int32[] pCntPosArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalCntVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalCntVel(HANDLETYPE cardHandle, Int16 index, Int32[] pCntVelArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalEncPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalEncPos(HANDLETYPE cardHandle, Int16 index, Int32[] pEncPosArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetLocalEncVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetLocalEncVel(HANDLETYPE cardHandle, Int16 index, Int32[] pEncVelArray, Int16 count = 1);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalCntPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_SetLocalCntPos(HANDLETYPE cardHandle, Int16 index, Int32 cntPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetLocalEncPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetLocalEncPos(HANDLETYPE cardHandle, Int16 index, Int32 encPos);

        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----2.2.3 位置锁存操作接口                                             ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ////_DLL_API UINT32 IMC_SetCaptPosMode(HANDLETYPE cardHandle, INT16 index, INT16 trigType, INT16 captSrc, INT16 sns);
        ////_DLL_API UINT32 IMC_GetCaptPosMode(HANDLETYPE cardHandle, INT16 index, INT16 *pTrigType, INT16 *pCaptSrc, INT16 *pSns);
        ////_DLL_API UINT32 IMC_GetCaptStatus(HANDLETYPE cardHandle, INT16 index, INT16 *pSts, INT32 *pCaptPos);
        // 捕获模式
        //public const uint CAPT_MODE_INDEX = (0);	// Index捕获
        //public const uint CAPT_MODE_HOME = (1);	// Home捕获
        //public const uint CAPT_MODE_PROBE = (3);	// 探针捕获
        //public const uint CAPT_MODE_LIMITP = (4);	// 正限位捕获
        //public const uint CAPT_MODE_LIMITN = (5);	// 负限位捕获

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetCaptPosMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_SetCaptPosMode(HANDLETYPE cardHandle, Int16 index, Int16 trigType, Int16 captSrc, Int16 sns);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCaptPosMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetCaptPosMode(HANDLETYPE cardHandle, Int16 index, ref Int16 pTrigType, ref Int16 pCaptSrc, ref Int16 pSns);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCaptStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetCaptStatus(HANDLETYPE cardHandle, Int16 index, ref Int16 pSts, ref Int32 pCaptPos);

        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----2.2.4 位置比较输出操作接口                                            ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_CompareSrcConfig(HANDLETYPE cardHandle, INT16 portNo, INT16 dimension, INT16 *pCompSrcArray, INT16 *pCmpTypeArray);
        //_DLL_API UINT32 IMC_CompareOutputConfig(HANDLETYPE cardHandle, INT16 portNo,  INT16 ctrlMode,INT16 stLevel, INT16 outputType, INT16 pulseWidth);
        //_DLL_API UINT32 IMC_GetCompareStatus(HANDLETYPE cardHandle, INT16 portNo, INT16 *pCmpSts, INT32 *pCmpCount);
        //_DLL_API UINT32 IMC_StopCompareOut(HANDLETYPE cardHandle, INT16 portNo);
        //_DLL_API UINT32 IMC_StartCompareOut(HANDLETYPE cardHandle, INT16 portNo);
        //_DLL_API UINT32 IMC_CompareManualOutput(HANDLETYPE cardHandle, INT16 portNo, INT16 outVal);
        //_DLL_API UINT32 IMC_LinearCompare(HANDLETYPE cardHandle, INT16 portNo,INT32 intervalLen, INT32 compTimes);
        //_DLL_API UINT32 IMC_SetCompareData(HANDLETYPE cardHandle, INT16 portNo, INT16 compCount, INT32 *pPosBufArray);
        //_DLL_API UINT32 IMC_SetCompareType(UINT64 cardHandle, INT16 portNo, INT16 type);
        //_DLL_API UINT32 IMC_GetCompareType(UINT64 cardHandle, INT16 portNo, INT16 *pType);
        //_DLL_API UINT32 IMC_SetMultiDimensComparePara(UINT64 cardHandle,INT16 portNo,INT16 error,INT16 outpinType);
        //_DLL_API UINT32 IMC_GetMultiDimensComparePara(UINT64 cardHandle,INT16 portNo,INT16 *pError,INT16 *pOutPinTyp);
        //_DLL_API UINT32 IMC_SetMultiDimensCompareData(UINT64 cardHandle,INT16 portNo,TMultiCmpData *pComparaDataArray,INT16 count);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CompareSrcConfig", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CompareSrcConfig(HANDLETYPE cardHandle, Int16 portNo, Int16 dimension, Int16[] pCompSrcArray, Int16[] pCmpTypeArray);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CompareOutputConfig", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CompareOutputConfig(HANDLETYPE cardHandle, Int16 portNo, Int16 ctrlMode, Int16 stLevel, Int16 outputType, Int16 pulseWidth);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCompareStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCompareStatus(HANDLETYPE cardHandle, Int16 portNo, ref Int16 pCmpSts, ref Int32 pCmpCount);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StopCompareOut", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StopCompareOut(HANDLETYPE cardHandle, Int16 portNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartCompareOut", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartCompareOut(HANDLETYPE cardHandle, Int16 portNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CompareManualOutput", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CompareManualOutput(HANDLETYPE cardHandle, Int16 portNo, Int16 outVal);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_LinearCompare", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_LinearCompare(HANDLETYPE cardHandle, Int16 portNo, Int32 intervalLen, Int32 compTimes);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetCompareData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetCompareData(HANDLETYPE cardHandle, Int16 portNo, Int16 compCount, Int32[] pPosBufArray);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetCompareType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_SetCompareType(HANDLETYPE cardHandle, Int16 portNo, Int16 type);
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCompareType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetCompareType(HANDLETYPE cardHandle, Int16 portNo, ref Int16 pType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetCompareDataType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetCompareDataType(HANDLETYPE cardHandle, Int16 portNo, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCompareDataType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCompareDataType(HANDLETYPE cardHandle, Int16 portNo, ref Int16 pType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetComparePosType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetComparePosType(HANDLETYPE cardHandle, Int16 portNo, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetComparePosType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetComparePosType(HANDLETYPE cardHandle, Int16 portNo, ref Int16 pType);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetMultiDimensComparePara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetMultiDimensComparePara(HANDLETYPE cardHandle, Int16 portNo, Int16 error, Int16 outpinType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetMultiDimensComparePara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetMultiDimensComparePara(HANDLETYPE cardHandle, Int16 portNo, ref Int16 pError, ref Int16 pOutPinTyp);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetMultiDimensCompareData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetMultiDimensCompareData(HANDLETYPE cardHandle, Int16 portNo, TMultiCmpData[] pComparaDataArray, Int16 count);
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----2.2.5 PWM输出控制                                                  ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_SetPwmPara (HANDLETYPE cardHandle, INT16 chn,INT16 pwmMode, INT32 frequency,double dutyRatio);
        //_DLL_API UINT32 IMC_GetPwmPara (HANDLETYPE cardHandle, INT16 chn,INT16 *pPwmMode, INT32 *pFrequency,double *pDutyRatio);
        //_DLL_API UINT32 IMC_SetPwmFrq (HANDLETYPE cardHandle, INT16 chn, INT32 frequency);
        //_DLL_API UINT32 IMC_SetPwmDuty (HANDLETYPE cardHandle, INT16 chn, double dutyRatio);
        //_DLL_API UINT32 IMC_SetPwmOnOff (HANDLETYPE cardHandle, INT16 chn, INT16 onOff);
        //_DLL_API UINT32 IMC_SetPwmOnDelay (HANDLETYPE cardHandle, INT16 chn, UINT16 onDelay);
        //_DLL_API UINT32 IMC_SetPwmOffDelay (HANDLETYPE cardHandle, INT16 chn, UINT16 offDelay);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetPwmPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetPwmPara(HANDLETYPE cardHandle, Int16 chn, Int16 pwmMode, Int32 frequency, double dutyRatio);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPwmPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPwmPara(HANDLETYPE cardHandle, Int16 chn, ref Int16 pPwmMode, ref Int32 pFrequency, ref double pDutyRatio);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetPwmFrq", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetPwmFrq(HANDLETYPE cardHandle, Int16 chn, Int32 frequency);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetPwmDuty", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetPwmDuty(HANDLETYPE cardHandle, Int16 chn, double dutyRatio);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetPwmOnOff", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetPwmOnOff(HANDLETYPE cardHandle, Int16 chn, Int16 onOff);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetPwmOnDelay", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetPwmOnDelay(HANDLETYPE cardHandle, Int16 chn, UInt16 onDelay);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetPwmOffDelay", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetPwmOffDelay(HANDLETYPE cardHandle, Int16 chn, UInt16 offDelay);

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		/*----2.2.6 PSO控制                                            ---*/
		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_SetPSOMode0Para(UINT64 cardHandle, INT16 portNo, INT16 dimension,INT16 posType,INT16 pinType,INT16 *pPsoPosIndexArray,double outPlsWidth,INT32 syncDeltaPos);
        //_DLL_API UINT32 IMC_GetPSOMode0Para(UINT64 cardHandle, INT16 portNo, INT16 *pDimension,INT16 *pPosType,INT16 *pPinType,INT16 *pPsoPosIndexArray,double *pOutPlsWidth,INT32 *pSyncDeltaPos);
        //_DLL_API UINT32 IMC_StartPSO(UINT64 cardHandle, INT16 portNo);
        //_DLL_API UINT32 IMC_StopPSO(UINT64 cardHandle, INT16 portNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetPSOMode0Para", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetPSOMode0Para(HANDLETYPE cardHandle, Int16 portNo, Int16 dimension, Int16 posType, Int16 pinType, Int16[] pPsoPosIndexArray, double outPlsWidth, Int32 syncDeltaPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPSOMode0Para", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPSOMode0Para(HANDLETYPE cardHandle, Int16 portNo, ref Int16 pDimension, ref Int16 pPosType, ref Int16 pPinType, ref Int16 pPsoPosIndexArray, ref double pOutPlsWidth, ref Int32 pSyncDeltaPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartPSO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartPSO(HANDLETYPE cardHandle, Int16 portNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StopPSO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StopPSO(HANDLETYPE cardHandle, Int16 portNo);

        ///*==========================================================================*/
        ///*----3 回零操作接口                                                     ---*/
        ///*==========================================================================*/
        ////_DLL_API UINT32 IMC_SetHomingPara(HANDLETYPE cardHandle, INT16 axNo, THomingPara *pHomingPara);
        ////_DLL_API UINT32 IMC_StartHoming(HANDLETYPE cardHandle, INT16 axNo);
        //_DLL_API UINT32 IMC_StartHoming(HANDLETYPE cardHandle, INT16 axNo, THomingPara *pHomingPara);
        //_DLL_API UINT32 IMC_StopHoming(HANDLETYPE cardHandle, INT16 axNo, INT16 stopType);
        //_DLL_API UINT32 IMC_GetHomingStatus(HANDLETYPE cardHandle, INT16 axNo, INT16 *pStatus);
        //_DLL_API UINT32 IMC_FinishHoming(HANDLETYPE cardHandle, INT16 axNo);

        //_DLL_API UINT32 IMC_StartEcatAxCSPHoming(UINT64 cardHandle, INT16 axNo,INT16 method,INT16 dir,INT16 level,double hVel,double lVel,double acc,double offset);
        //_DLL_API UINT32 IMC_StopEcatAxCSPHoming(UINT64 cardHandle, INT16 axNo,INT16 stopMode);
        //_DLL_API UINT32 IMC_GetEcatAxCSPHomingSts(UINT64 cardHandle, INT16 axNo,INT16 *pHomingMethod,INT16 *pSts);
        //_DLL_API UINT32 IMC_FinishEcatAxCSPHoming(UINT64 cardHandle, INT16 axNo);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetHomingPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_SetHomingPara(HANDLETYPE cardHandle, Int16 axNo, ref THomingPara pHomingPara);

        //_DLL_API UINT32 IMC_CloseAutoHomingFinish(UINT64 cardHandle);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartHomingInUnit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartHomingInUnit(HANDLETYPE cardHandle, Int16 axNo, ref THomingParaInUint pHomingPara);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartHoming(HANDLETYPE cardHandle, Int16 axNo, ref THomingPara pHomingPara);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StopHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StopHoming(HANDLETYPE cardHandle, Int16 axNo, Int16 stopType);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetHomingStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetHomingStatus(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pStatus);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FinishHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_FinishHoming(HANDLETYPE cardHandle, Int16 axNo);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartEcatAxCSPHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartEcatAxCSPHoming(HANDLETYPE cardHandle, Int16 axNo, Int16 method, Int16 dir, Int16 level, double hVel, double lVel, double acc, double offset);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StopEcatAxCSPHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StopEcatAxCSPHoming(HANDLETYPE cardHandle, Int16 axNo, Int16 stopMode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAxCSPHomingSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAxCSPHomingSts(HANDLETYPE cardHandle, Int16 axNo, ref  Int16 pHomingMethod, ref Int16 pSts);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FinishEcatAxCSPHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_FinishEcatAxCSPHoming(HANDLETYPE cardHandle, Int16 axNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CloseAutoHomingFinish", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CloseAutoHomingFinish(HANDLETYPE cardHandle);

        ///*==========================================================================*/
        ///*----4 状态管理接口                                                     ---*/
        ///*==========================================================================*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----4.2 轴状态管理接口                                                 ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_GetAxPrfMode(HANDLETYPE cardHandle, INT16 axNo, INT16 *pPrfModeArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetAxSts(HANDLETYPE cardHandle, INT16 axNo, INT32 *pAxStsArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_ClrAxSts(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetAxPrfPos(HANDLETYPE cardHandle, INT16 axNo, double *pPrfPosArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetAxPrfVel(HANDLETYPE cardHandle, INT16 axNo, double *pPrfVelArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetAxPrfAcc(HANDLETYPE cardHandle, INT16 axNo, double *pPrfAccArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetAxEncPos(HANDLETYPE cardHandle, INT16 axNo, double *pEncPosArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetAxEncVel(HANDLETYPE cardHandle, INT16 axNo, double *pEncVelArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetAxEncAcc(HANDLETYPE cardHandle, INT16 axNo, double *pEncAccArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetPrfPos(HANDLETYPE cardHandle, INT16 axNo, double *pPrfPosArray, INT16 count = 1);
        //_DLL_API UINT32 IMC_GetAxType(HANDLETYPE cardHandle, INT16 axNo, INT16 *pAxType, INT16 *pOutChn);
        //_DLL_API UINT32 IMC_GetMultiAxArrivalSts(UINT64 cardHandle, INT32 axMask,INT16 *pSts);
        //_DLL_API UINT32 IMC_AxServoOn(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxServoOff(HANDLETYPE cardHandle, INT16 axNo, INT16 count = 1);
        //_DLL_API UINT32 IMC_AxMoveStop(HANDLETYPE cardHandle, INT16 axNo, INT16 stopType);
        //_DLL_API UINT32 IMC_AxStopInBits(HANDLETYPE cardHandle, INT32 axMask, INT16 stopTypeBits);
        //_DLL_API UINT32 UINT32 IMC_SetAxCurPos(HANDLETYPE cardHandle, INT16 axNo, double setPos);
        //_DLL_API UINT32 IMC_SetAxCtrlMode(UINT64 cardHandle,INT16 axNo,INT16 ctrlMode);
        //_DLL_API UINT32 IMC_GetAxCtrlMode(UINT64 cardHandle,INT16 axNo,INT16 *pCtrlMode);
        //_DLL_API UINT32 IMC_PrfAxTorq(UINT64 cardHandle,INT16 axNo,INT16 tgtTrq,INT16 time);
        //_DLL_API UINT32 IMC_GetAxActTorq(UINT64 cardHandle,INT16 axNo,INT16 *pActTrq);

        //_DLL_API UINT32 IMC_SetAxTorqSlope(UINT64 cardHandle,INT16 axNo,INT32 trqSlope);
        //_DLL_API UINT32 IMC_GetAxTorqSlope(UINT64 cardHandle,INT16 axNo,INT32 *pTrqSlope);
        //_DLL_API UINT32 IMC_SetAxTgtTorq(UINT64 cardHandle,INT16 axNo,INT16 tgtTrq);
        //_DLL_API UINT32 IMC_GetAxTgtTorq(UINT64 cardHandle,INT16 axNo,INT16 *pTgtTrq);
        //_DLL_API UINT32 IMC_GetAxErrCode(UINT64 cardHandle,INT16 axNo,INT16 *pErrorCode);
        //_DLL_API UINT32 IMC_GetAxEcatDigitalInput(UINT64 cardHandle, INT16 axNo, INT16 *pDigitalInput);
        //_DLL_API UINT32 IMC_GetAxBacklashCmpVal(UINT64 cardHandle, INT16 axNo, INT32 *pCmpVal);
        //_DLL_API UINT32 IMC_SyncAxPos(UINT64 cardHandle, INT16 axNo);

        // 非标510
        //_DLL_API UINT32 IMC_SetAxPressFitCtrlWord(UINT64 cardHandle,INT16 axNo,UINT16 ctrlword);
        //_DLL_API UINT32 IMC_SetAxPressTgtVal(UINT64 cardHandle,INT16 axNo,INT16 pressVal);
        //_DLL_API UINT32 IMC_GetAxPressFitSts(UINT64 cardHandle, INT16 axNo, UINT16 *pSts);
        //_DLL_API UINT32 IMC_GetAxAIInputVal(UINT64 cardHandle, INT16 axNo, INT16 *pA1Volt,INT16 *pA2Volt);
        //_DLL_API UINT32 IMC_GetAxPressFeedback(UINT64 cardHandle, INT16 axNo, INT16 *pPressVal);

        // 转矩回零
        //_DLL_API UINT32 IMC_StartAxTorqHoming(UINT64 cardHandle,INT16 axNo,INT16 tgtTorq,INT16 torqSlopeTime,UINT32 maxVel);
        //_DLL_API UINT32 IMC_StopAxTorqHoming(UINT64 cardHandle,INT16 axNo,INT16 stopTime);
        //_DLL_API UINT32 IMC_GetAxTorqHomingSts(UINT64 cardHandle,INT16 axNo,INT16 *pStatus);
        //_DLL_API UINT32 IMC_FinishAxTorqHoming(UINT64 cardHandle,INT16 axNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxPrfMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxPrfMode(HANDLETYPE cardHandle, Int16 axNo, Int16[] pPrfModeArray, Int16 count = 1);

        // 轴状态位
        public const uint AX_ALARM_BIT = (0x00000001);		// 轴驱动报警
        public const uint AX_SVON_BIT = (0x00000002);		// 伺服使能
        public const uint AX_BUSY_BIT = (0x00000004);		// 轴忙状态
        public const uint AX_ARRIVE_BIT = (0x00000008);	// 轴到位状态
        public const uint AX_POSLMT_BIT = (0x00000010);		// 正硬限位报警
        public const uint AX_NEGLMT_BIT = (0x00000020);	// 负硬限位报警
        public const uint AX_SOFT_POSLMT_BIT = (0x00000040);		// 正软限位报警
        public const uint AX_SOFT_NEGLMT_BIT = (0x00000080);		// 负软限位报警
        public const uint AX_ERRPOS_BIT = (0x00000100);	// 轴位置误差越限标志
        public const uint AX_EMG_STOP_BIT = (0x00000200);		// 运动急停标志
        public const uint AX_ECAT_BIT = (0x00000400);		// 总线轴标志

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxSts(HANDLETYPE cardHandle, Int16 axNo, Int32[] pAxStsArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ClrAxSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ClrAxSts(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxPrfPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxPrfPos(HANDLETYPE cardHandle, Int16 axNo, double[] pPrfPosArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxPrfVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxPrfVel(HANDLETYPE cardHandle, Int16 axNo, double[] pPrfVelArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxPrfAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxPrfAcc(HANDLETYPE cardHandle, Int16 axNo, double[] pPrfAccArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxEncPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxEncPos(HANDLETYPE cardHandle, Int16 axNo, double[] pEncPosArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxOrgEncPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxOrgEncPos(HANDLETYPE cardHandle, Int16 axNo, Int32[] pOrgEncPosArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxEncVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxEncVel(HANDLETYPE cardHandle, Int16 axNo, double[] pEncVelArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxEncAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxEncAcc(HANDLETYPE cardHandle, Int16 axNo, double[] pEncAccArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPrfPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPrfPos(HANDLETYPE cardHandle, Int16 axNo, double[] pPrfPosArray, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxType(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pAxType, ref Int16 pOutChn);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetMultiAxArrivalSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetMultiAxArrivalSts(HANDLETYPE cardHandle, Int32 axMask, ref Int16 pSts);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxServoOn", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxServoOn(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxServoOff", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxServoOff(HANDLETYPE cardHandle, Int16 axNo, Int16 count = 1);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxMoveStop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxMoveStop(HANDLETYPE cardHandle, Int16 axNo, Int16 stopType);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AxStopInBits", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AxStopInBits(HANDLETYPE cardHandle, Int32 axMask, Int32 stopTypeBits);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxCurPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxCurPos(HANDLETYPE cardHandle, Int16 axNo, double setPos);




        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxCompenPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxCompenPos(HANDLETYPE cardHandle, Int16 axNo, double cmpPos, double cmpTime, Int16 posType);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxCompenPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxCompenPos(HANDLETYPE cardHandle, Int16 axNo, ref double pCmpPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxCtrlMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxCtrlMode(HANDLETYPE cardHandle, Int16 axNo, Int16 ctrlMode);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxCtrlMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxCtrlMode(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pCtrlMode);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PrfAxTorq", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PrfAxTorq(HANDLETYPE cardHandle, Int16 axNo, Int16 tgtTrq, Int16 time);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxActTorq", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxActTorq(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pActTrq);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxTorqSlope", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxTorqSlope(HANDLETYPE cardHandle, Int16 axNo, Int32 trqSlope);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxTorqSlope", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxTorqSlope(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pTrqSlope);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxTgtTorq", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxTgtTorq(HANDLETYPE cardHandle, Int16 axNo, Int16 tgtTrq);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxTgtTorq", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxTgtTorq(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pTgtTrq);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxErrCode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxErrCode(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pErrorCode);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxEcatDigitalInput", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxEcatDigitalInput(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pDigitalInput);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxBacklashCmpVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxBacklashCmpVal(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pCmpVal);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatAxMaxVelLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatAxMaxVelLmt(HANDLETYPE cardHandle, Int16 axNo, Int32 maxVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAxMaxVelLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAxMaxVelLmt(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pMaxVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatAxPosTorqLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatAxPosTorqLmt(HANDLETYPE cardHandle, Int16 axNo, Int16 posTorqLmt);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAxPosTorqLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAxPosTorqLmt(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pPosTorqLmt);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatAxNegTorqLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatAxNegTorqLmt(HANDLETYPE cardHandle, Int16 axNo, Int16 negTorqLmt);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAxNegTorqLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAxNegTorqLmt(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pNegTorqLmt);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEcatAxMaxTorqLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEcatAxMaxTorqLmt(HANDLETYPE cardHandle, Int16 axNo, Int16 maxTorqLmt);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatAxMaxTorqLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEcatAxMaxTorqLmt(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pMaxTorqLmt);














        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SyncAxPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SyncAxPos(HANDLETYPE cardHandle, Int16 axNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxPressFitCtrlWord", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxPressFitCtrlWord(HANDLETYPE cardHandle, Int16 axNo, UInt16 ctrlword);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxPressTgtVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxPressTgtVal(HANDLETYPE cardHandle, Int16 axNo, Int16 pressVal);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxPressFitSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxPressFitSts(HANDLETYPE cardHandle, Int16 axNo, ref UInt16 pSts);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxAIInputVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxAIInputVal(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pA1Volt, ref Int16 pA2Volt);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxPressFeedback", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxPressFeedback(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pPressVal);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartAxTorqHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartAxTorqHoming(HANDLETYPE cardHandle, Int16 axNo, Int16 tgtTorq, Int16 torqSlopeTime, UInt32 maxVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StopAxTorqHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StopAxTorqHoming(HANDLETYPE cardHandle, Int16 axNo, Int16 stopTime);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxTorqHomingSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxTorqHomingSts(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pStatus);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FinishAxTorqHoming", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_FinishAxTorqHoming(HANDLETYPE cardHandle, Int16 axNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartAxCsvPrf", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartAxCsvPrf(HANDLETYPE cardHandle, Int16 axNo, Int32 tgtVel, Int32 acc, Int16 prfType);//prfType s 1,t 0
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StopAxCsvPrf", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StopAxCsvPrf(HANDLETYPE cardHandle, Int16 axNo, Int32 dec);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EstopAxCsvPrf", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EstopAxCsvPrf(HANDLETYPE cardHandle, Int16 axNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_UpdateAxCsvPrf", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_UpdateAxCsvPrf(HANDLETYPE cardHandle, Int16 axNo, Int32 tgtVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxCsvPrfStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxCsvPrfStatus(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pStatus);// 0 未规划，1 规划中，2 停止规划中 

        ///*==========================================================================*/
        ///*----5 数据采集接口                                                     ---*/
        ///*==========================================================================*/
        ////_DLL_API UINT32 IMC_ConfigSamplePara(HANDLETYPE cardHandle, TSamplePara *pSamplePara);
        ////_DLL_API UINT32 IMC_ConfigSampleData(HANDLETYPE cardHandle, INT16 count, INT16 *pDataTypeArray, INT16 *pDataIndexArray);
        ////_DLL_API UINT32 IMC_GetSampleDataSts(HANDLETYPE cardHandle, INT16 *pPackLen, INT16 *pDataLen);
        ////_DLL_API UINT32 IMC_ConfigSampleEnable(HANDLETYPE cardHandle, INT16 enable);
        ////_DLL_API UINT32 IMC_GetSampleStatus(HANDLETYPE cardHandle, INT16 *pStatus, INT32 *pLen, INT32 *pLeakageCount);
        ////_DLL_API UINT32 IMC_GetSampleData(HANDLETYPE cardHandle, INT16 *pPackNum, INT16 *pDataArray);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ConfigSamplePara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ConfigSamplePara(HANDLETYPE cardHandle, ref TSamplePara pSamplePara);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ConfigSampleData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ConfigSampleData(HANDLETYPE cardHandle, Int16 count, Int16[] pDataTypeArray, Int16[] pDataIndexArray);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ConfigSampleEnable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ConfigSampleEnable(HANDLETYPE cardHandle, Int16 enable);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetSampleStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetSampleStatus(HANDLETYPE cardHandle, ref Int16 pStatus, ref Int32 pLen, ref Int32 pLeakageCount);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetSampleData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetSampleData(HANDLETYPE cardHandle, ref Int16 pPackNum, Int16[] pDataArray);

        /*==========================================================================*/
        /*----6 轴位置捕获接口                                                     ---*/
        /*==========================================================================*/
        //_DLL_API UINT32 IMC_SetAxCaptMode (HANDLETYPE cardHandle,INT16 axNo,INT16 trigType,INT16 captSrc,INT16 sns);
        //_DLL_API UINT32 IMC_GetAxCaptMode (HANDLETYPE cardHandle,INT16 axNo,INT16 *pTrigType,INT16 *pCaptSrc,INT16 *pSns);
        //_DLL_API UINT32 IMC_GetAxCaptStatus (HANDLETYPE cardHandle,INT16 axNo,INT16 *pSts,INT32 *pCaptPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetAxCaptMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetAxCaptMode(HANDLETYPE cardHandle, Int16 axNo, Int16 trigType, Int16 captSrc, Int16 sns);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxCaptMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxCaptMode(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pTrigType, ref Int16 pCaptSrc, ref Int16 pSns);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetAxCaptStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetAxCaptStatus(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pSts, ref Int32 pCaptPos);

        ///*==========================================================================*/
        ///*----7 运动模式接口                                                     ---*/
        ///*==========================================================================*/
        //_DLL_API UINT32 IMC_SetSingleAxMvPara(HANDLETYPE cardHandle, INT16 axNo, double vel, double acc, double dec);
        //_DLL_API UINT32 IMC_GetSingleAxMvPara(HANDLETYPE cardHandle, INT16 axNo, double *pVel, double *pAcc, double *pDec);
        //_DLL_API UINT32 IMC_SetSingleAxVelType(HANDLETYPE cardHandle, INT16 axNo, INT16 velType, double ratio);
        //_DLL_API UINT32 IMC_GetSingleAxVelType(HANDLETYPE cardHandle, INT16 axNo, INT16 *pVelType, double *pRatio);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetSingleAxMvPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetSingleAxMvPara(HANDLETYPE cardHandle, Int16 axNo, double vel, double acc, double dec);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetSingleAxMvPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetSingleAxMvPara(HANDLETYPE cardHandle, Int16 axNo, ref double pVel, ref double pAcc, ref double pDec);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetSingleAxVelType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetSingleAxVelType(HANDLETYPE cardHandle, Int16 axNo, Int16 velType, double ratio);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetSingleAxVelType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetSingleAxVelType(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pVelType, ref double pRatio);
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----7.1 PTP点位模式接口                                                ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_StartPtpMove(HANDLETYPE cardHandle, INT16 axNo, double tgtPos, INT16 posType = 0);
        //_DLL_API  UINT32 IMC_StartMultiPtpMove(UINT64 cardHandle, INT16 axNum, INT16 *pAxNoArray, double *pTgtPosArray, INT16 *pPosTypeArray);
        //_DLL_API UINT32 IMC_UpdatePtpTgtPos(HANDLETYPE cardHandle, INT16 axNo, double tgtPos);
        //_DLL_API UINT32 IMC_UpdatePtpMvPara(HANDLETYPE cardHandle, INT16 axNo, double tgtVel, double acc, double dec);
        //_DLL_API UINT32 IMC_UpdatePtpTgtVel(HANDLETYPE cardHandle, INT16 axNo, double tgtVel);
        //_DLL_API UINT32 IMC_PauseMove(UINT64 cardHandle, INT16 axNo);
        //_DLL_API UINT32 IMC_ResumeMove(UINT64 cardHandle, INT16 axNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartPtpMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartPtpMove(HANDLETYPE cardHandle, Int16 axNo, double tgtPos, Int16 posType = 0);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartMultiPtpMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartMultiPtpMove(HANDLETYPE cardHandle, Int16 axNum, Int16[] pAxNoArray, double[] pTgtPosArray, Int16[] pPosTypeArray);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_UpdatePtpTgtPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_UpdatePtpTgtPos(HANDLETYPE cardHandle, Int16 axNo, double tgtPos);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_UpdatePtpMvPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_UpdatePtpMvPara(HANDLETYPE cardHandle, Int16 axNo, double tgtVel, double acc, double dec);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_UpdatePtpTgtVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_UpdatePtpTgtVel(HANDLETYPE cardHandle, Int16 axNo, double tgtVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PauseMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PauseMove(HANDLETYPE cardHandle, Int16 axNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ResumeMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ResumeMove(HANDLETYPE cardHandle, Int16 axNo);

        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----7.2 Jog运动模式接口                                                ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_StartJogMove(HANDLETYPE cardHandle, INT16 axNo,  double tgVel);
        //_DLL_API UINT32 IMC_StartMultiJogMove(UINT64 cardHandle, INT16 axNum, INT16 *pAxNo,  double *pTgtVel);
        //_DLL_API UINT32 IMC_UpdateJogTgtVel(HANDLETYPE cardHandle, INT16 axNo, double tgVel);
        //_DLL_API UINT32 IMC_UpdateJogMvPara(HANDLETYPE cardHandle, INT16 axNo, double tgVel, double acc, double dec);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartJogMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartJogMove(HANDLETYPE cardHandle, Int16 axNo, double tgVel);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartMultiJogMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartMultiJogMove(HANDLETYPE cardHandle, Int16 axNum, Int16[] pAxNoArray, double[] pTgtVelArray);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_UpdateJogTgtVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_UpdateJogTgtVel(HANDLETYPE cardHandle, Int16 axNo, double tgVel);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_UpdateJogMvPara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_UpdateJogMvPara(HANDLETYPE cardHandle, Int16 axNo, double tgVel, double acc, double dec);

        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----7.3 手轮跟随运动模式接口                                           ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_EnterHandWheelMode(HANDLETYPE cardHandle, INT16 axNo, INT16 masterType, INT16 masterIndex, double ratio, double acc, double vel);
        //_DLL_API UINT32 IMC_ExitHandWheelMode(HANDLETYPE cardHandle);
        //_DLL_API UINT32 IMC_SwitchHandWheelRatio(HANDLETYPE cardHandle, double ratio);
        //_DLL_API uint32_t IMC_dsp_test(uint32_t cardHandle, float ratio);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EnterHandWheelMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EnterHandWheelMode(HANDLETYPE cardHandle, Int16 axNo, Int16 masterType, Int16 masterIndex, double ratio, double acc, double vel);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ExitHandWheelMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ExitHandWheelMode(HANDLETYPE cardHandle);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SwitchHandWheelRatio", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SwitchHandWheelRatio(HANDLETYPE cardHandle, double ratio);

        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_dsp_test", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_dsp_test(HANDLETYPE cardHandle, float ratio);

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----7.4 电子齿轮                                          ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        // 设置跟随参数
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GearSetParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GearSetParam(HANDLETYPE cardHandle, Int16 axNo, ref  TGearParam pGearParam);

        // 读取跟随参数
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GearGetParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GearGetParam(HANDLETYPE cardHandle, Int16 axNo, ref TGearParam pGearParam);
        // 更新主轴齿数和设置离合区
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GearUpdateScale", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GearUpdateScale(HANDLETYPE cardHandle, Int16 axNo, double masterScale, double slaveScale, Int32 masterDis);
        //电子齿轮模式主轴离合区设定
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GearSetMasterZone", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GearSetMasterZone(HANDLETYPE cardHandle, Int16 axNo, Int32 masterDis);
        //电子齿轮模式启动
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GearStart", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GearStart(HANDLETYPE cardHandle, Int16 axNo);
        //电子齿轮模式停止
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GearStop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GearStop(HANDLETYPE cardHandle, Int16 axNo, Int16 type);
        //电子齿轮错误获取状态
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GearGetStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GearGetStatus(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pStatus);

        //电子齿轮关系销毁
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GearDestroy", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GearDestroy(HANDLETYPE cardHandle, Int16 axNo);



        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        ///*----7.3 PVT模式接口                                           ---*/
        ///*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_AddPT(UINT64 cardHandle, INT16 tableNo, double *pPos,double *pTime,INT16 dataNum,double v0)
        //_DLL_API UINT32 IMC_AddPVTMode0(UINT64 cardHandle, INT16 tableNo, double *pPos,double *pTime,double *pVel,INT16 dataNum,double v0)
        //_DLL_API UINT32 IMC_AddPVTMode1(UINT64 cardHandle,INT16 tableNo, double *pPos,double *pTime, double vN,INT16 dataNum,double v0)
        //_DLL_API UINT32 IMC_AddPVTMode2(UINT64 cardHandle, INT16 tableNo, double *pPos,double *pTime,double *pK,INT16 dataNum, double v0)
        //_DLL_API UINT32 IMC_AddDynamicPT(UINT64 cardHandle,INT16 tableNo,double pPos,double pTime)
        //_DLL_API UINT32 IMC_StartStaticPVT (UINT64 cardHandle, INT16 axNo, INT16 *pList, INT16 listCnt, INT16 loopCnt)
        //_DLL_API UINT32 IMC_GetPVTStatus(UINT64 cardHandle, INT16 axNo, INT16 *pLoopCnt,INT16 *pTabCnt,INT16 *pCurveCnt)
        //_DLL_API UINT32 IMC_StartDynamicPVT(UINT64 cardHandle, INT16 axNo,INT16 tableNo)
        //_DLL_API UINT32 IMC_GetPVTTableSpace(UINT64 cardHandle,INT16 tableNo,INT16 *pSpace)
        //_DLL_API UINT32 IMC_GetPVTTableType(UINT64 cardHandle,INT16 tableNo,INT16 *pType)
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AddPT", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AddPT(HANDLETYPE cardHandle, Int16 tableNo, double[] pPos, double[] pTime, Int16 dataNum, double v0);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AddPVTMode0", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AddPVTMode0(HANDLETYPE cardHandle, Int16 tableNo, double[] pPos, double[] pTime, double[] pVel, Int16 dataNum, double v0);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AddPVTMode1", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AddPVTMode1(HANDLETYPE cardHandle, Int16 tableNo, double[] pPos, double[] pTime, double vN, Int16 dataNum, double v0);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AddPVTMode2", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AddPVTMode2(HANDLETYPE cardHandle, Int16 tableNo, double[] pPos, double[] pTime, double[] pK, Int16 dataNum, double v0);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AddDynamicPT", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AddDynamicPT(HANDLETYPE cardHandle, Int16 tableNo, double pPos, double pTime);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartStaticPVT", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartStaticPVT(HANDLETYPE cardHandle, Int16 axNo, Int16[] pList, Int16 listCnt, Int16 loopCnt);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPVTStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPVTStatus(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pLoopCnt, ref Int16 pTabCnt, ref Int16 pCurveCnt);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartDynamicPVT", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartDynamicPVT(HANDLETYPE cardHandle, Int16 axNo, Int16 tableNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPVTTableSpace", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPVTTableSpace(HANDLETYPE cardHandle, Int16 tableNo, ref Int16 pSpace);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPVTTableType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPVTTableType(HANDLETYPE cardHandle, Int16 tableNo, ref Int16 pType);
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----7.4 跟随运动                                          ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        // 设置跟随参数
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowSetParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowSetParam(HANDLETYPE cardHandle, Int16 axNo, ref TFolParam pFolParam);

        //// 读取跟随参数
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowGetParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowGetParam(HANDLETYPE cardHandle, Int16 axNo, ref TFolParam pFolParam);
        //// 更新主轴齿数
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowUpdateScale", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowUpdateScale(HANDLETYPE cardHandle, Int16 axNo, double masterScale, double slaveScale);
        ////跟随模式设定 （速度跟随还是位置跟随）
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowSetType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowSetType(HANDLETYPE cardHandle, Int16 axNo, Int16 type);
        ////跟随模式读取 （速度跟随还是位置跟随）
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowGetType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowGetType(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pType);
        ////立即跟随启动
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowSetStart", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowSetStart(HANDLETYPE cardHandle, Int16 axNo);
        ////位置限制使能
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowSetPosLmtEn", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowSetPosLmtEn(HANDLETYPE cardHandle, Int16 axNo, Int16 en);
        ////位置限制参数设置
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowSetPosLmt", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowSetPosLmt(HANDLETYPE cardHandle, Int16 axNo, double posLimitPos, double negLimitPos);
        ////位置限制参数读取
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowGetLmtInfo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowGetLmtInfo(HANDLETYPE cardHandle, Int16 axNo, ref Int16 pEn, ref double pPosLimitPos, ref double pNegLimitPos);
        //// 设置启动条件
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowSetStartCond", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowSetStartCond(HANDLETYPE cardHandle, Int16 axNo, ref TFolStartCond pFolStartCond);
        //// 读取启动条件
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowGetStartCond", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowGetStartCond(HANDLETYPE cardHandle, Int16 axNo, ref TFolStartCond pFolStartCond);
        ////设置PID参数
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowSetPid", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowSetPid(HANDLETYPE cardHandle, Int16 axNo, double kp, double ki);
        ////读取PID参数
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowGetPid", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowGetPid(HANDLETYPE cardHandle, Int16 axNo, ref double pKp, ref double pKi);
        ////读取跟随误差
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_FollowGetPosErr", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_FollowGetPosErr(HANDLETYPE cardHandle, Int16 axNo, ref double pErr);

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*---- 电子凸轮                                     ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EcamAdddata", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_EcamAdddata(HANDLETYPE cardHandle, Int16 tabNo, ref double pMasterPos, ref double pSlavePos, ref Int16 pType, Int16 dataNum);
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EcamStart", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_EcamStart(HANDLETYPE cardHandle, Int16 axNo, Int16 tabNo, Int16 masterNo, Int16 masterSrc, Int16 camType, double scale, ref TFolStartCond pStartCond);

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----7.4 直角坐标系插补运动模式接口                                     ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_CrdSetMtSys(UINT64 cardHandle, INT16 crdNo,INT16 *pMaskAxNoArray,INT16 lookAheadLen,double estopDec);
        //_DLL_API UINT32 IMC_CrdGetMtSysParam(UINT64 cardHandle, INT16 crdNo,INT16 *pMaskAxNoArray,INT16 *pLookAheadLen,double *pEstopDec);
        //_DLL_API UINT32 IMC_CrdDeleteMtSys(UINT64 cardHandle, INT16 crdNo);
        //_DLL_API UINT32 IMC_CrdSetAdvParam(UINT64 cardHandle, INT16 crdNo,TCrdAdvParam *pCrdAdvParam);
        //_DLL_API UINT32 IMC_CrdGetAdvParam(UINT64 cardHandle, INT16 crdNo,TCrdAdvParam *pCrdAdvParam);
        //_DLL_API UINT32 IMC_CrdSetSmoothParam(UINT64 cardHandle, INT16 crdNo, INT32 smoothLevel, double smoothTol);
        //_DLL_API UINT32 IMC_CrdGetSmoothParam(UINT64 cardHandle, INT16 crdNo, INT32 *pSmoothLevel, double *pSmoothTol);

        //_DLL_API UINT32 IMC_CrdSetTrajVel(UINT64 cardHandle, INT16 crdNo,double tgtVel);
        //_DLL_API UINT32 IMC_CrdSetTrajAcc(UINT64 cardHandle, INT16 crdNo,double tgtAcc);
        //_DLL_API UINT32 IMC_CrdSetTrajAccAndDec(UINT64 cardHandle, INT16 crdNo,double tgtAcc,double tgtdec);
        //_DLL_API UINT32 IMC_CrdSetZeroFlag(UINT64 cardHandle, INT16 crdNo,INT16 ZeroFlag);
        //_DLL_API UINT32 IMC_CrdSetIncMode(UINT64 cardHandle, INT16 crdNo,INT16 mode);
        //_DLL_API UINT32 IMC_CrdUserVelPlan(UINT64 cardHandle,INT16 crdNo, double uStartVel, double uEndVel);
        //_DLL_API UINT32 IMC_CrdSetTrajTol(UINT64 cardHandle, INT16 crdNo,double tol);
        //_DLL_API UINT32 IMC_CrdSetTrajTurnCoef(UINT64 cardHandle, INT16 crdNo,double turnCoef);

        //_DLL_API UINT32 IMC_CrdLineXYZ(UINT64 cardHandle, INT16 crdNo,double *pEndPos, INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdLineXY(UINT64 cardHandle, INT16 crdNo,double *pEndPos, INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdLineZX(UINT64 cardHandle, INT16 crdNo,double *pEndPos, INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdLineYZ(UINT64 cardHandle, INT16 crdNo,double *pEndPos, INT32 userID=0);

        //_DLL_API UINT32 IMC_CrdArcThreePoint(UINT64 cardHandle, INT16 crdNo,double *pMidPos,double *pEndPos, INT32 userID);

        ////空间arc插补
        //_DLL_API UINT32 IMC_Crd3DArcCenterNormal(UINT64 cardHandle, INT16 crdNo,double *pCenter,double *pEndPos,double *pNormal,double height=0,INT32 turn=0,INT32 userID=0);
        //_DLL_API UINT32 IMC_Crd3DArcRadiusNormal(UINT64 cardHandle, INT16 crdNo,double radius,double *pEndPos,double *pNormal,double height=0,INT32 turn=0,INT32 userID=0);
        //_DLL_API UINT32 IMC_Crd3DArcAngleNormal(UINT64 cardHandle, INT16 crdNo,double *pCenter,double angle,double *pNormal,double height=0,INT32 userID=0);
        ////平面arc插补
        ////圆心终点编程
        //_DLL_API UINT32 IMC_CrdArcCenterXYPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double *pEndPos,INT16 dir,double height=0,INT32 turn=0,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdArcCenterYZPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double *pEndPos,INT16 dir,double height=0,INT32 turn=0,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdArcCenterZXPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double *pEndPos,INT16 dir,double height=0,INT32 turn=0,INT32 userID=0);
        ////半径终点编程
        //_DLL_API UINT32 IMC_CrdArcRadiusXYPlane(UINT64 cardHandle, INT16 crdNo,double radius,double *pEndPos,INT16 dir,double height=0,INT32 turn=0,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdArcRadiusYZPlane(UINT64 cardHandle, INT16 crdNo,double radius,double *pEndPos,INT16 dir,double height=0,INT32 turn=0,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdArcRadiusZXPlane(UINT64 cardHandle, INT16 crdNo,double radius,double *pEndPos,INT16 dir,double height=0,INT32 turn=0,INT32 userID=0);
        ////圆心角度编程
        //_DLL_API UINT32 IMC_CrdArcAngleXYPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double angle,double height=0,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdArcAngleYZPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double angle,double height=0,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdArcAngleZXPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double angle,double height=0,INT32 userID=0);

        //涡旋线插补
        //_DLL_API UINT32 IMC_CrdSpiralAngleXYPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double angle,double height,double m,INT32 userID);
        //_DLL_API UINT32 IMC_CrdSpiralAngleYZPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double angle,double height,double m,INT32 userID);
        //_DLL_API UINT32 IMC_CrdSpiralAngleZXPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double angle,double height,double m,INT32 userID);

        //椭圆插补
        /*起点作为椭圆的一个顶点,给定圆心，以及另一个顶点到圆心的距离b，给定方向，走整椭圆*/
        //_DLL_API UINT32 IMC_CrdEllipseFullXYPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double b,INT16 dir,INT32 userID);
        //_DLL_API UINT32 IMC_CrdEllipseFullYZPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double b,INT16 dir,INT32 userID);
        //_DLL_API UINT32 IMC_CrdEllipseFullZXPlane(UINT64 cardHandle, INT16 crdNo,double *pCenter,double b,INT16 dir,INT32 userID);

        ////FIFO事件
        //_DLL_API UINT32 IMC_CrdWaitTime(UINT64 cardHandle, INT16 crdNo,INT32 waitPeriod,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdWaitDI(UINT64 cardHandle, INT16 crdNo,INT16 diNO,INT16 diType,INT16 diLevel,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdSetDO(UINT64 cardHandle, INT16 crdNo,INT16 doNO,INT16 doType,INT16 doLevel,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdPTPMove(UINT64 cardHandle, INT16 crdNo,INT16 axNo,double tgtPos,double tgtVel,double acc, INT16 mvType,INT16 waitFlag,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdSyncMove(UINT64 cardHandle, INT16 crdNo,INT16 axNo,double syncPos,INT32 userID=0);
        //_DLL_API UINT32 IMC_CrdEndData(UINT64 cardHandle, INT16 crdNo,INT16 *pIsFinished);


        //_DLL_API UINT32 IMC_CrdStart(UINT64 cardHandle, INT16 crdNo);
        //_DLL_API UINT32 IMC_CrdStop(UINT64 cardHandle, INT16 crdNo,INT16 stopType);
        //_DLL_API UINT32 IMC_CrdGetStatus(UINT64 cardHandle, INT16 crdNo,INT16 *pStatus);
        //_DLL_API UINT32 IMC_CrdSetRatio(UINT64 cardHandle, INT16 crdNo,double ratio);
        //_DLL_API UINT32 IMC_CrdGetRatio(UINT64 cardHandle, INT16 crdNo,double *pRatio);
        //_DLL_API UINT32 IMC_CrdGetPos(UINT64 cardHandle, INT16 crdNo,double *pCrdPos);
        //_DLL_API UINT32 IMC_CrdGetVel(UINT64 cardHandle, INT16 crdNo,double *pCrdVel);
        //_DLL_API UINT32 IMC_CrdGetUserID(UINT64 cardHandle, INT16 crdNo,INT32 *pUserID);
        //_DLL_API UINT32 IMC_CrdGetSpace(UINT64 cardHandle, INT16 crdNo,INT32 *pSpace);
        //_DLL_API UINT32 IMC_CrdClrData(UINT64 cardHandle, INT16 crdNo);
        //_DLL_API UINT32 IMC_CrdClrError(UINT64 cardHandle, INT16 crdNo);
        //_DLL_API UINT32 IMC_CrdGetTargetPos(UINT64 cardHandle, INT16 crdNo,double *pPos);
        //_DLL_API UINT32 IMC_CrdGetPausePos(UINT64 cardHandle, INT16 crdNo,double *pPos);
        //_DLL_API UINT32 IMC_CrdSetSmoothParam(UINT64 cardHandle, INT16 crdNo, INT32 smoothLevel, double smoothTol);
        //_DLL_API UINT32 IMC_CrdGetSmoothParam(UINT64 cardHandle, INT16 crdNo, INT32 *pSmoothLevel, double *pSmoothTol);
        //_DLL_API UINT32 IMC_CrdGetFifoEmpty(UINT64 cardHandle, INT16 crdNo,INT16 *pEmpty);

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----7.5 多轴直线插补运动模式接口                                     ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //多轴直线插补映射建立
        //_DLL_API UINT32 IMC_SetupMutiSyncSys(UINT64 cardHandle,INT16 groupNo,INT16 *pMaskAxNo,INT16 maxAxNum);
        //获取多轴直线插补的坐标系信息
        //_DLL_API UINT32 IMC_GetMutiSyncSysInfo(UINT64 cardHandle,INT16 groupNo,INT16 *pMaskAxNo,INT16 *pMaxAxNum);
        //删除多轴直线插补映射
        //_DLL_API UINT32 IMC_DeleteMutiSyncSys(UINT64 cardHandle, INT16 groupNo);
        //多轴线段输入
        //_DLL_API UINT32 IMC_MutiSyncMotion(UINT64 cardHandle,INT16 groupNo,double *pEndPos,short *pVelRatio,short *pAccRatio,double blendRatio,INT32 userID);

        //_DLL_API UINT32 IMC_MutiSyncMotionInRatio(UINT64 cardHandle,INT16 groupNo,double *pEndPos,short *pVelRatio,short *pAccRatio,double blendRatio,INT32 userID);
        //以各轴的实际速度、加速度参数的形式进行的多轴线段输入
        //_DLL_API UINT32 IMC_MutiSyncMotionInAxVelAcc(UINT64 cardHandle,INT16 groupNo,double *pEndPos,double *pTrajVel,double *pTrajAcc,double *pTrajdec,double blendRatio,INT32 userID);
        //以所有轴的合成速度、加速度的参数形式进行的多轴线段输入
        //_DLL_API UINT32 IMC_MutiSyncMotionInSynVelAcc(UINT64 cardHandle,INT16 groupNo,double *pEndPos,double trajVel,double trajAcc,double trajdec,double blendRatio,INT32 userID);

        //多轴插补启停
        //_DLL_API UINT32 IMC_MutiSyncStart(UINT64 cardHandle, INT16 groupNo);
        //_DLL_API UINT32 IMC_MutiSyncStop(UINT64 cardHandle, INT16 groupNo,INT16 stopType);
        //多轴插补状态获取
        //_DLL_API UINT32 IMC_MutiSyncGetStatus(UINT64 cardHandle, INT16 groupNo,INT16 *pStatus);
        //多轴插补倍率读写
        //_DLL_API UINT32 IMC_MutiSyncSetRatio(UINT64 cardHandle, INT16 groupNo,double ratio);
        //_DLL_API UINT32 IMC_MutiSyncGetRatio(UINT64 cardHandle, INT16 groupNo,double *pRatio);
        //多轴插补用户ID读取
        //_DLL_API UINT32 IMC_MutiSyncGetUserID(UINT64 cardHandle, INT16 groupNo,INT32 *pUserID);
        //获取多轴插补空间余量
        //_DLL_API UINT32 IMC_MutiSyncGetSpace(UINT64 cardHandle, INT16 groupNo,INT32 *pSpace);
        //清除多轴插补缓存区数据
        //_DLL_API UINT32 IMC_MutiSyncClrData(UINT64 cardHandle, INT16 groupNo);
        //多轴插补故障清除
        //_DLL_API UINT32 IMC_MutiSyncClrError(UINT64 cardHandle, INT16 groupNo);
        //多轴插补编程模式设定
        //_DLL_API UINT32 IMC_MutiSyncSetType(UINT64 cardHandle, INT16 groupNo,INT16 type);
        //多轴插补编程模式读取
        //_DLL_API UINT32 IMC_MutiSyncGetType(UINT64 cardHandle, INT16 groupNo,INT16 *type);


        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----7.6 PTP连续运动模式接口                                     ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_PTPCAddFixTData(UINT64 cardHandle,INT16 axNo,double tgtPos,double acc,double dec,double T,double finalVel,INT16 posType);
        //_DLL_API UINT32 IMC_PTPCAddFixVmData(UINT64 cardHandle,INT16 axNo,double tgtPos,double acc,double dec,double tgtVel,double finalVel,INT16 posType);
        //_DLL_API UINT32 IMC_PTPCInterruptData(UINT64 cardHandle,INT16 axNo,double tgtPos,double acc,double dec,double tgtVel,double finalVel,INT16 posType);
        //_DLL_API UINT32 IMC_PTPCStart(UINT64 cardHandle,INT16 axNo);
        //_DLL_API UINT32 IMC_PTPCGetSpace(UINT64 cardHandle,INT16 axNo,INT32 *pSpace);
        //_DLL_API UINT32 IMC_PTPCClrData(UINT64 cardHandle,INT16 axNo);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetMtSys", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetMtSys(HANDLETYPE cardHandle, Int16 crdNo, Int16[] pMaskAxNoArray, Int16 lookAheadLen, double estopDec);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetMtSysParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetMtSysParam(HANDLETYPE cardHandle, Int16 crdNo, Int16[] pMaskAxNoArray, ref Int16 pLookAheadLen, ref double pEstopDec);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdDeleteMtSys", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdDeleteMtSys(HANDLETYPE cardHandle, Int16 crdNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetAdvParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetAdvParam(HANDLETYPE cardHandle, Int16 crdNo, ref TCrdAdvParam pCrdAdvParam);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetAdvParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetAdvParam(HANDLETYPE cardHandle, Int16 crdNo, ref TCrdAdvParam pCrdAdvParam);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetSmoothParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetSmoothParam(HANDLETYPE cardHandle, Int16 crdNo, Int32 smoothLevel, double smoothTol);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetSmoothParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetSmoothParam(HANDLETYPE cardHandle, Int16 crdNo, ref Int32 pSmoothLevel, ref double pSmoothTol);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetTrajVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetTrajVel(HANDLETYPE cardHandle, Int16 crdNo, double tgtVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetTrajAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetTrajAcc(HANDLETYPE cardHandle, Int16 crdNo, double tgtAcc);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetTrajAccAndDec", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetTrajAccAndDec(HANDLETYPE cardHandle, Int16 crdNo, double tgtAcc, double tgtdec);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetZeroFlag", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetZeroFlag(HANDLETYPE cardHandle, Int16 crdNo, Int16 ZeroFlag);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetIncMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetIncMode(HANDLETYPE cardHandle, Int16 crdNo, Int16 mode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdUserVelPlan", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdUserVelPlan(HANDLETYPE cardHandle, Int16 crdNo, double uStartVel, double uEndVel);



        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetFolVelMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetFolVelMode(HANDLETYPE cardHandle, Int16 crdNo, Int16 mode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetFolVelMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetFolVelMode(HANDLETYPE cardHandle, Int16 crdNo, ref Int16 pMode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetTrajVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetTrajVel(HANDLETYPE cardHandle, Int16 crdNo, ref double pTgtVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetTrajAccAndDec", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetTrajAccAndDec(HANDLETYPE cardHandle, Int16 crdNo, ref double pTgtAcc, ref double pTgtdec);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetZeroFlag", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetZeroFlag(HANDLETYPE cardHandle, Int16 crdNo, ref Int16 pZeroFlag);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetIncMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetIncMode(HANDLETYPE cardHandle, Int16 crdNo, ref Int16 pMode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetUserVelPlan", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetUserVelPlan(HANDLETYPE cardHandle, Int16 crdNo, ref double pUStartVel, ref double pUEndVel);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetTrajTol", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetTrajTol(HANDLETYPE cardHandle, Int16 crdNo, double tol);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetTrajTurnCoef", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetTrajTurnCoef(HANDLETYPE cardHandle, Int16 crdNo, double turnCoef);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdLineXYZ", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdLineXYZ(HANDLETYPE cardHandle, Int16 crdNo, double[] pEndPosArray, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdLineXY", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdLineXY(HANDLETYPE cardHandle, Int16 crdNo, double[] pEndPosArray, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdLineZX", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdLineZX(HANDLETYPE cardHandle, Int16 crdNo, double[] pEndPosArray, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdLineYZ", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdLineYZ(HANDLETYPE cardHandle, Int16 crdNo, double[] pEndPosArray, Int32 userID = 0);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcThreePoint", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcThreePoint(HANDLETYPE cardHandle, Int16 crdNo, double[] pMidPosArray, double[] pEndPosArray, Int32 userID);

        //空间arc插补
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_Crd3DArcCenterNormal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_Crd3DArcCenterNormal(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double[] pEndPosArray, double[] pNormalArray, double height = 0, Int32 turn = 0, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_Crd3DArcRadiusNormal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_Crd3DArcRadiusNormal(HANDLETYPE cardHandle, Int16 crdNo, double radius, double[] pEndPosArray, double[] pNormalArray, double height = 0, Int32 turn = 0, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_Crd3DArcAngleNormal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_Crd3DArcAngleNormal(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double angle, double[] pNormalArray, double height = 0, Int32 userID = 0);
        //平面arc插补
        //圆心终点编程
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcCenterXYPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcCenterXYPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double[] pEndPosArray, Int16 dir, double height = 0, Int32 turn = 0, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcCenterYZPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcCenterYZPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double[] pEndPosArray, Int16 dir, double height = 0, Int32 turn = 0, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcCenterZXPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcCenterZXPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double[] pEndPosArray, Int16 dir, double height = 0, Int32 turn = 0, Int32 userID = 0);
        //半径终点编程
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcRadiusXYPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcRadiusXYPlane(HANDLETYPE cardHandle, Int16 crdNo, double radius, double[] pEndPosArray, Int16 dir, double height = 0, Int32 turn = 0, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcRadiusYZPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcRadiusYZPlane(HANDLETYPE cardHandle, Int16 crdNo, double radius, double[] pEndPosArray, Int16 dir, double height = 0, Int32 turn = 0, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcRadiusZXPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcRadiusZXPlane(HANDLETYPE cardHandle, Int16 crdNo, double radius, double[] pEndPosArray, Int16 dir, double height = 0, Int32 turn = 0, Int32 userID = 0);
        //圆心角度编程
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcAngleXYPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcAngleXYPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double angle, double height = 0, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcAngleYZPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcAngleYZPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double angle, double height = 0, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdArcAngleZXPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdArcAngleZXPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double angle, double height = 0, Int32 userID = 0);

        //涡旋线插补
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSpiralAngleXYPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSpiralAngleXYPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double angle, double height, double m, Int32 userID);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSpiralAngleYZPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSpiralAngleYZPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double angle, double height, double m, Int32 userID);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSpiralAngleZXPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSpiralAngleZXPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double angle, double height, double m, Int32 userID);


        //椭圆插补
        /*起点作为椭圆的一个顶点,给定圆心，以及另一个顶点到圆心的距离b，给定方向，走整椭圆*/
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdEllipseFullXYPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdEllipseFullXYPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double b, Int16 dir, Int32 userID);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdEllipseFullYZPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdEllipseFullYZPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double b, Int16 dir, Int32 userID);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdEllipseFullZXPlane", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdEllipseFullZXPlane(HANDLETYPE cardHandle, Int16 crdNo, double[] pCenterArray, double b, Int16 dir, Int32 userID);

        //FIFO事件
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdWaitTime", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdWaitTime(HANDLETYPE cardHandle, Int16 crdNo, Int32 waitPeriod, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdWaitDI", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdWaitDI(HANDLETYPE cardHandle, Int16 crdNo, Int16 diNO, Int16 diType, Int16 diLevel, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetDO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetDO(HANDLETYPE cardHandle, Int16 crdNo, Int16 doNO, Int16 doType, Int16 doLevel, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetDistanceDO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetDistanceDO(HANDLETYPE cardHandle, Int16 crdNo, double waitPos, Int16 doNO, Int16 doType, Int16 doLevel, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetTimeDO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetTimeDO(HANDLETYPE cardHandle, Int16 crdNo, Int32 waitPeriod, Int16 doNO, Int16 doType, Int16 doLevel, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdPTPMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdPTPMove(HANDLETYPE cardHandle, Int16 crdNo, Int16 axNo, double tgtPos, double tgtVel, double acc, Int16 mvType, Int16 waitFlag, Int32 userID = 0);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSyncMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSyncMove(HANDLETYPE cardHandle, Int16 crdNo, Int16 axNo, double syncPos, Int32 userID = 0);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdWaitInPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdWaitInPos(HANDLETYPE cardHandle, Int16 crdNo, Int32 userID = 0);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdEndData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdEndData(HANDLETYPE cardHandle, Int16 crdNo, ref Int16 pIsFinished);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdStart", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdStart(HANDLETYPE cardHandle, Int16 crdNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdStop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdStop(HANDLETYPE cardHandle, Int16 crdNo, Int16 stopType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetStatus(HANDLETYPE cardHandle, Int16 crdNo, ref Int16 pStatus);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetRatio", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdSetRatio(HANDLETYPE cardHandle, Int16 crdNo, double ratio);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetRatio", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetRatio(HANDLETYPE cardHandle, Int16 crdNo, ref double pRatio);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetPos(HANDLETYPE cardHandle, Int16 crdNo, double[] pCrdPosArray);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetVel(HANDLETYPE cardHandle, Int16 crdNo, ref double pCrdVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetUserID", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetUserID(HANDLETYPE cardHandle, Int16 crdNo, ref Int32 pUserID);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetSpace", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetSpace(HANDLETYPE cardHandle, Int16 crdNo, ref Int32 pSpace);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdClrData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdClrData(HANDLETYPE cardHandle, Int16 crdNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdClrError", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdClrError(HANDLETYPE cardHandle, Int16 crdNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetTargetPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetTargetPos(HANDLETYPE cardHandle, Int16 crdNo, double[] pPosArray);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetPausePos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetPausePos(HANDLETYPE cardHandle, Int16 crdNo, double[] pPosArray);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetArrivalSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetArrivalSts(HANDLETYPE cardHandle, Int16 crdNo, ref Int16 pSts);

        //多轴直线插补映射建立
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetupMutiSyncSys", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetupMutiSyncSys(HANDLETYPE cardHandle, Int16 groupNo, Int16[] pMaskAxNoArray, Int16 maxAxNum);
        //获取多轴直线插补的坐标系信息
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetMutiSyncSysInfo", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetMutiSyncSysInfo(HANDLETYPE cardHandle, Int16 groupNo, Int16[] pMaskAxNoArray, ref Int16 pMaxAxNum);
        //删除多轴直线插补映射
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DeleteMutiSyncSys", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DeleteMutiSyncSys(HANDLETYPE cardHandle, Int16 groupNo);
        //多轴线段输入
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotion(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, short[] pVelRatioArray, short[] pAccRatioArray, double blendRatio, Int32 userID);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotionInRatio", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotionInRatio(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, short[] pVelRatioArray, short[] pAccRatioArray, double blendRatio, Int32 userID);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotionLookaheadInRatio", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotionLookaheadInRatio(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, short[] pVelRatioArray, short[] pAccRatioArray, double cornerRatio, Int32 userID);

        // 以各轴的实际速度、加速度参数的形式进行的多轴线段输入
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotionInAxVelAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotionInAxVelAcc(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, double[] pTrajVelArray, double[] pTrajAccArray, double[] pTrajdecArray, double blendRatio, Int32 userID);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotionLookaheadInAxVelAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotionLookaheadInAxVelAcc(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, double[] pTrajVelArray, double[] pTrajAccArray, double[] pTrajdecArray, double cornerRatio, Int32 userID);

        // 以所有轴的合成速度、加速度的参数形式进行的多轴线段输入
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotionInSynVelAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotionInSynVelAcc(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, double trajVel, double trajAcc, double trajdec, double blendRatio, Int32 userID);
        
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotionLookaheadInSynVelAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotionLookaheadInSynVelAcc(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, double trajVel, double trajAcc, double trajdec, double cornerRatio, Int32 userID);


        // 以各轴的速度、加减速时间的参数形式进行的多轴线段输入：accTime，decTime单位为ms
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotionInAccDecTime", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotionInAccDecTime(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, double[] pTrajVelArray, double accTime, double decTime, double blendRatio, Int32 userID);

        // 以运动总执行时间、加减速时间的参数形式进行的多轴线段输入：runTime，accTime，decTime单位为ms
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncMotionInRunTime", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncMotionInRunTime(HANDLETYPE cardHandle, Int16 groupNo, double[] pEndPosArray, double runTime, double accTime, double decTime, double blendRatio, Int32 userID);


        //多轴插补启停
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncStart", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncStart(HANDLETYPE cardHandle, Int16 groupNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncStop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncStop(HANDLETYPE cardHandle, Int16 groupNo, Int16 stopType);
        //多轴插补状态获取
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncGetStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncGetStatus(HANDLETYPE cardHandle, Int16 groupNo, ref Int16 pStatus);
        //多轴插补倍率读写
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncSetRatio", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncSetRatio(HANDLETYPE cardHandle, Int16 groupNo, double ratio);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncGetRatio", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncGetRatio(HANDLETYPE cardHandle, Int16 groupNo, ref double pRatio);
        //多轴插补用户ID读取
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncGetUserID", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncGetUserID(HANDLETYPE cardHandle, Int16 groupNo, ref Int32 pUserID);
        //获取多轴插补空间余量
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncGetSpace", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncGetSpace(HANDLETYPE cardHandle, Int16 groupNo, ref Int32 pSpace);
        //清除多轴插补缓存区数据
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncClrData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncClrData(HANDLETYPE cardHandle, Int16 groupNo);
        //多轴插补故障清除
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncClrError", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncClrError(HANDLETYPE cardHandle, Int16 groupNo);
        //多轴插补编程模式设定
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncSetType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncSetType(HANDLETYPE cardHandle, Int16 groupNo, Int16 type);
        //多轴插补编程模式读取
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncGetType", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncGetType(HANDLETYPE cardHandle, Int16 groupNo, ref Int16 type);


        //多轴当前合成速度读取
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncGetTrajVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncGetTrajVel(HANDLETYPE cardHandle, Int16 groupNo, double[] pVel);

        //多轴获取到位状态
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_MutiSyncGetArrivalSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_MutiSyncGetArrivalSts(HANDLETYPE cardHandle, Int16 groupNo, Int16[] pSts);


        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----7.6 捆绑PT运动模式接口                                     ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetupPtPackSys", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetupPtPackSys(HANDLETYPE cardHandle, Int16 sysNo, Int16[] pMaskAxNoArray, Int16 maxAxNum);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AddMotionPointPtPack", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AddMotionPointPtPack(HANDLETYPE cardHandle, Int16 sysNo, Int32[] pPosArray, Int16[] pTypeArray, double T, Int16 dataNum);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_AddDoPointPtPack", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_AddDoPointPtPack(HANDLETYPE cardHandle, Int16 sysNo, Int16 doNo, Int16 doType, Int16 doLevel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartPtPack", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartPtPack(HANDLETYPE cardHandle, Int16 sysNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StopPtPack", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StopPtPack(HANDLETYPE cardHandle, Int16 sysNo, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPtPackRestSpace", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPtPackRestSpace(HANDLETYPE cardHandle, Int16 sysNo, ref Int16 pSpace);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPtPackStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPtPackStatus(HANDLETYPE cardHandle, Int16 sysNo, ref Int16 pStatus);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DeletePtPackSys", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DeletePtPackSys(HANDLETYPE cardHandle, Int16 sysNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ClrPtPackData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ClrPtPackData(HANDLETYPE cardHandle, Int16 sysNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetPtPackIncMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetPtPackIncMode(HANDLETYPE cardHandle, Int16 sysNo, Int16 incMode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPtPackIncMode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPtPackIncMode(HANDLETYPE cardHandle, Int16 sysNo, ref Int16 pIncMode);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EnablePtPackNoDataProtect", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EnablePtPackNoDataProtect(HANDLETYPE cardHandle, Int16 sysNo, double []pThresholdVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DisablePtPackNoDataProtect", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DisablePtPackNoDataProtect(HANDLETYPE cardHandle, Int16 sysNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPtPackNoDataProtectStatus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPtPackNoDataProtectStatus(HANDLETYPE cardHandle, Int16 sysNo, Int16 []pEnSts,  double []pThresholdVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ClrPtPackError", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ClrPtPackError(HANDLETYPE cardHandle, Int16 sysNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetPtPackError", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetPtPackError(HANDLETYPE cardHandle, Int16 sysNo, ref Int16 pErr);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PtPackGetArrivalSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PtPackGetArrivalSts(HANDLETYPE cardHandle, Int16 sysNo, ref Int16 pSts);


        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdSetSmoothParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_CrdSetSmoothParam(HANDLETYPE cardHandle, Int16 crdNo, Int32 smoothLevel, double smoothTol);
        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetSmoothParam", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_CrdGetSmoothParam(HANDLETYPE cardHandle, Int16 crdNo, ref Int32 pSmoothLevel, ref double pSmoothTol);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_CrdGetFifoEmpty", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_CrdGetFifoEmpty(HANDLETYPE cardHandle, Int16 crdNo, ref Int16 pEmpty);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PTPCAddFixTData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PTPCAddFixTData(HANDLETYPE cardHandle, Int16 axNo, double tgtPos, double acc, double dec, double T, double finalVel, Int16 posType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PTPCAddFixVmData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PTPCAddFixVmData(HANDLETYPE cardHandle, Int16 axNo, double tgtPos, double acc, double dec, double tgtVel, double finalVel, Int16 posType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PTPCInterruptData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PTPCInterruptData(HANDLETYPE cardHandle, Int16 axNo, double tgtPos, double acc, double dec, double tgtVel, double finalVel, Int16 posType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PTPCStart", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PTPCStart(HANDLETYPE cardHandle, Int16 axNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PTPCGetSpace", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PTPCGetSpace(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pSpace);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_PTPCClrData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_PTPCClrData(HANDLETYPE cardHandle, Int16 axNo);


        //[DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEcatPtVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        //public static extern UInt32 IMC_GetEcatPtVal(HANDLETYPE cardHandle, Int16 ptNo, ref double pValue);

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----7.8 多轴插补立即运动模式接口                                   ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateLineMoveInSynVelAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateLineMoveInSynVelAcc(HANDLETYPE cardHandle, Int16[] pMaskAxNoArray, Int16 axNum, double[] pEndPosArray, double trajVel, double trajAcc, double trajDec, double smoothCoef, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateLineMoveInAxisVelAcc", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateLineMoveInAxisVelAcc(HANDLETYPE cardHandle, Int16[] pMaskAxNoArray, Int16 axNum, double[] pEndPosArray, double[] pAxVelArray, double[] pAxAccArray, double[] pAxDecArray, double smoothCoef, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateArcThreePointMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateArcThreePointMove(HANDLETYPE cardHandle, Int16[] pMaskAxNoArray, Int16 axNum, double[] pMidPosArray, double[] pEndPosArray, double trajVel, double trajAcc, double trajDec, double smoothCoef, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateArcCenterMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateArcCenterMove(HANDLETYPE cardHandle, Int16[] pMaskAxNoArray, Int16 axNum, double[] pCenterArray, double[] pEndPosArray, Int16 dir, double height, int turn, double trajVel, double trajAcc, double trajDec, double smoothCoef, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateArcRadiusMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateArcRadiusMove(HANDLETYPE cardHandle, Int16[] pMaskAxNoArray, Int16 axNum, double radius, double[] pEndPosArray, Int16 dir, double height, int turn, double trajVel, double trajAcc, double trajDec, double smoothCoef, Int16 type);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateArcAngleMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateArcAngleMove(HANDLETYPE cardHandle, Int16[] pMaskAxNoArray, Int16 axNum, double[] pCenterArray, double angle, double height, double trajVel, double trajAcc, double trajDec, double smoothCoef);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateMoveGetVel", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateMoveGetVel(HANDLETYPE cardHandle, Int16 axNo, ref double pTrajVel);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateMoveStop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateMoveStop(HANDLETYPE cardHandle, Int16 axNo);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ImmediateMoveEStop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ImmediateMoveEStop(HANDLETYPE cardHandle, Int16 axNo, double estopDec);


        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----7.9 周期运动模式接口                                   ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_SetCycleMovePara(UINT64 cardHandle, INT16 axNo,INT32 cycleDis);
        //_DLL_API UINT32 IMC_GetCycleMovePara(UINT64 cardHandle, INT16 axNo,INT32 *pCycleDis);
        //_DLL_API UINT32 IMC_StartCycleMove(UINT64 cardHandle, INT16 axNo,INT32 tgtPos,INT16 posType);
        //_DLL_API UINT32 IMC_StopCycleMove(UINT64 cardHandle, INT16 axNo,INT16 stopType);
        //_DLL_API UINT32 IMC_GetCycleMovePrfPos(UINT64 cardHandle, INT16 axNo,INT32 *pPrfPos);
        //_DLL_API UINT32 IMC_GetCycleMoveEncPos(UINT64 cardHandle, INT16 axNo,INT32 *pEncPos);
        //_DLL_API UINT32 IMC_SetCycleMoveCurPos(UINT64 cardHandle, INT16 axNo,INT32 setPos);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetCycleMovePara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetCycleMovePara(HANDLETYPE cardHandle, Int16 axNo, Int32 cycleDis);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCycleMovePara", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCycleMovePara(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pCycleDis);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StartCycleMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StartCycleMove(HANDLETYPE cardHandle, Int16 axNo, Int32 tgtPos, Int16 posType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_StopCycleMove", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_StopCycleMove(HANDLETYPE cardHandle, Int16 axNo, Int16 stopType);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCycleMovePrfPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCycleMovePrfPos(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pPrfPos);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetCycleMoveEncPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetCycleMoveEncPos(HANDLETYPE cardHandle, Int16 axNo, ref Int32 pEncPos);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetCycleMoveCurPos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetCycleMoveCurPos(HANDLETYPE cardHandle, Int16 axNo, Int32 setPos);

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        /*----8.0 事件管理                                             ---*/
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        //_DLL_API UINT32 IMC_SetEventIO(UINT64 cardHandle,INT16 group,TEventIO *pEventIOPara);
        //_DLL_API UINT32 IMC_GetEventIO(UINT64 cardHandle,INT16 group,TEventIO *pEventIOPara);
        //_DLL_API UINT32 IMC_EnableEventIO(UINT64 cardHandle,INT16 group);
        //_DLL_API UINT32 IMC_DisableEventIO(UINT64 cardHandle,INT16 group);

        //_DLL_API UINT32 IMC_SetEventDiMotion(UINT64 cardHandle,INT16 group,TEventDiMotion *pEventDiMotionPara);
        //_DLL_API UINT32 IMC_GetEventDiMotion(UINT64 cardHandle,INT16 group,TEventDiMotion *pEventDiMotionPara);
        //_DLL_API UINT32 IMC_EnableEventDiMotion(UINT64 cardHandle,INT16 group);
        //_DLL_API UINT32 IMC_DisableEventDiMotion(UINT64 cardHandle,INT16 group);
        //_DLL_API UINT32 IMC_GetEventDiMotionSts(UINT64 cardHandle,INT16 group,INT16 *pSts);
        //_DLL_API UINT32 IMC_ClearEventDiMotionSts(UINT64 cardHandle,INT16 group);

        //_DLL_API UINT32 IMC_SetEventCompareOut(UINT64 cardHandle,INT16 group,TEventCompareOut *pEventCompareOutPara);
        //_DLL_API UINT32 IMC_GetEventCompareOut(UINT64 cardHandle,INT16 group,TEventCompareOut *pEventCompareOutPara);
        //_DLL_API UINT32 IMC_EnableEventCompareOut(UINT64 cardHandle,INT16 group);
        //_DLL_API UINT32 IMC_DisableEventCompareOut(UINT64 cardHandle,INT16 group);

        //_DLL_API UINT32 IMC_SetEventVirtualIOVal(UINT64 cardHandle,INT16 port,INT16 val);
        //_DLL_API UINT32 IMC_GetEventVirtualIOVal(UINT64 cardHandle,INT16 port,INT16 *pVal);
        //_DLL_API UINT32 IMC_GetEventVirtualIOUseSts(UINT64 cardHandle,INT16 port,INT16 *pSts);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEventIO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEventIO(HANDLETYPE cardHandle, Int16 group, ref TEventIO pEventIOPara);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventIO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventIO(HANDLETYPE cardHandle, Int16 group, ref TEventIO pEventIOPara);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EnableEventIO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EnableEventIO(HANDLETYPE cardHandle, Int16 group);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DisableEventIO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DisableEventIO(HANDLETYPE cardHandle, Int16 group);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventIOEnSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventIOEnSts(HANDLETYPE cardHandle, Int16 group, ref Int16 enSts);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DestroyEventIO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DestroyEventIO(HANDLETYPE cardHandle, Int16 group);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventIOSetupSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventIOSetupSts(HANDLETYPE cardHandle, Int16 group, ref Int16 pSts);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEventDiMotion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEventDiMotion(HANDLETYPE cardHandle, Int16 group, ref TEventDiMotion pEventDiMotionPara);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventDiMotion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventDiMotion(HANDLETYPE cardHandle, Int16 group, ref TEventDiMotion pEventDiMotionPara);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EnableEventDiMotion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EnableEventDiMotion(HANDLETYPE cardHandle, Int16 group);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DisableEventDiMotion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DisableEventDiMotion(HANDLETYPE cardHandle, Int16 group);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventDiMotionSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventDiMotionSts(HANDLETYPE cardHandle, Int16 group, ref Int16 pSts);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ClearEventDiMotionSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_ClearEventDiMotionSts(HANDLETYPE cardHandle, Int16 group);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_ClearEventDiMotionSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventDiMotionEnSts(HANDLETYPE cardHandle, Int16 group, ref Int16 enSts);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DestroyEventDiMotion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DestroyEventDiMotion(HANDLETYPE cardHandle, Int16 group);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventDiMotionSetupSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventDiMotionSetupSts(HANDLETYPE cardHandle, Int16 group, ref Int16 pSts);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEventCompareOut", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEventCompareOut(HANDLETYPE cardHandle, Int16 group, ref TEventCompareOut pEventCompareOutPara);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventCompareOut", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventCompareOut(HANDLETYPE cardHandle, Int16 group, ref TEventCompareOut pEventCompareOutPara);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EnableEventCompareOut", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EnableEventCompareOut(HANDLETYPE cardHandle, Int16 group);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DisableEventCompareOut", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DisableEventCompareOut(HANDLETYPE cardHandle, Int16 group);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventCompareOutEnSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventCompareOutEnSts(HANDLETYPE cardHandle, Int16 group, ref Int16 enSts);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_DestroyEventCompareOut", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_DestroyEventCompareOut(HANDLETYPE cardHandle, Int16 group);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventCompareOutSetupSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventCompareOutSetupSts(HANDLETYPE cardHandle, Int16 group, ref Int16 pSts);


        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_SetEventVirtualIOVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_SetEventVirtualIOVal(HANDLETYPE cardHandle, Int16 port, Int16 val);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventVirtualIOVal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventVirtualIOVal(HANDLETYPE cardHandle, Int16 port, ref Int16 pVal);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventVirtualIOUseSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventVirtualIOUseSts(HANDLETYPE cardHandle, Int16 port, ref Int16 pSts);

        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_EnableEventVirtualIO", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_EnableEventVirtualIO(HANDLETYPE cardHandle, Int16 port, Int16 en);
        [DllImport(EtherCATConfigApiDllName, EntryPoint = "IMC_GetEventVirtualIOEnableSts", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 IMC_GetEventVirtualIOEnableSts(HANDLETYPE cardHandle, Int16 port, ref Int16 pSts);

        #endregion
    }
}
