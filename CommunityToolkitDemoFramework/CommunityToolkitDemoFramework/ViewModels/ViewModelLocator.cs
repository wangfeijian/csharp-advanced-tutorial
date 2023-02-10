#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：DESKTOP-ARJBVM7
 * 公司名称：wangfeijian
 * 命名空间：CommunityToolkitDemoFramework.ViewModel
 * 唯一标识：889de902-e329-4820-9971-ea84aaa81266
 * 文件名：ViewModelLocator
 * 当前用户域：DESKTOP-ARJBVM7
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2023/2/10 13:10:24
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

namespace CommunityToolkitDemoFramework.ViewModels
{
    // 第四步: 添加ViewModel文件夹，新建MainWindow的Model类和ViewModelLocator类
    // 第五步：将ViewModelLocator添加的App.xaml的资源文件中
    // 第六步：在window的xaml文件中添加datacontext的绑定
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel => App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
        public OtherWindowViewModel OtherWindowViewModel => App.ServiceProvider.GetRequiredService<OtherWindowViewModel>();
    }
}