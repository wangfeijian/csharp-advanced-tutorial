using System;
using System.Text;
using System.Runtime.InteropServices;
#pragma warning disable 1591

namespace PCI_DMC_ERR
{
    public class CPCI_DMC_ERR
    {
        //General
        public const short ERR_SameCardNumber = 901;		//卡號重複(所有軸卡卡號不能相同)
        public const short ERR_CardType = 902;	//此軸卡不支援此API
        public const short ERR_CantFindDLL = 903;	//此API與其對應的DLL連結失敗(沒有DLL或是DLL函式庫中沒有此函式)
        public const short ERR_SerchErrorCode = 904;	//此API的錯誤需使用_DMC_01_get_cardtype_errorcode來取得
        public const short ERR_NoCardFound = 905;	//找不到此軸卡
        public const short ERR_DLLinUse = 910;	//合併版DLL正被使用中
        public const short ERR_CantFindAB01 = 911;		//找不到A01DLL
        public const short ERR_CantFindF01 = 912;	//找不到F01DLL
        public const short ERR_CantFindDMCDLL = 913;	//找到A01、F01DLL

        //PCI_DMC_A01 / PCI_DMC_B01  
        public const short ERR_NoError = 0;	//沒問題
        public const short ERR_FeedRate_Entry_Dec = 1;	//FeedRate overwrite 進入減速段,本指令不再變化速度,但下個指令將會新的速度變化
        public const short ERR_CardNoError = 3;	//卡號錯誤請確認卡片上的DIP Switch調整的號碼
        public const short ERR_bootmodeErr = 5;	//無法啟動DSP數位程序
        public const short ERR_downloadcode = 6;//DSP Program Memory R/W Error
        public const short ERR_downloadinit = 7;//DSP Data Memory R/W Error
        public const short ERR_PCI_boot_first = 8;	//使用此API需先啟動DSP數位程序  I16 PASCAL _DMC_01_pci_initial(U16 CardNo)
        public const short ERR_FeedRate_updata = 9;	//FeedRate overwrite 的值相同 
        public const short ERR_DSP_inside_calcu = 10;//DSP memory float error
        public const short ERR_AxisNoError = 11;	//軸數目過大
        public const short ERR_IPO_First = 12;	//需要為IPO模式
        public const short ERR_Target_reach = 13;//Mode 1 運行時,需Target到達
        public const short ERR_Servo_on_first = 14;	//需要先servo on 
        public const short ERR_MPG_Mode = 15;//在手輪模式下無法清除位置
        public const short ERR_PDO_TG = 16;	//使用PDO模式下指令給模組,無法回傳收到
        public const short ERR_ConfigFileOpenError = 17;	//建立Debug Information檔案錯誤
        public const short ERR_Ctrl_value = 18;	//使用控制指令錯誤
        public const short ERR_Security_Fifo = 19;	//使用Security Fpga write Error
        public const short ERR_Security_Fifo_busy = 20;	//使用Security Fpga busy
        public const short ERR_SpeedLimitError = 21;	//設定速度大於最大速度
        public const short ERR_Security_Page = 22;	//使用Security page 需要小於16
        public const short ERR_Slave_Security_op = 23;	//使用Security slave_operate 指令無效
        public const short ERR_channel_no = 24;	//channel no 錯誤
        public const short ERR_start_ring_first = 25;//使用此API需先啟動DMCNet  I16 PASCAL _DMC_01_start_ring(U16 CardNo, U8 RingNo)
        public const short ERR_NodeIDError = 26;	//無此NodeID
        public const short ERR_MailBoxErr = 27;	//指令無法下達, DSP Busy
        public const short ERR_SdoData = 28;	//SDO Data送出但無回應,SDO Data Send Request ,But Without ACK
        public const short ERR_IOCTL = 29;	//作業系統無法處理此IRP
        public const short ERR_SdoSvonFirst = 30;//使用SDO操作軸控時,需先Servo On
        public const short ERR_SlotIDError = 31;	//GA無此Slot號碼
        public const short ERR_PDO_First = 32;	//使用PDO指令需先將軸轉成PDO模式
        public const short ERR_Protocal_build = 33;	//Protocal,電氣尚未建立
        public const short ERR_Maching_TimeOut = 34;	//模組配對time Out
        public const short ERR_Maching_NG = 35;	//模組配對NG
        public const short ERR_Group_Num = 36;	//設定群組最大值為6組
        public const short ERR_Master_Alarm = 37;//故障發生(通訊不良,Driver Alm)
        public const short ERR_Alarm_reset = 38;
        public const short ERR_Master_Security_Wr = 40;	//使用Security Master Write指令無效
        public const short ERR_Master_Security_Rd = 41;	//使用Security Master Read指令無效
        public const short ERR_Master_Security_Pw = 42;	//需要先輸入正確的password
        public const short ERR_NonSupport_CardVer = 50;	//使用主控卡的版本錯誤,請連繫代理商,購買正確主控卡
        public const short ERR_Compare_Source = 51;	//Ver Type : B Compare Source 選擇錯誤
        public const short ERR_Compare_Direction = 52;	//Compare的方向錯誤  dir需為1或0 1:ccw,0:cw
        public const short ERR_GetDLLPath = 60;
        public const short ERR_GetDLLVersion = 61;
        public const short ERR_GA_Port = 62;
        public const short ERR_04PISTOP_Timeout = 70;	//04PI Stop Fifo time out 
        public const short ERR_ServoSTOP_Timeout = 71;	//Servo Stop Fifo time out
        public const short ERR_04PISTOP_status = 72;	//04PI Stop MC_done not to Error
        public const short ERR_04PIHoming_err = 73;	//04PI Home status error
        public const short ERR_04PISdo_trans = 74;	//04PI sdo send but get data error
        public const short ERR_QEP_INDEX = 75;		// QEP
        public const short ERR_GPIO_OUTPUT_SIZE = 76;	// GPIO
        public const short ERR_IPCMP_INDEX = 77;		// IPCMP
        public const short ERR_IPCMP_MPC_DATA_TYPE = 78;     	// It will never happened (internal use)

        public const short ERR_TPCMP_FIFO_INDEX = 79;	// TPCMP
        public const short ERR_TPCMP_QEP_SOURCE = 80;
        public const short ERR_TPCMP_TABLE_NO_DATA = 81;    	// Input table empty
        public const short ERR_TPCMP_TABLE_OUT_OF_RANGE = 82;		// Input table size is large than TPCMP max. size
        public const short ERR_TPCMP_TABLE_OVERFLOW = 83;    	// Input table size is large than TPCMP FIFO remainder size

        public const short ERR_EXTIO_RANGE = 84;		// EXT. IO
        public const short ERR_EXTIO_LATCH_TYPE = 85;  	// It will never happened (internal use)

        public const short ERR_LATCH_FIFO_EMPTY = 86;		// Latch FIFO
        public const short ERR_LATCH_FIFO_INVALID_DATA = 87;
        public const short ERR_RangeError = 112;	//設定軸的號碼錯誤  不用加,因為軸的判別加在NodeID
        public const short ERR_MotionBusy = 114;	//Motion 指令重疊
        public const short ERR_SpeedError = 116;	//最大速度設置為0 
        public const short ERR_AccTimeError = 117;	//加減速時大於1000秒
        public const short ERR_PitchZero = 124;	//Helix pitch參數等於0,無法運動
        public const short ERR_BufferFull = 127;	//運動指令Buffer己滿
        public const short ERR_PathError = 128;	//運動指令錯誤
        public const short ERR_NoSupportMode = 130;	//不支援速度變化
        public const short ERR_FeedHold_support = 132;	//Feedhold Stop 啟動,無法接受新指令 
        public const short ERR_SDStop_On = 133;	//執行減速指令, 無法接受新指令
        public const short ERR_VelChange_supper = 134;	//模式1.Feedhold,2.同動指令3.減速指令,無法執行速度變化功能
        public const short ERR_Command_set = 135;	//無法連續執行FeedHold的功能
        public const short ERR_sdo_message_choke = 136;	//Sdo指令回傳有誤,請檢查網路線接線是否OK
        public const short ERR_VelChange_buff_feedhold = 137;	//Feed Hold  功能必須先致能 ,無法速度變化
        public const short ERR_VelChange_sync_move = 138;	//目前軸卡正在等待同動指令,無法速度變化
        public const short ERR_VelChange_SD_On = 139;	//目前軸卡正在執行減速指令,無法速度變化
        public const short ERR_P_Change_Mode = 140;	//單軸點對點模式 加速段,速度等於0,非單軸點對點模式
        public const short ERR_BufferLength = 141;	//當模式在 _Path_p_change,_Path_velocity_change_onfly, _Path_Start_Move_2seg時 Buffer Length 需要為0
        public const short ERR_2segMove_Dist = 142;	//距離需要同向
        public const short ERR_CenterMatch = 143;	// 經過反向運算圓心要一致
        public const short ERR_EndMatch = 144;	//經過反向運算圓心要一致
        public const short ERR_AngleCalcu = 145;	//經過計算角度錯誤
        public const short ERR_RedCalcu = 146;	//經過半徑錯誤
        public const short ERR_GearSetting = 147;	//Gear的分子或分母為0
        public const short ERR_CamTable = 148;	// Table Setting First Arrary Point Error設定table不能到負值table[-1]沒有這種設定
        public const short ERR_AxesNum = 149;	// 多軸設定值必須要兩軸以上
        public const short ERR_SpiralPos = 150;	// 最終位置會到達螺旋圓心
        public const short ERR_SpeedMode_Slave = 151;	// 在速度連續時使用的Slave軸,無法執行Motion指令
        public const short ERR_SpeedMode_SlaveSet = 152;	// 設定軸的虛擬必須在軸的前半部
        public const short ERR_VelChange_high = 153;	// 設定值速度改變過大或是改變sec過長
        public const short ERR_Backlash_step = 154;	// accstep+contstep+decstep需小於100
        public const short ERR_Backlash_status = 155;	//設定時motion done必需為0且buffer length必需為0	
        public const short ERR_DistOver = 156;	//輸入Dist超過TotalDist

        public const short Compare_Cards_Not_Equal = 201;	//比對結果─軸卡資訊(卡號、數量)不符
        public const short Compare_Nodes_Not_Equal = 202;	//比對結果─站號資訊(數量)不符
        public const short Compare_Node_ID_Not_Equal = 203;	//比對結果─站號資訊(站號)不符
        public const short Compare_Node_Device_Type_Not_Equal = 204;	//比對結果─模組資訊(模組第二分類)不符
        public const short Compare_Node_Identity_Object_Not_Equal = 205;	//比對結果─模組資訊(模組第一分類)不符
        public const short Compare_File_Path_NULL = 206;	//檔案路徑錯誤
        public const short Compare_File_Open_Fail = 207;	//檔案開啟失敗(請確定路徑輸入正確)
        public const short Compare_File_Not_Exist = 208;	//檔案不存在

        //PCI_DMC_F01

        //main
        public const short ERR_NotCardFound = 301;//無此卡號或尚未Initial
        public const short ERR_CardInitial = 302;	//Initial失敗
        public const short ERR_MemoryAccess_Failed = 303;	//記憶體讀寫失敗
        public const short ERR_MemoryOutOfRange = 304;//記憶體使用超過Range
        public const short ERR_UartTxIsBusy = 305;	//Uart Tx is busy
        public const short ERR_UartRxError = 306;	//Uart Rx 讀取錯誤
        public const short ERR_UartRxIsNotReady = 307;	//Uart Rx 尚未準備完成
        public const short ERR_NotSupportFunc = 308;	//不支援此Function
        public const short ERR_NoNodeFound = 309;	//站號設置錯誤
        public const short ERR_APIInputError = 310;	//API參數輸入錯誤(超出限定值)
        public const short ERR_SDOFailed = 311;	//SDO傳送失敗
        public const short ERR_SDOBusy = 312;	//SDO忙碌中 / SDO同時有兩個指令被寫入
        public const short ERR_APITypeErr = 313;	//此模組不支援此API
        public const short ERR_ScaleFailed = 314;	//AD校正失敗
        public const short ERR_F_BufferFull = 315;//MailBox_Buffer已滿
        public const short ERR_ConnectErr = 316;	//通訊連線異常
        public const short ERR_MBWordChFailed = 317;	//MailBox同時有兩個指令寫入
        public const short ERR_MailBoxFailed = 318;	//MailBox傳送失敗
        public const short ERR_CantResetCard = 319;	//無法ResetCard
        public const short ERR_PDOFailed = 320;	//PDO未回應
        public const short ERR_MBCmding = 321;//MailBox正在處理指令
        public const short ERR_SVOFF = 322;	//此動作需Servo On方可執行
        public const short ERR_DriverError = 323;	//此動作因DriverErr無法執行，請先執行Ralm
        public const short ERR_ConnReset_Failed = 324;//初始化重置失敗
        public const short ERR_F01SlotIDError = 325;	//此裝置不支援Slot或輸入的Slot編號超出範圍
        public const short ERR_UartData_NoMatch = 326;//Download Code讀取Uart的資料時，回傳無法符合正確值
        public const short ERR_SVON = 327;//此動作需Servo Off方可執行
        public const short ERR_Mpg_Already_On = 328;	//此軸已經致能手輪、Jog或DDA功能，其餘功能暫時無法使用
        public const short ERR_MpgNumber_Over = 329;	//手輪或Jog致能數量已達上限(最大3組)
        public const short ERR_Mpg_Data_Failed = 330;	//手輪或Jog資料傳遞失敗
        public const short ERR_DDA_Buffer_Full = 331;	//DDA Buffer已滿
        public const short ERR_F_Slave_Security_op = 332;	//Slave Secutiry寫入失敗		
        public const short ERR_F_Security_Page = 333;	//Page超過設置
        public const short ERR_F_GetDLLPath = 334;//找不到DLL路徑
        public const short ERR_F_GetDLLVersion = 335;	//找不到DLL版本訊息
        public const short F_Compare_File_Open_Fail = 336;//檔案開啟失敗(請確定路徑輸入正確)	
        public const short F_Compare_File_Not_Exist = 337;//檔案不存在
        public const short F_Compare_Cards_Not_Equal = 338;	//比對結果─軸卡資訊(卡號、數量)不符
        public const short F_Compare_File_Path_NULL = 339;//檔案路徑錯誤
        public const short F_Compare_Node_ID_Not_Equal = 340;	//比對結果─站號資訊(站號)不符
        public const short F_Compare_Node_Device_Type_Not_Equal = 341;	//比對結果─模組資訊(模組第二分類)不符
        public const short F_Compare_Node_Identity_Object_Not_Equal = 342;	//比對結果─模組資訊(模組第一分類)不符
        public const short F_Compare_Nodes_Not_Equal = 343;	//比對結果─站號資訊(數量)不符
        public const short ERR_SecurityNoRet = 344;	//Security傳送結果未回傳
        public const short ERR_SDORetTimeOut = 345;	//SDO回傳時間過長
        public const short ERR_Uart_Connect_Fail = 346;	//Uart通訊失敗
        public const short ERR_CardNum_SetError = 347;//卡號設置錯誤
        public const short ERR_Target_Reached = 348;//無正確停止
        public const short ERR_NoF02Found = 349;//找不到F02
        public const short ERR_MCHSecurity = 350;//F02 Security Error


        public const short ERR_UseGetError = 399;	//系統型錯誤, 需使用_DMC_01_master_alm_code讀取錯誤碼
        //sub if main = 99
        public const short ERR_sub_NoError = 0;	//此卡號所對應之軸卡沒有發生錯誤
        public const short ERR_sub_CantConnect = 1;		//DMC通訊連結無法生成
        public const short ERR_sub_SDOFailed = 2;		//SDO傳送失敗
        public const short ERR_sub_CantReset = 3;		//無法重置通訊

    }
}