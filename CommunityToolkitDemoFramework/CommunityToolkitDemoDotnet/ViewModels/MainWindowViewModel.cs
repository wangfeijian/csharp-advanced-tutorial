#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：DESKTOP-ARJBVM7
 * 公司名称：wangfeijian
 * 命名空间：CommunityToolkitDemoDotnet.ViewModels
 * 唯一标识：087a5e6f-992d-4501-b5bc-2d40c3c911c7
 * 文件名：MainWindowViewModel
 * 当前用户域：DESKTOP-ARJBVM7
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2023/2/14 13:40:01
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
using CommunityToolkitDemoDotnet.Services;
using CommunityToolkitDemoDotnet.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CommunityToolkitDemoDotnet.ViewModels
{
    public partial class MainWindowViewModel : ObservableValidator
    {
        private readonly IDemoService _demoService;

        public MainWindowViewModel(IDemoService demoService)
        {
            _demoService = demoService;
            WeakReferenceMessenger.Default.Register<string>(this, (sender, args) => { ReceiveData = args;});
            WeakReferenceMessenger.Default.Register<string, string>(this, TokenHelperService.TestToken,(sender, args) => { ReceiveTokenData = args;});
        }

        private string _info = "test";

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
                    if (TrySetProperty(ref _info, value, out _errors))
                    {
                        // 该属性变更通知其它属性一起变更
                        OnPropertyChanged(nameof(Detail));
                    }

                    OnPropertyChanged(nameof(Errors));
                }
            }
        }
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ShowOtherWindowCommand))]
        private bool _enableClick;

        [ObservableProperty]
        private string _receiveData;

        [ObservableProperty]
        private string _receiveTokenData;
        [ObservableProperty]
        private IReadOnlyCollection<ValidationResult> _errors;
        public string Detail => $"{_demoService.GetInfo()}_{Info}";

        [RelayCommand(CanExecute=nameof(EnableClick))]
        private async Task ShowOtherWindow()
        {
            Info = "Updating...";
            await Task.Delay(2000);
            Info = "Click Test";

            var otherWindow = App.Current.Services.GetRequiredService<OtherWindow> ();
            otherWindow.Show();
        }
    }
}