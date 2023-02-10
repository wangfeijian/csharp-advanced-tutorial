#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：DESKTOP-ARJBVM7
 * 公司名称：wangfeijian
 * 命名空间：CommunityToolkitDemoFramework.ViewModel
 * 唯一标识：5322d091-780d-4a3c-b46d-b996bd33a50d
 * 文件名：MainWindowViewModel
 * 当前用户域：DESKTOP-ARJBVM7
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2023/2/10 13:09:20
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkitDemoFramework.Services;
using CommunityToolkitDemoFramework.View;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CommunityToolkitDemoFramework.ViewModels
{
    public class MainWindowViewModel : ObservableValidator
    {
        private readonly IDemoService demoService;

        public MainWindowViewModel(IDemoService demoService)
        {
            this.demoService = demoService;
            ShowOtherWindowCommand = new AsyncRelayCommand(ShowOtherWindow, canExecute: () => EnableClick, AsyncRelayCommandOptions.None);
            WeakReferenceMessenger.Default.Register<string>(this, (sender, args) => { ReceiveData = args;});
        }

        private IReadOnlyCollection<ValidationResult> _oldErrors;
        public IReadOnlyCollection<ValidationResult> Errors => _oldErrors;

        private string _info = "test";

        // 要想实现textbox实现和绑定的值更新，需要设置绑定中的UpdateSourceTrigger=PropertyChanged
        [Required]
        [MinLength(4)]
        [MaxLength(10)]
        public string Info
        {
            get { return _info; }
            set
            {
                if (_info != value)
                {
                    
                    //bool b = SetProperty(ref _info, value, true);
                    if (TrySetProperty(ref _info, value, out _oldErrors))
                    {
                        // 该属性变更通知其它属性一起变更
                        OnPropertyChanged(nameof(Detail));
                    }
                    
                    OnPropertyChanged(nameof(Errors));
                }

            }
        }

        private bool _enableClick;
        public bool EnableClick
        {
            get { return _enableClick; }
            set
            {
                if (_enableClick != value)
                {
                    if (SetProperty(ref _enableClick, value))
                    {
                        // 命令是否可以使用
                        // 必须通知更新
                        ShowOtherWindowCommand.NotifyCanExecuteChanged();
                    }
                }
            }
        }

        private string _receiveData;

        public string ReceiveData
        {
            get { return _receiveData; }
            set
            {
                SetProperty(ref _receiveData, value);
            }
        }

        public string Detail => $"{demoService.GetInfo()}_{Info}";

        public AsyncRelayCommand ShowOtherWindowCommand { get; }

        private async Task ShowOtherWindow()
        {
            Info = "Updating...";
            await Task.Delay(2000);
            Info = "Click test";
            var window = App.ServiceProvider.GetRequiredService<OtherWindow>();
            window.Show();
        }

    }
}