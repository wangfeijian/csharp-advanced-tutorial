using System;
using GalaSoft.MvvmLight;
using System.Windows;
using System.Windows.Input;
using AutoBuildConfig.View;
using GalaSoft.MvvmLight.CommandWpf;

namespace AutoBuildConfig.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public ICommand ShowSys { get; set; }
        public ICommand ShowEx { get; set; }
        public ICommand ShowPoint { get; set; }
        public ICommand ShowParam { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            InitCommand();
        }

        private void InitCommand()
        {
            ShowSys = new RelayCommand(()=>ShowWindows(new SystemCfgWindow()));
            ShowEx = new RelayCommand(()=>ShowWindows(new ExtentionWindow()));
            ShowPoint = new RelayCommand(()=>ShowWindows(new PointWindow()));
            ShowParam = new RelayCommand(()=>ShowWindows(new ParameterWindow()));
        }

        private void ShowWindows(Window window)
        {
            window.Show();
        }
    }
}