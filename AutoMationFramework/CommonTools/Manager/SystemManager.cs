/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-26                               *
*                                                                    *
*           ModifyTime:     2021-07-28                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    System manager class                     *
*********************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using GalaSoft.MvvmLight.Ioc;
using System.Windows;
using CommonTools.Model;
using CommonTools.Tools;
using CommonTools.ViewModel;

namespace CommonTools.Manager
{
    /// <summary>
    /// 系统运行模式设置，自动运行模式，空跑模式，标定模式，其它自定义模式，
    /// </summary>
    public enum SystemMode
    {
        /// <summary>
        /// 正常运行模式
        /// </summary>
        NormalRunMode,

        /// <summary>
        /// 空跑模式，是否带料由流程决定
        /// </summary>
        DryRunMode,

        /// <summary>
        /// 自动标定模式
        /// </summary>
        CalibRunMode,

        /// <summary>
        /// 模拟运行模式
        /// </summary>
        SimulateRunMode,

        /// <summary>
        /// 其它自定义模式
        /// </summary>
        OtherMode,
    }

    /// <summary>
    /// 系统管理类
    /// </summary>
    public class SystemManager:SingletonPattern<SystemManager>
    {
        #region Delegates

        /// <summary>
        /// 定义一个系统位寄存器的委托,响应自动界面函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="bBit"></param>
        public delegate void BitChangedHandler(int nIndex, bool bBit);

        /// <summary>
        /// 定义一个系统整型寄存器的委托,响应自动界面函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="nData"></param>
        public delegate void IntChangedHandler(int nIndex, int nData);

        /// <summary>
        /// 定义一个系统浮点型寄存器的委托,响应自动界面函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="fData"></param>
        public delegate void DoubleChangedHandler(int nIndex, double fData);

        /// <summary>
        /// 定义一个字符串型寄存器的委托,响应自动界面函数
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="sData"></param>
        public delegate void StringChangedHandler(int nIndex, string sData);

        /// <summary>
        /// 系统参数改变委托
        /// </summary>
        /// <param name="strParam">键值</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        public delegate void SystemParamChangedHandler(string strParam, object oldValue, object newValue);

        /// <summary>
        /// 文件监控目录下有新文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnCreate(object source, FileSystemEventArgs e);

        /// <summary>
        /// 文件监控目录下有删除文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnDelete(object source, FileSystemEventArgs e);

        /// <summary>
        /// 文件监控目录下有发生文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnChange(object source, FileSystemEventArgs e);

        /// <summary>
        /// 文件监控目录下文件名字有变化
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnRename(object source, RenamedEventArgs e);

        /// <summary>
        /// 状态变更委托
        /// </summary>
        /// <param name="Mode"></param>
        public delegate void StateChangedHandler(SystemMode Mode);

        #endregion

        #region Events

        /// <summary>
        /// 系统位寄存器变更事件
        /// </summary>
        public event BitChangedHandler BitChangedEvent;

        /// <summary>
        /// 系统整型寄存器变更事件
        /// </summary>
        public event IntChangedHandler IntChangedEvent;

        /// <summary>
        /// 系统浮点型寄存器变更事件
        /// </summary>
        public event DoubleChangedHandler DoubleChangedEvent;

        /// <summary>
        /// 字符串型寄存器变更事件
        /// </summary>
        public event StringChangedHandler StringChangedEvent;

        /// <summary>
        /// 系统参数改变事件
        /// </summary>
        public event SystemParamChangedHandler SystemParamChangedEvent;

        /// <summary>
        /// 定义状态变更事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;

            #endregion

        #region 库文件导入方法

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

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void AppendMonitor(string szPath, string szExt, double dbKeepDays, int bRestart);

        // c中的char *返回，在c#中需要通过IntPtr来接收
        //Marshal.PtrToStringAnsi(ptr1)通过这个方法来转成字符串
        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetString(int index);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetString(int index, string str);

        #endregion

        private FileSystemWatcher[] _fileWatcher = new FileSystemWatcher[4];

        private int _nScanTime = 20;

        private SystemMode _mode = SystemMode.NormalRunMode;

        private ConcurrentDictionary<string, double> _dictMonitorPath = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// 当前系统的运行模式
        /// </summary>
        public SystemMode Mode => _mode;

        /// <summary>
        /// 改变系统运行模式
        /// </summary>
        /// <param name="mode"></param>
        public void ChangeMode(SystemMode mode)
        {
            if (_mode == mode)
                return;

            SingletonPattern<RunInforManager>.GetInstance().Info(_mode + " change Mode to " + Mode);
            _mode = mode;

            StateChangedEvent?.Invoke(mode);
        }

        /// <summary>
        /// 判断当前是否属于空跑模式
        /// </summary>
        /// <returns></returns>
        public bool IsDryRunMode()
        {
            return _mode == SystemMode.DryRunMode;
        }

        /// <summary>
        /// 判断当前是否属于自动标定模式
        /// </summary>
        /// <returns></returns>
        public bool IsAutoCalibMode()
        {
            return _mode == SystemMode.CalibRunMode;
        }

        /// <summary>
        /// 判断当前是否属于模拟运行模式
        /// </summary>
        /// <returns></returns>
        public bool IsSimulateRunMode()
        {
            return _mode == SystemMode.SimulateRunMode;
        }

        /// <summary>
        /// 判断当前是否属于正常运行模式
        /// </summary>
        /// <returns></returns>
        public bool IsNormalRunMode()
        {
            return _mode == SystemMode.NormalRunMode;
        }

        /// <summary>
        /// 获取系统扫描周期时间
        /// </summary>
        public int ScanTime => _nScanTime;

        /// <summary>获取系统速度百分比</summary>
        public double SystemSpeed
        {
            get
            {
                return GetParamDouble(nameof(SystemSpeed));
            }
            set
            {
                SetParamDouble(nameof(SystemSpeed), value);
            }
        }

        private void InitParam()
        {
            _nScanTime = GetParamInt("ScanTime");
            if (_nScanTime == 0)
                _nScanTime = 20;
            AppendMonitor(GetImagePath(), "bmp", GetParamDouble("ImageKeepTime"), 1);
            AppendMonitor(GetImagePath(), "jpg", GetParamDouble("ImageKeepTime"), 0);
            AppendMonitor(GetImagePath(), "png", GetParamDouble("ImageKeepTime"), 0);
            AppendMonitor(GetDataPath(), "csv", GetParamDouble("DataKeepTime"), 0);
            AppendMonitor(GetLogPath(), "csv", GetParamDouble("LogKeepTime"), 0);
            foreach (KeyValuePair<string, double> keyValuePair in _dictMonitorPath)
            {
                string[] strArray = keyValuePair.Key.Split('=');
                if (strArray.Length == 2)
                    AppendMonitor(strArray[0], strArray[1], keyValuePair.Value, 0);
            }
        }

        /// <summary>
        /// 添加监控文件
        /// </summary>
        /// <param name="strPath">监控路径</param>
        /// <param name="strExt">监控文件后缀名</param>
        /// <param name="dbDays">监控天数</param>
        public void AppendMonitor(string strPath, string strExt, double dbDays)
        {
            AppendMonitor(strPath, strExt, dbDays, 0);
            this._dictMonitorPath.AddOrUpdate(strPath + "=" + strExt, dbDays, (key, value) => dbDays);
        }

        /// <summary>
        /// 获取参数浮点数值
        /// </summary>
        /// <param name="szParam">值索引</param>
        /// <returns></returns>
        public double GetParamDouble(string szParam)
        {
            var systemParm = SimpleIoc.Default.GetInstance<SysParamControlViewModel>();
            var param = from item in systemParm.AllParameters.ParameterInfos
                        where item.KeyValue == szParam
                        select item;

            var paramInfos = param as ParamInfo[] ?? param.ToArray();

            if (paramInfos.Length == 0)
            {
                string info = LocationServices.GetLang("LocationServices");
                string msg = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                return 0.0;
            }

            return Convert.ToDouble(paramInfos.First().CurrentValue);
        }

        /// <summary>
        /// 获取参数浮点数值
        /// </summary>
        /// <param name="szParam">值索引</param>
        /// <returns></returns>
        public int GetParamInt(string szParam)
        {
            var systemParm = SimpleIoc.Default.GetInstance<SysParamControlViewModel>();
            var param = from item in systemParm.AllParameters.ParameterInfos
                        where item.KeyValue == szParam
                        select item;

            var paramInfos = param as ParamInfo[] ?? param.ToArray();

            if (paramInfos.Length == 0)
            {
                string info = LocationServices.GetLang("LocationServices");
                string msg = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }

            return Convert.ToInt32(paramInfos.First().CurrentValue);
        }

        /// <summary>
        /// 设置参数浮点数值
        /// </summary>
        /// <param name="szParam">值索引</param>
        /// <param name="fData">浮点值</param>
        /// <returns></returns>
        public void SetParamDouble(string szParam, double fData)
        {
            var systemParm = SimpleIoc.Default.GetInstance<SysParamControlViewModel>();
            var param = from item in systemParm.AllParameters.ParameterInfos
                        where item.KeyValue == szParam
                        select item;
            var paramInfos = param as ParamInfo[] ?? param.ToArray();

            if (paramInfos.Length > 0)
            {

                foreach (var paramInfo in systemParm.AllParameters.ParameterInfos)
                {
                    // ISSUE: reference to a compiler-generated field
                    
                    if (paramInfo.CurrentValue != fData.ToString(CultureInfo.CurrentCulture))
                    {
                        // ISSUE: reference to a compiler-generated field
                        SystemParamChangedEvent?.Invoke(szParam, paramInfo.CurrentValue, fData);
                    }
                    paramInfo.CurrentValue = fData.ToString(CultureInfo.CurrentCulture);
                }
            }
            else
            {
                string info = LocationServices.GetLang("LocationServices");
                string msg = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 获取参数字符串值
        /// </summary>
        /// <param name="szParam">值索引</param>
        /// <returns></returns>
        public string GetParamString(string szParam)
        {
            var systemParm = SimpleIoc.Default.GetInstance<SysParamControlViewModel>();
            var param = from item in systemParm.AllParameters.ParameterInfos
                where item.KeyValue == szParam
                select item;

            var paramInfos = param as ParamInfo[] ?? param.ToArray();

            if (paramInfos.Length==0)
            {
                string info = LocationServices.GetLang("LocationServices");
                string msg = string.Format(info, szParam);
                MessageBox.Show(msg,LocationServices.GetLang("Tips"),MessageBoxButton.OK,MessageBoxImage.Error);
                return string.Empty;
            }

            return paramInfos.First().CurrentValue;
        }

        /// <summary>
        /// 得到保存Image路径下子文件夹的绝对路径
        /// </summary>
        /// <param name="strSubDir"></param>
        /// <returns></returns>
        public string GetImagePath(string strSubDir = "")
        {
            return GetOrCreateDir(GetParamString("ImageSavePath"), strSubDir);
        }

        /// <summary>
        /// 得到保存data路径下子文件夹的绝对路径
        /// </summary>
        /// <param name="strSubDir"></param>
        /// <returns></returns>
        public string GetDataPath(string strSubDir = "")
        {
            return GetOrCreateDir(GetParamString("DataSavePath"), strSubDir);
        }

        /// <summary>
        /// 得到保存log路径下子文件夹的绝对路径
        /// </summary>
        /// <param name="strSubDir"></param>
        /// <returns></returns>
        public string GetLogPath(string strSubDir = "")
        {
            return GetOrCreateDir(GetParamString("LogSavePath"), strSubDir);
        }

        private string GetOrCreateDir(string strDir, string strSubDir)
        {
            if (!Directory.Exists(strDir + strSubDir))
                Directory.CreateDirectory(strDir + strSubDir);
            return strDir + strSubDir;
        }
    }
}
