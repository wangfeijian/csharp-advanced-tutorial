using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace Adlink
{
	
	//ADLINK Struct++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	[StructLayout(LayoutKind.Sequential)]
	public struct STR_SAMP_DATA_4CH 
	{			
		public Int32 tick;
		public Int32 data0; //Total channel = 4
		public Int32 data1;
		public Int32 data2;
		public Int32 data3;
	} 

	[StructLayout(LayoutKind.Sequential)]
	public struct MOVE_PARA
	{
        public Int16 i16_accType;	//Axis parameter
        public Int16 i16_decType;	//Axis parameter
        public Int32 i32_acc;		//Axis parameter
        public Int32 i32_dec;		//Axis parameter
        public Int32 i32_initSpeed;	//Axis parameter
        public Int32 i32_maxSpeed;	//Axis parameter
        public Int32 i32_endSpeed; 	//Axis parameter
	} 

	[StructLayout(LayoutKind.Sequential)]
	public struct POINT_DATA
	{
		public Int32 i32_pos;		// Position data (relative or absolute) (pulse)
		public Int16 i16_accType;	// Acceleration pattern 0: T-curve,  1: S-curve
		public Int16 i16_decType;	// Deceleration pattern 0: T-curve,  1: S-curve
		public Int32 i32_acc;		// Acceleration rate ( pulse / ss )
		public Int32 i32_dec;		// Deceleration rate ( pulse / ss )
		public Int32 i32_initSpeed;	// Start velocity	( pulse / s )
		public Int32 i32_maxSpeed;	// Maximum velocity  ( pulse / s )
		public Int32 i32_endSpeed; 	// End velocity		( pulse / s )
		public Int32 i32_angle;		// Arc move angle    ( degree, -360 ~ 360 )
		public Int32 u32_dwell;		// Dwell times       ( unit: ms )
		public Int32 i32_opt;    	// Option //0xABCD , D:0 absolute, 1:relative
	}
			
	[StructLayout(LayoutKind.Sequential)]
	public struct PNT_DATA
	{
	    // Point table structure (One dimension)
	    public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32  i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32  i32_theta;      // x-y plane arc move angle (0.001 degree), [-360000,360000]
        public Int32  i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32  i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32  i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32  i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32  i32_ve;         // ending velocity (pulse/s), [0,2147484647]
	}
			
    [StructLayout(LayoutKind.Sequential)]
	public struct PNT_DATA_2D
	{
	      public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32  i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32  i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Int32  i32_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
        public Int32  i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32  i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32  i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32  i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32  i32_ve;         // ending velocity (pulse/s), [0,2147484647]
	}
			
	[StructLayout(LayoutKind.Sequential)]
	public struct PNT_DATA_2D_F64
	{
	    public UInt32  u32_opt;        // option, [0x00000000,0xFFFFFFFF]
	    public Double  f64_x;          // x-axis component (pulse), [-2147483648,2147484647]
	    public Double  f64_y;          // y-axis component (pulse), [-2147483648,2147484647]
	    public Double  f64_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
	    public Double  f64_acc;        // acceleration rate (pulse/ss), [0,2147484647]
	    public Double  f64_dec;        // deceleration rate (pulse/ss), [0,2147484647]
	    public Double  f64_vi;         // initial velocity (pulse/s), [0,2147484647]
	    public Double  f64_vm;         // maximum velocity (pulse/s), [0,2147484647]
	    public Double  f64_ve;         // ending velocity (pulse/s), [0,2147484647]
        public Double  f64_sf;			   // s-factor [0.0 ~ 1.0]
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PNT_DATA_4DL
	{
	    public UInt32 u32_opt;        // option, [0x00000000,0xFFFFFFFF]
        public Int32  i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
        public Int32  i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
        public Int32  i32_z;          // z-axis component (pulse), [-2147483648,2147484647]
        public Int32  i32_u;          // u-axis component (pulse), [-2147483648,2147484647]
        public Int32  i32_acc;        // acceleration rate (pulse/ss), [0,2147484647]
        public Int32  i32_dec;        // deceleration rate (pulse/ss), [0,2147484647]
        public Int32  i32_vi;         // initial velocity (pulse/s), [0,2147484647]
        public Int32  i32_vm;         // maximum velocity (pulse/s), [0,2147484647]
        public Int32  i32_ve;         // ending velocity (pulse/s), [0,2147484647]
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT_DATA_EX
	{
        public Int32 i32_pos;           //(Center)Position data (could be relative or absolute value) 
        public Int16 i16_accType;       //Acceleration pattern 0: T curve, 1:S curve   
        public Int16 i16_decType;       // Deceleration pattern 0: T curve, 1:S curve 
        public Int32 i32_acc;           //Acceleration rate ( pulse / sec 2 ) 
        public Int32 i32_dec;           //Deceleration rate ( pulse / sec 2  ) 
        public Int32 i32_initSpeed;     //Start velocity ( pulse / s ) 
        public Int32 i32_maxSpeed;      //Maximum velocity    ( pulse / s ) 
        public Int32 i32_endSpeed;      //End velocity  ( pulse / s )     
        public Int32 i32_angle;         //Arc move angle ( degree, -360 ~ 360 ) 
        public UInt32 u32_dwell;        //dwell times ( unit: ms ) *Divided by system cycle time. 
        public Int32 i32_opt;           //Point move option. (*) 
        public Int32 i32_pitch;			// pitch for helical move
        public Int32 i32_totalheight;	// total hight
	    public Int16 i16_cw;			// cw or ccw
        public Int16 i16_opt_ext;		// option extend
	}
			
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct POINT_DATA2
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public Int32[] i32_pos;                   // Position data (relative or absolute) (pulse)

		public Int32 i32_initSpeed;	              // Start velocity	( pulse / s ) 
		public Int32 i32_maxSpeed;	              // Maximum velocity  ( pulse / s ) 
		public Int32 i32_angle;		              // Arc move angle    ( degree, -360 ~ 360 ) 
		public UInt32 u32_dwell;		          // Dwell times       ( unit: ms ) 
		public Int32 i32_opt;    	              // Option //0xABCD , D:0 absolute, 1:relative
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct POINT_DATA3
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public Int32[] i32_pos;

		public Int32 i32_maxSpeed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public Int32[] i32_endPos;

		public Int32 i32_dir;
		public Int32 i32_opt;
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VAO_DATA
	{
		//Param
		public Int32 outputType;	//Output type, [0, 3]
		public Int32 inputType;	    //Input type, [0, 1]
		public Int32 config;	    //PWM configuration according to output type
		public Int32 inputSrc;		//Input source by axis, [0, 0xf]
	
		//Mapping table
		public Int32 minVel;		                     //Minimum linear speed, [ positive ]
		public Int32 velInterval;	                     //Speed interval, [ positive ]
		public Int32 totalPoints;	                     //Total points, [1, 32]
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public Int32[] mappingDataArr;	 //mapping data array
	}
			
    [StructLayout(LayoutKind.Sequential)]
	public struct PTSTS
	{
		public UInt16 BitSts;	        //b0: Is PTB work? [1:working, 0:Stopped]
					                    //b1: Is point buffer full? [1:full, 0:not full]
					                    //b2: Is point buffer empty? [1:empty, 0:not empty]
					                    //b3, b4, b5: Reserved for future
					                    //b6~: Be always 0
		public UInt16 PntBufFreeSpace; 
		public UInt16 PntBufUsageSpace;
		public UInt32 RunningCnt;
	}
			
	[StructLayout(LayoutKind.Sequential)]
	public struct LPSTS			
	{
		public UInt32 MotionLoopLoading;
		public UInt32 HostLoopLoading;
		public UInt32 MotionLoopLoadingMax;
		public UInt32 HostLoopLoadingMax;
	}
	
	
	
	[StructLayout(LayoutKind.Sequential)]
	public struct DEBUG_DATA
	{
		public UInt16  ServoOffCondition;	
		public Double  DspCmdPos;		
		public Double  DspFeedbackPos;		
		public Double  FpgaCmdPos;		
		public Double  FpgaFeedbackPos;		
		public Double  FpgaOutputVoltage;		
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct DEBUG_STATE
	{
		public UInt16 AxisState;
		public UInt16 GroupState;
		public UInt16 AxisSuperState;
	} 
			
	[StructLayout(LayoutKind.Sequential)]
	public struct PTDWL
	{
		public Double DwTime; //Unit is ms
	}

    [StructLayout(LayoutKind.Sequential)]
    public struct PTLINE
    {
        public Int32 Dim;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Double[] Pos;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PTA2CA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] Index;       //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Double[] Center;  //Center Arr

        public Double Angle;                          //Angle
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct PTA2CE
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public Byte[] Index; //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public Double[] Center; //

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public Double[] End; // 

		public Int16 Dir; //
	}

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
	public struct PTA3CA
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[]	Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[]	Normal; //Normal Arr

		public Double Angle;                         //Angle
	}

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
	public struct PTA3CE
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[]	End;    //End Arr

		public Int16 Dir; //
	}

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
	public struct PTHCA
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[] Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[]	Normal; //Normal Arr

		public Double Angle;                         //Angle
		public Double DeltaH;
		public Double FinalR;
	}

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    [StructLayout(LayoutKind.Sequential)] // revised 20160801
	public struct PTHCE
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Byte[] Index;      //Index X,Y

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[]	Center; //Center Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[]	Normal; //Normal Arr

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public Double[]	End;    //End Arr

		public Int16 Dir; //
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct PTINFO
	{
		public Int32 Dimension;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Int32[] AxisArr;
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct STR_SAMP_DATA_8CH
    {
	    public Int32 tick;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public Int32[] data; //Total channel = 8
    }
	   		   
	[StructLayout(LayoutKind.Sequential)]
	public struct JOG_DATA
	{
        public Int16 i16_jogMode;	  // Jog mode. 0:Free running mode, 1:Step mode
        public Int16 i16_dir;		  // Jog direction. 0:positive, 1:negative direction
        public Int16 i16_accType;	  // Acceleration pattern 0: T-curve,  1: S-curve
        public Int32 i32_acc;		  // Acceleration rate ( pulse / ss )
        public Int32 i32_dec;		  // Deceleration rate ( pulse / ss )
        public Int32 i32_maxSpeed;	  // Positive value, maximum velocity  ( pulse / s )
        public Int32 i32_offset;	  // Positive value, a step (pulse)
        public Int32 i32_delayTime;  // Delay time, ( range: 0 ~ 65535 millisecond, align by cycle time)
	} 

	[StructLayout(LayoutKind.Sequential)]
	public struct HOME_PARA
	{
        public ushort u8_homeMode;
        public ushort u8_homeDir;
        public ushort u8_curveType;
        public Int32 i32_orgOffset;
        public Int32 i32_acceleration;
        public Int32 i32_startVelocity;
        public Int32 i32_maxVelocity;
        public Int32 i32_OrgVelocity;
	}
			
	[StructLayout(LayoutKind.Sequential)]
	public struct POS_DATA_2D
	{
	    public UInt32  u32_opt;        // option, [0x00000000,0xFFFFFFFF]
	    public Int32   i32_x;          // x-axis component (pulse), [-2147483648,2147484647]
	    public Int32   i32_y;          // y-axis component (pulse), [-2147483648,2147484647]
	    public Int32   i32_theta;      // x-y plane arc move angle (0.000001 degree), [-360000,360000]
	}


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ASYNCALL
    {
        public void* h_event;
        public Int32 i32_ret;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct TSK_INFO
	{
	    public UInt16 State;      	// 
	    public UInt16 RunTimeErr;     // 
	    public UInt16 IP;
	    public UInt16 SP;
	    public UInt16 BP;
        public UInt16 MsgQueueSts;
	}
		
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SAMP_PARAM
    {
	    public Int32 rate;							//Sampling rate
	    public Int32 edge;							//Trigger edge
	    public Int32 level;							//Trigger level
	    public Int32 trigCh;					    //Trigger channel
        Int32[,] sourceByCh;

        public SAMP_PARAM(Int32 x, Int32 y) : this()
        {
            sourceByCh = new Int32[x, y];
        }
        //Sampling source by channel, named sourceByCh[a][b], 
	      //a: Channel
	      //b: 0: Sampling source 1: Sampling axis
	      //Sampling source: F64 data occupies two channel, I32 data occupies one channel.
    } 
    
    
    //Cognex 20150728
    [StructLayout(LayoutKind.Sequential)]
    public struct COGNEX_POINT
    {
        public Int32 cmdData;   // comparator data
        public UInt32 muxInBit; // bit 0: mux_c; bit 1: mux_en; bit 2: cmp_s
    }

    //**********************************************
    // New header functions; 20151102
    //**********************************************
    [StructLayout(LayoutKind.Sequential)]
    public struct MCMP_POINT
    {
        public Double axisX; // x axis data for multi-dimension comparator 0
        public Double axisY; // y axis data for multi-dimension comparator 1
        public Double axisZ; // z axis data for multi-dimension comparator 2
        public Double axisU; // u axis data for multi-dimension comparator 3
        public UInt32 chInBit; // pwm output channel in bit format; 20150609
    }

	//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++			
	public class APS168
	{
		// System & Initialization
		[DllImport("APS168.dll")]public static extern Int32  APS_initial( ref System.Int32  BoardID_InBits, System.Int32 Mode );
		[DllImport("APS168.dll")]public static extern Int32  APS_close();
		[DllImport("APS168.dll")]public static extern Int32  APS_version();
		[DllImport("APS168.dll")]public static extern Int32  APS_device_driver_version( System.Int32 Board_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_axis_info( System.Int32 Axis_ID, ref System.Int32 Board_ID, ref System.Int32  Axis_No, ref System.Int32 Port_ID, ref System.Int32  Module_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_board_param( System.Int32 Board_ID, System.Int32 BOD_Param_No, System.Int32 BOD_Param );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_board_param( System.Int32 Board_ID, System.Int32 BOD_Param_No, ref System.Int32  BOD_Param );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_axis_param( System.Int32 Axis_ID, System.Int32 AXS_Param_No, System.Int32  AXS_Param );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_axis_param( System.Int32 Axis_ID, System.Int32 AXS_Param_No, ref System.Int32  AXS_Param );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_device_info( System.Int32 Board_ID, System.Int32 Info_No, ref System.Int32  Info );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_card_name( System.Int32 Board_ID, ref System.Int32 CardName );
        [DllImport("APS168.dll")]public static extern Int32  APS_disable_device( System.Int32 DeviceName );
        [DllImport("APS168.dll")]public static extern Int32  APS_load_param_from_file( string pXMLFile );
        [DllImport("APS168.dll")]public static extern Int32 APS_get_first_axisId(System.Int32 Board_ID, ref System.Int32 StartAxisID, ref System.Int32 TotalAxisNum);
        [DllImport("APS168.dll")]public static extern Int32  APS_get_system_timer( System.Int32 Board_ID, ref System.Int32 Timer );
    
        //Control driver mode [Only for PCI-8254/8]
        [DllImport("APS168.dll")]public static extern Int32 APS_get_curr_sys_ctrl_mode( System.Int32 Axis_ID, ref System.Int32 Mode );
    
		//Virtual board settings  [Only for PCI-8254/8]
		[DllImport("APS168.dll")]public static extern Int32 APS_register_virtual_board( System.Int32 VirCardIndex, System.Int32 Count );	
		[DllImport("APS168.dll")]public static extern Int32 APS_get_virtual_board_info( System.Int32 VirCardIndex, ref System.Int32 Count );
		
		//Parameters setting by float [Only for PCI-8254/8]
		[DllImport("APS168.dll")]public static extern Int32 APS_set_axis_param_f( System.Int32 Axis_ID, System.Int32 AXS_Param_No,     System.Double AXS_Param );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_axis_param_f( System.Int32 Axis_ID, System.Int32 AXS_Param_No, ref System.Double AXS_Param );

		//Control driver mode [Only for PCI-8254/8]
		[DllImport("APS168.dll")]public static extern Int32 APS_get_eep_curr_drv_ctrl_mode( System.Int32 Board_ID, ref System.Int32 ModeInBit );

		//Only support PCI-7856/MoionNet series
        [DllImport("APS168.dll")]public static extern Int32 APS_save_param_to_file(System.Int32 Board_ID, string pXMLFile);

		// Monitor functions by float [Only for PCI-8254/8]
		[DllImport("APS168.dll")]public static extern Int32 APS_get_command_f( System.Int32 Axis_ID, ref System.Double Command );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_command_f( System.Int32 Axis_ID, System.Double Command );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_position_f( System.Int32 Axis_ID, ref System.Double Position );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_position_f(System.Int32 Axis_ID, System.Double Position );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_command_velocity_f(System.Int32 Axis_ID, ref System.Double Velocity );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_target_position_f( System.Int32 Axis_ID, ref System.Double Targ_Pos );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_error_position_f( System.Int32 Axis_ID, ref System.Double Err_Pos );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_feedback_velocity_f(System.Int32 Axis_ID, ref System.Double Velocity );

		//Motion queue status [Only for PCI-8254/8]
		[DllImport("APS168.dll")]public static extern Int32 APS_get_mq_free_space( System.Int32 Axis_ID, ref System.Int32 Space );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_mq_usage( System.Int32 Axis_ID, ref System.Int32 Usage );
		
		//Motion Stop Code [Only for PCI-8254/8]
		[DllImport("APS168.dll")]public static extern Int32 APS_get_stop_code( System.Int32 Axis_ID, ref System.Int32 Code );
		
		// Helical interpolation [Only for PCI-8392, PCI-8253/56]
		[DllImport("APS168.dll")]public static extern Int32 APS_absolute_helix_move( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, ref System.Int32 Center_Pos_Array, System.Int32 Max_Arc_Speed, System.Int32 Pitch, System.Int32 TotalHeight, System.Int32 CwOrCcw );
		[DllImport("APS168.dll")]public static extern Int32 APS_relative_helix_move( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, ref System.Int32 Center_PosOffset_Array, System.Int32 Max_Arc_Speed, System.Int32 Pitch, System.Int32 TotalHeight, System.Int32 CwOrCcw );
				
		// Circular interpolation( Support 2D and 3D ) [Only for PCI-8392, PCI-8253/56]
		[DllImport("APS168.dll")]public static extern Int32 APS_absolute_arc_move_3pe(System.Int32 Dimension, ref System.Int32 Axis_ID_Array, ref System.Int32 Pass_Pos_Array, ref System.Int32 End_Pos_Array, System.Int32 Max_Arc_Speed );
		[DllImport("APS168.dll")]public static extern Int32 APS_relative_arc_move_3pe(System.Int32 Dimension, ref System.Int32 Axis_ID_Array, ref System.Int32 Pass_PosOffset_Array, ref System.Int32 End_PosOffset_Array, System.Int32 Max_Arc_Speed );
				
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_int_factor_di( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32  bitsOfCheck );
        [DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_int_factor_di( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 bitsOfCheck );
	
		//[Only for PCI-7856 motion interrupt]
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_int_factor_motion( System.Int32 Axis_ID, System.Int32 Factor_No, System.Int32  Enable );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_int_factor_motion( System.Int32 Axis_ID, System.Int32 Factor_No, ref System.Int32 Enable );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_int_factor_error( System.Int32 Axis_ID, System.Int32 Factor_No, System.Int32  Enable );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_int_factor_error( System.Int32 Axis_ID, System.Int32 Factor_No, ref System.Int32 Enable );
		[DllImport("APS168.dll")]public static extern Int32 APS_reset_field_bus_int_motion( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32 APS_wait_field_bus_error_int_motion( System.Int32 Axis_ID, System.Int32 Time_Out );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_int_factorH( System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, System.Int32 Enable );
        [DllImport("APS168.dll")]public static extern Int32 APS_int_no_to_handle( System.Int32 Int_No );
				
		// Flash function [Only for PCI-8253/56, PCI-8392(H)]
		[DllImport("APS168.dll")]public static extern Int32  APS_save_parameter_to_flash( System.Int32 Board_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_load_parameter_from_flash( System.Int32 Board_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_load_parameter_from_default( System.Int32 Board_ID );
		
		// SSCNET-3 functions [Only for PCI-8392(H)] 
		[DllImport("APS168.dll")]public static extern Int32  APS_start_sscnet( System.Int32 Board_ID, ref System.Int32  AxisFound_InBits );
		[DllImport("APS168.dll")]public static extern Int32  APS_stop_sscnet( System.Int32 Board_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_sscnet_servo_param( System.Int32 Axis_ID, System.Int32 Para_No1, ref System.Int32  Para_Dat1, System.Int32 Para_No2, ref System.Int32  Para_Dat2 );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_sscnet_servo_param( System.Int32 Axis_ID, System.Int32 Para_No1, System.Int32 Para_Dat1, System.Int32 Para_No2, System.Int32 Para_Dat2 );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_sscnet_servo_alarm( System.Int32 Axis_ID, ref System.Int32  Alarm_No, ref System.Int32  Alarm_Detail );
		[DllImport("APS168.dll")]public static extern Int32  APS_reset_sscnet_servo_alarm( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_save_sscnet_servo_param( System.Int32 Board_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_sscnet_servo_abs_position( System.Int32 Axis_ID, ref System.Int32  Cyc_Cnt, ref System.Int32  Res_Cnt );
		[DllImport("APS168.dll")]public static extern Int32  APS_save_sscnet_servo_abs_position( System.Int32 Board_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_load_sscnet_servo_abs_position( System.Int32 Axis_ID, System.Int32 Abs_Option, ref System.Int32  Cyc_Cnt, ref System.Int32  Res_Cnt );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_sscnet_link_status( System.Int32 Board_ID, ref System.Int32  Link_Status );
        [DllImport("APS168.dll")]public static extern Int32  APS_set_sscnet_servo_monitor_src( System.Int32 Axis_ID, System.Int32 Mon_No, System.Int32 Mon_Src );
        [DllImport("APS168.dll")]public static extern Int32  APS_get_sscnet_servo_monitor_src( System.Int32 Axis_ID, System.Int32 Mon_No, ref System.Int32 Mon_Src );
        [DllImport("APS168.dll")]public static extern Int32  APS_get_sscnet_servo_monitor_data( System.Int32 Axis_ID, System.Int32 Arr_Size, ref System.Int32 Data_Arr );
        [DllImport("APS168.dll")]public static extern Int32  APS_set_sscnet_control_mode( System.Int32 Axis_ID, System.Int32 Mode );

		// Motion IO & motion status
		[DllImport("APS168.dll")]public static extern Int32  APS_get_command( System.Int32 Axis_ID, ref System.Int32  Command );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_command(System.Int32 Axis_ID, System.Int32 Command);
		[DllImport("APS168.dll")]public static extern Int32  APS_motion_status( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_motion_io_status( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_servo_on( System.Int32 Axis_ID, System.Int32 Servo_On );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_position( System.Int32 Axis_ID, ref System.Int32  Position );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_position(System.Int32 Axis_ID, System.Int32 Position);
		[DllImport("APS168.dll")]public static extern Int32  APS_get_command_velocity(System.Int32 Axis_ID, ref System.Int32  Velocity );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_feedback_velocity(System.Int32 Axis_ID, ref System.Int32  Velocity );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_error_position( System.Int32 Axis_ID, ref System.Int32  Err_Pos );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_target_position( System.Int32 Axis_ID, ref System.Int32 Targ_Pos );
		
		// Single axis motion
		[DllImport("APS168.dll")]public static extern Int32  APS_relative_move( System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed );
		[DllImport("APS168.dll")]public static extern Int32  APS_absolute_move( System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed );
		[DllImport("APS168.dll")]public static extern Int32  APS_velocity_move( System.Int32 Axis_ID, System.Int32 Max_Speed );
		[DllImport("APS168.dll")]public static extern Int32  APS_home_move( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_stop_move( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_emg_stop( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_relative_move2( System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 End_Speed, System.Int32 Acc_Rate, System.Int32 Dec_Rate );
		[DllImport("APS168.dll")]public static extern Int32  APS_absolute_move2( System.Int32 Axis_ID, System.Int32 Position, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 End_Speed, System.Int32 Acc_Rate, System.Int32 Dec_Rate );
		[DllImport("APS168.dll")]public static extern Int32  APS_home_move2( System.Int32 Axis_ID, System.Int32 Dir, System.Int32 Acc, System.Int32 Start_Speed, System.Int32 Max_Speed, System.Int32 ORG_Speed );
		[DllImport("APS168.dll")]public static extern Int32  APS_home_escape( System.Int32 Axis_ID );
		
		//JOG functions [Only for PCI-8392, PCI-8253/56]
		[DllImport("APS168.dll")]public static extern Int32  APS_set_jog_param( System.Int32 Axis_ID, ref JOG_DATA pStr_Jog, System.Int32 Mask );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_jog_param( System.Int32 Axis_ID,ref JOG_DATA pStr_Jog );
		[DllImport("APS168.dll")]public static extern Int32  APS_jog_mode_switch( System.Int32 Axis_ID, System.Int32 Turn_No );
		[DllImport("APS168.dll")]public static extern Int32  APS_jog_start( System.Int32 Axis_ID, System.Int32 STA_On );
		
		// Interpolation
        [DllImport("APS168.dll")]public static extern Int32 APS_absolute_linear_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Position_Array, System.Int32 Max_Linear_Speed);
        [DllImport("APS168.dll")]public static extern Int32 APS_relative_linear_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Distance_Array, System.Int32 Max_Linear_Speed);
        [DllImport("APS168.dll")]public static extern Int32 APS_absolute_arc_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Pos_Array, System.Int32 Max_Arc_Speed, System.Int32 Angle);
        [DllImport("APS168.dll")]public static extern Int32 APS_relative_arc_move(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32[] Center_Offset_Array, System.Int32 Max_Arc_Speed, System.Int32 Angle);
		
		// Interrupt functions
		[DllImport("APS168.dll")]public static extern Int32  APS_int_enable( System.Int32 Board_ID, System.Int32 Enable );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_int_factor( System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, System.Int32 Enable );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_int_factor( System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Factor_No, ref System.Int32 Enable );
			
		[DllImport("APS168.dll")]public static extern Int32  APS_wait_single_int( System.Int32 Int_No, System.Int32 Time_Out );
		[DllImport("APS168.dll")]public static extern Int32  APS_wait_multiple_int( System.Int32 Int_Count, ref System.Int32 Int_No_Array, System.Int32 Wait_All, System.Int32 Time_Out );
		[DllImport("APS168.dll")]public static extern Int32  APS_reset_int( System.Int32 Int_No );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_int( System.Int32 Int_No );
		
		//[Only for PCI-8154/58]
		[DllImport("APS168.dll")]public static extern Int32  APS_wait_error_int( System.Int32 Board_ID, System.Int32 Item_No, System.Int32 Time_Out );
		
		// Sampling functions [Only for PCI-8392, PCI-8253/56]
		[DllImport("APS168.dll")]public static extern Int32  APS_set_sampling_param( System.Int32 Board_ID, System.Int32 ParaNum, System.Int32 ParaDat );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_sampling_param( System.Int32 Board_ID, System.Int32 ParaNum, ref System.Int32 ParaDat );
		[DllImport("APS168.dll")]public static extern Int32  APS_wait_trigger_sampling( System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_4CH DataArr );
		[DllImport("APS168.dll")]public static extern Int32  APS_wait_trigger_sampling_async( System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_4CH DataArr );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_sampling_count( System.Int32 Board_ID, ref System.Int32 SampCnt );
		[DllImport("APS168.dll")]public static extern Int32  APS_stop_wait_sampling( System.Int32 Board_ID );
		[DllImport("APS168.dll")]public static extern Int32 APS_auto_sampling( System.Int32 Board_ID, System.Int32 StartStop );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_sampling_data( System.Int32 Board_ID, ref System.Int32 Length, ref STR_SAMP_DATA_4CH DataArr, ref System.Int32 Status );
		
		// Sampling functions extension[Only for PCI-82548 for up to 8 channel]
		[DllImport("APS168.dll")]public static extern Int32 APS_set_sampling_param_ex( System.Int32 Board_ID, ref SAMP_PARAM Param );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_sampling_param_ex( System.Int32 Board_ID, ref SAMP_PARAM Param );
		[DllImport("APS168.dll")]public static extern Int32 APS_wait_trigger_sampling_ex( System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_8CH DataArr );
		[DllImport("APS168.dll")]public static extern Int32 APS_wait_trigger_sampling_async_ex( System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_8CH DataArr );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_sampling_data_ex( System.Int32 Board_ID, ref System.Int32 Length, ref STR_SAMP_DATA_8CH DataArr, ref System.Int32 Status );
		
				
		//PCI-82548 Only for channel I/O
		[DllImport("APS168.dll")]public static extern Int32 APS_write_d_channel_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 Ch_No, System.Int32 DO_Data );
		[DllImport("APS168.dll")]public static extern Int32 APS_read_d_channel_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 Ch_No, ref System.Int32 DO_Data );
		[DllImport("APS168.dll")]public static extern Int32 APS_read_d_channel_input(System.Int32 Board_ID, System.Int32 DI_Group, System.Int32 Ch_No, ref System.Int32 DI_Data );
				
		//New AIO [Only for PCI-82548]
        [DllImport("APS168.dll")]public static extern Int32 APS_read_a_output_value(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Double Convert_Data);
				
		//DIO & AIO
		[DllImport("APS168.dll")]public static extern Int32  APS_write_d_output(System.Int32 Board_ID, System.Int32 DO_Group, System.Int32 DO_Data);
		[DllImport("APS168.dll")]public static extern Int32  APS_read_d_output(System.Int32 Board_ID, System.Int32 DO_Group, ref System.Int32 DO_Data);
		[DllImport("APS168.dll")]public static extern Int32  APS_read_d_input(System.Int32 Board_ID, System.Int32 DI_Group, ref System.Int32 DI_Data);
		
		[DllImport("APS168.dll")]public static extern Int32  APS_read_a_input_value(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Double Convert_Data);
		[DllImport("APS168.dll")]public static extern Int32  APS_read_a_input_data(System.Int32 Board_ID, System.Int32 Channel_No, ref System.Int32 Raw_Data);
		[DllImport("APS168.dll")]public static extern Int32  APS_write_a_output_value(System.Int32 Board_ID, System.Int32 Channel_No, System.Double  Convert_Data);
		[DllImport("APS168.dll")]public static extern Int32  APS_write_a_output_data(System.Int32 Board_ID, System.Int32 Channel_No, System.Int32 Raw_Data);
		
		//Point table move
		[DllImport("APS168.dll")]public static extern Int32  APS_set_point_table( System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA Point );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_point_table( System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA Point );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_running_point_index( System.Int32 Axis_ID, ref System.Int32 Index );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_start_point_index( System.Int32 Axis_ID, ref System.Int32 Index );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_end_point_index( System.Int32 Axis_ID, ref System.Int32 Index );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_table_move_pause( System.Int32 Axis_ID, System.Int32 Pause_en );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_table_move_repeat( System.Int32 Axis_ID, System.Int32 Repeat_en );
		[DllImport("APS168.dll")]public static extern Int32  APS_point_table_move( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, System.Int32 StartIndex, System.Int32 EndIndex );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_point_tableEx( System.Int32 Axis_ID, System.Int32 Index, ref PNT_DATA Point );
        [DllImport("APS168.dll")]public static extern Int32 APS_set_point_tableEx_2D( System.Int32 Axis_ID, System.Int32 Axis_ID_2, System.Int32 Index, ref PNT_DATA_2D Point );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_point_table_4DL( ref System.Int32 Axis_ID_Array, System.Int32 Index, ref PNT_DATA_4DL Point );
		
		//Point table + IO - Pause / Resume
		[DllImport("APS168.dll")]public static extern Int32 APS_set_table_move_ex_pause( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_table_move_ex_rollback( System.Int32 Axis_ID, System.Int32 Max_Speed );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_table_move_ex_resume( System.Int32 Axis_ID );
				
		//Only for PCI-8392 to replace APS_set_point_table & APS_get_point_table
		[DllImport("APS168.dll")]public static extern Int32 APS_set_point_table_ex( System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA_EX Point );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_point_table_ex( System.Int32 Axis_ID, System.Int32 Index, ref POINT_DATA_EX Point );
				
		//Point table Feeder (Only for PCI-825x)
		[DllImport("APS168.dll")]public static extern Int32 APS_set_feeder_group( System.Int32 GroupId, System.Int32 Dimension, ref System.Int32 Axis_ID_Array );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_feeder_group( System.Int32 GroupId, ref System.Int32 Dimension, ref System.Int32 Axis_ID_Array );
		[DllImport("APS168.dll")]public static extern Int32 APS_free_feeder_group( System.Int32 GroupId );
		[DllImport("APS168.dll")]public static extern Int32 APS_reset_feeder_buffer( System.Int32 GroupId );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_feeder_point_2D( System.Int32 GroupId, ref PNT_DATA_2D PtArray, System.Int32 Size, System.Int32 LastFlag );
        [DllImport("APS168.dll")]public static extern Int32 APS_set_feeder_point_2D_ex(System.Int32 GroupId, IntPtr PtArray, System.Int32 Size, System.Int32 LastFlag);
		[DllImport("APS168.dll")]public static extern Int32 APS_start_feeder_move( System.Int32 GroupId );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_feeder_status( System.Int32 GroupId, ref System.Int32 State, ref System.Int32 ErrCode );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_feeder_running_index( System.Int32 GroupId, ref System.Int32 Index );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_feeder_feed_index( System.Int32 GroupId, ref System.Int32 Index );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_feeder_ex_pause( System.Int32 GroupId );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_feeder_ex_rollback( System.Int32 GroupId, System.Int32 Max_Speed );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_feeder_ex_resume( System.Int32 GroupId );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_feeder_cfg_acc_type( System.Int32 GroupId, System.Int32 Type );
		
		//Point table move2
		[DllImport("APS168.dll")]public static extern Int32 APS_set_point_table_mode2( System.Int32 Axis_ID, System.Int32 Mode );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_point_table2( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, System.Int32 Index, ref POINT_DATA2 Point );
		[DllImport("APS168.dll")]public static extern Int32 APS_point_table_continuous_move2( System.Int32 Dimension, ref System.Int32 Axis_ID_Array );
		[DllImport("APS168.dll")]public static extern Int32 APS_point_table_single_move2( System.Int32 Axis_ID, System.Int32 Index );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_running_point_index2( System.Int32 Axis_ID, ref System.Int32 Index );
		[DllImport("APS168.dll")]public static extern Int32 APS_point_table_status2( System.Int32 Axis_ID, ref System.Int32 Status );
				
		//Point table Only for HSL-4XMO
		[DllImport("APS168.dll")]public static extern Int32 APS_set_point_table3( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, System.Int32 Index, ref POINT_DATA3 Point );
		[DllImport("APS168.dll")]public static extern Int32 APS_point_table_move3( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, System.Int32 StartIndex, System.Int32 EndIndex );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_point_table_param3( System.Int32 FirstAxid, System.Int32 ParaNum, System.Int32 ParaDat );
		
		// Digital filter functions. [Only for PCI-8253/56]
		[DllImport("APS168.dll")]public static extern Int32 APS_set_filter_param( System.Int32 Axis_ID, System.Int32 Filter_paramNo, System.Int32 param_val );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_filter_param( System.Int32 Axis_ID, System.Int32 Filter_paramNo, ref System.Int32 param_val );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_device_info( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Info_No, ref System.Int32 Info );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_slave_first_axisno( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 AxisNo, ref System.Int32 TotalAxes );
				
		//Field bus DIO slave fucntions For PCI-8392(H)
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_d_channel_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32  DO_Value );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_d_channel_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Int32 DO_Value );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_d_channel_input(  System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Int32 DI_Value );
				
		//Field bus AIO slave function
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_a_output_plc( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value, System.Int16 RunStep );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_a_input_plc( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value, System.Int16 RunStep );
				
		//Field bus Comparing trigger functions
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_trigger_param( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Param_No, System.Int32 Param_Val );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_trigger_param( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Param_No, ref System.Int32 Param_Val );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_trigger_linear( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, System.Int32 StartPoint, System.Int32 RepeatTimes, System.Int32 Interval );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_trigger_table( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, ref System.Int32 DataArr, System.Int32 ArraySize ); 
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_trigger_manual( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_trigger_manual_s( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgChInBit );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_trigger_table_cmp( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, ref System.Int32 CmpVal );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_trigger_linear_cmp( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, ref System.Int32 CmpVal );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_trigger_count( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh, ref System.Int32 TrgCnt );
		[DllImport("APS168.dll")]public static extern Int32 APS_reset_field_bus_trigger_count( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TrgCh );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_linear_cmp_remain_count( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 LCmpCh, ref System.Int32 Cnt );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_table_cmp_remain_count( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 TCmpCh, ref System.Int32 Cnt );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_field_bus_encoder( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 EncCh, ref System.Int32 EncCnt );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_field_bus_encoder( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 EncCh, System.Int32 EncCnt );
				
		// Comparing trigger functions
		[DllImport("APS168.dll")]public static extern Int32 APS_reset_trigger_count( System.Int32 Board_ID, System.Int32 TrgCh );
		[DllImport("APS168.dll")]public static extern Int32 APS_enable_trigger_fifo_cmp( System.Int32 Board_ID, System.Int32 FCmpCh, System.Int32 Enable );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_trigger_fifo_cmp( System.Int32 Board_ID, System.Int32 FCmpCh, ref System.Int32 CmpVal );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_trigger_fifo_status( System.Int32 Board_ID, System.Int32 FCmpCh, ref System.Int32 FifoSts );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_trigger_fifo_data( System.Int32 Board_ID, System.Int32 FCmpCh, ref System.Int32 DataArr, System.Int32 ArraySize, System.Int32 ShiftFlag ); 
		[DllImport("APS168.dll")]public static extern Int32 APS_set_trigger_encoder_counter( System.Int32 Board_ID, System.Int32 TrgCh, System.Int32 TrgCnt );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_trigger_encoder_counter( System.Int32 Board_ID, System.Int32 TrgCh, ref System.Int32 TrgCnt );
		[DllImport("APS168.dll")]public static extern Int32 APS_start_timer( System.Int32 Board_ID, System.Int32 TrgCh, System.Int32 Start );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_timer_counter( System.Int32 Board_ID, System.Int32 TmrCh, ref System.Int32 Cnt );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_timer_counter( System.Int32 Board_ID, System.Int32 TmrCh, System.Int32 Cnt );
		[DllImport("APS168.dll")]public static extern Int32 APS_field_bus_A_set_output_plc( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value, System.Int16 RunStep );
		[DllImport("APS168.dll")]public static extern Int32 APS_field_bus_A_get_input_plc( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value, System.Int16 RunStep );
	
		//VAO function(Laser function) [Only for PCI-8253/6]
		[DllImport("APS168.dll")]public static extern Int32 APS_set_vao_param( System.Int32 Board_ID, System.Int32 Param_No, System.Int32 Param_Val );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_vao_param( System.Int32 Board_ID, System.Int32 Param_No, ref System.Int32 Param_Val );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_vao_table( System.Int32 Board_ID, System.Int32 Table_No, System.Int32 MinVelocity, System.Int32 VelInterval, System.Int32 TotalPoints,  System.Int32[] MappingDataArray );
		[DllImport("APS168.dll")]public static extern Int32 APS_switch_vao_table( System.Int32 Board_ID, System.Int32 Table_No );
		[DllImport("APS168.dll")]public static extern Int32 APS_start_vao( System.Int32 Board_ID, System.Int32 Output_Ch, System.Int32 Enable );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_vao_status( System.Int32 Board_ID, ref System.Int32 Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_check_vao_param( System.Int32 Board_ID, System.Int32 Table_No, ref System.Int32 Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_vao_param_ex( System.Int32 Board_ID, System.Int32 Table_No, ref VAO_DATA VaoData );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_vao_param_ex( System.Int32 Board_ID, System.Int32 Table_No, ref VAO_DATA VaoData );
					
		//Simultaneous move [Only for MNET series and 8392]
		[DllImport("APS168.dll")]public static extern Int32 APS_set_relative_simultaneous_move  ( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, ref System.Int32 Distance_Array, ref System.Int32 Max_Speed_Array );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_absolute_simultaneous_move  ( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, ref System.Int32 Position_Array, ref System.Int32 Max_Speed_Array );
		[DllImport("APS168.dll")]public static extern Int32 APS_start_simultaneous_move         ( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32 APS_stop_simultaneous_move          ( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_velocity_simultaneous_move  ( System.Int32 Dimension, ref System.Int32 Axis_ID_Array, ref System.Int32 Max_Speed_Array ); 
		[DllImport("APS168.dll")]public static extern Int32 APS_Release_simultaneous_move       ( System.Int32 Axis_ID ); 
		[DllImport("APS168.dll")]public static extern Int32 APS_release_simultaneous_move       ( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32 APS_emg_stop_simultaneous_move      ( System.Int32 Axis_ID ); 		

		//Override functions [Only for MNET series]
	    [DllImport("APS168.dll")]public static extern Int32 APS_speed_override( System.Int32 Axis_ID, System.Int32 MaxSpeed );
				
		//Only for MNET-1XMO/MNET-4XMO
		[DllImport("APS168.dll")]public static extern Int32 APS_relative_move_ovrd( System.Int32 Axis_ID, System.Int32 Distance, System.Int32 Max_Speed );
		[DllImport("APS168.dll")]public static extern Int32 APS_absolute_move_ovrd( System.Int32 Axis_ID, System.Int32 Position, System.Int32 Max_Speed );
	
		//Point table Feeder (Only for PCI-8254/8)
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_dwell( System.Int32 Board_ID, System.Int32 PtbId,  ref PTDWL Prof,  ref PTSTS Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_line( System.Int32 Board_ID, System.Int32 PtbId,  ref PTLINE Prof, ref PTSTS Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_arc2_ca( System.Int32 Board_ID, System.Int32 PtbId, ref PTA2CA Prof, ref PTSTS Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_arc2_ce( System.Int32 Board_ID, System.Int32 PtbId, ref PTA2CE Prof, ref PTSTS Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_arc3_ca( System.Int32 Board_ID, System.Int32 PtbId, ref PTA3CA Prof, ref PTSTS Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_arc3_ce( System.Int32 Board_ID, System.Int32 PtbId, ref PTA3CE Prof, ref PTSTS Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_spiral_ca( System.Int32 Board_ID, System.Int32 PtbId, ref PTHCA Prof, ref PTSTS Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_spiral_ce( System.Int32 Board_ID, System.Int32 PtbId, ref PTHCE Prof, ref PTSTS Status );
		
		//enable & disable
        [DllImport("APS168.dll")]public static extern Int32 APS_pt_enable(System.Int32 Board_ID, System.Int32 PtbId, System.Int32 Dimension, System.Int32[] AxisArr);
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_disable( System.Int32 Board_ID, System.Int32 PtbId );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_pt_info( System.Int32 Board_ID, System.Int32 PtbId, ref PTINFO Info );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_vs( System.Int32 Board_ID, System.Int32 PtbId, System.Double Vs );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_get_vs( System.Int32 Board_ID, System.Int32 PtbId, ref System.Double Vs );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_start( System.Int32 Board_ID, System.Int32 PtbId );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_stop( System.Int32 Board_ID, System.Int32 PtbId );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_pt_status( System.Int32 Board_ID, System.Int32 PtbId, ref PTSTS Status );
		[DllImport("APS168.dll")]public static extern Int32 APS_reset_pt_buffer( System.Int32 Board_ID, System.Int32 PtbId );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_roll_back( System.Int32 Board_ID, System.Int32 PtbId, System.Double Max_Speed );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_get_error( System.Int32 Board_ID, System.Int32 PtbId, ref System.Int32 ErrCode );
		
		//Cmd buffer setting
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_ext_set_do_ch( System.Int32 Board_ID, System.Int32 PtbId, System.Int32 Channel, System.Int32 OnOff );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_ext_set_table_no( System.Int32 Board_ID, System.Int32 PtbId, System.Int32 CtrlNo, System.Int32 TableNo );
				
		//Profile buffer setting
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_absolute( System.Int32 Board_ID, System.Int32 PtbId );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_relative( System.Int32 Board_ID, System.Int32 PtbId );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_trans_buffered( System.Int32 Board_ID, System.Int32 PtbId );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_trans_inp( System.Int32 Board_ID, System.Int32 PtbId );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_trans_blend_dec( System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_trans_blend_dist( System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_trans_blend_pcnt( System.Int32 Board_ID, System.Int32 PtbId, System.Double Bp );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_acc( System.Int32 Board_ID, System.Int32 PtbId, System.Double Acc );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_dec( System.Int32 Board_ID, System.Int32 PtbId, System.Double Dec );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_acc_dec( System.Int32 Board_ID, System.Int32 PtbId, System.Double AccDec );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_s( System.Int32 Board_ID, System.Int32 PtbId, System.Double Sf );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_vm( System.Int32 Board_ID, System.Int32 PtbId, System.Double Vm );
		[DllImport("APS168.dll")]public static extern Int32 APS_pt_set_ve( System.Int32 Board_ID, System.Int32 PtbId, System.Double Ve );
		
		//Program download - APS
		[DllImport("APS168.dll")]public static extern Int32 APS_load_vmc_program( System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
		[DllImport("APS168.dll")]public static extern Int32 APS_save_vmc_program( System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
		[DllImport("APS168.dll")]public static extern Int32 APS_load_amc_program( System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
		[DllImport("APS168.dll")]public static extern Int32 APS_save_amc_program( System.Int32 Board_ID, System.Int32 TaskNum, string pFile, System.Int32 Password);
		
		[DllImport("APS168.dll")]public static extern Int32 APS_set_task_mode( System.Int32 Board_ID, System.Int32 TaskNum, System.Byte Mode, System.UInt16 LastIP );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_task_mode( System.Int32 Board_ID, System.Int32 TaskNum, ref System.Byte Mode, ref System.UInt16 LastIP );
		[DllImport("APS168.dll")]public static extern Int32 APS_start_task( System.Int32 Board_ID, System.Int32 TaskNum, System.Int32 CtrlCmd );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_task_info( System.Int32 Board_ID, System.Int32 TaskNum, ref TSK_INFO Info );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_task_msg( System.Int32 Board_ID, System.UInt16 QueueSts, ref System.UInt16 ActualSize, ref System.Byte CharArr );
				
		//Latch functins
		[DllImport("APS168.dll")]public static extern Int32 APS_get_encoder( System.Int32 Axis_ID, ref System.Int32 Encoder );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_latch_counter( System.Int32 Axis_ID, System.Int32 Src, ref System.Int32 Counter );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_latch_event( System.Int32 Axis_ID, System.Int32 Src, ref System.Int32 Event );
						
		//Raw command counter [Only for PCI-8254/8]
		[DllImport("APS168.dll")]public static extern Int32 APS_get_command_counter( System.Int32 Axis_ID, ref System.Int32 Counter );
		
		//Watch dog timer
		[DllImport("APS168.dll")]public static extern Int32 APS_wdt_start( System.Int32 Board_ID, System.Int32 TimerNo, System.Int32 TimeOut );
		[DllImport("APS168.dll")]public static extern Int32 APS_wdt_get_timeout_period( System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 TimeOut );
		[DllImport("APS168.dll")]public static extern Int32 APS_wdt_reset_counter( System.Int32 Board_ID, System.Int32 TimerNo );
		[DllImport("APS168.dll")]public static extern Int32 APS_wdt_get_counter( System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 Counter );
		[DllImport("APS168.dll")]public static extern Int32 APS_wdt_set_action_event( System.Int32 Board_ID, System.Int32 TimerNo, System.Int32 EventByBit );
		[DllImport("APS168.dll")]public static extern Int32 APS_wdt_get_action_event( System.Int32 Board_ID, System.Int32 TimerNo, ref System.Int32 EventByBit );
				
		//Multi-axes simultaneuos move start/stop
		[DllImport("APS168.dll")]public static extern Int32 APS_move_trigger( System.Int32 Dimension, ref System.Int32 Axis_ID_Array );
		[DllImport("APS168.dll")]public static extern Int32 APS_stop_move_multi( System.Int32 Dimension, ref System.Int32 Axis_ID_Array );
		[DllImport("APS168.dll")]public static extern Int32 APS_emg_stop_multi( System.Int32 Dimension, ref System.Int32 Axis_ID_Array );
				
		//Gear/Gantry function
		[DllImport("APS168.dll")]public static extern Int32 APS_start_gear( System.Int32 Axis_ID, System.Int32 Mode );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_gear_status( System.Int32 Axis_ID, ref System.Int32 Status );
				
		//Latch Function: for latching multi-points
		[DllImport("APS168.dll")]public static extern Int32 APS_set_ltc_counter( System.Int32 Board_ID, System.Int32 CntNum, System.Int32 CntValue );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_ltc_counter( System.Int32 Board_ID, System.Int32 CntNum, ref System.Int32 CntValue );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_ltc_fifo_param( System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Param_No, System.Int32 Param_Val );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_ltc_fifo_param( System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Param_No, ref System.Int32 Param_Val );
		[DllImport("APS168.dll")]public static extern Int32 APS_manual_latch( System.Int32 Board_ID, System.Int32 LatchSignalInBits );
		[DllImport("APS168.dll")]public static extern Int32 APS_enable_ltc_fifo( System.Int32 Board_ID, System.Int32 FLtcCh, System.Int32 Enable );
		[DllImport("APS168.dll")]public static extern Int32 APS_reset_ltc_fifo( System.Int32 Board_ID, System.Int32 FLtcCh );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_ltc_fifo_data( System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Data );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_ltc_fifo_usage( System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Usage );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_ltc_fifo_free_space( System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 FreeSpace );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_ltc_fifo_status( System.Int32 Board_ID, System.Int32 FLtcCh, ref System.Int32 Status );
		
		//For Single latch for PCI8154/58/MNET-4XMO-(C)
		[DllImport("APS168.dll")]public static extern Int32 APS_manual_latch2( System.Int32 Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_latch_data2( System.Int32 Axis_ID, System.Int32 LatchNum, ref System.Int32 LatchData );
		[DllImport("APS168.dll")]public static extern Int32 APS_set_backlash_en(System.Int32 Axis_ID, System.Int32 Enable );
		[DllImport("APS168.dll")]public static extern Int32 APS_get_backlash_en( System.Int32 Axis_ID, ref System.Int32 Enable );
		
		// Gantry functions. [Only for PCI-8392, PCI-8253/56]
		[DllImport("APS168.dll")]public static extern Int32  APS_set_gantry_param( System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 ParaNum, System.Int32 ParaDat );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_gantry_param( System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 ParaNum, ref System.Int32 ParaDat );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_gantry_axis( System.Int32 Board_ID, System.Int32 GroupNum, System.Int32 Master_Axis_ID, System.Int32 Slave_Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_gantry_axis( System.Int32 Board_ID, System.Int32 GroupNum, ref System.Int32 Master_Axis_ID, ref System.Int32 Slave_Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_gantry_error( System.Int32 Board_ID, System.Int32 GroupNum, ref System.Int32 GentryError );
		
		//Field bus master fucntions For PCI-8392(H)
		[DllImport("APS168.dll")]public static extern Int32  APS_set_field_bus_param( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 BUS_Param_No, System.Int32  BUS_Param );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_param( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 BUS_Param_No, ref System.Int32 BUS_Param );
		[DllImport("APS168.dll")]public static extern Int32  APS_start_field_bus( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 Start_Axis_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_stop_field_bus( System.Int32 Board_ID, System.Int32 BUS_No );
		
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_last_scan_info( System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 Info_Array, System.Int32 Array_Size, ref System.Int32 Info_Count );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_master_type( System.Int32 Board_ID, System.Int32 BUS_No, ref System.Int32 BUS_Type );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_slave_type( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 MOD_Type );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_slave_name( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 MOD_Name );
	
		//Field bus slave general functions
		[DllImport("APS168.dll")]public static extern Int32  APS_set_field_bus_slave_param( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, System.Int32 ParaDat  );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_slave_param( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, ref System.Int32 ParaDat );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_slave_connect_quality( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 Sts_data );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_slave_online_status( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 Live );

		//Field bus DIO slave fucntions For PCI-8392(H)
		[DllImport("APS168.dll")]public static extern Int32  APS_set_field_bus_d_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 DO_Value );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_d_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DO_Value );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_d_input( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DI_Value );
		
		//Field bus AIO slave function
		[DllImport("APS168.dll")]public static extern Int32  APS_set_field_bus_a_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double AO_Value );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_a_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AO_Value );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_a_input( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value );

        //PWM function
        [DllImport("APS168.dll")]public static extern Int32 APS_set_pwm_width(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 Width);
        [DllImport("APS168.dll")]public static extern Int32 APS_get_pwm_width(System.Int32 Board_ID, System.Int32 PWM_Ch, ref System.Int32 Width);
        [DllImport("APS168.dll")]public static extern Int32 APS_set_pwm_frequency(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 Frequency);
        [DllImport("APS168.dll")]public static extern Int32 APS_get_pwm_frequency(System.Int32 Board_ID, System.Int32 PWM_Ch, ref System.Int32 Frequency);
        [DllImport("APS168.dll")]public static extern Int32 APS_set_pwm_on(System.Int32 Board_ID, System.Int32 PWM_Ch, System.Int32 PWM_On);
		
		// Comparing trigger functions
		[DllImport("APS168.dll")]public static extern Int32  APS_set_trigger_param( System.Int32 Board_ID, System.Int32 Param_No, System.Int32 Param_Val );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_trigger_param( System.Int32 Board_ID, System.Int32 Param_No, ref System.Int32 Param_Val );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_trigger_linear( System.Int32 Board_ID, System.Int32 LCmpCh, System.Int32 StartPoint, System.Int32 RepeatTimes, System.Int32 Interval );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_trigger_table( System.Int32 Board_ID, System.Int32 TCmpCh,  System.Int32[] DataArr, System.Int32 ArraySize ); 
		[DllImport("APS168.dll")]public static extern Int32  APS_set_trigger_manual( System.Int32 Board_ID, System.Int32 TrgCh );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_trigger_manual_s( System.Int32 Board_ID, System.Int32 TrgChInBit );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_trigger_table_cmp( System.Int32 Board_ID, System.Int32 TCmpCh, ref System.Int32 CmpVal );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_trigger_linear_cmp( System.Int32 Board_ID, System.Int32 LCmpCh, ref System.Int32 CmpVal );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_trigger_count( System.Int32 Board_ID, System.Int32 TrgCh, ref System.Int32 TrgCnt );
	
		// Pulser counter function
		[DllImport("APS168.dll")]public static extern Int32  APS_get_pulser_counter( System.Int32 Board_ID, ref System.Int32 Counter );
		[DllImport("APS168.dll")]public static extern Int32  APS_set_pulser_counter( System.Int32 Board_ID, System.Int32 Counter );
		
		// Reserved functions
		[DllImport("APS168.dll")]public static extern Int32  APS_field_bus_slave_set_param( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, System.Int32 ParaDat  );
		[DllImport("APS168.dll")]public static extern Int32  APS_field_bus_slave_get_param( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Int32 ParaNum, ref System.Int32 ParaDat );
	
		[DllImport("APS168.dll")]public static extern Int32  APS_field_bus_d_set_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 DO_Value );
		[DllImport("APS168.dll")]public static extern Int32  APS_field_bus_d_get_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DO_Value );
		[DllImport("APS168.dll")]public static extern Int32  APS_field_bus_d_get_input( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 DI_Value );
		
		[DllImport("APS168.dll")]public static extern Int32  APS_field_bus_A_set_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, System.Double   AO_Value );
		[DllImport("APS168.dll")]public static extern Int32  APS_field_bus_A_get_output( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AO_Value );
		[DllImport("APS168.dll")]public static extern Int32  APS_field_bus_A_get_input( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, System.Int32 Ch_No, ref System.Double AI_Value );
		
		//Dpac Function
		[DllImport("APS168.dll")]public static extern Int32  APS_rescan_CF( System.Int32 Board_ID );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_battery_status( System.Int32 Board_ID, ref System.Int32 Battery_status);
		
		//DPAC Display & Display Button
		[DllImport("APS168.dll")]public static extern Int32  APS_get_display_data( System.Int32 Board_ID, System.Int32 displayDigit, ref System.Int32 displayIndex);
		[DllImport("APS168.dll")]public static extern Int32  APS_set_display_data( System.Int32 Board_ID, System.Int32 displayDigit, System.Int32 displayIndex);
		[DllImport("APS168.dll")]public static extern Int32  APS_get_button_status( System.Int32 Board_ID, ref System.Int32 buttonstatus);
		
		//nv RAM funciton
		[DllImport("APS168.dll")]public static extern Int32  APS_set_nv_ram( System.Int32 Board_ID, System.Int32 RamNo, System.Int32 DataWidth, System.Int32 Offset, System.Int32 Data );
		[DllImport("APS168.dll")]public static extern Int32  APS_get_nv_ram( System.Int32 Board_ID, System.Int32 RamNo, System.Int32 DataWidth, System.Int32 Offset, ref System.Int32 Data );
		[DllImport("APS168.dll")]public static extern Int32  APS_clear_nv_ram( System.Int32 Board_ID, System.Int32 RamNo );

        //New Interface
        [DllImport("APS168.dll")]public static extern Int32 APS_ptp( System.Int32 Axis_ID, System.Int32 Option, System.Double Position, ref ASYNCALL Wait );
        [DllImport("APS168.dll")]public static extern Int32 APS_ptp_v( System.Int32 Axis_ID, System.Int32 Option, System.Double Position, System.Double Vm, ref ASYNCALL Wait );
        [DllImport("APS168.dll")]public static extern Int32 APS_ptp_all( System.Int32 Axis_ID, System.Int32 Option, System.Double Position, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait );
        [DllImport("APS168.dll")]public static extern Int32 APS_vel( System.Int32 Axis_ID, System.Int32 Option, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_vel_all( System.Int32 Axis_ID, System.Int32 Option, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait );
        [DllImport("APS168.dll")]public static extern Int32 APS_line( System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] PositionArray, ref System.Double TransPara, ref ASYNCALL Wait );
        [DllImport("APS168.dll")]public static extern Int32 APS_line_v(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] PositionArray, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_line_all(System.Int32 Dimension, System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] PositionArray, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc2_ca(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc2_ca_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc2_ca_all(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double Angle, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc2_ce(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc2_ce_v(System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc2_ce_all(System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc3_ca( System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray, System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc3_ca_v( System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray,  System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait );
        [DllImport("APS168.dll")]public static extern Int32 APS_arc3_ca_all( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] NormalArray, System.Double Angle, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc3_ce( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc3_ce_v( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_arc3_ce_all( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_spiral_ca( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_spiral_ca_v( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_spiral_ca_all( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] NormalArray, System.Double Angle, System.Double DeltaH, System.Double FinalR, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_spiral_ce( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] NormalArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_spiral_ce_v( System.Int32[] Axis_ID_Array, System.Int32 Option,  System.Double[] CenterArray,  System.Double[] NormalArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vm, ref ASYNCALL Wait);
        [DllImport("APS168.dll")]public static extern Int32 APS_spiral_ce_all( System.Int32[] Axis_ID_Array, System.Int32 Option, System.Double[] CenterArray,  System.Double[] NormalArray,  System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);
        
        
        //Cognex 20150728   
        [DllImport("APS168.dll")]public static extern Int32 APS_COGNEX_set_point(System.Int32 Board_ID, COGNEX_POINT[] DataArr, System.Int32 ArraySize);
        [DllImport("APS168.dll")]public static extern Int32 APS_COGNEX_get_point(System.Int32 Board_ID, ref COGNEX_POINT Point, System.Int32 Index);
        [DllImport("APS168.dll")]public static extern Int32 APS_COGNEX_set_output_select(System.Int32 Board_ID, System.Int32 Mux_En, System.Int32 Mux_C);
        [DllImport("APS168.dll")]public static extern Int32 APS_COGNEX_set_mux_enable(System.Int32 Board_ID, System.Int32 Enable);
        [DllImport("APS168.dll")]public static extern Int32 APS_COGNEX_get_status(System.Int32 Board_ID, ref System.Int32 BufferCount, ref System.Int32 ActiveIndex);
        [DllImport("APS168.dll")]public static extern Int32 APS_COGNEX_set_do_width(System.Int32 Board_ID, System.Int32 Width);

        
        //**********************************************
        // New header functions; 20151102
        //**********************************************

        // pitch error compensation
        [DllImport("APS168.dll")]public static extern Int32 APS_set_pitch_table(System.Int32 Axis_ID, System.Int32 Comp_Type, System.Int32 Total_Points, System.Int32 MinPosition, System.UInt32 Interval, System.Int32[] Comp_Data);
        [DllImport("APS168.dll")]public static extern Int32 APS_get_pitch_table(System.Int32 Axis_ID, ref System.Int32 Comp_Type, ref System.Int32 Total_Points, ref System.Int32 MinPosition, ref System.UInt32 Interval, ref System.Int32 Comp_Data);
        [DllImport("APS168.dll")]public static extern Int32 APS_start_pitch_comp(System.Int32 Axis_ID, System.Int32 Enable);

        // Multi-dimension comparator function
        [DllImport("APS168.dll")]public static extern Int32 APS_set_multi_trigger_table(System.Int32 Board_ID, System.Int32 Dimension, MCMP_POINT[] DataArr, System.Int32 ArraySize, System.Int32 Window);
        [DllImport("APS168.dll")]public static extern Int32 APS_get_multi_trigger_table_cmp(System.Int32 Board_ID, System.Int32 Dimension, ref MCMP_POINT CmpVal);

		//Pulser function
        [DllImport("APS168.dll")]public static extern Int32 APS_manual_pulser_start(System.Int32 Board_ID, System.Int32 Enable);
        [DllImport("APS168.dll")]public static extern Int32 APS_manual_pulser_velocity_move(System.Int32 Axis_ID, System.Double SpeedLimit);
        [DllImport("APS168.dll")]public static extern Int32 APS_manual_pulser_relative_move(System.Int32 Axis_ID, System.Double Distance, System.Double SpeedLimit);
        [DllImport("APS168.dll")]public static extern Int32 APS_manual_pulser_home_move(System.Int32 Axis_ID);

        
        // [Wei-Li suggests to remove]
        //**********************************************
        // 2D arc-interpolation for 3-point
        [DllImport("APS168.dll")]public static extern Int32 APS_arc2_ct_all(System.Int32[] Axis_ID_Array, System.Int32 APS_Option, System.Double[] AnyArray, System.Double[] EndArray, System.Int16 Dir, ref System.Double TransPara, System.Double Vs, System.Double Vm, System.Double Ve, System.Double Acc, System.Double Dec, System.Double SFac, ref ASYNCALL Wait);

		//Pulser function
        [DllImport("APS168.dll")]public static extern Int32 APS_manual_pulser_configuration(System.Int32 Board_ID, System.Int32 Mode, System.Int32 InvPA, System.Int32 InvPB, System.Int32 InvDir, System.Double Ratio);
        //**********************************************


        // [Reserved for unknown usage]
        //**********************************************
        // Sampling functions extension[Only for PCI-82548 for up to 8 channel]
        //[DllImport("APS168.dll")]public static extern Int32 APS_wait_trigger_sampling_ex( System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_8CH DataArr );
       // [DllImport("APS168.dll")]public static extern Int32 APS_wait_trigger_sampling_async_ex( System.Int32 Board_ID, System.Int32 Length, System.Int32 PreTrgLen, System.Int32 TimeOutMs, ref STR_SAMP_DATA_8CH DataArr );
       // [DllImport("APS168.dll")]public static extern Int32 APS_get_sampling_data_ex( System.Int32 Board_ID, ref System.Int32 Length, ref STR_SAMP_DATA_8CH DataArr, ref System.Int32 Status );

        
        [DllImport("APS168.dll")]public static extern Int32  APS_get_watch_timer( System.Int32 Board_ID, ref System.Int32  Timer );
        [DllImport("APS168.dll")]public static extern Int32  APS_reset_wdt( System.Int32 Board_ID, System.Int32 WDT_No);

        [DllImport("APS168.dll")]public static extern Int32  APS_get_field_bus_slave_mapto_AxisID( System.Int32 Board_ID, System.Int32 BUS_No, System.Int32 MOD_No, ref System.Int32 AxisID );
        //**********************************************

	}
}