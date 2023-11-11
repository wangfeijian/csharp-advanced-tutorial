#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：UseSuperSocketCoreServer
 * 唯一标识：b720f461-b11b-4726-b273-da8395dcf808
 * 文件名：EndMarkFilter
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：11/11/2023 9:39:18 AM
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



using SuperSocket.ProtoBase;
using System.Text;

namespace UseSuperSocketCoreServer
{
    public class EndMarkFilter : TerminatorTextPipelineFilter
    {
        private static ReadOnlyMemory<byte> _endMark;
        public static void SetEndMark(string endMark)
        {
            _endMark = new ReadOnlyMemory<byte>(Encoding.Default.GetBytes(endMark));
        }
        public EndMarkFilter() : base(_endMark)
        {
        }
    }
}