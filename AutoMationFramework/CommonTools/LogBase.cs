using System.Collections.Generic;
using System.Windows.Controls;

namespace CommonTools
{
    /// <summary>日志信息显示委托函数</summary>
    /// <param name="logListBox"></param>
    /// <param name="strLog"></param>
    /// <param name="level">信息等级</param>
    public delegate void LogHandler(Control logListBox, string strLog, LogLevel level = LogLevel.Info);

    public enum LogLevel
    {
        /// <summary>信息</summary>
        Info,
        /// <summary>警告</summary>
        Warn,
        /// <summary>错误</summary>
        Error,
    }
    /// <summary>日志显示到列表框的基类</summary>
    public class LogBase
    {
        /// <summary>用来显示站位运行日志的列表框</summary>
        private List<Control> m_LogList = new List<Control>();

        /// <summary>日志信息显示事件</summary>
        public event LogHandler LogEvent;

        /// <summary>触发日志显示事件</summary>
        /// <param name="strLog"></param>
        /// <param name="level">显示的等级,便于颜色提示　</param>
        public void ShowLog(string strLog, LogLevel level = LogLevel.Info)
        {
            // ISSUE: reference to a compiler-generated field
            if (LogEvent == null || (uint)m_LogList.Count <= 0U)
                return;
            foreach (Control log in m_LogList)
            {
                // ISSUE: reference to a compiler-generated field
                LogEvent(log, strLog, level);
            }
        }

        /// <summary>设置日志要显示在哪个列表框上</summary>
        /// <param name="logListBox"></param>
        public void SetLogListBox(Control logListBox)
        {
            m_LogList.Add(logListBox);
        }
    }
}
