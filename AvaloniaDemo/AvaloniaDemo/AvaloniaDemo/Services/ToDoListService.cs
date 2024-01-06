#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：AvaloniaDemo.Services
 * 唯一标识：199ba89e-9205-40e5-8ad7-d87c3388aac6
 * 文件名：ToDoListService
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：1/3/2024 1:01:22 PM
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
using System.Collections.Generic;

namespace AvaloniaDemo.Services
{
    public class ToDoListService
    {
        public IEnumerable<ToDoItem> GetItems() => new[]
        {
            new ToDoItem{Description="Walk the dog" },
            new ToDoItem{Description="Bue some milk" },
            new ToDoItem{Description="Learn Avalonia", IsChecked=true }
        };
    }
}
