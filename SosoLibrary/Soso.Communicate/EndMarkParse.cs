#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Communicate
 * 唯一标识：a3cb6442-c0d5-4692-a8ef-ff7302db3958
 * 文件名：EndMarkParse
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:35:58 AM
 * 版本：V1.0.0
 * 描述：
 * 1、重新新建文件，自动添加版本注释
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

namespace Soso.Communicate
{
    internal static class EndMarkParse
    {
        internal static string Parse(string input)
        {
            switch (input.ToUpper())
            {
                case "CRLF":
                    return "\r\n";
                case "CR":
                    return "\r";
                case "LF":
                    return "\n";
                case "ETX":
                    return "\u0003";
                case "NONE":
                    return "";
                default:
                    return input;
            }
        }
    }
}
