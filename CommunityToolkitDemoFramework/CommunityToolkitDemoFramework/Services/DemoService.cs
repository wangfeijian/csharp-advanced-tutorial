#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：DESKTOP-ARJBVM7
 * 公司名称：wangfeijian
 * 命名空间：CommunityToolkitDemoFramework.Services
 * 唯一标识：574f8c63-8244-48cb-87fd-fe2073e19e26
 * 文件名：DemoService
 * 当前用户域：DESKTOP-ARJBVM7
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2023/2/10 11:42:01
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

namespace CommunityToolkitDemoFramework.Services
{
    public class DemoService : IDemoService
    {
        public DemoService()
        {

        }

        public string  GetInfo()
        {
            return "Demo Service";
        }
    }
}