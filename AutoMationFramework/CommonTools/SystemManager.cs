/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-26                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    System manager class                     *
*********************************************************************/

using System;
using System.Runtime.InteropServices;

namespace CommonTools
{
    /// <summary>系统运行模式设这一，自动运行模式，空跑模式，标定模式，其它自定义模式，</summary>
    public enum SystemMode
    {
        /// <summary>正常运行模式</summary>
        NormalRunMode,
        /// <summary>空跑模式，是否带料由流程决定</summary>
        DryRunMode,
        /// <summary>自动标定模式</summary>
        CalibRunMode,
        /// <summary>模拟运行模式</summary>
        SimulateRunMode,
        /// <summary>其它自定义模式</summary>
        OtherMode,
    }
    public class SystemManager:SingletonPattern<SystemManager>
    {
        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern char GetBit(int index);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetBit(int index, char value);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetInt(int index);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetInt(int index, int value);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double GetDouble(int index);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetDouble(int index, double value);

        // c中的char *返回，在c#中需要通过IntPtr来接收
        //Marshal.PtrToStringAnsi(ptr1)通过这个方法来转成字符串
        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetString(int index);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetString(int index, string str);
    }
}
