#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：DESKTOP-ARJBVM7
 * 公司名称：wangfeijian
 * 命名空间：CommunityToolkitDemoFramework.ViewModels
 * 唯一标识：f9c57b43-3ede-4b47-a66a-8809010691ff
 * 文件名：OtherWindowViewModel
 * 当前用户域：DESKTOP-ARJBVM7
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2023/2/10 16:15:04
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
using System.Threading.Tasks;

namespace CommunityToolkitDemoFramework.ViewModels
{
    public class OtherWindowViewModel : ObservableObject
    {
        public OtherWindowViewModel()
        {
            SendMessageCommand = new AsyncRelayCommand(SendMessage, canExecute: () => !string.IsNullOrWhiteSpace(Message), AsyncRelayCommandOptions.None);
        }

        public AsyncRelayCommand SendMessageCommand { get; }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    if(SetProperty(ref _message, value))
                    {
                        SendMessageCommand.NotifyCanExecuteChanged();
                    }
                }

            }
        }

        private async Task SendMessage()
        {
            await Task.Delay(1000);
            WeakReferenceMessenger.Default.Send(Message);
        }
    }
}