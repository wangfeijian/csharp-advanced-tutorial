#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Log
 * 唯一标识：ec6a6638-c4e8-46c5-a7f8-771ca4f1bf8f
 * 文件名：LogMgt
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/20/2023 3:42:56 PM
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

using log4net;
using Soso.Contract;
using Soso.Contract.Interface;
using System;
using System.IO;

namespace Soso.Log
{
    public sealed class LogServices : ILogServices
    {
        private LogServices()
        {
            var fileName = new FileInfo(AppContext.BaseDirectory + "log4net.config");
            log4net.Config.XmlConfigurator.Configure(fileName);

        }

        private void SaveToFile(ILog log, string message, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                    log.Info(message);
                    break;
                case LogLevel.Warn:
                    log.Warn(message);
                    break;
                case LogLevel.Error:
                    log.Error(message);
                    break;
            }
        }

        public void SaveLog(string message, LogLevel level = LogLevel.Info)
        {
            SaveToFile(LogManager.GetLogger("root"), message, level);
        }

        public void SaveErrorInfo(string message, LogLevel level = LogLevel.Error)
        {
            SaveToFile(LogManager.GetLogger("ErrorLog"), message, level);
        }
    }
}