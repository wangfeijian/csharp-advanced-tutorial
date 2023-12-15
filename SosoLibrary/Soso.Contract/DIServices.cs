#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Contract
 * 唯一标识：9ef22198-4dd0-41e7-aae0-452dc16791d9
 * 文件名：DIServices
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：8/25/2023 11:05:12 AM
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

using Autofac;
using System;

namespace Soso.Contract
{
    public class DIServices : SingletonInstance<DIServices>
    {
        public ContainerBuilder ContainerBuilder = new ContainerBuilder();

        public IContainer Container { get; private set; }

        public void AddPrivateCtorInstance<TService, TImplementation>() where TService : class
                                                                         where TImplementation : class, TService
        {
            ContainerBuilder.Register(o => Activator.CreateInstance(typeof(TImplementation), true)).As<TService>().SingleInstance();
        }

        public void ServicesBuilder()
        {
            Container = ContainerBuilder.Build();
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private DIServices()
        {

        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}