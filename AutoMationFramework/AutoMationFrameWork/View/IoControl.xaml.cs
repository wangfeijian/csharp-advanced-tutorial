/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-06-29                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    UserControl for IO back code             *
*********************************************************************/

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoMationFrameworkDll;
using CommonTools.Tools;
using MotionIO;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// IoControl.xaml 的交互逻辑
    /// </summary>
    public partial class IoControl
    {
        private string[] _systemIn = { "1.1", "1.2", "1.3", "1.4", "1.5" };  //系统常用IO输入定义数组
        private string[] _systemOut = { "1.1", "1.2", "1.3", "1.4", "1.5" }; //系统常用IO输出定义数组

        private Button[] _btnsIn;   //IO输入点指示
        private Button[] _btnsOut;     //IO输出指示按钮
        private Button[] _btnsInSystem; //系统常用IO输入指示
        private Button[] _btnsOutSystem;   //系统常用IO输出指示

        private int _nCardIn = 0;  //
        private int _nCardOut = 0;
        private int _nIndexIn = 0;
        private int _nIndexOut = 0;
        private long _nDataIn = 0;  //当前IO输入缓冲
        private long _nDataOut = 0; //当前IO输出缓冲

        public IoControl()
        {
            InitializeComponent();
        }

        private void IoControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitIoButton(ref _btnsIn, IoInGrid);
            InitIoButton(ref _btnsOut, IoOutGird);
            InitIoButton(ref _btnsInSystem, SystemInGrid);
            InitIoButton(ref _btnsOutSystem, SystemOutGrid);
            UpdateIoInText(_nCardIn, _nIndexIn);
            UpdateIoOutText(_nCardOut, _nIndexOut);
            UpdateSystemIoText();
            UpdateIoText(_btnsInSystem, _systemIn);
            UpdateIoText(_btnsOutSystem, _systemOut, false);
        }

        private void InitIoButton(ref Button[] buttons, Grid grid)
        {
            int index = 0;
            buttons = new Button[grid.Children.Count];

            foreach (var child in grid.Children)
            {
                buttons[index++] = (Button)child;
                if (grid.Name == IoOutGird.Name)
                {
                    //btns_out[nIndex].Click += ManaulTool.Form_IO_Out_Click; //定义输出按钮点击事件
                }
            }
        }

        /// <summary>
        /// 显示IO输入点按钮名字
        /// </summary>
        /// <param name="nCardNo">即将跳转到的卡号索引</param>
        /// <param name="nIndex">IO点索引</param>
        private void UpdateIoInText(int nCardNo, int nIndex)
        {
            if (IoManager.GetInstance().CountCard > nCardNo) //要跳转到的卡号小于总IO卡数
            {
                MotionIO.IoControl Card = IoManager.GetInstance().ListCard.ElementAt(nCardNo);  //取出当前IO卡号对象
                for (int i = 0; i < _btnsIn.Length; ++i)
                {
                    if (i + nIndex < Card.StrArrayIn.Length)
                    {
                        string strIoName = Card.StrArrayIn[nIndex + i];
                        if (LocationServices.GetLangType() == "en-us")
                        {
                            strIoName = IoManager.GetInstance().GetIoInTranslate(strIoName);
                        }
                        _btnsIn[i].Content = $"{nCardNo + 1}.{i + nIndex + 1,2} {strIoName}";
                        _btnsIn[i].Visibility = Visibility.Visible;
                        _btnsIn[i].Foreground = Brushes.Gray;
                    }
                    else
                    {
                        _btnsIn[i].Visibility = Visibility.Hidden;
                    }
                }

                if (Card.StrCardName == "Delta")
                {
                    IoInTextBlock.Text =
                        $"{Card.StrCardName} DI: Card ID:{((IoControlDelta)Card).AddrDi.CardId},Node ID:{((IoControlDelta)Card).AddrDi.NodeId},Port ID:{((IoControlDelta)Card).AddrDi.PortId}";
                }
                else
                {
                    IoInTextBlock.Text = $"{Card.StrCardName} DI:  Card ID:{_nCardIn + 1},Port ID:{Card.CountIoIn}";
                }
            }
        }

        /// <summary>
        /// 显示IO输出点按钮名字
        /// </summary>
        /// <param name="nCardNo">即将跳转到的卡号索引</param>
        /// <param name="nIndex">IO点索引</param>
        private void UpdateIoOutText(int nCardNo, int nIndex)
        {
            if (IoManager.GetInstance().CountCard > nCardNo) //要跳转到的卡号小于总IO卡数
            {
                MotionIO.IoControl Card = IoManager.GetInstance().ListCard.ElementAt(nCardNo);  //取出当前IO卡号对象
                for (int i = 0; i < _btnsOut.Length; ++i)
                {
                    if (i + nIndex < Card.StrArrayOut.Length)
                    {
                        string strIoName = Card.StrArrayOut[nIndex + i];
                        if (LocationServices.GetLangType() == "en-us")
                        {
                            strIoName = IoManager.GetInstance().GetIoInTranslate(strIoName);
                        }
                        _btnsOut[i].Content = $"{nCardNo + 1}.{i + nIndex + 1,2} {strIoName}";
                        _btnsOut[i].Visibility = Visibility.Visible;
                        _btnsOut[i].Foreground = Brushes.Gray; ;
                    }
                    else
                    {
                        _btnsOut[i].Visibility = Visibility.Hidden;
                    }
                }

                if (Card.StrCardName == "Delta")
                {
                    IoOutTextBlock.Text =
                        $"{Card.StrCardName} DI: Card ID:{((IoControlDelta)Card).AddrDo.CardId},Node ID:{((IoControlDelta)Card).AddrDo.NodeId},Port ID:{((IoControlDelta)Card).AddrDo.PortId}";
                }
                else
                {
                    IoOutTextBlock.Text = $"{Card.StrCardName} DO:  Card ID:{_nCardOut + 1},Port ID:{Card.CountIoOut}";
                }
            }
        }

        /// <summary>
        /// 更新系统常用IO按钮名字
        /// </summary>
        private void UpdateSystemIoText()
        {
            int nCount = IoManager.GetInstance().ListIoSystemIn.Count;  //系统IO输入的个数 
            for (int i = 0; i < _btnsInSystem.Length; ++i)
            {
                if (i < nCount)
                {
                    _systemIn[i] = IoManager.GetInstance().ListIoSystemIn[i].StrName;
                }
                else
                {
                    _systemIn[i] = "";
                    _btnsInSystem[i].Visibility = Visibility.Hidden;
                }
            }

            nCount = IoManager.GetInstance().ListIoSystemOut.Count;  //系统IO输出的个数
            for (int i = 0; i < _btnsOutSystem.Length; ++i)
            {
                if (i < nCount)
                {
                    _systemOut[i] = IoManager.GetInstance().ListIoSystemOut[i].StrName;
                    //btns_out_system[i].Click += ManaulTool.Form_IO_Out_Click; //定义系统IO输出按钮点击事件
                }
                else
                {
                    _systemOut[i] = "";
                    _btnsOutSystem[i].Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// 显示系统IO输入输出点名字
        /// </summary>
        /// <param name="btn">按钮对象数组</param>
        /// <param name="strIO">要显示IO点名字索引数组</param>
        /// <param name="bIn">ture为输入,false为输出</param>
        private void UpdateIoText(Button[] btn, string[] strIO, bool bIn = true)
        {
            for (int i = 0; i < btn.Length; ++i)
            {
                if (i > strIO.Length - 1)
                {
                    btn[i].Visibility = Visibility.Collapsed;
                }
                else
                {
                    long num;
                    int nCardNo, nIndex;
                    if (bIn)
                    {
                        if (IoManager.GetInstance().DicIn.TryGetValue(strIO[i], out num))
                        {
                            nCardNo = (int)(num >> 8);
                            nIndex = (int)(num & 0xFF);
                        }
                        else
                        {
                            btn[i].Visibility = Visibility.Collapsed;

                            continue;
                        }

                    }
                    else
                    {
                        if (IoManager.GetInstance().DicOut.TryGetValue(strIO[i], out num))
                        {
                            nCardNo = (int)(num >> 8);
                            nIndex = (int)(num & 0xFF);
                        }
                        else
                        {
                            btn[i].Visibility = Visibility.Collapsed;

                            continue;
                        }
                    }

                    if (nCardNo - 1 < IoManager.GetInstance().CountCard)
                    {
                        string strIoName = strIO[i];
                        if (LocationServices.GetLangType() == "en-us")
                        {
                            if (bIn)
                            {
                                strIoName = IoManager.GetInstance().GetIoInTranslate(strIoName);
                            }
                            else
                            {
                                strIoName = IoManager.GetInstance().GetIoOutTranslate(strIoName);
                            }
                        }
                        btn[i].Content = $"{nCardNo}.{nIndex,2} {strIoName}";
                        btn[i].Visibility = Visibility.Visible;
                        btn[i].Foreground = Brushes.Gray;
                    }

                }
            }
        }

        /// <summary>
        /// Io输入下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextPageButtonInOnClick(object sender, RoutedEventArgs e)
        {
            if (_nCardIn < IoManager.GetInstance().CountCard)
            {
                int n = IoManager.GetInstance().ListCard.ElementAt(_nCardIn).CountIoIn;
                if (_nIndexIn + _btnsIn.Length > n - 1)
                {
                    if (_nCardIn == IoManager.GetInstance().CountCard - 1)
                        return;
                    ++_nCardIn;
                    _nIndexIn = 0;
                }
                else
                {
                    _nIndexIn += _btnsIn.Length;
                }

                UpdateIoInText(_nCardIn, _nIndexIn);
            }
        }

        /// <summary>
        /// Io输入上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousPageButtonInOnClick(object sender, RoutedEventArgs e)
        {
            if (_nCardIn >= 0)
            {
                int n = IoManager.GetInstance().ListCard.ElementAt(_nCardIn).CountIoIn;
                if (_nIndexIn - _btnsIn.Length > n - 1)
                {
                    _nIndexIn -= _btnsIn.Length;
                }
                else
                {
                    if (_nCardIn == 0)
                        return;
                    --_nCardIn;
                    _nIndexIn = 0;
                }
                UpdateIoInText(_nCardIn, _nIndexIn);
            }
        }

        /// <summary>
        /// Io输出下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextPageButtonOutOnClick(object sender, RoutedEventArgs e)
        {
            if (_nCardOut < IoManager.GetInstance().CountCard)
            {
                int n = IoManager.GetInstance().ListCard.ElementAt(_nCardOut).CountIoOut;
                if (_nIndexOut + _btnsOut.Length > n - 1)
                {
                    if (_nCardOut == IoManager.GetInstance().CountCard - 1)
                        return;
                    ++_nCardOut;
                    _nIndexOut = 0;
                }
                else
                {
                    _nIndexOut += _btnsOut.Length;
                }
                UpdateIoOutText(_nCardOut, _nIndexOut);
            }
        }

        /// <summary>
        /// Io输出上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousPageButtonOutOnClick(object sender, RoutedEventArgs e)
        {
            if (_nCardOut >= 0)
            {
                int n = IoManager.GetInstance().ListCard.ElementAt(_nCardOut).CountIoOut;
                if (_nIndexOut - _btnsOut.Length > n - 1)
                {
                    _nIndexOut -= _btnsOut.Length;
                }
                else
                {
                    if (_nCardOut == 0)
                        return;
                    --_nCardOut;
                    _nIndexOut = 0;
                }

                UpdateIoOutText(_nCardOut, _nIndexOut);
            }
        }
    }
}
