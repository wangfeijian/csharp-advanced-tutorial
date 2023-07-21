#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：PartalTest
 * 唯一标识：0b35ea36-ebd2-4260-85fa-6db7834a4874
 * 文件名：PartialDemo
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/20/2023 2:52:33 PM
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

namespace PartalTest
{
    public partial class PartialDemo
    {
        public PartialDemo()
        {
            InitCtor();
        }

        partial void InitCtor();

    }

    public partial class PartialDemo : ICloneable
    {
        public object Clone()
        {
            Console.WriteLine("Clone!");
            return new object();
        }

        partial void InitCtor()
        {
            Console.WriteLine("这是一个组合！");
        }
    }

}