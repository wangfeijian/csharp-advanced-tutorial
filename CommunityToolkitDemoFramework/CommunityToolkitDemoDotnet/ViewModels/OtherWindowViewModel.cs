#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：DESKTOP-ARJBVM7
 * 公司名称：wangfeijian
 * 命名空间：CommunityToolkitDemoDotnet.ViewModels
 * 唯一标识：b71a8d82-046c-4493-bf8f-785ade4e4253
 * 文件名：OtherWindowViewModel
 * 当前用户域：DESKTOP-ARJBVM7
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2023/2/14 14:57:09
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
using System.Threading.Tasks;

namespace CommunityToolkitDemoDotnet.ViewModels
{
    public partial class OtherWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
        private string _message;

        [RelayCommand(CanExecute=nameof(CanSendMessage))]
        private async Task SendMessage()
        {
            await Task.Delay(1000);
            WeakReferenceMessenger.Default.Send(Message);
            WeakReferenceMessenger.Default.Send($"{Message}_token", TokenHelperService.TestToken);
        }

        private bool CanSendMessage => !string.IsNullOrEmpty(Message);
    }
}