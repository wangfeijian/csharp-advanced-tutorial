#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.FileHelp
 * 唯一标识：90db91b3-7f05-4a9a-a27d-e78bcba506a1
 * 文件名：FileBaseHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/29/2023 1:20:36 PM
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

using System;
using System.IO;

namespace Soso.Common.FileHelp
{
    /// <summary>
    /// 文件基础帮助类
    /// </summary>
    /// <remarks>
    /// 文件操作中的一些常用方法进行了封装，避免重复写代码
    /// </remarks>
    public static class FileBaseHelp
    {
        /// <summary>
        /// 根据文件的绝对路径，创建目录
        /// </summary>
        /// <param name="fullName">文件绝对路径</param>
        /// <exception cref="ArgumentNullException" />
        public static void CreateDirForFullName(string fullName)
        {
            if (fullName == null)
            {
                throw new ArgumentNullException(nameof(fullName));
            }

            string dir = Path.GetDirectoryName(fullName)!;

            if (string.IsNullOrWhiteSpace(dir))
            {
                throw new ArgumentNullException($"{fullName} doesn't contain a directory!!");
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}