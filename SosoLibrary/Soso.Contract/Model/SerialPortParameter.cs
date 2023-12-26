#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Contract.Model
 * 唯一标识：e2413d9e-9ff4-4f55-8c59-b46b097da94d
 * 文件名：SerialPortParameter
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:39:04 AM
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
    public class SerialPortParameter
    {
        public string Index { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int BaudRate { get; private set; }
        public int DataBit { get; private set; }
        public string ParityBit { get; private set; }
        public string StopBit { get; private set; }
        public string FlowControl { get; private set; }
        public int TimeOut { get; private set; }
        public int BufferSize { get; private set; }
        public string EndMark { get; set; }
    }
}
