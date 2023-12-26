#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.SystemHelp
 * 唯一标识：d51e0e6f-39df-4aca-b11c-44432974b92c
 * 文件名：CommonHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 10:15:46 AM
 * 版本：V1.0.0
 * 描述：
 * 
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Windows.Win32;
using Windows.Win32.System.SystemInformation;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Soso.Common.SystemHelp
{
    /// <summary>
    /// 系统常用帮助类
    /// </summary>
    public static class CommonHelp
    {
        private static PerformanceCounter _cpuPerformance;
        private static PerformanceCounter _ramPerformance;

        static CommonHelp()
        {
            try
            {
                _cpuPerformance = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
                _ramPerformance = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 字节转换，默认转换成最大单位
        /// </summary>
        /// <param name="v">大小</param>
        /// <param name="level">单位</param>
        public static void ByteConvert(ref double v, ref string level)
        {
            switch (level)
            {
                case "B":
                    if (v / 1024 > 1)
                    {
                        level = "KB";
                        v /= 1024;
                        ByteConvert(ref v, ref level);
                    }
                    break;

                case "KB":
                    if (v / 1024 > 1)
                    {
                        level = "MB";
                        v /= 1024;
                        ByteConvert(ref v, ref level);
                    }
                    break;

                case "MB":
                    if (v / 1024 > 1)
                    {
                        level = "GB";
                        v /= 1024;
                        ByteConvert(ref v, ref level);
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取系统空间时间
        /// </summary>
        /// <returns></returns>
        public static int GetIdleTicks()
        {
            LASTINPUTINFO lastInputInfo = default;
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            if (!PInvoke.GetLastInputInfo(ref lastInputInfo))
            {
                return 0;
            }
            return Environment.TickCount - (int)lastInputInfo.dwTime;
        }

        /// <summary>
        /// 字节数组转换为ASCII字符串
        /// </summary>
        /// <param name="array">字节数组</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">数组为空或长度为零</exception>
        public static string ByteToASCIIString(byte[] array)
        {
            if (array == null || array.Length <= 0)
            {
                throw new ArgumentException("Input array can't be null or count is zero!");
            }

            StringBuilder sb = new StringBuilder();
            foreach (byte data in array)
            {
                sb.Append(data.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 字节数组转换为ASCII字符串，指定起始位置和分割符
        /// </summary>
        /// <param name="array">字节数组</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="length">结束位置</param>
        /// <param name="split">分割符</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">输入的起始位置不正确或数组为空</exception>
        public static string ByteToASCIIString(byte[] array, int startIndex, int length, char split = ' ')
        {
            if (array == null || array.Length <= 0)
            {
                throw new ArgumentException("Input array can't be null or count is zero!");
            }

            if (length > array.Length || startIndex > length)
            {
                throw new ArgumentException("Input start index can't more than length or input length more than array length!");
            }

            StringBuilder sb = new StringBuilder();
            for (int i = startIndex; i < length; i++)
            {
                sb.Append(array[i].ToString("X2"));
                if (i < length - 1)
                {
                    sb.Append(split);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// ASCII字符串转换为字节数组
        /// </summary>
        /// <param name="strASCII"></param>
        /// <returns></returns>
        public static byte[] ASCIIStringToByte(string strASCII)
        {
            byte[] array = new byte[strASCII.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(strASCII.Substring(i * 2, 2), 16);
            }
            return array;
        }

        /// <summary>
        /// ASCII字符串转换为字节数组
        /// </summary>
        /// <param name="strASCII"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static byte[] ASCIIStringToByte(string strASCII, string split)
        {
            string strTemp = strASCII.Replace(split, "");
            return ASCIIStringToByte(strTemp);
        }

        private static long GetHardDiskSpace(string strHardDiskName, Func<DriveInfo, long> func)
        {
            if (string.IsNullOrWhiteSpace(strHardDiskName))
            {
                throw new ArgumentException("The drive letter was incorrectly entered!");
            }

            long size = 0L;
            strHardDiskName = strHardDiskName.Substring(0, 1) + ":\\";

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.Name == strHardDiskName)
                {
                    size = func(drive);
                }
            }
            return size;
        }

        /// <summary>
        /// 获取指定分区全部空间大小
        /// </summary>
        /// <param name="driverLetter">盘符</param>
        /// <returns></returns>
        public static long GetHardDiskTotalSpace(string driverLetter)
        {
            return GetHardDiskSpace(driverLetter, (drive) => { return drive.TotalSize; });
        }

        /// <summary>
        /// 获取指定分区剩余空间大小
        /// </summary>
        /// <param name="driverLetter">盘符</param>
        /// <returns></returns>
        public static long GetHardDiskFreeSpace(string driverLetter)
        {
            return GetHardDiskSpace(driverLetter, (drive) => { return drive.TotalFreeSpace; });
        }

        private static MEMORYSTATUSEX GetMemoryStatus()
        {
            MEMORYSTATUSEX memoryStatus = default;
            memoryStatus.dwLength = (uint)Marshal.SizeOf(memoryStatus);
            PInvoke.GlobalMemoryStatusEx(out memoryStatus);
            return memoryStatus;
        }

        /// <summary>
        /// 获取当前系统内存使用率
        /// </summary>
        /// <returns></returns>
        public static double GetMemortRate()
        {
            return GetMemoryStatus().dwMemoryLoad / 100.0;
        }

        /// <summary>
        /// 获得当前系统可用物理内存大小
        /// </summary>
        /// <returns>当前可用物理内存（B）</returns>
        public static ulong GetAvailPhys()
        {
            return GetMemoryStatus().ullAvailPhys;
        }

        /// <summary>
        /// 获得当前系统已使用的内存大小
        /// </summary>
        /// <returns>已使用的内存大小（B）</returns>
        public static ulong GetUsedPhys()
        {
            MEMORYSTATUSEX mi = GetMemoryStatus();
            return mi.ullTotalPhys - mi.ullAvailPhys;
        }

        /// <summary>
        /// 获得当前系统总计物理内存大小
        /// </summary>
        /// <returns>总计物理内存大小（B）</returns>
        public static ulong GetTotalPhys()
        {
            return GetMemoryStatus().ullTotalPhys;
        }

        /// <summary>
        /// 超过总内存的百分比清除缓存 
        /// </summary>
        /// <param name="used">当前程序使用的内存</param>
        /// <param name="limitedRate">百分比</param>
        public static void ClearMemory(double used, double limitedRate)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //超过设置值清除缓存 
                if (used > GetTotalPhys() * limitedRate)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    PInvoke.SetProcessWorkingSetSize((Windows.Win32.Foundation.HANDLE)Process.GetCurrentProcess().Handle, -1, -1);
                }

            }
        }

        /// <summary>
        /// 超过设置值清除内存
        /// </summary>
        /// <param name="limitedRate"></param>
        public static void ClearMemory(double limitedRate)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //超过设置值清除缓存 
                if (GetMemortRate() > limitedRate)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    PInvoke.SetProcessWorkingSetSize((Windows.Win32.Foundation.HANDLE)Process.GetCurrentProcess().Handle, -1, -1);
                }

            }
        }

        /// <summary>
        /// 获取当前程序的CPU使用情况
        /// </summary>
        /// <returns></returns>
        public static float GetCpuUsedPercent()
        {
            return _cpuPerformance.NextValue();
        }

        /// <summary>
        /// 获取程序使用内存情况
        /// </summary>
        /// <param name="size">大小</param>
        /// <returns>单位</returns>
        public static string GetRamUsed(ref double size)
        {
            size = _ramPerformance.NextValue();
            string units = "B";
            ByteConvert(ref size, ref units);
            return units;
        }

        /// <summary>
        /// 删除超过指定时间和硬盘空间的文件
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="days">超过天数</param>
        /// <param name="diskSpacePercent">硬盘空间百分比</param>
        /// <param name="bIgnoreDiskSpace">忽略硬盘空间</param>
        /// <exception cref="Exception">删除文件失败异常</exception>
        public static void DeleteOverdueFiles(string dirPath, int days, double diskSpacePercent, bool bIgnoreDiskSpace = false)
        {
            if (Directory.Exists(dirPath) && days > 0)
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(dirPath);

                    DirectoryInfo[] subdirs = dir.GetDirectories();

                    foreach (DirectoryInfo di in subdirs)
                    {
                        DeleteOverdueFiles(di.FullName, days, diskSpacePercent, bIgnoreDiskSpace);
                    }

                    List<FileInfo> files = dir.GetFiles("*.*").ToList();

                    //修改时间升序排序
                    files.Sort((x, y) => { return x.LastWriteTime.CompareTo(y.LastWriteTime); });

                    //采用时间升序排序，如果第一个时间没有过期，后面都不会过期
                    for (int i = 0; i < files.Count;)
                    {
                        FileInfo fi = files[i];

                        if ((DateTime.Now - fi.LastWriteTime).Days > days)
                        {
                            fi.Delete();

                            files.RemoveAt(i);

                            Thread.Sleep(100);
                        }
                        else
                        {
                            break;
                        }
                    }


                    if (!bIgnoreDiskSpace)
                    {
                        //判断磁盘剩余空间
                        DirectoryInfo dif = new DirectoryInfo(dirPath);

                        double dbFreeSpace = GetHardDiskFreeSpace(dif.Root.Name) * 1.0 / GetHardDiskTotalSpace(dif.Root.Name);

                        while (dbFreeSpace < diskSpacePercent && files.Count > 0)
                        {
                            files[0].Delete();

                            files.RemoveAt(0);

                            Thread.Sleep(100);

                            dbFreeSpace = GetHardDiskFreeSpace(dif.Root.Name) * 1.0 / GetHardDiskTotalSpace(dif.Root.Name);
                        }
                    }

                    //空文件夹不能直接删除，需要看空文件夹的创建时间，如果超过1小时则删除
                    TimeSpan span = DateTime.Now - dir.CreationTime;
                    if (dir.GetFiles("*.*").Length == 0 && dir.GetDirectories().Length == 0 && span.TotalHours > 1)
                    {
                        Directory.Delete(dirPath);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to delete file!", ex);
                }
            }
        }
    }
}
