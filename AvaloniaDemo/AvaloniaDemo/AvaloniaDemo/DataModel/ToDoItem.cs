#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：AvaloniaDemo.DataModel
 * 唯一标识：a33c49fb-f8e9-4b1f-b56d-ef4bb0378828
 * 文件名：ToDoItem
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：1/3/2024 12:59:11 PM
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

namespace AvaloniaDemo.DataModel
{
    public class ToDoItem
    {
        public string Description { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
    }
}
