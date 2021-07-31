using System.Runtime.InteropServices;
using System;

namespace lctdevice
{
    public class ecat_motion
    {
        #region ecat_motion接口中用到的结构体
 
		/*网络中的从站资源*/
        public struct SL_RES
        {
            public int SlaveNum;	//从站个数
			public int ServoNum;	//伺服轴数
			public int DiNum;		//数字量输入通道数
			public int DoNum;		//数字量输出通道数
			public int AiNum;		//模拟量输入通道数
			public int AoNum;		//模拟量输出通道数
        }
		/*从站信息*/
		public struct SL_INFO
		{
			public int VendorID;	//厂家ID
			public int ProductCode;	//产品编号
			public int RevisionNo;	//版本号
			public int SlaveType;	//从站类型，0-伺服，3-IO，16-耦合器
			public int ModuleNum;	//从站的模块数量（从站为IO时有效）
			[System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I4)]
			public int[] ModuleId;	//模块ID（从站为IO时有效）
		}
		
        /*点位模式运动参数*/
        public struct CmdPrm
        {
            public short sTime;
            public double acc;
            public double dec;
            
        }
		
        
       
		
        /*插补运动坐标系参数*/
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct CrdCfg
        {
            /// short
            public short dimension;
            /// short[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I2)]
            public short[] axis;
            /// short
            public short setOriginFlag;
            /// int[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I4)]
            public int[] orignPos;
            /// short
            public short evenTime;
            /// double
            public double synVelMax;
            /// double
            public double synAccMax;
            /// double
            public double synDecSmooth;
            /// double
            public double synDecAbrupt;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct CrdBufOperation
        {
            public ushort delay;                         // 延时时间
            public short doType;                        // 缓存区IO的类型,0:不输出IO
            public ushort doAddress;					 // IO模块地址
            public ushort doMask;                        // 缓存区IO的输出控制掩码
            public ushort doValue;                       // 缓存区IO的输出值
            public short dacChannel;					 // DAC输出通道
            public short dacValue;					     // DAC输出值
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U2)]
            public ushort[] dataExt;               // 辅助操作扩展数据
        }

        //前瞻缓冲区；与前瞻相关的数据结构
        public struct CrdBlockData
        {
            public short iMotionType;                             // 运动类型,0为直线插补,1为2D圆弧插补,2为3D圆弧插补,6为IO,7为延时，8位DAC
            public short iCirclePlane;                            // 圆弧插补的平面;XY—1，YZ-2，ZX-3
            public short arcPrmType;							   // 1-圆心表示法；2-半径表示法
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I4)]
            public int[] lPos;            // 当前段各轴终点位置

            public double dRadius;                                // 圆弧插补的半径
            public short iCircleDir;                             // 圆弧旋转方向,0:顺时针;1:逆时针
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R8)]
            public double[] dCenter;                             // 2维圆弧插补的圆心相对坐标值，即圆心相对于起点位置的偏移量
            // 3维圆弧插补的圆心在用户坐标系下的坐标值
            public int height;								   // 螺旋线的高度
            public double pitch;	// 螺旋线的螺距
            //double[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R8)]
            public double[] beginPos;
            //double[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R8)]
            public double[] midPos;
            //double[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R8)]
            public double[] endPos;
            //double[3][3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R8)]
            public double[] R_inv;
            //double[3][3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R8)]
            public double[] R;

            public double dVel;                                   // 当前段合成目标速度
            public double dAcc;                                   // 当前段合成加速度
            public short loop;
            public short iVelEndZero;                             // 标志当前段的终点速度是否强制为0,值0——不强制为0;值1——强制为0
            public CrdBufOperation operation;
            public double dVelEnd;                                // 当前段合成终点速度
            public double dVelStart;                              // 当前段合成的起始速度
            public double dResPos;                                // 当前段合成位移量
        }

        #endregion

        //初始化部分
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Open(short card, short param);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Close(short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Reset(short card);
		
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetVersion(out byte pVersion, int size, short card);

		//获取网络中从站资源
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetSlaveResource(out SL_RES pRes, short card);
        
		//获取从站信息
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetSlaveInfo(out SL_INFO pInfo, short slaveNo, short card);
		
		//加载eni文件
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_LoadEni(string eniPath, short card);
		
		//从站连接
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_ConnectECAT(short option, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_DisconnectECAT(short card);

        //SDO写
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_EcatSDOWrite(short axis, short index, short subindex, uint data, short data_size, short card);
        
		//SDO读
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_EcatSDORead(short axis, short index, short subindex, short data_size, out uint pBuf, short count, short card);
		
		//驱动器模式
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_EcatSetOperationMode(short axis, short mode, short card);

		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_EcatGetOperationMode(short axis, ref short mode, short card);

		//设置驱动器模式，模式6为回零模式，模式8为CSP模式
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetHomingMode(short axis, short mode, short card);

		//设置回零参数
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_SetHomingPrm(short axis, short method, int offset, uint speed1, uint speed2, uint acc, ushort probeFunction, short card=0);
        
        
        //获取回零参数
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetHomingPrm(short axis,ref short method,ref int offset,ref uint speed1,ref uint speed2,ref uint acc,ref ushort probeFunction, short card=0);

        //开始驱动器回零
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_HomingStart(short axis, short card = 0);

        //获取驱动器回零的状态
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_GetEcatHomingStatus(short axis, out short phomingStatus, short card);
		
		//回零取消
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_HomeCancel(uint mask, short card);
		
		//设置伺服数字输出
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_SetEcatDigitalOut(short axis, uint digitalOut, short card);
		
		//读取伺服数字输入
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_GetEcatDigitalInput(short axis, out uint digitalInput, short card);

        /*轴基本操作*************************************************************************************************************/
        //使能
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Servo_On(short axis, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Servo_Off(short axis, short card);

        //软限位
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetSoftLimit(short axis, int positive, int negative, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetSoftLimit(short axis, out int pPositive, out int pNegative, short card);
		
		//设置到位判断阈值
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_SetAxisBand(short axis, uint band, uint time, short card);
		
		//读取到位判断阈值
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_GetAxisBand(short axis, out uint pBand, out uint pTime, short card);

        //停止加速度
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetStopDec(short axis, double dSmoothDec, double dEmergencyDec, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetStopDec(short axis, out double dSmoothDec, out double dEmergencyDec, short card);
       
        /*运动状态检测指令************************************************************************************************************/
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetSts(short axis, out int pSts, short count, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_ClrSts(short axis, short count, short card);
        
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetCmd(short axis, out double pValue, short count, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetCmdVel(short axis, out double pValue, short count, short card);
        
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetMove(short axis, ref CmdPrm pPrm, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetMove(short axis, out CmdPrm pPrm, short card);
        
		//开始点位运动
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_AbsMove(short axis, int pos, double vel, short card);
		
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_RelMove(short axis, int pos, double vel, short card);
		
        
		//开始Jog运动
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_Jog(short axis, double vel, short card);
		
		//停止轴运动
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Stop(uint mask, uint option, short card);

        //数字IO
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Set_Digital_Chn_Output(short channel, short Value, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Get_Digital_Chn_Output(short channel,out short pValue, short card);

		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_Get_Digital_Chn_Input(short channel, out short pValue, short card);

		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_Set_Digital_Port_Output(short chnBegin, uint lValue, uint lMask, short card);
		
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_Get_Digital_Port_Output(short chnBegin, ref uint lValue, short card);
		
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_Get_Digital_Port_Input(short chnBegin, ref uint lValue, short card);
		
		//模拟量IO
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_Set_Analog_Output(short channel, ref short pValue, short count, short card);

		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_Get_Analog_Output(short channel, out short pValue, short count, short card);

		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_Get_Analog_Input(short channel, out short pValue, short count, short card);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Get_Analog_Input_32(short channel, out uint pValue,  short card);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Set_Analog_Output_32(short channel, ref uint pValue,  short card);

        //访问编码器
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetEncPos(short encoder, out double pValue, short count, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetEncVel(short encoder, out double pValue, short count, short card);
        
         //PT模式
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_PrfPt(short axis, short mode, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_PtSpace(short axis, out short pSpace, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_PtData(short axis, int pos, int time, short type, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_PtClear(short axis, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_PtStart(uint mask, uint option, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetPtLoop(short axis, int loop, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetPtLoop(short axis, out int loop, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetPtMemory(short axis, short memory, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetPtMemory(short axis, out short memory, short card);

        //Gear 运动
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Gear(short axis, short dir, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetGearMaster(short axis, short masterindex, short masterType, short masterItem, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetGearMaster(short axis, out short masterindex, out short masterType, out short masterItem, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetGearRatio(short axis, int masterEven, int slaveEven, int masterSlope, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetGearRatio(short axis, out int masterEven, out int slaveEven, out int masterSlope, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GearStart(uint mask, short card);

        //插补
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetCrd(short crd, ref CrdCfg pCrdPrm, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetCrd(short crd, out CrdCfg pCrdPrm, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_CrdSpace(short crd, out int pSpace, short count, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_CrdClear(short crd, short count, short fifo, short card);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetLastCrdPos(short crd, out int position,short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_BufIO(short crd, ushort channel, ushort doValue, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_BufDelay(short crd, uint delayTime, short fifo, short card);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_CrdStart(short mask, short option, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_CrdStop(short mask, short option, short card);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_CrdStatus(short crd, out short pSts, out short pCmdNum, out int pSpace, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetCrdPos(short crd, out double pPos, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetCrdVel(short crd, out double pSynVel, short card);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetCrdStopDec(short crd, double decSmoothStop, double decAbruptStop, short card);
		
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetCrdStopDec(short crd, out double decSmoothStop, out double decAbruptStop, short card);

        //基于2维圆弧半径加终点的输入方式的螺旋线插补  xyz是终点坐标 终点坐标要跟螺距信息匹配
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_HelicalLineXYR(short crd, int x, int y, int z, double radius, short circleDir, double pitch, double synVel, double synAcc, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_HelicalLineYZR(short crd, int y, int z, int x, double radius, short circleDir, double pitch, double synVel, double synAcc, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_HelicalLineZXR(short crd, int z, int x, int y, double radius, short circleDir, double pitch, double synVel, double synAcc, short fifo, short card);

		//基于2维圆弧圆心和终点的输入方式的螺旋线插补	xyz是终点坐标 终点坐标要跟螺距信息匹配
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_HelicalLineXYC(short crd, int x, int y, int z, double xCenter, double yCenter, short circleDir, double pitch, double synVel, double synAcc, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_HelicalLineYZC(short crd, int y, int z, int x, double yCenter, double zCenter, short circleDir, double pitch, double synVel, double synAcc, short fifo, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_HelicalLineZXC(short crd, int z, int x, int y, double zCenter, double xCenter, short circleDir, double pitch, double synVel, double synAcc, short fifo, short card);

		//前瞻部分
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetVelPlanning(short crd, short fifo, double T, double accMax, short n,ref double  pCrdData, short card);
        
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_CrdData(short crd,ref double pCrdData, short fifo, short card); //CrdBlockData

         [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_ResetFpga(short card);

		//获取急停状态，0-未触发，1-触发
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_GetEmg(ref short emg, short card);
		
		//清除急停触发状态
		[DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern short M_ClrEmg(short card);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void M_WriteAxisParam(string AxisNum, string ParamName, string Param, string filePath);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_LoadParamFromFile(string filename,short axisNum, short card);
        //获取轴状态字

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_EcatStatusWord(short axis, ref ushort statusword, short card);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Line(short crd, short dimension,ref short axisArray,ref  int posArray, double mVel, double acc, double velEnd = 0, short fifo = 0, short card = 0);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Arc2R(short crd, ref short axisArray, ref int posArray, double radius, short circleDir, double mVel, double synAcc, short fifo = 0, short card = 0);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Arc2C(short crd, ref short axisArray, ref int posArray,ref double centerArray, short circleDir, double mVel, double synAcc, short fifo = 0, short card = 0);

        /// <summary>
        /// 向插补坐标系中插入3D圆弧
        /// </summary>
        /// <param name="crd">坐标系号</param>
        /// <param name="endPosArray">终点位置坐标数组首地址</param>
        /// <param name="midPosArray">圆弧过程点位置坐标数组首地址</param>
        /// <param name="synVel">合成速度</param>
        /// <param name="synAcc">合成加速度</param>
        /// <param name="velEnd">停止速度</param>
        /// <param name="fifo">FIFO号</param>
        /// <param name="card">卡号</param>
        /// <returns></returns>
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Arc3D(short crd, ref double endPosArray, ref double midPosArray, double synVel, double synAcc, double velEnd = 0, short fifo = 0, short card = 0);


        /*********************************************************
        * 函数说明: 急停动作设置
        * 参数说明：
        *   EAction - 急停触发时的动作，
        *       0x00 - 不减速，直接掉使能（默认值）
        *       0x01 - 以缓停减速度停机，然后掉使能
        *       0x02 - 以急停减速度停机，然后掉使能
        *       0x11 - 以缓停减速度停机，不掉使能
        *       0x12 - 以急停减速度停机，不掉使能
        *   card - 卡号，从0开始
        ********************************************************/
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_SetEmgAction(short EAction = 0, short card = 0);

        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_GetEmgAction(ref short EAction, short card = 0);


        /*********************************************************
        * 函数功能: 读取伺服错误码
        * 参数说明：axis   - 轴号，从1开始
        *			pCode  - 错误码
        *			count  - 读取的个数
        *			card   - 主站卡地址，从0开始
        ********************************************************/
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_ReadErrorCode(short axis, ref short  pCode, short count = 1, short card = 0);


        /*********************************************************
        * 函数功能: 读取目标转矩
        * 参数说明：axis    - 轴号，从1开始
        *			pTorque - 目标转矩
        *			count   - 读取的个数
        *			card    - 主站卡地址，从0开始
        ********************************************************/
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_ReadActualTorque(short axis, ref short pTorque, short count = 1, short card = 0);

        /*********************************************************
        * 函数功能: 设置目标转矩
        * 参数说明：axis   - 轴号，从1开始
        *			torque - 目标转矩
        *			card   - 主站卡地址，从0开始
        ********************************************************/
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_WriteTargetTorque(short axis, short torque, short card = 0);

        /// <summary>
        /// 多轴插补运动
        /// </summary>
        /// <param name="demension">插补维度</param>
        /// <param name="axis">参与插补轴的数组首地址</param>
        /// <param name="position">插补坐标的位置首地址</param>
        /// <param name="acc">插补加速度</param>
        /// <param name="vel">插补速度</param>
        /// <param name="card">卡号</param>
        /// <returns></returns>
        [DllImport("ecat_motion.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short M_Line_All(short demension, ref short axis, ref int position, double acc, double vel, short card = 0);


      
    }
}
