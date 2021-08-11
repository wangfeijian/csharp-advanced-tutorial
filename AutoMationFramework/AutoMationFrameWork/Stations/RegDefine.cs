using System;
namespace AutoMationFrameWork.Stations
{
    /// <summary>
    /// 系统网口配置
    /// </summary>
    public enum SysEthPortCfg
    {
        镭射一站激光控制器一,
        镭射一站激光控制器二,
        镭射二站激光控制器 = 0,
        镭射三站激光控制器,
        

        线扫镭射编码器触发 = 0,
        线扫镭射连续测量_1,
        线扫镭射连续测量_2,
        线扫镭射连续测量_3,
        线扫镭射连续测量_4,
        线扫镭射连续测量_5,
        线扫镭射连续测量角点_1,
        线扫镭射连续测量角点_2,

        线扫镭射三站连续测量137和146=7,
        测试平面度 = 11,
        Max_Index,
    }
    /// 
    /// </summary>
    public enum SysBitReg
    {
        xxx,

        机器归位完成,
        产品扫码完成,

        转盘站初始化完成,
        转盘站通知视觉检测一站转动到位,
        转盘站通知视觉检测二站转动到位,
        转盘站通知镭射一站站转动到位,
        转盘站通知镭射二站站转动到位,
        转盘站通知镭射三站站转动到位,

        视觉检测一站初始化完成,
        视觉检测一站工作中,
        视觉检测一站工作完成,

        视觉检测二站初始化完成,
        视觉检测二站工作中,
        视觉检测二站工作完成,

        镭射一站初始化完成,
        镭射一站工作中,
        镭射一站工作完成,

        镭射二站初始化完成,
        镭射二站工作中,
        镭射二站工作完成,

        镭射三站初始化完成,
        镭射三站工作中,
        镭射三站工作完成,

        清料模式,

        生产中,
        点检,
        离线模式,
        光栅启动,

        转盘转动到位,
    }

    /// <summary>
    /// 系统整型寄存器索引枚举声明
    /// </summary>
    public enum SysIntReg 
    {
        /// <summary>
        /// 站位运行进度百分比，用于界面显示进度条      
        /// </summary>
        Int_Process_Step,

        Int_Ok_Count,
        Int_Ng_Count,
        Int_Mesh_Index, //取Mesh的位置
    };

    /// <summary>
    /// 浮点型整型寄存器索引枚举声明
    /// </summary>
    public enum SysDoubleReg
    {
        Double_LeftAnglePoint,
        Double_RightAnglePoint,

    };

    /// <summary>
    /// 系统字符串寄存器索引枚举声明
    /// </summary>
    public enum SysStrReg
    {
        Str_BarCode,
        Str_Item

    };
}