using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace P440
{
    /// <summary>
    /// 返回的错误码
    /// </summary>
    public enum ErrorCode : int
    {
        /// <summary>
        /// 正常
        /// </summary>
        Success = 0,
        
        /// <summary>
        /// 失败
        /// </summary>
        Failed = -1,

        /// <summary>
        /// 打开失败
        /// </summary>
        OpenFailed = -2,

        /// <summary>
        /// 超时
        /// </summary>
        TimeOut = -3,

        /// <summary>
        /// 轴号错误
        /// </summary>
        AxisNoError = -4,
        
        /// <summary>
        /// 参数错误
        /// </summary>
        ParameterError = -5,

        /// <summary>
        /// PCI写入输入错误
        /// </summary>
        WriteFailed = -6,

        /// <summary>
        /// PCI读取数据错误
        /// </summary>
        ReadFailed = -7,

        /// <summary>
        /// 正在运行
        /// </summary>
        Moving = -8,

        /// <summary>
        /// IO卡编号错误
        /// </summary>
        IoCardNoError = -9
    };

    /// <summary>
    /// P440 IO卡的库函数
    /// </summary>
    public class Function
    {
        /// <summary>
        /// 获取DLL库的版本号
        /// </summary>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_GetLibVer();

        /// <summary>
        /// 获取固件版本号
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="FirmVersion"></param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_GetFirmwareVer(int CardNo, ref int FirmVersion);

        /// <summary>
        /// 获取采集卡类型，0：IO卡，1：运动控制卡
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="CardType"></param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_GetCardType(int CardNo, ref int CardType);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_Initial(int CardNo);

        /// <summary>
        /// 输出一组端口
        /// </summary>
        /// <param name="CardNo">卡序号，第一张卡序号为0</param>
        /// <param name="NodeId">默认为0</param>
        /// <param name="OutputState">端口值</param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_WriteOutput(int CardNo, int NodeId, UInt32 OutputState);

        /// <summary>
        /// 设置一个输出口的值
        /// </summary>
        /// <param name="nCardNo"></param>
        /// <param name="NodeId"></param>
        /// <param name="nIndex"></param>
        /// <param name="nValue"></param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]        
        public static extern int SCT_WriteOutBit(int nCardNo, int NodeId, int nIndex, int nValue);

        /// <summary>
        /// 读取一组输入端口
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="NodeId"></param>
        /// <param name="InputState"></param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_ReadInput(int CardNo, int NodeId, ref UInt32 InputState);

        /// <summary>
        /// 读取一个输入口
        /// </summary>
        /// <param name="nCardNo"></param>
        /// <param name="NodeId"></param>
        /// <param name="nIndex"></param>
        /// <param name="InBit"></param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_ReadInBit(int nCardNo, int NodeId, int nIndex, ref UInt32 InBit);

        /// <summary>
        /// 读取输出口的状态
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="NodeId"></param>
        /// <param name="OutputState"></param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_ReadOutput(int CardNo, int NodeId, ref UInt32 OutputState);

        /// <summary>
        /// 读取输出口的状态（单个端口）
        /// </summary>
        /// <param name="nCardNo"></param>
        /// <param name="NodeId"></param>
        /// <param name="nIndex"></param>
        /// <param name="pOutBit"></param>
        /// <returns></returns>
        [DllImport("M2400.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SCT_ReadOutBit(int nCardNo, int NodeId, int nIndex, ref UInt32 pOutBit);  
    }
}
