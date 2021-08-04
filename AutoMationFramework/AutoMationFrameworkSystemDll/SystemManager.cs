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
using AutoMationFrameworkModel;
using AutoMationFrameworkViewModel;
using CommonTools.Tools;

namespace AutoMationFrameworkSystemDll
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
    public class SystemManager : SingletonPattern<SystemManager>
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

        //未实现
        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void AppendMonitor(string szPath, string szExt, double dbKeepDays, int bRestart);

        // c中的char *返回，在c#中需要通过IntPtr来接收
        //Marshal.PtrToStringAnsi(ptr1)通过这个方法来转成字符串
        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetString(int index);

        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetString(int index, string str);

        //未实现
        [DllImport("Dll\\SecurityLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint GetLastInputTime();

        #endregion

        #region 属性字段

        private FileSystemWatcher[] _fileWatcher = new FileSystemWatcher[4];

        private int _nScanTime = 20;

        private SystemMode _mode = SystemMode.NormalRunMode;

        private ConcurrentDictionary<string, double> _dictMonitorPath = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// 当前系统的运行模式
        /// </summary>
        public SystemMode Mode => _mode;

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
        

        #endregion

        #region 系统模式相关

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

        #endregion

      
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
            _dictMonitorPath.AddOrUpdate(strPath + "=" + strExt, dbDays, (key, value) => dbDays);
        }

        #region 系统寄存器读写方法

        /// <summary>
        /// 获取指定的位寄存器的状态
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <returns></returns>
        public bool GetRegBit(int nIndex)
        {
            return GetBit(nIndex) == '1';
        }

        /// <summary>
        /// 向指定的位寄存器写入状态
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="bBit">状态值</param>
        /// <param name="bNotify">是否通知Form_Auto自动界面</param>
        public void WriteRegBit(int nIndex, bool bBit, bool bNotify = true)
        {
            char value = bBit ? '1' : '0';
            SetBit(nIndex, value);

            // ISSUE: reference to a compiler-generated field
            if (!bNotify || BitChangedEvent == null)
                return;
            // ISSUE: reference to a compiler-generated field
            BitChangedEvent(nIndex, bBit);
        }

        /// <summary>
        /// 获取指定的整型寄存器的值
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <returns></returns>
        public int GetRegInt(int nIndex)
        {
            return GetInt(nIndex);
        }

        /// <summary>
        /// 向指定的整型寄存器写入值
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="nData">要写入的数值</param>
        /// <param name="bNotify">是否通知Form_Auto自动界面</param>
        public void WriteRegInt(int nIndex, int nData, bool bNotify = true)
        {
            SetInt(nIndex, nData);
            // ISSUE: reference to a compiler-generated field
            if (!bNotify || IntChangedEvent == null)
                return;
            // ISSUE: reference to a compiler-generated field
            IntChangedEvent(nIndex, nData);
        }

        /// <summary>
        /// 获取一个浮点型寄存器的值
        /// </summary>
        /// <param name="nIndex">寄存器的值</param>
        /// <returns></returns>
        public double GetRegDouble(int nIndex)
        {
            return GetDouble(nIndex);
        }

        /// <summary>
        /// 向一个浮点数寄存器写入值
        /// </summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="fData">要写入的值</param>
        /// <param name="bNotify">是否通知Form_Auto自动界面</param>
        public void WriteRegDouble(int nIndex, double fData, bool bNotify = true)
        {
            SetDouble(nIndex, fData);
            // ISSUE: reference to a compiler-generated field
            if (!bNotify || DoubleChangedEvent == null)
                return;
            // ISSUE: reference to a compiler-generated field
            DoubleChangedEvent(nIndex, fData);
        }

        /// <summary>获取一个字符串型寄存器的值</summary>
        /// <param name="nIndex">寄存器的值</param>
        /// <returns></returns>
        public string GetRegString(int nIndex)
        {
            IntPtr ptr1 =GetString(nIndex);
            return Marshal.PtrToStringAnsi(ptr1);
        }

        /// <summary>向一个字符创寄存器写入值</summary>
        /// <param name="nIndex">寄存器索引</param>
        /// <param name="sData">要写入的值</param>
        /// <param name="bNotify">是否通知Form_Auto自动界面</param>
        public void WriteRegString(int nIndex, string sData, bool bNotify = true)
        {
            SetString(nIndex, sData);
            // ISSUE: reference to a compiler-generated field
            if (!bNotify || StringChangedEvent == null)
                return;
            // ISSUE: reference to a compiler-generated field
            StringChangedEvent(nIndex, sData);
        }

        #endregion


        #region 系统参数读写方法

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
                string info = LocationServices.GetLang("ParamNotExist");
                string msg  = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                return 0.0;
            }

            return Convert.ToDouble(paramInfos.First().CurrentValue);
        }

        /// <summary>
        /// 获取参数整型数值
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
                string info = LocationServices.GetLang("ParamNotExist");
                string msg  = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }

            return Convert.ToInt32(paramInfos.First().CurrentValue);
        }

        /// <summary>
        /// 获取参数布尔数值
        /// </summary>
        /// <param name="szParam">值索引</param>
        /// <returns></returns>
        public bool GetParamBool(string szParam)
        {
            var systemParm = SimpleIoc.Default.GetInstance<SysParamControlViewModel>();
            var param = from item in systemParm.AllParameters.ParameterInfos
                where item.KeyValue == szParam
                select item;

            var paramInfos = param as ParamInfo[] ?? param.ToArray();

            if (paramInfos.Length == 0)
            {
                string info = LocationServices.GetLang("ParamNotExist");
                string msg  = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return paramInfos.First().CurrentValue =="1";
        }

        /// <summary>
        /// 设置参数整型数值
        /// </summary>
        /// <param name="szParam">值索引</param>
        /// <param name="fData">浮点值</param>
        /// <returns></returns>
        public void SetParamInt(string szParam, int fData)
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
                string info = LocationServices.GetLang("ParamNotExist");
                string msg  = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 设置参数布尔数值
        /// </summary>
        /// <param name="szParam">值索引</param>
        /// <param name="fData">浮点值</param>
        /// <returns></returns>
        public void SetParamBool(string szParam, bool fData)
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
                    string value = fData ? "1" : "0";
                    // ISSUE: reference to a compiler-generated field

                    if (paramInfo.CurrentValue != value)
                    {
                        // ISSUE: reference to a compiler-generated field
                        SystemParamChangedEvent?.Invoke(szParam, paramInfo.CurrentValue, value);
                    }
                    paramInfo.CurrentValue = value;
                }
            }
            else
            {
                string info = LocationServices.GetLang("ParamNotExist");
                string msg  = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 设置参数布尔数值
        /// </summary>
        /// <param name="szParam">值索引</param>
        /// <param name="fData">浮点值</param>
        /// <returns></returns>
        public void SetParamString(string szParam, string fData)
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

                    if (paramInfo.CurrentValue != fData)
                    {
                        // ISSUE: reference to a compiler-generated field
                        SystemParamChangedEvent?.Invoke(szParam, paramInfo.CurrentValue, fData);
                    }
                    paramInfo.CurrentValue = fData;
                }
            }
            else
            {
                string info = LocationServices.GetLang("ParamNotExist");
                string msg  = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                string info = LocationServices.GetLang("ParamNotExist");
                string msg  = string.Format(info, szParam);
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

            if (paramInfos.Length == 0)
            {
                string info = LocationServices.GetLang("ParamNotExist");
                string msg  = string.Format(info, szParam);
                MessageBox.Show(msg, LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }

            return paramInfos.First().CurrentValue;
        }

        #endregion

        #region 文件夹文件操作方法

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
        /// 获取自动测试报告路径
        /// </summary>
        /// <param name="strSubDir"></param>
        /// <returns></returns>
        public string GetReportPath(string strSubDir = "")
        {
            return GetOrCreateDir(GetParamString("ReportSavePath"), strSubDir);
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
            if (!Directory.Exists(strDir         + strSubDir))
                Directory.CreateDirectory(strDir + strSubDir);
            return strDir + strSubDir;
        }

        /// <summary>
        /// 复制文件夹中的所有文件夹与文件到另一个文件夹
        /// </summary>
        /// <param name="sourcePath">源文件夹</param>
        /// <param name="destPath">目标文件夹</param>
        public void CopyFiles(string sourcePath, string destPath)
        {
            if (!Directory.Exists(sourcePath))
                return;
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);
            new List<string>(Directory.GetFiles(sourcePath)).ForEach(c =>
            {
                string destFileName = Path.Combine(new[]
                {
                    destPath,
                    Path.GetFileName(c)
                });
                File.Copy(c, destFileName, true);
            });
            new List<string>(Directory.GetDirectories(sourcePath)).ForEach(c =>
            {
                string destPath1 = Path.Combine(new []
                {
                    destPath,
                    Path.GetFileName(c)
                });
                CopyFiles(c, destPath1);
            });
        }

        /// <summary>
        /// 移动文件夹中的所有文件夹与文件到另一个文件夹
        /// </summary>
        /// <param name="strDir">源文件夹</param>
        /// <param name="strDestDir">目的文件夹</param>
        public void MoveFiles(string strDir, string strDestDir)
        {
            if (!Directory.Exists(strDir))
                return;
            if (!Directory.Exists(strDestDir))
                Directory.CreateDirectory(strDestDir);
            new List<string>(Directory.GetFiles(strDir)).ForEach(c =>
            {
                string str = Path.Combine(new[]
                {
                    strDestDir,
                    Path.GetFileName(c)
                });
                if (File.Exists(str))
                    File.Delete(str);
                File.Move(c, str);
            });
            new List<string>(Directory.GetDirectories(strDir)).ForEach(c =>
            {
                string strDestDir1 = Path.Combine(new[]
                {
                    strDestDir,
                    Path.GetFileName(c)
                });
                MoveFiles(c, strDestDir1);
            });
        }

        /// <summary>
        /// 监控指定目录文件夹下文件的增加、改变、删除、重命名
        /// </summary>
        /// <param name="numberFile">数组索引</param>
        /// <param name="strMonitorFloder">要监控的指定目录文件夹</param>
        /// <param name="strFilter">指定文件类型,可以过滤掉其他类型的文件</param>
        /// <param name="fileCreate">文件创建事件</param>
        /// <param name="fileDelete">文件删除事件</param>
        /// <param name="fileRename">文件重命名事件</param>
        /// <param name="fileChange">文件内容改变事件</param>
        /// <returns></returns>
        public bool MonitorImgFile(int numberFile, string strMonitorFloder, string strFilter, OnCreate fileCreate, OnDelete fileDelete, OnRename fileRename, OnChange fileChange)
        {
            try
            {
                _fileWatcher[numberFile]                       =  new FileSystemWatcher();
                _fileWatcher[numberFile].Path                  =  strMonitorFloder;
                _fileWatcher[numberFile].Filter                =  strFilter;
                _fileWatcher[numberFile].Changed               += fileChange.Invoke;
                _fileWatcher[numberFile].Created               += fileCreate.Invoke;
                _fileWatcher[numberFile].Deleted               += fileDelete.Invoke;
                _fileWatcher[numberFile].Renamed               += fileRename.Invoke;
                _fileWatcher[numberFile].EnableRaisingEvents   =  true;
                _fileWatcher[numberFile].NotifyFilter          =  NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Security;
                _fileWatcher[numberFile].IncludeSubdirectories =  true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 停止对文件夹的监视
        /// </summary>
        /// <returns></returns>
        public bool StopMonitorImgFile(int numberFile)
        {
            try
            {
                if (_fileWatcher[numberFile] != null)
                    _fileWatcher[numberFile].EnableRaisingEvents = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 监视文件增加处理过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            string info = LocationServices.GetLang("FileNewEventLogic");
            string msg  = string.Format(info, e.ChangeType, e.FullPath, e.Name);
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 监视文件改变处理过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string info = LocationServices.GetLang("FileChangeEventLogic");
            string msg  = string.Format(info, e.ChangeType, e.FullPath, e.Name);
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 监视文件增删除处理过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            string info = LocationServices.GetLang("FileDeleteEventLogic");
            string msg  = string.Format(info, e.ChangeType, e.FullPath, e.Name);
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 监视文件重命名处理过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string info = LocationServices.GetLang("FileRenameEventLogic");
            string msg  = string.Format(info, e.ChangeType, e.FullPath, e.Name);
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 检测屏幕鼠标键盘是否无操作超过设定的时间
        /// </summary>
        /// <returns></returns>
        public bool CheckSystemIdle()
        {
            if (GetLastInputTime() <= GetParamInt("IdleTime") * 60 || Authority.IsOpMode())
                return false;
            Authority.ChangeOpMode();
            return true;
        }

        #endregion
    }
}
