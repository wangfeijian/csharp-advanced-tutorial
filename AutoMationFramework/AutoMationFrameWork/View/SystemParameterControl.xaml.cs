using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
/*********************************************************************
*           Author:        wangfeijian                               *
*                                                                    *
*           CreatTime:     2021-06-30                                *
*                                                                    *
*           ModifyTime:    2021-07-27                                *
*                                                                    *
*           Email:         wangfeijianhao@163.com                    *
*                                                                    *
*           Description:   UserControl for system parameter back code*
*********************************************************************/

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// SystemParameterControl.xaml 的交互逻辑
    /// </summary>
    public partial class SystemParameterControl 
    {
        public SystemParameterControl()
        {
            InitializeComponent();
            //IoInputDatGrid.DataContext = SimpleIoc.Default.GetInstance<SysParamControlViewModel>();

        }
    }
}
