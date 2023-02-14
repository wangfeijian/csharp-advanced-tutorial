#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：DESKTOP-ARJBVM7
 * 公司名称：wangfeijian
 * 命名空间：CommunityToolkitDemoDotnet.ViewModels
 * 唯一标识：fb9bad89-dc75-4422-a9c5-938981a7cb47
 * 文件名：ViewModelLocator
 * 当前用户域：DESKTOP-ARJBVM7
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2023/2/14 13:34:53
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

using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkitDemoDotnet.ViewModels
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel => App.Current.Services.GetRequiredService<MainWindowViewModel>();
        public OtherWindowViewModel OtherWindowViewModel => App.Current.Services.GetRequiredService<OtherWindowViewModel>();
    }
}