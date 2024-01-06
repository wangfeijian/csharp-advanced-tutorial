#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：AvaloniaDemo.ViewModels
 * 唯一标识：fb4fc78a-5168-4074-a127-fe8f50f33835
 * 文件名：AddItemViewModel
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：1/3/2024 1:20:45 PM
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

using AvaloniaDemo.DataModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace AvaloniaDemo.ViewModels
{
    public partial class AddItemViewModel : ViewModelBase
    {
        public IAsyncRelayCommand OkCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }

        [ObservableProperty]
        public string description;

        public AddItemViewModel()
        {
            OkCommand = new AsyncRelayCommand(OkCommandAction);
            CancelCommand = new AsyncRelayCommand(async () => { });
        }

        private Task<ToDoItem> OkCommandAction()
        {
            var isValidObservable = !string.IsNullOrWhiteSpace(description);
            return Task.FromResult(new ToDoItem() { Description = this.Description, IsChecked = isValidObservable });
        }
    }
}
