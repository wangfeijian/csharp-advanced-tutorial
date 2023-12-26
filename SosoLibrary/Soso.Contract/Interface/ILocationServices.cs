#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Contract.Interface
 * 唯一标识：f259ac0e-0542-4523-9d23-733ac6a9efae
 * 文件名：ILanguageMgt
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/26/2023 11:05:01 AM
 * 版本：V1.0.0
 * 描述：
 * 1、重新将文件移动到Interface文件夹下
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using System;
using System.Globalization;

namespace Soso.Contract.Interface
{
    public delegate void LocationChangedHandler(LocationChangedEventArgs eventArgs);
    public class LocationChangedEventArgs : EventArgs
    {
        private CultureInfo _lang;

        public CultureInfo Lang
        {
            get { return _lang; }
            set { _lang = value; }
        }

        public LocationChangedEventArgs(CultureInfo lang)
        {
            _lang = lang;
        }
    }

    public interface ILocationServices
    {
        event LocationChangedHandler LocationChangedEvent;
        CultureInfo CurrentCultrueInfo { get; }

        void ChangeLang(CultureInfo cultureInfo);

        string GetLang(string infoKey);
        string GetLang(string infoKey, string infoValue);
    }
}