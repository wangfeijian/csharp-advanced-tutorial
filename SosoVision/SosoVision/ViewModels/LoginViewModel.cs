using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SosoVisionCommonTool.Authority;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SosoVision.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        private string _passwordValue;

        public string PasswordValue
        {
            get { return _passwordValue; }
            set { _passwordValue = value; RaisePropertyChanged(); }
        }

        private int _userSelect;

        public int UserSelect
        {
            get { return _userSelect; }
            set { _userSelect = value; RaisePropertyChanged(); }
        }

        public DelegateCommand LoginCommand { get; }
        public DelegateCommand<object> PasswordChangedCommand { get; }
        public LoginViewModel()
        {
            LoginCommand = new DelegateCommand(Login);
            PasswordChangedCommand = new DelegateCommand<object>(PasswordChange);
        }

        private void PasswordChange(object obj)
        {
            var temp = obj as PasswordBox;
            if (temp != null)
            {
                PasswordValue = temp.Password;
            }
        }

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(PasswordValue))
            {
                MessageBox.Show("请输入密码，再进行登陆！");
                return;
            }

            switch (UserSelect)
            {
                case 0:
                    AuthorityTool.ChangeOpMode();
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                    break;
                case 1:
                    if (!AuthorityTool.ChangeAdjustorMode(PasswordValue))
                    {
                        MessageBox.Show("密码错误！");
                        break;
                    }
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                    break;
                case 2:
                    if (!AuthorityTool.ChangeEngMode(PasswordValue))
                    {
                        MessageBox.Show("密码错误！");
                        break;
                    }
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                    break;
            }
        }

        public string Title { get; } = "选择用户";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
