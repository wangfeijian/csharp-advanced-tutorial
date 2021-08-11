using AutoMationFrameworkDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ToolExtensions
{
    /// <summary>
    /// 手动操作类
    /// </summary>
    public static class ManualOperation
    {
        /// <summary>
        /// 通过窗体对象得到相应工站对象
        /// </summary>
        /// <param name="frm">窗体对象</param>
        /// <returns></returns>
        public static StationBase GetStation(Control control)
        {
            StationBase s = StationManager.GetInstance().GetStation(control);
            if (s == null)
            {
                string str = string.Format("手动调试时{0}找不到对应的站位类可用，配置不匹配", control.Tag);
                MessageBox.Show(str);
                return null;
            }
            return s;
        }
    }
}
