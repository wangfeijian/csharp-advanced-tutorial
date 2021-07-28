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
using System.IO;
using System.Runtime.InteropServices;
using GalaSoft.MvvmLight.Ioc;
using System.Windows;

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
        /// <param name="Mode"></param>
        public void ChangeMode(SystemMode Mode)
        {
            if (_mode == Mode)
                return;

            //SingletonTemplate<WarningMgr>.GetInstance().Info(this._mode.ToString() + " change Mode to " + Mode.ToString());
            _mode = Mode;

            StateChangedEvent?.Invoke(Mode);
        }

        /// <summary>获取参数字符串值</summary>
        /// <param name="szParam">值索引</param>
        /// <returns></returns>
        public string GetParamString(string szParam)
        {
            //var systemParm = SimpleIoc.Default.GetInstance<AutoMationFrameWork.ViewModel.SysParamControlViewModel>
            //if (this.m_DicParam.ContainsKey(szParam))
            //    return this.m_DicParam[szParam].m_strValue;
            //string format = "系统配置文件中{0}参数不存在";
            //string caption = "参数配置错误";
            //if ((uint)SingletonTemplate<LanguageMgr>.GetInstance().LanguageID > 0U)
            //{
            //    format = "The {0} parameter does not exist in the system configuration file";
            //    caption = "Parameter configuration error";
            //}
            //int num = (int)MessageBox.Show(string.Format(format, (object)szParam), caption);
            return string.Empty;

        }
    }
}
