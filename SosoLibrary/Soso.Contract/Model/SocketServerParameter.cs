#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Contract.Model
 * 唯一标识：dde013d3-9691-47b9-8c8a-e49b3fd73bca
 * 文件名：SocketServerParameter
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:40:25 AM
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

namespace Soso.Contract.Model
{
    public class SocketServerParameter
    {
        public string Index { get; private set; }
        public string Name { get; private set; }
        public string IPAddress { get; private set; }
        public int Port { get; private set; }
        public bool Enable { get; private set; }
        public string BeginMark { get; private set; }
        public string EndMark { get; set; }
        public string CommandSpliter { get; private set; }
        public string ParameterSpliter { get; private set; }
    }
}
