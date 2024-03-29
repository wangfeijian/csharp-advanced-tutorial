﻿#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.SystemHelp
 * 唯一标识：182b41a0-8183-429b-bf7f-03a4b690bfd5
 * 文件名：ComputerInfo
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：11/12/2023 11:09:29 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 * 
 * 版本：V1.0.0
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>
using System;
using System.Collections.Generic;
using System.Management;

namespace Soso.Common.SystemHelp
{
    /// <summary>
    /// Windows API类型
    /// </summary>
    public enum WindowsAPIType
    {
        /// <summary>
        /// 内存
        /// </summary>
        Win32_PhysicalMemory,
        /// <summary>
        /// cpu
        /// </summary>
        Win32_Processor,
        /// <summary>
        /// 硬盘
        /// </summary>
        win32_DiskDrive,
        /// <summary>
        /// 电脑型号
        /// </summary>
        Win32_ComputerSystemProduct,
        /// <summary>
        /// 分辨率
        /// </summary>
        Win32_DesktopMonitor,
        /// <summary>
        /// 显卡
        /// </summary>
        Win32_VideoController,
        /// <summary>
        /// 操作系统
        /// </summary>
        Win32_OperatingSystem

    }

    /// <summary>
    /// Windows API 键
    /// </summary>
    public enum WindowsAPIKeys
    {
        /// <summary>
        /// 名称
        /// </summary>
        Name,
        /// <summary>
        /// 显卡芯片
        /// </summary>
        VideoProcessor,
        /// <summary>
        /// 显存大小
        /// </summary>
        AdapterRAM,
        /// <summary>
        /// 分辨率宽
        /// </summary>
        ScreenWidth,
        /// <summary>
        /// 分辨率高
        /// </summary>
        ScreenHeight,
        /// <summary>
        /// 电脑型号
        /// </summary>
        Version,
        /// <summary>
        /// 硬盘容量
        /// </summary>
        Size,
        /// <summary>
        /// 内存容量
        /// </summary>
        Capacity,
        /// <summary>
        /// cpu核心数
        /// </summary>
        NumberOfCores
    }

    /// <summary>
    /// windows电脑信息获取类
    /// </summary>
    public static class ComputerInfo
    {
        /// <summary>
        /// 获取系统内存大小，单位(B)
        /// </summary>
        /// <returns></returns>
        public static double GetSystemMemorySize()
        {
            double size = 0;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();   //用于查询一些如系统信息的管理对象
            searcher.Query = new SelectQuery(WindowsAPIType.Win32_PhysicalMemory.ToString(), "",
                new string[] { WindowsAPIKeys.Capacity.ToString() });//设置查询条件
            ManagementObjectCollection collection = searcher.Get();   //获取内存容量
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties[WindowsAPIKeys.Capacity.ToString()].Value != null)
                {
                    try
                    {
                        size += double.Parse(baseObj.Properties[WindowsAPIKeys.Capacity.ToString()].Value.ToString());
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            return size;
        }

        /// <summary>
        /// 获取硬盘容量(B)
        /// </summary>
        /// <returns>硬盘序列号和容量</returns>
        public static Dictionary<string, double> GetDiskSizeAndSerialNumber()
        {
            var result = new Dictionary<string, double>();
            double size = 0;
            string caption;
            try
            {
                ManagementClass hardDisk = new ManagementClass(WindowsAPIType.win32_DiskDrive.ToString());
                ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                foreach (ManagementObject m in hardDiskC)
                {
                    size = Convert.ToDouble(m[WindowsAPIKeys.Size.ToString()].ToString());
                    caption = m["SerialNumber"].ToString();
                    result[caption] = size;
                }
            }
            catch
            {

            }
            return result;
        }

        /// <summary>
        /// 操作系统版本
        /// </summary>
        public static string GetOSVersion()
        {
            string result = "Windows";
            try
            {
                ManagementClass hardDisk = new ManagementClass(WindowsAPIType.Win32_OperatingSystem.ToString());
                ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                foreach (ManagementObject m in hardDiskC)
                {
                    result = m[WindowsAPIKeys.Name.ToString()].ToString().Split('|')[0].Replace("Microsoft", "");
                    break;
                }
            }
            catch
            {

            }
            return result.Trim();
        }

        /// <summary>
        /// 获取物理网卡地址
        /// </summary>
        public static Dictionary<string, string> GetPhysicalNetworkCardMacAddress()
        {
            var result = new Dictionary<string, string>();
            List<string> networks = new List<string>();

            try
            {
                // 获取物理网卡名称
                string qry = "SELECT * FROM MSFT_NetAdapter WHERE Virtual=False";
                ManagementScope scope = new ManagementScope(@"\\.\ROOT\StandardCimv2");
                ObjectQuery query = new ObjectQuery(qry);
                ManagementObjectSearcher mos = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection moc = mos.Get();

                foreach (ManagementObject mo in moc)
                {
                    networks.Add(mo["DriverDescription"].ToString());
                    mo.Dispose();
                }

                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc2 = mc.GetInstances();
                foreach (ManagementObject mo in moc2)
                {
                    if (networks.Contains(mo["Description"].ToString()))
                    {
                        result[mo["Description"].ToString()] = mo["MacAddress"].ToString();
                    }
                }

            }
            catch
            {

            }

            return result;
        }

        /// <summary>
        /// 获取电脑名称
        /// </summary>
        /// <returns></returns>
        public static string GetComputerName()
        {
            return Environment.GetEnvironmentVariable("ComputerName");
        }

        /// <summary>
        /// 获取CPU信息
        /// </summary>
        public static Dictionary<string, string> GetCPUInfo()
        {
            var result = new Dictionary<string, string>();
            try
            {

                ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_Processor");//Win32_Processor  CPU处理器
                foreach (ManagementObject mo in mos.Get())
                {
                    result["Name"] = mo["Name"].ToString();
                    result["ProcessorId"] = mo["ProcessorId"].ToString();
                    result["ThreadCount"] = mo["ThreadCount"].ToString();
                    result["L2CacheSize"] = mo["L2CacheSize"].ToString();
                    result["L3CacheSize"] = mo["L3CacheSize"].ToString();
                }
                mos.Dispose();

            }
            catch
            {

            }
            return result;
        }
    }
}
