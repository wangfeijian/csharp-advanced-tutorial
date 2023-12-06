#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：SosoMS.SingletonServices
 * 唯一标识：776cc141-264b-47d3-80d1-5ac8695e58f2
 * 文件名：ConfigServices
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：8/25/2023 8:50:31 AM
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
using Soso.Contract;
using System;
using System.Collections.Generic;

namespace Soso.Services
{
    public sealed class ConfigServices : SingletonInstance<ConfigServices>
    {
        private readonly ILogServices _logServices;
        private static readonly string _configPath = AppContext.BaseDirectory + $"Config\\system.json";
        private Dictionary<string, string> _configs;

        public string[] AllProjects
        {
            get
            {
                if (_configs.ContainsKey(nameof(AllProjects)))
                {
                    return _configs[nameof(AllProjects)].Split(',');
                }

                return new string[] { "Default" };
            }
        }

        public string CurrentProject
        {
            get
            {
                if (_configs.ContainsKey(nameof(CurrentProject)))
                {
                    return _configs[nameof(CurrentProject)];
                }

                return "Default";
            }
        }

        public string SystemParameterPath => AppContext.BaseDirectory + $"Config\\{CurrentProject}\\SystemParameters.xml";
        public string CommunicateParameterPath => AppContext.BaseDirectory + $"Config\\{CurrentProject}\\CommunicateParameters.xml";
        public string DataParameterPath => AppContext.BaseDirectory + $"Config\\{CurrentProject}\\DataParameters.xml";
        public string MotionParameterPath => AppContext.BaseDirectory + $"Config\\{CurrentProject}\\MotionParameters.xml";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ConfigServices()
        {
            _logServices = DIServices.Instance.Services.GetRequiredService<ILogServices>();
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}