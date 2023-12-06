#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Contract
 * 唯一标识：49f7cf50-1dc6-4e54-b771-68b6bc3e48e9
 * 文件名：ILogMgt
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/20/2023 3:37:11 PM
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

namespace Soso.Contract
{
    public interface ILogServices
    {
        void SaveLog(string message, LogLevel level = LogLevel.Info);

        void SaveErrorInfo(string message, LogLevel level = LogLevel.Error);
    }
}