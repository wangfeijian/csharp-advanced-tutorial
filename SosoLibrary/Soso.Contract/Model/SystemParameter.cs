#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Contract.Model
 * 唯一标识：652d6fc7-a4ac-4b64-9a7b-c59dcff2e634
 * 文件名：SystemParameter
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:40:57 AM
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

namespace Soso.Contract.Model
{
    public partial class SystemParameter : ObservableObject
    {
        public string Key { get; private set; }
        public string Value { get; set; }
        public string Description { get; private set; }
        public string EnglishDescription { get; private set; }
        public string MaxValue { get; private set; }
        public string MinValue { get; private set; }
        public string Units { get; private set; }
        public int AuthorityLevel { get; private set; }

        [NotifyPropertyChangedFor(nameof(ShowDescription))]
        [ObservableProperty]
        private int langType;
        public string ShowDescription
        {
            get
            {
                return LangType == 0 ? Description : EnglishDescription;
            }
        }
    }
}
