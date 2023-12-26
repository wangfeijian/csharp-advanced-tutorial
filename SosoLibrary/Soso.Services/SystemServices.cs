#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：SosoMS.SingletonServices
 * 唯一标识：79569319-c552-4fb9-8d72-8baafe96ee90
 * 文件名：SystemServices
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：8/25/2023 8:51:49 AM
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

using CommunityToolkit.Mvvm.ComponentModel;
using Soso.Contract;
using Soso.Contract.Model;
using System.Collections.ObjectModel;

namespace Soso.Services
{
    public sealed partial class SystemServices : SingletonInstance<SystemServices>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private SystemServices()
        {
            ConfigServices.Instance.InitSystemParameters();
            systemParameters = new ObservableCollection<SystemParameter>(ConfigServices.Instance.SystemParameters);
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        [ObservableProperty]
        private ObservableCollection<SystemParameter> systemParameters;

        /// <summary>
        /// 更改系统参数描述语言，目前只支持中文和英文
        /// </summary>
        /// <param name="langType">0-中文；1-英文；</param>
        public void ChangeLanguage(int langType)
        {
            foreach (var parameter in SystemParameters)
            {
                parameter.LangType = langType;
            }
        }
    }
}
