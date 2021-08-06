/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-06                               *
*                                                                    *
*           ModifyTime:     2021-08-06                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Message window class                     *
*********************************************************************/
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
using System.Windows.Shapes;
using System.Windows.Threading;
using AutoMationFrameworkSystemDll;
using CommonTools.Tools;

namespace AutoMationFrameworkDll
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        private StationBase _station = null;
        private int _nTimeRemain = 20;
        private string[] _strArrBindIo = { "复位", "", "" };
        private bool _bBindIoPress = false;
        private DateTime _dtBindIoPressTime = DateTime.Now;
        private int _nPressIndex = -1;
        private DispatcherTimer _showTimer;

        /// <summary>
        /// 确认键绑定的IO
        /// </summary>
        public string[] BindIo
        {
            get
            {
                return _strArrBindIo;
            }
            set
            {
                _strArrBindIo = value;
            }
        }

        /// <summary>
        /// 通知模式
        /// </summary>
        public bool NotifyMode { get; set; }

        /// <summary>
        /// 绑定IO保持时间,单位秒
        /// </summary>
        public int IoKeepTimeS { get; set; }

        public MessageWindow(StationBase sb)
        {
            InitializeComponent();
            _showTimer = new DispatcherTimer();
            _showTimer.Tick += ShowTimerTick;
            _showTimer.Interval = new TimeSpan(0, 0, 0, 1);
            _showTimer.Start();
            _station = sb;
            _nTimeRemain = SingletonPattern<SystemManager>.GetInstance().GetParamInt("MessageTimeOut");
            IoKeepTimeS = 3;
            SingletonPattern<IoManager>.GetInstance().IoChangedEvent += OnIoChangedEvent;
        }

        private void OnIoChangedEvent(int nCard)
        {
            long num;

            if (_strArrBindIo == null)
                return;

            for (int i = 0; i < _strArrBindIo.Length; i++)
            {
                if (!string.IsNullOrEmpty(_strArrBindIo[i]) && (!_bBindIoPress || _nPressIndex == i) && SingletonPattern<IoManager>.GetInstance().DicIn.TryGetValue(_strArrBindIo[i], out num) && nCard == (num >> 8) - 1L)
                {
                    bool ioInState = SingletonPattern<IoManager>.GetInstance().GetIoInState((int)(num >> 8), (int)(num & byte.MaxValue));
                    if (!_bBindIoPress & ioInState)
                    {
                        _bBindIoPress = true;
                        _dtBindIoPressTime = DateTime.Now;
                        _nPressIndex = i;
                    }
                    if (_bBindIoPress && !ioInState && (DateTime.Now - _dtBindIoPressTime).TotalSeconds > IoKeepTimeS)
                    {
                        var i1 = i;
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            switch (i1)
                            {
                                case 0:
                                    if (ButtonYes.Visibility == Visibility.Hidden)
                                        break;
                                    ButtonYesClick();
                                    break;
                                case 1:
                                    if (ButtonNo.Visibility == Visibility.Hidden)
                                        break;
                                    ButtonNoClick();
                                    break;
                                case 2:
                                    if (ButtonCancel.Visibility == Visibility.Hidden)
                                        break;
                                    ButtonCancelClick();
                                    break;
                            }
                        });
                    }

                    if (!ioInState)
                        _bBindIoPress = false;
                }
            }
        }

        /// <summary>
        /// 显示超时对话框
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="title"></param>
        /// <param name="btns"></param>
        /// <returns></returns>
        public bool? MessageShow(string strText, string title, MessageBoxButton btns)
        {
            Title = title;
            InfoMessage.Text = strText;
            switch (btns)
            {
                case MessageBoxButton.OK:
                    ButtonCancel.Visibility = Visibility.Hidden;
                    ButtonCancel.IsEnabled = false;
                    ButtonNo.Visibility = Visibility.Hidden;
                    ButtonNo.IsEnabled = false;
                    ButtonNo.Focus();
                    break;
                case MessageBoxButton.YesNo:
                    ButtonCancel.Visibility = Visibility.Hidden;
                    ButtonCancel.IsEnabled = false;
                    ButtonYes.Focus();
                    break;
            }
            _showTimer.Start();
            return ShowDialog();
        }

        private void ButtonYesClick()
        {
            DialogResult = true;
            _station = null;
            _showTimer.Stop();
            Close();
        }

        private void ButtonNoClick()
        {
            DialogResult = null;
            _station = null;
            _showTimer.Stop();
            Close();
        }

        private void ButtonCancelClick()
        {
            DialogResult = false;
            _station = null;
            _showTimer.Stop();
            Close();
        }

        /// <summary>
        /// 设置YES按钮的文本显示
        /// </summary>
        /// <param name="strText"></param>
        public void SetYesText(string strText)
        {
            if (Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke((Action)delegate { ButtonYes.Content = strText; });
            else
                ButtonYes.Content = strText;
        }

        /// <summary>
        /// 设置NO按钮的文本显示
        /// </summary>
        /// <param name="strText"></param>
        public void SetNoText(string strText)
        {
            if (Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke((Action)delegate { ButtonNo.Content = strText; });
            else
                ButtonNo.Content = strText;
        }

        /// <summary>
        /// 设置Cancel按钮的文本显示
        /// </summary>
        /// <param name="strText"></param>
        public void SetCancelText(string strText)
        {
            if (Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke((Action)delegate { ButtonCancel.Content = strText; });
            else
                ButtonCancel.Content = strText;
        }

        private void ShowTimerTick(object sender, EventArgs e)
        {
            _showTimer.Stop();
            if (_station == null)
                return;
            try
            {
                _station.CheckContinue();
                _showTimer.Start();
            }
            catch (Exception)
            {
                DialogResult = false;
                _station = null;
                _showTimer.Stop();
                Close();
            }
        }

        private void MessageWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Activate();
            Topmost = true;
            if (LocationServices.GetLangType() == "en-us")
            {
                ButtonYes.Content = "Continte";
                ButtonNo.Content = "Abort";
                ButtonCancel.Content = "Ignore";
            }
            if (NotifyMode)
                return;
            ButtonNo.Visibility = Visibility.Hidden;
            OnModeChangedEvent();
            Authority.ModeChangedEvent += OnModeChangedEvent;
        }

        private void OnModeChangedEvent()
        {
            if (Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action)OnModeChangedEvent);
            }
            else
            {
                switch (Authority.GetUserMode())
                {
                    case UserMode.Engineer:
                        ButtonCancel.Visibility = Visibility.Visible;
                        break;
                    default:
                        ButtonCancel.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        private int GetMessageTimeOut()
        {
            int paramInt = SingletonPattern<SystemManager>.GetInstance().GetParamInt("MessageTimeOut");
            if (paramInt == 0)
                return 20;
            return paramInt;
        }


        private void MessageWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                DialogResult = true;
                ButtonYesClick();
            }
        }
    }
}
